using LibraryUI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net;
using Newtonsoft.Json;
using System.Text;
using Microsoft.AspNetCore.Http;
using static System.Reflection.Metadata.BlobBuilder;
using System.Net.Http;
using System.Diagnostics.Eventing.Reader;
using System.Xml.Linq;
using System.Data;
using System.IO.Pipelines;
using System.Net.Http.Headers;

namespace LibraryUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        public string SessionName = "BookCount";
        public string SessionReader = "ReaderId";
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        private int GetBookCount()
        {
            var records = HttpContext.Session.GetString(SessionName);
            List<int> Booksids = new List<int>();
            if (records != null)
            {
                Booksids = JsonConvert.DeserializeObject<List<int>>(records);

            }
            return Booksids.Count();
        }
       
        private List<Books> GetBooks()
        {
            string json = new WebClient().DownloadString("http://localhost:5127/api/Library/Books");
            List<Books> data = JsonConvert.DeserializeObject<List<Books>>(json);
            ViewBag.TotalBooks = GetBookCount();
            return data;
        }
        private List<Reader> GetReaders()
        {

            string json = new WebClient().DownloadString("http://localhost:5127/api/Library/Readers");
            List<Reader> data = JsonConvert.DeserializeObject<List<Reader>>(json);
            ViewBag.TotalBooks = GetBookCount();
            return data;
        }
        private int GetReaderInfo(string name, string address)
        {
            var url = "http://localhost:5127/api/Library/GetReaderInfo?name=" + name + "&address=" + address;
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            var response = client.Send(request);
            string json = response.Content.ReadAsStringAsync().Result;
            Reader data = JsonConvert.DeserializeObject<Reader>(json);
            return data.Id;
        }

        [HttpGet]
        public ActionResult Books()
        {
            ViewBag.TotalBooks = GetBookCount();

            return View(GetBooks());
        }
        [HttpPost]
        public ActionResult Books(IFormCollection form)
        {
            ViewBag.message = string.Empty;
            try
            {
                string name = Request.Form["search"];
                var url = "http://localhost:5127/api/Library/SearchBook?data=" + name;
                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, url);
                client.DefaultRequestHeaders.Authorization =
     new AuthenticationHeaderValue("Role", "Admin");
                var response = client.Send(request);
                string json = response.Content.ReadAsStringAsync().Result;
                List<Books> data = JsonConvert.DeserializeObject<List<Books>>(json);
                ViewBag.TotalBooks = GetBookCount();

                return View(data);

            }
            catch (Exception ex)
            {
                ViewBag.message = ex.Message;
                ViewBag.TotalBooks = GetBookCount();
                return View(GetBooks());
            }


        }
        [HttpGet]
        public ActionResult AddBook()
        {
            ViewBag.message = "";
            ViewBag.TotalBooks = GetBookCount();
            return View(GetBooks());
        }
        [HttpPost]
        public ActionResult AddBook(IFormCollection form)
        {
            string message = string.Empty;
            try
            {
                Books book = new Books();
                book.BookName = Request.Form["bookname"];
                book.Authorname = Request.Form["authorname"];
                book.Quantity = Convert.ToInt32(Request.Form["quantity"]);

                string url = "http://localhost:5127/api/Library/AddBook";
                
                string json = JsonConvert.SerializeObject(book);               
                String result = PostData(url, json);
                string m = JsonConvert.DeserializeObject<string>(result);
                if (m == "Success")
                {
                    message = "Added Successfully";
                }
                else
                {
                    message = m;
                }

            }
            catch (Exception ex)
            {
                message = "Incorrect data or may be server is down";
            }

            ViewBag.message = message;
            ViewBag.TotalBooks = GetBookCount();
            return View(GetBooks());
        }
        [HttpGet]
        public ActionResult AddReader()
        {
            ViewBag.message = "";
            ViewBag.TotalBooks = GetBookCount();
            return View(GetReaders());
        }
        [HttpGet]
        public ActionResult Readers()
        {
            ViewBag.TotalBooks = GetBookCount();
            return View(GetReaders());
        }
        [HttpPost]
        public ActionResult Readers(IFormCollection form)
        {
            ViewBag.TotalBooks = GetBookCount();
            return View();
        }
        private void AddEntry()
        {

        }
        private string PostData(string url,string json)
        {
           
            HttpClient client = new HttpClient();
            
            var response = client.PostAsync(url, new StringContent(json, Encoding.UTF8, "application/json")).Result;
            String result = response.Content.ReadAsStringAsync().Result;
            return result;

        }
        [HttpPost]
        public ActionResult AddReader(IFormCollection form)
        {
            string message = string.Empty;
            try
            {
                Reader reader = new Reader();
                reader.ReaderName = Request.Form["name"];
                reader.ReaderAddress = Request.Form["address"];
                string url = "http://localhost:5127/api/Library/AddReader";                
                string json = JsonConvert.SerializeObject(reader);               
                String result = PostData(url, json);
                int readerId = 0;
                try
                {
                    readerId = JsonConvert.DeserializeObject<int>(result);
                    HttpContext.Session.SetInt32(SessionReader, readerId);

                }
                catch (Exception ex1)
                {
                    message = ex1.Message;
                }
            }
            catch (Exception ex)
            {
                message = "Error Occured";
            }

            ViewBag.message = message;
            ViewBag.TotalBooks = GetBookCount();
            return View(GetReaders());
        }

        [HttpGet]
        public ActionResult AddEntryOfBook()
        {
            ViewBag.TotalBooks = GetBookCount();
            return View();
        }
        [HttpPost]
        public ActionResult AddEntryOfBook(FormCollection form)
        {
            string message = string.Empty;
           
            ViewBag.message = "";
            ViewBag.TotalBooks = GetBookCount();
            return View(GetReaders());
        }

        [HttpPost]
        public ActionResult AddCart(int data)
        {
            var records = HttpContext.Session.GetString(SessionName);
            List<int> Booksids = new List<int>();
            if (records != null)
            {
                Booksids = JsonConvert.DeserializeObject<List<int>>(records);

            }
            if (Booksids.Contains(data))
            {
                return Json(new { Message = "Book already exists in cart" });

            }
            if(Booksids.Count()==5)
            {
                return Json(new { Message = "Only 5 books can be added." });
            }
            var url = "http://localhost:5127/api/Library/BookInStock?data=" + data;
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            var response = client.Send(request);
            string json = response.Content.ReadAsStringAsync().Result;
            bool result = JsonConvert.DeserializeObject<bool>(json);
            if (!result)
            {
                return Json(new { Message = "Book is out of stock" });
            }
            Booksids.Add(data);
            var serializedRecords = JsonConvert.SerializeObject(Booksids);
            HttpContext.Session.SetString(SessionName, serializedRecords);
            ViewBag.TotalBooks = Booksids.Count();
            return Json(new { Message = "Added successfully" });
        }
        [HttpPost]
        public ActionResult Delete(int id)
        {
            var records = HttpContext.Session.GetString(SessionName);
            List<int> Booksids = new List<int>();
            if (records != null)
            {
                Booksids = JsonConvert.DeserializeObject<List<int>>(records);

            }
            Booksids.Remove(id);

            var serializedRecords = JsonConvert.SerializeObject(Booksids);
            HttpContext.Session.SetString(SessionName, serializedRecords);
            ViewBag.TotalBooks = Booksids.Count();
            return Json(new { Message = "Removed successfully" });
        }
        private List<Table> GetTables(List<int> bookids, int readerId)
        {
            List<Table> data = new List<Table>();

            foreach (int c in bookids)
            {
                var table = new Table();
                table.ReaderId = readerId;
                table.BookId = c;
                data.Add(table);
            }
            return data;
        }
        [HttpGet]
        public ActionResult Review()
        {
            ViewBag.TotalBooks = GetBookCount();
            var records = HttpContext.Session.GetString(SessionName);
            List<int> Booksids = new List<int>();
            if (records != null)
            {
                Booksids = JsonConvert.DeserializeObject<List<int>>(records);
            }
            return View(GetBooks().Where(p => Booksids.Contains(p.Id)).ToList())
            ;
        }
        [HttpPost]
        public ActionResult CheckOut()
        {
            var records = HttpContext.Session.GetString(SessionName);
            List<int> Booksids = new List<int>();
            if (records != null)
            {
                Booksids = JsonConvert.DeserializeObject<List<int>>(records);

            }
            else
            {
                return Json(new { Message = "Please add books" });
            }
            if (Booksids.Count() == 0)
            {
                return Json(new { Message = "Please add books" });

            }
            var readerid = HttpContext.Session.GetInt32(SessionReader);
            if (readerid == null)
            {
                return Json(new { Message = "Please add reader" });

            }
            List<Table> data = GetTables(Booksids, Convert.ToInt32(readerid));
            string json = JsonConvert.SerializeObject(data);

            string url = "http://localhost:5127/api/Library/AddEntryOfBook";
           
            String result =PostData(url, json);
            string m = JsonConvert.DeserializeObject<string>(result);

            if (m == "success")
            {
                HttpContext.Session.SetString(SessionName, null);
                HttpContext.Session.SetString(SessionReader, null);
                return Json(new { Message = "Order Placed Successfully" });
            }
            else
            {
                return Json(new { result });
            }

        }
    }
}