using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Driver;
using Moq;

namespace SensorDataParserTests
{
    [TestClass]
    public class DataParserTests
    {
        private Mock<IMongoCollection<SensorData>> _mockCollection;
        private DataParser _dataParser;

        [TestInitialize]
        public void Initialize()
        {
            // Mock the MongoDB collection
            _mockCollection = new Mock<IMongoCollection<SensorData>>();

            // Mock the InsertOne method to do nothing
            _mockCollection.Setup(c => c.InsertOne(It.IsAny<SensorData>(), null, default));

            // Initialize the DataParser with the mocked collection
            _dataParser = new DataParser("mockConnectionString", "mockDatabase", "mockCollection")
            {
                _collection = _mockCollection.Object
            };
        }

        [TestMethod]
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
            _dataParser.ParseAndStoreData("testData.txt");

            // Assert
            _mockCollection.Verify(c => c.InsertOne(It.IsAny<SensorData>(), null, default), Times.Exactly(6));
        }

        [TestMethod]
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
            _dataParser.ParseAndStoreData("testData.txt");

            // Assert
            _mockCollection.Verify(c => c.InsertOne(It.IsAny<SensorData>(), null, default), Times.Exactly(2));
        }

        [TestMethod]
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
            _dataParser.ParseAndStoreData("testData.txt");

            // Assert
            _mockCollection.Verify(c => c.InsertOne(It.IsAny<SensorData>(), null, default), Times.Exactly(2));
        }

        [TestMethod]
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
            _dataParser.ParseAndStoreData("testData.txt");

            // Assert
            _mockCollection.Verify(c => c.InsertOne(It.IsAny<SensorData>(), null, default), Times.Exactly(3));
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
