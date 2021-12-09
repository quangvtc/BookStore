using System.Collections.Generic;
using Persistance;
using DAL;

namespace BL
{
    public class BookBl
    {
        private BookDal bdal = new BookDal();
        public Book GetBookById(int bookId)
        {
            return bdal.GetBookById(bookId);
        }
        public List<Book> GetAll()
        {
            return bdal.GetBooks(BookFilter.GET_ALL, null);
        }
        public List<Book> GetByName(string bookName)
        {
            return bdal.GetBooks(BookFilter.FILTER_BY_BOOK_NAME, new Book{BookName=bookName});
        }
        
    }
}
