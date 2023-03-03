using Infrastructure.DBHelper;
using Infrastructure.Interface;
using Infrastructure.Model;
using Infrastructure.Utility;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
    public class EmployeeLeaveRepository : IEmployeeLeave, IDisposable
    {
        private DBConnect _DBConnect;
        private IMongoCollection<EmployeeLeaveModel> _mongoColection;

        public EmployeeLeaveRepository()
        {
            _DBConnect = new DBConnect();
            _mongoColection = _DBConnect.db.GetCollection<EmployeeLeaveModel>(DBTableNames.EmployeeLeave);
        }

        public void Dispose()
        {


        }
        public async Task<IEnumerable<EmployeeLeaveModel>> GetLeaveDetails()
        {
            var pageFilter = Builders<EmployeeLeaveModel>.Filter.Ne(x => x.id, string.Empty);
            return _mongoColection.FindAsync(pageFilter).Result.ToEnumerable();
        }

        public async Task<bool> ModifyLeaveDetails(EmployeeLeaveModel eLeave)
        {
            try
            {
                if (!string.IsNullOrEmpty(eLeave.EmployeeName))
                {
                    eLeave.id = eLeave.EmployeeName.ToPrimaryKey();
                }
                
                await _mongoColection.InsertOneAsync(eLeave);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
