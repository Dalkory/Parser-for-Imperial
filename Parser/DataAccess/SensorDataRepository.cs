using MongoDB.Driver;

public class SensorDataRepository : ISensorDataRepository
{
    private readonly IMongoCollection<SensorData> _collection;

    public SensorDataRepository(IMongoCollection<SensorData> collection)
    {
        _collection = collection ?? throw new ArgumentNullException(nameof(collection));
    }

    public void InsertSensorData(IEnumerable<SensorData> data)
    {
        try
        {
            if (data == null || !data.Any())
            {
                throw new ArgumentException("Must contain at least 1 request.", nameof(data));
            }

            _collection.InsertMany(data);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while inserting sensor data: {ex.Message}");
            throw;
        }
    }
}
