﻿using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MvcTraining.Models;
using MvcTraining.Resources;
using System;
using System.Collections.Generic;

namespace MvcTraining.Repositories.Recipe
{
    public class RecipeDao : IRepository<RecipeDto>
    {
        public readonly ConnectionStringModel _connection;

        public RecipeDao(IOptions<ConnectionStringModel> connection)
        {
            _connection = connection.Value;
        }
        public long Create(RecipeDto item)
        {
            long result = 0;
            try
            {
                using(var con=new SqlConnection(_connection.DbConnection))
                {
                    con.Open();
                    var cmd=con.CreateCommand();
                    cmd.CommandText = SqlResources.SaveRecipe;
                    cmd.Parameters.AddWithValue("@title",item.Title);
                    cmd.Parameters.AddWithValue("@descript",item.Description);
                    cmd.Parameters.AddWithValue("@instruction",item.Instruction);
                    cmd.Parameters.AddWithValue("@prepareTime",item.PreparationTime);
                    cmd.Parameters.AddWithValue("@cookingTime",item.CookingTime);
                    cmd.Parameters.AddWithValue("@author",item.Author);
                    cmd.Parameters.AddWithValue("@createdDate",DateTime.Now);
                    cmd.Parameters.AddWithValue("@modifiedDate",DateTime.Now);
                    cmd.Parameters.AddWithValue("@category",item.Category);
                    cmd.Parameters.AddWithValue("@image", item.DishPhoto);
                     result=(long)(cmd.ExecuteScalar());
                }
            }catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }

        public int Delete(long id)
        {
            throw new System.NotImplementedException();
        }

        public int DuplicateCreate(RecipeDto item)
        {
            throw new System.NotImplementedException();
        }

        public int DuplicateUpdate(RecipeDto item)
        {
            throw new System.NotImplementedException();
        }

        public int ListCount()
        {
            int result = 0;
            try
            {
                using (var con = new SqlConnection(_connection.DbConnection))
                {
                    con.Open();
                    var cmd = con.CreateCommand();
                    cmd.CommandText = SqlResources.RecipeCount;
                    result = (int)cmd.ExecuteScalar();

                    con.Close();
                }
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
            
        }

        public int FilterListCount(string searchParam)
        {
            int count = 0;
            try
            {
                using(var con=new SqlConnection(_connection.DbConnection))
                {
                    con.Open();
                    var cmd = con.CreateCommand();
                    cmd.CommandText = SqlResources.RecipeCount;
                    if (!String.IsNullOrEmpty(searchParam))
                    {
                        cmd.CommandText += searchParam;
                    }
                   count= (int)cmd.ExecuteScalar();
                }
            }catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return count;
        }

        public List<RecipeDto> GetAll(string searchParam, string pagination)
        {
            List<RecipeDto> recipeDtos = new List<RecipeDto>();
            try
            {
                using(var con=new SqlConnection(_connection.DbConnection))
                {
                    con.Open();
                    var cmd=con.CreateCommand();
                    cmd.CommandText = SqlResources.GetAllRecipe;
                    if (!String.IsNullOrEmpty(searchParam))
                    {
                        cmd.CommandText+= searchParam;
                    }
                    cmd.CommandText += pagination;
                   using(SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            RecipeDto dto = new RecipeDto();
                            dto.Id = Convert.ToInt64(rdr["id"]);
                            dto.Title = rdr["title"].ToString();
                            dto.Description = rdr["descript"].ToString();
                            dto.Instruction = rdr["instruction"].ToString();
                            dto.Category = rdr["category"].ToString();
                            dto.Author = rdr["author"].ToString();
                            dto.PreparationTime = rdr["prepare_time"].ToString();
                            dto.CookingTime = rdr["cooking_time"].ToString();
                            dto.DishPhoto = rdr["dish_image"].ToString();
                            dto.CreatedDate = Convert.ToDateTime(rdr["created_date"]);
                            recipeDtos.Add(dto);
                        }
                    }
                    con.Close();
                }
            }catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return recipeDtos;
        }

        public RecipeDto GetById(long id)
        {
            RecipeDto dto = new RecipeDto();
            try
            {
                using(var con=new SqlConnection(_connection.DbConnection))
                {
                    con.Open();
                    var cmd = con.CreateCommand();
                    cmd.CommandText = SqlResources.GetRecipeById;
                    cmd.Parameters.AddWithValue("id", id);
                    using(SqlDataReader rdr=cmd.ExecuteReader())
                    {
                       while(rdr.Read())
                        {
                            dto.Id = Convert.ToInt64(rdr["id"]);
                            dto.Title = rdr["title"].ToString();
                            dto.Description = rdr["descript"].ToString();
                            dto.Instruction = rdr["instruction"].ToString();
                            dto.PreparationTime = rdr["prepare_time"].ToString();
                            dto.CookingTime = rdr["cooking_time"].ToString();
                            dto.Author = rdr["author"].ToString();
                            dto.Category = rdr["category"].ToString();
                            dto.DishPhoto = rdr["dish_image"].ToString();
                            dto.CreatedDate = Convert.ToDateTime(rdr["created_date"]);
                       }
                    }
                    con.Close();
                }
            }catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return dto;
        }

        public int UpdateRecipeAndIngredient(RecipeDto item,List<IngredientDto> ingreDto)
        {
            int result = 0;
            using(var con=new SqlConnection(_connection.DbConnection))
            {
                SqlTransaction transaction=null;  
                try
                {
                    con.Open();
                    transaction = con.BeginTransaction();
                    var cmd = con.CreateCommand();
                    cmd.Transaction = transaction;

                    cmd.CommandText = SqlResources.UpdateRecipe;
                    cmd.Parameters.AddWithValue("@title", item.Title);
                    cmd.Parameters.AddWithValue("@descript", item.Description);
                    cmd.Parameters.AddWithValue("@instruct", item.Instruction);
                    cmd.Parameters.AddWithValue("@preTime", item.PreparationTime);
                    cmd.Parameters.AddWithValue("@cookTime", item.CookingTime);
                    cmd.Parameters.AddWithValue("@author", item.Author);
                    cmd.Parameters.AddWithValue("@modifiedDate", item.ModifiedDate);
                    cmd.Parameters.AddWithValue("@category", item.Category);
                    cmd.Parameters.AddWithValue("@image", item.DishPhoto);
                    cmd.Parameters.AddWithValue("@id", item.Id);
                    result = cmd.ExecuteNonQuery();

                    if(ingreDto.Count >0)
                    {
                        cmd.CommandText = SqlResources.SaveIngredient;
                        foreach (var ingre in ingreDto)
                        {
                            cmd.Parameters.AddWithValue("@name", ingre.Name);
                            cmd.Parameters.AddWithValue("@quantity", ingre.Quantity);
                            cmd.Parameters.AddWithValue("@unit", ingre.Unit);
                            cmd.Parameters.AddWithValue("@recipeId", ingre.RecipeId);

                            result += cmd.ExecuteNonQuery();
                            cmd.Parameters.Clear();
                        }
                        
                    }
                    transaction.Commit();
                    con.Close();
                }
                catch (Exception ex)
                {
                    try
                    {
                        transaction.Rollback();
                    }catch(Exception ex2)
                    {
                        throw new Exception(ex2.Message);
                    }
                    throw new Exception(ex.Message);
                }
            }
            return result;
        }

        int IRepository<RecipeDto>.Create(RecipeDto item)
        {
            throw new NotImplementedException();
        }

        public int Update(RecipeDto item)
        {
            throw new NotImplementedException();
        }
    }
}
