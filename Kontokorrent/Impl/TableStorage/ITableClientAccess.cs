using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;

namespace Kontokorrent.Impl.TableStorage
{
    public interface ITableClientAccess
    {
        Task<CloudTableClient> Get();
    }
}