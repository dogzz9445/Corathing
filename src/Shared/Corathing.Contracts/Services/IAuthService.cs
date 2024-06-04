using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Corathing.Contracts.Services;

public enum AuthorizationLevel
{
    Guest,
    Modifier,
    Administrator,
}

public enum AuthenticationCode
{
    Error,
    NoUser,
    InsufficientPrivileges,
    Success,
}

public class AuthenticationInfo
{
    public string username;
    public string password;
}

public class AuthenticationResponse
{
    public AuthenticationCode Code = AuthenticationCode.Error;
}

public class AuthorizationInfo
{
    public AuthorizationLevel AuthrizationLevel { get; set; }
}

public interface IAuthService
{
    public string SecretKey { get; set; }
    public bool UseAuthService { get; set; }
    public bool IsAuthorized { get; set; }
}
