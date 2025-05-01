using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComparadorWebRequests.Logic.Models
{
    public class HttpRequestContent : IHttpContent
    {
        public string RawText { get; private set; }

        public string Method { get; private set; }
        public string Url { get; private set; }
        public Dictionary<string, string> Headers { get; private set; } = new();
        public Dictionary<string, string> Cookies { get; private set; } = new();
        public string Body { get; private set; } // se tiver uma linha vazia no texto a seguinte será considerada o body

        public HttpRequestContent(string rawText)
        {
            RawText = rawText;
            Parse();
        }

        private void Parse()
        {
            var lines = RawText.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None).ToList();

            if (lines.Count == 0) return;

            var requestLineParts = lines[0].Split(' ');
            Method = requestLineParts[0];
            Url = requestLineParts.Length > 1 ? requestLineParts[1] : "";

            for (int i = 1; i < lines.Count; i++)
            {
                var line = lines[i];

                if (string.IsNullOrWhiteSpace(line)) // Início do corpo
                {
                    Body = string.Join("\n", lines.Skip(i + 1));
                    break;
                }

                var parts = line.Split(':', 2);
                if (parts.Length == 2)
                {
                    var key = parts[0].Trim();
                    var value = parts[1].Trim();

                    if (key.Equals("Cookie", StringComparison.OrdinalIgnoreCase))
                    {
                        var cookies = value.Split(';');
                        foreach (var cookie in cookies)
                        {
                            var kv = cookie.Split('=');
                            if (kv.Length == 2)
                                Cookies[kv[0].Trim()] = kv[1].Trim();
                        }
                    }
                    else
                    {
                        Headers[key] = value;
                    }
                }
            }
        }

        public List<string> GetNormalizedLines()
        {
            var list = new List<string> { $"{Method} {Url}" };
            list.AddRange(Headers.Select(h => $"{h.Key}: {h.Value}"));
            list.AddRange(Cookies.Select(c => $"Cookie: {c.Key}={c.Value}"));
            if (!string.IsNullOrWhiteSpace(Body))
            {
                list.Add("---BODY---");
                list.AddRange(Body.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None));
            }

            return list;
        }
    }
}
