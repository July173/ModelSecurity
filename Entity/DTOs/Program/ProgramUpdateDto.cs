using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.DTOs.Program
{
    public class ProgramUpdateDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string TypeProgram { get; set; }
        public string Description { get; set; }
    }
}
