using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.IO;
using System.Data.SqlClient;
using System.Text.Json;

namespace CorpChatServer
{
    class Hosting
    {
        static bool Is_Work_Now = true;
        IPEndPoint ipPoint_first_Chat_from_Connect;
        IPEndPoint ipPoint_first_Chat_from_Work;
        //Dictionary<User,TcpClient> Users;
        List<User> list__first_Chat;
        Thread thread_listning_Connect;
        Thread thread_listning_Work;
        string Msg;
        TcpListener tcpListener_Connect;
        TcpListener tcpListener_Work;
        BD bd;
        bool Udalenie = false;

        public Hosting(int Port)
        {
            ipPoint_first_Chat_from_Connect = new IPEndPoint(IPAddress.Parse("192.168.0.107"), Port);
            ipPoint_first_Chat_from_Work = new IPEndPoint(IPAddress.Parse("192.168.0.107"), Port+1);
            tcpListener_Connect = new TcpListener(ipPoint_first_Chat_from_Connect);
            tcpListener_Work = new TcpListener(ipPoint_first_Chat_from_Work);
            list__first_Chat = new List<User>();
            //thread_listning_Connect = new Thread(Listning_Connection);
            thread_listning_Connect = new Thread(Listen_Connect);
            thread_listning_Work = new Thread(Listning_Work);
            
            Msg = "";
            bd = new BD(@"data source=HPLAPTOP\SQLEXPRESS; Initial Catalog=Chat;Integrated Security = True;");
            bd.Open_Conection();
        }
        public void ReceiveCallback_Connection(IAsyncResult AsyncCall)
        {
            TcpClient tcpClient = ((TcpListener)AsyncCall.AsyncState).EndAcceptTcpClient(AsyncCall);
            list__first_Chat.Add(new CorpChatServer.User((TcpListener)AsyncCall.AsyncState, tcpClient));
            ((TcpListener)AsyncCall.AsyncState).BeginAcceptTcpClient(new AsyncCallback(ReceiveCallback_Connection), ((TcpListener)AsyncCall.AsyncState));
        }
        public void Delete()
        {
            Is_Work_Now = false;
            ipPoint_first_Chat_from_Connect = null;
            ipPoint_first_Chat_from_Work = null;
            tcpListener_Connect.Stop();
            tcpListener_Work.Stop();
            tcpListener_Connect = null;
            tcpListener_Work = null;
        }
        public void ReceiveCallback_Work(IAsyncResult AsyncCall)
        {
            TcpClient tcpClient = ((TcpListener)AsyncCall.AsyncState).EndAcceptTcpClient(AsyncCall);
            IPEndPoint work_point = tcpClient.Client.RemoteEndPoint as IPEndPoint;
            IPEndPoint conn_point;
            for (int i = 0; i < list__first_Chat.Count; i++)
            {
                conn_point = list__first_Chat[i].client_Connection.Client.RemoteEndPoint as IPEndPoint;
                if (work_point.Address.ToString() == conn_point.Address.ToString())
                {
                    list__first_Chat[i].Set_listner_Client_Work(tcpListener_Work, tcpClient);
                }
            }
            //list__first_Chat.Add(new CorpChatServer.User((TcpListener)AsyncCall.AsyncState, tcpClient));
            ((TcpListener)AsyncCall.AsyncState).BeginAcceptTcpClient(new AsyncCallback(ReceiveCallback_Work), ((TcpListener)AsyncCall.AsyncState));
        }

        public void Listen_Connect()
        {
            try
            {
                while (Is_Work_Now)
                {
                    Console.WriteLine("Подключено " + list__first_Chat.Count.ToString() + " пользователей к " + ipPoint_first_Chat_from_Connect.Port);
                    for (int i = 0; i < list__first_Chat.Count; i++)
                    {
                        list__first_Chat[i].Send_Proverka();
                        if (list__first_Chat[i].Get_Is_Connect() == false)
                        {
                            Udalenie = true;
                            list__first_Chat.Remove(list__first_Chat[i]);
                            i--;
                            Console.WriteLine("Пользователь отключился от " + ipPoint_first_Chat_from_Connect.Port);
                            Console.WriteLine("Подключено " + list__first_Chat.Count.ToString() + " пользователей к " + ipPoint_first_Chat_from_Connect.Port);
                            Udalenie = false;
                        }
                    }
                    Thread.Sleep(1000);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Ошибка");
                Console.WriteLine(e.Message);
            }
        }
        public void Main_Vhod()
        {
            try
            { 
                tcpListener_Connect.Start();
                thread_listning_Connect.Start();
                tcpListener_Work.Start();
                thread_listning_Work.Start();
                Console.WriteLine("Сервер " + ipPoint_first_Chat_from_Connect.Port + " запущен");
                //thread_listning_Connect.Start();
                tcpListener_Connect.BeginAcceptTcpClient(new AsyncCallback(ReceiveCallback_Connection), tcpListener_Connect);//сокет полключения
                tcpListener_Work.BeginAcceptTcpClient(new AsyncCallback(ReceiveCallback_Work), tcpListener_Work);
                
            }

            catch (Exception e)
            {
                Console.WriteLine("Ошибка");
                Console.WriteLine(e.Message);
            }
        }
        void Listning_Work()
        {
            while (Is_Work_Now)
            {
                try
                {
       
                    for (int i = 0; i < list__first_Chat.Count; i++)
                    {
                        if (Udalenie == true)
                        {
                            Console.WriteLine("Прослушивание приостановлено");
                            Thread.Sleep(500);
                        }
                        if ((list__first_Chat[i].client_Work != null))
                        {
                            if (list__first_Chat[i].client_Work.Available > 0)
                            {
                                Msg = list__first_Chat[i].Get_Msg();
                                Console.WriteLine("От клиента: "+Msg);
                                list__first_Chat[i].Dcoding_Json(Msg);
                                string Result_For_Send = "";
                                switch (list__first_Chat[i].jsonClassToServer.Instraction)
                                {
                                    case "GI":
                                        list__first_Chat[i].jsonClassToClient = new JsonClassToClient("GI",
                                            bd.Get_Result_Access(list__first_Chat[i].jsonClassToServer.Login, list__first_Chat[i].jsonClassToServer.Password));
                                        Result_For_Send = list__first_Chat[i].Create_Json();
                                        break;
                                    case "GD/SI":
                                        list__first_Chat[i].Set_ID(list__first_Chat[i].jsonClassToServer.Sender_ID);
                                        list__first_Chat[i].jsonClassToClient = new JsonClassToClient();
                                        list__first_Chat[i].jsonClassToClient.Instraction = "GD";
                                        list__first_Chat[i].jsonClassToClient.FIO_ID_for_All_Dialogs = new List<string>();
                                        bd.Get_Dilogs(list__first_Chat[i].jsonClassToServer.Sender_ID, list__first_Chat[i].jsonClassToClient.FIO_ID_for_All_Dialogs);
                                        Result_For_Send = list__first_Chat[i].Create_Json();
                                        break;
                                    case "GH":
                                        list__first_Chat[i].jsonClassToClient = new JsonClassToClient();
                                        list__first_Chat[i].jsonClassToClient.Instraction = "GH";
                                        list__first_Chat[i].jsonClassToClient.Message_Hystory_List = new List<string>();
                                        bd.Get_Hystory_Message(list__first_Chat[i].jsonClassToServer.Sender_ID, list__first_Chat[i].jsonClassToServer.ID_Contact, list__first_Chat[i].jsonClassToServer.Count_of_Massege_for_load, list__first_Chat[i].jsonClassToClient.Message_Hystory_List);
                                        Result_For_Send = list__first_Chat[i].Create_Json();
                                        break;
                                    case "FC":
                                        list__first_Chat[i].jsonClassToClient = new JsonClassToClient();
                                        list__first_Chat[i].jsonClassToClient.Instraction = "FC";
                                        list__first_Chat[i].jsonClassToClient.FIO_ID_for_All_Dialogs = new List<string>();
                                        bd.Get_Find_Dilog(list__first_Chat[i].jsonClassToServer.Sender_ID, list__first_Chat[i].jsonClassToServer.Param_Find, list__first_Chat[i].jsonClassToClient.FIO_ID_for_All_Dialogs);
                                        Result_For_Send = list__first_Chat[i].Create_Json();
                                        break;
                                    case "SM":
                                        list__first_Chat[i].jsonClassToClient = new JsonClassToClient();
                                        list__first_Chat[i].jsonClassToClient.ID_Sender = list__first_Chat[i].jsonClassToServer.Sender_ID;
                                        list__first_Chat[i].jsonClassToClient.Message_Hystory_List = new List<string>();
                                        list__first_Chat[i].jsonClassToClient.Instraction = bd.Insert_Messages(list__first_Chat[i].jsonClassToServer.Sender_ID, list__first_Chat[i].jsonClassToServer.ID_Contact, list__first_Chat[i].jsonClassToServer.Message, list__first_Chat[i].jsonClassToClient.Message_Hystory_List);                
                                        Result_For_Send = list__first_Chat[i].Create_Json();
                                        break;
                                }
                                Console.WriteLine("Клиенту: "+ Result_For_Send);
                                list__first_Chat[i].Send_Msg(Result_For_Send);
                                if (list__first_Chat[i].jsonClassToClient.Instraction== "SM/S")
                                {
                                    for (int j = 0; j < list__first_Chat.Count; j++)
                                    {
                                        if (list__first_Chat[j].Get_ID() == list__first_Chat[i].jsonClassToServer.ID_Contact)
                                        {
                                            list__first_Chat[i].jsonClassToClient.Message_Hystory_List[0] = "S";
                                            list__first_Chat[j].Send_Msg(list__first_Chat[i].Create_Json());
                                            Console.WriteLine("Клиенту: " + list__first_Chat[i].Create_Json());
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Ошибка");
                    Console.WriteLine(e.Message);
                    continue;
                }
            }
        }
    }
}
/*
 Коды:
    GI - Получение ID
    GD/SI - Получение диалогов по ID, установка ID
    GH - Получение истории сообщений
    FC - поиск контакта 
    SM - Отправка сообщения 
*/