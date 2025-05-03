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

            var allLines = new HashSet<string>(linhasPortal);
            allLines.UnionWith(linhasRobo);

            foreach (var line in allLines)
            {
                if (result.Results.Where(x => x.LineLeft.Contains(line) || x.LineRight.Contains(line)).Count() > 0) continue;

                bool existeLinhaNoPortal = linhasPortal.Contains(line);
                bool existeLinhaNoRobo = linhasRobo.Contains(line);

                if (existeLinhaNoPortal && existeLinhaNoRobo)
                {
                    result.Results.Add(new ComparisonResult.LineComparison("", line, line, ComparisonResult.LineStatus.Equal));
                }
                else if (existeLinhaNoPortal)
                {
                    var similarRobo = linhasRobo.FirstOrDefault(r => ComparerUtils.IsSimilar(r, line));

                    if (similarRobo != null)
                        result.Results.Add(new ComparisonResult.LineComparison("", line, similarRobo, ComparisonResult.LineStatus.Different));
                    else
                        result.Results.Add(new ComparisonResult.LineComparison("", line, "", ComparisonResult.LineStatus.MissingRight));
                }
                else if (existeLinhaNoRobo)
                {
                    var similarPortal = linhasPortal.FirstOrDefault(l => ComparerUtils.IsSimilar(l, line));

                    if (similarPortal != null)
                        result.Results.Add(new ComparisonResult.LineComparison("", similarPortal, line, ComparisonResult.LineStatus.Different));
                    else
                        result.Results.Add(new ComparisonResult.LineComparison("", "", line, ComparisonResult.LineStatus.MissingLeft));
                    
                }
            }

            return result;
        }


    }
}
