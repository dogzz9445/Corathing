using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;

namespace Corathing.Contracts.Bases;

public partial class ContentDialogContext : ObservableObject
{
    [ObservableProperty]
    private string? _dialogTitle;
    [ObservableProperty]
    private int? _dialogWidth;
    [ObservableProperty]
    private int? _dialogHeight;
}
