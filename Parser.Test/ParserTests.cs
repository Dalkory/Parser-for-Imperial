using Moq;

namespace SensorDataParserTests
{
    [TestClass]
    public class SensorDataParserTests
    {
        private Mock<ISensorDataRepository> _mockRepository;
        private SensorDataParser _parser;

        [TestInitialize]
        public void Initialize()
        {
            _mockRepository = new Mock<ISensorDataRepository>();
            _parser = new SensorDataParser(_mockRepository.Object);
        }

        [TestMethod] // ���� ���� ���������, ��� ����� ParseAndStoreData ��������� ������������ � ��������� ��� ������ ������.
        public void ParseAndStoreData_ShouldParseAllLines()
        {
            // Arrange
            var testData = new List<string>
            {
                "Date Time,Battery[V],Temperature,Humidity",
                "2015\\06\\15 12:39:10,13.3,10.7,63",
                "2015\\06\\15 15:39:10,13.4,17.1,59"
            };
            File.WriteAllLines("testData.txt", testData);

            // Act
            _parser.ParseAndStoreData("testData.txt");

            // Assert
            _mockRepository.Verify(repo => repo.InsertSensorData(It.IsAny<IEnumerable<SensorData>>()), Times.Once());
        }

        [TestMethod] // ���� ���� ����������, ��� ����� ParseAndStoreData ��������� ������������ ������ � ������� ���������� �����.
        public void ParseAndStoreData_ShouldHandleEmptyValues()
        {
            // Arrange
            var testData = new List<string>
            {
                "Date Time,Battery[V],Temperature,Humidity",
                "2015\\06\\15 12:39:10,13.3,,63"
            };
            File.WriteAllLines("testData.txt", testData);

            // Act
            _parser.ParseAndStoreData("testData.txt");

            // Assert
            _mockRepository.Verify(repo => repo.InsertSensorData(It.IsAny<IEnumerable<SensorData>>()), Times.Once());
        }

        [TestMethod] // ���� ���� ���������, ��� ����� ParseAndStoreData ����������� �� ��������, ����������� ������������ ������.
        public void ParseAndStoreData_ShouldHandleInvalidData()
        {
            // Arrange
            var testData = new List<string>
            {
                "Date Time,Battery[V],Temperature,Humidity",
                "2015\\06\\15 12:39:10,not_a_number,10.7,63"
            };
            File.WriteAllLines("testData.txt", testData);

            // Act
            _parser.ParseAndStoreData("testData.txt");

            // Assert
            _mockRepository.Verify(repo => repo.InsertSensorData(It.IsAny<IEnumerable<SensorData>>()), Times.Once());
        }

        [TestMethod] // ���� ���� ���������, ��� ����� ParseAndStoreData ��������� ������������ ������ � ����������� ������������� ��������.
        public void ParseAndStoreData_ShouldHandleMultipleHeaderLines()
        {
            // Arrange
            var testData = new List<string>
            {
                "Date Time,Battery[V],Temperature,Humidity",
                "Date Time,Battery[V],Temperature,Humidity",
                "2015\\06\\15 12:39:10,13.3,10.7,63"
            };
            File.WriteAllLines("testData.txt", testData);

            // Act
            _parser.ParseAndStoreData("testData.txt");

            // Assert
            _mockRepository.Verify(repo => repo.InsertSensorData(It.IsAny<IEnumerable<SensorData>>()), Times.Once());
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (File.Exists("testData.txt"))
            {
                File.Delete("testData.txt");
            }
        }
    }
}
