using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Corathing.Contracts.Services;

public enum SnackbarMessageType
{
    Primary,
    Secondary,
    Success,
    Danger,
    Warning,
    Info,
    Light,
    Dark,
}

public interface IDialogService
{
    void ShowErrorMeessage(string message);

    /// <summary>
    /// Show alert message with title, message, icon and message type on the snackbar
    /// 스낵바를 이용해 제목, 메시지, 아이콘, 메시지 타입을 가진 알림 메시지를 표시합니다.
    /// </summary>
    /// <param name="title"></param>
    /// <param name="message"></param>
    /// <param name="icon"></param>
    /// <param name="messageType"></param>
    /// <returns></returns>
    void ShowAlert(string title, string message, string icon, SnackbarMessageType messageType = SnackbarMessageType.Primary);
    void ShowAlertSuccess(string message);
    void ShowAlertWarning(string message);
    void ShowAlertDanger(string message);
    void ShowAlertInfo(string message);
}
