using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Model
{
    public class AprendizProgram
    {
        public int id { get; set; }
        public int aprendizId { get; set; }
        public Aprendiz aprendiz { get; set; }
        public int programId { get; set; }
        public Program program { get; set; }
    }
}
