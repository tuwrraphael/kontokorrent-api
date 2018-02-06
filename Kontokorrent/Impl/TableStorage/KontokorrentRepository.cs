using Kontokorrent.Models;
using Kontokorrent.Services;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kontokorrent.Impl.TableStorage
{
    public class KontokorrentRepository : IKontokorrentRepository
    {
        private readonly ITableClientAccess tableClientAccess;
        private const string Table = "Kontokorrent";

        public KontokorrentRepository(ITableClientAccess tableClientAccess)
        {
            this.tableClientAccess = tableClientAccess;
        }

        public async Task<string> GetIdAsync(string secret)
        {
            var client = await tableClientAccess.Get();
            var tableRef = client.GetTableReference(Table);
            await tableRef.CreateIfNotExistsAsync();
            //tableRef.CreateQuery<string>();
            return null;
        }

        public Task CreateAsync(NeuerKontokorrent kontokorrent)
        {
            throw new NotImplementedException();
        }

        public Task<PersonenStatus[]> Get(string id)
        {
            throw new NotImplementedException();
        }
    }
}
