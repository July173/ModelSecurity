using Entity.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.DTOs
{
   public  class InstructorProgramDto
    {
        public int id { get; set; }
        public int instructorId { get; set; }
        public int programId { get; set; }
    }
}
