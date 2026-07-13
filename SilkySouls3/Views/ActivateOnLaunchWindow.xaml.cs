using System.Windows;
using System.Windows.Input;

namespace SilkySouls3.Views
{
    public partial class ActivateOnLaunchWindow
    {
        public ActivateOnLaunchWindow()
        {
            InitializeComponent();
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount != 2) DragMove();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e) => Close();
    }
}
