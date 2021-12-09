using System;
using Xunit;
using DAL;
using Persistance;

namespace DALTest
{
    public class BookDalTest
    {
        private BookDal bdal = new BookDal();
        private Book b = new Book();

        [Fact]
        public void GetBookByIdTest1()
        {
            int expect = 1;
            int result = bdal.GetBookById(expect).BookId;
            Assert.True(expect == result);
        }
        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]

        public void GetBookByIdTest2(int expect){
            int result = bdal.GetBookById(expect).BookId;
            Assert.True(expect == result);
        }
        
    }
}