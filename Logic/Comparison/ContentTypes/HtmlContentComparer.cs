using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;

namespace ComparadorWebRequests.Logic.Comparison.ContentTypes
{
    public class HtmlContentComparer : IContentComparer
    {
        public bool CanCompare(string content)
        {
            try
            {
                var doc = new HtmlDocument();
                doc.LoadHtml(content);
                return doc.DocumentNode != null;
            }
            catch
            {
                return false;
            }
        }

        public List<ComparisonResult.LineComparison> Compare(string leftContent, string rightContent)
        {
            var lDoc = new HtmlDocument();
            var rDoc = new HtmlDocument();
            lDoc.LoadHtml(leftContent);
            rDoc.LoadHtml(rightContent);

            var diffs = new List<ComparisonResult.LineComparison>();
            CompareNodes("/", lDoc.DocumentNode, rDoc.DocumentNode, diffs);
            return diffs;
        }
        private void CompareNodes(string path, HtmlNode l, HtmlNode r, List<ComparisonResult.LineComparison> diffs)
        {
            if (l == null && r == null) return;

            if (l == null)
            {
                diffs.Add(new(path, "", r.OuterHtml, ComparisonResult.LineStatus.MissingLeft));
                return;
            }

            if (r == null)
            {
                diffs.Add(new(path, l.OuterHtml, "", ComparisonResult.LineStatus.MissingRight));
                return;
            }

            if (l.Name != r.Name)
            {
                diffs.Add(new(path, l.OuterHtml, r.OuterHtml, ComparisonResult.LineStatus.Different));
                return;
            }

            var current = $"{path}/{l.Name}";

            // Atributos
            var lAttrs = l.Attributes.ToDictionary(a => a.Name, a => a.Value);
            var rAttrs = r.Attributes.ToDictionary(a => a.Name, a => a.Value);
            var allKeys = new HashSet<string>(lAttrs.Keys.Concat(rAttrs.Keys));

            foreach (var key in allKeys)
            {
                lAttrs.TryGetValue(key, out var lVal);
                rAttrs.TryGetValue(key, out var rVal);

                if (lVal != rVal)
                    diffs.Add(new($"{current}[@{key}]", lVal, rVal, ComparisonResult.LineStatus.Different));
            }

            // Texto
            if (l.InnerText.Trim() != r.InnerText.Trim())
                diffs.Add(new($"{current}/text()", l.InnerText.Trim(), r.InnerText.Trim(), ComparisonResult.LineStatus.Different));

            // Filhos
            var lChildren = l.ChildNodes.Where(n => n.NodeType == HtmlNodeType.Element).ToList();
            var rChildren = r.ChildNodes.Where(n => n.NodeType == HtmlNodeType.Element).ToList();

            int max = Math.Max(lChildren.Count, rChildren.Count);

            for (int i = 0; i < max; i++)
            {
                var lc = i < lChildren.Count ? lChildren[i] : null;
                var rc = i < rChildren.Count ? rChildren[i] : null;
                CompareNodes(current, lc, rc, diffs);
            }
        }
    }
}
