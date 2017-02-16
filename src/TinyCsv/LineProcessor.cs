using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TinyCsv
{
    public class LineProcessor
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
}