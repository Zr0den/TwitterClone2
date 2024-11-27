using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Authentication.Services
{
    public class JwtTokenService
    {
        private const string SecurityKey = "nMdAhOoLeEerR1/240lLlpJK9Bm3FK2JK2KLM6dV3Dae4/Lolhgp05ledfg40yu5HRo064KHRO240/fffGH5311PASV5t3KFe5LLEF0";

        public AuthenticationToken CreateToken()
        {
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecurityKey));
            var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim("scope", "twitterapi.read"),
                //new Claim("scope", ".read")
            };

            var tokenOptions = new JwtSecurityToken(signingCredentials: signingCredentials, claims: claims);

            var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
            var authToken = new AuthenticationToken 
            {
                Value = tokenString
            };

            return authToken;
        }
    }
}
