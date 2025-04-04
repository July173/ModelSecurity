using Entity.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.DTOs
{
    public class InstructorProgramDto
    {
        public int Id { get; set; }
        public int InstructorId { get; set; }
        public int ProgramId { get; set; }
    }
}
