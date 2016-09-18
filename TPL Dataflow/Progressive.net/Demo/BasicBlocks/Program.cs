using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks.Dataflow;

namespace BasicBlocks
{
    class Program
    {
        static void Main(string[] args)
        {
            Join();
            Console.ReadKey();

            var bufferBlock = new BufferBlock<int>();
            var broadCast = new BroadcastBlock<int>(x => x + 10);
            var writeOnceBlock = new WriteOnceBlock<int>((a) => a + 100);

            var writer = new ActionBlock<int>(x => Console.WriteLine(x));

            var multiplyer = new TransformBlock<int, int>(x => x * x);
            var multiplyerMany = new TransformManyBlock<int, int>(x => Enumerable.Range(0, x).ToList());

            var joinBlock = new JoinBlock<int, int>();
            var batchBlock = new BatchBlock<int>(1000);
            var bacthedJoinBlock = new BatchedJoinBlock<int, int>(2);

            //AsyncDownloading();
            Console.ReadKey();

        }

        private static void Join()
        {
            var jb = new JoinBlock<int, string>(new GroupingDataflowBlockOptions() {Greedy = true});
            jb.Target1.Post(2);
            jb.Target1.Post(3);
            jb.Target2.Post("hello");
            jb.Target2.Post("Hmm");

            
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
