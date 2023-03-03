using Infrastructure.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Interface
{
    public interface IEmployeeLeave
    {
        Task<bool> ModifyLeaveDetails(EmployeeLeaveModel eLeave);
        Task<IEnumerable<EmployeeLeaveModel>> GetLeaveDetails();
    }
}
