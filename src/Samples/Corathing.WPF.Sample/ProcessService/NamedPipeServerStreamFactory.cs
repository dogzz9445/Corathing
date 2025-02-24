using System;
using System.Diagnostics;
using System.IO.Pipes;

namespace Corathing.WPF.Sample.ProcessService;

public static class NamedPipeServerStreamFactory
{
    public static NamedPipeServerStream GetServerPipe()
    {
        return new NamedPipeServerStream(Process.GetCurrentProcess().ProcessName, PipeDirection.InOut);
    }

    public static NamedPipeClientStream GetClientPipe()
    {
        return new NamedPipeClientStream(".", Process.GetCurrentProcess().ProcessName, PipeDirection.InOut);
    }

    public static NamedPipeClientStream GetClientPipe(string processName)
    {
        return new NamedPipeClientStream(processName, Process.GetCurrentProcess().ProcessName, PipeDirection.InOut);
    }
}
