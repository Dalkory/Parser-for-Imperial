using MongoDB.Driver;
using MoreLinq;

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

            var batchSize = 1; 
            var batches = data.Batch(batchSize);
            foreach (var batch in batches)
            {
                _collection.InsertMany(batch);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while inserting sensor data: {ex.Message}");
            throw;
        }
    }
}
