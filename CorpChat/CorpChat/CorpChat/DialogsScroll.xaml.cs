using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using System.Net.Sockets;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Threading;
using Android.App;
using Android;
using Android.Content;
using Android.OS;
using System.Windows;
using System.Text.Json;

namespace CorpChat
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DialogsScroll : ContentPage
    {
        public static int Count_Jump_Page;
        public static bool Is_No_Listen;
        public int Count_Try_Conect;
        public TcpClient tcpClient_Work;
        public TcpClient tcpClient_Connection;
        public Thread thread;
        public static Thread Listning;
        public Thread try_Connect;
        public List<string> Name_of_Dilogs_List;
        public Dictionary<Button, string> Key_Pair_Button_ID;
        public Dictionary<string, Button> Key_Pair_ID_Button;
        public Dictionary<string, string> Key_Pair_ID_FIO;
        public static string Dialog_ID_Enter;
        public Dialog dialog;

        JsonClassToServer jsonClassToServer;
        JsonClassToClient jsonClassToClient;
        public DialogsScroll()
        {
            InitializeComponent();
            tcpClient_Connection = new TcpClient(MainPage.IP, 33023);
            Count_Try_Conect = 0;
            //tcpClient = new TcpClient();
            //try_Connect = new Thread(Try_Connect);
            //try_Connect.Start();
            Key_Pair_Button_ID = new Dictionary<Button, string>();
            Key_Pair_ID_Button = new Dictionary<string, Button>();
            Key_Pair_ID_FIO = new Dictionary<string, string>();
            Name_of_Dilogs_List = new List<string>();
            tcpClient_Work = new TcpClient(MainPage.IP, 33024);

        }
        protected override void OnAppearing()
        {
            Count_Jump_Page = 0;
            thread = new Thread(Load_Data);
            thread.Start();
            Listning = new Thread(Listning_Messages);
            Listning.Start();
        }


        public DialogsScroll(TcpClient tcpClient_Work,TcpClient tcpClient_Connection)
        {
            InitializeComponent();
            Key_Pair_Button_ID = new Dictionary<Button, string>();
            Key_Pair_ID_Button = new Dictionary<string, Button>();
            Key_Pair_ID_FIO = new Dictionary<string, string>();
            Name_of_Dilogs_List = new List<string>();
            this.tcpClient_Connection = tcpClient_Connection;
            this.tcpClient_Work = tcpClient_Work;
            thread = new Thread(Load_Data);
            thread.Start();
        }
        
        async private void LogOut_Clicked(object sender, EventArgs e)
        {
            bool result = await DisplayAlert("Внимание!", "Вы точно хотите выйти из аккаунта?", "Да", "Нет");
            if (result == true)
            {
                Preferences.Clear();
                await Navigation.PopModalAsync();
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        //void Decoding_String_to_List(string Input,List<string> Name_of_Dilogs_List) не нужно!
        //{
        //    string Prom = "";
        //    for (int j = 0; j < Input.Length; j++)
        //    {
        //        if (Input[j] == '#')
        //        {
        //            Name_of_Dilogs_List.Add(Prom);
        //            Prom = "";
        //            continue;
        //        }
        //        else
        //        {
        //            Prom += Input[j];
        //        }
        //    }
        //    Name_of_Dilogs_List.Add(Prom);
        //}
        void Create_Button_Name()
        {
            for (int i = 0; i < Name_of_Dilogs_List.Count; i += 2)
            {

                Button button = new Button()
                {
                    BackgroundColor = Color.Black,
                    Text = Name_of_Dilogs_List[i],
                    TextColor = Color.FromHex("#AFEEEE"),
                    BorderWidth = 2,
                    BorderColor = Color.FromHex("#AFEEEE"),
                    CornerRadius = 20,
                    FontSize = 20,
                    FontAttributes = FontAttributes.Bold,
                    HeightRequest = 80
                };
                button.Clicked += Load_Dilog;
                try
                {
                    Key_Pair_ID_FIO.Add(Name_of_Dilogs_List[i + 1], Name_of_Dilogs_List[i]);
                    Key_Pair_Button_ID.Add(button, Name_of_Dilogs_List[i + 1]);
                    Key_Pair_ID_Button.Add(Name_of_Dilogs_List[i + 1], button);
                    Scroll_Layout_Dialogs.Children.Add(button);
                }
                catch { }
            }
        }
        //////////////////////////////////////////////////////////////////////////////////
        async void Load_Dilog(object sender, EventArgs e)
        {
            ((Button)sender).BackgroundColor = Color.Black;
            dialog = new Dialog(tcpClient_Work, Key_Pair_ID_FIO[Key_Pair_Button_ID[(Button)sender]], Key_Pair_Button_ID[(Button)sender]);
            Dialog_ID_Enter = Key_Pair_Button_ID[(Button)sender];
            await Navigation.PushModalAsync(dialog);
        }
        //async void Try_Connect()
        //{
        //    while (true)
        //    {
        //        if (!(tcpClient.Available > 0))
        //        {
        //            while (true)
        //            {
        //                try
        //                {
        //                    //  tcpClient = new TcpClient(MainPage.IP, 33023);
        //                    await tcpClient.ConnectAsync(MainPage.IP, 33023);
        //                    if (tcpClient.Available > 0)
        //                    {
        //                        Name_of_Dilogs_List.Clear();
        //                        Key_Pair_ID_FIO.Clear();
        //                        Key_Pair_Button_ID.Clear();
        //                        Key_Pair_ID_Button.Clear();
        //                        Device.BeginInvokeOnMainThread(() =>
        //                        {
        //                            Scroll_Layout_Dialogs.Children.Clear();
        //                        });
        //                        Load_Data();
        //                        break;
        //                    }
        //                }
        //                catch
        //                { }
        //                Thread.Sleep(2000);
        //            }
        //        }
        //    }
        //}
        void Load_Data()
        {
            try
            {
                Is_No_Listen = true;
                jsonClassToServer = new JsonClassToServer("GD/SI", Preferences.Get("ID", ""));
                string Send_String = JsonSerializer.Serialize(jsonClassToServer);
                NetworkStream networkStream = tcpClient_Work.GetStream();
                byte[] bytes = Encoding.UTF8.GetBytes(Send_String);
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
                        if ((jsonClassToClient.FIO_ID_for_All_Dialogs==null)||(jsonClassToClient.FIO_ID_for_All_Dialogs.Count<1))
                        {
                            break;
                        }
                        else
                        {
                            //Preferences.Set("Dialogs", Msg);                                //потом исправить очень важно!!!!!
                            //Name_of_Dilogs_List = jsonClassToClient.FIO_ID_for_All_Dialogs;
                            Name_of_Dilogs_List.Clear();
                            foreach (var i in jsonClassToClient.FIO_ID_for_All_Dialogs)
                            {
                                Name_of_Dilogs_List.Add(i);
                            }
                            jsonClassToClient.Clear();
                            //Decoding_String_to_List(Msg, Name_of_Dilogs_List);
                            Device.BeginInvokeOnMainThread(() =>
                            {
                                Create_Button_Name();
                            });
                            break;
                        }
                        networkStream1.Flush();
                    }
                }
            }
            catch (Exception e)
            {
                string x = e.Message.ToString();
                if (Count_Try_Conect != 0)
                {
                    DisplayAlert("Ошибка", "Сервер прилег, поднимаем", "Ок");//на том месте
                }
               // Decoding_String_to_List(Preferences.Get("Dialogs", ""), Name_of_Dilogs_List);Исправить
                Device.BeginInvokeOnMainThread(() =>
                {
                    Create_Button_Name();
                });
                Count_Try_Conect++;
            }
            thread.Abort();
        }

        async private void Find_Button_Clicked(object sender, EventArgs e)
        {
            string Result = await DisplayPromptAsync("Поиск", "", "Ok", "", "", 40, Keyboard.Chat, "");
            await Navigation.PushModalAsync(new FindPage(tcpClient_Work,tcpClient_Connection, Preferences.Get("ID", ""), Result));
            Count_Jump_Page++;
        }

        void Listning_Messages()
        {
            try
            {
                NetworkStream networkStream = tcpClient_Work.GetStream();
                networkStream.Flush();
                while (true)
                {
                    if (Is_No_Listen == true)
                    {
                        Thread.Sleep(5000);
                        Is_No_Listen = false;
                    }
                    if (tcpClient_Work.Available > 0)
                    {
                        byte[] _bytes = new byte[32000];
                        NetworkStream networkStream1 = tcpClient_Work.GetStream();
                        int size = networkStream1.Read(_bytes, 0, _bytes.Length);
                        jsonClassToClient = JsonSerializer.Deserialize<JsonClassToClient>(Encoding.UTF8.GetString(_bytes, 0, size));
                        if (jsonClassToClient.Instraction == "SM/S")
                        {
                            Vibration.Vibrate(2000);
                            List<string> Prom = new List<string>();
                            Prom.Clear();
                            Prom.Add(jsonClassToClient.ID_Sender);
                            foreach (var i in jsonClassToClient.Message_Hystory_List)
                            {
                                Prom.Add(i);
                            }
                            jsonClassToClient.Clear();

                            Device.BeginInvokeOnMainThread(() =>
                            {
                                if (Scroll_Layout_Dialogs.Children.Count > 1)
                                {
                                    Scroll_Layout_Dialogs.Children.Remove(Key_Pair_ID_Button[Prom[0]]);
                                    (Key_Pair_ID_Button[Prom[0]]).BackgroundColor = Color.FromHex("#008080");
                                    Scroll_Layout_Dialogs.Children.Insert(0, Key_Pair_ID_Button[Prom[0]]);
                                }
                                else
                                {
                                    (Key_Pair_ID_Button[Prom[0]]).BackgroundColor = Color.FromHex("#008080");
                                }
                            });
                            if (Dialog_ID_Enter == Prom[0])
                            {
                                Device.BeginInvokeOnMainThread(() =>
                                {
                                    (Key_Pair_ID_Button[Prom[0]]).BackgroundColor = Color.FromHex("#000000");
                                    Load_message_On_Dialog(Prom);
                                });
                            }
                        }
                        networkStream1.Flush();
                    }
                }
            }
            catch (Exception e)
            {
                string x = e.Message.ToString();
            }
        }
        async void Load_message_On_Dialog(List<string> Prom_Message)
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
                Text = Prom_Message[3],
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill,
                FontSize = 20
            };
            Label Date = new Label()
            {
                Text = Prom_Message[2],
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
            Msg.TextColor = Color.FromHex("#008080");
            Date.TextColor = Color.FromHex("#008080");
            frame.BorderColor = Color.FromHex("#008080");
            frame.HorizontalOptions = LayoutOptions.Start;
            grid.Children.Add(Msg, 0, 0);
            Grid.SetColumnSpan(Msg, 2);
            grid.Children.Add(Date, 1, 1);
            frame.Content = grid;
            // var x =Dialog.first;//.Children.Stack_Layout(frame);
            dialog._Stack_Layout.Children.Add(frame);
            await dialog._Scroll_Hystory_Message.ScrollToAsync(0, dialog._Stack_Layout.Children.Count * 300, false);
        }
    }
}