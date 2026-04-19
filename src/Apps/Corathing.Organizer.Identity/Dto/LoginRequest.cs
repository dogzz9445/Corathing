using System;

namespace Corathing.Organizer.Identity.Dto;

public class LoginRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
