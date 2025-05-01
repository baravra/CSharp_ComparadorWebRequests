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

        public static IContentComparer CreateComparer(ComparisonType type)
        {
            return type switch
            {
                ComparisonType.Request => new RequestComparer(),
                ComparisonType.Response => new ResponseComparer(),
                _ => throw new NotSupportedException("Tipo de comparação não suportado")
            };
        }
    }
}
