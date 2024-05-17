using MongoDB.Driver;

class Program
{
    static void Main(string[] args)
    {
        try
        {
            var connectionString = "mongodb://localhost:27017";
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase("SensorDatabase");
            var collection = database.GetCollection<SensorData>("SensorDataCollection");

            var repository = new SensorDataRepository(collection);
            var parser = new SensorDataParser(repository);

            parser.ParseAndStoreData("DH_HS5_HS2_E2_Data_corr_V2.csv");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }
}
