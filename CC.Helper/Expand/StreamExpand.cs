using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace CC.Helper.Expand
{
    /// <summary>
    /// Stream拓展方法
    /// </summary>
    public static class StreamExpand
    {
        /// <summary>
        /// 异步读取全部文本
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static async Task<string> ReadAllTextAsync(this Stream stream)
        {
            using (StreamReader reader = new StreamReader(stream))
            {
                return await reader.ReadToEndAsync();
            }
        }

        /// <summary>
        /// 异步读取全部文本
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static async Task<string> ReadAllTextAsync(this Stream stream,Encoding encoding)
        {
            using (StreamReader reader = new StreamReader(stream, encoding))
            {
                return await reader.ReadToEndAsync();
            }
        }
    }
}
