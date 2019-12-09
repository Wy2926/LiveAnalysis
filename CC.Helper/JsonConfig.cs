using CC.Helper.Expand;
using CC.Helper.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace CC.Helper
{
    public class JsonConfig<T> where T : IJsonConfig, new()
    {
        public static T Config { get; private set; }
        public static List<T> Configs { get; private set; }
        /// <summary>
        /// 创建或者修改配置
        /// </summary>
        /// <param name="siteConfig"></param>
        /// <returns></returns>
        public static void CreateOrUpdateSiteConfig(T siteConfig)
        {
            siteConfig.FilePath.SaveJson(siteConfig);
            Config = siteConfig;
        }

        /// <summary>
        /// 创建或者修改配置
        /// </summary>
        /// <param name="siteConfig"></param>
        /// <returns></returns>
        public static void CreateOrUpdateSiteConfig(List<T> siteConfig)
        {
            if (siteConfig != null && siteConfig.Count > 0)
                siteConfig[0].FilePath.SaveJson(siteConfig);
            else
                (new T()).FilePath.SaveJson(siteConfig);
            Configs = siteConfig;
        }

        /// <summary>
        /// 获取配置
        /// </summary>
        /// <returns></returns>
        public static T GetSiteConfig()
        {
            if (Config != null)
                return Config;
            T t = new T();
            if (!File.Exists(t.FilePath))
                return t;
            return Config = t.FilePath.GetJsonEntity<T>();
        }

        /// <summary>
        /// 获取配置集合
        /// </summary>
        /// <returns></returns>
        public static List<T> GetSiteConfigs()
        {
            if (Config != null)
                return Configs;
            T t = new T();
            if (!File.Exists(t.FilePath))
                return new List<T>();
            return Configs = t.FilePath.GetJsonEntity<List<T>>();
        }
    }
}
