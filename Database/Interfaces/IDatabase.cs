using CarpoolFood.Models;
using Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Database.Interfaces
{
    public interface IDatabase
    {
        List<PickupRequest> GetPickupRequests(string restaurant, string status, int userId, int caller);

        List<DriverService> GetDriverService(string restaurant, string status, int userId, int caller);

        string SaveNewUser (CarpoolFoodUser carpoolFoodUser);

        bool SaveNewPickupRequest(PickupRequest pickupRequest);

        bool SaveNewDriverServe(DriverService driverService);

        bool UpdatePickupRequest(PickupRequest pickupRequest);

        bool UpdateDriverService(DriverService driverService);

        string SaveServicePlacement(int requestID, int serviceID);

        CarpoolFoodUser CheckUser(string loginName, string pwd);

        bool DeleteServiceOrRequest(int userId = 0, int requestId = 0, int serviceId = 0, int type = 0);

        List<int> CompleteDriverService(int serviceId);
    }
}
