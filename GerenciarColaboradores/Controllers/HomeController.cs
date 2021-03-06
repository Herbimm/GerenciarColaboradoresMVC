using GerenciarColaboradores.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GerenciarColaboradores.Controllers
{    
    public class HomeController : Controller
    {
        private readonly HttpClient _httpclient;
        private readonly ILogger<HomeController> _logger;
        private readonly HttpClient _jwtAut;

        public HomeController(ILogger<HomeController> logger)
        {
            _httpclient = new HttpClient();
            _httpclient.BaseAddress = new Uri("http://localhost:26278");
            _logger = logger;
            _jwtAut = new HttpClient();
            _jwtAut.BaseAddress = new Uri("http://localhost:5191");
        }        
        public async Task<IActionResult> BuscarColaboradoresAsync()
        {
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
            
            return View();
        }
        public async Task<IActionResult> LoginAuth(LoginModel loginModel)
        {

            var convertJson = new StringContent(JsonConvert.SerializeObject(loginModel), Encoding.UTF8, "application/json");
            var login = await _jwtAut.PostAsync("Authentication/Login", convertJson);
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
