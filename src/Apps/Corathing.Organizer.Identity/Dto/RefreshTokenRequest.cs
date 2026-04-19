using System;

namespace Corathing.Organizer.Identity.Dto;

public class RefreshTokenRequest
{
    public string RefreshToken { get; set; } = string.Empty;
}
