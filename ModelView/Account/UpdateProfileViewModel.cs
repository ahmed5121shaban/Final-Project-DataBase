﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Final.Enums;

namespace ModelView.Account
{
    public class UpdateProfileViewModel
    {
        [StringLength(15, MinimumLength = 3)]
        public string FirstName { get; set; }

        [StringLength(15, MinimumLength = 3)]
        public string LastName { get; set; }
        public IFormFile ProfileImage { get; set; }
        [EmailAddress]
        public string Email {  get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Street { get; set; }
        public string PostalCode { get; set; }
        public string TimeZone { get; set; }
        public List<string> PhoneNumbers { get; set; }


        public int Age { get; set; }
        public string NationalId { get; set; }
        public string Description { get; set; }
        public Gender Gender { get; set; }
     
    }
}
