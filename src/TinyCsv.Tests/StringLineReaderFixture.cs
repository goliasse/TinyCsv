﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Should;
using Xunit;

namespace TinyCsv.Tests
{
    public class StringLineReaderFixture
    {
        [Fact]
        public async Task Should_return_all_lines_then_null()
        {
            // Given
            var input = "this\nthat\nthe other";
            var reader = new StringLineReader(input);
            var output = new List<string>();

            // When
            string current = null;
            do
            {
                current = await reader.GetNextLine();

                output.Add(current);
            } while (current != null);

            // Then
            output[0].ShouldEqual("this");
            output[1].ShouldEqual("that");
            output[2].ShouldEqual("the other");
            output[3].ShouldEqual(null);
        }
    }
}