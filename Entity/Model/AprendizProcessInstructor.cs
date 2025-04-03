using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Model
{
    public class AprendizProcessInstructor
    {
        public int id { get; set; }
        public int typeModalityId { get; set; }
        public TypeModality typeModality { get; set; }
        public int registerySofiaId { get; set; }
        public RegisterySofia registerySofia { get; set; }
        public int conceptId { get; set; }
        public Concept concept { get; set; }
        public int enterpriseId { get; set; }
        public Enterprise enterprise { get; set; }
        public int processId { get; set; }
        public Process process { get; set; }
        public int aprendizId { get; set; }
        public Aprendiz aprendiz { get; set; }
        public int instructorId { get; set; }
        public Instructor instructor { get; set; }
        public int stadeId { get; set; }
        public Stade stade { get; set; }
        public int verificationId { get; set; }
        public Verification verification { get; set; }  
    }
}
