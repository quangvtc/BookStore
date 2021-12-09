using System;
using System.Collections.Generic;
using Persistance;
using DAL;

namespace BL
{
    public class CustomerBl
    {
        private CustomerDal cdal = new CustomerDal();
        public Customer GetById(int customerId)
        {
            return cdal.GetById(customerId);
        }
        public List<Customer> GetAll()
        {
            return cdal.GetCustomers(CustomerFilter.GET_ALL, null);
        }
        public List<Customer> GetByName(string customerName)
        {
            return cdal.GetCustomers(CustomerFilter.FILTER_BY_CUSTOMER_NAME, new Customer{CustomerName=customerName});
        }

        // public int AddCustomer(Customer customer)
        // {
        //     return CustomerDal.AddCustomer(customer) ?? 0;
        // }
    }
}