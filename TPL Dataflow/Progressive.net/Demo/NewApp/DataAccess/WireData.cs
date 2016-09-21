using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using NewApp.Domain;
using Wire;

namespace NewApp.DataAccess
{
    public class WireData<T> : IDataReader<T>, IDataWriter<T>
    {
        public string Extention => ".wir";
        private readonly Serializer _serializer;

        public WireData()
        {
            _serializer = new Serializer();

        }
        public int WriteStreamedData(string source, IEnumerable<T> items, Action<string> logger = null)
        {
            int roundCount = 0;

            IList<Type> types = new List<Type>();
            types.Add(typeof(Trial));
            types.Add(typeof(Loss));

            var sw = new Stopwatch();
            sw.Start();

            var ss = _serializer.GetSerializerSession();

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
                    _serializer.Serialize(item, stream, ss);
                }
            }

            sw.Stop();
            logger?.Invoke($"Saved data to disk, it took {sw.Elapsed.TotalSeconds} seconds");

            return roundCount;
        }

        public IEnumerable<T> GetStreamedData(string source, int roundCount)
        {
            IList<Type> types = new List<Type>();
            types.Add(typeof(Trial));
            types.Add(typeof(Loss));

            using (var stream = new FileStream(source + Extention, FileMode.Open))
            {
                for (var i = 0; i < roundCount; i++)
                {
                    var round = _serializer.Deserialize<T>(stream);
                    yield return round;
                }
            }
        }

        public List<T> ReadAll(string fileName)
        {
            using (var stream = new FileStream(fileName + Extention, FileMode.Open))
            {
                var rounds = _serializer.Deserialize<List<T>>(stream);
                return rounds;
            }
        }

        public void WriteAll(List<T> data, string fileName, Action<string> logger = null)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            using (var stream = new FileStream(fileName + Extention, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                _serializer.Serialize(data, stream);
            }
            sw.Stop();
            logger?.Invoke($"Saved data to disk, it took {sw.Elapsed.TotalSeconds} seconds");
        }

        public void WriteItem(Stream stream, T item)
        {
            _serializer.Serialize(item, stream);
        }
    }
}