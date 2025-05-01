using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComparadorWebRequests.Logic.Models
{
    public class HttpResponseContent : IHttpContent
    {
        public string RawText { get; private set; }

        public string StatusLine { get; private set; }
        public Dictionary<string, string> Headers { get; private set; } = new();
        public string Body { get; private set; }

        public HttpResponseContent(string rawText)
        {
            RawText = rawText;
            Parse();
        }

        private void Parse()
        {
            var lines = RawText.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None).ToList();

            if (lines.Count == 0) return;

            StatusLine = lines[0];

            for (int i = 1; i < lines.Count; i++)
            {
                var line = lines[i];

                if (string.IsNullOrWhiteSpace(line)) // Corpo inicia aqui
                {
                    Body = string.Join("\n", lines.Skip(i + 1));
                    break;
                }

                var parts = line.Split(':', 2);
                if (parts.Length == 2)
                {
                    Headers[parts[0].Trim()] = parts[1].Trim();
                }
            }
        }

        public List<string> GetNormalizedLines()
        {
            var list = new List<string> { StatusLine };
            list.AddRange(Headers.Select(h => $"{h.Key}: {h.Value}"));
            if (!string.IsNullOrWhiteSpace(Body))
            {
                list.Add("---BODY---");
                list.AddRange(Body.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None));
            }
            return list;
        }
    }
}
