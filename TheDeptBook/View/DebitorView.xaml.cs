using System.Windows;
using TheDeptBook.ViewModel;

namespace TheDeptBook.View;

public partial class DebitorView : Window
{
    public DebitorView()
    {
        InitializeComponent();
    }
    
    private void btnSave_Click(object sender, RoutedEventArgs e)
    {
        var vm = DataContext as DebitorViewModel;
        if (vm.IsValid)
            DialogResult = true;
        else
            MessageBox.Show("Enter values for Id, codename and specialities", "Missing data");
    }
}