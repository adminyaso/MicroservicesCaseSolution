﻿namespace AuthService.Application.Dtos
{
    public class LogoutRequestDto
    {
        public string RefreshToken { get; set; } = string.Empty;
    }
}