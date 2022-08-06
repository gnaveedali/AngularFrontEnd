using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Access_Layer
{
    public class Api_Url_Service
    {
        private readonly IConfiguration _config;
        public Api_Url_Service(IConfiguration config)
        {
            _config = config;
        }
        public string GetFilesUrl()
        {
            return _config.GetSection("NodeFileUrl").Value;
        }
    }
}
