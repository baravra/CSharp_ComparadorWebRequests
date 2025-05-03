using ComparadorWebRequests.Logic.Comparison.ContentTypes;
using ComparadorWebRequests.Logic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComparadorWebRequests.Logic.Comparison
{
    public class ResponseComparer
    {
            private readonly ContentComparer _contentComparer = new();

            public ComparisonResult Compare(IHttpContent portalContent, IHttpContent roboContent)
            {
                var result = new ComparisonResult();

                var linhasPortal = portalContent.GetNormalizedLines();
                var linhasRobo = roboContent.GetNormalizedLines();

                string bodyPortal = string.Join("\n", ExtractBodyLines(linhasPortal));
                string bodyRobo = string.Join("\n", ExtractBodyLines(linhasRobo));

                var bodyDifferences = _contentComparer.Compare(bodyPortal, bodyRobo);
                result.Results.AddRange(bodyDifferences);

                return result;
            }

            private IEnumerable<string> ExtractBodyLines(List<string> lines)
            {
                int index = lines.FindIndex(l => l == "---BODY---");
                return index >= 0 ? lines.Skip(index + 1) : Enumerable.Empty<string>();
            }
        
    }
}
