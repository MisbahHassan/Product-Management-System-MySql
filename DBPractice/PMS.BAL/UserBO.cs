using PMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMS.BAL
{
    public class UserBO
    {
        public static int Save(UserDTO dto)
        {
            return PMS.DAL.UserDAO.Save(dto);
        }
        public static UserDTO getUserByLogin(String login)
        {
            return PMS.DAL.UserDAO.getUserByLogin(login);
        }
        public static int UpdatePassword(UserDTO dto)
        {
            return PMS.DAL.UserDAO.UpdatePassword(dto);
        }
        public static int UpdateUser(UserDTO u)
        {
            return PMS.DAL.UserDAO.UpdateUser(u);
        }
        public static UserDTO ValidateUser(String pLogin, String pPassword)
        {
            return PMS.DAL.UserDAO.ValidateUser(pLogin, pPassword);
        }
        public static UserDTO isUserExist(string login,string email)
        {
            return PMS.DAL.UserDAO.isUserExist(login,email);
        }
        public static UserDTO GetUserByEmail(String email)
        {
            return PMS.DAL.UserDAO.GetUserByEmail(email);
        }
        
        public static List<UserDTO> GetAllUsers()
        {
            return PMS.DAL.UserDAO.GetAllUsers();
        }
        public static bool sendEmail(String toEmailAddress, String subject, String body)
        {
            return PMS.DAL.UserDAO.sendEmail(toEmailAddress,subject,body);
        }
         public static int DeleteUser(int pid)
        {
            return PMS.DAL.UserDAO.DeleteUser(pid);
        }

        public static UserDTO GetUserById(int userID)
        {
            return PMS.DAL.UserDAO.getUserByID(userID);
        }
        public static UserDTO GetUserByPictureName(String img)
        {
            return PMS.DAL.UserDAO.GetUserByPictureName(img);
        }

    }
}
