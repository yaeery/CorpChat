using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Configuration;
using Xamarin.Essentials;

namespace CorpChat
{
    public partial class MainPage : ContentPage
    {
        Thread thread2;// = new Thread(New_Page);

        public static string IP;
        public MainPage()
        {
            // thread2 = new Thread(New_Page);
            IP = "192.168.1.4";
            InitializeComponent();
            // Thread thread = new Thread(Animate);

            // Animate();
            //thread2.Start();
            New_Page();
            //Animate();
        }
        protected override void OnAppearing()
        {
           // Animate();
        }
        protected override bool OnBackButtonPressed()
        {
            return true;
        }
        protected override void OnDisappearing()
        {
            //Navigation.PopAsync();
        }

        public void New_Page()
        {
           // Preferences.Set("ID", "1");
            //Thread.Sleep(500);
            if ((Preferences.Get("ID", "") == "") || (Preferences.Get("ID", "") == null))
            {
                //Gear.RotateTo(360, 2000);
                Navigation.PushModalAsync(new Registration());
            }
            else
            {
                //await Gear.RotateTo(360, 2000);
                //Thread.Sleep(2000);
                Navigation.PopModalAsync();
                Navigation.PushModalAsync(new DialogsScroll());
            }
        }
         public void Animate()
        {
            int x = 0;
           // while (x<2)
           // {
                Gear.RotateTo(360, 2000);
            //    Gear.Rotation = 0;
            //    x++;
          //  }
            thread2.Start();
            //thread2.Start();
        }
    }
}
