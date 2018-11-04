using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using UHub.CoreLib.Attributes;
using UHub.CoreLib.DataInterop;
using UHub.CoreLib.Entities.Users.Interfaces;
using UHub.CoreLib.Tools;

namespace UHub.CoreLib.Entities.Users.DTOs
{
    [DtoClass(typeof(User))]
    public class User_CredentialDTO : DtoEntityBase, IUserCredential
    {
        [DisplayName("Email")]
        [Required]
        [StringLength(250, MinimumLength = 3, ErrorMessage = "Invalid email address")]
        [RegularExpression(RgxPatterns.User.EMAIL_B, ErrorMessage = "Email address is not valid")]
        public string Email { get; set; }


        [DisplayName("Password")]
        [Required]
        [StringLength(150, MinimumLength = 8, ErrorMessage = "Password must be between 8 and 150 characters")]
        [RegularExpression(RgxPatterns.User.PASSWORD_B, ErrorMessage = "Password must be between 8 and 150 characters")]
        public string Password { get; set; }
    }
}