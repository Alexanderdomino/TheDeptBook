using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using Prism.Commands;
using Prism.Mvvm;
using TheDeptBook.Model;

namespace TheDeptBook.ViewModel;

public class TransactionHistoryViewModel : BindableBase
{
    public TransactionHistoryViewModel(string title, Debitor debitor)
    {
        Trace.WriteLine("Entering Constructor of TransactionHistoryViewModel");
        CurrentTransactions = new ObservableCollection<double>();
        Title = title;
        Trace.WriteLine(Title);
        CurrentDebitor = debitor;
        Trace.WriteLine(CurrentDebitor);
        CurrentTransactions = debitor.Transactions;
        Trace.WriteLine(debitor.Transactions);
        Trace.WriteLine(CurrentTransactions);
    }

    #region Properties
    
    private ObservableCollection<double> currentTransactions;
    public ObservableCollection<double> CurrentTransactions
    {
        get { return currentTransactions; }
        set { SetProperty(ref currentTransactions, value); }
    }
    
    string title;

    public string Title
    {
        get { return title; }
        set
        {
            SetProperty(ref title, value);
        }
    }
    Debitor currentDebitor;

    public Debitor CurrentDebitor
    {
        get { return currentDebitor; }
        set
        {
            SetProperty(ref currentDebitor, value);
        }
    }

    //bool isValid;

    public bool IsValid
    {
        get
        {
            bool isValid = !string.IsNullOrWhiteSpace(CurrentDebitor.Name);
            return isValid;
        }
    }

    #endregion

    #region Commands

    ICommand _addBtnCommand;
    public ICommand AddBtnCommand
    {
        get
        {
            return _addBtnCommand ??= new DelegateCommand(
                    AddBtnCommand_Execute, AddBtnCommand_CanExecute)
                .ObservesProperty(() => CurrentDebitor.Name);
        }
    }

    private void AddBtnCommand_Execute()
    {
        Trace.WriteLine("entering addCommand");
        CurrentTransactions.Add(CurrentDebitor.BalanceDiff);
        Trace.WriteLine("added to list");
        Trace.WriteLine(CurrentTransactions);
        CurrentDebitor.Balance += CurrentDebitor.BalanceDiff;
        Trace.WriteLine("added to balance");
    }

    private bool AddBtnCommand_CanExecute()
    {
        return IsValid;
    }

    #endregion
    
    ICommand _closeCommand;
    public ICommand CloseCommand
    {
        get
        {
            return _closeCommand ??= new DelegateCommand(
                    CloseCommand_Execute, CloseCommand_CanExecute)
                .ObservesProperty(() => CurrentDebitor.Name);
        }
    }
    private void CloseCommand_Execute()
    {
        // No action here - is handled i code behind
    }

    private bool CloseCommand_CanExecute()
    {
        return IsValid;
    }
}

