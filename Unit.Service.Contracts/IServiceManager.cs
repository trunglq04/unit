using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unit.Service.Contracts
{
    public interface IServiceManager
    {
        IUserService UserService { get; }

        IAuthenticationService AuthenticationService { get; }
    }
}
