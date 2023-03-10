using System.Windows;
using TheDeptBook.ViewModel;

namespace TheDeptBook.View;

public partial class TransactionHistoryView : Window
{
    public TransactionHistoryView()
    {
        InitializeComponent();
    }

    private void btnSave_Click(object sender, RoutedEventArgs e)
    {
        var vm = DataContext as TransactionHistoryViewModel;
        if (vm.IsValid)
            DialogResult = true;
        else
            MessageBox.Show("Enter values", "Missing data");
    }
}