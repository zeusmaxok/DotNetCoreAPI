using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using CarpoolFood.Models;
using Database.DAL;
using Database.Interfaces;

using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Models.Models;
using Microsoft.AspNetCore.Cors;


namespace CarpoolFood.Controllers
{
    [Route("carpoolfood")]
    [EnableCors("CorsPolicy")]
    public class CarpoolFoodController : Controller
    {

        private readonly IDatabase _database;

        public CarpoolFoodController(IDatabase database)
        {
            _database = database;
        }

        [HttpPost]
        [Route("/login")]
        [Produces("application/json")]
        public IActionResult Login(string loginName, string pwd)
        {           
            if (string.IsNullOrEmpty(loginName) || string.IsNullOrEmpty(pwd))
            {
                return BadRequest("faild");
            }
            else
            {
                var carpoolFoodUser = _database.CheckUser(pwd: pwd.Trim(), loginName: loginName.Trim());

                if(carpoolFoodUser != null)
                {
                    return Ok(JsonConvert.SerializeObject(carpoolFoodUser));
                }
                else
                {
                    return NotFound();
                }
            }
        }


        [HttpGet]
        [Route("/getpickuprequests")]
        [Produces("application/json")]
        public IActionResult GetPickupRequest(string restaurant = "", string status = "", int userId = 0, int caller = 0)
        {
            List<PickupRequest> pickupRequests = _database.GetPickupRequests(restaurant, status, userId, caller);

            if(pickupRequests.Count > 0)
            {
                return Ok(pickupRequests);
            }
            else
            {
                return NoContent();
            }            
        }

        [HttpGet]
        [Route("/getdriverservices")]
        [Produces("application/json")]
        public IActionResult GetDriverServices(string restaurant = "", string status = "", int userId = 0, int caller = 0)
        {
            List<DriverService> driverServices = _database.GetDriverService(restaurant, status, userId, caller);

            if(driverServices.Count > 0)
            {
                return Ok(driverServices);
            }
            else
            {
                return NoContent();
            }          
        }

        [HttpPost]
        [Route("/register")]
        [Produces("application/json")]  
        public IActionResult Post([FromBody]CarpoolFoodUser carpoolFoodUser)
        {
            var result = _database.SaveNewUser(carpoolFoodUser);

            if (result.ToLower().Equals("success"))
            {
                return Ok("success");
            }
            else
            {
                return BadRequest(result);
            }
        }

        [HttpPost]
        [Route("/newpickuprequest")]
        [Produces("application/json")]
        public IActionResult CreatePickupRequest([FromBody]PickupRequest pickupRequest)
        {
            var result = _database.SaveNewPickupRequest(pickupRequest);

            if (result)
            {
                return NoContent();
            }
            else
            {
                return BadRequest();
            } 
        }

        [HttpPost]
        [Route("/newdriverservice")]
        [Produces("application/json")]
        public IActionResult CreateDriverService([FromBody]DriverService driverService)
        {
            var result = _database.SaveNewDriverServe(driverService);

            if (result)
            {
                return NoContent();
            }
            else
            {
                return BadRequest();
            }            
        }

        [HttpPut]
        [Route("/modifypickuprequest")]
        [Produces("application/json")]
        public IActionResult ModifyPickupRequest([FromBody]PickupRequest pickupRequest)
        {
            var result = _database.UpdatePickupRequest(pickupRequest);

            if (result)
            {
                return NoContent();
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPut]
        [Route("/modifydriverservice")]
        [Produces("application/json")]
        public IActionResult ModifyDriverService([FromBody]DriverService driverService)
        {
            var result = _database.UpdateDriverService(driverService);

            if (result)
            {
                return NoContent();
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("/plcaerequestorservice")]
        [Produces("application/json")]
        public IActionResult PlaceRequestOrService(int requestID, int serviceID)
        {
            var result = _database.SaveServicePlacement(requestID, serviceID);

            if (result.ToLower().Equals("success"))
            {
                return Ok("success");
            }
            else if (result.ToLower().Equals("service spots is full"))
            {
                return BadRequest("service full");
            }
            else
            {
                return StatusCode(500, result);
            }
        }

        [HttpPost]
        [Route("/cancelRequestOrService")]
        [Produces("application/json")]
        public IActionResult CancelRequestOrService(int userId = 0, int requestID = 0, int serviceID = 0, int type = 0)
        {
            var result = _database.DeleteServiceOrRequest(userId: userId, requestId: requestID, serviceId: serviceID, type: type);

            if (result)
            {
                return NoContent();
            }
            else
            {
                return BadRequest("Invalid request or service");
            }
        }

        [HttpPost]
        [Route("/completedriverservice")]
        [Produces("application/json")]
        public IActionResult CompleteDriverService(int serviceID)
        {
            var requestorIds = _database.CompleteDriverService(serviceID);

            if(requestorIds.Count > 0)
            {
                return Ok(requestorIds);
            }
            else if(requestorIds.Count == 0)
            {
                return NoContent();
            }
            else
            {
                return StatusCode(500);
            }

            
        }



        // DELETE api/values/5
        [HttpGet]
        [Route("/helloworld")]
        public IActionResult Get()
        {
            return Ok("Hello world");
        }
    }
}
