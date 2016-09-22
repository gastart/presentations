using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using NewApp.Blocks;
using NewApp.DataAccess;
using NewApp.Domain;

namespace NewApp
{
    class Program
    {
        static void Main(string[] args)
        {
            string outFile = @"c:\temp\losses_large_streamed_out";

            var executionDataflowBlockOptionsSingle = new ExecutionDataflowBlockOptions {MaxDegreeOfParallelism = 1};
            var executionDataflowBlockOptions = new ExecutionDataflowBlockOptions {MaxDegreeOfParallelism = 10};

            var roundCount = 1000000;

            var dataReader = new DataReaderBlock(DataFormat.Protobuf, roundCount);
            var dataWriter = new DataWriterBlock(outFile, DataFormat.Protobuf, roundCount, executionDataflowBlockOptionsSingle);
            var limiter = new LimitBlock(300000, executionDataflowBlockOptions);
            var scaler = new ScaleBlock(0.9m, executionDataflowBlockOptions);
            var risk = new RiskMeasuresBlock(executionDataflowBlockOptionsSingle);
            var nullTarget = DataflowBlock.NullTarget<Trial>();

            dataReader
                .Then(limiter)
                .Then(scaler)
                .Then(risk)
                .Then(dataWriter)
                .Then(nullTarget);


            dataReader.InputBlock.Post(@"c:\temp\losses_large_streamed_in1");

            var calculationStartDate = DateTime.UtcNow;
            dataReader.InputBlock.Complete();

            while (!dataWriter.GetOutput().Completion.IsCompleted)
            {
                var totalTimeElapsed = Math.Round((DateTime.UtcNow - calculationStartDate).TotalSeconds, 2);
                var calculatedTrials = risk.GetProcessedItemsCount();
                var percentComplete = (float)Math.Round(calculatedTrials / (float)roundCount * 100, 2);

                Console.WriteLine($"Elapsed: {totalTimeElapsed}s, {calculatedTrials}/{roundCount} ({percentComplete}%)");

                Task[] tasksToWaitFor = { Task.Delay(1000), dataWriter.GetOutput().Completion };
                Task.WaitAny(tasksToWaitFor);
            }
            Console.WriteLine("");
            var res = risk.GetCalculationResult();
            Console.WriteLine(res.ToString());

            Console.ReadKey();

        }

        private static void GetAndSendTrials(JoinLossesBlock joiner, int roundCount)
        {
            string inFile = @"c:\temp\losses_large_streamed_in1";
            string inFile2 = @"c:\temp\losses_large_streamed_in2";

            var dataReader = new DataReaderBlock(DataFormat.Protobuf, roundCount);
            dataReader.InputBlock.Post(@"c:\temp\losses_large_streamed_in1");

            var dataReader2 = new DataReaderBlock(DataFormat.Protobuf, roundCount);
            dataReader2.InputBlock.Post(@"c:\temp\losses_large_streamed_in2");

            IDataReader<Trial> reader = new ProtobufData<Trial>();
            var source1 = reader.GetStreamedData(inFile, roundCount).GetEnumerator();
            var source2 = reader.GetStreamedData(inFile2, roundCount).GetEnumerator();

            foreach (var i in Enumerable.Range(0, roundCount))
            {
                source1.MoveNext();
                source2.MoveNext();

                joiner.Post1(source1.Current);
                joiner.Post2(source2.Current);
            }
        }
    }
}
