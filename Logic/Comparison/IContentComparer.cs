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

        ComparisonResult Compare(IHttpContent portalContent, IHttpContent roboContent);

    }
}
