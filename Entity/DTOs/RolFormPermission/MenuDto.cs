using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.DTOs.RolFormPermission
{
   public  class MenuDto
    {
        public string Rol { get; set; }
        public List<MenuFormDto> ModuleForm { get; set; }



    }
}
