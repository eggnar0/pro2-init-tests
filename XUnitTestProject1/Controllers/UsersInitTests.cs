using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Library.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Xunit;

namespace XUnitTestProject1.Controllers
{
    public class UsersInitTests
    {
        private readonly TestServer _server;
        private readonly HttpClient _client;


        public UsersInitTests()
        {
            _server = ServerFactory.GetServerInstance();
            _client = _server.CreateClient();


            using (var scope = _server.Host.Services.CreateScope())
            {
                var _db = scope.ServiceProvider.GetRequiredService<LibraryContext>();

                _db.User.Add(new User
                {
                    IdUser = 1,
                    Email = "jd@pja.edu.pl",
                    Name = "Daniel",
                    Surname = "Jabłoński",
                    Login = "jd",
                    Password = "ASNDKWQOJRJOP!JO@JOP"
                });

                _db.SaveChanges();
            }
        }


        [Fact]
        public async Task GetUsers_200Ok()
        {
            //Arrange i Act

            var cc = new TestServer(new WebHostBuilder()
                     .UseContentRoot(Directory.GetCurrentDirectory())
                   .UseStartup<TestStartup>()).CreateClient();

            var httpResponse = await cc.GetAsync($"{_client.BaseAddress.AbsoluteUri}api/users");

            httpResponse.EnsureSuccessStatusCode();
            var content = await httpResponse.Content.ReadAsStringAsync();
            var users = JsonConvert.DeserializeObject<IEnumerable<User>>(content);
            // using (var scope = _server.Host.Services.CreateScope())
            // {
            //     var _db = scope.ServiceProvider.GetRequiredService<LibraryContext>();
            //     Assert.True(_db.User.Any());
            // }

            Assert.True(users.Count() == 1);
            Assert.True(users.ElementAt(0).Login == "jd");
        }

        [Fact]
        public async Task GetUser_200Ok()
        {
            //Arrange i Act

            var cc = new TestServer(new WebHostBuilder()
                     .UseContentRoot(Directory.GetCurrentDirectory())
                   .UseStartup<TestStartup>()).CreateClient();

            var httpResponse = await cc.GetAsync($"{_client.BaseAddress.AbsoluteUri}api/users/1");

            httpResponse.EnsureSuccessStatusCode();
            var content = await httpResponse.Content.ReadAsStringAsync();
            var user = JsonConvert.DeserializeObject<User>(content);

            Assert.True(user.Login == "jd");
        }

        [Fact]
        public async Task AddUser_200Ok()
        {
            //Arrange i Act

            var cc = new TestServer(new WebHostBuilder()
                     .UseContentRoot(Directory.GetCurrentDirectory())
                   .UseStartup<TestStartup>()).CreateClient();

            var request = new
            {
                Url = "/api/users",
                Body = new
                {
                    IdUser = 2,
                    Email = "pp@pja.edu.pl",
                    Name = "Piotr",
                    Surname = "Pawlak",
                    Login = "pp",
                    Password = "qwertyuiop"
                }
            };

            var httpResponse = await cc.PostAsync(request.Url, new StringContent(JsonConvert.SerializeObject(request.Body), Encoding.Default, "application/json"));

            httpResponse.EnsureSuccessStatusCode();

            var content = await httpResponse.Content.ReadAsStringAsync();
            var user = JsonConvert.DeserializeObject<User>(content);

            Assert.True(user.Login == "pp");
        }

    }
}
