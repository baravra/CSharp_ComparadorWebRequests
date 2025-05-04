using ComparadorWebRequests.Logic.Comparison.ContentTypes;
using ComparadorWebRequests.Logic.Models;
using ComparadorWebRequests.Utils;

namespace ComparadorWebRequests.Logic.Comparison
{
    public class RequestComparer 
    {
        // Implementa comparação de Requests
        public ComparisonResult Compare(IHttpContent portalContent, IHttpContent roboContent)
        {
            var result = new ComparisonResult();

            var linhasPortal = portalContent.GetNormalizedLines();
            var linhasRobo = roboContent.GetNormalizedLines();

            var headerDifferences = HeadersComparer.Compare(linhasPortal, linhasRobo);
            result.Results.AddRange(headerDifferences);


            return result;
        }


    }
}
