using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace CC.Helper.Expand
{
    public static class IEnumerableExpand
    {
        public static IEnumerable<T> Remove<T>(this IEnumerable<T> list, Func<T, bool> predicate)
        {
            List<T> ls = new List<T>();
            foreach (var item in list)
            {
                if (!predicate.Invoke(item))
                {
                    ls.Add(item);
                }
            }
            return ls;
        }
    }
}
