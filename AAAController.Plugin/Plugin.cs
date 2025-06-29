using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using PhoneApp.Domain.Attributes;
using PhoneApp.Domain.DTO;
using PhoneApp.Domain.Interfaces;
using Newtonsoft.Json;

namespace AAAController.Plugin
{
    public class User
    {
        public string FirstName { get; set; }
        public string Phone { get; set; }
    }

    public class ApiResponse
    {
        public List<User> Users { get; set; }
    }

    [Author(Name = "Андрей Колобов")]
    public class Plugin : IPluggable
    {
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public IEnumerable<DataTransferObject> Run(IEnumerable<DataTransferObject> input)
        {
            logger.Info("DummyUsersPlugin запущен");

            var users = LoadUsers().GetAwaiter().GetResult();

            logger.Info($"Загружено пользователей: {users.Count}");

            Console.WriteLine("📋 Список загруженных пользователей:");
            for (int i = 0; i < users.Count; i++)
            {
                Console.WriteLine($"{i} Name: {users[i].Name} | Phone: {users[i].Phone}");
            }

            Console.WriteLine("Нажми любую клавишу, чтобы выйти...");
            Console.ReadKey();

            return users;
        }

        private async Task<List<EmployeesDTO>> LoadUsers()
        {
            using (var client = new HttpClient())
            {
                var json = await client.GetStringAsync("https://dummyjson.com/users");
                var data = JsonConvert.DeserializeObject<ApiResponse>(json);

                var employees = new List<EmployeesDTO>();

                foreach (var user in data.Users)
                {
                    var emp = new EmployeesDTO { Name = user.FirstName };
                    emp.AddPhone(user.Phone);
                    employees.Add(emp);
                }

                return employees;
            }
        }
    }
}
