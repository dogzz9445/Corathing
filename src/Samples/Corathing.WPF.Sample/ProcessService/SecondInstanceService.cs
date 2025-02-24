using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Corathing.WPF.Sample.ProcessService;

/// <summary>
/// This class handles communication between two instances form the already running instance
/// </summary>
public sealed class SecondInstanceService : IDisposable
{
    public enum SecondInstanceRequest : byte
    {
        None = 0,
        MaximizeWindow = 1
    }

    private bool _disposed = false;
    private Task _executeTask;

    private CancellationTokenSource _stoppingCts;

    //
    // 요약:
    //     Gets the Task that executes the background operation.
    //
    // 설명:
    //     Will return null if the background operation hasn't started.
    public Task? ExecuteTask => _executeTask;

    private NamedPipeServerStream _serverPipe = null;

    public event Action<SecondInstanceRequest> OnSecondInstanceRequest;

    public SecondInstanceService()
    {
        _serverPipe = null;
    }

    //
    // 요약:
    //     Triggered when the application host is ready to start the service.
    //
    // 매개 변수:
    //   cancellationToken:
    //     Indicates that the start process has been aborted.
    //
    // 반환 값:
    //     A System.Threading.Tasks.Task that represents the asynchronous Start operation.
    public Task StartAsync(CancellationToken cancellationToken)
    {
        _stoppingCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        _executeTask = ExecuteAsync(_stoppingCts.Token);
        if (_executeTask.IsCompleted)
        {
            return _executeTask;
        }

        return Task.CompletedTask;
    }

    protected async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _serverPipe = NamedPipeServerStreamFactory.GetServerPipe();
        while (!stoppingToken.IsCancellationRequested)
        {
            await _serverPipe.WaitForConnectionAsync(stoppingToken);
            while (_serverPipe.IsConnected)
            {
                using (var stream = new MemoryStream())
                {
                    byte[] buffer = new byte[32];
                    int bytesRead;
                    while ((bytesRead = _serverPipe.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        stream.Write(buffer, 0, bytesRead);
                    }
                    byte[] result = stream.ToArray();
                    if (result.Length == 1)
                    {
                        SecondInstanceRequest request = (SecondInstanceRequest)result[0];
                        new Task(() => OnSecondInstanceRequest?.Invoke(request)).Start();
                    }
                }
            }
            _serverPipe.Disconnect();
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_executeTask == null)
        {
            return;
        }

        try
        {
            _stoppingCts.Cancel();
        }
        finally
        {
            TaskCompletionSource<object> taskCompletionSource = new TaskCompletionSource<object>();
            using (cancellationToken.Register((s) =>
            {
                if (s is TaskCompletionSource<object> source)
                {
                    source.SetCanceled();
                }
            }, taskCompletionSource))
            {
                await Task.WhenAny(_executeTask, taskCompletionSource.Task).ConfigureAwait(continueOnCapturedContext: false);
            }
        }
    }

    public void Dispose()
    {
        Dispose(true);
    }

    private void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (_disposed)
            {
                return;
            }
            _disposed = true;
            _stoppingCts?.Dispose();
            Cancel();
        }
    }

    /// <summary>
    /// Cancels communication and disconnects from other instance if connected
    /// </summary>
    public void Cancel()
    {
        if (_serverPipe == null)
            return;

        if (_serverPipe.IsConnected)
        {
            _serverPipe.Disconnect();
        }
        _serverPipe.Dispose();

        _serverPipe = null;
    }
}
