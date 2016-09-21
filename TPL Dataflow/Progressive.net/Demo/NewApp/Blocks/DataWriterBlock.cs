using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks.Dataflow;
using NewApp.BaseBlocks;
using NewApp.DataAccess;
using NewApp.Domain;

namespace NewApp.Blocks
{
    public class DataWriterBlock : CalculationSameBaseBlock<Trial>
    {
        private readonly string _fileName;
        private readonly int _roundCount;
        private Stream _stream;
        public override string BlockName => "DataReader";

        private readonly IDataWriter<Trial> _dataWriter;

        public DataWriterBlock(string fileName, DataFormat dataFormat, int roundCount, ExecutionDataflowBlockOptions options)
        {
            _fileName = fileName;
            _roundCount = roundCount;
            switch (dataFormat)
            {
                case DataFormat.Wire:
                    _dataWriter = new WireData<Trial>();
                    break;
                case DataFormat.Protobuf:
                    _dataWriter = new ProtobufData<Trial>();
                    break;
                default:
                    throw new ArgumentException($"DataFormat {dataFormat} is not supported");
            }

            ProcessingBlock = new TransformBlock<Trial, Trial>((Func<Trial, Trial>)ProcessItem, options);
        }

        public override Trial DoWork(Trial item)
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