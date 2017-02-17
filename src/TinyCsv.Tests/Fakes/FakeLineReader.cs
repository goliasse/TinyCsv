using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        public int? NumberOfLines
        {
            get { return this._lines.Length; }
        }

        public Task<string> GetNextLine()
        {
            return Task.FromResult(_currentIndex == _lines.Length ? null : _lines[_currentIndex++]);
        }
    }
}