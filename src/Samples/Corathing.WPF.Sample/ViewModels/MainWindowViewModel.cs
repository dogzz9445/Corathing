using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;

using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Input;
using System.Windows.Documents;
using CommunityToolkit.Mvvm.Input;
using System.Runtime.Serialization;

namespace Corathing.WPF.Sample.ViewModels
{
    public partial class LogItem : ObservableObject
    {
        [ObservableProperty]
        private string _title;

        [ObservableProperty]
        private string _content;
    }

    public partial class MainWindowViewModel : ObservableObject
    {
        public ICommand ClosePopupCommand { get; }
        public ObservableCollection<LogItem> Logs { get; set; } = new ObservableCollection<LogItem>();

        [ObservableProperty]
        private string _detailsText;
        [ObservableProperty]
        private LogItem _selectedLog;

        public MainWindowViewModel()
        {
            for (int i = 0; i < 100; i++)
            {
                Logs.Add(new LogItem() { Title = $"Test{i}", Content = $"Sample Log 0000{i}" });
            }
        }

        [RelayCommand]
        private void MouseDown(object param)
        {
        }

        partial void OnSelectedLogChanged(LogItem item)
        {
            if (item != null)
            {
                // 상세 정보 업데이트
                DetailsText = item.Content;
            }
        }
    }
}
