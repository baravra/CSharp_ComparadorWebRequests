using ComparadorWebRequests.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComparadorWebRequests.Logic.Comparison.ContentTypes
{
    public static class HeadersComparer
    {

        public static List<ComparisonResult.LineComparison> Compare(List<string> headerPortal, List<string> headerRobo)
        {
            var result = new ComparisonResult();

            var allLines = new HashSet<string>(headerPortal);
            allLines.UnionWith(headerRobo);

            foreach (var line in allLines)
            {
                if (result.Results.Where(x => x.LineLeft.Contains(line) || x.LineRight.Contains(line)).Count() > 0) continue;

                bool existeLinhaNoPortal = headerPortal.Contains(line);
                bool existeLinhaNoRobo = headerRobo.Contains(line);

                if (existeLinhaNoPortal && existeLinhaNoRobo)
                {
                    result.Results.Add(new ComparisonResult.LineComparison("", line, line, ComparisonResult.LineStatus.Equal));
                }
                else if (existeLinhaNoPortal)
                {
                    var similarRobo = headerRobo.FirstOrDefault(r => ComparerUtils.IsSimilar(r, line));

                    if (similarRobo != null)
                        result.Results.Add(new ComparisonResult.LineComparison("", line, similarRobo, ComparisonResult.LineStatus.Different));
                    else
                        result.Results.Add(new ComparisonResult.LineComparison("", line, "", ComparisonResult.LineStatus.MissingRight));
                }
                else if (existeLinhaNoRobo)
                {
                    var similarPortal = headerPortal.FirstOrDefault(l => ComparerUtils.IsSimilar(l, line));

                    if (similarPortal != null)
                        result.Results.Add(new ComparisonResult.LineComparison("", similarPortal, line, ComparisonResult.LineStatus.Different));
                    else
                        result.Results.Add(new ComparisonResult.LineComparison("", "", line, ComparisonResult.LineStatus.MissingLeft));

                }
            }

            return result.Results;
        }
    }
}
