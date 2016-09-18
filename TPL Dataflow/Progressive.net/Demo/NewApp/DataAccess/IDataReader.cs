using System.Collections.Generic;

namespace NewApp.DataAccess
{
    public interface IDataReader<T>
    {
        IEnumerable<T> GetStreamedData(string source, int roundCount);
        List<T> ReadAll(string fileName);
        
    }
}