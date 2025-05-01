using Newtonsoft.Json.Linq;

namespace ComparadorWebRequests.Logic.Comparison.ContentTypes
{
    public class JsonContentComparer : IContentComparer
    {
        public bool CanCompare(string content)
        {
            try
            {
                JToken.Parse(content);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public List<ComparisonResult.LineComparison> Compare(string leftContent, string rightContent)
        {
            var leftToken = JToken.Parse(leftContent);
            var rightToken = JToken.Parse(rightContent);

            var differences = new List<ComparisonResult.LineComparison>();
            CompareTokens(string.Empty, leftToken, rightToken, differences);
            return differences;
        }

        private void CompareTokens(string path, JToken left, JToken right, List<ComparisonResult.LineComparison> differences)
        {
            if (JToken.DeepEquals(left, right)) return;

            if (left.Type != right.Type)
            {
                differences.Add(new(path, left.ToString(), right.ToString(), ComparisonResult.LineStatus.Different));
                return;
            }

            switch (left.Type)
            {
                case JTokenType.Object:
                    var allKeys = new HashSet<string>(
                        ((JObject)left).Properties().Select(p => p.Name)
                        .Union(((JObject)right).Properties().Select(p => p.Name))
                    );

                    foreach (var key in allKeys)
                    {
                        var lProp = ((JObject)left).Property(key);
                        var rProp = ((JObject)right).Property(key);
                        var newPath = string.IsNullOrEmpty(path) ? key : $"{path}.{key}";

                        if (lProp == null)
                            differences.Add(new(newPath, "", rProp?.Value.ToString(), ComparisonResult.LineStatus.MissingLeft));
                        else if (rProp == null)
                            differences.Add(new(newPath, lProp.Value.ToString(), "", ComparisonResult.LineStatus.MissingRight));
                        else
                            CompareTokens(newPath, lProp.Value, rProp.Value, differences);
                    }
                    break;

                case JTokenType.Array:
                    var lArr = left as JArray;
                    var rArr = right as JArray;
                    int maxLen = Math.Max(lArr.Count, rArr.Count);

                    for (int i = 0; i < maxLen; i++)
                    {
                        var p = $"{path}[{i}]";
                        if (i >= lArr.Count)
                            differences.Add(new(p, "", rArr[i].ToString(), ComparisonResult.LineStatus.MissingLeft));
                        else if (i >= rArr.Count)
                            differences.Add(new(p, lArr[i].ToString(), "", ComparisonResult.LineStatus.MissingRight));
                        else
                            CompareTokens(p, lArr[i], rArr[i], differences);
                    }
                    break;

                default:
                    differences.Add(new(path, left.ToString(), right.ToString(), ComparisonResult.LineStatus.Different));
                    break;
            }
        }
    }
}
