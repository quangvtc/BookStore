using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using Persistance;

namespace DAL
{
  public static class CustomerFilter{
        public const int GET_ALL = 0;
        public const int FILTER_BY_CUSTOMER_NAME = 1;
  }
  public class CustomerDal
  {
    private string query;
    private MySqlConnection connection = DbConfig.GetConnection();

    public Customer GetById(int customerId)
    {
      Customer c = null;
      try
      {
        connection.Open();
        query = @"select customer_id, customer_name,
                        ifnull(customer_address, '') as customer_address,
                        ifnull(customer_phone, '') as customer_phone
                        from Customers where customer_id=" + customerId + ";";
        MySqlCommand command = new MySqlCommand(query, connection);
        MySqlDataReader reader = command.ExecuteReader();
        if (reader.Read())
        {
          c = GetCustomer(reader);
        }
        reader.Close();
      }
      catch { }
      finally
      {
        connection.Close();
      }
      return c;
    }

    public List<Customer> GetCustomers(int customerFilter, Customer customer)
        {
            List<Customer> lst = null;
            try
            {
                connection.Open();
                MySqlCommand command = new MySqlCommand("", connection);
                switch (customerFilter)
                {
                    case CustomerFilter.GET_ALL:
                        query = @"select * from Customers;";
                        break;
                    case CustomerFilter.FILTER_BY_CUSTOMER_NAME:
                        query = @"select * from Customers
                                where customer_name like concat('%',@customer_name,'%');";
                        command.Parameters.AddWithValue("@customer_name", customer.CustomerName);
                        break;
                }
                command.CommandText = query;
                MySqlDataReader reader = command.ExecuteReader();
                lst = new List<Customer>();
                while (reader.Read())
                {
                lst.Add(GetCustomer(reader));
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

    public Customer GetCustomer(MySqlDataReader reader)
    {
      Customer c = new Customer();
      c.CustomerId = reader.GetInt32("customer_id");
      c.CustomerName = reader.GetString("customer_name");
      c.CustomerAddress = reader.GetString("customer_address");
      c.CustomerPhone = reader.GetString("customer_phone");
      return c;
    }

    public int? AddCustomer(Customer c)
    {
      int? result = null;
      if (connection.State == System.Data.ConnectionState.Closed)
      {
        connection.Open();
      }
      MySqlCommand cmd = new MySqlCommand("sp_createCustomer", connection);
      try
      {
        cmd.CommandType = System.Data.CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@customerName", c.CustomerName);
        cmd.Parameters["@customerName"].Direction = System.Data.ParameterDirection.Input;
        cmd.Parameters.AddWithValue("@customerAddress", c.CustomerAddress);
        cmd.Parameters["@customerAddress"].Direction = System.Data.ParameterDirection.Input;
        cmd.Parameters.AddWithValue("@customerPhone",c.CustomerPhone);
        cmd.Parameters["@customerPhone"].Direction = System.Data.ParameterDirection.Input;
        cmd.Parameters.AddWithValue("@customerId", MySqlDbType.Int32);
        cmd.Parameters["@customerId"].Direction = System.Data.ParameterDirection.Output;
        cmd.ExecuteNonQuery();
        result = (int)cmd.Parameters["@customerId"].Value;
      }
      catch
      {
      }
      finally
      {
        connection.Close();
      }
      return result;
    }
  }
}