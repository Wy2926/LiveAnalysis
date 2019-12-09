using LogR;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DouyuAgreement
{
    public class DouyuEntrance
    {
        ConcurrentDictionary<int, List<DouyuRoom>> douyuRooms = new ConcurrentDictionary<int, List<DouyuRoom>>();

        ConcurrentDictionary<int, RoomImmedInfo> douyuInfos = new ConcurrentDictionary<int, RoomImmedInfo>();

        /// <summary>
        /// 操作锁
        /// </summary>
        static object objLock = new object();

        /// <summary>
        /// 连接结果
        /// </summary>
        public Action<int, bool> OnConnectResult;

        /// <summary>
        /// 退出监控回调
        /// </summary>
        public Action<int, int> OnSignOut;

        /// <summary>
        /// 弹幕通知
        /// </summary>

        public Action<BrrageMsg> BarrageNotice;

        /// <summary>
        /// 礼物通知
        /// </summary>
        public Action<RoomGiftInfo> GiftNotice;

        public DouyuEntrance()
        {
            OnConnectResult += ConnectResult;
            OnSignOut += (roomId, nums) =>
            {
                douyuInfos[roomId].OkCount -= nums;
            };
        }

        #region 私有方法

        /// <summary>
        /// 连接成功或失败时触发
        /// </summary>
        /// <param name="roomId"></param>
        /// <param name="flag"></param>
        private void ConnectResult(int roomId, bool flag)
        {
            lock (objLock)
            {
                if (!flag)
                {
                    douyuInfos[roomId].ErrorCount++;
                    douyuInfos[roomId].OkCount = GetNumber(roomId);
                }
                else
                    douyuInfos[roomId].OkCount++;
            }
        }

        #endregion

        /// <summary>
        /// 添加监控房间
        /// </summary>
        /// <param name="ROOM_ID"></param>
        /// <param name="IsMonitorRoom"></param>
        /// <returns></returns>
        public async void AddMonitor(int ROOM_ID, bool IsMonitorRoom = true)
        {
            await Task.Run(async () =>
              {
                  if (!douyuRooms.ContainsKey(ROOM_ID))
                  {
                     //获取礼物
                     await DouyuRoom.GetLiwu(ROOM_ID);
                      if (JSImplement.GetJSImplement(ROOM_ID) == null)
                          new JSImplement(ROOM_ID);
                      douyuRooms[ROOM_ID] = new List<DouyuRoom>();
                  }
                  var room = new DouyuRoom(ROOM_ID, IsMonitorRoom);
                  //事件绑定
                  room.BarrageNotice = BarrageNotice;
                  room.GiftNotice = GiftNotice;
                  room.DouyuRoomError += (DouyuRoom obj) =>
                  {
                      douyuRooms[obj.ROOM_ID].Remove(obj);
                      Console.WriteLine($"【房间：{obj.ROOM_ID}】人数{douyuRooms[obj.ROOM_ID].Count}");
                      OnConnectResult?.Invoke(room.ROOM_ID, false);
                  };
                  Console.WriteLine($"初始化房间:{ROOM_ID}");
                  bool flag = await ConnectRoomAsync(room);
                 //如果成功连接
                 if (flag)
                  {
                      douyuRooms[ROOM_ID].Add(room);
                     //房间连接成功通知
                     OnConnectResult?.Invoke(room.ROOM_ID, true);
                  }
                  else
                  {
                     //房间连接失败通知
                     OnConnectResult?.Invoke(room.ROOM_ID, false);
                      room.Dispose();
                  }
              });
        }

        /// <summary>
        /// 添加监控房间
        /// </summary>
        /// <param name="ROOM_ID"></param>
        /// <param name="IsMonitorRoom"></param>
        /// <param name="nums">数量</param>
        /// <returns></returns>
        public void AddMonitors(int ROOM_ID, bool IsMonitorRoom = true, int nums = 1)
        {
            if (!douyuInfos.ContainsKey(ROOM_ID))
                douyuInfos[ROOM_ID] = new RoomImmedInfo(ROOM_ID, 0, 0, 0);
            douyuInfos[ROOM_ID].Total += nums;
            for (int i = 0; i < nums; i++)
            {
                AddMonitor(ROOM_ID, IsMonitorRoom);
            }
        }

        /// <summary>
        /// 退出房间
        /// </summary>
        /// <param name="ROOM_ID"></param>
        /// <param name="num"></param>
        public void SignOutRoom(int ROOM_ID, int num = 1)
        {
            if (!douyuRooms.ContainsKey(ROOM_ID))
                throw new Exception($"没有监控该房间【{ROOM_ID}】");
            var rooms = douyuRooms[ROOM_ID];
            var removeRoonms = rooms.Take(num).ToList();
            Console.WriteLine(removeRoonms.Count);
            removeRoonms.ForEach(p => p.Dispose());
            for (int i = 0; i < removeRoonms.Count; i++)
            {
                var room = removeRoonms[i];
                rooms.Remove(room);
            }
            OnSignOut?.Invoke(ROOM_ID, removeRoonms.Count);
            AppLog.Debug($"【房间：{ROOM_ID}】退出房间人数:{removeRoonms.Count}");
        }

        /// <summary>
        /// 获取房间人数
        /// </summary>
        /// <param name="RoomId"></param>
        /// <returns></returns>
        public int GetNumber(int RoomId)
        {
            return douyuRooms[RoomId].Count;
        }

        /// <summary>
        /// 获取房间人数
        /// </summary>
        /// <param name="RoomId"></param>
        /// <returns></returns>
        public List<DouyuRoom> GetDouyuRooms(int RoomId)
        {
            return douyuRooms[RoomId];
        }

        /// <summary>
        /// 获取指定房间的信息
        /// </summary>
        /// <param name="RoomId"></param>
        /// <returns></returns>
        public RoomImmedInfo GetRoomInfo(int RoomId)
        {
            return douyuInfos[RoomId];
        }

        /// <summary>
        /// 获取所有房间的信息
        /// </summary>
        /// <param name="RoomId"></param>
        /// <returns></returns>
        public List<RoomImmedInfo> GetRoomInfos()
        {
            return douyuInfos.Values.ToList();
        }

        public async static Task<bool> ConnectRoomAsync(DouyuRoom room)
        {
            bool flag = await room.Reconnect();
            return flag;
        }
    }
}
