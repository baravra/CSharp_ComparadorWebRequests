using ComparadorWebRequests.Logic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComparadorWebRequests.Logic.Comparison
{
    public interface IContentComparer
    {
        // IContentComparer.cs → Interface para classes comparadoras
        bool CanCompare(string content);
        List<ComparisonResult.LineComparison> Compare(string leftContent, string rightContent);

    }
}
