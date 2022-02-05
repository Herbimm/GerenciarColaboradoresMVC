using System.Threading.Tasks;

namespace GerenciarColaboradores
{
    public interface IToken
    {
        Task<string> GetToken();
    }
}
