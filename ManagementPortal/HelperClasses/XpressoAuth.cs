using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System.Text.Json;

namespace ManagementPortal.HelperClasses
{
    public class XpressoAuth : AuthenticationStateProvider
    {

        private readonly IJSRuntime _jsRuntime;

        public XpressoAuth(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        private string Username { get; set; }
        private string Password { get; set; }

        public ClaimsPrincipal LoggedInUser { get; set; }


        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            string authToken = string.Empty;
            try
            {
                authToken = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "authToken");
            }
            catch (Exception ex)
            { 
            }

            if (!string.IsNullOrEmpty(authToken) && authToken != "null")
            {
                try
                {
                    XpressoTokenHelper objTokenHelper = new XpressoTokenHelper();
                    LoggedInUser = await objTokenHelper.DecodeJWT(authToken);
                    return new AuthenticationState(LoggedInUser);
                }
                catch (Exception ex)
                {
                    var output = await _jsRuntime.InvokeAsync<string>("localStorage.setItem", "authToken", null);
                    LoggedInUser = new ClaimsPrincipal();
                    return new AuthenticationState(LoggedInUser);
                }
            }
            if (Username == "tanuj@xpresso.cash" && Password == "Genx123!@#")
            {
                XpressoTokenHelper objTokenHelper = new XpressoTokenHelper();
                string jwtToken = await objTokenHelper.GenerateJSONWebToken("abcdefg");

                var identity = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, "Tanuj Roy"),
                     new Claim(ClaimTypes.Role, "Support Manager"),
                     new Claim(ClaimTypes.Email, "tanuj@xpresso.cash")
                }, "Xpresso Authentication");


                LoggedInUser = new ClaimsPrincipal(identity);

                var output = await _jsRuntime.InvokeAsync<string>("localStorage.setItem", "authToken", jwtToken);
                return new AuthenticationState(LoggedInUser);
            }
            else
            {
                LoggedInUser = new ClaimsPrincipal();
                return new AuthenticationState(LoggedInUser);
            }
        }

        public async Task CheckLogin(string usernameParam, string password)
        {
            Username = usernameParam;
            Password = password;

            Task<AuthenticationState> objAuthState = GetAuthenticationStateAsync();
            NotifyAuthenticationStateChanged(objAuthState);
            
        }

        public async Task LogoutUser()
        {
            LoggedInUser = new ClaimsPrincipal();
            Username = string.Empty;
            Password = string.Empty;
            var output = await _jsRuntime.InvokeAsync<string>("localStorage.setItem", "authToken", null);
            Task<AuthenticationState> objAuthState = GetAuthenticationStateAsync();
            NotifyAuthenticationStateChanged(objAuthState);
            
        }

        private static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            var payload = jwt.Split('.')[1];
            var jsonBytes = ParseBase64WithoutPadding(payload);
            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);
            return keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString()));
        }

        private static byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            return Convert.FromBase64String(base64);
        }
    }
}
