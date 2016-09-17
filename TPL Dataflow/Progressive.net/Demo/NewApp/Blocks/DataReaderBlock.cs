using System.Collections.Generic;
using Common.DataAccess;
using Common.Domain;
using NewApp.BaseBlocks;

namespace NewApp.Blocks
{
    public class DataReaderBlock : ProducerBlock<string, Round>
    {
        public override string BlockName => "DataReader";
        private readonly WireData<Round> _wireData;

        public DataReaderBlock() 
        {
            _wireData = new WireData<Round>();
        }

        public override IEnumerable<Round> DoWork(string item)
        {
            return _wireData.ReadAll(item);
        }
    }
}