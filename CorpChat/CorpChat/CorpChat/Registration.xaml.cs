using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Text.Json;
using System.Threading;
namespace CorpChat
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Registration : ContentPage
    {
        TcpClient tcpClient_Connection;
        TcpClient tcpClient_Work;
        JsonClassToServer jsonClassToServer;
        JsonClassToClient jsonClassToClient;
        public Registration()
        {
            InitializeComponent();
            //this.ParentViewController.NavigationItem.SetHidesBackButton(true, false);
        }
         
        protected override bool OnBackButtonPressed()
        {
            return true;
        }
        private void Button_Enter_Clicked(object sender, EventArgs e)
        {
            bool Is_No_Correct = false;
            if(((Login_Enter.Text==null)||(Login_Enter.Text.ToString()==""))&&((Pass_Enter.Text==null)||(Pass_Enter.Text.Length != 8)))
            {
                Is_No_Correct = true;
                Login_Enter.Text = null;
                Pass_Enter.Text = null;
                DisplayAlert("Внимание!", "Введите логин и пароль", "Ок");
            }
            else if((Login_Enter.Text == null) || (Login_Enter.Text.ToString() == ""))
            {
                Is_No_Correct = true;
                Login_Enter.Text = null;
                DisplayAlert("Внимание!", "Введите логин", "Ок");
            }
            else if ((Pass_Enter.Text == null) || (Pass_Enter.Text.Length != 8))
            {
                Is_No_Correct = true;
                Pass_Enter.Text = null;
                DisplayAlert("Внимание!", "Введите пароль", "Ок");
            }
            if(Is_No_Correct==false)
            {
                Work("GI#"+Login_Enter.Text.ToString()+"#"+ Pass_Enter.Text.ToString());
            }
        }


        public void Work(string Inputx)
        {
            try
            {
                jsonClassToServer = new JsonClassToServer("GI", Login_Enter.Text.ToString(), Pass_Enter.Text.ToString());
                string Input = "";
                tcpClient_Connection = new TcpClient(MainPage.IP, 33023);
                Input+=JsonSerializer.Serialize(jsonClassToServer);
                Thread.Sleep(5000);
                tcpClient_Work = new TcpClient(MainPage.IP, 33024);
                NetworkStream networkStream = tcpClient_Work.GetStream();
                byte[] bytes = Encoding.UTF8.GetBytes(Input);//(Login.Text + "#" + Password.Text);
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
                        if (jsonClassToClient.Result_Registration=="N")
                        {
                            DisplayAlert("Внимание!", "Неверный логин или пароль", "Ок");
                            break;
                        }
                        else
                        {
                            Preferences.Set("ID", jsonClassToClient.Result_Registration);
                            Navigation.PushModalAsync(new DialogsScroll(tcpClient_Work,tcpClient_Connection));
                            break;
                        }
                        jsonClassToClient.Clear();
                        Pass_Enter.Text = null;
                        Login_Enter.Text = null;
                        networkStream1.Flush();
                    }
                }
            }
            catch
            {
                DisplayAlert("Ошибка", "Сервер прилег, поднимаем", "Ок");
            }
        }
    }
}