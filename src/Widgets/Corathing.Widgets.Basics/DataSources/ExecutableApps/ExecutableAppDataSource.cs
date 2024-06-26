﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;

using Corathing.Contracts.Attributes;

namespace Corathing.Widgets.Basics.DataSources.ExecutableApps;

[EntryCoraDataSource(typeof(ExecutableAppDataSource))]
public class ExecutableAppDataSource : ObservableRecipient
{
    public string Name { get; set; }
}
