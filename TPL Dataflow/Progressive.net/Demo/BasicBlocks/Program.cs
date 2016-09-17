using System;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace BasicBlocks
{
    class Program
    {
        static void Main(string[] args)
        {
            var bufferBlock = new BufferBlock<int>();
            var broadCast = new BroadcastBlock<int>(x => x + 10);
            var writeOnceBlock = new WriteOnceBlock<int>((a) => a + 100);

            var writer = new ActionBlock<int>(x => Console.WriteLine(x));

            var multiplyer = new TransformBlock<int, int>(x => x * x);
            var multiplyerMany = new TransformManyBlock<int, int>(x => Enumerable.Range(0, x).ToList());

            var joinBlock = new JoinBlock<int, int>();
            var batchBlock = new BatchBlock<int>(1000);
            var bacthedJoinBlock = new BatchedJoinBlock<int, int>(2);

            JoinAndCalculatePairs();
            //AsyncDownloading();
            Console.ReadKey();

        }

        private static void JoinAndCalculatePairs()
        {
            var bufferBlock = new BufferBlock<int>();
            var bufferBlock2 = new BufferBlock<int>();

            var joinBlock = new JoinBlock<int, int>(new GroupingDataflowBlockOptions { Greedy = false }); //new GroupingDataflowBlockOptions { Greedy = false }
            var transform = new TransformBlock<Tuple<int, int>, string>(x => $"{x.Item1} + {x.Item2} = {x.Item1 + x.Item2}");

            var writer = new ActionBlock<string>(x => Console.WriteLine(x));

            bufferBlock.LinkTo(joinBlock.Target1);
            bufferBlock2.LinkTo(joinBlock.Target2);

            joinBlock.LinkTo(transform);
            transform.LinkTo(writer);

            foreach (var i in Enumerable.Range(0, 10))
            {
                    bufferBlock.Post(i);
            }

            foreach (var i in Enumerable.Range(10, 20))
            {
                    bufferBlock2.Post(i);
            }

            bufferBlock.Complete();
            bufferBlock2.Complete();
            bufferBlock.Completion.Wait();
            bufferBlock2.Completion.Wait();

            Console.WriteLine("Done");
        }

        private static void AsyncDownloading()
        {
            var downloadAndPrintBlock = new ActionBlock<string>(async url =>
            {
                var client = new WebClient();
                string content = await client.DownloadStringTaskAsync(url);
                //Console.WriteLine(content);
                Console.WriteLine("url done");
            });

            downloadAndPrintBlock.Post("http://www.bbc.co.uk");
            downloadAndPrintBlock.Post("http://www.google.com");
            downloadAndPrintBlock.Post("http://www.develop.com");
            Console.WriteLine("Done posting");
        }
    }
}
