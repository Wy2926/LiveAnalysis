using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DouyuAgreement
{
    public class RoomGiftInfo
    {
        /// <summary>
        /// 直播间ID
        /// </summary>
        public int RoomId { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 用户昵称
        /// </summary>
        public string NickName { get; set; }

        /// <summary>
        /// 礼物ID
        /// </summary>
        public int GiftId { get; set; }

        /// <summary>
        /// 礼物数量
        /// </summary>
        public int GiftNum { get; set; }

        /// <summary>
        /// 礼物连击数
        /// </summary>
        public int Hits { get; set; }

        /// <summary>
        /// 时间戳
        /// </summary>
        public DateTime Time { get; set; }
    }
}
