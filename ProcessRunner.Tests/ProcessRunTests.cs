using Ploeh.AutoFixture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Extensions;

namespace TwoPS.Processes.Tests
{
    public class ProcessRunTests
    {
        [Fact]
        public void EchoEmpty()
        {
            // Arrange
            var process = CreateEchoProcess();

            var expected = new List<string> { };

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
