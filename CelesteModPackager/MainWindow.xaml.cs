using CelesteModPackager.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace CelesteModPackager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private MainWindowViewModel ViewModel;

        public MainWindow()
        {
            ViewModel = new MainWindowViewModel( this );
            InitializeComponent();
            DataContext = ViewModel;
        }

        //private void IsCodeModCheckBox_Checked( object sender, RoutedEventArgs e )
        //{
        //    CheckBox cb = ( sender as CheckBox );
        //    ViewModel.Project.IsCodeMod = cb.IsChecked.HasValue && cb.IsChecked.Value;
        //}
    }
}
