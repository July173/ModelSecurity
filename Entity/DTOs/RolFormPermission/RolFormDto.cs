using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.DTOs.RolFormPermission
{
    public class RolFormDto
    {
        public string Rol { get; set; }                                 // Nombre del rol
        public List<FormWithPermissionDto> Form { get; set; } // Lista de formularios con permisos
    }
}

