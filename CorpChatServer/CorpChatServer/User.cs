using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Text.Json;

namespace CorpChatServer
{
    class User
    {
        public TcpListener listner_Connection { get; }
        public TcpListener listner_Work { set; get; }
        public TcpClient client_Connection { get; set; }
        public TcpClient client_Work { get; set; }
        private bool Is_Connect = true;
       // private bool Is_Sending;
        //private string Msg;
        public JsonClassToServer jsonClassToServer { set; get; }
        public JsonClassToClient jsonClassToClient { set; get; }
        private string ID { get; set; }

        public User(TcpListener listner, TcpClient client)
        {
            this.client_Connection = client;
            this.listner_Connection = listner;
        }
        public string Create_Json()
        {
            return JsonSerializer.Serialize(jsonClassToClient);
        }
        public void Dcoding_Json(string Input)
        {
            jsonClassToServer = JsonSerializer.Deserialize<JsonClassToServer>(Input);
        }
        //public void Send_Msg(string Msg)
        //{
        //   // Is_Sending = true;
        //    this.Msg = Msg;
        //}
        public string Get_Msg()
        {
            byte[] bytes = new byte[32000];
            NetworkStream networkStream = client_Work.GetStream();
            int size = networkStream.Read(bytes, 0, bytes.Length);
            return Encoding.UTF8.GetString(bytes, 0, size);
        }
        public bool Get_Is_Connect()
        {
            return Is_Connect;
        }
        public string Get_ID()
        {
            return ID;
        }
        public void Set_listner_Client_Work(TcpListener tcpListener,TcpClient tcpClient)
        {
            this.client_Work = tcpClient;
            this.listner_Work = tcpListener;
        }
        public void Set_ID(string ID)
        {
            
            this.ID = ID;
        }
        public void Send_Msg(string Msg)
        {
            try
            {
                byte[] message = Encoding.UTF8.GetBytes(Msg);
                NetworkStream networkStream = client_Work.GetStream();
                networkStream.Write(message, 0, message.Length);
                networkStream.Flush();
            }
            catch
            {
                Is_Connect = false;
            }
        }
        public void Send_Proverka()
        {
            try
            {
                //if (Is_Sending == true)
                //{
                //    byte[] message = Encoding.UTF8.GetBytes(Msg);
                //    NetworkStream networkStream = client.GetStream();
                //    networkStream.Write(message, 0, message.Length);
                //    networkStream.Flush();
                //    Is_Sending = false;
                //    Msg = "";
                //}
                //else
                {
                    byte[] message = Encoding.UTF8.GetBytes("~");
                    NetworkStream networkStream = client_Connection.GetStream();
                    networkStream.Write(message, 0, message.Length);
                    networkStream.Flush();
                }
            }
            catch
            {
                Is_Connect = false;
            }

        }
    }
}