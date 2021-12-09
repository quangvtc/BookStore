using System;
using Xunit;
using DAL;
using Persistance;

namespace DALTest
{
    public class InvoiceDalTest
    {
        private InvoiceDal idal = new InvoiceDal();
        private Invoice i = new Invoice();

        [Fact]
        public void GetInvoiceByIdTest1()
        {
            int expect = 1;
            int result = idal.GetInvoiceId(expect).InvoiceId;
            Assert.True(expect == result);
        }
        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void GetInvoiceByIdTest2(int expect){
            int result = idal.GetInvoiceId(expect).InvoiceId;
            Assert.True(expect == result);
        }
    }
}