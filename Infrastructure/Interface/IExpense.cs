using Infrastructure.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Interface
{
    public interface IExpense
    {
        Task<bool> ModifyExpenseDetails(ExpenseModel expense);
        Task<IEnumerable<ExpenseModel>> GetExpenseDetails();
    }
}
