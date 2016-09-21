using System;
using System.Collections.Generic;
using NewApp.BaseBlocks;
using NewApp.DataAccess;
using NewApp.Domain;

namespace NewApp.Blocks
{
    public class DataReaderBlock : ProducerBlock<string, Trial>
    {
        private readonly int _roundCount;
        public override string BlockName => "DataReader";

        private readonly IDataReader<Trial> _dataReader;

        public DataReaderBlock(DataFormat dataFormat, int roundCount)
        {
            _roundCount = roundCount;
            switch (dataFormat)
            {
                case DataFormat.Wire:
                    _dataReader = new WireData<Trial>();
                    break;
                case DataFormat.Protobuf:
                    _dataReader = new ProtobufData<Trial>();
                    break;
                default:
                    throw new ArgumentException($"DataFormat {dataFormat} is not supported");
            }
        }

        public override IEnumerable<Trial> DoWork(string item)
        {
            return _dataReader.GetStreamedData(item, _roundCount);
        }
    }
}