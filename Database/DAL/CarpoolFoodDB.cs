using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using CarpoolFood.Models;
using Database.Interfaces;
using Microsoft.Extensions.Configuration;
using Models.Models;
using Activity = CarpoolFood.Models.Activity;

namespace Database.DAL
{
    public class CarpoolFoodDB: IDatabase
    {
        private readonly IConfiguration configuration;
        private string ConnString { get; set; }

        public CarpoolFoodDB(IConfiguration config)
        {
            this.configuration = config;
            ConnString = configuration.GetConnectionString("DefaultDatabase");          
        }       

        public List<DriverService> GetDriverService(string restaurant, string status, int userId = 0, int caller = 0)
        {
            List<DriverService> results = new List<DriverService>();

            var _StoredProcedure = "SearchDriverServices";

            using(var con = new SqlConnection(ConnString))
            {
                try
                {
                    con.Open();

                    using (var cmd = new SqlCommand(_StoredProcedure, con)
                                {
                                    CommandType = CommandType.StoredProcedure
                                })
                    {
                        cmd.Parameters.Add(new SqlParameter("@restaurant", restaurant));
                        cmd.Parameters.Add(new SqlParameter("@status", status));
                        cmd.Parameters.Add(new SqlParameter("@userId", userId));
                        cmd.Parameters.Add(new SqlParameter("@condition", caller));


                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                DriverService service = new DriverService()
                                {
                                    Activity = new Activity()
                                    {
                                        Id = int.Parse(reader["id"].ToString()),
                                        UserId = int.Parse(reader["userid"].ToString()),
                                        Restaurant = reader["restaurant"].ToString(),
                                        Notes = reader["notes"].ToString(),
                                        RequestStatus = reader["RequestStatus"].ToString()
                                    },
                                    PickupQuantity = int.Parse(reader["pickupQuantity"].ToString()),
                                    AvailableSpots = int.Parse(reader["AvailableSpots"].ToString()),
                                    TipReceived = int.Parse(reader["tipReceived"].ToString()),
                                    DeliveringTimeStart = reader["deliveringTime"].ToString(),
                                    Address = new Address()
                                    {
                                        Id = int.Parse(reader["idAddress"].ToString()),
                                        Address1 = reader["Address1"].ToString(),
                                        Address2 = reader["Address2"].ToString(),
                                        City = reader["City"].ToString(),
                                        State = reader["State"].ToString(),
                                        Zipcode = reader["Zipcode"].ToString()
                                    }
                                };

                                results.Add(service);
                            }
                        }
                        return results;
                    }
                }
                catch (Exception E)
                {
                    Debug.WriteLine(E.Message);
                    throw new Exception("GetDriverService", E);
                }
            }         
        }

        public List<PickupRequest> GetPickupRequests(string restaurant, string status, int userId = 0, int caller = 0)
        {
            List<PickupRequest> results = new List<PickupRequest>();

            var _StoredProcedure = "SearchPickupRequests";

            try
            {              
                using (var con = new SqlConnection(ConnString))
                {
                    con.Open();

                    using (var cmd = new SqlCommand(_StoredProcedure, con)
                                {
                                    CommandType = CommandType.StoredProcedure
                                })
                    {
                        cmd.Parameters.Add(new SqlParameter("@restaurant", restaurant));
                        cmd.Parameters.Add(new SqlParameter("@status", status));
                        cmd.Parameters.Add(new SqlParameter("@userId", userId));
                        cmd.Parameters.Add(new SqlParameter("@condition", caller));

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                PickupRequest request = new PickupRequest
                                {
                                    Activity = new Activity()
                                    {
                                        Id = int.Parse(reader["id"].ToString()),
                                        UserId = int.Parse(reader["userid"].ToString()),
                                        Restaurant = reader["restaurant"].ToString(),
                                        Notes = reader["notes"].ToString(),
                                        RequestStatus = reader["RequestStatus"].ToString()
                                    },
                                    FoodOrderNumber = reader["foodOrderNumber"].ToString(),
                                    PreferedPickupTime = reader["preferedPickupTime"].ToString(),
                                    Tips = int.Parse(reader["tips"].ToString()),
                                    Address = new Address()
                                    {
                                        Id = int.Parse(reader["idAddress"].ToString()),
                                        Address1 = reader["Address1"].ToString(),
                                        Address2 = reader["Address2"].ToString(),
                                        City = reader["City"].ToString(),
                                        State = reader["State"].ToString(),
                                        Zipcode = reader["Zipcode"].ToString()
                                    }
                                };

                                results.Add(request);
                            }
                            return results;
                        }
                    }
                }
            }
            catch (Exception E)
            {
                Debug.WriteLine(E.Message);
                throw new Exception("GetPickupRequests", E);
            }                     
        }

        public bool SaveNewDriverServe(DriverService driverService)
        {
            var isSuccess = false;

            try
            {
                using (var con = new SqlConnection(ConnString))
                {                
                    con.Open(); 

                    using (var cmd = GenerateDriverServiceSqlCommand("AddNewDriverService", driverService, con))
                    {
                        var returnParameter = cmd.Parameters.Add("@errorMessage", SqlDbType.VarChar, 256);
                        returnParameter.Direction = ParameterDirection.Output;
                        
                        cmd.ExecuteNonQuery();

                        var result = returnParameter.Value;

                        if (result.ToString().Equals("success"))
                        {
                            isSuccess = true;
                        }
                    }                   
                }
                
            }
            catch (Exception E)
            {
                Debug.WriteLine(E.Message);
                throw new Exception("SaveNewDriverServe", E);
            }

            return isSuccess;
        }        

        public bool SaveNewPickupRequest(PickupRequest pickupRequest)
        {
            var isSuccess = false;
            try
            {
                using (var con = new SqlConnection(ConnString))
                {                
                    con.Open();

                    using(var cmd = GeneratePickupRequestSqlCommand("AddNewPickupRequest", pickupRequest, con))
                    {
                        var returnParameter = cmd.Parameters.Add("@errorMessage", SqlDbType.VarChar, 256);
                        returnParameter.Direction = ParameterDirection.Output;                        

                        cmd.ExecuteNonQuery();

                        var result = returnParameter.Value;

                        if (result.ToString().Equals("success"))
                        {
                            isSuccess = true;
                        }
                    }
                }
            }
            catch (Exception E)
            {
                Debug.WriteLine(E.Message);
                throw new Exception("SaveNewPickupRequest", E);
            }

            return isSuccess;
        }

        private SqlCommand GenerateDriverServiceSqlCommand(string sqlCommand, DriverService driverService, SqlConnection con)
        {
            var command = new SqlCommand(sqlCommand, con)
            {
                CommandType = CommandType.StoredProcedure
            };

            SqlParameter[] sqlParameters =
            {
                new SqlParameter("@driverUserID", driverService.Activity.UserId),
                new SqlParameter("@restaurant", driverService.Activity.Restaurant),
                new SqlParameter("@pickupQuantity", driverService.PickupQuantity),
                new SqlParameter("@deliveringTime", driverService.DeliveringTimeStart),
                new SqlParameter("@notes", driverService.Activity.Notes),
                new SqlParameter("@address1", driverService.Address.Address1),
                new SqlParameter("@address2", driverService.Address.Address2),
                new SqlParameter("@city", driverService.Address.City),
                new SqlParameter("@state", driverService.Address.State),
                new SqlParameter("@zipcode", driverService.Address.Zipcode),
                new SqlParameter("@latitude", 0.00),
                new SqlParameter("@longitude", 0.00)
            };

            command.Parameters.AddRange(sqlParameters);

            return command;
        }

        private SqlCommand GeneratePickupRequestSqlCommand(string sqlCommand, PickupRequest pickupRequest, SqlConnection con)
        {
            var command = new SqlCommand(sqlCommand, con)
            {
                CommandType = CommandType.StoredProcedure
            };

            SqlParameter[] sqlParameters =
            {
                new SqlParameter("@userID", pickupRequest.Activity.UserId),
                new SqlParameter("@restaurant", pickupRequest.Activity.Restaurant),
                new SqlParameter("@foodOrderNumber", pickupRequest.FoodOrderNumber),
                new SqlParameter("@preferedPickupTime", pickupRequest.PreferedPickupTime),
                new SqlParameter("@tips", pickupRequest.Tips),
                new SqlParameter("@notes", pickupRequest.Activity.Notes),
                new SqlParameter("@address1", pickupRequest.Address.Address1),
                new SqlParameter("@address2", pickupRequest.Address.Address2),
                new SqlParameter("@city", pickupRequest.Address.City),
                new SqlParameter("@state", pickupRequest.Address.State),
                new SqlParameter("@zipcode", pickupRequest.Address.Zipcode),
                new SqlParameter("@latitude", 0.00),
                new SqlParameter("@longitude", 0.00)
            };

            if (true)
            {
                  
            }

            command.Parameters.AddRange(sqlParameters);

            return command;
        }

        public string SaveNewUser(CarpoolFoodUser carpoolFoodUser)
        {
            var message = "";

            using(var con = new SqlConnection(ConnString))
            {
                try
                {
                    con.Open();

                    using (var cmd = new SqlCommand("AddUser", con)
                            {
                                CommandType = CommandType.StoredProcedure
                            })
                    {
                        SqlParameter[] sqlParameters =
                        {
                            new SqlParameter("@loginName", carpoolFoodUser.LoginName),
                            new SqlParameter("@password", carpoolFoodUser.Password),
                            new SqlParameter("@firstName", carpoolFoodUser.FirstName),
                            new SqlParameter("@lastName", carpoolFoodUser.LastName),
                            new SqlParameter("@email", carpoolFoodUser.Email),
                            new SqlParameter("@phone", carpoolFoodUser.Phone),
                            new SqlParameter("@isPickup", carpoolFoodUser.IsPickupUser),
                            new SqlParameter("@isDriver", carpoolFoodUser.IsDriverUser)
                        };

                        cmd.Parameters.AddRange(sqlParameters);

                        var returnParameter = cmd.Parameters.Add("@responseMessage", SqlDbType.VarChar, 256);
                        returnParameter.Direction = ParameterDirection.Output;

                        cmd.ExecuteNonQuery();

                        var result = returnParameter.Value;

                        return message = result.ToString();                       
                    }
                }catch(Exception E)
                {
                    Debug.WriteLine(E.Message);
                    throw new Exception("SaveNewUser", E);
                }
            }          
        }

        public bool UpdateDriverService(DriverService driverService)
        {
            var result = false;

            try
            {
                using (var con = new SqlConnection(ConnString))
                {
                    con.Open();

                    using (var cmd = GenerateDriverServiceSqlCommand("Driver_UpdateDriverService", driverService, con))
                    {
                        cmd.Parameters.RemoveAt("@driverUserID");
                        cmd.Parameters.RemoveAt("@latitude");
                        cmd.Parameters.RemoveAt("@longitude");
                        cmd.Parameters.Add(new SqlParameter("@driverServiceID", driverService.Activity.Id));
                        cmd.Parameters.Add(new SqlParameter("idAddress", driverService.Address.Id));
                        var updatedRecords = cmd.ExecuteNonQuery();

                        if(updatedRecords == 2)
                        {
                            result = true;
                        }
                    }
                }               
            }
            catch (Exception E)
            {
                Debug.WriteLine(E.Message);
                throw new Exception("UpdateDriverService", E);
            }

            return result;
        }

        public bool UpdatePickupRequest(PickupRequest pickupRequest)
        {
            var result = false;

            try
            {
                using (var con = new SqlConnection(ConnString))
                {
                    con.Open();

                    using (var cmd = GeneratePickupRequestSqlCommand("Requester_UpdatePickupRequest", pickupRequest, con))
                    {
                        cmd.Parameters.RemoveAt("@userID");
                        cmd.Parameters.RemoveAt("@latitude");
                        cmd.Parameters.RemoveAt("@longitude");
                        cmd.Parameters.Add(new SqlParameter("@requestID", pickupRequest.Activity.Id));
                        cmd.Parameters.Add(new SqlParameter("idAddress", pickupRequest.Address.Id));

                        var updatedRecords = cmd.ExecuteNonQuery();

                        if (updatedRecords == 2)
                        {
                            result = true;
                        }
                    }
                }
            }
            catch (Exception E)
            {
                Debug.WriteLine(E.Message);
                throw new Exception("UpdateDriverService", E);
            }

            return result;
        }

        public CarpoolFoodUser CheckUser(string loginName, string pwd)
        {
            CarpoolFoodUser carpoolFoodUser = null;

            try
            {
                using (var con = new SqlConnection(ConnString))
                {
                    con.Open();

                    using (var cmd = new SqlCommand("UserLogin", con)
                        {
                            CommandType = CommandType.StoredProcedure                         
                        })
                    {
                        cmd.Parameters.Add(new SqlParameter("@uloginName", loginName));
                        cmd.Parameters.Add(new SqlParameter("@pword", pwd));

                        var outputParameter = cmd.Parameters.Add("@responseMessage", SqlDbType.VarChar, 256);
                        outputParameter.Direction = ParameterDirection.Output;

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                carpoolFoodUser = new CarpoolFoodUser()
                                {
                                    Id = int.Parse(reader["userID"].ToString()),
                                    LoginName = reader["loginName"].ToString(),
                                    FirstName = reader["firstName"].ToString(),
                                    LastName = reader["lastName"].ToString(),
                                    Email = reader["email"].ToString(),
                                    Phone = reader["phone"].ToString(),
                                    IsDriverUser = reader.GetBoolean(6),
                                    IsPickupUser = reader.GetBoolean(7)

                                };
                            }

                            reader.Close();

                            var outputValue = outputParameter.Value.ToString();

                            if(outputValue.Equals("Incorrect passwor") || outputValue.Equals("Invalid login"))
                            {
                                carpoolFoodUser = null;
                            }
                            
                        }
                    }
                }
            }
            catch (Exception E)
            {
                Debug.WriteLine(E.Message);
                throw new Exception("CheckUser", E);
            }

            return carpoolFoodUser;
        }

        public string SaveServicePlacement(int requestID, int serviceID)
        {
            string result = "";

            try
            {
                using (var con = new SqlConnection(ConnString))
                {
                    con.Open();

                    using (var cmd = new SqlCommand("PlaceDriverPickups_CarpoolFood", con)
                    {
                        CommandType = CommandType.StoredProcedure
                    })
                    {
                        cmd.Parameters.Add(new SqlParameter("@requestID", requestID));
                        cmd.Parameters.Add(new SqlParameter("@serviceID", serviceID));

                        var outputParameter = cmd.Parameters.Add("@responseMessage", SqlDbType.VarChar, 256);
                        outputParameter.Direction = ParameterDirection.Output;

                        cmd.ExecuteNonQuery();

                        result = outputParameter.Value.ToString();                        
                    }
                }

                    return result;
            }
            catch (Exception E)
            {
                Debug.WriteLine(E.Message);
                throw new Exception("SaveServicePlacement", E);
            }
        }

        public bool DeleteServiceOrRequest(int userId = 0, int requestId = 0, int serviceId = 0, int type = 0)
        {
            bool result = false;
            string _StoredProcedure = "";

            switch (type)
            {
                case 1:
                    _StoredProcedure = "DeleteRequest";
                    break;
                case 2:
                    _StoredProcedure = "DeleteService";
                    break;
                default:
                    return result;
            }

            try
            {
                using (var con = new SqlConnection(ConnString))
                {
                    con.Open();

                    using (var cmd = new SqlCommand(_StoredProcedure, con)
                    {
                        CommandType = CommandType.StoredProcedure
                    })
                    {
                        if(type == 1)
                        {
                            cmd.Parameters.Add(new SqlParameter("@requestId", requestId));
                        }
                        else if(type == 2)
                        {
                            cmd.Parameters.Add(new SqlParameter("@serviceId", serviceId));
                        }
                        else
                        {
                            return result;
                        }

                        cmd.Parameters.Add(new SqlParameter("@userId", userId));

                        var outputParameter = cmd.Parameters.Add("@responseMessage", SqlDbType.VarChar, 256);
                        outputParameter.Direction = ParameterDirection.Output;

                        cmd.ExecuteNonQuery();

                        var message = outputParameter.Value.ToString();

                        if (message.Equals("success"))
                        {
                            result = true;
                        }
                    }
                }

                return result;
            }
            catch (Exception E)
            {
                Debug.WriteLine(E.Message);
                throw new Exception("DeleteServiceOrRequest", E);
            }
        }

        public List<int> CompleteDriverService(int serviceId)
        {
            var resultList = new List<int>();

            try
            {
                using (var con = new SqlConnection(ConnString))
                {
                    con.Open();

                    using (var cmd = new SqlCommand("CompleteDriverService", con)
                    {
                        CommandType = CommandType.StoredProcedure
                    })
                    {
                       
                        cmd.Parameters.Add(new SqlParameter("@serviceId", serviceId));

                        var reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            resultList.Add(int.Parse(reader["userId"].ToString()));
                        }
                    }
                }

                return resultList;
            }
            catch (Exception E)
            {
                Debug.WriteLine(E.Message);
                throw new Exception("DeleteServiceOrRequest", E);
            }
        }
    }
}
