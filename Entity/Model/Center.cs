    using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Model
{
    public class Center
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string CodeCenter { get; set; }
        public string Address { get; set; }
        public bool Active { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime DeleteDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public int RegionalId { get; set; }
        public Regional Regional { get; set; }
    }
}
