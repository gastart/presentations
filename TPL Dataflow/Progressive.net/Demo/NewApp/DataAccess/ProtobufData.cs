using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using ProtoBuf;

namespace NewApp.DataAccess
{
    public class ProtobufData<T> : IDataReader<T>, IDataWriter<T>
    {
        public string Extention => ".pb";

        public int WriteStreamedData(string source, IEnumerable<T> items, Action<string> logger = null)
        {
            int roundCount = 0;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            using (var stream = new FileStream(source + Extention, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                foreach (var item in items)
                {
                    Serializer.SerializeWithLengthPrefix(stream, item, PrefixStyle.Base128);
                    roundCount++;
                }
            }
            sw.Stop();
            logger?.Invoke($"Saved data to disk, it took {sw.Elapsed.TotalSeconds} seconds");

            return roundCount;
        }

        public IEnumerable<T> GetStreamedData(string source, int roundCount)
        {
            using (var stream = new FileStream(source + Extention, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                for (int i = 0; i < roundCount; i++)
                {
                    var round = Serializer.DeserializeWithLengthPrefix<T>(stream, PrefixStyle.Base128);
                    yield return round;
                }
            }
        }

        public List<T> ReadAll(string fileName)
        {
            using (var stream = new FileStream(fileName + Extention, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                {
                    var rounds = Serializer.Deserialize<List<T>>(stream);
                    return rounds;
                }
            }
        }

        public void WriteAll(List<T> data, string fileName, Action<string> logger = null)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            using (var stream = new FileStream(fileName + Extention, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                Serializer.Serialize(stream, data);
            }
            sw.Stop();
            logger?.Invoke($"Saved data to disk, it took {sw.Elapsed.TotalSeconds} seconds");
        }

        public void WriteItem(Stream stream, T item)
        {
            Serializer.SerializeWithLengthPrefix<T>(stream, item, PrefixStyle.Base128);
        }

       
    }
}