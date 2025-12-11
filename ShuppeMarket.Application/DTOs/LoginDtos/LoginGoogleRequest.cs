using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShuppeMarket.Application.DTOs.LoginDtos
{
    public class LoginGoogleRequest
    {
        public string IdToken { get; set; } = string.Empty;
    }
}
