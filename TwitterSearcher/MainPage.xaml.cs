using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitterSearcher.Models;
using TwitterSearcher.ViewModels;
using Windows.Foundation;
using Windows.Storage;
using Windows.UI.ApplicationSettings;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace TwitterSearcher
{
    partial class MainPage
    {
        TwitterSearchesViewModel viewModel;

        public MainPage()
        {
            InitializeComponent();
            viewModel = new TwitterSearchesViewModel(this.Dispatcher);
            this.DataContext = viewModel;

            //handling a "suspend" event, new to win 8 (similar to win phone 7)
            Application.Current.Suspending += new SuspendingEventHandler(Current_Suspending);
        }

        void Current_Suspending(object sender, Windows.ApplicationModel.SuspendingEventArgs e)
        {
            //using local settings to persist data between active application instances
            var settingValues = ApplicationData.Current.LocalSettings.Values;
            if (settingValues.ContainsKey("Searches"))
            {
                settingValues.Remove("Searches");
            }
            var searches = new StringBuilder();
            foreach (var searchModel in viewModel.Searches.AsEnumerable<TwitterSearchViewModel>())
            {
                searches.AppendFormat("{0};", searchModel.Model.SearchTerm);
            }
            settingValues.Add("Searches", searches.ToString());
        }

        internal void InitFromStorage(string searches)
        {
            string[] terms = searches.Split(';');
            foreach(var term in terms)
            {
                viewModel.NewSearch(term);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ShowSettings();
        }

        private void ShowSettings()
        {
            var settings = SettingsPane.GetForCurrentView();
            //adding a custom command to the application settings window
            var command = new SettingsCommand(KnownSettingsCommand.Preferences, ShowPreferences);
            settings.ApplicationCommands.Add(command);
            SettingsPane.Show();
        }

        private async void ShowPreferences(IUICommand command)
        {
            var dialog = new MessageDialog("Preferences!", "Preferences?");
            await dialog.ShowAsync();
        }
    }
}
