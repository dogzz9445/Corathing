// This Source Code Form is subject to the terms of the Mozilla Public
// License, v.2.0. If a copy of the MPL was not distributed with this file,
// You can obtain one at https://mozilla.org/MPL/2.0/.
// https://github.com/hexagon-oss/openhardwaremonitor

using System.Diagnostics;
using System.IO.Pipes;

namespace Corathing.Organizer.Utils;

public sealed class InterprocessCommunicationFactory
{
    public static NamedPipeServerStream GetServerPipe()
    {
        return new NamedPipeServerStream(Process.GetCurrentProcess().ProcessName, PipeDirection.InOut);
    }

    public static NamedPipeClientStream GetClientPipe()
    {
        return new NamedPipeClientStream(".", Process.GetCurrentProcess().ProcessName, PipeDirection.InOut);
    }
}
