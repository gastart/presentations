using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Common.Domain;
using Wire;

namespace Common.DataAccess
{
    public class WireData<T> : IDataReader<T>, IDataWriter<T>
    {
        private const string Extention = ".wir";

        public int WriteStreamedData(string source, IEnumerable<T> items, Action<string> logger = null)
        {
            int roundCount = 0;

            IList<Type> types = new List<Type>();
            types.Add(typeof(Round));
            types.Add(typeof(Loss));

            Serializer serializer = new Serializer();

            var sw = new Stopwatch();
            sw.Start();

            var ss = serializer.GetSerializerSession();

            //using (var stream = new FileStream(source, FileMode.Create, FileAccess.Write, FileShare.None))
            //{
            //        roundCount = items.Count();
            //        serializer.Serialize(items, stream, ss);
            //}

            using (var stream = new FileStream(source + Extention, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                foreach (var item in items)
                {
                    roundCount++;
                    serializer.Serialize(item, stream, ss);
                }
            }

            sw.Stop();
            logger?.Invoke($"Saved data to disk, it took {sw.Elapsed.TotalSeconds} seconds");

            return roundCount;
        }

        public IEnumerable<T> GetStreamedData(string source, int roundCount)
        {
            IList<Type> types = new List<Type>();
            types.Add(typeof(Round));
            types.Add(typeof(Loss));

            Serializer serializer = new Serializer();
            using (var stream = new FileStream(source + Extention, FileMode.Open))
            {
                for (var i = 0; i < roundCount; i++)
                {
                    var round = serializer.Deserialize<T>(stream);
                    yield return round;
                }

            }
        }

        public List<T> ReadAll(string fileName)
        {
            Serializer serializer = new Serializer();
            using (var stream = new FileStream(fileName + Extention, FileMode.Open))
            {
                var rounds = serializer.Deserialize<List<T>>(stream);
                return rounds;
            }
        }

        public void WriteAll(List<T> data, string fileName, Action<string> logger = null)
        {
            Serializer serializer = new Serializer();
            Stopwatch sw = new Stopwatch();
            sw.Start();
            using (var stream = new FileStream(fileName + Extention, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                serializer.Serialize(data, stream);
            }
            sw.Stop();
            logger?.Invoke($"Saved data to disk, it took {sw.Elapsed.TotalSeconds} seconds");
        }
    }
}