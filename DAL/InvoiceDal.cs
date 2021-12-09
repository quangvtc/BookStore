using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using Persistance;

namespace DAL
{
  public class InvoiceDal
  {
    private string query;
    private MySqlConnection connection = DbConfig.GetConnection();
    public bool CreateInvoice(Invoice invoice)
    {
      if (invoice == null || invoice.BooksList == null || invoice.BooksList.Count == 0)
      {
        return false;
      }
      bool result = false;
      try
      {
        connection.Open();
        MySqlCommand cmd = connection.CreateCommand();
        cmd.Connection = connection;
        //Lock update all tables
        cmd.CommandText = "lock tables Customers write, Invoices write, Books write, InvoiceDetails write;";
        cmd.ExecuteNonQuery();
        MySqlTransaction trans = connection.BeginTransaction();
        cmd.Transaction = trans;
        MySqlDataReader reader = null;
        if (invoice.InvoiceCustomer == null || invoice.InvoiceCustomer.CustomerName == null || invoice.InvoiceCustomer.CustomerName == "")
        {
          //set default customer with customer id = 1
          invoice.InvoiceCustomer = new Customer() { CustomerId = 1 };
        }
        try
        {
          if (invoice.InvoiceCustomer.CustomerId < 1)
          {
            //Insert new Customer
            cmd.CommandText = @"insert into Customers(customer_name, customer_address,customer_phone)
                            values ('" + invoice.InvoiceCustomer.CustomerName + "','" + (invoice.InvoiceCustomer.CustomerAddress ?? "") + "','" + (invoice.InvoiceCustomer.CustomerPhone ?? "") + "');";
            cmd.ExecuteNonQuery();
            //Get new customer id
            cmd.CommandText = "select customer_id from Customers order by customer_id desc limit 1;";
            reader = cmd.ExecuteReader();
            if (reader.Read())
            {
              invoice.InvoiceCustomer.CustomerId = reader.GetInt32("customer_id");
            }
            reader.Close();
          }
          else
          {
            //get Customer by Id
            cmd.CommandText = "select * from Customers where customer_id=@customerId;";
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@customerId", invoice.InvoiceCustomer.CustomerId);
            reader = cmd.ExecuteReader();
            if (reader.Read())
            {
              invoice.InvoiceCustomer = new CustomerDal().GetCustomer(reader);
            }
            reader.Close();
          }
          if (invoice.InvoiceCustomer == null || invoice.InvoiceCustomer.CustomerId < 1)
          {
            throw new Exception("Can't find Customer!");
          }
          //Insert Invoice
          cmd.CommandText = "insert into Invoices(customer_id, invoice_status) values (@customerId, @invoiceStatus);";
          cmd.Parameters.Clear();
          cmd.Parameters.AddWithValue("@customerId", invoice.InvoiceCustomer.CustomerId);
          cmd.Parameters.AddWithValue("@invoiceStatus", InvoiceStatus.CREATE_NEW_INVOICE);
          cmd.ExecuteNonQuery();
          //get new Invoice_ID
          cmd.CommandText = "select LAST_INSERT_ID() as invoice_id";
          reader = cmd.ExecuteReader();
          if (reader.Read())
          {
            invoice.InvoiceId = reader.GetInt32("invoice_id");
          }
          reader.Close();

          //insert Order Details table
          foreach (var book in invoice.BooksList)
          {
            if (book.BookId < 1 || book.Amount <= 0)
            {
              throw new Exception("Not Exists Item");
            }
            //get unit_price
            cmd.CommandText = "select unit_price from Books where book_id=@bookId";
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@bookId", book.BookId);
            reader = cmd.ExecuteReader();
            if (!reader.Read())
            {
              throw new Exception("Not Exists Item");
            }
            book.BookPrice = reader.GetDecimal("unit_price");
            reader.Close();

            //insert to Invoice Details
            cmd.CommandText = @"insert into InvoiceDetails(invoice_id, book_id, unit_price, quantity) values 
                            (" + invoice.InvoiceId + ", " + book.BookId + ", " + book.BookPrice + ", " + book.Amount + ");";
            cmd.ExecuteNonQuery();

            //update amount in Items
            cmd.CommandText = "update Books set amount=amount-@quantity where book_id=" + book.BookId + ";";
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@quantity", book.Amount);
            cmd.ExecuteNonQuery();
          }
          //commit transaction
          trans.Commit();
          result = true;
        }
        catch(Exception ex)
        {
          Console.WriteLine(ex);
          try
          {
            trans.Rollback();
          }
          catch { }
        }
        finally
        {
          //unlock all tables;
          cmd.CommandText = "unlock tables;";
          cmd.ExecuteNonQuery();
        }
      }
      catch { }
      finally
      {
        connection.Close();
      }
      return result;
    }

    public Invoice GetInvoiceId(int id){
      Invoice invoices = null;
      lock(connection){
        try{
          connection.Open();
          query = @"select i.invoice_id, c.customer_name, i.invoice_date, i.invoice_status from Invoices i inner join Customers c on i.customer_id = c.customer_id where i.invoice_status = 1 and invoice_id = " + id + ";";
          MySqlCommand command = new MySqlCommand(query, connection);
          MySqlDataReader reader = command.ExecuteReader();
          if(reader.Read()){
            invoices = GetInvoice(reader);
            reader.Close();
            query = @"select i.invoice_id, b.book_name, i.unit_price, i.quantity from InvoiceDetails i inner join Books b on i.book_id = b.book_id where i.invoice_id =" + id + ";";
            command.CommandText = query;
            reader = command.ExecuteReader();
            while(reader.Read()){
              Book book = new Book();
              book.BookName = reader.GetString("book_name");
              book.BookPrice = reader.GetDecimal("unit_price");
              book.Amount = reader.GetInt32("quantity");
              invoices.BooksList.Add(book);
            }
          }
          reader.Close();
        }catch(Exception ex){
          Console.WriteLine(ex);
        }
        finally{
           connection.Close();
        }
      }
      return invoices;
    }

    public List<Invoice> GetInvoices(){
      List<Invoice> invoices = null;
      try{
        connection.Open();
        query = @"select i.invoice_id, c.customer_name, i.invoice_date, i.invoice_status from Invoices i inner join Customers c on i.customer_id = c.customer_id where i.invoice_status = 1;";
        MySqlCommand command = new MySqlCommand(query, connection);
        MySqlDataReader reader = command.ExecuteReader();
        invoices = new List<Invoice>();
        while (reader.Read())
        {
          invoices.Add(GetInvoice(reader));
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
      return invoices;
    }
    public bool PayMent(int id){
      bool isSuccess = false;
      try{
        connection.Open();
        query = "update Invoices set invoice_status = 2 where invoice_id = "+ id +";";
        MySqlCommand command = new MySqlCommand(query, connection);
        command.ExecuteNonQuery();
        isSuccess = true;
      }catch(Exception ex){
        Console.WriteLine(ex);
      }
      finally {
        connection.Close();
      }
      return isSuccess;
    }

    private Invoice GetInvoice(MySqlDataReader reader)
    {
      Invoice invoice = new Invoice();
      invoice.InvoiceId = reader.GetInt32("invoice_id");
      invoice.InvoiceDate = reader.GetDateTime("invoice_date");
      invoice.Status = reader.GetInt32("invoice_status");
      invoice.InvoiceCustomer.CustomerName = reader.GetString("customer_name");
      return invoice;
    }
  }
}