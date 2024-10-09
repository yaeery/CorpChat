using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net.Sockets;  
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;
using System.Text.Json;

namespace CorpChat
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Dialog : ContentPage
    {
        public bool All_Message;
        public List<float> List_Size_Of_Framse;
        public  StackLayout _Stack_Layout;
        public ScrollView _Scroll_Hystory_Message;
        public TcpClient tcpClient_Work;
        public TcpClient tcpClient_Connection;
        public List<string> Messages;
        public Thread thread;
        public string FIO;
        public string ID;
        public int Count_of_Message;
        JsonClassToServer jsonClassToServer;
        JsonClassToClient jsonClassToClient;
        public Dialog()
        {
            InitializeComponent();
        }
        public Dialog(TcpClient tcpClient,string FIO,string ID)
        {
            Count_of_Message = 0;
            Messages = new List<string>();
            this.ID = ID;
            this.FIO = FIO;
            thread = new Thread(Load_Message);
            thread.Start();
            this.tcpClient_Work = tcpClient;
            InitializeComponent();
            _Stack_Layout = Stack_Layout;
            _Scroll_Hystory_Message = Scroll_Hystory_Message;
            FIO_Dialog.Text = FIO;
            List_Size_Of_Framse = new List<float>();
        }
        //void Decoding_String_to_List(string Input)
        //{
        //    string Prom = "";
        //    for (int j = 0; j < Input.Length; j++)
        //    {
        //        if (Input[j] == '#')
        //        {
        //            Messages.Add(Prom);
        //            Prom = "";
        //            continue;
        //        }
        //        else
        //        {
        //            Prom += Input[j];
        //        }
        //    }
        //    Messages.Add(Prom);
        //}
       async public void Create_Dialog(int End,bool Is_new)
        {
            double Size_Of_Framse = 0;
            int Start_for = 0;
            int End_for = 0;
            if(Is_new==false)
            {
                Start_for = 0;
                End_for = End;
            }
            else
            {
                Start_for = End;
                End_for = Messages.Count;
            }
            for (int i = Start_for; i < End_for; i+=3)
            {
                Grid grid = new Grid
                {
                    RowDefinitions =
                        {
                            new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) },
                            new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) }
                        },
                    ColumnDefinitions =
                        {
                            new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                             new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                        }
                };
                Label Msg = new Label()
                {
                    Text = Messages[i + 2],
                    HorizontalOptions = LayoutOptions.Fill,
                    VerticalOptions = LayoutOptions.Fill,
                    FontSize = 20
                };
                Label Date = new Label()
                {
                    Text = Messages[i + 1],
                    HorizontalTextAlignment = TextAlignment.End,
                    TextColor = Color.FromHex("#AFEEEE"),
                    HorizontalOptions = LayoutOptions.Fill,
                    VerticalOptions = LayoutOptions.Fill,
                    VerticalTextAlignment = TextAlignment.End,
                };

                Frame frame = new Frame()
                {
                    BackgroundColor = Color.Black,
                    CornerRadius = 20,
                    WidthRequest = 300
                };

                if (Messages[i] == "F")
                {
                  Msg.TextColor = Color.FromHex("#AFEEEE");
                  Date.TextColor = Color.FromHex("#AFEEEE");
                  frame.BorderColor = Color.FromHex("#AFEEEE");
                  frame.HorizontalOptions = LayoutOptions.End;
                }

                else if (Messages[i] == "S")
                {
                    Msg.TextColor = Color.FromHex("#008080");
                    Date.TextColor = Color.FromHex("#008080");
                    frame.BorderColor = Color.FromHex("#008080");
                    frame.HorizontalOptions = LayoutOptions.Start;
                }
                grid.Children.Add(Msg, 0, 0);
                Grid.SetColumnSpan(Msg, 2);
                grid.Children.Add(Date,1, 1);
                frame.Content = grid;
                Stack_Layout.Children.Insert(i/3,frame);
                Size_Of_Framse += frame.Height;
            }
            if (Count_of_Message == 0)
            {
                await Scroll_Hystory_Message.ScrollToAsync(0, Stack_Layout.Children.Count * 300, false);
            }
            else
            {
                await Scroll_Hystory_Message.ScrollToAsync(0, Size_Of_Framse, false);
            }
        }
        public void Load_Message()
        {
            try
            {
                jsonClassToServer = new JsonClassToServer("GH", Preferences.Get("ID", ""), ID, Count_of_Message.ToString());
                string Send_String = JsonSerializer.Serialize(jsonClassToServer);
                DialogsScroll.Is_No_Listen = true;
                NetworkStream networkStream = tcpClient_Work.GetStream();
                byte[] bytes = Encoding.UTF8.GetBytes(Send_String);//(Login.Text + "#" + Password.Text);
                networkStream.Write(bytes, 0, bytes.Length);
                networkStream.Flush();
                while (true)
                {
                    if (tcpClient_Work.Available > 0)
                    {
                        byte[] _bytes = new byte[32000];
                        NetworkStream networkStream1 = tcpClient_Work.GetStream();
                        int size = networkStream1.Read(_bytes, 0, _bytes.Length);
                        string x = Encoding.UTF8.GetString(_bytes, 0, size);
                        jsonClassToClient = JsonSerializer.Deserialize<JsonClassToClient>(Encoding.UTF8.GetString(_bytes, 0, size));
                        if ((jsonClassToClient.Message_Hystory_List==null) ||(jsonClassToClient.Message_Hystory_List.Count < 1))
                        {
                            All_Message = true;
                            break;
                        }
                        else
                        {
                            // Preferences.Set(ID, Msg); поправить !!!!!!!!!!!!!!
                            //Decoding_String_to_List(Msg);
                            //foreach (var i in jsonClassToClient.Message_Hystory_List)
                            //{
                            //    Messages.Add(i);
                            //}
                            for (int i = 0; i < jsonClassToClient.Message_Hystory_List.Count; i+=3)
                            {
                                Messages.Insert(0, jsonClassToClient.Message_Hystory_List[i]);
                                Messages.Insert(1, jsonClassToClient.Message_Hystory_List[i+1]);
                                Messages.Insert(2, jsonClassToClient.Message_Hystory_List[i+2]);
                            }
                            int Count_Msg_For_Build = jsonClassToClient.Message_Hystory_List.Count;
                            Device.BeginInvokeOnMainThread(() =>
                            {
                                Create_Dialog(Count_Msg_For_Build,false);
                            });
                            jsonClassToClient.Clear();
                            break;
                        }
                        networkStream1.Flush();
                    }
                }
            }
            catch(Exception E)
            {
                string x = E.Message.ToString();
                if(!((Preferences.Get(ID, "") == "") || (Preferences.Get(ID, "") == null)))
                {
                    //Decoding_String_to_List(Preferences.Get(ID, "")); поправить через Json
                    Device.BeginInvokeOnMainThread(() =>
                    {
                       // Create_Dialog(0);
                    });
                }
            }
        }

        async private void Return_Button_Clicked(object sender, EventArgs e)
        {
            DialogsScroll.Dialog_ID_Enter = "";
            await Navigation.PopModalAsync();
        }

        private void Send_Button_Clicked(object sender, EventArgs e)
        {
            Thread Sending = new Thread(Sending_Msg);
            Sending.Start();
        }

        void Sending_Msg()
        {
            if((Message_Entry.Text!=null)||(Message_Entry.Text!=""))
            {
                try
                {
                    DialogsScroll.Is_No_Listen = true;
                    jsonClassToServer = new JsonClassToServer("SM", Preferences.Get("ID", ""), ID, Message_Entry.Text.ToString());
                    string Send_String = JsonSerializer.Serialize(jsonClassToServer);
                    NetworkStream networkStream = tcpClient_Work.GetStream();
                    byte[] bytes = Encoding.UTF8.GetBytes(Send_String);//(Login.Text + "#" + Password.Text);
                    networkStream.Write(bytes, 0, bytes.Length);
                    networkStream.Flush();
                    while (true)
                    {
                        if (tcpClient_Work.Available > 0)
                        {
                            byte[] _bytes = new byte[32000];
                            NetworkStream networkStream1 = tcpClient_Work.GetStream();
                            int size = networkStream1.Read(_bytes, 0, _bytes.Length);
                            jsonClassToClient = JsonSerializer.Deserialize<JsonClassToClient>(Encoding.UTF8.GetString(_bytes, 0, size));
                            if ((jsonClassToClient.Message_Hystory_List == null) || (jsonClassToClient.Message_Hystory_List.Count < 1))
                            {
                                DisplayAlert("Ошибка", "Попробуйте позже", "Ок");
                                break;
                            }
                            else
                            {
                                //Preferences.Set(ID, Preferences.Get(ID, "") + "#F#" + DateTime.Now.ToString("") + "#" + Message_Entry.Text.ToString());
                                foreach (var i in jsonClassToClient.Message_Hystory_List)
                                {
                                    Messages.Add(i);
                                }
                                jsonClassToClient.Clear();
                                Device.BeginInvokeOnMainThread(() =>
                                {
                                    Create_Dialog(Messages.Count-3, true);
                                });
                                break;
                            }
                            networkStream1.Flush();
                        }
                    }
                }
                catch
                {
                }
                Device.BeginInvokeOnMainThread(() =>
                {
                    Message_Entry.Text = null;
                });
            }


        }

        private void Scroll_Hystory_Message_Scrolled(object sender, ScrolledEventArgs e)
        {
            if((All_Message==false)&&(Scroll_Hystory_Message.ScrollY==0))
            {
                Count_of_Message++;
                Load_Message();
            }
        }
    }
}