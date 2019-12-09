using CC.Helper;
using CC.Helper.Expand;
using IPPool;
using LogR;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using WebSocketSharp;

namespace DouyuAgreement
{
    public class DouyuRoom : IDisposable
    {
        static ConcurrentDictionary<int, DouyuGift> douyuGifts = new ConcurrentDictionary<int, DouyuGift>();

        /// <summary>
        /// 房间号
        /// </summary>
        public int ROOM_ID { get; private set; }

        /// <summary>
        /// 斗鱼认证Scoket连接实例
        /// </summary>
        public WebSocket AuthClient { get; private set; }

        /// <summary>
        /// 斗鱼弹幕Scoket连接实例
        /// </summary>
        public WebSocket DanmuClient { get; private set; }

        /// <summary>
        /// 是否需要监控弹幕
        /// </summary>
        public bool IsMonitorRoom { get; private set; }

        /// <summary>
        /// 代理信息
        /// </summary>
        public AgentIp agent { get; private set; }

        /// <summary>
        /// 弹幕通知
        /// </summary>
        public Action<BrrageMsg> BarrageNotice { get; set; }

        /// <summary>
        /// 礼物通知
        /// </summary>
        public Action<RoomGiftInfo> GiftNotice { get; set; }

        /// <summary>
        /// 重试失败通知
        /// </summary>
        public Action<DouyuRoom> DouyuRoomError { get; set; }

        /// <summary>
        /// 用于取消该实例等待所有线程
        /// </summary>
        private CancellationTokenSource source;

        /// <summary>
        /// 拼接处理后被丢弃的数据，防止弹幕丢失
        /// </summary>
        private string FIX_TAIL = String.Empty;

        /// <summary>
        /// 重试次数
        /// </summary>
        public static int RetryNum = 10;
        /// <summary>
        /// 已连续连接次数
        /// </summary>
        public int AlreadyRetryNum { get; private set; }

        #region 私有字段

        /// <summary>
        /// 锁
        /// </summary>
        private object objLock = new object();

        #endregion

        #region 斗鱼请求服务器所需参数

        /// <summary>
        /// 所需加密参数
        /// </summary>
        static string md5Yan = "r5*^5;}2#${XF[h+;'./.Q'1;,-]f'p[";


        /// <summary>
        /// 设备ID
        /// </summary>
        private string devid = string.Empty;

        /// <summary>
        /// 时间戳
        /// </summary>
        private long timeStamp = 0;

        /// <summary>
        /// 时间戳
        /// </summary>
        private string kd = null;

        /// <summary>
        /// 用户名
        /// </summary>
        private string username = null;

        /// <summary>
        /// UID
        /// </summary>
        private string userid = null;

        #endregion



        /// <summary>
        /// 初始化斗鱼房间
        /// </summary>
        /// <param name="ROOM_ID">要进入的房间的房间号</param>
        /// <param name="IsMonitorRoom">是否监控弹幕</param>
        public DouyuRoom(int ROOM_ID, bool IsMonitorRoom = true)
        {
            this.ROOM_ID = ROOM_ID;
            this.IsMonitorRoom = IsMonitorRoom;
            Console.WriteLine($"斗鱼房间:{ROOM_ID}");
        }


        /// <summary>
        /// 初始化连接是否成功
        /// </summary>
        /// <returns></returns>
        public bool Initialization()
        {
            try
            {
                //取消之前正在运行的线程
                if (source != null)
                    source.Cancel();
                source = new CancellationTokenSource();

                agent?.RemoveRoom(ROOM_ID);
                var ips = AgentPool.GetAgentIp(ROOM_ID);
                if (ips.Count > 0)
                {
                    agent = ips[0];
                    Console.WriteLine($"{agent.Ip}:{agent.Port}");
                }
                else
                    agent = null;
                AuthClient?.CloseAsync();
                DanmuClient?.CloseAsync();
                //创建实例
                AuthClient = CreateAuthSocket(source.Token);

                //认证
                Authentication();
                //认证通过开启心跳
                AuthKeepAlive(source.Token);

                DanmuClient = CreateDanmuSocket(source.Token);
                //tcpClient = SocketHelper.InitTcp(agent.Ip, agent.Port);
                //ConnectProxyServer(SERVER_DOMAIN, SERVER_PORT, tcpClient);

                //登录房间
                RoomLogin();
                //心跳维持
                DanmmuKeepAlive(source.Token);
                AppLog.Debug($"监控房间【{ROOM_ID}】成功");
                return true;
            }
            catch (Exception ex)
            {
                AppLog.Error($"【房间：{ROOM_ID}】【监控失败】", ex);
                return false;
            }
        }

        /// <summary>
        /// 取消监控弹幕
        /// </summary>
        public void CloseMonitor()
        {
            IsMonitorRoom = false;
        }

        /// <summary>
        /// 认证
        /// </summary>
        private async void Authentication()
        {
            //获取游客账号或者登录
            devid = Guid.NewGuid().ToString("N");
            timeStamp = await GetTimestamp(devid);
            string login = $"type@=loginreq/roomid@={ROOM_ID}/dfl@=sn@AA=105@ASss@AA=1/username@=/password@=/ltkid@=/biz@=/stk@=/devid@={devid}/ct@=0/pt@=2/cvr@=0/tvr@=7/apd@=/rt@={timeStamp}/vk@={$"{timeStamp}{md5Yan}{devid}".ToMd5Str()}/ver@=20190610/aver@=218101901/";
            byte[] loginBytes = DataToBytes(login);
            AuthClient.Send(loginBytes);
            string str = $"type@=h5ckreq/rid@={ROOM_ID}/ti@=220120191130/";
            AuthClient.Send(DataToBytes(str));
            str = $"type@=qrl/et@=0/rid@={ROOM_ID}/";
            AuthClient.Send(DataToBytes(str));
        }

        /// <summary>
        /// 登录房间
        /// </summary>
        private void RoomLogin()
        {
            //string login = "type@=loginreq/roomid@=" + ROOM_ID + "/";
            string login = $"type@=loginreq/roomid@={ROOM_ID}/dfl@=sn@AA=105@ASss@AA=1/username@={username}/uid@={userid}/ver@=20190610/aver@=218101901/ct@=0/";
            byte[] loginBytes = DataToBytes(login);
            DanmuClient.Send(loginBytes);
            string joingroup = "type@=joingroup/rid@=" + ROOM_ID + "/gid@=-9999/";
            byte[] joingroupBytes = DataToBytes(joingroup);
            DanmuClient.Send(joingroupBytes);
            AppLog.Debug($"登录房间【{ROOM_ID}】成功");
        }

        /// <summary>
        /// 提取弹幕信息
        /// </summary>
        /// <param name="msg"></param>
        void ShowMsg(string msg)
        {
            msg = FIX_TAIL + msg;
            string[] chatmsgArray = Regex.Split(msg, "type@=", RegexOptions.IgnoreCase);
            FIX_TAIL = chatmsgArray[chatmsgArray.Length - 1];   //截取最后的丢弃数据，放在下个包的开头，防止数据丢失
            string[] newChatmsgArrayArr = new string[chatmsgArray.Length - 1];
            Array.Copy(chatmsgArray, 0, newChatmsgArrayArr, 0, chatmsgArray.Length - 1);

            foreach (string t in newChatmsgArrayArr)
            {
                string[] msgType = t.Split('/');
                if (msgType.Length >= 2)
                {
                    string type = msgType[0];
                    //if (!ls.Contains(type))
                    //{
                    //    Console.WriteLine(t);
                    //    Console.WriteLine();
                    //    Console.WriteLine();
                    //    Console.WriteLine();
                    //    Console.WriteLine();
                    //    Console.WriteLine();
                    //    ls.Add(type);
                    //}
                    if (type == "chatmsg")
                    {
                        BrrageMsg brrageMsg = GetMsgType(msgType);
                        brrageMsg.ROOM_ID = ROOM_ID;
                        //通知弹幕来了
                        BarrageNotice?.Invoke(brrageMsg);
                    }
                    else if (type == "spbc")
                    {
                        Console.WriteLine(t);
                    }
                    else if (type == "dgb")
                    {
                        RoomGiftInfo info = GetGift(msgType);
                        Console.WriteLine(t);
                        if (!douyuGifts.ContainsKey(info.GiftId))
                        {
                            Console.WriteLine($"不包含礼物【{info.GiftId}】");
                            continue;
                        }
                        GiftNotice?.Invoke(info);
                        Console.WriteLine($"【{info.NickName}】送了{info.GiftNum}个【{douyuGifts[info.GiftId].Name}】");
                    }
                }
            }
        }

        HashSet<string> ls = new HashSet<string>();

        #region 心跳包

        async void AuthKeepAlive(CancellationToken token)
        {
            await Task.Run(async () =>
            {
                await Task.Delay(5000);
                while (true)
                {
                    try
                    {
                        long timeStamp = CCTime.GetTimeStamp();
                        string str = $"type@=keeplive/vbw@=0/cdn@=cmcc/tick@={timeStamp}/kd@={(kd == null ? "" : await Getkd(kd, ROOM_ID, devid, timeStamp))}/";
                        byte[] aliveMsg = DataToBytes(str);
                        if (token.IsCancellationRequested)
                            return;
                        AppLog.Info($"认证服务器【房间{ROOM_ID}】【发送心跳包】");
                        AuthClient.Send(aliveMsg);
                        await Task.Delay(20000);
                    }
                    catch (Exception ex)
                    {
                        //if (AuthClient == null || AuthClient.ReadyState != WebSocketState.Open)
                        //    return;
                        AppLog.Error("【认证服务器】心跳异常", ex);
                        //if (!token.IsCancellationRequested)
                        //    Reconnect();
                        return;
                    }
                }
            });
        }

        /// <summary>
        /// 心跳包监控
        /// </summary>
        /// <param name="obj"></param>
        async void DanmmuKeepAlive(CancellationToken token)
        {
            byte[] aliveMsg = DataToBytes("type@=mrkl/");
            await Task.Run(async () =>
            {
                while (true)
                {
                    try
                    {
                        if (token.IsCancellationRequested)
                            return;
                        AppLog.Info($"弹幕服务器【房间{ROOM_ID}】【发送心跳包】");
                        DanmuClient.Send(aliveMsg);
                        await Task.Delay(30000);
                    }
                    catch (Exception ex)
                    {
                        //if (DanmuClient == null|| DanmuClient.ReadyState!= WebSocketState.Open)
                        //    return;
                        AppLog.Error("心跳异常", ex);
                        //if (!token.IsCancellationRequested)
                        //    Reconnect();
                        return;
                    }
                }
            });
        }

        #endregion

        #region 创建连接

        /// <summary>
        /// 创建认证服务器所需要的websocket实例
        /// </summary>
        /// <returns></returns>
        WebSocket CreateAuthSocket(CancellationToken token)
        {
            WebSocket webSocket = new WebSocket("wss://wsproxy.douyu.com:6671/")
            {
                Origin = "https://www.douyu.com",
            };
            if (agent != null)
                webSocket.SetProxy($"http://{agent.Ip}:{agent.Port}", null, null);
            webSocket.OnClose += (s, e) =>
            {
                Console.WriteLine($"认证服务器【{ROOM_ID}】断开连接{e.Reason}");
                if (!token.IsCancellationRequested)
                    Reconnect();
            };
            webSocket.OnError += (s, e) =>
            {
                Console.WriteLine($"认证服务器【{ROOM_ID}】报错{e.Message}");
            };
            webSocket.OnMessage += (s, e) =>
            {
                string data = Encoding.UTF8.GetString(e.RawData);
                if (data.Contains("type@=keeplive"))
                {
                    string[] str = data.Split('/').Select(p => p.Split('=')).FirstOrDefault(p => p[0] == "kd@");
                    //kd = str[1];
                }
                else if (data.Contains("type@=loginres"))
                {
                    var _data = data.Split('/').Select(p => p.Split('='));
                    username = _data.FirstOrDefault(p => p[0] == "username@")[1];
                    userid = _data.FirstOrDefault(p => p[0] == "userid@")[1];
                }
            };
            try
            {
                Console.WriteLine("【认证创建连接】");
                webSocket.Connect();
                Console.WriteLine("【认证连接成功】");
                return webSocket;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 创建认证服务器所需要的websocket实例
        /// </summary>
        /// <returns></returns>
        WebSocket CreateDanmuSocket(CancellationToken token)
        {
            WebSocket webSocket = new WebSocket("wss://danmuproxy.douyu.com:8501")
            {
                Origin = "https://www.douyu.com",
            };
            if (agent != null)
                webSocket.SetProxy($"http://{agent.Ip}:{agent.Port}", null, null);
            webSocket.OnClose += (s, e) =>
           {
               Console.WriteLine($"弹幕服务器【{ROOM_ID}】断开连接{e.Reason}");
               if (!token.IsCancellationRequested)
                   Reconnect();
           };
            webSocket.OnError += (s, e) =>
           {
               Console.WriteLine($"弹幕服务器【{ROOM_ID}】报错{e.Message}");

           };
            webSocket.OnMessage += (s, e) =>
            {
                string data = Encoding.UTF8.GetString(e.RawData);
                ShowMsg(data);
            };
            try
            {
                webSocket.Connect();
                return webSocket;
            }
            catch (Exception)
            {
                return null;
            }
        }

        #endregion

        #region 信息获取

        /// <summary>
        /// 获取弹幕信息
        /// </summary>
        /// <param name="msgType"></param>
        /// <returns></returns>
        static BrrageMsg GetMsgType(string[] msgType)
        {
            BrrageMsg brrageMsg = new BrrageMsg();
            foreach (string keyValueTemp in msgType)
            {
                string[] keyValue = Regex.Split(keyValueTemp, "@=", RegexOptions.IgnoreCase);
                if (keyValue.Length >= 2)
                {
                    string key = keyValue[0];
                    string[] textArr = new string[keyValue.Length - 1];
                    Array.Copy(keyValue, 1, textArr, 0, keyValue.Length - 1);
                    string value = String.Join("@", textArr);
                    if (key == "nn")
                    {
                        brrageMsg.Name = value;
                    }
                    if ((key == "txt"))
                    {
                        brrageMsg.Txt = value;
                    }
                }
            }
            return brrageMsg;
        }

        /// <summary>
        /// 获取礼物信息
        /// </summary>
        /// <param name="msgType"></param>
        /// <returns></returns>
        static RoomGiftInfo GetGift(string[] msgType)
        {
            RoomGiftInfo info = new RoomGiftInfo();
            var gift = msgType.Select(p => Regex.Split(p, "@=", RegexOptions.IgnoreCase))
                  .Where(p => p.Length >= 2)
                  .ToList();
            info.RoomId = Convert.ToInt32(gift.FirstOrDefault(p=>p[0]== "rid")[1]);
            info.GiftId = Convert.ToInt32(gift.FirstOrDefault(p=>p[0]== "gfid")[1]);
            info.UserId = Convert.ToInt32(gift.FirstOrDefault(p=>p[0]== "uid")[1]);
            info.GiftNum = Convert.ToInt32(gift.FirstOrDefault(p=>p[0]== "gfcnt")[1]);
            info.NickName = gift.FirstOrDefault(p => p[0] == "nn")[1];
            info.Time = DateTime.Now;
            return info;
        }


        #endregion

        #region 杂七杂八

        /// <summary>
        /// 根据斗鱼的协议对文本转换成二进制数组
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        static byte[] DataToBytes(string data)
        {
            string dantaNew = data + "\0";
            byte[] bodyDataByte = Encoding.UTF8.GetBytes(dantaNew);
            byte[] cType = BitConverter.GetBytes(689);

            //int dataLength = dantaNew.Length + cType.Length + 8;
            int dataLength = dantaNew.Length + cType.Length + 4;
            byte[] dataLengthByte = BitConverter.GetBytes(dataLength);
            byte[] dataLengthByte2 = BitConverter.GetBytes(dataLength);
            byte[] result = new byte[dataLength + 4];

            Array.Copy(dataLengthByte, 0, result, 0, 4);
            Array.Copy(dataLengthByte2, 0, result, 4, 4);
            Array.Copy(cType, 0, result, 8, 4);
            Array.Copy(bodyDataByte, 0, result, 12, bodyDataByte.Length);
            byte[] source = new byte[result.Length];
            Array.Copy(result, 0, source, 0, result.Length);
            return result;
        }

        async static Task<long> GetTimestamp(string dy_did)
        {
            Dictionary<string, string> header = new Dictionary<string, string>();
            header["authority"] = "www.douyu.com";
            header["method"] = "GET";
            header["path"] = "/api/v1/timestamp";
            header["scheme"] = "https";
            header["Cookie"] = $"dy_did={dy_did};acf_did={dy_did}";
            HttpWebResponse response = await CCHttpRequest.CreateGetHttpResponseAsync("https://www.douyu.com/api/v1/timestamp", header: header);
            string json = await response.GetResponseStream().ReadAllTextAsync();
            dynamic obj = JsonConvert.DeserializeObject<dynamic>(json);
            return obj.data;
        }

        static bool ConnectProxyServer(string strRemoteHost, int iRemotePort, Socket sProxyServer)
        {
            //构造Socks5代理服务器第一连接头(无用户名密码)
            byte[] bySock5Send = new Byte[10];
            bySock5Send[0] = 5;
            bySock5Send[1] = 1;
            bySock5Send[2] = 0;
            //发送Socks5代理第一次连接信息
            sProxyServer.Send(bySock5Send, 3, SocketFlags.None);
            byte[] bySock5Receive = new byte[10];
            int iRecCount = sProxyServer.Receive(bySock5Receive, bySock5Receive.Length, SocketFlags.None);
            if (iRecCount < 2)
            {
                sProxyServer.Close();
                throw new Exception("不能获得代理服务器正确响应。");
            }
            if (bySock5Receive[0] != 5 || (bySock5Receive[1] != 0 && bySock5Receive[1] != 2))
            {
                sProxyServer.Close();
                throw new Exception("代理服务其返回的响应错误。");
            }
            if (bySock5Receive[1] == 0)
            {
                bySock5Send[0] = 5;
                bySock5Send[1] = 1;
                bySock5Send[2] = 0;
                bySock5Send[3] = 1;

                string strIp = strRemoteHost;
                bool flag = IPAddress.TryParse(strRemoteHost, out IPAddress hostadd);
                //如果传进来的不是IP
                if (!flag)
                {
                    IPAddress ipAdd = Dns.GetHostEntry(strRemoteHost).AddressList[0];
                    strIp = ipAdd.ToString();
                }
                string[] strAryTemp = strIp.Split(new char[] { '.' });
                bySock5Send[4] = Convert.ToByte(strAryTemp[0]);
                bySock5Send[5] = Convert.ToByte(strAryTemp[1]);
                bySock5Send[6] = Convert.ToByte(strAryTemp[2]);
                bySock5Send[7] = Convert.ToByte(strAryTemp[3]);
                bySock5Send[8] = (byte)(iRemotePort / 256);
                bySock5Send[9] = (byte)(iRemotePort % 256);
                sProxyServer.Send(bySock5Send, bySock5Send.Length, SocketFlags.None);
                iRecCount = sProxyServer.Receive(bySock5Receive, bySock5Receive.Length, SocketFlags.None);
                if (bySock5Receive[0] != 5 || bySock5Receive[1] != 0)
                {
                    sProxyServer.Close();
                    throw new Exception("第二次连接Socks5代理返回数据出错。");
                }
                return true;
            }
            else
            {
                if (bySock5Receive[1] == 2)
                    throw new Exception("代理服务器需要进行身份确认。");
                else return false;
            }
        }

        /// <summary>
        /// 获取礼物
        /// </summary>
        /// <param name="roomId"></param>
        /// <returns></returns>
        public async static Task<bool> GetLiwu(int roomId)
        {
            for (int i = 0; i < 10; i++)
            {
                try
                {
                    HttpWebResponse response = await CCHttpRequest.CreateGetHttpResponseAsync($"https://gift.douyucdn.cn/api/gift/v2/web/list?rid={roomId}");
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        string json = await response.GetResponseStream().ReadAllTextAsync();
                        dynamic _dy = JsonConvert.DeserializeObject<dynamic>(json);
                        if (_dy.error == 0)
                        {
                            foreach (var item in _dy.data.giftList)
                            {
                                if (!douyuGifts.ContainsKey((int)item.id))
                                {
                                    DouyuGift douyuGift = new DouyuGift()
                                    {
                                        Id = (int)item.id,
                                        Name = item.name,
                                        Type = item.priceInfo.priceType,
                                        Price = (double)item.priceInfo.price
                                    };
                                    Console.WriteLine($"礼物【{douyuGift.Name}】 ID:{douyuGift.Id}");
                                    douyuGifts[douyuGift.Id] = douyuGift;
                                }
                            }
                            return true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    AppLog.Error($"获取直播间【{roomId}】礼物信息失败", ex);
                }
            }
            return false;
        }

        /// <summary>
        /// 获取指定礼物信息
        /// </summary>
        /// <param name="giftId"></param>
        /// <returns></returns>
        public static DouyuGift GetDouyuGift(int giftId)
        {
            return douyuGifts[giftId];
        }

        #endregion

        #region 斗鱼加密方法

        static long[] hex2bin(string kd)
        {
            string e = kd;
            var r = new List<string>();
            int t = e.Length;
            int o = 0;
            for (; o < t;)
            {
                r.Add(e.Substring(o, 2));
                o += 2;
            }
            var n = new List<long>();
            int i = r.Count;
            int s = 0;
            for (; s < i;)
            {
                var sss = r.GetRange(s, 4);
                sss.Reverse();
                n.Add(Convert.ToInt64(string.Join("", sss), 16));
                s += 4;
            }
            return n.ToArray();
        }

        static string hex(long[] e)
        {
            foreach (var item in e)
            {
                Console.WriteLine(item);
            }
            var r = "0123456789abcdef".ToCharArray();
            StringBuilder sb = new StringBuilder(32);
            foreach (var item in e)
            {
                int o = 0;
                for (; o < 4; o++)
                    sb.Append("" + r[item >> 8 * o + 4 & 15] + r[item >> 8 * o & 15]);
            }
            return sb.ToString();
        }

        public static async Task<string> Getkd(string kd, int rommId, string devid, long timeStamp)
        {
            object[] obj = new object[4];
            obj[0] = hex2bin(kd);
            obj[1] = rommId;
            obj[2] = devid;
            obj[3] = timeStamp;
            long[] longs = await JSImplement.GetJSImplement(rommId).Getkd(obj);
            return hex(longs);
        }
        #endregion

        /// <summary>
        /// 是否需要重试
        /// </summary>
        private bool IsReconnect = true;

        public async Task<bool> Reconnect()
        {
            lock (objLock)
            {
                if (!IsReconnect)
                {
                    return false;
                }
                IsReconnect = false;
            }
            for (; AlreadyRetryNum < 10; AlreadyRetryNum++)
            {
                Console.WriteLine($"重试第{AlreadyRetryNum}次");
                bool flag = Initialization();
                if (flag)
                {
                    lock (objLock)
                    {
                        AlreadyRetryNum = 0;
                        IsReconnect = true;
                        return true;
                    }
                }
                await Task.Delay(3000);
            }
            //重试超时
            if (AlreadyRetryNum >= RetryNum)
            {
                this.Dispose();
                DouyuRoomError?.Invoke(this);
                return false;
            }
            return true;
        }


        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            Console.WriteLine($"开始退出房间【{ROOM_ID}】");
            source.Cancel();
            AuthClient?.CloseAsync();
            DanmuClient?.CloseAsync();
            AuthClient = null;
            DanmuClient = null;
            if (agent != null)
                agent.RemoveRoom(ROOM_ID);
            Console.WriteLine($"结束监控房间【{ROOM_ID}】");
        }
    }
}
