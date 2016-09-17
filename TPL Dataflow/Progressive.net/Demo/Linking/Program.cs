using System;
using System.Linq;
using System.Threading.Tasks.Dataflow;

namespace Linking
{
    class Program
    {
        static void Main(string[] args)
        {
            ActionBlock<int> writer = new ActionBlock<int>(x => Console.WriteLine(x));

            TransformBlock<int, int> multiplyer = new TransformBlock<int, int>(x => x * x);

            multiplyer.LinkTo(writer);

            foreach (var i in Enumerable.Range(0,20))
            {
                multiplyer.Post(i);
            }
            multiplyer.Complete();

            Console.ReadKey();

        }
    }
}
