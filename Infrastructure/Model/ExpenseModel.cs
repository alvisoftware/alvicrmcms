using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Model
{
    public class ExpenseModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Amount { get; set; }
        public DateTime Date { get; set; }
    }
}
