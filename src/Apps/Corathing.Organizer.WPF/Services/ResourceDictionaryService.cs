using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using Corathing.Contracts.Services;

namespace Corathing.Organizer.WPF.Services;

public class ResourceDictionaryService : IResourceDictionaryService
{
    public void RegisterResourceDictionary(Uri uri)
    {
        App.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = uri });
    }
}
