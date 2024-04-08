using IPLogAnalyzer.Handlers;
using IPLogAnalyzer.Models;
using IPLogAnalyzer.Services;
using Microsoft.Extensions.Configuration;
using Moq;

namespace IPLogAnalyzerTests
{
    [TestFixture]
    internal class ParseServiceTests
    {
        [Test]
        public void ParseCommandLineArgs_GivenEmptyArgs_ReturnsArgumentException()
        {
            // Arrange
            string[] args = [];

            // Act & Assert
            Assert.Throws<ArgumentException>(() => ParseService.ParseCommandLineArgs(args), "Недостаточно аргументов. Использование: --file-log <путь к файлу> --file-output <путь к файлу> [--address-start <IP-адрес>] [--address-mask <маска подсети>] --time-start <начальное время> --time-end <конечное время>Недостаточно аргументов. Использование: --file-log <путь к файлу> --file-output <путь к файлу> [--address-start <IP-адрес>] [--address-mask <маска подсети>] [--time-start <начальное время>] [--time-end <конечное время>]");
        }

        [Test]
        public void ParseCommandLineArgs_GivenValidArguments_ReturnsParameters()
        {
            // Arrange
            string[] args = 
            {
                "--file-log", "log.txt",
                "--file-output", "output.txt",
                "--address-start", "192.168.1.1",
                "--address-mask", "255.255.255.0",
                "--time-start", "01.04.2024",
                "--time-end", "03.04.2024"
            };

            // Act
            var actual = ParseService.ParseCommandLineArgs(args);

            // Assert
            Assert.That(actual.LogFilePath, Is.EqualTo("log.txt"));
            Assert.That(actual.OutputFilePath, Is.EqualTo("output.txt"));
            Assert.That(actual.StartAddress.ToString(), Is.EqualTo("192.168.1.1"));
            Assert.That(actual.Mask.ToString(), Is.EqualTo("255.255.255.0"));
            Assert.That(actual.StartTime, Is.EqualTo(new DateTime(2024, 4, 1)));
            Assert.That(actual.EndTime, Is.EqualTo(new DateTime(2024, 4, 3)));
        }

        [Test]
        public void ParseCommandLineArgs_GivenOnlyRequiredArguments_ReturnsParameters()
        {
            // Arrange
            string[] args =
            {
                "--file-log", "log.txt",
                "--file-output", "output.txt",
            };

            // Act
            var actual = ParseService.ParseCommandLineArgs(args);

            // Assert
            Assert.That(actual.LogFilePath, Is.EqualTo("log.txt"));
            Assert.That(actual.OutputFilePath, Is.EqualTo("output.txt"));
            Assert.That(actual.StartAddress, Is.Null);
            Assert.That(actual.Mask, Is.Null);
            Assert.That(actual.StartTime, Is.EqualTo(DateTime.MinValue));
            Assert.That(actual.EndTime, Is.EqualTo(DateTime.MaxValue));
        }

        [Test]
        public void ParseCommandLineArgs_GivenArgsWithoutLogFilePath_ReturnsArgumentException()
        {
            // Arrange
            string[] args =
            {
                "--file-output", "output.txt",
                "--address-start", "192.168.1.1",
                "--address-mask", "255.255.255.0",
                "--time-start", "01.04.2024",
                "--time-end", "03.04.2024"
            };

            // Act && Assert
            Assert.Throws<ArgumentException>(() => ParseService.ParseCommandLineArgs(args), "Не указан путь к файлу логов или к файлу вывода");
        }

        [Test]
        public void ParseCommandLineArgs_GivenArgsWithInvalidParameter_ReturnsArgumentException()
        {
            // Arrange
            string[] args =
            {
                "--file-log", "log.txt",
                "--file-output", "output.txt",
                "--unknown-param", "value"
            };

            // Act && Assert
            Assert.Throws<ArgumentException>(() => ParseService.ParseCommandLineArgs(args));
        }

        [Test]
        public void ParseCommandLineArgs_GivenArgsWithoutFileOutput_ReturnsArgumentException()
        {
            // Arrange
            string[] args =
            {
                "--file-log", "log.txt",
                "--address-start", "192.168.1.1",
                "--address-mask", "255.255.255.0",
                "--time-start", "01.04.2024",
                "--time-end", "03.04.2024"
            };

            // Act && Assert
            Assert.Throws<ArgumentException>(() => ParseService.ParseCommandLineArgs(args), "Не указан путь к файлу логов или к файлу вывода");
        }

        [Test]
        public void ParseCommandLineArgs_GivenArgsWithoutStartAddress_ReturnsArgumentException()
        {
            // Arrange
            string[] args =
            {
                "--file-log", "log.txt",
                "--file-output", "output.txt",
                "--address-mask", "255.255.255.0",
                "--time-start", "01.04.2024",
                "--time-end", "03.04.2024"
            };

            // Act && Assert
            Assert.Throws<ArgumentException>(() => ParseService.ParseCommandLineArgs(args), "Параметр --address-mask может использоваться только с параметром --address-start");
        }

        [Test]
        public void ParseCommandLineArgs_GivenArgsWhenStartTimeIsGreaterThanEndTime_ReturnsArgumentException()
        {
            // Arrange
            string[] args =
            {
                "--file-log", "log.txt",
                "--file-output", "output.txt",
                "--address-start", "192.168.1.1",
                "--address-mask", "255.255.255.0",
                "--time-start", "04.04.2024",
                "--time-end", "03.04.2024"
            };

            // Act && Assert
            Assert.Throws<ArgumentException>(() => ParseService.ParseCommandLineArgs(args), "Параметр --time-start должен быть больше --time-end");

        }

        [Test]
        public void ParseCommandLineArgs_GivenArgsWithoutStartTime_ReturnsParameters()
        {
            // Arrange
            string[] args =
            {
                "--file-log", "log.txt",
                "--file-output", "output.txt",
                "--address-start", "192.168.1.1",
                "--address-mask", "255.255.255.0",
                "--time-end", "03.04.2024"
            };

            // Act
            var actual = ParseService.ParseCommandLineArgs(args);

            // Assert
            Assert.That(actual.LogFilePath, Is.EqualTo("log.txt"));
            Assert.That(actual.OutputFilePath, Is.EqualTo("output.txt"));
            Assert.That(actual.StartAddress.ToString(), Is.EqualTo("192.168.1.1"));
            Assert.That(actual.Mask.ToString(), Is.EqualTo("255.255.255.0"));
            Assert.That(actual.StartTime, Is.EqualTo(DateTime.MinValue));
            Assert.That(actual.EndTime, Is.EqualTo(new DateTime(2024, 4, 3)));
        }

        [Test]
        public void ParseCommandLineArgs_GivenArgsWithoutEndTime_ReturnsParameters()
        {
            // Arrange
            string[] args =
            {
                "--file-log", "log.txt",
                "--file-output", "output.txt",
                "--address-start", "192.168.1.1",
                "--address-mask", "255.255.255.0",
                "--time-start", "01.04.2024"
            };

            // Act
            var actual = ParseService.ParseCommandLineArgs(args);

            // Assert
            Assert.That(actual.LogFilePath, Is.EqualTo("log.txt"));
            Assert.That(actual.OutputFilePath, Is.EqualTo("output.txt"));
            Assert.That(actual.StartAddress.ToString(), Is.EqualTo("192.168.1.1"));
            Assert.That(actual.Mask.ToString(), Is.EqualTo("255.255.255.0"));
            Assert.That(actual.StartTime, Is.EqualTo(new DateTime(2024, 4, 1)));
            Assert.That(actual.EndTime, Is.EqualTo(DateTime.MaxValue));
        }

        [Test]
        public void ParseCommandLineArgs_GivenArgsWithInvalidStartAddress_ReturnsFormatException()
        {
            // Arrange
            string[] args =
            {
                "--file-log", "log.txt",
                "--file-output", "output.txt",
                "--address-start", "192.168..1",
                "--address-mask", "255.255.255.0",
                "--time-start", "01-04-2024",
                "--time-end", "03.04.2024"
            };

            // Act && Assert
            Assert.Throws<FormatException>(() => ParseService.ParseCommandLineArgs(args));
        }

        [Test]
        public void ParseCommandLineArgs_GivenArgsWithInvalidAddressMask_ReturnsFormatException()
        {
            // Arrange
            string[] args =
            {
                "--file-log", "log.txt",
                "--file-output", "output.txt",
                "--address-start", "192.168.1.1",
                "--address-mask", "255.255..0",
                "--time-start", "01.04.2024",
                "--time-end", "03.04.2024"
            };

            // Act && Assert
            Assert.Throws<FormatException>(() => ParseService.ParseCommandLineArgs(args));
        }

        [Test]
        public void ParseCommandLineArgs_GivenArgsWithInvalidStartTime_ReturnsFormatException()
        {
            // Arrange
            string[] args =
            {
                "--file-log", "log.txt",
                "--file-output", "output.txt",
                "--address-start", "192.168.1.1",
                "--address-mask", "255.255.255.0",
                "--time-start", "01-04-2024",
                "--time-end", "03.04.2024"
            };

            // Act && Assert
            Assert.Throws<FormatException>(() => ParseService.ParseCommandLineArgs(args));
        }

        [Test]
        public void ParseCommandLineArgs_GivenArgsWithInvalidEndTime_ReturnsFormatException()
        {
            // Arrange
            string[] args =
            {
                "--file-log", "log.txt",
                "--file-output", "output.txt",
                "--address-start", "192.168.1.1",
                "--address-mask", "255.255.255.0",
                "--time-start", "01.04.2024",
                "--time-end", "03-04-2024"
            };

            // Act && Assert
            Assert.Throws<FormatException>(() => ParseService.ParseCommandLineArgs(args));
        }

        [Test]
        public void ParseCommandLineArgs_GivenArgsWithEmptyFileLog_ReturnsArgumentException()
        {
            // Arrange
            string[] args =
            {
                "--file-log", 
                "--file-output", "output.txt",
                "--address-start", "192.168.1.1",
                "--address-mask", "255.255.255.0",
                "--time-start", "01.04.2024",
                "--time-end", "03.04.2024"
            };

            // Act && Assert
            Assert.Throws<ArgumentException>(() => ParseService.ParseCommandLineArgs(args), "Отсутствует путь к файлу логов после параметра --file-log");
        }

        [Test]
        public void ParseCommandLineArgs_GivenArgsWithEmptyFileOutput_ReturnsArgumentException()
        {
            // Arrange
            string[] args =
            {
                "--file-log", "log.txt",
                "--file-output",
                "--address-start", "192.168.1.1",
                "--address-mask", "255.255.255.0",
                "--time-start", "01.04.2024",
                "--time-end", "03.04.2024"
            };

            // Act && Assert
            Assert.Throws<ArgumentException>(() => ParseService.ParseCommandLineArgs(args), "Отсутствует путь к файлу вывода после параметра --file-output");
        }

        [Test]
        public void ParseCommandLineArgs_GivenArgsWithEmptyStartAddress_ReturnsFormatException()
        {
            // Arrange
            string[] args =
            {
                "--file-log", "log.txt",
                "--file-output", "output.txt",
                "--address-start",
                "--address-mask", "255.255.255.0",
                "--time-start", "01.04.2024",
                "--time-end", "03.04.2024"
            };

            // Act && Assert
            Assert.Throws<FormatException>(() => ParseService.ParseCommandLineArgs(args));
        }

        [Test]
        public void ParseCommandLineArgs_GivenArgsWithEmptyAddressMask_ReturnsFormatException()
        {
            // Arrange
            string[] args =
            {
                "--file-log", "log.txt",
                "--file-output", "output.txt",
                "--address-start", "192.168.1.1",
                "--address-mask",
                "--time-start", "01.04.2024",
                "--time-end", "03.04.2024"
            };

            // Act && Assert
            Assert.Throws<FormatException>(() => ParseService.ParseCommandLineArgs(args));
        }

        [Test]
        public void ParseCommandLineArgs_GivenArgsWithEmptyStartTime_ReturnsFormatException()
        {
            // Arrange
            string[] args =
            {
                "--file-log", "log.txt",
                "--file-output", "output.txt",
                "--address-start", "192.168.1.1",
                "--address-mask", "255.255.255.0",
                "--time-start",
                "--time-end", "03.04.2024"
            };

            // Act && Assert
            Assert.Throws<FormatException>(() => ParseService.ParseCommandLineArgs(args));
        }

        [Test]
        public void ParseCommandLineArgs_GivenArgsWithEmptyEndTime_ReturnsArgumentException()
        {
            // Arrange
            string[] args =
            {
                "--file-log", "log.txt",
                "--file-output", "output.txt",
                "--address-start", "192.168.1.1",
                "--address-mask", "255.255.255.0",
                "--time-start", "01.04.2024",
                "--time-end", 
            };

            // Act && Assert
            Assert.Throws<ArgumentException>(() => ParseService.ParseCommandLineArgs(args), "Отсутствует время конца интервала после параметра --time-end");
        }

        [Test]
        public void ParseLogParametersInConfig_GivenEmptyConfig_ReturnsArgumentException()
        {
            // Arrange
            var mockConfig = new Mock<IConfigurationRoot>();

            // Act && Assert
            Assert.Throws<ArgumentException>(() => ParseService.ParseLogParametersInConfig(new LogAnalysisParameters(), mockConfig.Object));
        }

        [Test]
        public void ParseLogParametersInConfig_GivenValidConfig_ReturnsParameters()
        {
            // Arrange
            var mockConfig = new Mock<IConfigurationRoot>();
            mockConfig.Setup(x => x[CommandNames.FileLog]).Returns("log.txt");
            mockConfig.Setup(x => x[CommandNames.FileOutput]).Returns("output.txt");
            mockConfig.Setup(x => x[CommandNames.StartAddress]).Returns("192.168.1.1");
            mockConfig.Setup(x => x[CommandNames.AddressMask]).Returns("255.255.255.0");
            mockConfig.Setup(x => x[CommandNames.StartTime]).Returns("03.04.2024");
            mockConfig.Setup(x => x[CommandNames.EndTime]).Returns("04.04.2024");

            var logAnalysisParameters = new LogAnalysisParameters();

            // Act
            var actual = ParseService.ParseLogParametersInConfig(logAnalysisParameters, mockConfig.Object);

            // Assert
            Assert.That(actual.LogFilePath, Is.EqualTo("log.txt"));
            Assert.That(actual.OutputFilePath, Is.EqualTo("output.txt"));
            Assert.That(actual.StartAddress.ToString(), Is.EqualTo("192.168.1.1"));
            Assert.That(actual.Mask.ToString(), Is.EqualTo("255.255.255.0"));
            Assert.That(actual.StartTime, Is.EqualTo(new DateTime(2024, 4, 3)));
            Assert.That(actual.EndTime, Is.EqualTo(new DateTime(2024, 4, 4)));
        }

        [Test]
        public void ParseLogParametersInConfig_GivenConfigWithoutLogFilePath_ReturnsArgumentException()
        {
            // Arrange
            var mockConfig = new Mock<IConfigurationRoot>();
            mockConfig.Setup(x => x[CommandNames.FileOutput]).Returns("output.txt");
            mockConfig.Setup(x => x[CommandNames.StartAddress]).Returns("192.168.1.1");
            mockConfig.Setup(x => x[CommandNames.AddressMask]).Returns("255.255.255.0");
            mockConfig.Setup(x => x[CommandNames.StartTime]).Returns("03.04.2024");
            mockConfig.Setup(x => x[CommandNames.EndTime]).Returns("04.04.2024");

            // Act && Assert
            Assert.Throws<ArgumentException>(() => ParseService.ParseLogParametersInConfig(new LogAnalysisParameters(), mockConfig.Object));
        }

        [Test]
        public void ParseLogParametersInConfig_GivenConfigWithoutFileOutput_ReturnsArgumentException()
        {
            // Arrange
            var mockConfig = new Mock<IConfigurationRoot>();
            mockConfig.Setup(x => x[CommandNames.FileLog]).Returns("log.txt");
            mockConfig.Setup(x => x[CommandNames.StartAddress]).Returns("192.168.1.1");
            mockConfig.Setup(x => x[CommandNames.AddressMask]).Returns("255.255.255.0");
            mockConfig.Setup(x => x[CommandNames.StartTime]).Returns("03.04.2024");
            mockConfig.Setup(x => x[CommandNames.EndTime]).Returns("04.04.2024");

            // Act && Assert
            Assert.Throws<ArgumentException>(() => ParseService.ParseLogParametersInConfig(new LogAnalysisParameters(), mockConfig.Object));
        }

        [Test]
        public void ParseLogParametersInConfig_GivenConfigWithoutStartAddress_ReturnsArgumentException()
        {
            // Arrange
            var mockConfig = new Mock<IConfigurationRoot>();
            mockConfig.Setup(x => x[CommandNames.FileLog]).Returns("log.txt");
            mockConfig.Setup(x => x[CommandNames.FileOutput]).Returns("output.txt");
            mockConfig.Setup(x => x[CommandNames.AddressMask]).Returns("255.255.255.0");
            mockConfig.Setup(x => x[CommandNames.StartTime]).Returns("03.04.2024");
            mockConfig.Setup(x => x[CommandNames.EndTime]).Returns("04.04.2024");

            // Act && Assert
            Assert.Throws<ArgumentException>(() => ParseService.ParseLogParametersInConfig(new LogAnalysisParameters(), mockConfig.Object));
        }

        [Test]
        public void ParseLogParametersInConfig_GivenConfigWhenStartTimeIsGreaterThanEndTime_ReturnsArgumentException()
        {
            // Arrange
            var mockConfig = new Mock<IConfigurationRoot>();
            mockConfig.Setup(x => x[CommandNames.FileLog]).Returns("log.txt");
            mockConfig.Setup(x => x[CommandNames.FileOutput]).Returns("output.txt");
            mockConfig.Setup(x => x[CommandNames.StartAddress]).Returns("192.168.1.1");
            mockConfig.Setup(x => x[CommandNames.AddressMask]).Returns("255.255.255.0");
            mockConfig.Setup(x => x[CommandNames.StartTime]).Returns("05.04.2024");
            mockConfig.Setup(x => x[CommandNames.EndTime]).Returns("04.04.2024");

            // Act && Assert
            Assert.Throws<ArgumentException>(() => ParseService.ParseLogParametersInConfig(new LogAnalysisParameters(), mockConfig.Object));
        }

        [Test]
        public void ParseLogParametersInConfig_GivenConfigWithoutStartTime_ReturnsParameters()
        {
            // Arrange
            var mockConfig = new Mock<IConfigurationRoot>();
            mockConfig.Setup(x => x[CommandNames.FileLog]).Returns("log.txt");
            mockConfig.Setup(x => x[CommandNames.FileOutput]).Returns("output.txt");
            mockConfig.Setup(x => x[CommandNames.StartAddress]).Returns("192.168.1.1");
            mockConfig.Setup(x => x[CommandNames.AddressMask]).Returns("255.255.255.0");
            mockConfig.Setup(x => x[CommandNames.EndTime]).Returns("04.04.2024");

            // Act
            var actual = ParseService.ParseLogParametersInConfig(new LogAnalysisParameters(), mockConfig.Object);

            // Assert
            Assert.That(actual.LogFilePath, Is.EqualTo("log.txt"));
            Assert.That(actual.OutputFilePath, Is.EqualTo("output.txt"));
            Assert.That(actual.StartAddress.ToString(), Is.EqualTo("192.168.1.1"));
            Assert.That(actual.Mask.ToString(), Is.EqualTo("255.255.255.0"));
            Assert.That(actual.StartTime, Is.EqualTo(DateTime.MinValue));
            Assert.That(actual.EndTime, Is.EqualTo(new DateTime(2024, 4, 4)));
        }

        [Test]
        public void ParseLogParametersInConfig_GivenConfigWithoutEndTime_ReturnsParameters()
        {
            // Arrange
            var mockConfig = new Mock<IConfigurationRoot>();
            mockConfig.Setup(x => x[CommandNames.FileLog]).Returns("log.txt");
            mockConfig.Setup(x => x[CommandNames.FileOutput]).Returns("output.txt");
            mockConfig.Setup(x => x[CommandNames.StartAddress]).Returns("192.168.1.1");
            mockConfig.Setup(x => x[CommandNames.AddressMask]).Returns("255.255.255.0");
            mockConfig.Setup(x => x[CommandNames.StartTime]).Returns("03.04.2024");

            // Act
            var actual = ParseService.ParseLogParametersInConfig(new LogAnalysisParameters(), mockConfig.Object);

            // Assert
            Assert.That(actual.LogFilePath, Is.EqualTo("log.txt"));
            Assert.That(actual.OutputFilePath, Is.EqualTo("output.txt"));
            Assert.That(actual.StartAddress.ToString(), Is.EqualTo("192.168.1.1"));
            Assert.That(actual.Mask.ToString(), Is.EqualTo("255.255.255.0"));
            Assert.That(actual.StartTime, Is.EqualTo(new DateTime(2024, 4, 3)));
            Assert.That(actual.EndTime, Is.EqualTo(DateTime.MaxValue));
        }

        [Test]
        public void ParseLogParametersInConfig_GivenConfigWithInvalidStartAddress_ReturnsArgumentException()
        {
            // Arrange
            var mockConfig = new Mock<IConfigurationRoot>();
            mockConfig.Setup(x => x[CommandNames.FileLog]).Returns("log.txt");
            mockConfig.Setup(x => x[CommandNames.FileOutput]).Returns("output.txt");
            mockConfig.Setup(x => x[CommandNames.StartAddress]).Returns("192.168..1");
            mockConfig.Setup(x => x[CommandNames.AddressMask]).Returns("255.255.255.0");
            mockConfig.Setup(x => x[CommandNames.StartTime]).Returns("03.04.2024");
            mockConfig.Setup(x => x[CommandNames.EndTime]).Returns("04.04.2024");

            // Act && Assert
            Assert.Throws<FormatException>(() => ParseService.ParseLogParametersInConfig(new LogAnalysisParameters(), mockConfig.Object));
        }

        [Test]
        public void ParseLogParametersInConfig_GivenConfigWithInvalidAddressMask_ReturnsArgumentException()
        {
            // Arrange
            var mockConfig = new Mock<IConfigurationRoot>();
            mockConfig.Setup(x => x[CommandNames.FileLog]).Returns("log.txt");
            mockConfig.Setup(x => x[CommandNames.FileOutput]).Returns("output.txt");
            mockConfig.Setup(x => x[CommandNames.StartAddress]).Returns("192.168.1.1");
            mockConfig.Setup(x => x[CommandNames.AddressMask]).Returns("255.255..0");
            mockConfig.Setup(x => x[CommandNames.StartTime]).Returns("03.04.2024");
            mockConfig.Setup(x => x[CommandNames.EndTime]).Returns("04.04.2024");

            // Act && Assert
            Assert.Throws<FormatException>(() => ParseService.ParseLogParametersInConfig(new LogAnalysisParameters(), mockConfig.Object));
        }

        [Test]
        public void ParseLogParametersInConfig_GivenConfigWithInvalidStartTime_ReturnsArgumentException()
        {
            // Arrange
            var mockConfig = new Mock<IConfigurationRoot>();
            mockConfig.Setup(x => x[CommandNames.FileLog]).Returns("log.txt");
            mockConfig.Setup(x => x[CommandNames.FileOutput]).Returns("output.txt");
            mockConfig.Setup(x => x[CommandNames.StartAddress]).Returns("192.168.1.1");
            mockConfig.Setup(x => x[CommandNames.AddressMask]).Returns("255.255..0");
            mockConfig.Setup(x => x[CommandNames.StartTime]).Returns("2024-04-01");
            mockConfig.Setup(x => x[CommandNames.EndTime]).Returns("04.04.2024");

            // Act && Assert
            Assert.Throws<FormatException>(() => ParseService.ParseLogParametersInConfig(new LogAnalysisParameters(), mockConfig.Object));
        }

        [Test]
        public void ParseLogParametersInConfig_GivenConfigWithInvalidEndTime_ReturnsArgumentException()
        {
            // Arrange
            var mockConfig = new Mock<IConfigurationRoot>();
            mockConfig.Setup(x => x[CommandNames.FileLog]).Returns("log.txt");
            mockConfig.Setup(x => x[CommandNames.FileOutput]).Returns("output.txt");
            mockConfig.Setup(x => x[CommandNames.StartAddress]).Returns("192.168.1.1");
            mockConfig.Setup(x => x[CommandNames.AddressMask]).Returns("255.255..0");
            mockConfig.Setup(x => x[CommandNames.StartTime]).Returns("03.04.2024");
            mockConfig.Setup(x => x[CommandNames.EndTime]).Returns("2024-04-02");

            // Act && Assert
            Assert.Throws<FormatException>(() => ParseService.ParseLogParametersInConfig(new LogAnalysisParameters(), mockConfig.Object));
        }
    }
}
