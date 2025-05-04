using ComparadorWebRequests.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

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

        public List<ComparisonResult.LineComparison> Compare(string portalContent, string roboContent)
        {
            var portalToken = JToken.Parse(portalContent);
            var roboToken = JToken.Parse(roboContent);

            var results = new List<ComparisonResult.LineComparison>();
            CompareToken(string.Empty, portalToken, roboToken, results);
            return results;
        }

        private void CompareToken(string path, JToken portalToken, JToken roboToken, List<ComparisonResult.LineComparison> results)
        {
            if (portalToken.Type != roboToken.Type)
            {
                results.Add(new(path, portalToken.ToString(), roboToken.ToString(), ComparisonResult.LineStatus.Different));
                return;
            }

            switch (portalToken.Type)
            {
                case JTokenType.Object:
                    CompareObjects(path, (JObject)portalToken, (JObject)roboToken, results);
                    break;
                case JTokenType.Array:
                    CompareArrays(path, (JArray)portalToken, (JArray)roboToken, results);
                    break;
                default:
                    CompareValues(path, portalToken, roboToken, results);
                    break;
            }
        }

        private void CompareObjects(string path, JObject portalObj, JObject roboObj, List<ComparisonResult.LineComparison> results)
        {
            var handledKeys = new HashSet<string>();

            foreach (var portalProp in portalObj.Properties())
            {
                var fullPath = string.IsNullOrEmpty(path) ? portalProp.Name : $"{path}.{portalProp.Name}";

                if (roboObj.TryGetValue(portalProp.Name, out JToken roboValue))
                {
                    handledKeys.Add(roboValue.ToString(Formatting.None));
                    CompareToken(fullPath, portalProp.Value, roboValue, results);
                }
                else
                {
                    var bestMatch = ComparerUtils.FindBestMatch(
                        roboObj.Properties(),
                        portalProp,
                        p => p.Name,
                        p => p.Value.ToString(),
                        handledKeys,
                        0.95);

                    if (bestMatch.Item != null)
                    {
                        handledKeys.Add(bestMatch.Item.Value.ToString(Formatting.None));
                        AdicionarValorResult(fullPath, fullPath + ": " + portalProp.Value, fullPath + ": " + bestMatch.Item.Value, ComparisonResult.LineStatus.Different, results);
                    }
                    else
                    {
                        AdicionarValorResult(fullPath, fullPath + ": " + portalProp.Value, "", ComparisonResult.LineStatus.MissingRight, results);
                    }
                }
            }

            // Verificar propriedades restantes do roboObj
            foreach (var roboProp in roboObj.Properties())
            {
                string roboValueKey = roboProp.Value.ToString(Formatting.None);
                if (handledKeys.Contains(roboValueKey)) continue;

                var fullPath = string.IsNullOrEmpty(path) ? roboProp.Name : $"{path}.{roboProp.Name}";


                var bestMatch = ComparerUtils.FindBestMatch(
                    portalObj.Properties(),
                    roboProp,
                    p => p.Name,
                    p => p.Value.ToString(),
                    handledKeys,
                    0.95);

                if (bestMatch.Item != null)
                {
                    handledKeys.Add(roboValueKey);
                    AdicionarValorResult(fullPath, fullPath + ": " + bestMatch.Item.Value, fullPath + ": " + roboProp.Value, ComparisonResult.LineStatus.Different, results);
                }
                else
                {
                    AdicionarValorResult(fullPath, "", fullPath + ": " + roboProp.Value, ComparisonResult.LineStatus.MissingLeft, results);
                }
            }
        }

        private void CompareArrays(string path, JArray portalArray, JArray roboArray, List<ComparisonResult.LineComparison> results)
        {
            var handledRoboItems = new HashSet<string>();
            foreach (var portalItem in portalArray)
            {
                string serializedPortalItem = portalItem.ToString(Formatting.None);

                var exactMatch = roboArray.FirstOrDefault(roboItem =>
                    !handledRoboItems.Contains(roboItem.ToString(Formatting.None)) &&
                    JToken.DeepEquals(portalItem, roboItem));

                string fullPath = $"{path}[]";

                if (exactMatch != null)
                {
                    handledRoboItems.Add(exactMatch.ToString(Formatting.None));
                    AdicionarValorResult(fullPath, portalItem.ToString(), exactMatch.ToString(), ComparisonResult.LineStatus.Equal, results);
                }
                else
                {
                    // Buscar melhor similaridade se não achar um DeepEquals
                    var bestMatch = ComparerUtils.FindBestMatch(
                        roboArray,
                        portalItem,
                        t => "",
                        t => t.ToString(Formatting.None),
                        handledRoboItems,
                        0.7);

                    if (bestMatch.Item != null)
                    {
                        handledRoboItems.Add(bestMatch.Item.ToString(Formatting.None));
                        AdicionarValorResult(fullPath, portalItem.ToString(), bestMatch.Item.ToString(), ComparisonResult.LineStatus.Different, results);
                    }
                    else
                    {
                        AdicionarValorResult(fullPath, portalItem.ToString(), "", ComparisonResult.LineStatus.MissingRight, results);
                    }
                }
            }

            // Agora procura os itens restantes do robo que não foram casados
            foreach (var roboItem in roboArray)
            {
                string roboSerialized = roboItem.ToString(Formatting.None);
                if (!handledRoboItems.Contains(roboSerialized))
                {
                    string fullPath = $"{path}[]";
                    AdicionarValorResult(fullPath, "", roboSerialized, ComparisonResult.LineStatus.MissingLeft, results);
                }
            }
        }

        private void CompareValues(string path, JToken portalValue, JToken roboValue, List<ComparisonResult.LineComparison> results)
        {
            var status = JToken.DeepEquals(portalValue, roboValue)
                    ? ComparisonResult.LineStatus.Equal
                    : (ComparerUtils.IsSimilar(portalValue.ToString(), roboValue.ToString(), 0.9) ? ComparisonResult.LineStatus.Different : ComparisonResult.LineStatus.Different);

            AdicionarValorResult(
                path,
                path + ": " + portalValue.ToString(),
                path + ": " + roboValue.ToString(),
                status,
                results
            );
        }

        /*private void CompareObjects_bkp(string path, JObject portalObj, JObject roboObj, List<ComparisonResult.LineComparison> results)
        {
            var allKeys = new HashSet<string>(portalObj.Properties().Select(p => p.Name)
                .Union(roboObj.Properties().Select(p => p.Name)));

            foreach (var key in allKeys)
            {
                var fullPath = string.IsNullOrEmpty(path) ? key : $"{path}.{key}";
                var portalProp = portalObj.Property(key);
                var roboProp = roboObj.Property(key);

                if (portalProp == null)
                {
                    AdicionarValorResult(
                        fullPath
                        , ""
                        , fullPath + ": " + roboProp?.Value.ToString()
                        , ComparisonResult.LineStatus.MissingLeft
                        , results
                    );
                }
                else if (roboProp == null)
                {
                    AdicionarValorResult(
                        fullPath
                        , fullPath + ": " + portalProp.Value.ToString()
                        , ""
                        , ComparisonResult.LineStatus.MissingRight
                        , results
                    );
                }
                else
                {
                    CompareToken(fullPath, portalProp.Value, roboProp.Value, results);
                }
            }
        }

        private void CompareArrays_bkp(string path, JArray portalArray, JArray roboArray, List<ComparisonResult.LineComparison> results)
        {
            bool isObjectArray = portalArray.All(t => t.Type == JTokenType.Object) && roboArray.All(t => t.Type == JTokenType.Object);

            if (isObjectArray)
            {
                var unmatchedRoboItems = new List<JToken>(roboArray);
                foreach (var portalItem in portalArray)
                {
                    var match = unmatchedRoboItems.FirstOrDefault(r => JToken.DeepEquals(r, portalItem));
                    if (match != null)
                    {
                        unmatchedRoboItems.Remove(match);
                        AdicionarValorResult(
                            path
                            , path + ": " + portalItem.ToString()
                            , path + ": " + match.ToString()
                            , ComparisonResult.LineStatus.Equal
                            , results
                        );
                    }
                    else
                    {
                        AdicionarValorResult(
                            path
                            , path + ": " + portalItem.ToString()
                            , ""
                            , ComparisonResult.LineStatus.MissingRight
                            , results
                        );
                    }
                }

                foreach (var remaining in unmatchedRoboItems)
                {
                    AdicionarValorResult(
                        path
                        , ""
                        , path + ": " + remaining.ToString()
                        , ComparisonResult.LineStatus.MissingLeft
                        , results
                    );
                }
            }
            else
            {
                int maxLength = Math.Max(portalArray.Count, roboArray.Count);
                for (int i = 0; i < maxLength; i++)
                {
                    var itemPath = $"{path}[{i}]";

                    if (i >= portalArray.Count)
                    {
                        AdicionarValorResult(
                            itemPath
                            , ""
                            , itemPath + ": " + roboArray[i].ToString()
                            , ComparisonResult.LineStatus.MissingLeft
                            , results
                        );
                    }
                    else if (i >= roboArray.Count)
                    {
                        AdicionarValorResult(
                            itemPath
                            , itemPath + ": " + portalArray[i].ToString()
                            , ""
                            , ComparisonResult.LineStatus.MissingRight
                            , results
                        );
                    }
                    else
                    {
                        CompareToken(itemPath, portalArray[i], roboArray[i], results);
                    }
                }
            }
        }

        private void CompareValues_bkp(string path, JToken portalValue, JToken roboValue, List<ComparisonResult.LineComparison> results)
        {
            var status = JToken.DeepEquals(portalValue, roboValue)
                    ? ComparisonResult.LineStatus.Equal
                    : ComparisonResult.LineStatus.Different;

            AdicionarValorResult(
                path
                , path + ": " + portalValue.ToString()
                , path + ": " + roboValue.ToString()
                , status
                , results
            );
        }*/

        private void AdicionarValorResult(string path, string portalValue, string roboValue, ComparisonResult.LineStatus status, List<ComparisonResult.LineComparison> results)
        {
            results.Add(new
                (path
                , portalValue.Replace("\r\n", "")
                , roboValue.Replace("\r\n", "")
                , status
            ));
        }
    }
}
