using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ComparadorWebRequests.Logic.Comparison.ContentTypes
{
    public class XmlContentComparer : IContentComparer
    {
        public bool CanCompare(string content)
        {
            try
            {
                XDocument.Parse(content);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public List<ComparisonResult.LineComparison> Compare(string leftContent, string rightContent)
        {
            var left = XDocument.Parse(leftContent).Root;
            var right = XDocument.Parse(rightContent).Root;
            var differences = new List<ComparisonResult.LineComparison>();
            CompareElements("", left, right, differences);
            return differences;
        }

        private void CompareElements(string path, XElement left, XElement right, List<ComparisonResult.LineComparison> diffs)
        {
            if (left == null && right == null) return;

            string currentPath = string.IsNullOrEmpty(path) ? left?.Name.LocalName ?? right?.Name.LocalName : $"{path}/{left?.Name.LocalName ?? right?.Name.LocalName}";

            if (left == null)
            {
                diffs.Add(new(currentPath, "", right.ToString(), ComparisonResult.LineStatus.MissingLeft));
                return;
            }

            if (right == null)
            {
                diffs.Add(new(currentPath, left.ToString(), "", ComparisonResult.LineStatus.MissingRight));
                return;
            }

            if (left.Name != right.Name)
            {
                diffs.Add(new(currentPath, left.ToString(), right.ToString(), ComparisonResult.LineStatus.Different));
                return;
            }

            // Atributos
            var lAttr = left.Attributes().ToDictionary(a => a.Name, a => a.Value);
            var rAttr = right.Attributes().ToDictionary(a => a.Name, a => a.Value);
            var allAttr = new HashSet<XName>(lAttr.Keys.Concat(rAttr.Keys));

            foreach (var attr in allAttr)
            {
                lAttr.TryGetValue(attr, out var lVal);
                rAttr.TryGetValue(attr, out var rVal);

                if (lVal != rVal)
                    diffs.Add(new($"{currentPath}[@{attr.LocalName}]", lVal, rVal, ComparisonResult.LineStatus.Different));
            }

            // Conteúdo textual
            if (left.Value != right.Value)
                diffs.Add(new($"{currentPath}/text()", left.Value, right.Value, ComparisonResult.LineStatus.Different));

            // Elementos filhos
            var lChildren = left.Elements().ToList();
            var rChildren = right.Elements().ToList();
            int count = Math.Max(lChildren.Count, rChildren.Count);

            for (int i = 0; i < count; i++)
            {
                var lEl = i < lChildren.Count ? lChildren[i] : null;
                var rEl = i < rChildren.Count ? rChildren[i] : null;
                CompareElements(currentPath, lEl, rEl, diffs);
            }
        }
    }
}
