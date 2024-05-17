using System.Globalization;

public class SensorDataParser
{
    private readonly ISensorDataRepository _repository;

    public SensorDataParser(ISensorDataRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public void ParseAndStoreData(string filePath)
    {
        try
        {
            var sensorData = ParseFile(filePath).ToList();
            if (sensorData.Any())
            {
                _repository.InsertSensorData(sensorData);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while parsing and storing sensor data: {ex.Message}");
            throw;
        }
    }

    private IEnumerable<SensorData> ParseFile(string filePath)
    {
        using (var reader = new StreamReader(filePath))
        {
            string line;
            var headerProcessed = false;
            while ((line = reader.ReadLine()) != null)
            {
                if (line.StartsWith("DateTime") && !headerProcessed)
                {
                    headerProcessed = true;
                    continue;
                }

                var sensorData = ParseLine(line);
                if (sensorData != null)
                {
                    yield return sensorData;
                }
            }
        }
    }

    private SensorData ParseLine(string line)
    {
        var dataParts = line.Split(';').Select(p => p.Trim()).ToArray();

        if (dataParts.Length < 12)
            return null;

        if (!DateTime.TryParseExact(dataParts[0], "dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out var measurementTime))
            return null;

        var sensorName = dataParts[1];
        if (!double.TryParse(dataParts[10], NumberStyles.Any, CultureInfo.InvariantCulture, out double value1))
            return null;
        if (!double.TryParse(dataParts[11], NumberStyles.Any, CultureInfo.InvariantCulture, out double value2))
            return null;

        return new SensorData
        {
            SensorName = sensorName,
            MeasurementTime = measurementTime,
            Value1 = value1,
            Value2 = value2
        };
    }
}
