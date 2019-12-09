using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace DouyuAgreement
{
    internal class SocketHelper
    {

        /// <summary>
        /// 建立Tcp连接
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public static Socket InitTcp(string host, int port)
        {
            bool flag = IPAddress.TryParse(host, out IPAddress hostadd);
            if (!flag)
            {
                IPHostEntry hostInfo = Dns.GetHostEntry(host);
                hostadd = hostInfo.AddressList[0]; //域名转IP
            }
            IPEndPoint ipe = new IPEndPoint(hostadd, port);
            Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            s.Connect(ipe);
            return s;
        }
    }
}
