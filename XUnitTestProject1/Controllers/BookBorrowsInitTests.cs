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
    public class BookBorrowsInitTests
    {
        private readonly TestServer _server;
        private readonly HttpClient _client;


        public BookBorrowsInitTests()
        {
            _server = ServerFactory.GetServerInstance();
            _client = _server.CreateClient();


            using (var scope = _server.Host.Services.CreateScope())
            {
                var _db = scope.ServiceProvider.GetRequiredService<LibraryContext>();

                _db.BookBorrow.Add(new BookBorrow
                {
                    IdUser = 1,
                    IdBook = 1,
                    Comments = "ok"
                });

                _db.SaveChanges();
            }
        }


        [Fact]
        public async Task AddBookBorrow_200Ok()
        {
            //Arrange i Act

            var cc = new TestServer(new WebHostBuilder()
                     .UseContentRoot(Directory.GetCurrentDirectory())
                   .UseStartup<TestStartup>()).CreateClient();

            var request = new
            {
                Url = "/api/book-borrows",
                Body = new
                {
                    IdUser = 2,
                    IdBook = 2,
                    Comments = "okk"
                }
            };

            var httpResponse = await cc.PostAsync(request.Url, new StringContent(JsonConvert.SerializeObject(request.Body), Encoding.Default, "application/json"));

            httpResponse.EnsureSuccessStatusCode();

            var content = await httpResponse.Content.ReadAsStringAsync();
            var bookBorrow = JsonConvert.DeserializeObject<BookBorrow>(content);

            Assert.True(bookBorrow.Comments == "okk");
        }

        [Fact]
        public async Task ChangeBookBorrow_200Ok()
        {
            //Arrange i Act

            var cc = new TestServer(new WebHostBuilder()
                     .UseContentRoot(Directory.GetCurrentDirectory())
                   .UseStartup<TestStartup>()).CreateClient();

            var request = new
            {
                Url = "/api/book-borrows/1",
                Body = new
                {
                    Comments = "newComment"
                }
            };

            var httpResponse = await cc.PutAsync(request.Url, new StringContent(JsonConvert.SerializeObject(request.Body), Encoding.Default, "application/json"));

            httpResponse.EnsureSuccessStatusCode();

            //zauwazylem, ze w klasie BookBorowRepository metoda ChangeBookBorrow zwraca po prostu true, więc nie wiem 
            //do czego skonwertować responsa i do czego przyrównać.

           
            
            //var content = await httpResponse.Content.ReadAsStringAsync();
            //var bookBorrow = JsonConvert.DeserializeObject<BookBorrow>(content);

            //Assert.True(bookBorrow.Comments == "okk");
        }

    }
}
