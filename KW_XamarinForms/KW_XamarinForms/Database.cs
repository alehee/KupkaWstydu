using System.Collections.Generic;
using System.Threading.Tasks;
using SQLite;

namespace KW_XamarinForms
{
    public class Database
    {
        readonly SQLiteAsyncConnection _database;
        MySqlConnector mySqlConnector = new MySqlConnector();

        public Database(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<DT_Category>().Wait();
            _database.CreateTableAsync<DT_Arrear>().Wait();
        }

        public void TerminateDatabase()
        {
            _database.DeleteAllAsync<DT_Category>();
            _database.DeleteAllAsync<DT_Arrear>();
        }

        public Task<List<DT_Category>> GetCategoriesAsync()
        {
            return _database.Table<DT_Category>().ToListAsync();
        }

        public Task<int> SaveCategoriesAsync(DT_Category category)
        {
            return _database.InsertAsync(category);
        }

        public async Task DeleteArrearsAsync(short arrearId)
        {
            _database.DeleteAsync<DT_Arrear>(arrearId);
        }

        public async Task DeleteCategoryAsync(short categoryId)
        {
            _database.DeleteAsync<DT_Category>(categoryId);
        }

        public Task<List<DT_Arrear>> GetArrearsAsync()
        {
            return _database.Table<DT_Arrear>().ToListAsync();
        }

        public Task<int> SaveArrearAsync(DT_Arrear arrear)
        {
            return _database.InsertAsync(arrear);
        }

        public async Task UpdateArrearAsync(short arrearId, short updateValue)
        {
            _database.QueryAsync<DT_Arrear>("UPDATE DT_Arrear SET status=" + updateValue + " WHERE id=" + arrearId + "");
        }

        public async Task HideArrearsInCategoryAsync(short categoryId, bool updateValue)
        {
            if (updateValue)
            {
                _database.QueryAsync<DT_Category>("UPDATE DT_Category SET hidden=1 WHERE id=" + categoryId);
            }

            else
            {
                _database.QueryAsync<DT_Category>("UPDATE DT_Category SET hidden=0 WHERE id=" + categoryId);
            }
                
        }

        public async Task DbUpdate()
        {
            _database.QueryAsync<DT_Category>("UPDATE DT_Category SET hidden=1");
        }
    }
}