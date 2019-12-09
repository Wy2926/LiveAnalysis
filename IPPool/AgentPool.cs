using CC.Helper;
using CC.Helper.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPPool
{
    /// <summary>
    /// 代理IP
    /// </summary>
    public class AgentIp : IJsonConfig
    {
        public string FilePath => $@"\Config\ips.json";
        /// <summary>
        /// 城市
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// 到期时间
        /// </summary>
        public DateTime Expire_time { get; set; }

        /// <summary>
        /// Host
        /// </summary>
        public string Ip { get; set; }

        /// <summary>
        /// 服务提供商
        /// </summary>
        public string Ips { get; set; }

        /// <summary>
        /// 端口
        /// </summary>
        public int Port { get; set; }

        public AgentIp()
        {

        }
        public AgentIp(string City, DateTime Expire_time, string Ip, int Port, string Ips)
        {
            this.City = City;
            this.Expire_time = Expire_time;
            this.Ip = Ip;
            this.Ips = Ips;
            this.Port = Port;
        }

        internal HashSet<int> Rooms { get; set; } = new HashSet<int>();
        private static object obj = new object();
        public void RemoveRoom(int RoomId)
        {
            lock (obj)
            {
                Rooms.Remove(RoomId);
            }
        }
    }

    public static class AgentPool
    {
        static List<AgentIp> _agentIps = new List<AgentIp>();

        static object objLock = new object();

        public static Action<int> IpsChange;
        static AgentPool()
        {
            Task.Run(async () =>
            {
                FilterIp();
                await Task.Delay(1000);
            });
        }

        /// <summary>
        /// 过滤超过时间的IP
        /// </summary>
        private static List<AgentIp> FilterIp(List<AgentIp> agentIps)
        {
            return agentIps.Where(p => p.Expire_time > DateTime.Now).ToList();
        }

        /// <summary>
        /// 过滤超过时间的IP
        /// </summary>
        private static void FilterIp()
        {
            lock (objLock)
            {
                _agentIps = _agentIps.Where(p => p.Expire_time > DateTime.Now).ToList();
            }
        }
        /// <summary>
        /// 添加代理IP
        /// </summary>
        /// <param name="agentIps"></param>
        public static void AddAgentIp(List<AgentIp> agentIps)
        {
            _agentIps.AddRange(FilterIp(agentIps));
            IpsChange?.Invoke(_agentIps.Count);
        }

        /// <summary>
        /// 读取IP（房间号记录）
        /// </summary>
        /// <param name="room_Id">房间ID</param>
        /// <param name="num">IP数量</param>
        /// <returns></returns>
        public static List<AgentIp> GetAgentIp(int room_Id, int num = 1)
        {
            lock (objLock)
            {
                var ips = _agentIps
                    .Where(p => !p.Rooms.Contains(room_Id))
                    .ToList();
                ips = RandomSortList(ips).Take(num).ToList();
                ips.ForEach(p => p.Rooms.Add(room_Id));
                return ips;
            }
        }

        public static List<T> RandomSortList<T>(List<T> ListT)
        {
            Random random = new Random();
            List<T> newList = new List<T>();
            foreach (T item in ListT)
            {
                newList.Insert(random.Next(newList.Count + 1), item);
            }
            return newList;
        }

        /// <summary>
        /// 读取IP（房间号记录）
        /// </summary>
        /// <returns></returns>
        public static AgentIp GetAgentIp()
        {
            lock (objLock)
            {
                return _agentIps[new Random(Guid.NewGuid().GetHashCode()).Next(0, _agentIps.Count - 1)];
            }
        }

        public static void SaveIps()
        {
            JsonConfig<AgentIp>.CreateOrUpdateSiteConfig(_agentIps);
        }
    }
}
