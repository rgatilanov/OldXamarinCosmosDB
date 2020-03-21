using NotesCosmosDB.Models;
using NotesCosmosDB.Services.CosmosDB;
using NotesCosmosDB.Services.Navigation;
using NotesCosmosDB.ViewModels.Base;
using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace NotesCosmosDB.ViewModels
{
    public class NoteItemViewModel : ViewModelBase
    {
        private string _id;
        private string _notes;
        private DateTime _date;
        private ICommand _saveCommand;
        private ICommand _deleteCommand;
        private ICommand _cancelCommand;

        private INavigationService _navigationService;
        private ICosmosDBService _cosmosService;

        public NoteItemViewModel(
            INavigationService navigationService,
            ICosmosDBService cosmosService)
        {
            _navigationService = navigationService;
            _cosmosService = cosmosService;
        }

        public string Id
        {
            get { return _id; }
            set
            {
                _id = value;
                OnPropertyChanged();
            }
        }

        public string Notes
        {
            get { return _notes; }
            set
            {
                _notes = value;
                OnPropertyChanged();
            }
        }

        public DateTime Date
        {
            get { return _date; }
            set
            {
                _date = value;
                OnPropertyChanged();
            }
        }

        public ICommand SaveCommand
        {
            get { return _saveCommand = _saveCommand ?? new Command(SaveCommandExecute); }
        }

        public ICommand DeleteCommand
        {
            get { return _deleteCommand = _deleteCommand ?? new Command(DeleteCommandExecute); }
        }

        public ICommand CancelCommand
        {
            get { return _cancelCommand = _cancelCommand ?? new Command(CancelCommandExecute); }
        }

        public override void OnAppearing(object navigationContext)
        {
            var note = navigationContext as Note;

            if (note != null)
            {
                Id = note.Id;
                Notes = note.Notes;
            }

            base.OnAppearing(navigationContext);
        }

        private void SaveCommandExecute()
        {
            var isNew = false;

            if(string.IsNullOrEmpty(Id))
            {
                Id = Guid.NewGuid().ToString();
                isNew = true;
            }

            Date = DateTime.Now;

            var item = new Note
            {
                Id = Id,
                Notes = Notes,
                Date = Date
            };

            _cosmosService.SaveItemAsync(item, Id, isNew);
            _navigationService.NavigateBack();
        }


        private void DeleteCommandExecute()
        {
            _cosmosService.DeleteItemAsync(Id);
            _navigationService.NavigateBack();
        }

        private void CancelCommandExecute()
        {
            _navigationService.NavigateBack();
        }
    }
}