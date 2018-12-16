using System;
using System.Collections.Generic;
using System.Text;

namespace CarpoolFood.Models
{
    public class Activity
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Restaurant { get; set; }
        public string Notes { get; set; }
        public string RequestStatus { get; set; }
        public string CreatedTime { get; set; }
        public string LastModifiedTime { get; set; }

    }

    public class Person
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
