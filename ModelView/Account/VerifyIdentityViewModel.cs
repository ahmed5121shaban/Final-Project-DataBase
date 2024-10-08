﻿using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace Final.ViewModels
{
    public class VerifyIdentityViewModel
    {
        [Required]
        [StringLength(40, ErrorMessage = "First Name cannot exceed 40 characters.")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(40, ErrorMessage = "Last Name cannot exceed 40 characters.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Birth date is required.")]
        [DataType(DataType.Date)]
        public DateTime BarthDate { get; set; }

        [Required]
        [StringLength(20, ErrorMessage = "ID number cannot exceed 20 characters.")]
        public string IdNumber { get; set; }

        [Required(ErrorMessage = "ID document is required.")]
        public IFormFile IdDocument { get; set; }
    }
}
