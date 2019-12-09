using System;
using System.Linq;
using System.Linq.Expressions;

namespace CC.Helper.Expand
{
    /// <summary>
    /// IQueryable的泛型拓展方法
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class IQueryableExpand
    {
        /// <summary>
        /// 如果<param name="condition">为真，则执行<param name="predicate"></param>过滤
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="condition"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static IQueryable<T> WhereIfTree<T>(this IQueryable<T> source, bool condition, Expression<Func<T, bool>> predicate)
        {
            if (condition)
               return source.Where(predicate);
            return source;
        }
    }
}
