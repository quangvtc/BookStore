using System;
using System.Collections.Generic;
using Persistance;
using DAL;

namespace BL
{
    public class InvoiceBl
    {
        private InvoiceDal idl = new InvoiceDal();
        public bool CreateInvoice(Invoice invoice)
        {
            bool result = idl.CreateInvoice(invoice);
            return result;
        }
        public Invoice GetInvoiceId(int id){
            return idl.GetInvoiceId(id);
        }
        public List<Invoice> GetInvoices(){
            return idl.GetInvoices();
        }
        public bool PayMent(int id){
            bool result = idl.PayMent(id);
            return result;
        }
    }
}