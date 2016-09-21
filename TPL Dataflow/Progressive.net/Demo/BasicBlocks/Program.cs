using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace BasicBlocks
{
    class Program
    {
        static void Main(string[] args)
        {
            ActionBlock<int> action = new ActionBlock<int>((x) =>
            {
                throw new Exception("Test");
            });

            action.Post(1);

            try
            {
                action.Completion.Wait();
            }
            catch (AggregateException ex)
            {
                Console.WriteLine(ex.InnerException.Message);
            }

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

        private static void T()
        {
            var buffer = new CustomBufferBlock<int>();
            var transform1 = new CustomTransformBlock<int>();
            var transform2 = new CustomTransformBlock<int>();
            var action = new StatefulBlock();
            var nullTarget = DataflowBlock.NullTarget<int>();

            buffer.Then(transform1)
                .Then(transform2)
                .Then(action)
                .ThenTerminate(nullTarget);

            buffer.Post(2);
            buffer.Post(5);
            buffer.Post(6);

            buffer.Complete();
            action.Completion.Wait();

            Console.WriteLine(action.Average());
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

        private static void T2()
        {
            T();
            Console.ReadKey();

            var rnd = new Random();
            var executionDataflowBlockOptions = new ExecutionDataflowBlockOptions() { MaxDegreeOfParallelism = 10 };
            ActionBlock<int> action1 = new ActionBlock<int>(async (x) =>
            {
                var millisecondsDelay = rnd.Next(10, 2000);
                await Task.Delay(millisecondsDelay);
                Console.WriteLine("Block1: " + x);
            }, executionDataflowBlockOptions);

            ActionBlock<int> action2 = new ActionBlock<int>(async (x) =>
            {
                await Task.Delay(20);
                Console.WriteLine("Block2: " + x);
            }, executionDataflowBlockOptions);
            ActionBlock<int> action3 = new ActionBlock<int>(async (x) =>
            {
                await Task.Delay(200);
                Console.WriteLine("Block3: " + x);
            }, executionDataflowBlockOptions);

            var buffer1 = new BufferBlock<int>();
            buffer1.LinkTo(action1);

            foreach (var i in Enumerable.Range(1, 100))
            {
                buffer1.Post(i);
            }
            Console.ReadKey();



            var jb = new JoinBlock<int, string>();
            jb.Target1.Post(1);
            jb.Target2.Post("Name1");
            Console.WriteLine(jb.Receive());


            Console.WriteLine("Done");
            Console.ReadKey();

            //Join();
            //Console.ReadKey();

            //var bufferBlock = new BufferBlock<int>();
            //var broadCast = new BroadcastBlock<int>(x => x + 10);
            //var writeOnceBlock = new WriteOnceBlock<int>((a) => a + 100);

            //var writer = new ActionBlock<int>(x => Console.WriteLine(x));

            //var multiplyer = new TransformBlock<int, int>(x => x * x);
            //var multiplyerMany = new TransformManyBlock<int, int>(x => Enumerable.Range(0, x).ToList());

            //var joinBlock = new JoinBlock<int, int>();
            //var batchBlock = new BatchBlock<int>(1000);
            //var bacthedJoinBlock = new BatchedJoinBlock<int, int>(2);

            ////AsyncDownloading();
            //Console.ReadKey();
        }
    }
}
