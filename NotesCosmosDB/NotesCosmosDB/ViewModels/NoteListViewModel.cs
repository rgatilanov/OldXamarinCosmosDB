using NotesCosmosDB.Models;
using NotesCosmosDB.Services.CosmosDB;
using NotesCosmosDB.Services.Navigation;
using NotesCosmosDB.ViewModels.Base;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Xamarin.Forms;

namespace NotesCosmosDB.ViewModels
{
    public class NoteListViewModel : ViewModelBase
    {
        private ObservableCollection<Note> _items;
        private Note _selectedItem;

        private ICommand _addCommand;

        private INavigationService _navigationService;
        private ICosmosDBService _cosmosService;

        public NoteListViewModel(
            INavigationService navigationService,
            ICosmosDBService cosmosService)
        {
            _navigationService = navigationService;
            _cosmosService = cosmosService;
        }

        public ObservableCollection<Note> Items
        {
            get { return _items; }
            set
            {
                _items = value;
                OnPropertyChanged();
            }
        }

        public Note SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                _navigationService.NavigateTo<NoteItemViewModel>(_selectedItem);
            }
        }

        public ICommand AddCommand
        {
            get { return _addCommand = _addCommand ?? new Command(AddCommandExecute); }
        }

        public override async void OnAppearing(object navigationContext)
        {
            base.OnAppearing(navigationContext);

            await _cosmosService.CreateDatabase(AppSettings.DatabaseName);
            await _cosmosService.CreateDocumentCollection(AppSettings.DatabaseName, AppSettings.CollectionName);
            var result = await _cosmosService.GetItemsAsync<Note>();

            Items = new ObservableCollection<Note>();

            foreach (var item in result)
            {
                Items.Add(item);
            }
        }

        private void AddCommandExecute()
        {
            var todoItem = new Note();
            _navigationService.NavigateTo<NoteItemViewModel>(todoItem);
        }
    }
}