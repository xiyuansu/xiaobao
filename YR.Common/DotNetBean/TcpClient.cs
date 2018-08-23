using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Web;

namespace YR.Common.DotNetBean
{
    public class TcpClient
    {
        private Socket socket = null;

        private System.Threading.Thread thread = null;

        public event ReceiveDataHandler receiveEvent;

        public int ReceiveTimeout
        {
            get;
            set;
        }

        public int SendTimeout
        {
            get;
            set;
        }

        public TcpClient()
        {
            //设置读写超时间为30秒
            ReceiveTimeout = 30000;
            SendTimeout = 30000;
        }

        public bool Connect(string ip, int port)
        {
            bool result = false;
            try
            {
                if (socket == null)
                    socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                if (socket.Connected)
                    socket.Disconnect(true);
                socket.Connect(ip, port);
                socket.SendTimeout = SendTimeout;
                socket.ReceiveTimeout = ReceiveTimeout;
                if (receiveEvent != null)
                {
                    if (thread == null)
                        thread = new System.Threading.Thread(new System.Threading.ThreadStart(HandleReceiveData));
                    thread.Start();
                }

                result = true;
                return result;
            }
            catch
            {
                return result;
            }

        }

        public bool Send(byte[] bs)
        {
            try
            {
                socket.Send(bs);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Send(string msg)
        {
            try
            {
                byte[] bs = System.Text.Encoding.UTF8.GetBytes(msg);
                return this.Send(bs);
            }
            catch
            {
                return false;
            }
        }

        public bool Send(object obj)
        {
            try
            {
                string json = JsonConvert.SerializeObject(obj);
                return this.Send(json);
            }
            catch
            {
                return false;
            }
        }

        public byte[] Receive()
        {
            try
            {
                byte[] bs = new byte[1024];
                int len = socket.Receive(bs);
                byte[] bs2 = new byte[len];
                Array.Copy(bs, bs2, len);
                return bs2;
            }
            catch
            {
                return null;
            }
        }

        public string ReceiveString()
        {
            try
            {
                byte[] bs = Receive();
                return System.Text.Encoding.UTF8.GetString(bs);
            }
            catch
            {
                return null;
            }
        }

        public object ReceiveJson()
        {
            try
            {
                string json = ReceiveString();
                return JsonConvert.DeserializeObject(json);
            }
            catch
            {
                return null;
            }
        }

        public void HandleReceiveData()
        {
            while (true)
            {
                byte[] bs = Receive();
                if (receiveEvent != null)
                {
                    receiveEvent(this, new ReceiveDataEventArgs(bs));
                }
            }
        }

        public void Disconnect()
        {
            if (socket.Connected)
            {
                socket.Shutdown(SocketShutdown.Both);
                //socket.Disconnect(true);
                socket.Disconnect(false);
                socket.Close();
                socket.Dispose();
                socket = null;
            }
            if (thread != null)
            {
                thread.Abort();
                thread = null;
            }
        }



        /// <summary>
        /// 发送登录命令
        /// </summary>
        /// <param name="cmd">数据包长度</param>
        /// <param name="info">信息序列号</param>
        public byte[] SetLogin(int info)
        {
            byte[] logincmd = new byte[7];

            //命令头
            logincmd[0] = Convert.ToByte(Convert.ToInt32("67", 16));
            logincmd[1] = Convert.ToByte(Convert.ToInt32("67", 16));

            //登录协议号
            logincmd[2] = Convert.ToByte(Convert.ToInt32("1", 16));

            //数据包长度,固定为2
            string strlen = Convert.ToInt32("2", 16).ToString("X4");

            byte[] bytelen = strToToHexByte(strlen);

            //信息序列号
            string strinfo = info.ToString("X4");
            byte[] byteinfo = strToToHexByte(strinfo);

            Array.Copy(bytelen, 0, logincmd, 3, 2);
            Array.Copy(byteinfo, 0, logincmd, 5, 2);

            return logincmd;
        }

        /// <summary>
        /// 响应心跳数据包
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public byte[] SetHeartbeat(int info)
        {
            byte[] logincmd = new byte[7];

            //命令头
            logincmd[0] = Convert.ToByte(Convert.ToInt32("67", 16));
            logincmd[1] = Convert.ToByte(Convert.ToInt32("67", 16));

            //登录协议号
            logincmd[2] = Convert.ToByte(Convert.ToInt32("3", 16));

            //数据包长度,固定为2
            string strlen = Convert.ToInt32("2", 16).ToString("X4");

            byte[] bytelen = strToToHexByte(strlen);

            //信息序列号
            string strinfo = info.ToString("X4");
            byte[] byteinfo = strToToHexByte(strinfo);

            Array.Copy(bytelen, 0, logincmd, 3, 2);
            Array.Copy(byteinfo, 0, logincmd, 5, 2);

            return logincmd;
        }


        /// <summary>
        /// 获取IMIE号
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public string GetIMIE(byte[] data)
        {
            string imie = null;
            for (int i = 0; i < data.Length; i++)
            {
                imie = imie + Convert.ToInt32(BitConverter.ToString(data, i, 1)).ToString();

                //imie = imie + Int32.Parse(data[i].ToString(), System.Globalization.NumberStyles.HexNumber);
            }

            return imie;
        }


        #region 进制转换
        /// <summary>
        /// 十六进制换算为十进制
        /// </summary>
        /// <param name="strColorValue"></param>
        /// <returns></returns>
        public int GetHexadecimalValue(String strColorValue)
        {
            char[] nums = strColorValue.ToCharArray();
            int total = 0;
            try
            {
                for (int i = 0; i < nums.Length; i++)
                {
                    String strNum = nums[i].ToString().ToUpper();
                    switch (strNum)
                    {
                        case "A":
                            strNum = "10";
                            break;
                        case "B":
                            strNum = "11";
                            break;
                        case "C":
                            strNum = "12";
                            break;
                        case "D":
                            strNum = "13";
                            break;
                        case "E":
                            strNum = "14";
                            break;
                        case "F":
                            strNum = "15";
                            break;
                        default:
                            break;
                    }
                    double power = Math.Pow(16, Convert.ToDouble(nums.Length - i - 1));
                    total += Convert.ToInt32(strNum) * Convert.ToInt32(power);
                }

            }
            catch (System.Exception ex)
            {
                String strErorr = ex.ToString();
                return 0;
            }

            return total;
        }


        /// <summary>   
        /// 16进制字符串转字节数组   
        /// </summary>   
        /// <param name="hexString"></param>   
        /// <returns></returns>   
        public byte[] strToToHexByte(string hexString)
        {
            hexString = hexString.Replace(" ", "");
            if ((hexString.Length % 2) != 0)
                hexString += " ";
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2).Trim(), 16);


            return returnBytes;

        }


        public byte[] StringToBytes(string hexString)
        {

            hexString = hexString.Replace(" ", "");
            if ((hexString.Length % 2) != 0)
                hexString = "0" + hexString;
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
            {
                returnBytes[i] = Convert.ToByte(Convert.ToInt32(hexString.Substring(i * 2, 2), 16));
            }

            return returnBytes;

        }


        /// <summary>   
        /// 字节数组转16进制字符串   
        /// </summary>   
        /// <param name="bytes"></param>   
        /// <returns></returns>   
        public string byteToHexStr(byte[] bytes)
        {
            string returnStr = "";
            if (bytes != null)
            {
                for (int i = 0; i < bytes.Length; i++)
                {
                    returnStr += bytes[i].ToString("X2");
                }
            }
            return returnStr;
        }
        #endregion
    }

    public class ReceiveDataEventArgs : EventArgs
    {
        private byte[] data;

        public ReceiveDataEventArgs(byte[] bs)
        {
            data = bs;
        }

        public byte[] Data
        {
            get
            {
                return data;
            }
        }
    }

    public delegate void ReceiveDataHandler(object sender, ReceiveDataEventArgs e);

}
