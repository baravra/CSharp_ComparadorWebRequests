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

        public List<ComparisonResult.LineComparison> Compare(string portalContent, string roboContent)
        {
            var portal = XDocument.Parse(portalContent).Root;
            var robo = XDocument.Parse(roboContent).Root;
            var differences = new List<ComparisonResult.LineComparison>();
            CompareElements("", portal, robo, differences);
            return differences;
        }

        private void CompareElements(string path, XElement portal, XElement robo, List<ComparisonResult.LineComparison> diffs)
        {
            if (portal == null && robo == null) return;

            string currentPath = string.IsNullOrEmpty(path) ? portal?.Name.LocalName ?? robo?.Name.LocalName : $"{path}/{portal?.Name.LocalName ?? robo?.Name.LocalName}";

            if (portal == null)
            {
                diffs.Add(new(currentPath, "", robo.ToString(), ComparisonResult.LineStatus.MissingLeft));
                return;
            }

            if (robo == null)
            {
                diffs.Add(new(currentPath, portal.ToString(), "", ComparisonResult.LineStatus.MissingRight));
                return;
            }

            if (portal.Name != robo.Name)
            {
                diffs.Add(new(currentPath, portal.ToString(), robo.ToString(), ComparisonResult.LineStatus.Different));
                return;
            }

            // Atributos
            var lAttr = portal.Attributes().ToDictionary(a => a.Name, a => a.Value);
            var rAttr = robo.Attributes().ToDictionary(a => a.Name, a => a.Value);
            var allAttr = new HashSet<XName>(lAttr.Keys.Concat(rAttr.Keys));

            foreach (var attr in allAttr)
            {
                lAttr.TryGetValue(attr, out var lVal);
                rAttr.TryGetValue(attr, out var rVal);

                if (lVal != rVal)
                    diffs.Add(new($"{currentPath}[@{attr.LocalName}]", lVal, rVal, ComparisonResult.LineStatus.Different));
            }

            // Conteúdo textual
            if (portal.Value != robo.Value)
                diffs.Add(new($"{currentPath}/text()", portal.Value, robo.Value, ComparisonResult.LineStatus.Different));

            // Elementos filhos
            var lChildren = portal.Elements().ToList();
            var rChildren = robo.Elements().ToList();
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
