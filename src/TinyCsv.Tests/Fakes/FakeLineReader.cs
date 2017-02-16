using System.Collections.Generic;
using System.Linq;

namespace TinyCsv.Tests.Fakes
{
    public class FakeLineReader : ILineReader
    {
        private int _currentIndex = 0;
        private readonly string[] _lines;

        public FakeLineReader(IEnumerable<string> lines)
        {
            this._lines = lines.ToArray();
        }

        public string GetNextLine()
        {
            return _currentIndex == _lines.Length ? null : _lines[_currentIndex++];
        }
    }
}