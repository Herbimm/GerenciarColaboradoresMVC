using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace GerenciarColaboradores
{
    public class Token : IToken
    {
        private static HttpClient _jwtAut;
        private string token;
        public Token()
        {
            _jwtAut = new HttpClient();
            _jwtAut.BaseAddress = new Uri("http://localhost:5191/Authentication/login");
        }
        public string TokenString { get; set; }
        public DateTime ExpireTime { get; set; }

        public async Task<string> GetToken()
        {
            if (TokenExpirado())
            {
                var userToken = new
                {
                    UserName = "herbertzin",
                    Password = "Corey777"
                };
                var convertJson = new StringContent(JsonConvert.SerializeObject(userToken), Encoding.UTF8, "application/json");
                var token = await _jwtAut.PostAsync(string.Empty, convertJson);
                var resultToken = await token.Content.ReadAsStringAsync();
                ExpireTime = DateTime.Now;
                TokenString = resultToken;
                return TokenString;
            }
            else
            {
                return TokenString;
            }
        }
        public bool TokenExpirado()
        {
            if (TokenString == null)
            {
                return true;
            }
            var comparaRegra5Dias = DateTime.Now.Subtract(ExpireTime).Minutes;
            if (comparaRegra5Dias > 10)
            {
                return true;
            }
          
            return false;
        }

    }
}
