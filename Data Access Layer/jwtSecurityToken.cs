
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
namespace Data_Access_Layer
{
    public class jwtSecurityToken
    {
        public static string jwtsecuritytoken()
        {
            string Token;
            var secretKey = new SymmetricSecurityKey
                   (Encoding.UTF8.GetBytes("thisisasecretkey@123"));

            var signinCredentials = new SigningCredentials

           (secretKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: "ABCXYZ",
                audience: "http://localhost:51398",
                claims: new List<Claim>(),
                expires: DateTime.Now.AddMinutes(10),
                signingCredentials: signinCredentials
            );
            Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);


            return Token;
        }

       

    }
}
