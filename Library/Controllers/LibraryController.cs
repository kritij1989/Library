using Library.Entities;
using Library.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.Json.Nodes;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Library.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class LibraryController : ControllerBase
    {
        private LibraryDbContext libraryDbContext;
        private readonly ILibrary lib;

        public LibraryController(LibraryDbContext db, ILibrary libraray)
        {
            libraryDbContext = db;
            lib = libraray;

        }
        /// <summary>
        /// Return all book of Library
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("Books")]
        public string GetBooks()
        {
            return JsonConvert.SerializeObject(libraryDbContext.BookMasters.ToList());
        }
        /// <summary>
        ///  Return all readers
        /// </summary>
        /// <returns></returns>
        [HttpGet]

        [Route("Readers")]
        public string GetReaders()
        {
            return JsonConvert.SerializeObject(libraryDbContext.ReaderInfos.ToList());
        }

        [HttpPost]
        [Route("BookInStock")]
        public string BookInStock(int data)
        {
            if (libraryDbContext.BookMasters.Where(p => p.Id == data).FirstOrDefault().RemainingQty > 0)
            {
                return JsonConvert.SerializeObject("True");
            }
            return JsonConvert.SerializeObject("False");
        }
        /// <summary>
        /// Add new book
        /// </summary>
        /// <param name="book"></param>
        /// <returns></returns>        
        [HttpPost]
        [Route("AddBook")]        
        public string AddBook(BookMaster book)
        {
            if (ModelState.IsValid)
            {
                book.RemainingQty = book.Quantity;
                libraryDbContext.BookMasters.Add(book);

                libraryDbContext.SaveChanges();
                return JsonConvert.SerializeObject("Success");
            }

            string message = string.Join(" | ", ModelState.Values
        .SelectMany(v => v.Errors)
        .Select(e => e.ErrorMessage));
            return JsonConvert.SerializeObject(message);
        }
        /// <summary>
        /// Addn new reader
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("AddReader")]
        public string AddReader(ReaderInfo reader)
        {
            if (ModelState.IsValid)
            {
                libraryDbContext.ReaderInfos.Add(reader);
                libraryDbContext.SaveChanges();
                return JsonConvert.SerializeObject(reader.Id);
            }

            string message = string.Join(" | ", ModelState.Values
        .SelectMany(v => v.Errors)
        .Select(e => e.ErrorMessage));
            return JsonConvert.SerializeObject(message);
        }
        /// <summary>
        /// Check book avilable or not
        /// </summary>
        /// <param name="books"></param>
        /// <returns></returns>
        private string CheckBooks(List<ENtryDetails> books)
        {
            string message = string.Empty;
            foreach (var c in books)
            {
                BookMaster book = libraryDbContext.BookMasters.Where(p => p.Id == c.BookId).FirstOrDefault();
                if (book.RemainingQty == 0)
                {
                    message += book.BookName + ",";
                }
            }
            return message;

        }
        /// <summary>
        /// add entry of book rented
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("AddEntryOfBook")]
        public string AddEntryOfBook(List<ENtryDetails> data)
        {
            string message = CheckBooks(data);
            if (message != string.Empty)
            {
                message += "out of stock";
                return JsonConvert.SerializeObject(message);
            }
           
            foreach (var c in data)
            {
                Table t1 = new Table();
                t1.BookId = c.BookId;
                t1.ReaderId = c.ReaderId;
                BookMaster book= libraryDbContext.BookMasters.Where(p => p.Id == c.BookId).FirstOrDefault();
                t1.Book = book;
                t1.Reader = libraryDbContext.ReaderInfos.Where(p => p.Id == c.ReaderId).FirstOrDefault();
                libraryDbContext.Tables.Add(t1);                
                book.RemainingQty = book.RemainingQty - 1;
            }

            libraryDbContext.SaveChanges();
            return JsonConvert.SerializeObject("success");
        }


        [HttpPost]
        [Route("ReturnBook")]
        public string ReturnBook(Table data)
        {
            if (ModelState.IsValid)
            {
                Table table = libraryDbContext.Tables.Where(p => p.BookId == data.BookId && p.ReaderId == data.ReaderId).FirstOrDefault();
                if (table != null)
                {
                    libraryDbContext.Tables.Remove(table);
                }
                BookMaster book = libraryDbContext.BookMasters.Where(p => p.Id == data.BookId).FirstOrDefault();
                book.RemainingQty = book.RemainingQty + 1;
                libraryDbContext.SaveChanges();
                return JsonConvert.SerializeObject("Succcess");
            }

            string message = string.Join(" | ", ModelState.Values
        .SelectMany(v => v.Errors)
        .Select(e => e.ErrorMessage));
            return JsonConvert.SerializeObject(message);
        }
        [HttpPost]
        [Route("SearchBook")]
        public string SearchBook(string data)
        {
            var result = libraryDbContext.BookMasters.Where(p => p.BookName.Contains(data) || p.Authorname.Contains(data)).ToList();
            return JsonConvert.SerializeObject(result);
        }
        [HttpGet]
        [Route("GetReaderInfo")]
        public string GetReaderInfo(string name, string address)
        {
            var result = libraryDbContext.ReaderInfos.Where(p => p.ReaderName == name || p.ReaderAddress == address).FirstOrDefault();
            return JsonConvert.SerializeObject(result);
        }
    }
}