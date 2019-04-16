using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PMS.Entities;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;

namespace PMS.DAL
{
    public static class ProductDAO
    {
        public static int Save(ProductDTO dto)
        {
            try
            {
                using (DBHelper helper = new DBHelper())
                {
                    String sqlQuery = "";
                    if (dto.ProductID > 0)
                    {
                        sqlQuery = String.Format("Update Products Set Name='{0}',Price='{1}',PictureName='{2}',ModifiedOn=NOW(),ModifiedBy='{3}' Where ProductID={4}",
                        dto.Name, dto.Price, dto.PictureName, dto.ModifiedBy, dto.ProductID);
                        helper.ExecuteQuery(sqlQuery);
                        return dto.ProductID;
                    }
                    else
                    {
                        sqlQuery = String.Format("INSERT INTO Products(Name, Price, PictureName, CreatedOn, CreatedBy,IsActive) VALUES('{0}','{1}','{2}',NOW(),'{3}',{4}); Select @@IDENTITY",
                        dto.Name, dto.Price, dto.PictureName, dto.CreatedBy, 1);

                        var obj = helper.ExecuteScalar(sqlQuery);
                        return Convert.ToInt32(obj);
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return 0;
        }
        public static ProductDTO GetProductById(int pid)
        {
            try
            {
                var query = String.Format("Select * from Products Where ProductId={0}", pid);

                using (DBHelper helper = new DBHelper())
                {
                    var reader = helper.ExecuteReader(query);

                    ProductDTO dto = null;

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
       

        public static List<ProductDTO> GetAllProducts(Boolean pLoadComments=false)
        {
            try
            {
                var query = "Select * from Products Where IsActive = 1;";

                using (DBHelper helper = new DBHelper())
                {
                    var reader = helper.ExecuteReader(query);
                    List<ProductDTO> list = new List<ProductDTO>();

                    while (reader.Read())
                    {
                        var dto = FillDTO(reader);
                        if (dto != null)
                        {
                            list.Add(dto);
                        }
                    }
                    if (pLoadComments == true)
                    {
                        //var commentsList = CommentDAO.GetAllComments();

                        var commentsList = CommentDAO.GetTopComments(2);

                        foreach (var prod in list)
                        {
                            List<CommentDTO> prodComments = commentsList.Where(c => c.ProductID == prod.ProductID).ToList();
                            prod.Comments = prodComments;
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

        public static int DeleteProduct(int pid)
        {
            try
            {
                String sqlQuery = String.Format("Update Products Set IsActive=0 Where ProductID={0}", pid);

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

        private static ProductDTO FillDTO(MySqlDataReader reader)
        {
            var dto = new ProductDTO();
            dto.ProductID = reader.GetInt32(0);
            dto.Name = reader.GetString(1);
            dto.Price = reader.GetDouble(2);
            dto.PictureName = reader.GetString(3);
            dto.CreatedOn = reader.GetDateTime(4);
            dto.CreatedBy = reader.GetInt32(5);
            if (reader.GetValue(6) != DBNull.Value)
                dto.ModifiedOn = reader.GetDateTime(6);
            if (reader.GetValue(7) != DBNull.Value)
                dto.ModifiedBy = reader.GetInt32(7);

            dto.IsActive = reader.GetBoolean(8);
            return dto;
        }
    }
}
