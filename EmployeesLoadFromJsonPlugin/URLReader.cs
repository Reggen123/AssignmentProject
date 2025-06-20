using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace EmployeesLoadFromJsonPlugin
{
    internal class URLReader : IDataReader
    {
        private readonly string url;

        public URLReader(string url)
        {
            this.url = url;
        }
        public string GetData()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            try
            {
                return new HttpClient().GetStringAsync(url).Result;
            }
            catch
            {
                return null;
            }
        }
    }
}
