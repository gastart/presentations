using System;
using NewApp.Blocks;

namespace NewApp
{
    class Program
    {
        static void Main(string[] args)
        {
            string file1 = @"c:\temp\losses_small_better";

            var dataReader = new DataReaderBlock();
            var limiter = new LimitBlock();
            var scaler = new ScaleBlock(0.9m);
            var risk = new RiskMeasuresBlock();

            dataReader
                .Then(limiter)
                .Then(scaler)
                .Then(risk);

            dataReader.InputBlock.Post(file1);

            dataReader.InputBlock.Complete();
            dataReader.InputBlock.Completion.Wait();

            var res = risk.GetCalculationResult();

            Console.ReadKey();

        }
    }
}
