using System;
using System.Collections.Generic;

namespace Common.DataAccess
{
    public interface IDataWriter<T>
    {
        int WriteStreamedData(string source, IEnumerable<T> items, Action<string> logger = null);
        void WriteAll(List<T> data, string fileName, Action<string> logger = null);
    }
}