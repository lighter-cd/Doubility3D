using System;
using System.Net.Sockets;
using System.Net;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.IO;

namespace Network
{
    internal enum DisType
    {
        Exception,
        Disconnect,
    }

    public class NetworkClient
    {
        private TcpClient client = null;
        private NetworkStream outStream = null;
        private MemoryStream memStream;
        private BinaryReader reader;

        private const int MAX_READ = 8192;
        private byte[] byteBuffer = new byte[MAX_READ];
        private string netName = "";
        //private NetHandler netHandler;

        public delegate void ConnectedCallBackFunc();//链接成功回调函数委托

        private NetworkClient.ConnectedCallBackFunc connectedCallBack;

        internal NetworkClient(string name_)
        {
            netName = name_;
            memStream = new MemoryStream();
            reader = new BinaryReader(memStream);
            //netHandler = new NetHandler();

            //Debug.Log(netName);
        }

        /// <summary>
        /// 移除
        /// </summary>
        internal void destroy()
        {
            this.Close();
            reader.Close();
            memStream.Close();
        }

        /*public void RegisterProtocol(Protocol.ProtocolBase protocol)
        {
            netHandler.RegisterProtocol(protocol);
            protocol.serverHandler = this;
        }*/
        
        /// <summary>
        /// 连接服务器
        /// </summary>
        public bool ConnectServer(string host, int port, NetworkClient.ConnectedCallBackFunc fun)
        {
            client = null;
            client = new TcpClient();
            client.SendTimeout = 1000;
            client.ReceiveTimeout = 1000;
            client.NoDelay = true;

            connectedCallBack = fun;
            try
            {
                client.BeginConnect(host, port, new AsyncCallback(OnConnect), null);
            }
            catch (Exception e)
            {
                Close(); 
                //Debugger.LogError(e.Message);
            }
            return true;
        }

        public bool IsConnected()
        {
            return (client!=null)?client.Connected:false;
        }
        /// <summary>
        /// 连接上服务器
        /// </summary>
        void OnConnect(IAsyncResult asr)
        {
            if (client.Connected)
            {
                outStream = client.GetStream();
                client.GetStream().BeginRead(byteBuffer, 0, MAX_READ, new AsyncCallback(OnRead), null);
                if (connectedCallBack != null)
                {
                    connectedCallBack();
                }
            }
            else
            {
                //Debugger.LogError("client.connected----->>false");
            }
        }

        /// <summary>
        /// 写数据
        /// </summary>
        private void WriteMessage(byte[] message)
        {
            MemoryStream ms = null;
            using (ms = new MemoryStream())
            {
                ms.Position = 0;
                BinaryWriter writer = new BinaryWriter(ms);
                ushort msglen = (ushort)(message.Length - 4);
                writer.Write(msglen);
                writer.Write(message);
                writer.Flush();
                if (client != null && client.Connected)
                {
                    //NetworkStream stream = client.GetStream(); 
                    byte[] payload = ms.ToArray();
                    outStream.BeginWrite(payload, 0, payload.Length, new AsyncCallback(OnWrite), null);
                }
                else
                {
                    //Debugger.LogError("client.connected----->>false");
                }
            }
        }

        /// <summary>
        /// 读取消息
        /// </summary>
        void OnRead(IAsyncResult asr)
        {
            int bytesRead = 0;
            try
            {
                lock (client.GetStream())
                {         //读取字节流到缓冲区
                    bytesRead = client.GetStream().EndRead(asr);
                }
                if (bytesRead < 1)
                {
                    //服务器主动断开链接
                    OnDisconnected(DisType.Disconnect, "bytesRead < 1");
                    return;
                }
                OnReceive(byteBuffer, bytesRead);   //分析数据包内容，抛给逻辑层
                lock (client.GetStream())
                {         //分析完，再次监听服务器发过来的新消息
                    Array.Clear(byteBuffer, 0, byteBuffer.Length);   //清空数组
                    client.GetStream().BeginRead(byteBuffer, 0, MAX_READ, new AsyncCallback(OnRead), null);
                }
            }
            catch (Exception ex)
            {
                //数据流异常，断开连接
                OnDisconnected(DisType.Exception, ex.Message);
            }
        }

        /// <summary>
        /// 丢失链接
        /// </summary>
        void OnDisconnected(DisType dis, string msg)
        {
            //关掉客户端链接
            Close();   
            //消息发出
            //EventProc.fireOut(NotifyEvent.Net_Disconnect, new object[] { netName, msg });
        }

        /// <summary>
        /// 打印字节
        /// </summary>
        /// <param name="bytes"></param>
        void PrintBytes()
        {
            string returnStr = string.Empty;
            for (int i = 0; i < byteBuffer.Length; i++)
            {
                returnStr += byteBuffer[i].ToString("X2");
            }
            //Debugger.LogWarning(returnStr);
        }

        /// <summary>
        /// 向链接写入数据流
        /// </summary>
        private void OnWrite(IAsyncResult r)
        {
            try
            {
                outStream.EndWrite(r);
            }
            catch (Exception ex)
            {
                //Debugger.LogError("OnWrite--->>>" + ex.Message);
            }
        }

        /// <summary>
        /// 接收到消息
        /// </summary>
        void OnReceive(byte[] bytes, int length)
        {
            memStream.Seek(0, SeekOrigin.End);
            memStream.Write(bytes, 0, length);
            //Reset to beginning
            memStream.Seek(0, SeekOrigin.Begin);
            while (RemainingBytes() > /*Protocol.DataPacket.PKG_HEAD*/16)
            {
                //读出包长
                ushort messageLen =  reader.ReadUInt16();
                uint protUInt32ID = 0;
                //Debug.Log("##0 messageLen: " + messageLen);
                if (RemainingBytes() >= messageLen + 4)
                {
                    OnReceivedMessage(protUInt32ID, reader.ReadBytes(messageLen));
                }
                else
                {
                    //Back up the position two bytes
                    memStream.Position = memStream.Position - 2;
                    break;
                }
            }
            //Create a new stream with any leftover bytes
            byte[] leftover = reader.ReadBytes((int)RemainingBytes());
            memStream.SetLength(0);     //Clear
            memStream.Write(leftover, 0, leftover.Length);
        }

        /// <summary>
        /// 剩余的字节
        /// </summary>
        private long RemainingBytes()
        {
            return memStream.Length - memStream.Position;
        }

        /// <summary>
        /// 接收到消息
        /// </summary>
        /// <param name="protID">协议ID</param>
        /// <param name="ms">数据包信息</param>
        void OnReceivedMessage(uint protID, byte[] message)
        {
           try
           {
                /*Protocol.ByteBuffer buffer = new Protocol.ByteBuffer(message);
                Protocol.DataPacket packet = new Protocol.DataPacket(buffer);
                packet.protUInt32Id = protID;
                netHandler.Process(packet);*/
            }
            catch (Exception ex)
            {
                uint _protId = (protID & 0x00ff00) >> 8;
                uint _funcId = (protID & 0xff000000) >> 24;

                //Debugger.LogError("OnReceivedMessage--->>>协议ID:" + protID + "  错误信息：" + ex.Message + " protID:" + _protId + " functionID:" + _funcId);
            }
        }


        /// <summary>
        /// 会话发送
        /// </summary>
        private void SessionSend(byte[] bytes)
        {
            WriteMessage(bytes);
        }

        /// <summary>
        /// 关闭链接
        /// </summary>
        public void Close()
        {
            if (client != null)
            {
                //Debugger.Log("NetworkClient::Close()...");
                if (client.Connected) client.Close();
                client = null;
            }
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        public bool SendMessage(byte[] buffer)
        {
            if (IsConnected() == false)
            {
                //TODO 理应不需要
                OnDisconnected(DisType.Disconnect,"");
                return false;
            }
            SessionSend(buffer);
            return true;
        }

    }
}
