using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BusinessLayer.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ModelLayer.DTO;
using ModelLayer.Model.Entities;
using RepositoryLayer.Interface;

namespace BusinessLayer.Service
{
    public class AuthBL : IAuthBL
    {
        private readonly IUserRL _userRL;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;
        private readonly IRabbitMQPublisherService _rabbitMQ;

        public AuthBL(IUserRL userRL, IMapper mapper, IConfiguration config, IRabbitMQPublisherService rabbitMQ)
        {
            _userRL = userRL;
            _mapper = mapper;
            _config = config;
            _rabbitMQ = rabbitMQ;
        }

        public async Task<string> RegisterAsync(RegisterDTO registerDto)
        {
            var existingUser = await _userRL.GetUserByUsernameAsync(registerDto.Username);
            if (existingUser != null)
            {
                return "User already exists";
            }

            var user = new User
            {
                Username = registerDto.Username,
                Email = registerDto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password)
            };

            await _userRL.CreateUserAsync(user);

            // Publish event to RabbitMQ
            var message = $"New user registered: {registerDto.Email}";
            _rabbitMQ.PublishMessage(message, "user_registered");

            return GenerateJwtToken(user);
        }

        public async Task<string> LoginAsync(LoginDTO loginDto)
        {
            var user = await _userRL.GetUserByUsernameAsync(loginDto.Username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            {
                return null;
            }

            return GenerateJwtToken(user);
        }

        private string GenerateJwtToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var token = new JwtSecurityToken(
                _config["Jwt:Issuer"],
                _config["Jwt:Audience"],
                claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
