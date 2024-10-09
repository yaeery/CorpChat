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
using System.Text.Json;

namespace CorpChat
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FindPage : ContentPage
    {
        public TcpClient tcpClient_Work;
        public TcpClient tcpClient_Connection;
        public string ID;
        public string Param_find;
        public Thread thread;
        public List<string> Name_of_Dilogs_List;
        public Dictionary<Button, string> Key_Pair_ID_Button;
        public Dictionary<string, string> Key_Pair_ID_FIO;
        JsonClassToServer jsonClassToServer;
        JsonClassToClient jsonClassToClient;
        public FindPage()
        {
            InitializeComponent();
        }
        public FindPage(TcpClient tcpClient_Work, TcpClient tcpClient_Connection, string ID, string Param_find)
        {
            InitializeComponent();
            this.tcpClient_Work = tcpClient_Work;
            this.tcpClient_Connection = tcpClient_Connection;
            this.ID = ID;
            this.Param_find = Param_find;
            Key_Pair_ID_Button = new Dictionary<Button, string>();
            Key_Pair_ID_FIO = new Dictionary<string, string>();
            Name_of_Dilogs_List = new List<string>();
            thread = new Thread(Work);
            thread.Start();
        }
        protected override void OnAppearing()
        {
          if( DialogsScroll.Count_Jump_Page == 2)
            {
                Navigation.PopModalAsync();
            }
        }
        async private void Return_Button_on_Find_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }
        void Work()
        {
            try
            {
                jsonClassToServer = new JsonClassToServer("FC", Preferences.Get("ID", ""), Param_find);
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
                        jsonClassToClient = JsonSerializer.Deserialize<JsonClassToClient>(Encoding.UTF8.GetString(_bytes, 0, size));
                        if ((jsonClassToClient.FIO_ID_for_All_Dialogs==null) ||(jsonClassToClient.FIO_ID_for_All_Dialogs.Count < 1))
                        {
                            break;
                        }
                        else
                        {
                            Name_of_Dilogs_List.Clear();
                            foreach (var i in jsonClassToClient.FIO_ID_for_All_Dialogs)
                            {
                                Name_of_Dilogs_List.Add(i);
                            }
                            jsonClassToClient.Clear();
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
                DisplayAlert("Ошибка", "Сервер прилег, поднимаем", "Ок");//на том месте
                //Decoding_String_to_List(Preferences.Get("Dialogs", ""));                              переделать важно!!!!
                Device.BeginInvokeOnMainThread(() =>
                {
                    Create_Button_Name();
                });
            }
        }
        //void Decoding_String_to_List(string Input)
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
        async void Load_Dilog(object sender, EventArgs e)
        {
            //await Navigation.PopModalAsync();
            DialogsScroll.Count_Jump_Page++;
            DialogsScroll.Dialog_ID_Enter = Key_Pair_ID_Button[(Button)sender];
            await Navigation.PushModalAsync(new Dialog(tcpClient_Work, Key_Pair_ID_FIO[Key_Pair_ID_Button[(Button)sender]], Key_Pair_ID_Button[(Button)sender]));
        }
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
                    Key_Pair_ID_Button.Add(button, Name_of_Dilogs_List[i + 1]);
                    Scroll_Layout_Dialogs.Children.Add(button);
                }
                catch { }
            }
        }
    }
}