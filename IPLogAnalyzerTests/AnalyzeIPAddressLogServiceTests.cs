using IPLogAnalyzer.Services;
using System.Net;

namespace IPLogAnalyzerTests
{
    [TestFixture]
    internal class AnalyzeIPAddressLogServiceTests
    {
        [Test]
        public void FilterLogEntries_GivenValidInputs_ReturnsFilteredEntries()
        {
            // Arrange
            IPAddress startAddress = IPAddress.Parse("192.168.1.0");
            IPAddress mask = IPAddress.Parse("255.255.255.0");
            DateTime startTime = new DateTime(2021, 01, 01, 00, 00, 00);
            DateTime endTime = new DateTime(2021, 01, 02, 23, 59, 59);

            List<(IPAddress Address, DateTime Timestamp)> logEntries = new List<(IPAddress Address, DateTime Timestamp)>
                {
                    (IPAddress.Parse("192.168.1.10"), new DateTime(2021, 01, 01, 12, 00, 00)),
                    (IPAddress.Parse("192.168.1.20"), new DateTime(2021, 01, 01, 13, 00, 00)),
                    (IPAddress.Parse("192.168.1.30"), new DateTime(2021, 01, 03, 14, 00, 00)),
                    (IPAddress.Parse("192.168.2.10"), new DateTime(2021, 01, 01, 12, 00, 00)),
                };

            List<(IPAddress Address, DateTime Timestamp)> expectedFilteredEntries = new List<(IPAddress Address, DateTime Timestamp)>
                {
                    (IPAddress.Parse("192.168.1.10"), new DateTime(2021, 01, 01, 12, 00, 00)),
                    (IPAddress.Parse("192.168.1.20"), new DateTime(2021, 01, 01, 13, 00, 00)),
                };

            // Act
            var actualFilteredEntries = AnalyzeIPAddressLogService.FilterLogEntries(logEntries, startAddress, mask, startTime, endTime);

            // Assert
            Assert.That(actualFilteredEntries, Is.EqualTo(expectedFilteredEntries));
        }

        [Test]
        public void FilterLogEntries_GivenEmptyLogEntries_ReturnsEmptyList()
        {
            // Arrange
            IPAddress startAddress = IPAddress.Parse("192.168.1.0");
            IPAddress mask = IPAddress.Parse("255.255.255.0");
            DateTime startTime = new DateTime(2021, 01, 01, 00, 00, 00);
            DateTime endTime = new DateTime(2021, 01, 01, 23, 59, 59);

            List<(IPAddress Address, DateTime Timestamp)> logEntries = new List<(IPAddress Address, DateTime Timestamp)>();

            // Act
            var actualFilteredEntries = AnalyzeIPAddressLogService.FilterLogEntries(logEntries, startAddress, mask, startTime, endTime);

            // Assert
            Assert.That(actualFilteredEntries, Is.Empty);
        }

        [Test]
        public void CountIPAddresses_GivenValidLogEntries_ReturnsCorrectCounts()
        {
            // Arrange
            var logEntries = new List<(IPAddress Address, DateTime Timestamp)>
                {
                    (IPAddress.Parse("192.168.1.10"), DateTime.Now),
                    (IPAddress.Parse("192.168.1.10"), DateTime.Now.AddHours(-1)),
                    (IPAddress.Parse("192.168.1.20"), DateTime.Now),
                    (IPAddress.Parse("192.168.1.20"), DateTime.Now.AddHours(-2)),
                    (IPAddress.Parse("192.168.1.20"), DateTime.Now.AddHours(-3)),
                    (IPAddress.Parse("192.168.1.30"), DateTime.Now),
                };

            var expected = new Dictionary<IPAddress, int>
                {
                    { IPAddress.Parse("192.168.1.10"), 2 },
                    { IPAddress.Parse("192.168.1.20"), 3 },
                    { IPAddress.Parse("192.168.1.30"), 1 }
                };

            // Act
            var actual = AnalyzeIPAddressLogService.CountIPAddresses(logEntries);

            // Assert
            Assert.That(actual.Count, Is.EqualTo(expected.Count));
            foreach (var expectedKey in expected.Keys)
            {
                Assert.That(actual.ContainsKey(expectedKey));
                Assert.That(expected[expectedKey], Is.EqualTo(actual[expectedKey]));
            }
        }

        [Test]
        public void CountIPAddresses_GivenEmptyLogEntries_ReturnsEmptyDictionary()
        {
            // Arrange
            var logEntries = new List<(IPAddress Address, DateTime Timestamp)>();

            // Act
            var actual = AnalyzeIPAddressLogService.CountIPAddresses(logEntries);

            // Assert
            Assert.That(actual, Is.Empty);
        }
    }
}
