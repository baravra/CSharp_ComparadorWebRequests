using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComparadorWebRequests.Logic.Comparison.ContentTypes
{
    public class TextContentComparer : IContentComparer
    {
        public bool CanCompare(string content) => true;

        public List<ComparisonResult.LineComparison> Compare(string left, string right)
        {
            var lines1 = left.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
            var lines2 = right.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

            var results = new List<ComparisonResult.LineComparison>();
            int max = Math.Max(lines1.Length, lines2.Length);

            for (int i = 0; i < max; i++)
            {
                string l = i < lines1.Length ? lines1[i] : "";
                string r = i < lines2.Length ? lines2[i] : "";

                if (l == r)
                    results.Add(new("", l, r, ComparisonResult.LineStatus.Equal));
                else if (string.IsNullOrWhiteSpace(l))
                    results.Add(new("", "", r, ComparisonResult.LineStatus.MissingLeft));
                else if (string.IsNullOrWhiteSpace(r))
                    results.Add(new("", l, "", ComparisonResult.LineStatus.MissingRight));
                else
                    results.Add(new("", l, r, ComparisonResult.LineStatus.Different));
            }

            return results;
        }
    }
}
