using System.Text;

namespace ComparadorWebRequests.Logic.Comparison
{
    public class ComparisonResult
    {
        // ComparisonResult.cs → Modelo de resultado comum (reutilizado)
        public enum LineStatus
        {
            Equal,
            Different,
            MissingLeft,
            MissingRight
        }
        public record LineComparison(string Path, string LineLeft, string LineRight, LineStatus Status);

        public List<LineComparison> Results { get; set; } = new();

        public override string ToString()
        {
            var builder = new StringBuilder();
            foreach (var line in Results)
            {
                builder.AppendLine(line.Status switch
                {
                    LineStatus.Equal => $"✔️ {line.LineLeft}",
                    LineStatus.Different => $"⚠️ {line.LineLeft} ⇄ {line.LineRight}",
                    LineStatus.MissingLeft => $"❌ (Robô apenas) {line.LineRight}",
                    LineStatus.MissingRight => $"❌ (Portal apenas) {line.LineLeft}",
                    _ => ""
                });
            }
            return builder.ToString();
        }

    }
}
