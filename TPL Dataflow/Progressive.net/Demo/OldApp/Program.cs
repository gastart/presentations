using System;
using System.Collections.Generic;
using Common.DataAccess;
using Common.Domain;

namespace OldApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var wireData = new WireData<Round>();

            string file1 = @"c:\temp\losses_small_better";
            string file2 = @"c:\temp\losses_small_better";

            var joiner = new LossJoiner();
            var limiter = new LossLimiter();

            List<Round> roundsFromSource1 = wireData.ReadAll(file1);
            joiner.AddData(roundsFromSource1);

            List<Round> roundsFromSource2 = wireData.ReadAll(file2);
            joiner.AddData(roundsFromSource2);

            var allRounds = joiner.GetRounds();

            var limitedRounds = limiter.Limit(allRounds, 30000);

            var average = RiskMeasures.Average(limitedRounds);
            var maxLoss = RiskMeasures.MaxLoss(limitedRounds);
            var tvar10 = RiskMeasures.TailValueAtRisk(limitedRounds, 0.9);


            Console.WriteLine($"Average: {average}");
            Console.WriteLine($"maxLoss: {maxLoss}");
            Console.WriteLine($"tvar10: {tvar10}");

            Console.ReadKey();

        }
    }
}
