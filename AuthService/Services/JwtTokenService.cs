using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthService.Services
{
    public class JwtTokenService
    {
        private const string SecurityKey = "mMdAhocQbIAa1/4iD8W5BiDCD9Lxg9ULp4qROgJVN8oRZommyAsnRalnNlzWGbKGJItr/kh2jVd2d9brhSBAJttV7NE47dvyX6n36cFKlnz3k9AodqqVgH/S52oQMYamtI+HsQqBmsvZMqOE+oGlEIzJG9tmDZ1JE/qJHq+bXo3RCEuBf26dGuIG4DWpjh+G4xTVC7ZoByCmq5zTUUyTlFZCQ2483iJe1Thkem9mlzt3cOy8O5SYJBafIb0xdIBYEoHl56Z805fO/W4eAw+M5stSCUdJTBUtWbCiId9zSapmilb20sCg4l5xYTsaJImTfHlo0t9kF1o/RXwr1cw3zCPoyt9tjWhZ83LMsi1ydBg=";

        public AuthenticationToken CreateToken()
        {
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecurityKey));
            var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim("scope", "userapi.read"),
                new Claim("scope", "tweetapi.read"),
                new Claim("scope", "searchapi.read")
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
