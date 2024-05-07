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
        _collection.InsertMany(data);
    }
}