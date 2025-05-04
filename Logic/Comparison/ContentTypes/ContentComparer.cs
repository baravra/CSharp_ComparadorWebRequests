namespace ComparadorWebRequests.Logic.Comparison.ContentTypes
{
    public class ContentComparer
    {
        private readonly List<IContentComparer> _contentComparers;

        public ContentComparer()
        {
            _contentComparers = new List<IContentComparer>
            {
                new JsonContentComparer(),
                new XmlContentComparer(),
                new HtmlContentComparer(),
                new TextContentComparer()
            };
        }

        public List<ComparisonResult.LineComparison> Compare(string portalContent, string roboContent)
        {
            foreach (var comparer in _contentComparers) // identificar automaticamente o type do body
            {
                if (comparer.CanCompare(portalContent) && comparer.CanCompare(roboContent))
                {
                    return comparer.Compare(portalContent, roboContent);
                }
            }

            // Fallback para texto
            return new TextContentComparer().Compare(portalContent, roboContent);
        }
    }
}
