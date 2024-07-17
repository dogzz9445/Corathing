﻿// MADE Apps licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Corathing.UI.ObjectModel;

/// <summary>
/// Defines event handler for when an <see cref="INotifyPropertyChanged"/> object has invoked the
/// <see cref="INotifyPropertyChanged.PropertyChanged"/> event within a <see cref="ObservableItemCollection{T}"/>.
/// </summary>
/// <param name="sender">
/// The <see cref="ObservableItemCollection{T}"/> sender.
/// </param>
/// <param name="args">
/// The associated property changed event argument for the item.
/// </param>
public delegate void ItemPropertyChangedEventHandler(
    object? sender,
    ItemPropertyChangedEventArgs args);
