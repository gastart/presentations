using System;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Linking
{
    class Program
    {
        static void Main(string[] args)
        {
            //Overflow();
            //Console.ReadKey();

            AsyncThrottling();
            Console.ReadKey();

            ActionBlock<int> writer = new ActionBlock<int>(x => Console.WriteLine(x));

            TransformBlock<int, int> multiplyer = new TransformBlock<int, int>(x => x * x);

            multiplyer.LinkTo(writer);

            foreach (var i in Enumerable.Range(0, 20))
            {
                multiplyer.Post(i);
            }
            multiplyer.Complete();
        }

        private static async void AsyncThrottling()
        {

            var consumer = new ActionBlock<int>(async x =>
                {
                    await Task.Delay(200);
                    Console.WriteLine(x);
                   
                }, new ExecutionDataflowBlockOptions { BoundedCapacity = 1 }
            );

            var producer = new ActionBlock<int>(async x =>
            {
                foreach (var i in Enumerable.Range(0, 100000000))
                {
                    await consumer.SendAsync(i);
                }
            });

            producer.Post(0);
            producer.Complete();

            await producer.Completion;

        }

        private static async void Overflow()
        {

            var consumer = new ActionBlock<int>(async x =>
            {
                await Task.Delay(2000);
                Console.WriteLine(x);

            });

            var producer = new ActionBlock<int>(x =>
            {
                foreach (var i in Enumerable.Range(0,100000000))
                {
                    consumer.Post(i);
                }

                Console.WriteLine("Sent all");

            });

            producer.Post(1);
            producer.Complete();

            await producer.Completion;

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
    }
}
