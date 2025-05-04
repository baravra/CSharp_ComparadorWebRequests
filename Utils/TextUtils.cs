using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComparadorWebRequests.Utils
{
    public static class TextUtils
    {
        public static IEnumerable<string> ExtractBodyLines(List<string> lines)
        {
            int index = lines.FindIndex(l => l == "---BODY---");
            return index >= 0 ? lines.Skip(index + 1) : Enumerable.Empty<string>();
        }
        public static IEnumerable<string> ExtractHeaderLines(List<string> lines)
        {
            int index = lines.FindIndex(l => l == "---BODY---");
            return index >= 0 ? lines.Take(index) : lines;
        }


    }
}
