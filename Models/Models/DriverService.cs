using Models.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace CarpoolFood.Models
{
    public  class DriverService
    {
        public Activity Activity { get; set; }
        public int PickupQuantity { get; set; }
        public int AvailableSpots { get; set; }
        public string DeliveringTimeStart { get; set; }
        public string DeliveringTimeEnd { get; set; }
        public int TipReceived { get; set; }
        public Address Address { get; set; }
        public List<PickupRequest> PickupRequests { get; set; }
    }
}
