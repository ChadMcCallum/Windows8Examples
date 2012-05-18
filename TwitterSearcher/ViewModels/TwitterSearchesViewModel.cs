using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitterSearcher.Models;
using W8Shared;
using Windows.UI.Core;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;

namespace TwitterSearcher.ViewModels
{
    public class TwitterSearchesViewModel : INotifyPropertyChanged
    {
        public BindableObservableCollection<TwitterSearchViewModel> Searches { get; set; }

        public ICommand SearchCommand { get; set; }
        public ICommand RefreshCommand { get; set; }
        private CoreDispatcher _dispatcher;

        public string SearchText { get; set; }

        public TwitterSearchesViewModel(CoreDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
            Searches = new BindableObservableCollection<TwitterSearchViewModel>();

            this.SearchCommand = new SearchCommand(this);
            this.RefreshCommand = new RefreshCommand(this);
        }

        public void NewSearch(string searchText)
        {
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                var model = new TwitterSearchViewModel(this, searchText);
                Searches.Add(model);
                this.SearchText = string.Empty;
                if (this.PropertyChanged != null)
                    this.PropertyChanged(this, new PropertyChangedEventArgs("SearchText"));
            }
        }

        public void Refresh()
        {
            foreach (var search in Searches.AsEnumerable<TwitterSearchViewModel>())
            {
                search.Model.Update();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class RefreshCommand : ICommand
    {
        TwitterSearchesViewModel _viewModel;

        public RefreshCommand(TwitterSearchesViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        public bool CanExecute(object parameter)
        {
            return _viewModel.Searches.Count > 0;
        }

        public event Windows.UI.Xaml.EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            _viewModel.Refresh();
        }
    }

    public class SearchCommand : ICommand
    {
        TwitterSearchesViewModel _viewModel;

        public SearchCommand(TwitterSearchesViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event Windows.UI.Xaml.EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            _viewModel.NewSearch(_viewModel.SearchText);
        }
    }
}
