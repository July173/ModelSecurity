    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    namespace Entity.DTOs.RolFormPermission
    {
        public  class MenuFormDto
        {
            public string Name { get; set; }
            public List<FormItemDto> Form { get; set; }          // Lista de  formularios

        }
    }
