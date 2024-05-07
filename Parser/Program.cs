using MongoDB.Driver;

class Program
{
    static void Main(string[] args)
    {
        var connectionString = "mongodb://localhost:27017";
        var client = new MongoClient(connectionString);
        var database = client.GetDatabase("SensorDatabase");
        var collection = database.GetCollection<SensorData>("SensorDataCollection");

        var repository = new SensorDataRepository(collection);
        var parser = new SensorDataParser(repository);

        parser.ParseAndStoreData("OMNI-mLog.txt");
    }
}
