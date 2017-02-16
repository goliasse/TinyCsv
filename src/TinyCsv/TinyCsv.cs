//===============================================================================
// TinyCsv
//
// https://github.com/grumpydev/TinyCsv
//===============================================================================
// Copyright © 2017 Steven Robbins  All rights reserved.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
//===============================================================================

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace TinyCsv
{
    public interface ILineReader
    {
        string GetNextLine();
    }

    public class StringLineReader : ILineReader
    {
        private readonly string[] _contentLines;
        private int _currentIndex = 0;

        public StringLineReader(string contents)
        {
            _contentLines = Regex.Split(contents, "\r\n|\r|\n");
        }

        public string GetNextLine()
        {
            if (this._currentIndex >= this._contentLines.Length)
            {
                return null;
            }

            return this._contentLines[this._currentIndex++];
        }
    }

    public class FileProcessor
    {
        private readonly ILineReader _reader;
        private readonly ILineProcessor _processor;

        public FileProcessor(ILineReader reader, ILineProcessor processor)
        {
            _reader = reader;
            _processor = processor;
        }

        public IEnumerable<IEnumerable<string>> Process()
        {
            string current = null;
            do
            {
                current = _reader.GetNextLine();

                if (current != null)
                {
                    yield return this._processor.Process(current);
                }

            } while (current != null);
        }
    }

    public interface ILineProcessor
    {
        IEnumerable<string> Process(string line);
    }

    public class LineProcessor : ILineProcessor
    {
        private static readonly char[] DefaultQuoteCharacters = {'\'', '"'};

        private readonly char _separator;

        private readonly char[] _quoteCharacters;

        public LineProcessor(char separator, char[] quoteCharacters = null)
        {
            this._separator = separator;
            this._quoteCharacters = quoteCharacters ?? DefaultQuoteCharacters;
        }

        public IEnumerable<string> Process(string line)
        {
            var lineCharacters = line.ToCharArray();
            var currentCharacterIndex = 0;
            var currentValueBuilder = new StringBuilder();
            char? currentQuoteCharacter = null;

            while (currentCharacterIndex < lineCharacters.Length)
            {
                var currentCharacter = lineCharacters[currentCharacterIndex];

                if (this.IsSeparator(currentCharacter) && !currentQuoteCharacter.HasValue)
                {
                    yield return currentValueBuilder.ToString();
                    
                    currentValueBuilder.Clear();

                    currentCharacterIndex++;
                    continue;
                }

                if (this.IsQuote(currentCharacter) && currentValueBuilder.Length == 0)
                {
                    currentQuoteCharacter = currentCharacter;

                    currentCharacterIndex++;
                    continue;
                }

                if (currentQuoteCharacter.HasValue && currentCharacter == currentQuoteCharacter.Value)
                {
                    var next = this.PeekCharacter(lineCharacters, currentCharacterIndex);

                    if (!next.HasValue || IsSeparator(next.Value))
                    {
                        currentQuoteCharacter = null;
                        currentCharacterIndex++;

                        continue;
                    }

                    if (next.Value == currentQuoteCharacter.Value)
                    {
                        currentValueBuilder.Append(currentCharacter);
                        currentCharacterIndex = currentCharacterIndex + 2;

                        continue;
                    }

                    currentValueBuilder.Insert(0, currentQuoteCharacter.Value);
                    currentValueBuilder.Append(currentCharacter);
                    currentQuoteCharacter = null;
                    currentCharacterIndex++;
                    continue;
                }

                currentValueBuilder.Append(currentCharacter);
                currentCharacterIndex++;
            }

            yield return currentValueBuilder.ToString();
        }

        private char? PeekCharacter(char[] lineCharacters, int currentCharacterIndex)
        {
            if (currentCharacterIndex + 1 >= lineCharacters.Length)
            {
                return null;
            }

            return lineCharacters[currentCharacterIndex + 1];
        }

        private bool IsQuote(char currentCharacter)
        {
            return this._quoteCharacters.Contains(currentCharacter);
        }

        private bool IsSeparator(char currentCharacter)
        {
            return currentCharacter == _separator;
        }
    }

    public static class Process
    {
        public static IEnumerable<IEnumerable<string>> FromCommaSeparatedString(string input, char[] quoteCharacters = null)
        {
            var reader = new StringLineReader(input);
            var processor = new LineProcessor(',', quoteCharacters);

            return Execute(reader, processor);
        }        
        
        public static IEnumerable<IEnumerable<string>> FromTabSeparatedString(string input, char[] quoteCharacters = null)
        {
            var reader = new StringLineReader(input);
            var processor = new LineProcessor('\t', quoteCharacters);

            return Execute(reader, processor);
        }

        private static IEnumerable<IEnumerable<string>> Execute(ILineReader reader, ILineProcessor processor)
        {
            var fileProcessor = new FileProcessor(reader, processor);

            return fileProcessor.Process();
        }
    }
}