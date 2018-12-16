using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Models
{
    public class CarpoolFoodUser
    {
        public int Id { get; set; }
        public string LoginName { get; set; }
        private string _Password;
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public bool IsPickupUser { get; set; }
        public bool IsDriverUser { get; set; }

        public string Password
        {
            get
            {
                return this._Password;
            }
            set
            {
                _Password = value;
            }
        }
    }
}
