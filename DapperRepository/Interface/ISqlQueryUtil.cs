using System.Collections.Generic;

namespace DapperRepository.Interface
{
    public interface ISqlQueryUtil<T>
    {
        string keyfield { get; }
        string Insert();
        string Update(string idField = null, IList<string> columns = null);
        string Delete(string idField = null);
        string SelectById(string idField = null, IDictionary<string, string> colums = null);
        string SelectAll(IDictionary<string, string> columns = null);
    }
}
