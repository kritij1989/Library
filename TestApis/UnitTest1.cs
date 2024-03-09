using Library;
using Library.Controllers;
using Library.Entities;
using Library.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;
using System.Security.Cryptography.Xml;
using Newtonsoft.Json;
using Microsoft.Extensions.DependencyInjection;
using System;
using Moq;
using Microsoft.AspNetCore.SignalR;
using Moq.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using System.Reflection.Metadata;
using System.Diagnostics.Contracts;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace TestApis
{
    public class Tests
    {
        private readonly ILibrary _library;
        private LibraryController _controller;
        private readonly BookMaster _table1 = new BookMaster()
        {
            Authorname = "Test",
            BookName = "TestAutomation",
            Quantity = 1
        };
        public Tests()
        {
            List<BookMaster> expectedBooks = GetbooksData();
            List<BookMaster> expectedReaders = GetbooksData();

            _library = new LibraryClass();

        }
        /// <summary>
        /// initialize Books
        /// </summary>
        /// <returns></returns>
        private static List<BookMaster> GetbooksData()
        {
            List<BookMaster> expected = new List<BookMaster>();
            BookMaster book1 = new BookMaster();
            book1.Authorname = "Test Author1";
            book1.BookName = "Test Books";
            book1.Quantity = 1;

            BookMaster book2 = new BookMaster();
            book2.Authorname = "Test Author2";
            book2.BookName = "Test Books2";
            book2.Quantity = 2;
            expected.Add(book1);
            expected.Add(book2);
            return expected;
        }
        /// <summary>
        ///  initialize Readers
        /// </summary>
        /// <returns></returns>
        private static List<ReaderInfo> GetReadersData()
        {
            List<ReaderInfo> expected = new List<ReaderInfo>();
            ReaderInfo reader1 = new ReaderInfo();
            reader1.ReaderName = "Test Reader1";
            reader1.ReaderAddress = "Address1";
            ReaderInfo reader2 = new ReaderInfo();
            reader2.ReaderName = "Test reader2";
            reader2.ReaderAddress = "Address1";
            expected.Add(reader1);
            expected.Add(reader2);
            return expected;
        }

        [SetUp]
        public void Setup()
        {
        }
        /// <summary>
        /// Test to get books
        /// </summary>

        [Test]
        public void Test_GetBooks()
        {
            Moq.Mock<LibraryDbContext> libContextMock = new Mock<LibraryDbContext>();
            libContextMock.Setup<DbSet<BookMaster>>(x => x.BookMasters)
                .ReturnsDbSet(GetbooksData());
            _controller = new LibraryController(libContextMock.Object, _library);

            List<BookMaster> data = JsonConvert.DeserializeObject<List<BookMaster>>(_controller.GetBooks());
            Assert.AreEqual(data.Count(), 2);
        }
        /// <summary>
        /// Test to get Readers
        /// </summary>
        [Test]
        public void Test_GetReaders()
        {
            Moq.Mock<LibraryDbContext> libContextMock = new Mock<LibraryDbContext>();
            libContextMock.Setup<DbSet<ReaderInfo>>(x => x.ReaderInfos)
                .ReturnsDbSet(GetReadersData());
            _controller = new LibraryController(libContextMock.Object, _library);

            List<ReaderInfo> data = JsonConvert.DeserializeObject<List<ReaderInfo>>(_controller.GetReaders());
            Assert.AreEqual(data.Count(), 2);
        }
        /// <summary>
        /// Test to validate validations
        /// </summary>
        [Test]
        public void Book_isValidData()
        {
            BookMaster add = new BookMaster
            {
               BookName = "Test",
               Quantity = 1
            };
            var lstErrors = ValidateModel(add);
            Assert.IsTrue(lstErrors.Count() > 0);
        }
        
        private IList<ValidationResult> ValidateModel(object model)
        {
            var validationResults = new List<ValidationResult>();
            var ctx = new ValidationContext(model, null, null);
            Validator.TryValidateObject(model, ctx, validationResults, true);
            return validationResults;
        }
        /// <summary>
        /// Test new entry of books
        /// </summary>
        [Test]
        public void Test_AddBook()
        {
            var settings = new List<BookMaster>();
            BookMaster book = new BookMaster();
            book.Quantity = 1;
            book.BookName = "Test";
            book.Authorname = "Test";

            Moq.Mock<LibraryDbContext> libContextMock = new Mock<LibraryDbContext>();
           
            libContextMock.Setup(m => m.BookMasters.Add(It.IsAny<BookMaster>())).Callback<BookMaster>(s => {
                settings.Add(book);
            });
            
           
            _controller = new LibraryController(libContextMock.Object, _library);
           
            string data1 = JsonConvert.DeserializeObject<string>(_controller.AddBook(book));
            Assert.AreEqual(data1, "Success");
        }


        /// <summary>
        /// Test new entry of reader
        /// </summary>
        [Test]
        public void Test_AddReader()
        {

            var settings = new List<ReaderInfo>();
            ReaderInfo reader = new ReaderInfo();
            reader.ReaderName = "Test";
            reader.ReaderAddress = "test1";

            Moq.Mock<LibraryDbContext> libContextMock = new Mock<LibraryDbContext>();
            libContextMock.Setup(m => m.ReaderInfos.Add(It.IsAny<ReaderInfo>())).Callback<ReaderInfo>(s => {
                settings.Add(reader);
            });


            _controller = new LibraryController(libContextMock.Object, _library);

            string data1 = JsonConvert.DeserializeObject<string>(_controller.AddReader(reader));
            Assert.AreEqual(data1,"0");

        }
       
    }
}