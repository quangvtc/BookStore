using System;

namespace Persistance
{
    public class User
    {
        public int UserId { get; set; }
        public string StaffName { get; set; }
        public string UserName { get; set; }
        public string UserPassword { get; set; }
        public string Address { get; set; }
        public string Telephone { get; set; }
        public string UserEmail { get; set; }
        public int Role { get; set; }

        public static int Sale_Role = 1;
        public static int Accountance_Role = 2;
        public static int WareHouse_Role = 3;
        
    }
}