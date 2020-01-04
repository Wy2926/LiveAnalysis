using CC.Helper;
using CC.Helper.Expand;
using DouyuAgreement;
using IPPool;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LivePopularity
{
    public partial class FrmMain : Form
    {
        #region 私有变量

        /// <summary>
        /// 斗鱼协议入口实例
        /// </summary>
        DouyuEntrance douyuEntrance = new DouyuEntrance();
        #endregion

        public FrmMain()
        {
            InitializeComponent();
        }

        #region MyRegion

        private void Initialization()
        {
            //监听IP数量
            AgentPool.IpsChange += (nums) =>
            {
                this.Invoke(new Action(() => lblIpsNums.Text = $"IP数量:{nums}"));
            };
            AgentPool.AddAgentIp(JsonConfig<AgentIp>.GetSiteConfigs());
            AgentPool.SaveIps();
            RefreshRoom();
            return;
            Task.Run(async () =>
            {
                while (true)
                {
                    for (int i = 0; i < 10; i++)
                    {
                        await Task.Delay(2000);
                        try
                        {
                            using (HttpWebResponse httpWebResponse = await CCHttpRequest.CreateGetHttpResponseAsync("http://http.tiqu.alicdns.com/getip3?num=20&type=2&pro=&city=0&yys=0&port=2&pack=74963&ts=1&ys=1&cs=1&lb=1&sb=0&pb=45&mr=2&regions=&gm=4"))
                            {
                                //获取返回内容
                                string json = await httpWebResponse.GetResponseStream().ReadAllTextAsync();
                                MsgInfo(json);
                                //将JSON字符串转换为dynamic类型
                                dynamic obj = JsonConvert.DeserializeObject<dynamic>(json);
                                if (obj.success != true)
                                    continue;
                                List<AgentIp> ips = obj.data.ToObject<List<AgentIp>>();
                                AgentPool.AddAgentIp(ips);
                                AgentPool.SaveIps();
                                return;
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"【获取IP错误】：{ex.Message}");
                            MsgError($"【获取IP错误】：{ex.Message}");
                        }
                    }
                    return;
                    await Task.Delay(10000);
                }
            });
        }

        private void RefreshRoom()
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    Invoke(new Action(() => dgvRooms.DataSource = douyuEntrance.GetRoomInfos()));
                    await Task.Delay(3000);
                }
            });
        }

        private void MsgShow(RichTextBox richText, string msg, Color color)
        {
            this.Invoke(new Action(() =>
            {
                richText.Select(richText.Text.Length, 0);
                richText.SelectionColor = color;
                richText.AppendText($"{msg}\r\n");
                LimitLine(richText, 10);
            }));
        }

        private void LimitLine(RichTextBox richText, int maxLength)
        {
            string[] lines = richText.Lines;
            if (lines.Length >= maxLength)
            {
                richText.SelectionStart = 0;
                richText.SelectionLength = richText.GetFirstCharIndexFromLine(lines.Length - maxLength);
                richText.SelectedText = "";
            }
        }

        /// <summary>
        /// 异常消息
        /// </summary>
        /// <param name="msg"></param>
        public void MsgError(string msg)
        {
            MsgShow(rtbMsg, msg, Color.Red);
        }

        /// <summary>
        /// 弹幕Info消息
        /// </summary>
        /// <param name="msg"></param>
        public void MsgInfo(string msg)
        {
            MsgShow(rtbMsg, msg, Color.Green);
        }

        /// <summary>
        /// 礼物Info消息
        /// </summary>
        /// <param name="msg"></param>
        public void GiftInfo(string msg)
        {
            MsgShow(rtbGift, msg, Color.Green);
        }

        #endregion


        private void btnRoomAdd_Click(object sender, EventArgs ee)
        {
            douyuEntrance.AddMonitors(txtRoomId.Text.ToInt32(), true, txtIpNums.Text.ToInt32());
        }

        private void FrmMain_Load(object sender, EventArgs ee)
        {
            douyuEntrance.BarrageNotice += (BrrageMsg brrageMsg) =>
            {
                string result = String.Format("房间号【{2}】[{0}]: {1}", brrageMsg.Name, brrageMsg.Txt, brrageMsg.ROOM_ID);
                MsgInfo(result);
            };
            douyuEntrance.GiftNotice += (RoomGiftInfo info) =>
            {
                string giftInfo = $"【房间:{info.RoomId}】【{info.NickName}】送了{info.GiftNum}个【{DouyuRoom.GetDouyuGift(info.GiftId).Name}】";
                GiftInfo(giftInfo);
            };
            JSImplement.Control = this;
            Initialization();
            douyuEntrance.OnConnectResult += (int roomId, bool flag) =>
                MsgInfo($"【房间{roomId}】连接{(flag ? $"成功 房间人数:{douyuEntrance.GetNumber(roomId)}" : "失败")}");
            douyuEntrance.OnSignOut += (int roomId, int nums) =>
                MsgInfo($"【房间{roomId}】退出人数：{nums} 当前剩余人数:{douyuEntrance.GetNumber(roomId)}");
        }

        private void btnDelete_Click(object sender, EventArgs ee)
        {
            douyuEntrance.SignOutRoom(txtRoomId.Text.ToInt32(), txtIpNums.Text.ToInt32());
        }
    }
}
