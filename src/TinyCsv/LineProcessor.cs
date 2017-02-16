using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace TinyCsv
{
    public class LineProcessor
    {
        private readonly char _separator;

        public LineProcessor(char separator)
        {
            _separator = separator;
        }

        public static LineProcessor Csv { get {  return new LineProcessor(','); } }

        public IEnumerable<string> Process(string line)
        {
            var lineCharacters = line.ToCharArray();
            var currentCharacterIndex = 0;
            var currentValueBuilder = new StringBuilder();
            char? currentQuoteCharacter = null;

            while (currentCharacterIndex < lineCharacters.Length)
            {
                var currentCharacter = lineCharacters[currentCharacterIndex];

                if (this.IsSeparator(currentCharacter))
                {
                    yield return currentValueBuilder.ToString();
                    
                    currentCharacterIndex++;

                    currentValueBuilder.Clear();

                    continue;
                }

                currentValueBuilder.Append(currentCharacter);
                currentCharacterIndex++;
            }

            yield return currentValueBuilder.ToString();
        }

        private bool IsSeparator(char currentCharacter)
        {
            return currentCharacter == _separator;
        }
    }
}