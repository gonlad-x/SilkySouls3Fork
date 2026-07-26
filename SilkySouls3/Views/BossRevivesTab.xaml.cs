using SilkySouls3.ViewModels;

namespace SilkySouls3.Views
{
    public partial class BossRevivesTab
    {
        public BossRevivesTab(BossRevivesViewModel bossRevivesViewModel)
        {
            InitializeComponent();
            DataContext = bossRevivesViewModel;
        }
    }
}
