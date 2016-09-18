using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks.Dataflow;
using NewApp.BaseBlocks;
using NewApp.DataAccess;
using NewApp.Domain;

namespace NewApp.Blocks
{
    public class DataWriterBlock : CalculationSameBaseBlock<Round>
    {
        private readonly string _fileName;
        private readonly int _roundCount;
        private Stream _stream;
        public override string BlockName => "DataReader";

        private readonly IDataWriter<Round> _dataWriter;

        public DataWriterBlock(string fileName, DataFormat dataFormat, int roundCount, ExecutionDataflowBlockOptions options)
        {
            _fileName = fileName;
            _roundCount = roundCount;
            switch (dataFormat)
            {
                case DataFormat.Wire:
                    _dataWriter = new WireData<Round>();
                    break;
                case DataFormat.Protobuf:
                    _dataWriter = new ProtobufData<Round>();
                    break;
                default:
                    throw new ArgumentException($"DataFormat {dataFormat} is not supported");
            }

            ProcessingBlock = new TransformBlock<Round, Round>((Func<Round, Round>)ProcessItem, options);
        }

        public override Round DoWork(Round item)
        {
            if (item.Id == 0)
            {
                _stream = new FileStream(_fileName + _dataWriter.Extention, FileMode.Create, FileAccess.Write, FileShare.None);
            }
             _dataWriter.WriteItem(_stream, item);

            if (item.Id == _roundCount)
            {
                _stream.Close();
                _stream.Dispose();
            }

            return item;
        } 
    }
}