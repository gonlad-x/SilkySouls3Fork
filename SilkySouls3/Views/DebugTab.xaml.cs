using SilkySouls3.ViewModels;

namespace SilkySouls3.Views
{
    public partial class DebugTab
    {
        public DebugTab(DebugViewModel debugViewModel)
        {
            InitializeComponent();
            DataContext = debugViewModel;
        }
    }
}
