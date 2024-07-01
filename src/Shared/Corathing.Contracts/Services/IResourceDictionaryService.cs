using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Corathing.Contracts.Services;

public interface IResourceDictionaryService
{
    void RegisterResourceDictionary(Uri uri);
}
