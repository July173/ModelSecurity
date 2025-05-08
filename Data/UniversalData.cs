using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entity.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Data
{
    public class UniversalData<T> : BaseData<T> where T : class
    {
        public UniversalData(ApplicationDbContext context) : base(context) { }
    }

}
