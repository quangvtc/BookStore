using System;
using Xunit;
using DAL;
using Persistance;

namespace DALTest
{
    public class UserDalTest
    {
        private UserDal udal = new UserDal();
        private User u = new User();

        [Fact]
        public void LoginTest1()
        {
            u.UserName = "quangnguyen";
            u.UserPassword = "12345";
            int expect = 1;
            int result = udal.Login(u);
            Assert.True(expect == result);
        }

        [Theory]
        [InlineData("quangnguyen1","12345",0)]
        [InlineData("quangnguyen","123456",0)]
        [InlineData("quangnguyen","12345",1)]
        [InlineData("pf15","12345",1)]
        [InlineData("pf16","12345",0)]
        [InlineData("pf17","12345",0)]
        [InlineData("pf18","12345",0)]
        [InlineData("pf19","12345",0)]
        [InlineData("pf20","12345",0)]

        public void LoginTest2(string username, string password,int expect){
            u.UserName = username;
            u.UserPassword = password;
            int result = udal.Login(u);
            Assert.True(expect == result);
        }
    }
}
