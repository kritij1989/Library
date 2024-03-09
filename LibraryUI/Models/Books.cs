namespace LibraryUI.Models
{
    public class Books
    {
        public int Id { get; set; }


        public string BookName { get; set; } = null!;


        public string Authorname { get; set; } = null!;


        public int Quantity { get; set; }
        public int RemainingQty { get; set; }

    }
}
