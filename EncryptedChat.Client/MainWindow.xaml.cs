using EncryptedChat.Client.ViewModels;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace EncryptedChat.Client
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();

            ((INotifyCollectionChanged)MessagesItems.ItemsSource).CollectionChanged += (s, e) => Scroller.ScrollToEnd();
        }

        private void DockPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Application.Current.MainWindow.DragMove();
        }
    }
}
