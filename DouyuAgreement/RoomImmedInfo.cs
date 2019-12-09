using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DouyuAgreement
{
    public class RoomImmedInfo
    {
        static object objLock = new object();

        private int roomId;
        /// <summary>
        /// 房间ID
        /// </summary>
        public int RoomId
        {
            get
            {
                lock (objLock)
                {
                    return roomId;
                }
            }
            set
            {
                lock (objLock)
                {
                    roomId = value;
                }
            }
        }


        private int total;
        /// <summary>
        /// 需要的总数
        /// </summary>
        public int Total
        {
            get
            {
                lock (objLock)
                {
                    return total;
                }
            }
            set
            {
                lock (objLock)
                {
                    total = value;
                }
            }
        }

        private int errorCount;
        /// <summary>
        /// 失败数
        /// </summary>
        public int ErrorCount
        {
            get
            {
                lock (objLock)
                {
                    return errorCount;
                }
            }
            set
            {
                lock (objLock)
                {
                    errorCount = value;
                }
            }
        }

        private int okCount;
        /// <summary>
        /// 成功数
        /// </summary>
        public int OkCount
        {
            get
            {
                lock (objLock)
                {
                    return okCount;
                }
            }
            set
            {
                lock (objLock)
                {
                    okCount = value;
                }
            }
        }

        public RoomImmedInfo(int RoomId,int Total,int ErrorCount,int OkCount)
        {
            this.RoomId = RoomId;
            this.Total = Total;
            this.RoomId = RoomId;
            this.OkCount = OkCount;
        }
    }
}
