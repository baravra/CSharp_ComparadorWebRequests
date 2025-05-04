using ComparadorWebRequests.Logic.Comparison.ContentTypes;
using ComparadorWebRequests.Logic.Models;
using ComparadorWebRequests.Utils;
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

            // Header
            var headerPortal = TextUtils.ExtractHeaderLines(linhasPortal);
            var headerRobo = TextUtils.ExtractHeaderLines(linhasRobo);

            var headerDifferences = HeadersComparer.Compare(headerPortal.ToList(), headerRobo.ToList());
            result.Results.AddRange(headerDifferences);

            // Body
            string bodyPortal = string.Join("\n", TextUtils.ExtractBodyLines(linhasPortal));
            string bodyRobo = string.Join("\n", TextUtils.ExtractBodyLines(linhasRobo));

            var bodyDifferences = _contentComparer.Compare(bodyPortal, bodyRobo);
            result.Results.AddRange(bodyDifferences);

            // Return
            return result;
        }
        
    }
}
