using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Model
{
    public class Form
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Cuestion { get; set; }
        public string TypeCuestion { get; set; }
        public string Answer { get; set; }
        public bool Active { get; set; }
        public int FormModuleId { get; set; }
        public ICollection<FormModule> FormModule { get; set; }

        public int RolFormId { get; set; }
        public ICollection<RolForm> RolForm { get; set; }

        public DateTime CreateDate { get; set; }
        public DateTime DeleteDate { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}