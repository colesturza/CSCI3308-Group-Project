using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using UHub.CoreLib.Tools;

namespace UHub.Models.Account
{
    public class ClientCreds
    {
        [DisplayName("Email")]
        [Required]
        [StringLength(250, MinimumLength=3, ErrorMessage = "Invalid email address")]
        [RegularExpression(RgxPatterns.User.EMAIL)]
        public string Email { get; set; }


        [DisplayName("Password")]
        [Required]
        [StringLength(150, MinimumLength = 8, ErrorMessage = "Password must be between 8 and 150 characters")]
        [RegularExpression(RgxPatterns.User.PASSWORD)]
        public string Password { get; set; }


    }
}