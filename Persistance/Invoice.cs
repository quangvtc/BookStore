using System;
using System.Collections.Generic;

namespace Persistance
{

    public static class InvoiceStatus
    {
        public const int CREATE_NEW_INVOICE = 1;
        public const int INVOICE_INPROGRESS = 2;
    }

    public class Invoice
    {
        public int InvoiceId { set; get; }
        public DateTime InvoiceDate { set; get; }
        public Customer InvoiceCustomer { set; get; }
        public int? Status {set;get;}
        public List<Book> BooksList { set; get; }

        public Book this[int index]
        {
            get
            {
                if (BooksList == null || BooksList.Count == 0 || index < 0 || BooksList.Count < index) return null;
                return BooksList[index];
            }
            set
            {
                if (BooksList == null) BooksList = new List<Book>();
                BooksList.Add(value);
            }
        }

        public Invoice()
        {
            BooksList = new List<Book>();
            InvoiceCustomer = new Customer();
        }

        public override bool Equals(object obj){
            if(obj is Invoice){
                return ((Invoice)obj).InvoiceId.Equals(InvoiceId);
            }
            return false;
        }

        public override int GetHashCode(){
            return InvoiceId.GetHashCode();
        }

    }
}