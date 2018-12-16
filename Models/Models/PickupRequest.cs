using Models.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace CarpoolFood.Models
{
    public class PickupRequest
    {
        public Activity Activity { get; set; }
        public string FoodOrderNumber { get; set; }
        public int Tips { get; set; }
        public string PreferedPickupTime { get; set; }
        public Address Address { get; set; }
        public int IdDriverService { get; set; }
    }
}
