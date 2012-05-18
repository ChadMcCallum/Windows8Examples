using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;

namespace TwitterSearcher
{
    partial class App
    {
        public App()
        {
            InitializeComponent();
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            //every time our app "launches" (whether it's been initialized or not)
            //read from application data store and re-initialize app values
            var mainPage = new MainPage();
            Window.Current.Content = mainPage;
            Window.Current.Activate();

            var settingValues = ApplicationData.Current.LocalSettings.Values;
            if (settingValues.ContainsKey("Searches"))
            {
                var searches = settingValues["Searches"];
                mainPage.InitFromStorage(searches.ToString());
            }
        }
    }
}
