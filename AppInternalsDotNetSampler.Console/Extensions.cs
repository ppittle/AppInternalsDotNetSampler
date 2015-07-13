using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppInternalsDotNetSampler.Console
{
    public static class Extensions
    {
        /// <summary>
        /// http://stackoverflow.com/questions/1450774/splitting-a-string-into-chunks-of-a-certain-size
        /// </summary>
        public static IEnumerable<string> SplitIntoChunks(this string str, int chunkSize)
        {
            for (int i = 0; i < str.Length; i += chunkSize)
                yield return str.Substring(i, Math.Min(chunkSize, str.Length - i));
        }
    }
}
