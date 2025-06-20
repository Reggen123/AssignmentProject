using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PhoneApp.Domain.Attributes;
using PhoneApp.Domain.DTO;
using PhoneApp.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace EmployeesLoadFromJsonPlugin
{
    [Author(Name = "Ilya Bel")]
    public class Plugin : IPluggable
    {
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        public IEnumerable<DataTransferObject> Run(IEnumerable<DataTransferObject> args)
        {
            logger.Info("Provide path to employlist");
            Console.Write("> ");
            string path = Console.ReadLine();

            logger.Info("Local path or url? (type local or url)");
            Console.Write("> ");
            string type = Console.ReadLine();
            IDataReader dataReader;
            switch (type)
            {
                case "local":
                    {
                        dataReader = new LocalReader(path);
                        break;
                    }
                case "url":
                    {
                        dataReader = new URLReader(path);
                        break;
                    }
                default:
                    {
                        logger.Info("No Data provided");
                        return args.ToList();
                    }
            }
            if(dataReader.GetData() == null)
            {
                logger.Info("Cannot read provided path or link");
                return args.ToList();
            }
            List<EmployeesDTO> list = new List<EmployeesDTO>();
            list = FindFields(dataReader.GetData());

            IEnumerable<DataTransferObject> toret = args;
            if (list != null && list.Count > 0)
                toret = args.Concat(list.Cast<DataTransferObject>());
            logger.Info("Data Saved");
            return toret;
        }

        static JArray FindArrays(JToken token)
        {
            if(token.Type == JTokenType.Array)
            {
                JArray arr = (JArray)token;
                bool isUsers = arr.Count > 0 && arr[0].Type == JTokenType.Object && arr[0]["firstname"] != null && arr[0]["phone"] != null;

                if (isUsers)
                    return arr;
            }
            else if(token.Type == JTokenType.Object)
            {
                foreach(var p in token.Children<JProperty>())
                {
                    var res = FindArrays(p.Value);
                    if (res != null)
                        return res;
                }
            }
            return null;
        }

        static List<EmployeesDTO> FindFields(string path)
        {
            List<EmployeesDTO> employees = new List<EmployeesDTO>();
            JObject json = JObject.Parse(path);
            JArray usersArray = FindArrays(json);

            if (usersArray == null)
            {
                return new List<EmployeesDTO>();
            }

            foreach (var user in usersArray)
            {
                string name = user["firstName"]?.ToString();
                string lastname = user["lastName"]?.ToString();
                string phone = user["phone"]?.ToString();
                EmployeesDTO employee = new EmployeesDTO();
                employee.Name = name + " " + lastname;
                employee.AddPhone(phone);
                employees.Add(employee);
            }
            return employees;
        }
    }
}
