using SilkySouls3.ViewModels;

namespace SilkySouls3.Views
{
    public partial class EnemyTab
    {
        public EnemyTab(EnemyViewModel enemyViewModel, BossRevivesTab bossRevivesTab)
        {
            InitializeComponent();
            DataContext = enemyViewModel;
            BossRevivesHost.Content = bossRevivesTab;
        }
    }
}
