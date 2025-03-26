using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Model
{
    class ChangeLog
    {
        public int Id { get; set; }
        public DateTime CreateAt { get; set; }
        public string TableName { get; set; }
        public int IdUser { get; set; }
        public string Action { get; set; }
        public string Observation { get; set; }
        public string AfectedId { get; set; }
    }
}
