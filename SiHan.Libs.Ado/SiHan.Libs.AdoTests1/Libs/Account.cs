using System;
using System.Collections.Generic;
using System.Text;
using SiHan.Libs.Ado;

namespace SiHan.Libs.AdoTests1.Libs
{
    public class Account : BaseEntity
    {
        [Column(Convert = typeof(GuidValueConvert))]
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        public string Email { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime LoginTime { get; set; }
        public decimal Money { get; set; }
    }
}
