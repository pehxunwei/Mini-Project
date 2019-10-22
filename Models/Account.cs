//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Mini_Project.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class Account
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        [Display(Name = "User Name")]
        [DataType(DataType.EmailAddress)]
        public string UserName { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,100}$", ErrorMessage = "Invalid Username/Password")]
        public string Password { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        [DataType(DataType.PhoneNumber)]
        //[RegularExpression(@"^\+65[9|8]\d{7}$", ErrorMessage = "Invalid phone number")]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        [Display(Name = "Birthday")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        [DataType(DataType.Date)]
        public Nullable<System.DateTime> Birthday { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        [Display(Name = "Gender")]
        public string Gender { get; set; }
        public Nullable<bool> EmailConfirmed { get; set; }
        public Nullable<int> otpValuePasswordReset { get; set; }
        public Nullable<int> otpValueUserName { get; set; }
        public Nullable<bool> Terminated { get; set; }
        public string ActivationCode { get; set; }
        public Nullable<bool> Locked { get; set; }
        public Nullable<int> AddCount { get; set; }
    }
}
