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

        [TestMethod] // Проверяет правильно ли ParseAndStoreData обрабатывает строки с пустыми значениями полей.
        public void ParseAndStoreData_ShouldHandleEmptyValues()
        {
            // Arrange
            var testData = new List<string>
            {
                "DateTime;SensorName;CustomerName;Flags;SensorType;Unit;East;North;Height;km;VALUE1;VALUE2",
                "28.06.2022 15:30:31;TurbineMonitoring:DH_HS5_HS2_E2_comp;;;HydrostaticLevel;Hectopascal;0.0000;0.0000;0.0000;0.0000;;15.0"
            };
            File.WriteAllLines("testData.csv", testData);

            // Act
            _parser.ParseAndStoreData("testData.csv");

            // Assert
            _mockRepository.Verify(repo => repo.InsertSensorData(It.IsAny<IEnumerable<SensorData>>()), Times.Never());
        }

        [TestMethod] // Проверяет справляется ли ParseAndStoreData со строками, содержащими недопустимые данные.
        public void ParseAndStoreData_ShouldHandleInvalidData()
        {
            // Arrange
            var testData = new List<string>
            {
                "DateTime;SensorName;CustomerName;Flags;SensorType;Unit;East;North;Height;km;VALUE1;VALUE2",
                "28.06.2022 15:30:31;TurbineMonitoring:DH_HS5_HS2_E2_comp;;;HydrostaticLevel;Hectopascal;0.0000;0.0000;0.0000;0.0000;not_a_number;15.0"
            };
            File.WriteAllLines("testData.csv", testData);

            // Act
            _parser.ParseAndStoreData("testData.csv");

            // Assert
            _mockRepository.Verify(repo => repo.InsertSensorData(It.IsAny<IEnumerable<SensorData>>()), Times.Never());
        }

        [TestMethod] // Проверяет правильно ли ParseAndStoreData обрабатывает случаи с несколькими заголовочными строками.
        public void ParseAndStoreData_ShouldHandleMultipleHeaderLines()
        {
            // Arrange
            var testData = new List<string>
            {
                "DateTime;SensorName;CustomerName;Flags;SensorType;Unit;East;North;Height;km;VALUE1;VALUE2",
                "DateTime;SensorName;CustomerName;Flags;SensorType;Unit;East;North;Height;km;VALUE1;VALUE2",
                "28.06.2022 15:30:31;TurbineMonitoring:DH_HS5_HS2_E2_comp;;;HydrostaticLevel;Hectopascal;0.0000;0.0000;0.0000;0.0000;0.0;15.0"
            };
            File.WriteAllLines("testData.csv", testData);

            // Act
            _parser.ParseAndStoreData("testData.csv");

            // Assert
            _mockRepository.Verify(repo => repo.InsertSensorData(It.Is<IEnumerable<SensorData>>(data => data.Count() == 1)), Times.Once());
        }

        [TestMethod] // Проверяет корректно ли обрабатывает ParseAndStoreData пустой файл.
        public void ParseAndStoreData_ShouldHandleEmptyFile()
        {
            // Arrange
            var testData = new List<string>();
            File.WriteAllLines("testData.csv", testData);

            // Act
            _parser.ParseAndStoreData("testData.csv");

            // Assert
            _mockRepository.Verify(repo => repo.InsertSensorData(It.IsAny<IEnumerable<SensorData>>()), Times.Never());
        }

        [TestMethod] // Проверяет пропускает ли недопустимые строки и обрабатывает только допустимые строки ParseAndStoreData.
        public void ParseAndStoreData_ShouldSkipInvalidLines()
        {
            // Arrange
            var testData = new List<string>
            {
                "DateTime;SensorName;CustomerName;Flags;SensorType;Unit;East;North;Height;km;VALUE1;VALUE2",
                "invalid line",
                "28.06.2022 15:30:31;TurbineMonitoring:DH_HS5_HS2_E2_comp;;;HydrostaticLevel;Hectopascal;0.0000;0.0000;0.0000;0.0000;0.0;15.0"
            };
            File.WriteAllLines("testData.csv", testData);

            // Act
            _parser.ParseAndStoreData("testData.csv");

            // Assert
            _mockRepository.Verify(repo => repo.InsertSensorData(It.Is<IEnumerable<SensorData>>(data => data.Count() == 1)), Times.Once());
        }

        [TestMethod] // Проверяет бросает ли исключение ParseAndStoreData, если файл не существует.
        [ExpectedException(typeof(FileNotFoundException))]
        public void ParseAndStoreData_ShouldThrowArgumentException_WhenFileDoesNotExist()
        {
            // Act & Assert
            _parser.ParseAndStoreData("nonexistent.csv");
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (File.Exists("testData.csv"))
            {
                File.Delete("testData.csv");
            }
        }
    }
}
