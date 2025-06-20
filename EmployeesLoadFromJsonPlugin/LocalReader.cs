using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeesLoadFromJsonPlugin
{
    internal class LocalReader : IDataReader
    {
        private readonly string path; 

        public LocalReader(string path)
        {
            this.path = path;
        }
        public string GetData()
        {
            try
            {
                return File.ReadAllText(path);
            }
            catch
            {
                return null;
            }
        }
    }
}
