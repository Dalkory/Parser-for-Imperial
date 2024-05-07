using MongoDB.Bson;

public class SensorData
{
    public ObjectId Id { get; set; }
    public string SensorId { get; set; }
    public DateTime MeasurementTime { get; set; }
    public double MeasurementValue { get; set; }
}