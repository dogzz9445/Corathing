﻿// MADE Apps licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Corathing.UI.ObjectModel;

public class ObservableItemCollection<T> : ObservableCollection<T>, IDisposable
    where T : class
{
    private bool _enableCollectionChanged = true;

    private bool _disposed;

    /// <summary>
    /// Occurs when an item is added, removed, changed, moved, or the entire list is refreshed.
    /// </summary>
    public override event NotifyCollectionChangedEventHandler? CollectionChanged;

    /// <summary>
    /// Occurs when an item's <see cref="INotifyPropertyChanged.PropertyChanged"/> event is invoked.
    /// </summary>
    public event ItemPropertyChangedEventHandler? ItemPropertyChanged;

    /// <summary>
    /// Initializes a new instance of the <see cref="ObservableItemCollection{T}"/> class that is empty and has a default initial capacity.
    /// </summary>
    /// <exception cref="Exception">Potentially thrown by the <see cref="CollectionChanged"/> callback.</exception>
    public ObservableItemCollection()
    {
        base.CollectionChanged += (s, e) =>
        {
            if (_enableCollectionChanged)
            {
                CollectionChanged?.Invoke(this, e);
            }
        };
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ObservableItemCollection{T}"/> class that contains elements copied from the specified collection
    /// and has sufficient capacity to accommodate the number of elements copied.
    /// </summary>
    /// <param name="collection">
    /// The collection whose elements are copied to the new list.
    /// </param>
    /// <exception cref="T:System.ArgumentNullException">The <paramref name="collection">collection</paramref> parameter cannot be null.</exception>
    /// <exception cref="Exception">Potentially thrown by the <see cref="CollectionChanged"/> callback.</exception>
    public ObservableItemCollection(IEnumerable<T> collection)
        : base(collection)
    {
        base.CollectionChanged += (s, e) =>
        {
            if (_enableCollectionChanged)
            {
                CollectionChanged?.Invoke(this, e);
            }
        };
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ObservableItemCollection{T}"/> class that contains elements copied from the specified list.
    /// </summary>
    /// <param name="list">
    /// The list whose elements are copied to the new list.
    /// </param>
    /// <exception cref="T:System.ArgumentNullException">The <paramref name="list">list</paramref> parameter cannot be null.</exception>
    /// <exception cref="Exception">Potentially thrown by the <see cref="CollectionChanged"/> callback.</exception>
    public ObservableItemCollection(List<T> list)
        : base(list)
    {
        base.CollectionChanged += (s, e) =>
        {
            if (_enableCollectionChanged)
            {
                CollectionChanged?.Invoke(this, e);
            }
        };
    }

    /// <summary>
    /// Adds a range of objects to the end of the collection.
    /// </summary>
    /// <param name="items">
    /// The objects to add to the end of the collection.
    /// </param>
    /// <exception cref="Exception">Potentially thrown by the <see cref="CollectionChanged"/> callback.</exception>
    public void AddRange(IEnumerable<T> items)
    {
        CheckDisposed();
        _enableCollectionChanged = false;

        var itemsToAdd = items.ToList();

        foreach (var item in itemsToAdd)
        {
            Add(item);
        }

        _enableCollectionChanged = true;
        CollectionChanged?.Invoke(
            this,
            new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, itemsToAdd));
    }

    /// <summary>
    /// Removes a range of objects from the collection.
    /// </summary>
    /// <param name="items">
    /// The objects to remove from the collection.
    /// </param>
    /// <exception cref="Exception">Potentially thrown by the <see cref="CollectionChanged"/> callback.</exception>
    public void RemoveRange(IEnumerable<T> items)
    {
        CheckDisposed();
        _enableCollectionChanged = false;

        var itemsToRemove = items.ToList();

        foreach (var item in itemsToRemove)
        {
            Remove(item);
        }

        _enableCollectionChanged = true;
        CollectionChanged?.Invoke(
            this,
            new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, itemsToRemove));
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }
        if (disposing)
        {
            ClearItems();
        }
        _disposed = true;
    }

    /// <summary>
    /// Checks whether the collection is disposed.
    /// </summary>
    /// <exception cref="ObjectDisposedException">
    /// Thrown if the object is disposed.
    /// </exception>
    public void CheckDisposed()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(GetType().FullName);
        }
    }

    /// <summary>
    /// Raises the <see cref="CollectionChanged"/> event with the provided arguments.
    /// </summary>
    /// <param name="e">The arguments of the event being raised.</param>
    protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
    {
        CheckDisposed();
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                RegisterPropertyChangedEvents(e.NewItems);
                break;
            case NotifyCollectionChangedAction.Remove:
            case NotifyCollectionChangedAction.Replace:
                UnregisterPropertyChangedEvents(e.OldItems);
                if (e.NewItems != null)
                {
                    RegisterPropertyChangedEvents(e.NewItems);
                }

                break;
            case NotifyCollectionChangedAction.Move:
            case NotifyCollectionChangedAction.Reset: break;
        }

        base.OnCollectionChanged(e);
    }

    /// <summary>
    /// Removes all items from the collection.
    /// </summary>
    protected override void ClearItems()
    {
        UnregisterPropertyChangedEvents(this);
        base.ClearItems();
    }

    private void RegisterPropertyChangedEvents(IEnumerable items)
    {
        CheckDisposed();
        foreach (INotifyPropertyChanged item in items.Cast<INotifyPropertyChanged>().Where(item => item != null))
        {
            item.PropertyChanged += OnItemPropertyChanged;
        }
    }

    private void UnregisterPropertyChangedEvents(IEnumerable items)
    {
        CheckDisposed();
        foreach (INotifyPropertyChanged item in items.Cast<INotifyPropertyChanged>().Where(item => item != null))
        {
            item.PropertyChanged -= OnItemPropertyChanged;
        }
    }

    private void OnItemPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        CheckDisposed();
        if (sender is T item)
        {
            ItemPropertyChanged?.Invoke(
                this,
                new ItemPropertyChangedEventArgs(sender, IndexOf(item), e));
        }
    }
}
