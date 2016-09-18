using System;
using System.Collections.Generic;
using NewApp.BaseBlocks;
using NewApp.DataAccess;
using NewApp.Domain;

namespace NewApp.Blocks
{
    public class DataReaderBlock : ProducerBlock<string, Round>
    {
        private readonly int _roundCount;
        public override string BlockName => "DataReader";

        private readonly IDataReader<Round> _dataReader;

        public DataReaderBlock(DataFormat dataFormat, int roundCount)
        {
            _roundCount = roundCount;
            switch (dataFormat)
            {
                case DataFormat.Wire:
                    _dataReader = new WireData<Round>();
                    break;
                case DataFormat.Protobuf:
                    _dataReader = new ProtobufData<Round>();
                    break;
                default:
                    throw new ArgumentException($"DataFormat {dataFormat} is not supported");
            }
        }

        public override IEnumerable<Round> DoWork(string item)
        {
            return _dataReader.GetStreamedData(item, _roundCount);
        }
    }
}