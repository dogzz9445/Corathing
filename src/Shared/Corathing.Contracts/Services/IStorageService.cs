using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Corathing.Contracts.Bases;

namespace Corathing.Contracts.Services;

public enum StorageHandleCode
{
    None,
    Error,
    Null,
    Success,
}

public interface IStorageHandleArgs<T>
{
    StorageHandleCode Code { get; }
    Func<T> Data { get; }
    string Message { get; }
}

public class StorageHandleArgs<T> : IStorageHandleArgs<T>
{
    public StorageHandleCode Code { get; set; }
    public Func<T> Data { get; set; }
    public string Message { get; set; }
}

public interface IStorageService
{
    string GetAppDataPath();

    string GetAppPackagePath();

    string GetEntityFolder(IEntity entity);
    void DeleteEntityFolder(IEntity entity);

    FileStream OpenFile(IEntity entity, string path, FileMode mode);

    Task<StorageHandleArgs<T>> ReadAsync<T>(string filename);

    Task<StorageHandleArgs<T>> SaveAsync<T>(string filename, T content);

    void Delete(string filename);
}
