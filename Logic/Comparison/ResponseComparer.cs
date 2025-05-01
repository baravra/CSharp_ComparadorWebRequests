using ComparadorWebRequests.Logic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComparadorWebRequests.Logic.Comparison
{
    public class ResponseComparer : IContentComparer
    {
        public ComparisonResult Compare(IHttpContent portalContent, IHttpContent roboContent)
        {
            var left = portalContent.GetNormalizedLines();
            var right = roboContent.GetNormalizedLines();

            var result = new ComparisonResult();
            var allLines = new HashSet<string>(left.Concat(right));

            foreach (var line in allLines)
            {
                bool inLeft = left.Contains(line);
                bool inRight = right.Contains(line);

                if (inLeft && inRight)
                {
                    result.Results.Add(new ComparisonResult.LineComparison(line, line, ComparisonResult.LineStatus.Equal));
                }
                else if (inLeft)
                {
                    var similar = right.FirstOrDefault(r => IsSimilar(r, line));
                    if (similar != null)
                        result.Results.Add(new ComparisonResult.LineComparison(line, similar, ComparisonResult.LineStatus.Different));
                    else
                        result.Results.Add(new ComparisonResult.LineComparison(line, "", ComparisonResult.LineStatus.MissingRight));
                }
                else if (inRight)
                {
                    var similar = left.FirstOrDefault(l => IsSimilar(l, line));
                    if (similar != null)
                        result.Results.Add(new ComparisonResult.LineComparison(similar, line, ComparisonResult.LineStatus.Different));
                    else
                        result.Results.Add(new ComparisonResult.LineComparison("", line, ComparisonResult.LineStatus.MissingLeft));
                }
            }

            return result;
        }
        private bool IsSimilar(string a, string b)
        {
            if (string.IsNullOrEmpty(a) || string.IsNullOrEmpty(b)) return false;
            int distance = LevenshteinDistance(a, b);
            double similarity = 1.0 - ((double)distance / Math.Max(a.Length, b.Length));
            return similarity > 0.75;
        }

        private int LevenshteinDistance(string s, string t)
        {
            var n = s.Length;
            var m = t.Length;
            var d = new int[n + 1, m + 1];

            for (int i = 0; i <= n; d[i, 0] = i++) ;
            for (int j = 0; j <= m; d[0, j] = j++) ;

            for (int i = 1; i <= n; i++)
            {
                for (int j = 1; j <= m; j++)
                {
                    int cost = s[i - 1] == t[j - 1] ? 0 : 1;
                    d[i, j] = Math.Min(
                                Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                                d[i - 1, j - 1] + cost);
                }
            }

            return d[n, m];
        }
    }
}
