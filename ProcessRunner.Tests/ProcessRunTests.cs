using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoFixture;
using Xunit;

namespace TwoPS.Processes.Tests
{
    public class ProcessRunTests
    {
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(5)]
        public void EchoArgs(int count)
        {
            // Arrange
            var fixture = new Fixture();
            var args = count > 0 ? fixture.CreateMany<string>(count) : new string[] { };
            var process = CreateEchoProcess();
            foreach (var arg in args)
            {
                process.Options.Add(arg);
            }
            var expected = args.Select((a, i) => string.Format("args[{0}]: \"{1}\"", i, a));

            // Act
            var actual = process.Run();

            // Assert
            Assert.Equal(expected, actual.StandardOutputList);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(5)]
        public void EchoLines(int count)
        {
            // Arrange
            var fixture = new Fixture();
            var lines = count > 0 ? fixture.CreateMany<string>(count) : new string[] { };
            var process = CreateEchoProcess();
            process.Options.StandardInputAppendLines(lines);
            var expected = lines.Select(l => string.Format("Line: \"{0}\"", l));

            // Act
            var actual = process.Run();

            // Assert
            Assert.Equal(expected, actual.StandardOutputList);
        }

        [Theory]
        [InlineData(0, 0)]
        [InlineData(1, 0)]
        [InlineData(2, 0)]
        [InlineData(5, 0)]
        [InlineData(0, 1)]
        [InlineData(1, 1)]
        [InlineData(2, 1)]
        [InlineData(5, 1)]
        [InlineData(0, 2)]
        [InlineData(1, 2)]
        [InlineData(2, 2)]
        [InlineData(5, 2)]
        [InlineData(0, 5)]
        [InlineData(1, 5)]
        [InlineData(2, 5)]
        [InlineData(5, 5)]
        public void EchoArgsAndLines(int argCount, int lineCount)
        {
            // Arrange
            var fixture = new Fixture();
            var args = argCount > 0 ? fixture.CreateMany<string>(argCount) : new string[] { };
            var lines = lineCount > 0 ? fixture.CreateMany<string>(lineCount) : new string[] { };
            var process = CreateEchoProcess();
            foreach (var arg in args)
            {
                process.Options.Add(arg);
            }
            process.Options.StandardInputAppendLines(lines);
            var expected = args.Select((a, i) => string.Format("args[{0}]: \"{1}\"", i, a))
                .Concat(lines.Select(l => string.Format("Line: \"{0}\"", l)));

            // Act
            var actual = process.Run();

            // Assert
            Assert.Equal(expected, actual.StandardOutputList);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(5)]
        public void AsyncEchoLines(int count)
        {
            // Arrange
            var fixture = new Fixture();
            var lines = count > 0 ? fixture.CreateMany<string>(count) : new string[] { };
            var process = CreateEchoProcess();
            process.Options.StandardInputAppendLines(lines);
            var expected = lines.Select(l => string.Format("Line: \"{0}\"", l));
            var actual = new List<string>();
            process.StandardOutputRead += (s, e) => actual.Add(e.Line);

            // Act
            var result = Task.Run(() => process.Run()).Result;

            // Assert
            Assert.Equal(ProcessStatus.Finished, result.Status);
            Assert.True(result.Success, "Process did not succeed");
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void AsyncEchoCancelled()
        {
            // Arrange
            var fixture = new Fixture();
            var process = CreateEchoProcess();
            process.Options.AutoCloseStandardInput = false;
            //process.Options.Add("wait");
            //process.Options.Add(5000);

            // Act
            var task = Task.Run(() => process.Run());
            process.Cancel();
            var actual = task.Result;

            // Assert
            Assert.Equal(ProcessStatus.Cancelled, actual.Status);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(5)]
        public void EchoWriteStandardInput(int count)
        {
            // Arrange
            var fixture = new Fixture();
            var lines = count > 0 ? fixture.CreateMany<string>(count) : new string[] { };
            var process = CreateEchoProcess();
            process.Options.AutoCloseStandardInput = false;
            var expected = lines.Select(l => string.Format("Line: \"{0}\"", l));
            var actual = new List<string>();
            process.StandardOutputRead += (s, e) => actual.Add(e.Line);

            // Act
            var task = Task.Run(() => process.Run());
            foreach (var line in lines)
            {
                process.StandardInput.WriteLine(line);
            }
            process.StandardInput.Close();
            var result = task.Result;

            // Assert
            Assert.Equal(ProcessStatus.Finished, result.Status);
            Assert.True(result.Success, "Process did not succeed");
            Assert.Equal(expected, actual);
        }

        private static Process CreateEchoProcess()
        {
            var process = new Process("Echo.exe");
            process.Options.StandardInputEncoding = Encoding.UTF8;
            process.Options.StandardOutputEncoding = Encoding.UTF8;
            process.Options.StandardErrorEncoding = Encoding.UTF8;
            return process;
        }
    }
}
