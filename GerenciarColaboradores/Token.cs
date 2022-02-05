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
        DateTime? _expiresToken;
        private string token;
        public Token()
        {
            _jwtAut = new HttpClient();
            _jwtAut.BaseAddress = new Uri("http://localhost:5191/Authentication/login");
        }
        public string TokenString { get; set; }
        public DateTime ExpireTime{ get; set; }
        
        public async Task<string> GetToken()
        {
            var userToken = new
            {
                UserName = "herbertzin",
                Password = "Corey777"
            };
            var convertJson = new StringContent(JsonConvert.SerializeObject(userToken), Encoding.UTF8, "application/json");
            var token = await _jwtAut.PostAsync(string.Empty, convertJson);
            var resultToken = await token.Content.ReadAsStringAsync();
            TokenString = resultToken;
            return TokenString;

        }    
              
        
    }
}
