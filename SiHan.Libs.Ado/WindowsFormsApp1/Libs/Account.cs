using SiHan.Libs.Ado;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1.Libs
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
