using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Corathing.Contracts.Services;

namespace Corathing.Organizer.WPF.Services;

public class DialogService : IDialogService
{
    public void ShowErrorMeessage(string message)
    {
    }

    #region Alert Message

    private async Task ShowAlertInternalAsync(string title, string message, string icon, SnackbarMessageType messageType)
    {
        await Task.Yield();

    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="title"></param>
    /// <param name="message"></param>
    /// <param name="icon"></param>
    /// <param name="messageType"></param>
    /// <returns></returns>
    public async void ShowAlert(string title, string message, string icon, SnackbarMessageType messageType = SnackbarMessageType.Primary)
    {
        await ShowAlertInternalAsync(title, message, icon, messageType);
    }

    public async void ShowAlertDanger(string message)
    {
        await ShowAlertInternalAsync("Error", message, "Error", SnackbarMessageType.Danger);
    }

    public async void ShowAlertInfo(string message)
    {
        await ShowAlertInternalAsync("Info", message, "Info", SnackbarMessageType.Info);
    }

    public async void ShowAlertSuccess(string message)
    {
        await ShowAlertInternalAsync("Success", message, "Success", SnackbarMessageType.Success);
    }

    public async void ShowAlertWarning(string message)
    {
        await ShowAlertInternalAsync("Warning", message, "Warning", SnackbarMessageType.Warning);
    }
    #endregion
}
