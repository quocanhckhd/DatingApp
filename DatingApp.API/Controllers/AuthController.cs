using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DatingApp.API.Models;
using DatingApp.API.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.API.Controllers {
    [Route ("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase {
        private readonly IAuthRepository _repo;
        private readonly IConfiguration _config;
        public AuthController (IAuthRepository repo, IConfiguration config) {
            _repo = repo;
            _config = config;
        }

        [HttpPost ("register")]
        public async Task<IActionResult> Register (RegisterDTO dto) {
            //Validate Request
            if (!ModelState.IsValid)
                return BadRequest (ModelState);

            dto.UserName = dto.UserName.ToLower ();
            if (await _repo.UserExisted (dto.UserName)) return BadRequest ("User already exists");

            var userToCreate = new User {
                UserName = dto.UserName
            };

            var createdUser = await _repo.Register (userToCreate, dto.Password);

            return StatusCode (201);
        }

        [HttpPost ("login")]
        public async Task<IActionResult> Login (LoginDTO dto) {

            try {
                var user = await _repo.Login (dto.UserName, dto.Password);

                if (user == null) return Unauthorized ();

                var claims = new [] {
                    new Claim (ClaimTypes.NameIdentifier, user.Id.ToString ()),
                    new Claim (ClaimTypes.Name, user.UserName)
                };

                var key = new SymmetricSecurityKey (Encoding.UTF8
                    .GetBytes (_config.GetSection ("AppSettings:Token").Value));

                var creds = new SigningCredentials (key, SecurityAlgorithms.HmacSha512Signature);

                var tokenDescriptor = new SecurityTokenDescriptor {
                    Subject = new ClaimsIdentity (claims),
                    Expires = DateTime.Now.AddDays (1),
                    SigningCredentials = creds
                };

                var tokenHandler = new JwtSecurityTokenHandler ();

                var token = tokenHandler.CreateToken (tokenDescriptor);

                return Ok (
                    new {
                        token = tokenHandler.WriteToken (token)
                    });
            } catch {
                return StatusCode (500, "Computers really says no");
            }
        }
    }
}