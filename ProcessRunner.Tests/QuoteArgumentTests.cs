using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Extensions;

namespace TwoPS.Processes.Tests
{
    public class QuoteArgumentTests
    {
        [Theory]
        [InlineData(null, null)]
        [InlineData(@"", @"")]
        [InlineData(@"alpha", @"alpha")]
        [InlineData(@"alpha beta", @"""alpha beta""")]
        [InlineData(@"alpha ""beta""", @"""alpha \""beta\""""")]
        [InlineData(@"alpha \""beta""", @"""alpha \\\""beta\""""")]
        [InlineData(@"alpha \\""beta""", @"""alpha \\\\\""beta\""""")]
        [InlineData(@"alpha beta\", @"""alpha beta\\""")]
        [InlineData(@"alpha beta\\", @"""alpha beta\\\\""")]
        public void QuoteArgument(string argument, string expected)
        {
            // Arrange

            // Act
            var actual = ProcessOptions.QuoteArgument(argument);

            // Assert
            Assert.Equal(expected, actual);
        }
    }
}
