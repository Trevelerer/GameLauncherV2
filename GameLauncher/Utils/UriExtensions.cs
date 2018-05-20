using System;
using System.Linq;

namespace GameLauncher.Utils
{
    public static class UriExtensions
    {
        /// <summary>
        /// Append a series of paths to a <see cref="Uri"/> instance.
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="paths"></param>
        /// <returns></returns>
        public static Uri Append(this Uri uri, params string[] paths)
        {
            return new Uri(paths.Aggregate(uri.AbsoluteUri, (current, path) => string.Format("{0}/{1}", current.TrimEnd('/'), path.TrimStart('/'))));
        }
    }
}
