// MADE Apps licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Corathing.UI.ObjectModel;

public class ItemPropertyChangedEventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ObservableItemCollectionPropertyChangedEventArgs"/> class.
    /// </summary>
    /// <param name="sender">
    /// The <see cref="INotifyPropertyChanged"/> object sender.
    /// </param>
    /// <param name="index">
    /// The index of the <paramref name="sender"/> within the <see cref="ObservableItemCollection{T}"/>.
    /// </param>
    /// <param name="eventArgs">
    /// The associated property changed event argument.
    /// </param>
    public ItemPropertyChangedEventArgs(
        object? sender,
        int index,
        PropertyChangedEventArgs eventArgs)
    {
        Sender = sender;
        Index = index;
        EventArgs = eventArgs;
    }

    /// <summary>
    /// Gets the <see cref="INotifyPropertyChanged"/> object sender.
    /// </summary>
    public object? Sender { get; }

    /// <summary>
    /// Gets index of the <see cref="Sender"/> within the <see cref="ObservableItemCollection{T}"/>.
    /// </summary>
    public int Index { get; }

    /// <summary>
    /// Gets associated property changed event argument.
    /// </summary>
    public PropertyChangedEventArgs EventArgs { get; }
}
