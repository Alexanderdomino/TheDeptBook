using System.Collections.ObjectModel;
using System.Windows.Input;
using Prism.Commands;
using Prism.Mvvm;
using TheDeptBook.Model;

namespace TheDeptBook.ViewModel;

public class TransactionHistoryViewModel : BindableBase
{
    public TransactionHistoryViewModel(string title, Debitor debitor)
    {
        Title = title;
        CurrentDebitor = debitor;
        CurrentTransactions = debitor.Transactions;
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
        CurrentTransactions.Add(CurrentDebitor.BalanceDiff);
        CurrentDebitor.Balance += CurrentDebitor.BalanceDiff;
    }

    private bool AddBtnCommand_CanExecute()
    {
        return IsValid;
    }

    #endregion
}

