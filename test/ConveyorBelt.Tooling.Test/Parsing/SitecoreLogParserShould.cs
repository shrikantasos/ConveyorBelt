using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConveyorBelt.Tooling.Parsing;
using Xunit;

namespace ConveyorBelt.Tooling.Test.Parsing
{
    
    public class SitecoreLogParserShould
    {
        [Fact]
        public void ParseSingleInfoLevel()
        {
            using (var stream = new MemoryStream(File.ReadAllBytes(@"data\SitecoreLog1.txt")))
            {
                var sitecoreLogParser = new SitecoreLogParser();
                var uri = new Uri("http://localhost/data/SitecoreLog1.log.20160613.172129.txt");

                var result = sitecoreLogParser.Parse(() => stream, uri, new DiagnosticsSourceSummary());
                Assert.NotNull(result);
                var parsedLog = result.FirstOrDefault();
                Assert.NotNull(parsedLog);
                Assert.Equal("ManagedPoolThread #0", parsedLog[SitecoreLogParser.SitecoreLogFields.ProcessId]);
                Assert.Equal("INFO", parsedLog[SitecoreLogParser.SitecoreLogFields.Level]);
                Assert.Equal("2016-06-13T17:12:32", parsedLog["@timestamp"]);
                Assert.Equal("Trying to load XML configuration /App_Config/Security/GlobalRoles.config", parsedLog[SitecoreLogParser.SitecoreLogFields.Message]);
            }
        }

        [Fact]
        public void ParseSingleDebugLevel()
        {
            using (var stream = new MemoryStream(File.ReadAllBytes(@"data\SitecoreLog2.txt")))
            {
                var sitecoreLogParser = new SitecoreLogParser();
                var uri = new Uri("http://localhost/data/baselogfile.20160613.180755.txt");

                var result = sitecoreLogParser.Parse(() => stream, uri, new DiagnosticsSourceSummary());
                Assert.NotNull(result);
                var parsedLog = result.FirstOrDefault();
                Assert.NotNull(parsedLog);
                Assert.Equal("DEBUG", parsedLog[SitecoreLogParser.SitecoreLogFields.Level]);
            }
        }

        [Fact]
        public void ParseCompleteErrorLevel()
        {
            using (var stream = new MemoryStream(File.ReadAllBytes(@"data\SitecoreLog3.txt")))
            {
                var sitecoreLogParser = new SitecoreLogParser();
                var uri = new Uri("http://localhost/data/sitecoredev228CA/xyz/baselogfile.20160101.180755.txt");

                var result = sitecoreLogParser.Parse(() => stream, uri, new DiagnosticsSourceSummary()).ToList();
                Assert.NotNull(result);
                var parsedLog = result.FirstOrDefault();
                Assert.NotNull(parsedLog);
                Assert.Equal("ERROR", parsedLog[SitecoreLogParser.SitecoreLogFields.Level]);
                Assert.StartsWith("Test Error with exception\r\n", parsedLog[SitecoreLogParser.SitecoreLogFields.Message]);
                Assert.EndsWith("Parameter name: database", parsedLog[SitecoreLogParser.SitecoreLogFields.Message]);
                Assert.Equal(2, result.Count);
            }
        }

        [Fact]
        public void ParseExceptionMessage()
        {
            using (var stream = new MemoryStream(File.ReadAllBytes(@"data\SitecoreLog5.txt")))
            {
                var sitecoreLogParser = new SitecoreLogParser();
                var uri = new Uri("http://localhost/data/SitecoreLog1.log.20160606.172133.txt");

                var result = sitecoreLogParser.Parse(() => stream, uri, new DiagnosticsSourceSummary()).ToList();
                Assert.NotNull(result);
                Assert.Equal(2, result.Count);

                var parsedLog = result.First();
                Assert.NotNull(parsedLog);
                Assert.Equal("ERROR", parsedLog[SitecoreLogParser.SitecoreLogFields.Level]);
                Assert.StartsWith("Test Message1:\r\n", parsedLog[SitecoreLogParser.SitecoreLogFields.Message]);
                Assert.Contains("The password failed.  Password=**PASSWORD**REDACTED**\r\n", parsedLog[SitecoreLogParser.SitecoreLogFields.Message]);
                Assert.DoesNotContain("TESTPassword", parsedLog[SitecoreLogParser.SitecoreLogFields.Message]);


                parsedLog = result.Last();
                Assert.NotNull(parsedLog);
                Assert.Equal("ERROR", parsedLog[SitecoreLogParser.SitecoreLogFields.Level]);
                Assert.StartsWith("SINGLE MSG: Sitecore heartbeat:\r\n", parsedLog[SitecoreLogParser.SitecoreLogFields.Message]);
                Assert.Contains(";Password=**PASSWORD**REDACTED**;", parsedLog[SitecoreLogParser.SitecoreLogFields.Message]);
                Assert.Contains("User ID=**USER**REDACTED**;", parsedLog[SitecoreLogParser.SitecoreLogFields.Message]);
                Assert.DoesNotContain("Not!actuallyApa$$word", parsedLog[SitecoreLogParser.SitecoreLogFields.Message]);

                foreach (var log in result)
                {
                    Assert.NotNull(log);
                    Assert.NotEqual(log[SitecoreLogParser.SitecoreLogFields.Message], string.Empty);
                    Assert.DoesNotContain("****", log[SitecoreLogParser.SitecoreLogFields.Message]);
                }
            }
        }

        [Fact]
        public void RemoveEmptyLogEntries()
        {
            using (var stream = new MemoryStream(File.ReadAllBytes(@"data\SitecoreLog4.txt")))
            {
                var sitecoreLogParser = new SitecoreLogParser();
                var uri = new Uri("http://localhost/data/SitecoreLog1.log.20160606.172133.txt");

                var result = sitecoreLogParser.Parse(() => stream, uri, new DiagnosticsSourceSummary()).ToList();
                Assert.NotNull(result);
                Assert.Single(result);
                var parsedLog = result.Last();
                Assert.NotNull(parsedLog);
                Assert.Equal("INFO", parsedLog[SitecoreLogParser.SitecoreLogFields.Level]);
                Assert.Equal("Sitecore started", parsedLog[SitecoreLogParser.SitecoreLogFields.Message]);
            }
        }
    }
}
