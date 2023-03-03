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
    public class ExpenseRepository : IExpense, IDisposable
    {
        private DBConnect _dbConnect;
        private IMongoCollection<ExpenseModel> _mongoCollect;
        public ExpenseRepository()
        {
            _dbConnect = new DBConnect();
            _mongoCollect = _dbConnect.db.GetCollection<ExpenseModel>(DBTableNames.ExpenseTable);
        }
        public void Dispose()
        {
            
        }

        public async Task<IEnumerable<ExpenseModel>> GetExpenseDetails()
        {
            var pageFilter = Builders<ExpenseModel>.Filter.Ne(x => x.Id, string.Empty);
            return _mongoCollect.FindAsync(pageFilter).Result.ToEnumerable();
        }

        public async Task<bool> ModifyExpenseDetails(ExpenseModel expense)
        {
            try
            {
                if(!string.IsNullOrEmpty(expense.Name))
                {
                    expense.Id = expense.Name.ToPrimaryKey();
                }
                else if(!string.IsNullOrEmpty(expense.Amount))
                {
                    expense.Id=expense.Amount.ToPrimaryKey();
                }
                await _mongoCollect.InsertOneAsync(expense);
                return true;
            }
            catch 
            {
                return false;
            }
        }
    }
}
