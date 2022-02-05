using AuthenticationClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Polly;
using Polly.Retry;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace GerenciarColaboradores
{
    public class AuthenticationClient : IAuthenticationClient
    {       
        private static HttpClient _jwtAut;
        DateTime? _expiresToken;
        private string token;
        public AuthenticationClient()        
        {
            _jwtAut = new HttpClient();
            _jwtAut.BaseAddress = new Uri("http://localhost:5191/Authentication/login");
            
        }
        public async Task Authentication()
        {
            var userToken = new
            {
                UserName = "herbertzin",
                Password = "Corey777"
            };
            var convertJson = new StringContent(JsonConvert.SerializeObject(userToken), Encoding.UTF8, "application/json");
            var login = await _jwtAut.PostAsync(string.Empty, convertJson);
            var token = await login.Content.ReadAsStringAsync();
            _jwtAut.DefaultRequestHeaders.Remove("Authorization");
            _jwtAut.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
            _expiresToken = DateTime.Now.AddMinutes(5);
        }

        public async Task<TResult> Post<TResult>(string path , object data)
        {
            try
            {
                if (!UsuarioLogado())
                {
                    await Authentication();
                }
                var jsonSettings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,

                };
                var content = new StringContent(JsonConvert.SerializeObject(data, jsonSettings), Encoding.UTF8, "application/json");
                //var response = await HttpClient.PostAsync(path, content);
                AsyncRetryPolicy retryIfException = Policy.Handle<Exception>().RetryAsync(3);
                AsyncRetryPolicy<HttpResponseMessage> httpRetryPolicy = Policy
                 .HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
                 .Or<HttpRequestException>()
                 .WaitAndRetryAsync(3, retryAttempt =>
                         TimeSpan.FromSeconds(Math.Pow(2, retryAttempt) / 2));

                HttpResponseMessage response = await
                httpRetryPolicy.ExecuteAsync(
                    () => _jwtAut.PostAsync(path, content));
                switch (response.StatusCode)
                {
                    case System.Net.HttpStatusCode.OK:
                        {
                            var resultado = await response.Content.ReadAsStringAsync();
                            return JsonConvert.DeserializeObject<TResult>(resultado, new JsonSerializerSettings
                            {
                                Error = HandleDeserializationError
                            });
                            break;
                        }
                    case System.Net.HttpStatusCode.BadRequest:
                        {
                            throw new Exception(response.ReasonPhrase);
                        }
                       
                }
                return await Task.FromResult(default(TResult));
            }
            catch (Exception)
            {

                throw;
            }
            
            

        }
        private bool UsuarioLogado()
        {
            if (_expiresToken == null || string.IsNullOrEmpty(token) || _expiresToken.GetValueOrDefault() < DateTime.Now)
                return false;

            return true;
        }
        public void HandleDeserializationError(object sender, ErrorEventArgs errorArgs)
        {
            var currentError = errorArgs.ErrorContext.Error.Message;
            errorArgs.ErrorContext.Handled = true;
        }
    }
}
