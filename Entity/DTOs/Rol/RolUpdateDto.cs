using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.DTOs.Rol
{
    public class RolUpdateDto
    {
        public int Id { get; set; }
        public string TypeRol { get; set; }
        public string Description { get; set; }
    }
}
