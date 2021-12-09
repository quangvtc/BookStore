using System;
using Xunit;
using DAL;
using Persistance;

namespace DALTest
{
    public class CustomerDalTest
    {
        private CustomerDal cdal = new CustomerDal();
        private Customer c = new Customer();

        [Fact]
        public void GetCustomerByIdTest1()
        {
            int expect = 1;
            int result = cdal.GetById(expect).CustomerId;
            Assert.True(expect == result);
        }
        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        [InlineData(6)]
        [InlineData(7)]
        [InlineData(8)]
        [InlineData(9)]
        [InlineData(10)]

        public void GetCustomerByIdTest2(int expect){
            int result = cdal.GetById(expect).CustomerId;
            Assert.True(expect == result);
        }
        
    }
}