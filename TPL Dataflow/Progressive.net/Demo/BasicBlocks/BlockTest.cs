using System.Threading.Tasks.Dataflow;
using NUnit.Framework;

namespace BasicBlocks
{
    public class BlockTest
    {
        [TestFixture]
        public class WhenPostingToTransformBlock
        {
            private TransformBlock<int, int> _block;
            int posted = 1;

            [SetUp]
            public void Setup()
            {
                _block = new TransformBlock<int, int>(x => x);
                _block.Post(posted);
            }

            [Test]
            public void CanRecieveItem()
            {
                var recieved = _block.Receive();
                Assert.That(recieved, Is.EqualTo(posted));
            }
        }
    }
}
