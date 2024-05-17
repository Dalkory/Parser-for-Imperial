using MongoDB.Bson;

public class SensorData
{
    public ObjectId Id { get; set; }
    public string SensorName { get; set; }
    public DateTime MeasurementTime { get; set; }
    public double Value1 { get; set; }
    public double Value2 { get; set; }
}
