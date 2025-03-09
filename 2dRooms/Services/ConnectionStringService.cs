namespace _2dRooms.Services
{
    public class ConnectionStringService
    {
        private readonly string _connectionString;

        public ConnectionStringService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public string GetConnectionString()
        {
            return _connectionString;
        }
    }
}
