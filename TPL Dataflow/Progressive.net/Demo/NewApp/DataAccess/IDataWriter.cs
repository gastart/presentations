using System;
using System.Collections.Generic;
using System.IO;
using NewApp.Domain;

namespace NewApp.DataAccess
{
    public interface IDataWriter<T>
    {
        int WriteStreamedData(string source, IEnumerable<T> items, Action<string> logger = null);
        void WriteAll(List<T> data, string fileName, Action<string> logger = null);
        void WriteItem(Stream s, T item);
        string Extention { get; }
    }
}