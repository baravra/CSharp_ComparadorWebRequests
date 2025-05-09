﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComparadorWebRequests.Utils
{
    public static class ComparerUtils
    {
        public static double SimilarityScore(string a, string b)
        {
            if (string.IsNullOrWhiteSpace(a) || string.IsNullOrWhiteSpace(b)) return 0.0;
            int distance = LevenshteinDistance(a.Trim(), b.Trim());
            return 1.0 - ((double)distance / Math.Max(a.Length, b.Length));
        }

        public static bool IsSimilar(string a, string b, double porcentage = 0.75)
        {
            if (string.IsNullOrEmpty(a) || string.IsNullOrEmpty(b)) return false;
            int distance = LevenshteinDistance(a.Trim(), b.Trim());
            double similarity = 1.0 - ((double)distance / Math.Max(a.Length, b.Length));
            return similarity >= porcentage; // se tiver uma similaridade de pelo menos porcentage% será considerado que é diferente invés de faltando
        }


        /* Detectar semelhanças "parciais" entre linhas de texto — mesmo que não sejam idênticas
         * Calcula a Distância de Levenshtein entre duas strings
         * Métrica de similaridade que mede quantas operações mínimas são necessárias para transformar uma string na outra
         */
        public static int LevenshteinDistance(string s, string t)
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

        public static (T? Item, double Similarity) FindBestMatch<T>(
        IEnumerable<T> source,
        T target,
        Func<T, string> getKey,
        Func<T, string> getValue,
        HashSet<string> handledKeys,
        double threshold = 0.75)
        where T : class
        {
            var bestMatch = source
                .Where(item => !handledKeys.Contains(getKey(item)))
                .Select(item =>
                {
                    double nameSim = SimilarityScore(getKey(item), getKey(target));
                    double valueSim = SimilarityScore(getValue(item), getValue(target));
                    double totalSim = (nameSim + valueSim) / 2.0;
                    return new { Item = item, Similarity = totalSim };
                })
                .Where(match => match.Similarity >= threshold)
                .OrderByDescending(match => match.Similarity)
                .FirstOrDefault();

            return bestMatch != null
                ? (bestMatch.Item, bestMatch.Similarity)
                : (null, 0.0);
        }


    }
}
