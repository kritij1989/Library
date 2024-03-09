using Library.Entities;

namespace Library.Interface
{
    public interface ILibrary
    {
        public string GetBooks();
        public void GetReaders();
        public string AddBook(BookMaster book);
        public string AddReader(ReaderInfo reader);
        public string AddEntryOfBook(Table data);
        public string ReturnBook(Table data);
        public string SearchBook(string data);
    }
}
