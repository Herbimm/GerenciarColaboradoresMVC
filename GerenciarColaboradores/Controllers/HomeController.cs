using AuthenticationClient;
using GerenciarColaboradores.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace GerenciarColaboradores.Controllers
{    
    public class HomeController : Controller
    {
        public static HttpClient Httpclient;
        private readonly IToken _token;
        private readonly IAuthenticationClient _authenticationClient;
        private readonly HttpClient _httpclient;
        private readonly ILogger<HomeController> _logger;
        private readonly HttpClient _jwtAut;

        public HomeController(ILogger<HomeController> logger, IAuthenticationClient authenticationClient, IToken token)
        {
            _token = token;
            _authenticationClient = authenticationClient;
            _httpclient = new HttpClient();
            _httpclient.BaseAddress = new Uri("http://localhost:26278");
            _logger = logger;
            _jwtAut = new HttpClient();
            _jwtAut.BaseAddress = new Uri("http://localhost:5191");
        }        
        public async Task<IActionResult> BuscarColaboradoresAsync()
        {
            var getToken = await _token.GetToken();
            _jwtAut.DefaultRequestHeaders.Remove("Authorization");
            _jwtAut.DefaultRequestHeaders.Add("Authorization","Bearer " + getToken);
            var teste = await _jwtAut.GetAsync("Authentication/Teste");
            var buscarcolaborador = await _httpclient.GetAsync("/GerenciarColaboradores/BuscarColaboradores");
            var content = buscarcolaborador.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<IEnumerable<ColaboradorModel>>(content.Result);
            return View("ListaColaboradores", result);
        }
        
        public async Task<IActionResult> BuscarColaboradoresPeloNomeAsync(string nome)
        {            
            var parameters = JsonConvert.SerializeObject(nome);
            //var result = await _httpclient.PostAsync("/GerenciarColaboradores/BuscarColaboradorPorNome", new StringContent(parameters, Encoding.UTF8, "application/json"));
            var content = new StringContent(JsonConvert.SerializeObject(nome), Encoding.UTF8, "application/json");
            var buscarPeloNome = await _httpclient.PostAsync("/GerenciarColaboradores/BuscarColaboradorPorNome",content);
            var readReturn = buscarPeloNome.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<IEnumerable<ColaboradorModel>>(readReturn.Result);
            return View("ListaColaboradores", result);
        }

        public async Task<IActionResult> CadastrarColaboradorAsync(ColaboradorModel colaborador)
        {
            var parameters = JsonConvert.SerializeObject(colaborador);
            var content = new StringContent(JsonConvert.SerializeObject(colaborador), Encoding.UTF8, "application/json");
            var cadastrar = await _httpclient.PostAsync("/GerenciarColaboradores/CadastrarColaborador", content);
            var readReturn = cadastrar.Content.ReadAsStringAsync();
            if (readReturn.IsCompletedSuccessfully)
            {
                TempData["AlertMensage"] = "Cadastro criado com Sucesso";
            }
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> RemoverColaboradorAsync(string nome)
        {
            
            var teste = await _jwtAut.GetAsync("Authentication/Teste");
            var parameters = JsonConvert.SerializeObject(nome);
            var content = new StringContent(JsonConvert.SerializeObject(nome), Encoding.UTF8, "application/json");
            var removerColaborador = await _httpclient.PostAsync("/GerenciarColaboradores/RemoverColaborador", content);
            var readReturn = removerColaborador.Content.ReadAsStringAsync();           
            if (readReturn.IsCompletedSuccessfully)
            {
                TempData["AlertMensage"] = "Excluido colaborador com Sucesso";
            }
            return RedirectToAction("Index");
        }
            public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> Login(LoginModel loginModel)
        {
            
            return View("_Login");
        }
        public async Task<IActionResult> LoginAuth(LoginModel loginModel)
        {
            var getToken = _token.GetToken();            
            var convertJson1 = new StringContent(JsonConvert.SerializeObject(loginModel), Encoding.UTF8, "application/json");
            var login1 = await _jwtAut.PostAsync("Authentication/Login", convertJson1);
            var token1 = await login1.Content.ReadAsStringAsync();            

            object obj = "ok";
            var testeToken = await _authenticationClient.Post<object>("aasd", obj);
            var convertJson = new StringContent(JsonConvert.SerializeObject(loginModel), Encoding.UTF8, "application/json");
            var login = await _jwtAut.PostAsync("Authentication/Login", convertJson);            
            var token = await login.Content.ReadAsStringAsync();
            _jwtAut.DefaultRequestHeaders.Remove("Authorization");
            _jwtAut.DefaultRequestHeaders.Add("Authorization", "Bearer "+ token);
            
            var teste = await _jwtAut.GetAsync("Authentication/Teste");           

            if (login.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            return View();
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

       
    }
}
