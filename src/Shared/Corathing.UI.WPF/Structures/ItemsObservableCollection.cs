using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Corathing.UI.WPF.Structures;

public class ItemsObservableCollection<T> : ObservableCollection<T> where T : INotifyPropertyChanged
{
    public ItemsObservableCollection(IEnumerable<T> initialData) : base(initialData)
    {
        Initialize();
    }

    public ItemsObservableCollection()
    {
        Initialize();
    }

    private void Initialize()
    {
        foreach (T item in Items)
            item.PropertyChanged += OnItemsPropertyChanged;

        CollectionChanged += OnCollectionChangedChanged;
    }

    private void OnCollectionChangedChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems != null)
        {
            foreach (T item in e.NewItems)
            {
                if (item != null)
                    item.PropertyChanged += OnItemsPropertyChanged;
            }
        }

        if (e.OldItems != null)
        {
            foreach (T item in e.OldItems)
            {
                if (item != null)
                    item.PropertyChanged -= OnItemsPropertyChanged;
            }
        }
    }

    private void OnItemsPropertyChanged(object sender, PropertyChangedEventArgs e)
        => CollectionPropertyChanged?.Invoke(sender, e);

    public event PropertyChangedEventHandler CollectionPropertyChanged;
}
