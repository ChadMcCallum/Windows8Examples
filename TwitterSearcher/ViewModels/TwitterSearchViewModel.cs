using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitterSearcher.Models;
using Windows.UI.StartScreen;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;

namespace TwitterSearcher.ViewModels
{
    public class TwitterSearchViewModel
    {
        public TwitterSearchModel Model { get; set; }
        public ICommand PinToStart { get; set; }
        public ICommand Remove { get; set; }
        TwitterSearchesViewModel _viewModel;
        DispatcherTimer _timer;

        public TwitterSearchViewModel(TwitterSearchesViewModel viewModel, string searchTerm)
        {
            _viewModel = viewModel;
            Model = new TwitterSearchModel(searchTerm);
            PinToStart = new PinToStartCommand(this);
            Remove = new RemoveCommand(this);
            _timer = new DispatcherTimer();
            _timer.Tick += new Windows.UI.Xaml.EventHandler(_timer_Tick);
            _timer.Interval = TimeSpan.FromSeconds(60);
            _timer.Start();
        }

        void _timer_Tick(object sender, object e)
        {
            Model.Update();
        }


        internal void Close()
        {
            _viewModel.Searches.Remove(this);
        }


        internal async void DoPintoStart()
        {
            //creating a new app tile for the specific search
            var logo = new Uri("ms-resource:images/Logo.png");
            var smallLogo = new Uri("ms-resource:images/SmallLogo.png");
            var tile = new SecondaryTile("TST." + this.Model.SearchTerm, this.Model.SearchTerm, this.Model.SearchTerm, 
                string.Empty, TileDisplayAttributes.DynamicTileCapable, logo);
            tile.Logo = logo;
            tile.DisplayName = this.Model.SearchTerm;
            tile.ForegroundText = ForegroundText.Light;
            tile.SmallLogo = smallLogo;
            //creating a tile is an async operation
            var pinned = await tile.RequestCreateAsync();

            //to be honest, I have no idea how to make this tile do anything once clicked... documentation wasn't clear.
            //but, it's there.
        }
    }

    class PinToStartCommand : ICommand
    {
        private TwitterSearchViewModel _model;

        public PinToStartCommand(TwitterSearchViewModel model)
        {
            _model = model;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event Windows.UI.Xaml.EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            _model.DoPintoStart();
        }
    }

    class RemoveCommand : ICommand
    {
        private TwitterSearchViewModel _model;

        public RemoveCommand(TwitterSearchViewModel model)
        {
            _model = model;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event Windows.UI.Xaml.EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            _model.Close();
        }
    }

}
