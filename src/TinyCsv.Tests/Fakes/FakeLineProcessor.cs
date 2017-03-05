using System.Collections.Generic;

namespace TinyCsv.Tests.Fakes
{
    public class FakeLineProcessor : ILineProcessor
    {
        public List<string> PassedLines { get; set; }

        public FakeLineProcessor()
        {
            this.PassedLines = new List<string>();
        }

        public IEnumerable<string> Process(string line)
        {
            this.PassedLines.Add(line);

            return line.Split(',');
        }

        public string ReplaceBlankValuesWith { get; set; }
        public bool ReplaceNullValueWithActualNull { get; set; }
    }
}