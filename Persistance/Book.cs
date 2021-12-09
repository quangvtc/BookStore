using System;

namespace Persistance
{
    public static class BookStatus
    {
        public const int NOT_ACTIVE = 0;
        public const int ACTIVE = 1;
    }

    public class Book
    {
        public int BookId { set; get; }
        public string BookName { set; get; }
        public string Author { set; get; }
        public string Category { set; get; }
        public decimal BookPrice { set; get; }
        public int? Amount { set; get; }
        public int? Status { set; get; }
        public string Description { set; get; }

        public override bool Equals(object obj)
        {
            if (obj is Book)
            {
                return ((Book)obj).BookId.Equals(BookId);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return BookId.GetHashCode();
        }

        public override string ToString(){
            string output = "-----Book Detail-----\n";
            output += $"Book ID     : {BookId}\n";
            output += $"Book Name   : {BookName}\n";
            output += $"Author  : {Author}\n";
            output += $"Category : {Category}\n";
            output += $"Book Price : {BookPrice} $\n";
            output += $"Description : {Description}\n";
            output += $"Amount    : {Amount}\n";
            output += $"Status  : {Status}\n";
            return output;
        }
    }
}