using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ManagementPortal.HelperClasses
{
    public class XpressoTokenHelper
    {
        public XpressoTokenHelper()
        {
        }

        public async Task<string> GenerateJSONWebToken(string newCommunicationToken)
        {
            AWSParamStore objParamStore = new AWSParamStore();
            //string storedAppAuthToken = Configuration["Jwt:Key"].ToString();
            string actualAuthToken = await objParamStore.GetValueFromParamStore(DataConstants.ParamStoreJWTKey);
            string jwtIssuer = await objParamStore.GetValueFromParamStore(DataConstants.ParamStoreJWTIssuer);

            //CryptoServ objCrypt = new CryptoServ();
            //string storedAppAuthToken = isLoggedin ? Configuration["Jwt:KeyLoggedin"] : Configuration["Jwt:Key"];
            //string actualAuthToken = objCrypt.Decrypt(storedAppAuthToken, string.Empty); 
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(actualAuthToken));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            //string newCommunicationTokenEncrypted = objCrypt.Encrypt(newCommunicationToken, string.Empty);

            //Claim objClaim = new Claim("communication_token", newCommunicationTokenEncrypted);
            List<Claim> lstClaim = new List<Claim>();
            lstClaim.Add(new Claim(ClaimTypes.Name, "Tanuj Roy"));
            lstClaim.Add(new Claim(ClaimTypes.Role, "Support Manager"));
            lstClaim.Add(new Claim(ClaimTypes.Email, "tanuj@xpresso.cash"));
            var token = new JwtSecurityToken(jwtIssuer,
              jwtIssuer,
              lstClaim,
              expires: DateTime.Now.AddHours(1),
              signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<ClaimsPrincipal> DecodeJWT(string token)
        {

            AWSParamStore objParamStore = new AWSParamStore();
            //string storedAppAuthToken = Configuration["Jwt:Key"].ToString();
            string actualAuthToken = await objParamStore.GetValueFromParamStore(DataConstants.ParamStoreJWTKey);
            string jwtIssuer = await objParamStore.GetValueFromParamStore(DataConstants.ParamStoreJWTIssuer);

            //CryptoServ objCrypt = new CryptoServ();

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(actualAuthToken));

            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var handler = new JwtSecurityTokenHandler();

            var claimsPrincipal = handler.ValidateToken(
                token,
                new TokenValidationParameters
                {
                    ValidAudience = jwtIssuer,
                    ValidIssuer = jwtIssuer,
                    RequireSignedTokens = false,
                    IssuerSigningKey = securityKey
                },
                out SecurityToken securityToken);

            //string tokenParts = securityToken.ToString().Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries)[1];
            //JObject phrase = JsonConvert.DeserializeObject<JObject>(tokenParts);

            //string encryptedToken = phrase["communication_token"].ToString();
            //string actualToken = objCrypt.Decrypt(encryptedToken, string.Empty);
            return claimsPrincipal;
        }
    }
}
