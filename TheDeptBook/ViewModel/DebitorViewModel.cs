using System.Windows.Input;
using Prism.Commands;
using Prism.Mvvm;
using TheDeptBook.Model;

namespace TheDeptBook.ViewModel;

public class DebitorViewModel : BindableBase
{
    public DebitorViewModel(string title, Debitor debitor)
    {
        Title = title;
        CurrentDebitor = debitor;
    }

    #region Properties
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

    ICommand _saveBtnCommand;
    public ICommand OkBtnCommand
    {
        get
        {
            return _saveBtnCommand ??= new DelegateCommand(
                    SaveBtnCommand_Execute, SaveBtnCommand_CanExecute)
                .ObservesProperty(() => CurrentDebitor.Name);
        }
    }

    private void SaveBtnCommand_Execute()
    {
        // No action here - is handled i code behind
    }

    private bool SaveBtnCommand_CanExecute()
    {
        return IsValid;
    }

    #endregion
}