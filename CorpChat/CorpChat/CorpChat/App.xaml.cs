using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CorpChat
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new MainPage())
            {
                BarBackgroundColor = Color.Black
            };
          //  NavigationPage.BarBackgroundColor = Color.Black;
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
