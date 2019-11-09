using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Reco.Models
{
    public class LoginUserModel
    {
        [Required(ErrorMessage = "Email este camp obligatoriu!")]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Parola este camp obligatoriu!")]
        [DataType(DataType.Password)]
        [Display(Name = "Parola")]
        public string Password { get; set; }
    }

    public class RegisterUserModel
    {
        [Required(ErrorMessage = "Nume este camp obligatoriu!")]
        [Display(Name = "Nume")]
        public string Surname { get; set; }

        [Required(ErrorMessage = "Prenume este camp obligatoriu!")]
        [Display(Name = "Prenume")]
        public string Forename { get; set; }

        [Required(ErrorMessage = "Email este camp obligatoriu!")]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Parola este camp obligatoriu!")]
        [DataType(DataType.Password)]
        [Display(Name = "Parola")]
        public string Password { get; set; }
    }

    public class EditProfileModel
    {
        public long Id { get; set; }

        [Required(ErrorMessage = "Nume este camp obligatoriu!")]
        [Display(Name = "Nume")]
        public string Surname { get; set; }

        [Required(ErrorMessage = "Prenume este camp obligatoriu!")]
        [Display(Name = "Prenume")]
        public string Forename { get; set; }

        public string Email { get; set; }

        public string ImageUrl { get; set; }
    }
}