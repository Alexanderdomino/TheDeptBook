using Prism.Mvvm;

namespace TheDeptBook.Model;

public class Debitor : BindableBase
{
    string name;
    double balance;

    public Debitor()
    {
    }

    public Debitor(string dName, double dBalance)
    {
        name = dName;
        balance = dBalance;
    }
    
    public Debitor Clone()
    {
        return this.MemberwiseClone() as Debitor;
    }

    public string Name
    {
        get
        {
            return name;
        }
        set
        {
            SetProperty(ref name, value);
        }
    }

    public double Balance
    {
        get
        {
            return balance;
        }
        set
        {
            SetProperty(ref balance, value);
        }
    }
}