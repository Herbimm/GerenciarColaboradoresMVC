using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthenticationClient
{
    public interface IAuthenticationClient
    {
        Task<TResult> Post<TResult>(string path, object data);
    }
}
