using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComparadorWebRequests.Logic.Models
{
    public interface IHttpContent
    {
        string RawText { get; }
        List<string> GetNormalizedLines();
    }
}
