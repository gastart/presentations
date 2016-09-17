using System;
using System.Collections.Generic;
using System.Threading;

namespace NewApp.BaseBlocks
{
    public abstract class BaseBlock
    {
        private static int _blockCounter;

        public static void ResetBlockCounter()
        {
            Interlocked.Exchange(ref _blockCounter, 0);
        }
        protected BaseBlock()
        {
            _nextBlockList = new List<BaseBlock>();
            Interlocked.Increment(ref _blockCounter);
            Id = _blockCounter.ToString("D5");
        }

        public string Id { get; }

        public string Context { get; }
        private readonly IList<BaseBlock> _nextBlockList;

        public IEnumerable<BaseBlock> GetNextBlocks()
        {
            return _nextBlockList;
        }

        public void AddNextBlock(BaseBlock block)
        {
            if (block == this)
            {
                throw new Exception("Attempted to connect a block to itself!");
            }
            _nextBlockList.Add(block);
        }

        public abstract int InputCount();
        public abstract int OutputCount();

        public abstract string BlockName { get; }
        public string BlockIdentifier => BlockName + $"[{Id}]";
    }
}