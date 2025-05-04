using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComparadorWebRequests.Logic.Comparison
{
    public static class ContentComparerFactory
    {
        public enum ComparisonType
        {
            Request,
            Response
        }
        public static RequestComparer CreateRequestComparer() => new RequestComparer();
        public static ResponseComparer CreateResponseComparer() => new ResponseComparer();

        public static object CreateComparer(ComparisonType type)
        {
            return type switch
            {
                ComparisonType.Request => CreateRequestComparer(),
                ComparisonType.Response => CreateResponseComparer(),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}
