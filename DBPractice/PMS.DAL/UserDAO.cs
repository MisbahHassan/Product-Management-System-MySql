using MySql.Data.MySqlClient;
using PMS.Entities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace PMS.DAL
{
    public static class UserDAO
    {
        public static int Save(UserDTO dto)
        {
            
            String sqlQuery = "";
            if (dto.UserID > 0)
            {
                sqlQuery = String.Format("Update Users Set Name='{0}', PictureName='{1}' Where UserID={2}",
                    dto.Name, dto.PictureName, dto.UserID);
            }
            else
            {
                sqlQuery = String.Format("INSERT INTO Users(Name, Login,Password, PictureName, IsAdmin,IsActive,Email) VALUES('{0}','{1}','{2}','{3}',{4},{5},'{6}')",
                    dto.Name, dto.Login, dto.Password, dto.PictureName, 0, 1, dto.Email);
            }
            try
            {
                using (DBHelper helper = new DBHelper())
                {
                    return helper.ExecuteQuery(sqlQuery);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return 0;
        }
        public static bool sendEmail(String toEmailAddress, String subject, String body)
        {
            try
            {

                System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();

                MailAddress to = new MailAddress(toEmailAddress);
                mail.To.Add(to);

                MailAddress from = new MailAddress("eadsef15morning@gmail.com", "Admin");
                mail.From = from;

                mail.Subject = subject;
                mail.Body = body;

                var sc = new SmtpClient("smtp.gmail.com", 587)
                {
                    Credentials = new System.Net.NetworkCredential("eadsef15morning@gmail.com", "EAD_sef15"),
                    EnableSsl = true
                };

                sc.Send(mail);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return false;

        } 
        public static UserDTO getUserByLogin(string login)
        {
            try
            {
                var query = String.Format("Select * from Users Where Login='" + login + "' ");
                using (DBHelper helper = new DBHelper())
                {
                    var reader = helper.ExecuteReader(query);

                    UserDTO dto = null;

                    if (reader.Read())
                    {
                        dto = FillDTO(reader);
                    }

                    return dto;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }
        public static UserDTO getUserByID(int id)
        {
            try
            {
                var query = String.Format("Select * from Users Where UserID='" + id + "' ");
                using (DBHelper helper = new DBHelper())
                {
                    var reader = helper.ExecuteReader(query);

                    UserDTO dto = null;

                    if (reader.Read())
                    {
                        dto = FillDTO(reader);
                    }

                    return dto;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }
        public static int UpdateUser(UserDTO u)
        {
            try
            {
                String sqlQuery = "";
                sqlQuery = String.Format("Update Users Set Name='" + u.Name + "',Email='" + u.Email + "',PictureName='" + u.PictureName + "' Where UserID='" + u.UserID + "'");
                using (DBHelper helper = new DBHelper())
                {
                    return helper.ExecuteQuery(sqlQuery);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return 0;
        }
        public static UserDTO GetUserByPictureName(String img)
        {
            try
            {
                var query = String.Format("Select * from Users Where PictureName='{0}'", img);

                using (DBHelper helper = new DBHelper())
                {
                    var reader = helper.ExecuteReader(query);

                    UserDTO dto = null;

                    if (reader.Read())
                    {
                        dto = FillDTO(reader);
                    }

                    return dto;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }
        public static UserDTO isUserExist(string login,string email)
        {
            try
            {
                var query = String.Format("select * from Users where Login='" + login + "' OR Email='" + email + "' ");
                using (DBHelper helper = new DBHelper())
                {
                    var reader = helper.ExecuteReader(query);

                    UserDTO dto = null;

                    if (reader.Read())
                    {
                        dto = FillDTO(reader);
                    }
                    return dto;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }
        public static int UpdatePassword(UserDTO dto)
        {
            try
            {
                String sqlQuery = "";
                sqlQuery = String.Format("Update Users Set Password='{0}' Where UserID={1}", dto.Password, dto.UserID);


                using (DBHelper helper = new DBHelper())
                {
                    return helper.ExecuteQuery(sqlQuery);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return 0;
        }

        public static UserDTO ValidateUser(String pLogin, String pPassword)
        {
            try
            {
                var query = String.Format("Select * from Users Where Login='{0}' and Password='{1}'", pLogin, pPassword);

                using (DBHelper helper = new DBHelper())
                {
                    var reader = helper.ExecuteReader(query);

                    UserDTO dto = null;

                    if (reader.Read())
                    {
                        dto = FillDTO(reader);
                    }

                    return dto;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }

        public static UserDTO GetUserByEmail(String email)
        {
            try
            {
                var query = String.Format("Select * from Users Where Email='" + email + "' ");

                using (DBHelper helper = new DBHelper())
                {
                    var reader = helper.ExecuteReader(query);

                    UserDTO dto = null;

                    if (reader.Read())
                    {
                        dto = FillDTO(reader);
                    }

                    return dto;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }

        public static List<UserDTO> GetAllUsers()
        {
            try
            {
                using (DBHelper helper = new DBHelper())
                {
                    var query = "Select * from Users Where IsActive = 1;";
                    var reader = helper.ExecuteReader(query);
                    List<UserDTO> list = new List<UserDTO>();

                    while (reader.Read())
                    {
                        var dto = FillDTO(reader);
                        if (dto != null)
                        {
                            list.Add(dto);
                        }
                    }

                    return list;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }

        public static int DeleteUser(int pid)
        {
            try
            {
                String sqlQuery = String.Format("Update Users Set IsActive=0 Where UserID={0}", pid);

                using (DBHelper helper = new DBHelper())
                {
                    return helper.ExecuteQuery(sqlQuery);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return 0;
        }

        private static UserDTO FillDTO(MySqlDataReader reader)
        {
            var dto = new UserDTO();
            dto.UserID = reader.GetInt32(0);
            dto.Name = reader.GetString(1);
            dto.Login = reader.GetString(2);
            dto.Password = reader.GetString(3);
            dto.PictureName = reader.GetString(4);
            dto.IsAdmin = reader.GetBoolean(5);
            dto.IsActive = reader.GetBoolean(6);
            dto.Email = reader.GetString(7);
            return dto;
        }
    }
}
