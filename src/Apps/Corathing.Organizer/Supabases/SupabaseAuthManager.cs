using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Supabase;
using Supabase.Interfaces;

namespace Corathing.Organizer.Supabases;

public class SupabaseAuthManager
{
    public Supabase.Client Client { get; private set; }

    public SupabaseAuthManager(IServiceProvider services)
    {

    }

    public async Task InitializeAsync()
    {
        var url = Environment.GetEnvironmentVariable("SUPABASE_URL");
        var key = Environment.GetEnvironmentVariable("SUPABASE_KEY");

        var options = new Supabase.SupabaseOptions
        {
            AutoConnectRealtime = true
        };

        var supabase = new Supabase.Client(url, key, options);
        await supabase.InitializeAsync();
    }
}
