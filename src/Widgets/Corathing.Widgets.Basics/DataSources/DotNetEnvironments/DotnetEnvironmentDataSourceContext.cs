using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Microsoft.CSharp;

namespace Corathing.Widgets.Basics.DataSources.DotNetEnvironments;

public class DotnetEnvironmentDataSourceContext
{
    public void GetDotnetEnvironments()
    {
        var provider = new CSharpCodeProvider();
        var options = new Dictionary<string, string>
        {
            { "CompilerVersion", "v4.0" }
        };

    }

    private CSharpCodeProvider GetProvider()
    {
        var options = new Dictionary<string, string>
        {
            { "CompilerVersion", "v4.0" }
        };

        var provider = new CSharpCodeProvider(options);
        return provider;
    }

    public void Execute(string codes)
    {
        using var provider = GetProvider();

        CompilerParameters compilerParams = new CompilerParameters
        {
            GenerateInMemory = true,
            GenerateExecutable = false
        };

        CompilerResults results = provider.CompileAssemblyFromSource(compilerParams, codes);

        if (results.Errors.Count != 0)
            throw new Exception("Mission failed!");

        object o = results.CompiledAssembly.CreateInstance("Foo.Bar");
        MethodInfo mi = o.GetType().GetMethod("SayHello");
        mi.Invoke(o, null);
    }
}
