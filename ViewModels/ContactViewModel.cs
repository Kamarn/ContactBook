using ContactBookApp.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace ContactBookApp.ViewModels
{
    public class ContactViewModel : INotifyPropertyChanged
    {
        private ContactDatabase _contactDatabase;
        private ObservableCollection<Contact> _contacts;
        private Contact _selectedContact;
        private string _searchText;
        private Contact _newContact;
        private Contact _tempContact;

        public ObservableCollection<Contact> Contacts
        {
            get => _contacts;
            set => SetProperty(ref _contacts, value);
        }

        public Contact SelectedContact
        {
            get => _selectedContact;
            set
            {
                if (SetProperty(ref _selectedContact, value) && _selectedContact != null)
                {
                    // Create a temporary copy when a contact is selected
                    TempContact = new Contact()
                    {
                        Id = _selectedContact.Id,
                        FirstName = _selectedContact.FirstName,
                        LastName = _selectedContact.LastName,
                        PhoneNumber = _selectedContact.PhoneNumber,
                        EmailAddress = _selectedContact.EmailAddress,
                        BirthDate = _selectedContact.BirthDate
                    };
                }
            }
        }

        public string SearchText
        {
            get => _searchText;
            set => SetProperty(ref _searchText, value);
        }

        public Contact NewContact
        {
            get => _newContact;
            set => SetProperty(ref _newContact, value);
        }

        public Contact TempContact
        {
            get => _tempContact;
            set => SetProperty(ref _tempContact, value);
        }

        public ICommand AddContactCommand { get; }
        public ICommand EditContactCommand { get; }
        public ICommand DeleteContactCommand { get; }
        public ICommand SearchCommand { get; }

        public ContactViewModel()
        {
            _contactDatabase = new ContactDatabase();
            Contacts = new ObservableCollection<Contact>(_contactDatabase.GetContacts());
            NewContact = new Contact
            {
                BirthDate = DateTime.Today
            };

            AddContactCommand = new RelayCommand(AddContact);
            EditContactCommand = new RelayCommand(EditContact);
            DeleteContactCommand = new RelayCommand(DeleteContact);
            SearchCommand = new RelayCommand(SearchContacts);
        }

        private void AddContact()
        {
            _contactDatabase.AddContact(NewContact);
            Contacts.Add(NewContact);
            NewContact = new Contact
            {
                BirthDate = DateTime.Today
            };
        }

        private void EditContact()
        {
            if (SelectedContact != null)
            {
                // Copy data from TempContact to SelectedContact before saving
                SelectedContact.FirstName = TempContact.FirstName;
                SelectedContact.LastName = TempContact.LastName;
                SelectedContact.PhoneNumber = TempContact.PhoneNumber;
                SelectedContact.EmailAddress = TempContact.EmailAddress;
                SelectedContact.BirthDate = TempContact.BirthDate;

                _contactDatabase.UpdateContact(SelectedContact);
                RefreshContacts();
            }
        }

        private void DeleteContact()
        {
            if (SelectedContact != null)
            {
                _contactDatabase.DeleteContact(SelectedContact.Id);
                Contacts.Remove(SelectedContact);
                SelectedContact = null;
                TempContact = new Contact();
            }
        }

        private void SearchContacts()
        {
            if (!string.IsNullOrEmpty(SearchText))
            {
                var filteredContacts = _contactDatabase.GetContacts().FindAll(c =>
                    c.FirstName.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                    c.LastName.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                    c.PhoneNumber.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                    c.EmailAddress.Contains(SearchText, StringComparison.OrdinalIgnoreCase));
                Contacts = new ObservableCollection<Contact>(filteredContacts);
            }
            else
            {
                Contacts = new ObservableCollection<Contact>(_contactDatabase.GetContacts());
            }
        }

        private void RefreshContacts()
        {
            Contacts = new ObservableCollection<Contact>(_contactDatabase.GetContacts());
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value)) return false;
            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }

    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute();
        }

        public void Execute(object parameter)
        {
            _execute();
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}