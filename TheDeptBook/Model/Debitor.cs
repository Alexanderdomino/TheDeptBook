using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Prism.Mvvm;

namespace TheDeptBook.Model;

public class Debitor : BindableBase
{
    string name;
    double balance;
    double balanceDiff;
    
    private ObservableCollection<double> transactions;
    public ObservableCollection<double> Transactions
    {
        get { return transactions; }
        set { SetProperty(ref transactions, value); }
    }

    public Debitor()
    {
    }

    public Debitor(string dName, double dBalance)
    {
        name = dName;
        balance = dBalance;
        balanceDiff = 0;
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
    
    public double BalanceDiff
    {
        get
        {
            return balanceDiff;
        }
        set
        {
            SetProperty(ref balanceDiff, value);
        }
    }
}