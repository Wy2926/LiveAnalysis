using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace CC.Helper.Expand
{
    /// <summary>
    /// String拓展方法
    /// </summary>
    public static class StringExpand
    {
        public static string ToAsciiTurnUtf8(this string str)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(str);
            string utf = Encoding.Default.GetString(buffer);
            return utf;
        }

        /// <summary>
        /// 获取html中纯文本（无法排除JS代码）
        /// </summary>
        /// <param name="html">html</param>
        /// <returns>纯文本</returns>
        public static string GetHtmlText(this string html)
        {
            html = System.Text.RegularExpressions.Regex.Replace(html, @"<\/*[^<>]*>", "", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            html = html.Replace("\r\n", "").Replace("\r", "").Replace("&nbsp;", "").Replace(" ", "");
            return html;
        }

        /// <summary>
        /// 将对象以JSON格式保存
        /// </summary>
        /// <param name="path"></param>
        /// <param name="obj"></param>
        public static void SaveJson(this string path, object obj)
        {
            string s = path.Substring(0, path.LastIndexOf('\\'));
            Directory.CreateDirectory(s);//如果文件夹不存在就创建它
            File.WriteAllText(path, JsonConvert.SerializeObject(obj));
        }

        /// <summary>
        /// 读取路径文件转换为实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        public static T GetJsonEntity<T>(this string path)
        {
            string json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<T>(json);
        }

        /// <summary>
        /// 转Int32类型
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static int ToInt32(this string str)
        {
            return Convert.ToInt32(str);
        }

        public static string ToMd5Str(this string str)
        {
            MD5 md5 = MD5.Create();
            byte[] c = Encoding.Default.GetBytes(str);
            byte[] b = md5.ComputeHash(c);//用来计算指定数组的hash值
            //将每一个字节数组中的元素都tostring，在转成16进制
            string newStr = null;
            for (int i = 0; i < b.Length; i++)
            {
                newStr += b[i].ToString("x2");  //ToString(param);//传入不同的param可以转换成不同的效果
            }
            return newStr;
        }
    }
}
