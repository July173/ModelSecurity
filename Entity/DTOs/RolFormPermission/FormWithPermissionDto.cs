using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.DTOs.RolFormPermission
{
    public  class FormWithPermissionDto
    {
        public string Name  { get; set; }                  // Nombre del formulario 
        public List<string> Permission { get; set; }          // Lista de permisos 
    }
}
