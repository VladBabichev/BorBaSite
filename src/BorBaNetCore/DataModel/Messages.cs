using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BorBaNetCore.DataModel
{
    public partial class Messages
    {
        public int MessageId { get; set; }

        [Required(ErrorMessage = "NameErrorRequired")]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Subject")]
        public string Subject { get; set; }

        [Required(ErrorMessage = "EmailErrorMessage")]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email")]
        [MaxLength(50)]
        [EmailAddress( ErrorMessage = "EmailErrorMessage")]     
        public string Email { get; set; }

        [Display(Name = "Text")]
        public string Text { get; set; }

        [Display(Name = "RegDate")]
        public DateTime? RegDate { get; set; }
    }
}
