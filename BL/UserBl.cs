using System;
using Persistance;
using DAL;

namespace BL
{
    public class UserBl
    {
        private UserDal dal = new UserDal();
        public int Login(User user){
            return dal.Login(user);
        }
    }
}
