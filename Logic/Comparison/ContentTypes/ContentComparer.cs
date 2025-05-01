using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComparadorWebRequests.Logic.Comparison.ContentTypes
{
    public class ContentComparer
    {
        private readonly List<IContentComparer> _comparers;

        public ContentComparer()
        {
            _comparers = new List<IContentComparer>
            {
                new JsonContentComparer(),
                new XmlContentComparer(),
                new HtmlContentComparer(),
                new TextContentComparer()
            };
        }

        public List<ComparisonResult.LineComparison> Compare(string leftContent, string rightContent)
        {
            foreach (var comparer in _comparers)
            {
                if (comparer.CanCompare(leftContent) && comparer.CanCompare(rightContent))
                {
                    return comparer.Compare(leftContent, rightContent);
                }
            }

            // Fallback para texto
            return new TextContentComparer().Compare(leftContent, rightContent);
        }
    }
}
