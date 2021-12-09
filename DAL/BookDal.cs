using System;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using Persistance;

namespace DAL
{
    public static class BookFilter{
        public const int GET_ALL = 0;
        public const int FILTER_BY_BOOK_NAME = 1;
    }
    public class BookDal{
        private string query;
        private MySqlConnection connection = DbConfig.GetConnection();

        public Book GetBookById(int bookId)
        {
            Book book = null;
            try
                {
                connection.Open();
                query = @"select book_id, book_name, unit_price, amount,author, category, book_status,
                        ifnull(book_description, '') as book_description
                        from Books where book_id=@bookId;";
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@bookId", bookId);
                MySqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                book = GetBook(reader);
                }
                reader.Close();
            }
            catch { }
            finally { connection.Close(); }
            return book;
        }
        private Book GetBook(MySqlDataReader reader)
        {
            Book book = new Book();
            book.BookId = reader.GetInt32("book_id");
            book.BookName = reader.GetString("book_name");
            book.BookPrice = reader.GetDecimal("unit_price");
            book.Amount = reader.GetInt32("amount");
            book.Author = reader.GetString("author");
            book.Category = reader.GetString("category");
            book.Status = reader.GetInt16("book_status");
            book.Description = reader.GetString("book_description");
            return book;
        }
        public List<Book> GetBooks(int bookFilter, Book book)
        {
            List<Book> lst = null;
            try
            {
                connection.Open();
                MySqlCommand command = new MySqlCommand("", connection);
                switch (bookFilter)
                {
                    case BookFilter.GET_ALL:
                        query = @"select book_id, book_name, unit_price, amount, author, category, book_status, ifnull(book_description, '') as book_description from Books";
                        break;
                    case BookFilter.FILTER_BY_BOOK_NAME:
                        query = @"select book_id, book_name, unit_price, amount, author, category, book_status, ifnull(book_description, '') as book_description from Books
                                where book_name like concat('%',@bookName,'%');";
                        command.Parameters.AddWithValue("@bookName", book.BookName);
                        break;
                }
                command.CommandText = query;
                MySqlDataReader reader = command.ExecuteReader();
                lst = new List<Book>();
                while (reader.Read())
                {
                lst.Add(GetBook(reader));
                }
                reader.Close();
            }
            catch(Exception ex) {
                Console.WriteLine(ex);
            }
            finally
            {
                connection.Close();
            }
            return lst;
        }
    }
}