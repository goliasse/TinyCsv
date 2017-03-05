using System;
using System.Linq;
using Should;
using Xunit;

namespace TinyCsv.Tests
{
    public class LineProcessorFixture
    {
        private readonly LineProcessor _processor;

        public LineProcessorFixture()
        {
            this._processor = new LineProcessor(',');
        }

        [Fact]
        public void Should_handle_basic_line()
        {
            // Given
            var line = "this,that,the other";

            // When
            var result = this._processor.Process(line);

            // Then
            result.SequenceEqual(new [] { "this", "that", "the other" }).ShouldBeTrue();
        }

        [Fact]
        public void Should_handle_empty_elements()
        {
            // Given
            var line = "this,,the other";

            // When
            var result = this._processor.Process(line);

            // Then
            result.SequenceEqual(new[] { "this", "", "the other" }).ShouldBeTrue();
        }

        [Fact]
        public void Should_handle_quotes()
        {
            // Given
            var line = "\"this\",\"that\",'the other'";

            // When
            var result = this._processor.Process(line);

            // Then
            result.SequenceEqual(new[] { "this", "that", "the other" }).ShouldBeTrue();
        }

        [Fact]
        public void Should_handle_commas_in_quotes()
        {
            // Given
            var line = "\"this\",\"that\",'the,other'";

            // When
            var result = this._processor.Process(line);

            // Then
            result.SequenceEqual(new[] { "this", "that", "the,other" }).ShouldBeTrue();
        }

        [Fact]
        public void Should_handle_different_quote_character_in_quotes()
        {
            // Given
            var line = "\"this\",\"that\",'the\"other'";

            // When
            var result = this._processor.Process(line);

            // Then
            result.SequenceEqual(new[] { "this", "that", "the\"other" }).ShouldBeTrue();
        }

        [Fact]
        public void Should_handle_escaped_quotes()
        {
            // Given
            var line = "\"this\",\"that\",'the''other'";

            // When
            var result = this._processor.Process(line);

            // Then
            result.SequenceEqual(new[] { "this", "that", "the'other" }).ShouldBeTrue();
        }

        [Fact]
        public void Should_only_assume_quotes_if_initial_character_is_a_quote()
        {
            // Given
            var line = "this,that,the \"other\"";

            // When
            var result = this._processor.Process(line);

            // Then
            result.SequenceEqual(new[] { "this", "that", "the \"other\"" }).ShouldBeTrue();
        }

        [Fact]
        public void Should_not_consider_value_quoted_if_closing_quote_not_at_end()
        {
            // Given
            var line = "'thi's,that,the other";

            // When
            var result = this._processor.Process(line);

            // Then
            result.SequenceEqual(new[] { "'thi's", "that", "the other" }).ShouldBeTrue();
        }

        [Fact]
        public void Should_use_null_for_blank_values_if_set()
        {
            // Given
            var line = "this,,the other";
            this._processor.ReplaceBlankValuesWith = null;

            // When
            var result = this._processor.Process(line);

            // Then
            result.SequenceEqual(new[] { "this", null, "the other" }).ShouldBeTrue();
        }

        [Fact]
        public void Should_not_use_null_for_empty_strings_even_if_set()
        {
            // Given
            var line = "this,'',the other";
            this._processor.ReplaceBlankValuesWith = null;

            // When
            var result = this._processor.Process(line);

            // Then
            result.SequenceEqual(new[] { "this", "", "the other" }).ShouldBeTrue();
        }

        [Fact]
        public void Should_use_empty_string_for_blank_values_if_set()
        {
            // Given
            var line = "this,,the other";
            this._processor.ReplaceBlankValuesWith = string.Empty;

            // When
            var result = this._processor.Process(line);

            // Then
            result.SequenceEqual(new[] { "this", "", "the other" }).ShouldBeTrue();
        }

        [Fact]
        public void Should_replace_null_with_actual_null_if_Set()
        {
            // Given
            var line = "this,null,the other";
            this._processor.ReplaceNullValueWithActualNull = true;

            // When
            var result = this._processor.Process(line);

            // Then
            result.SequenceEqual(new[] { "this", null, "the other" }).ShouldBeTrue();
        }

        [Fact]
        public void Should_not_replace_null_with_actual_null_if_not_set()
        {
            // Given
            var line = "this,null,the other";
            this._processor.ReplaceNullValueWithActualNull = false;

            // When
            var result = this._processor.Process(line);

            // Then
            result.SequenceEqual(new[] { "this", "null", "the other" }).ShouldBeTrue();
        }

        [Fact]
        public void Should_not_replace_null_with_actual_null_if_quoted()
        {
            // Given
            var line = "this,'null',the other";
            this._processor.ReplaceNullValueWithActualNull = true;

            // When
            var result = this._processor.Process(line);

            // Then
            result.SequenceEqual(new[] { "this", "null", "the other" }).ShouldBeTrue();
        }
    }
}