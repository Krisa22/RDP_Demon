using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Simple_RDP_Client.Test
{
    [TestClass]
    public class RDP_ClientTests
    {
        [TestMethod]
        public void IPtestMetod()
        {
            //arrange
            int i = 0;
            int combo = 1;
            string adr = "";
            string actual = "192.168.1.254:5001";
            //act
            IPHostEntry host1 = Dns.GetHostEntry(Dns.GetHostName());
            for (i = 131; i <= 254; i++)
            {
                Ping ping = new Ping();
                PingReply pingresult = ping.Send("192.168." + combo + "." + i, 200);
               
                    foreach (IPAddress ip in host1.AddressList)
                        adr = ip.ToString();
                    Byte[] SendBytes = Encoding.Default.GetBytes(adr);
                    IPEndPoint EndPoint = new IPEndPoint(IPAddress.Parse("192.168." + combo + "." + i), 5001);



                adr = Convert.ToString(EndPoint);
            }
            //assert
            Assert.AreEqual(actual, adr);
        }
    }
}
