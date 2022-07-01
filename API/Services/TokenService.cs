using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Constants;
using API.DTOs;
using Domain;

namespace API.Services
{
    public class TokenService
    {
        private readonly IConfiguration _config;
        public TokenService(IConfiguration config)
        {
            _config = config;
        }

        public string CreateToken(User user, string role, TagDto tagDto)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.UserName),
                new Claim(ClaimTypes.Name, user.FirstName),
                new Claim(ClaimTypes.Surname, user.LastName),
                new Claim(nameof(User.InOffice), user.InOffice.ToString())
            };

            if (!string.IsNullOrWhiteSpace(role))
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            if (tagDto is not null)
            {

                claims.Add(
                    new Claim(
                        AppConstants.TagName,
                        tagDto.Code));

                claims.Add(
                    new Claim(
                        AppConstants.TagTunnelStatusName,
                        tagDto.StatusTunnel.ToString()));

                claims.Add(
                    new Claim(
                        AppConstants.TagTunnelIsAuthorizedName,
                        tagDto.IsAuthorizedTunnel.ToString()));

                claims.Add(
                    new Claim(
                        AppConstants.TagTunnelExpirationName,
                        tagDto.TagTunnelExpiresAt.ToString("dd/MM/yyyy HH:mm:ss")));

                claims.Add(
                    new Claim(
                        AppConstants.TagOfficeStatusName,
                        tagDto.StatusOffice.ToString()));

                claims.Add(
                    new Claim(
                        AppConstants.TagOfficeIsAuthorizedName,
                        tagDto.IsAuthorizedOffice.ToString()));

                claims.Add(
                    new Claim(
                        AppConstants.TagOfficeExpirationName,
                        tagDto.TagOfficeExpiresAt.ToString("dd/MM/yyyy HH:mm:ss")));

            }

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["TokenKey"]));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);

        }
    
        public string DecodeToken(string jwtToken)
        {
            var handler = new JwtSecurityTokenHandler();
            if (jwtToken != "Bearer"){
                var tokenData =  handler.ReadJwtToken(jwtToken);
            return tokenData.Payload.Claims.FirstOrDefault(x =>x.Type == "email").Value;
            }
            return string.Empty;
        }
    }
}