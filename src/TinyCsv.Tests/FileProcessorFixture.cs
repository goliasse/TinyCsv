using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Should;
using TinyCsv.Tests.Fakes;
using Xunit;

namespace TinyCsv.Tests
{
    public class FileProcessorFixture
    {
        [Fact]
        public async Task Should_pass_each_line_to_processor()
        {
            // Given
            var lines = new []{ "One", "Two", "Three" };
            var reader = new FakeLineReader(lines);
            var processor = new FakeLineProcessor();
            var fileProcessor = new FileProcessor(reader, processor);

            // When
            var result = (await fileProcessor.Process()).ToArray();

            // Then
            processor.PassedLines.Count.ShouldEqual(3);
            processor.PassedLines[0].ShouldEqual("One");
            processor.PassedLines[1].ShouldEqual("Two");
            processor.PassedLines[2].ShouldEqual("Three");
        }

        [Fact]
        public async Task Should_dispose_of_reader_when_finished()
        {
            // Given
            var lines = new []{ "One", "Two", "Three" };
            var reader = new FakeLineReader(lines);
            var processor = new FakeLineProcessor();
            var fileProcessor = new FileProcessor(reader, processor);

            // When
            var result = (await fileProcessor.Process()).ToArray();

            // Then
            reader.Disposed.ShouldBeTrue();
        }

        [Fact]
        public async Task Should_skip_lines_if_set()
        {
            // Given
            var lines = new []{ "Nope", "Nope2", "One", "Two", "Three" };
            var reader = new FakeLineReader(lines);
            var processor = new FakeLineProcessor();
            var fileProcessor = new FileProcessor(reader, processor) { Skip = 2 };

            // When
            var result = (await fileProcessor.Process()).ToArray();

            // Then
            processor.PassedLines.Count.ShouldEqual(3);
            processor.PassedLines[0].ShouldEqual("One");
            processor.PassedLines[1].ShouldEqual("Two");
            processor.PassedLines[2].ShouldEqual("Three");
        }
    }
}