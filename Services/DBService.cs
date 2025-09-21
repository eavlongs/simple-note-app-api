using Microsoft.Data.SqlClient;
using Dapper;

namespace simple_note_app_api.Services
{
    public interface IDBService
    {
        Task<T> GetOne<T>(string command, object parms);
        Task<List<T>> GetAll<T>(string command, object parms);
        Task<int> Execute(string command, object parms);

        Task<T> CreateOrSave<T>(string command, object parms);
        SqlConnection GetDbConnection();
    }

    public class DBService: IDBService
    {
        private readonly SqlConnection _db;
        public DBService(IConfiguration configuration)
        {
            _db = new SqlConnection(configuration.GetConnectionString(Constants.DB_CONNECTION));
        }

        public SqlConnection GetDbConnection()
        {
            return _db;
        }

        public async Task<T> GetOne<T>(string command, object parms)
        {
            T result;

            result = (await _db.QueryAsync<T>(command, parms).ConfigureAwait(false)).FirstOrDefault();

            return result;

        }

        public async Task<List<T>> GetAll<T>(string command, object parms)
        {

            List<T> result = new List<T>();

            result = (await _db.QueryAsync<T>(command, parms)).ToList();

            return result;
        }

        public async Task<int> Execute(string command, object parms)
        {
            int result;

            result = await _db.ExecuteAsync(command, parms);

            return result;
        }
        public async Task<T> CreateOrSave<T>(string command, object parms)
        {
            T result;
            result = await _db.QuerySingleAsync<T>(command, parms);
            return result;
        }
    }
}
