using ComparadorWebRequests.Logic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComparadorWebRequests.Logic.Comparison
{
    public class RequestComparer : IContentComparer
    {
        // Implementa comparação de Requests
        public ComparisonResult Compare(IHttpContent portalContent, IHttpContent roboContent)
        {
            var result = new ComparisonResult();

            var linhasPortal = portalContent.GetNormalizedLines();
            var linhasRobo = roboContent.GetNormalizedLines();

            var allLines = new HashSet<string>(linhasPortal.Concat(linhasRobo));

            foreach (var line in allLines)
            {
                if (result.Results.Where(x => x.LineLeft.Contains(line) || x.LineRight.Contains(line)).Count() > 0) continue;

                bool existeLinhaNoPortal = linhasPortal.Contains(line);
                bool existeLinhaNoRobo = linhasRobo.Contains(line);

                if (existeLinhaNoPortal && existeLinhaNoRobo)
                {
                    result.Results.Add(new ComparisonResult.LineComparison(line, line, ComparisonResult.LineStatus.Equal));
                }
                else if (existeLinhaNoPortal)
                {
                    var similarRobo = linhasRobo.FirstOrDefault(r => IsSimilar(r, line));

                    if (similarRobo != null)
                        result.Results.Add(new ComparisonResult.LineComparison(line, similarRobo, ComparisonResult.LineStatus.Different));
                    else
                        result.Results.Add(new ComparisonResult.LineComparison(line, "", ComparisonResult.LineStatus.MissingRight));
                }
                else if (existeLinhaNoRobo)
                {
                    var similarPortal = linhasPortal.FirstOrDefault(l => IsSimilar(l, line));

                    if (similarPortal != null)
                        result.Results.Add(new ComparisonResult.LineComparison(similarPortal, line, ComparisonResult.LineStatus.Different));
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
            return similarity > 0.75; // se tiver uma similaridade de pelo menos 75% será considerado que é diferente invés de faltando
        }

        /* Detectar semelhanças "parciais" entre linhas de texto — mesmo que não sejam idênticas
         * Calcula a Distância de Levenshtein entre duas strings
         * Métrica de similaridade que mede quantas operações mínimas são necessárias para transformar uma string na outra
         */
        private int LevenshteinDistance(string s, string t)
        {
            int n = s.Length;
            int m = t.Length;
            var d = new int[n + 1, m + 1];

            for (int i = 0; i <= n; d[i, 0] = i++) ;
            for (int j = 0; j <= m; d[0, j] = j++) ;

            for (int i = 1; i <= n; i++)
            {
                for (int j = 1; j <= m; j++)
                {
                    int cost = s[i - 1] == t[j - 1] ? 0 : 1;
                    d[i, j] = Math.Min(Math.Min(
                        d[i - 1, j] + 1,
                        d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }

            return d[n, m];
        }

    }
}
