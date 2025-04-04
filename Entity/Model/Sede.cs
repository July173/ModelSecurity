using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Model
{
    public class Sede
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string CodeSede { get; set; }
        public string Address { get; set; }
        public string PhoneSede { get; set; }
        public string EmailContact { get; set; }
        public bool Active { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime DeleteDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public int CenterId { get; set; }
        public Center Center { get; set; }
    }
}
