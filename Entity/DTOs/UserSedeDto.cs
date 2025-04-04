using Entity.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.DTOs
{
    public class UserSedeDto
    {
        public int Id { get; set; }
        public string StatusProcedure { get; set; }
        public int UserId { get; set; }
        public int SedeId { get; set; }
    }
}
