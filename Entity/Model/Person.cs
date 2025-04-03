using Entity.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Model
{
    public class Person
    {
        public int id { get; set; }
        public bool active { get; set; }
        public string name { get; set; }
        public string first_name { get; set; }
        public string second_name { get; set; }
        public string first_las_tname { get; set; }
        public string second_last_name { get; set; }
        public string phone_number { get; set; }
        public string email { get; set; }
        public string type_identification { get; set; }
        public int number_identification { get; set; }
        public bool signig { get; set; }
        public DateTime create_date { get; set; }
        public DateTime delete_date { get; set; }
        public DateTime update_date { get; set; }
    }
}
