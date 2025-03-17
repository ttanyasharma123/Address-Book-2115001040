using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BusinessLayer.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ModelLayer.DTO;
using ModelLayer.Model.Entities;
using RepositoryLayer.Interface;

namespace BusinessLayer.Service
{
    public class ForgotPasswordService : IForgotPasswordService
    {
        private readonly IUserRL _userRL;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _config;

        public ForgotPasswordService(IUserRL userRL, IEmailService emailService, IConfiguration config)
        {
            _userRL = userRL;
            _emailService = emailService;
            _config = config;
        }

        public async Task<bool> ForgotPasswordAsync(ForgotPasswordDTO dto)
        {
            var user = await _userRL.GetUserByEmailAsync(dto.Email);
            if (user == null) return false;

            var token = GenerateResetToken(user);
            user.PasswordResetToken = token;
            user.ResetTokenExpiry = DateTime.UtcNow.AddHours(1);
            await _userRL.UpdateUserAsync(user);

            var resetLink = $"{_config["FrontendUrl"]}/reset-password?token={token}&email={dto.Email}";
            await _emailService.SendEmailAsync(dto.Email, "Password Reset Request", $"Click <a href='{resetLink}'>here</a> to reset your password.");

            return true;
        }

        public async Task<bool> ResetPasswordAsync(ResetPasswordDTO dto)
        {
            var user = await _userRL.GetUserByEmailAsync(dto.Email);
            if (user == null || user.PasswordResetToken != dto.Token || user.ResetTokenExpiry < DateTime.UtcNow)
                return false;

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            user.PasswordResetToken = null;
            user.ResetTokenExpiry = null;
            await _userRL.UpdateUserAsync(user);
            return true;
        }

        private string GenerateResetToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var token = new JwtSecurityToken(
                _config["Jwt:Issuer"],
                _config["Jwt:Audience"],
                claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
