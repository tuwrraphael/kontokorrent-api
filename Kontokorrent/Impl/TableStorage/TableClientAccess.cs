using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kontokorrent.Impl.TableStorage
{
    public class TableClientAccess : ITableClientAccess
    {
        private TableClientCredentials options;

        public TableClientAccess(IOptions<TableClientCredentials> optionsAccessor)
        {
            options = optionsAccessor.Value;
        }

        public async Task<CloudTableClient> Get()
        {
            CloudStorageAccount storageAccount = new CloudStorageAccount(
                new StorageCredentials(options.StorageAccount, options.StorageKey), false);

            return storageAccount.CreateCloudTableClient();
        }
    }
}
