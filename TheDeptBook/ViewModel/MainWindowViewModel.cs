using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Media;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Mvvm;
using TheDeptBook.Data;
using TheDeptBook.Model;
using TheDeptBook.View;

namespace TheDeptBook.ViewModel;

public class MainWindowViewModel : BindableBase
{
    private readonly string AppTitle = "TheDeptBook";
    private string filePath = "";

        public MainWindowViewModel()
        {
            Debitors = new ObservableCollection<Debitor>();
            Debitors.Add(new Debitor("Søren Nielsen", 2000));
            Debitors.Add(new Debitor("Søren Hansen",-5560));
            CurrentDebitor = Debitors[0];
        }

        #region Properties

        private Debitor currentDebitor;
        public Debitor CurrentDebitor
        {
            get { return currentDebitor; }
            set { SetProperty(ref currentDebitor, value); }
        }

        private ObservableCollection<Debitor> debitors;
        public ObservableCollection<Debitor> Debitors
        {
            get { return debitors; }
            set { SetProperty(ref debitors, value); }
        }

        private int currentIndex;
        public int CurrentIndex
        {
            get { return currentIndex; }
            set { SetProperty(ref currentIndex, value); }
        }

        private string filename = "";
        public string Filename
        {
            get { return filename; }
            set
            {
                SetProperty(ref filename, value);
                RaisePropertyChanged("Title");
            }
        }

        public string Title
        {
            get
            {
                var s = "";
                if (Dirty)
                    s = "*";
                return Filename + s + " - " + AppTitle;
            }
        }

        private bool dirty = false;
        public bool Dirty
        {
            get { return dirty; }
            set
            {
                SetProperty(ref dirty, value);
                RaisePropertyChanged("Title");
            }
        }
        #endregion properties

        #region Commands

        private DelegateCommand addCommand;
        public DelegateCommand AddCommand =>
            addCommand ??= new DelegateCommand(ExecuteAddCommand);

        void ExecuteAddCommand()
        {
            var newDebitor = new Debitor();
            var vm = new DebitorViewModel("Add new debitor", newDebitor);

            var dlg = new DebitorView
            {
                DataContext = vm
            };
            if (dlg.ShowDialog() == true)
            {
                Debitors.Add(newDebitor);
                Trace.WriteLine(newDebitor.Transactions);
                CurrentDebitor = newDebitor; // Or CurrentIndex = Debitors.Count - 1;
                Dirty = true;
            }
        }

        private DelegateCommand deleteCommand;
        public DelegateCommand DeleteCommand =>
            deleteCommand ??= new DelegateCommand(ExecuteDeleteCommand, DeleteAgent_CanExecute)
                .ObservesProperty(() => CurrentIndex);

        void ExecuteDeleteCommand()
        {
            MessageBoxResult res = MessageBox.Show("Are you sure you want to delete agent " + CurrentDebitor.Name +
                "?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
            if (res == MessageBoxResult.Yes)
            {
                Debitors.Remove(CurrentDebitor);
                Dirty = true;
            }
        }

        private bool DeleteAgent_CanExecute()
        {
            if (Debitors.Count > 0 && CurrentIndex >= 0)
                return true;
            else
                return false;
        }

        private DelegateCommand closeAppCommand;
        public DelegateCommand CloseAppCommand =>
            closeAppCommand ??= new DelegateCommand(ExecuteCloseAppCommand);

        void ExecuteCloseAppCommand()
        {
            if (Application.Current.MainWindow != null) Application.Current.MainWindow.Close();
        }

        private DelegateCommand<string> _colorCommand;
        public DelegateCommand<string> ColorCommand =>
            _colorCommand ??= new DelegateCommand<string>(ExecuteColorCommand);

        void ExecuteColorCommand(string colorStr)
        {
            SolidColorBrush newBrush = SystemColors.WindowBrush; // Default color

            try
            {
                if (colorStr != null)
                {
                    if (colorStr != "Default")
                        newBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(colorStr));
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Unknown color name, default color is used", "Program error!");
            }

            Application.Current.Resources["BackgroundBrush"] = newBrush;
        }

        private DelegateCommand _editCommand;
        public DelegateCommand EditCommand =>
            _editCommand ??= new DelegateCommand(ExecuteEditCommand, CanExecuteEditCommand)
                .ObservesProperty(() => CurrentIndex);

        void ExecuteEditCommand()
        {
            var tempDebitor = CurrentDebitor.Clone();
            var vm = new DebitorViewModel("Edit debitor", tempDebitor)
            {
                //Specialities = specialities
            };
            var dlg = new DebitorView 
            {
                DataContext = vm,
                Owner = Application.Current.MainWindow
            };
            if (dlg.ShowDialog() != true) return;
            // Copy values back
            CurrentDebitor.Name = tempDebitor.Name;
            CurrentDebitor.Balance = tempDebitor.Balance;
            Dirty = true;
        }

        bool CanExecuteEditCommand()
        {
            return CurrentIndex >= 0;
        }

        private DelegateCommand _addTransactionCommand;
        public DelegateCommand AddTransactionCommand =>
            _addTransactionCommand ??= new DelegateCommand(ExecuteAddTransactionCommand, CanExecuteEditCommand)
                .ObservesProperty(() => CurrentIndex);

        private void ExecuteAddTransactionCommand()
        {
            var tempDebitor = CurrentDebitor.Clone();
            var vm = new TransactionHistoryViewModel("Add Transaction", tempDebitor)
            {
            };
            var dlg = new TransactionHistoryView() 
            {
                DataContext = vm,
                Owner = Application.Current.MainWindow
            };
            if (dlg.ShowDialog() == true)
            {
                CurrentDebitor.Balance = tempDebitor.Balance;
                CurrentDebitor.Transactions = tempDebitor.Transactions;
                Dirty = true;
            }
        }

        DelegateCommand _NewFileCommand;
        public DelegateCommand NewFileCommand
        {
            get { return _NewFileCommand ??= new DelegateCommand(NewFileCommand_Execute); }
        }

        private void NewFileCommand_Execute()
        {
            MessageBoxResult res = MessageBox.Show("Any unsaved data will be lost. Are you sure you want to initiate a new file?", "Warning",
                MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
            if (res == MessageBoxResult.Yes)
            {
                Debitors.Clear();
                Filename = "";
                Dirty = false;
            }
        }


        DelegateCommand _OpenFileCommand;
        public DelegateCommand OpenFileCommand
        {
            get { return _OpenFileCommand ?? (_OpenFileCommand = new DelegateCommand(OpenFileCommand_Execute)); }
        }

        private void OpenFileCommand_Execute()
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Agent assignment documents|*.tdb|All Files|*.*",
                DefaultExt = "tdb"
            };
            if (filePath == "")
                dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            else
                dialog.InitialDirectory = Path.GetDirectoryName(filePath);

            if (dialog.ShowDialog(App.Current.MainWindow) == true)
            {
                filePath = dialog.FileName;
                Filename = Path.GetFileName(filePath);
                try
                {
                    Debitors = Repository.ReadFile(filePath);
                    Dirty = false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Unable to open file", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        DelegateCommand _SaveAsCommand;
        public DelegateCommand SaveAsCommand
        {
            get { return _SaveAsCommand ??= new DelegateCommand(SaveAsCommand_Execute); }
        }

        private void SaveAsCommand_Execute()
        {
            var dialog = new SaveFileDialog
            {
                Filter = "The Dept Book documents|*.tdb|All Files|*.*",
                DefaultExt = "tdb"
            };
            if (filePath == "")
                dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            else
                dialog.InitialDirectory = Path.GetDirectoryName(filePath);

            if (dialog.ShowDialog(App.Current.MainWindow) == true)
            {
                filePath = dialog.FileName;
                Filename = Path.GetFileName(filePath);
                SaveFile();
            }
        }

        DelegateCommand _SaveCommand;
        public DelegateCommand SaveCommand
        {
            get
            {
                return _SaveCommand ??= new DelegateCommand(SaveFileCommand_Execute, SaveFileCommand_CanExecute)
                    .ObservesProperty(() => Debitors.Count);
            }
        }

        private void SaveFileCommand_Execute()
        {
            SaveFile();
        }

        private bool SaveFileCommand_CanExecute()
        {
            return (filename != "") && (Debitors.Count > 0);
        }

        private void SaveFile()
        {
            try
            {
                Repository.SaveFile(filePath, Debitors);
                Dirty = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Unable to save file", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        DelegateCommand<CancelEventArgs> _closingCommand;
        public DelegateCommand<CancelEventArgs> ClosingCommand
        {
            get
            {
                return _closingCommand ??= new
                    DelegateCommand<CancelEventArgs>(ClosingCommand_Execute);
            }
        }

        private void ClosingCommand_Execute(CancelEventArgs arg)
        {
            if (Dirty)
                arg.Cancel = UserRegrets();
        }

        private bool UserRegrets()
        {
            var regret = false;
            MessageBoxResult res = MessageBox.Show("You have unsaved data. Are you sure you want to close the application without saving data first?",
                            "Warning", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
            if (res == MessageBoxResult.No)
            {
                regret = true;
            }
            return regret;
        }
        #endregion Commands
}