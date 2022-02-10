using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace RESTApiWithAzureFunction
{
    public static class TaskListFunction
    {
        [FunctionName("CreatePerson")]
        public static async Task<IActionResult> CreatePerson(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "task")] HttpRequest req, ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var input = JsonConvert.DeserializeObject<CreatePerson>(requestBody);
            CreatePerson cp = input;
            try
            {
                using (SqlConnection connection = new SqlConnection(Environment.GetEnvironmentVariable("SqlConnectionString")))
                {
                    connection.Open();
                    if (input.ID >0)
                    {
                        var query = $"INSERT INTO [Persons] (ID,first_name,pref_lang,Age) VALUES('{input.ID}','{input.first_name}', '{input.pref_lang}' , '{input.Age}')";
                        SqlCommand command = new SqlCommand(query, connection);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception e)
            {
                log.LogError(e.ToString());
                return new BadRequestResult();
            }
            //return new OkResult(cp);
            return new OkObjectResult("inserted successfully !");
        }

        [FunctionName("GetPerson")]
        public static async Task<IActionResult> GetPerson(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "task")] HttpRequest req, ILogger log)
        {
            List<PersonData> taskList = new List<PersonData>();
            try
            {
                using (SqlConnection connection = new SqlConnection(Environment.GetEnvironmentVariable("SqlConnectionString")))
                {
                    connection.Open();
                    var query = @"Select * from Persons";
                    SqlCommand command = new SqlCommand(query, connection);
                    var reader = await command.ExecuteReaderAsync();
                    while (reader.Read())
                    {
                        PersonData task = new PersonData()
                        {
                            ID = (int)reader["ID"],
                            first_name = reader["first_name"].ToString(),
                            pref_lang = reader ["pref_lang"].ToString(),
                            Age = (int) reader["Age"]
                            
                        };
                        taskList.Add(task);
                    }
                }
            }
            catch (Exception e)
            {
                log.LogError(e.ToString());
            }
            if (taskList.Count > 0)
            {
                return new OkObjectResult(taskList);
            }
            else
            {
                return new NotFoundResult();
            }
        }

        [FunctionName("Getall")]
        public static IActionResult Getall(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "task/{ID}")] HttpRequest req, ILogger log, int ID)
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection connection = new SqlConnection(Environment.GetEnvironmentVariable("SqlConnectionString")))
                {
                    connection.Open();
                    var query = @"Select * from Persons Where ID = @ID";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@ID", ID);
                    SqlDataAdapter da = new SqlDataAdapter(command);
                    da.Fill(dt);
                }
            }
            catch (Exception e)
            {
                log.LogError(e.ToString());
            }
            if (dt.Rows.Count == 0)
            {
                return new NotFoundResult();
            }
            return new OkObjectResult(dt);
        }

        [FunctionName("DeletePerson")]
        public static IActionResult DeletePerson(
        [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "task/{ID}")] HttpRequest req, ILogger log, int ID)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(Environment.GetEnvironmentVariable("SqlConnectionString")))
                {
                    connection.Open();
                    var query = @"Delete from Persons Where ID = @ID";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@ID", ID);
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                log.LogError(e.ToString());
                return new BadRequestResult();
            }
            return new OkResult();
        }

        [FunctionName("UpdatePerson")]
        public static async Task<IActionResult> UpdatePerson(
        [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "task/{ID}")] HttpRequest req, ILogger log, int ID)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var input = JsonConvert.DeserializeObject<UpdatePerson>(requestBody);
            try
            {
                using (SqlConnection connection = new SqlConnection(Environment.GetEnvironmentVariable("SqlConnectionString")))
                {
                    connection.Open();
                    var query = @"Update Persons Set first_name = @first_name , pref_lang = @pref_lang , Age = @Age Where ID = @ID";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@first_name", input.first_name);
                    command.Parameters.AddWithValue("@pref_lang", input.pref_lang);
                    command.Parameters.AddWithValue("@Age", input.Age);
                    command.Parameters.AddWithValue("@ID", ID);
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                log.LogError(e.ToString());
            }
            return new OkResult();
        }
    }
}