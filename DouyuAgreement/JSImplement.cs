using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DouyuAgreement
{
    public class JSImplement
    {
        #region 静态变量

        static ConcurrentDictionary<int, JSImplement> JSImplements = new ConcurrentDictionary<int, JSImplement>();

        public static Control Control = null;
        #endregion
        //public ChromiumWebBrowser webBrowser;

        public Action<int> JsLoadSuccess;

        public Action<int> JsLoadError;

        private int roomId = 0;

        //        void Load(ChromiumWebBrowser _webBrowser)
        //        {
        //            _webBrowser.IsBrowserInitializedChanged += (s, e) =>
        //            {
        //                Console.WriteLine("初始化完成");
        //            };
        //            _webBrowser.LoadingStateChanged += (s, e) =>
        //            {
        //                Console.WriteLine($"加载状态：{e.IsLoading}");
        //                if (!e.IsLoading)
        //                {
        //                    _webBrowser.GetBrowser().MainFrame.EvaluateScriptAsync("document.body.innerHTML='';");
        //                    _webBrowser.GetBrowser().MainFrame.EvaluateScriptAsync(@"        
        //function Getkd(aaa, b, c, d) {
        //            var sss = window['k927cea2d4369'](aaa, b, c, d);
        //                    return aaa.join(',');
        //                }
        //                ");
        //                    //加载成功通知
        //                    JsLoadSuccess?.Invoke(roomId);
        //                }
        //            };
        //            _webBrowser.LoadError += (s, e) =>
        //            {
        //                Load();
        //                JsLoadError?.Invoke(roomId);
        //            };
        //        }



        public JSImplement(int roomId)
        {
            //加载斗鱼房间的加密JS
            //webBrowser = new ChromiumWebBrowser($"https://www.douyu.com/{roomId}");
            //webBrowser.Visible = false;
            //this.webBrowser.ActivateBrowserOnCreation = false;
            //this.roomId = roomId;
            //Load(webBrowser);
            //JSImplements[roomId] = this;
            //Control.Controls.Add(webBrowser);
        }

        /// <summary>
        /// 重新加载页面
        /// </summary>
        public void Load()
        {
            //webBrowser.Load($"https://www.douyu.com/{roomId}");
        }

        /// <summary>
        /// 获取加密之后的kd
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public async Task<long[]> Getkd(object[] obj)
        {
            //var req = await webBrowser.GetBrowser().MainFrame.EvaluateScriptAsync($"Getkd([{string.Join(",", obj[0] as long[])}],{obj[1]},'{obj[2]}',{obj[3]});");//运行页面上js的test方法
            //long[] longs = req.Result.ToString().Split(',').Select(p => long.Parse(p)).ToArray();
            //return longs;
            return null;
        }


        public static JSImplement GetJSImplement(int roomId)
        {
            if (JSImplements.ContainsKey(roomId))
                return JSImplements[roomId];
            return null;
        }
    }
}
