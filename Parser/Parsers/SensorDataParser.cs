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
        var sensorData = ParseFile(filePath);
        _repository.InsertSensorData(sensorData);
    }

    private IEnumerable<SensorData> ParseFile(string filePath)
    {
        var lines = File.ReadAllLines(filePath);
        var sensorNames = new List<string>();

        foreach (var line in lines)
        {
            if (line.StartsWith("Date Time"))
            {
                sensorNames = ParseHeader(line);
                continue;
            }

            foreach (var sensorData in ParseLine(line, sensorNames))
            {
                yield return sensorData;
            }
        }
    }

    private List<string> ParseHeader(string headerLine)
    {
        return headerLine.Split(',').Skip(1).Select(name => name.Trim()).ToList();
    }

    private IEnumerable<SensorData> ParseLine(string line, List<string> sensorNames)
    {
        var dataParts = line.Split(',').Select(p => p.Trim()).ToArray();
        var measurementTime = DateTime.ParseExact(dataParts[0], "yyyy\\\\MM\\\\dd HH:mm:ss", CultureInfo.InvariantCulture);

        for (int i = 1; i < dataParts.Length; i++)
        {
            if (double.TryParse(dataParts[i], NumberStyles.Any, CultureInfo.InvariantCulture, out double measurementValue))
            {
                yield return new SensorData
                {
                    SensorId = sensorNames[i - 1],
                    MeasurementTime = measurementTime,
                    MeasurementValue = measurementValue
                };
            }
        }
    }
}