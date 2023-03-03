using Infrastructure.DBHelper;
using Infrastructure.Interface;
using Infrastructure.Model;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
    public class DashboardRepository : IDashboard,IDisposable
    {
        private DBConnect _dbConnect;
        private IMongoCollection<DashboardModel> _mongoCollect;

        public DashboardRepository()
        {
            _dbConnect = new DBConnect();
            _mongoCollect = _dbConnect.db.GetCollection<DashboardModel>(DBTableNames.DashBoard);
        }
        public void Dispose()
        {
            
        }

        public async Task<IEnumerable<DashboardModel>> GetDashboardData()
        {
            var pageFilter = Builders<DashboardModel>.Filter.Ne(x => x.Id, string.Empty);
            return _mongoCollect.FindAsync(pageFilter).Result.ToEnumerable();
        }
    }
}
