using SiHan.Libs.Ado;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Libs
{
    [Table(Name ="account")]
    public class Account
    {
        [Key]
        [Column]
        public Guid Id { get; set; }

        [Column(Name = "username", Convert = typeof(GuidValueConvert))]
        public string UserName { get; set; }

        [Column(Name = "email")]
        public string Email { get; set; }

        [Column(Name = "password")]
        public string Password { get; set; }

        [Column(Name = "created_time")]
        public DateTime CreatedTime { get; set; }

        [Column(Name = "login_time")]
        public DateTime LoginTime { get; set; }
    }
}