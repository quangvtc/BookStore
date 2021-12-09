using System;
using MySql.Data.MySqlClient;
using Persistance;

namespace DAL
{
    public class UserDal{
      public int Login(User user){
        int login = 0;
        MySqlConnection connection = DbConfig.GetConnection();
        try{
            connection.Open();
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "select * from Users where user_name='"+
                user.UserName+"' and user_pass='"+
                Md5Algorithm.CreateMD5(user.UserPassword)+"';";
            MySqlDataReader reader = command.ExecuteReader();
            if(reader.Read()){
                login = reader.GetInt32("role");
            }else
            {
                login = 0;
            }
            reader.Close();
        }catch{
            login = -1;
        }
        finally{ 
            connection.Close(); 
        }
        return login;
        }
    }
}