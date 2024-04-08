using IPLogAnalyzer.Services;
using System.Net;

namespace IPLogAnalyzerTests
{
    [TestFixture]
    internal class FileServiceTests
    {
        private const string TestLogFilePath = "test_log.txt";
        private const string TestOutputFilePath = "test_output.txt";

        [OneTimeSetUp]
        public void Setup()
        {
            using (StreamWriter writer = new StreamWriter(TestLogFilePath))
            {
                writer.WriteLine("192.168.1.1:2024-04-01 10:00:00");
                writer.WriteLine("192.168.1.2:2024-04-01 11:00:00");
                writer.WriteLine("10.0.0.1:2024-04-02 12:00:00");
            }
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            File.Delete(TestLogFilePath);
        }

        [Test]
        public void ReadLogFile_GivenValidFileLog_ReturnCorrectLogEntries()
        {
            // Arrange 
            string testLogFilePath = Path.Combine(Path.GetTempPath(), "test_log.txt");
            string[] lines = { "192.168.1.1:2024-04-01 10:00:00", "192.168.1.2:2024-04-01 11:00:00", "10.0.0.1:2024-04-02 12:00:00" };
            File.WriteAllLines(testLogFilePath, lines);

            // Act
            var logEntries = FileService.ReadLogFile(testLogFilePath);

            // Assert
            Assert.That(logEntries.Count, Is.EqualTo(3));
            Assert.That(logEntries[0].Address, Is.EqualTo(IPAddress.Parse("192.168.1.1")));
            Assert.That(logEntries[0].Timestamp, Is.EqualTo(new DateTime(2024, 4, 1, 10, 0, 0)));
            Assert.That(logEntries[1].Address, Is.EqualTo(IPAddress.Parse("192.168.1.2")));
            Assert.That(logEntries[1].Timestamp, Is.EqualTo(new DateTime(2024, 4, 1, 11, 0, 0)));
            Assert.That(logEntries[2].Address, Is.EqualTo(IPAddress.Parse("10.0.0.1")));
            Assert.That(logEntries[2].Timestamp, Is.EqualTo(new DateTime(2024, 4, 2, 12, 0, 0)));

            File.Delete(testLogFilePath);
        }

        [Test]
        public void ReadLogFile_GivenInvalidFilePath_ReturnsFileDoesNotExist()
        {
            // Arrange
            string nonExistentFilePath = "non_existent_log.txt";

            // Act & Assert
            Assert.Throws<FileNotFoundException>(() => FileService.ReadLogFile(nonExistentFilePath));
        }

        [Test]
        public void WriteResultToFile_GivenValidEntry_ReturnsCorrectOutputFile()
        {
            // Arrange
            var ipAddressCounts = new Dictionary<IPAddress, int>
            {
                { IPAddress.Parse("192.168.1.1"), 5 },
                { IPAddress.Parse("192.168.1.2"), 10 },
                { IPAddress.Parse("10.0.0.1"), 3 }
            };

            // Act
            FileService.WriteResultToFile(TestOutputFilePath, ipAddressCounts);

            // Assert
            Assert.That(File.Exists(TestOutputFilePath));
            string[] lines = File.ReadAllLines(TestOutputFilePath);
            Assert.That(lines.Length, Is.EqualTo(3));
            Assert.That(lines[0], Is.EqualTo("192.168.1.1 - 5"));
            Assert.That(lines[1], Is.EqualTo("192.168.1.2 - 10"));
            Assert.That(lines[2], Is.EqualTo("10.0.0.1 - 3"));

            File.Delete(TestOutputFilePath);
        }

        [Test]
        public void ReadLogFile_GivenInvalidIPAddressFormat_ReturnsInfoInConsoleAndCorrectFile()
        {
            using (StringWriter sw = new StringWriter())
            {
                // Arrange
                string[] invalidLines = { "192.168.1.1:2024-04-01 10:00:00", "192.168..1:2024-04-01 11:00:00" };
                File.WriteAllLines(TestLogFilePath, invalidLines);

                // Act
                Console.SetOut(sw);
                var readLogFile = FileService.ReadLogFile(TestLogFilePath);

                // Assert
                Assert.That(sw.ToString(), Is.EqualTo("Не удалось разобрать IP-адрес в строке: 192.168..1:2024-04-01 11:00:00\r\n"));
                Assert.That(readLogFile.Count, Is.EqualTo(1));
            }
        }

        [Test]
        public void ReadLogFile_GivenInvalidTimestampFormat_ReturnsInfoInConsoleAndCorrectFile()
        {
            using (StringWriter sw = new StringWriter())
            {
                // Arrange
                string[] invalidLines = { "192.168.1.1:2024-04-01 10:00:00", "192.168.1.2:2024.04.01 11:00:00" };
                File.WriteAllLines(TestLogFilePath, invalidLines);

                // Act
                Console.SetOut(sw);
                var readLogFile = FileService.ReadLogFile(TestLogFilePath);

                // Assert
                Assert.That(sw.ToString(), Is.EqualTo("Не удалось разобрать время в строке: 192.168.1.2:2024.04.01 11:00:00\r\n"));
                Assert.That(readLogFile.Count, Is.EqualTo(1));
            }
        }

        [Test]
        public void ReadLogFile_GivenIncorrectLineFormat_ReturnsInfoInConsoleAndCorrectFile()
        {
            using (StringWriter sw = new StringWriter())
            {
                // Arrange
                string[] invalidLines = { "192.168.1.1:2024-04-01 10:00:00", "192.168.1.2" };
                File.WriteAllLines(TestLogFilePath, invalidLines);

                // Act
                Console.SetOut(sw);
                var readLogFile = FileService.ReadLogFile(TestLogFilePath);

                // Assert
                Assert.That(sw.ToString(), Is.EqualTo("Некорректный формат строки в файле логов: 192.168.1.2\r\n"));
                Assert.That(readLogFile.Count, Is.EqualTo(1));
            }
        }
    }
}
