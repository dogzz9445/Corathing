using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Corathing.Contracts.Services;

namespace Corathing.Organizer.Services;

public class StorageService : IStorageService
{
    public void Delete(string filename)
    {
        throw new NotImplementedException();
    }

    public Task<StorageHandleArgs<T>> ReadAsync<T>(string filename)
    {
        throw new NotImplementedException();
    }

    public Task<StorageHandleArgs<T>> SaveAsync<T>(string filename, T content)
    {
        throw new NotImplementedException();
    }
}
