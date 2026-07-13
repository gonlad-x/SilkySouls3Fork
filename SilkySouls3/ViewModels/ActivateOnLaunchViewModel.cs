using SilkySouls3.Enums;
using SilkySouls3.Interfaces;
using SilkySouls3.Utilities;

namespace SilkySouls3.ViewModels
{
    public class ActivateOnLaunchViewModel : BaseViewModel
    {
        private readonly PlayerViewModel _playerViewModel;
        private readonly TargetViewModel _targetViewModel;
        private readonly UtilityViewModel _utilityViewModel;
        private readonly TravelViewModel _travelViewModel;
        private readonly ItemViewModel _itemViewModel;

        public ActivateOnLaunchViewModel(PlayerViewModel playerViewModel, TargetViewModel targetViewModel,
            UtilityViewModel utilityViewModel, TravelViewModel travelViewModel, ItemViewModel itemViewModel,
            IStateService stateService)
        {
            _playerViewModel = playerViewModel;
            _targetViewModel = targetViewModel;
            _utilityViewModel = utilityViewModel;
            _travelViewModel = travelViewModel;
            _itemViewModel = itemViewModel;

            LoadPrefs();

            stateService.Subscribe(State.Loaded, OnGameLoaded);
            stateService.Subscribe(State.OnNewGameStart, OnNewGameStart);
        }

        #region Properties

        private bool _isEnabled;

        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                if (!SetProperty(ref _isEnabled, value)) return;
                SettingsManager.Default.ActivateOnLaunchEnabled = value;
                SettingsManager.Default.Save();
            }
        }

        private bool _isNoDeathChecked;

        public bool IsNoDeathChecked
        {
            get => _isNoDeathChecked;
            set
            {
                if (!SetProperty(ref _isNoDeathChecked, value)) return;
                SettingsManager.Default.ActivateOnLaunchNoDeath = value;
                SettingsManager.Default.Save();
            }
        }

        private bool _isNoDamageChecked;

        public bool IsNoDamageChecked
        {
            get => _isNoDamageChecked;
            set
            {
                if (!SetProperty(ref _isNoDamageChecked, value)) return;
                SettingsManager.Default.ActivateOnLaunchNoDamage = value;
                SettingsManager.Default.Save();
            }
        }

        private bool _isInfiniteStaminaChecked;

        public bool IsInfiniteStaminaChecked
        {
            get => _isInfiniteStaminaChecked;
            set
            {
                if (!SetProperty(ref _isInfiniteStaminaChecked, value)) return;
                SettingsManager.Default.ActivateOnLaunchInfiniteStamina = value;
                SettingsManager.Default.Save();
            }
        }

        private bool _isNoGoodsConsumeChecked;

        public bool IsNoGoodsConsumeChecked
        {
            get => _isNoGoodsConsumeChecked;
            set
            {
                if (!SetProperty(ref _isNoGoodsConsumeChecked, value)) return;
                SettingsManager.Default.ActivateOnLaunchNoGoodsConsume = value;
                SettingsManager.Default.Save();
            }
        }

        private bool _isInfiniteFpChecked;

        public bool IsInfiniteFpChecked
        {
            get => _isInfiniteFpChecked;
            set
            {
                if (!SetProperty(ref _isInfiniteFpChecked, value)) return;
                SettingsManager.Default.ActivateOnLaunchInfiniteFp = value;
                SettingsManager.Default.Save();
            }
        }

        private bool _isInfiniteDurabilityChecked;

        public bool IsInfiniteDurabilityChecked
        {
            get => _isInfiniteDurabilityChecked;
            set
            {
                if (!SetProperty(ref _isInfiniteDurabilityChecked, value)) return;
                SettingsManager.Default.ActivateOnLaunchInfiniteDurability = value;
                SettingsManager.Default.Save();
            }
        }

        private bool _isOneShotChecked;

        public bool IsOneShotChecked
        {
            get => _isOneShotChecked;
            set
            {
                if (!SetProperty(ref _isOneShotChecked, value)) return;
                SettingsManager.Default.ActivateOnLaunchOneShot = value;
                SettingsManager.Default.Save();
            }
        }

        private bool _isInvisibleChecked;

        public bool IsInvisibleChecked
        {
            get => _isInvisibleChecked;
            set
            {
                if (!SetProperty(ref _isInvisibleChecked, value)) return;
                SettingsManager.Default.ActivateOnLaunchInvisible = value;
                SettingsManager.Default.Save();
            }
        }

        private bool _isSilentChecked;

        public bool IsSilentChecked
        {
            get => _isSilentChecked;
            set
            {
                if (!SetProperty(ref _isSilentChecked, value)) return;
                SettingsManager.Default.ActivateOnLaunchSilent = value;
                SettingsManager.Default.Save();
            }
        }

        private bool _isNoAmmoConsumeChecked;

        public bool IsNoAmmoConsumeChecked
        {
            get => _isNoAmmoConsumeChecked;
            set
            {
                if (!SetProperty(ref _isNoAmmoConsumeChecked, value)) return;
                SettingsManager.Default.ActivateOnLaunchNoAmmoConsume = value;
                SettingsManager.Default.Save();
            }
        }

        private bool _isInfinitePoiseChecked;

        public bool IsInfinitePoiseChecked
        {
            get => _isInfinitePoiseChecked;
            set
            {
                if (!SetProperty(ref _isInfinitePoiseChecked, value)) return;
                SettingsManager.Default.ActivateOnLaunchInfinitePoise = value;
                SettingsManager.Default.Save();
            }
        }

        private bool _isNoHitChecked;

        public bool IsNoHitChecked
        {
            get => _isNoHitChecked;
            set
            {
                if (!SetProperty(ref _isNoHitChecked, value)) return;
                SettingsManager.Default.ActivateOnLaunchNoHit = value;
                SettingsManager.Default.Save();
            }
        }

        private bool _isHealOverTimeChecked;

        public bool IsHealOverTimeChecked
        {
            get => _isHealOverTimeChecked;
            set
            {
                if (!SetProperty(ref _isHealOverTimeChecked, value)) return;
                SettingsManager.Default.ActivateOnLaunchHealOverTime = value;
                SettingsManager.Default.Save();
            }
        }

        private bool _isFpRegenChecked;

        public bool IsFpRegenChecked
        {
            get => _isFpRegenChecked;
            set
            {
                if (!SetProperty(ref _isFpRegenChecked, value)) return;
                SettingsManager.Default.ActivateOnLaunchFpRegen = value;
                SettingsManager.Default.Save();
            }
        }

        private bool _isNoRollChecked;

        public bool IsNoRollChecked
        {
            get => _isNoRollChecked;
            set
            {
                if (!SetProperty(ref _isNoRollChecked, value)) return;
                SettingsManager.Default.ActivateOnLaunchNoRoll = value;
                SettingsManager.Default.Save();
            }
        }

        private bool _isAutoNewGameSevenChecked;

        public bool IsAutoNewGameSevenChecked
        {
            get => _isAutoNewGameSevenChecked;
            set
            {
                if (!SetProperty(ref _isAutoNewGameSevenChecked, value)) return;
                SettingsManager.Default.ActivateOnLaunchAutoNewGameSeven = value;
                SettingsManager.Default.Save();
            }
        }

        private bool _isTargetOptionsChecked;

        public bool IsTargetOptionsChecked
        {
            get => _isTargetOptionsChecked;
            set
            {
                if (!SetProperty(ref _isTargetOptionsChecked, value)) return;
                SettingsManager.Default.ActivateOnLaunchTargetOptions = value;
                SettingsManager.Default.Save();
            }
        }

        private bool _isUnlockFpsChecked;

        public bool IsUnlockFpsChecked
        {
            get => _isUnlockFpsChecked;
            set
            {
                if (!SetProperty(ref _isUnlockFpsChecked, value)) return;
                SettingsManager.Default.ActivateOnLaunchUnlockFps = value;
                SettingsManager.Default.Save();
            }
        }

        private int _launchFps;

        public int LaunchFps
        {
            get => _launchFps;
            set
            {
                if (!SetProperty(ref _launchFps, value)) return;
                SettingsManager.Default.ActivateOnLaunchFps = value;
                SettingsManager.Default.Save();
            }
        }

        private bool _isUnlockBonfiresChecked;

        public bool IsUnlockBonfiresChecked
        {
            get => _isUnlockBonfiresChecked;
            set
            {
                if (!SetProperty(ref _isUnlockBonfiresChecked, value)) return;
                SettingsManager.Default.ActivateOnLaunchUnlockBonfires = value;
                SettingsManager.Default.Save();
            }
        }

        private bool _isSpawnWeaponAtStartChecked;

        public bool IsSpawnWeaponAtStartChecked
        {
            get => _isSpawnWeaponAtStartChecked;
            set
            {
                if (!SetProperty(ref _isSpawnWeaponAtStartChecked, value)) return;
                SettingsManager.Default.ActivateOnLaunchSpawnWeaponAtStart = value;
                SettingsManager.Default.Save();
            }
        }

        #endregion

        private void LoadPrefs()
        {
            _isEnabled = SettingsManager.Default.ActivateOnLaunchEnabled;
            _isNoDeathChecked = SettingsManager.Default.ActivateOnLaunchNoDeath;
            _isNoDamageChecked = SettingsManager.Default.ActivateOnLaunchNoDamage;
            _isInfiniteStaminaChecked = SettingsManager.Default.ActivateOnLaunchInfiniteStamina;
            _isNoGoodsConsumeChecked = SettingsManager.Default.ActivateOnLaunchNoGoodsConsume;
            _isInfiniteFpChecked = SettingsManager.Default.ActivateOnLaunchInfiniteFp;
            _isInfiniteDurabilityChecked = SettingsManager.Default.ActivateOnLaunchInfiniteDurability;
            _isOneShotChecked = SettingsManager.Default.ActivateOnLaunchOneShot;
            _isInvisibleChecked = SettingsManager.Default.ActivateOnLaunchInvisible;
            _isSilentChecked = SettingsManager.Default.ActivateOnLaunchSilent;
            _isNoAmmoConsumeChecked = SettingsManager.Default.ActivateOnLaunchNoAmmoConsume;
            _isInfinitePoiseChecked = SettingsManager.Default.ActivateOnLaunchInfinitePoise;
            _isNoHitChecked = SettingsManager.Default.ActivateOnLaunchNoHit;
            _isHealOverTimeChecked = SettingsManager.Default.ActivateOnLaunchHealOverTime;
            _isFpRegenChecked = SettingsManager.Default.ActivateOnLaunchFpRegen;
            _isNoRollChecked = SettingsManager.Default.ActivateOnLaunchNoRoll;
            _isAutoNewGameSevenChecked = SettingsManager.Default.ActivateOnLaunchAutoNewGameSeven;
            _isTargetOptionsChecked = SettingsManager.Default.ActivateOnLaunchTargetOptions;
            _isUnlockFpsChecked = SettingsManager.Default.ActivateOnLaunchUnlockFps;
            _launchFps = SettingsManager.Default.ActivateOnLaunchFps;
            _isUnlockBonfiresChecked = SettingsManager.Default.ActivateOnLaunchUnlockBonfires;
            _isSpawnWeaponAtStartChecked = SettingsManager.Default.ActivateOnLaunchSpawnWeaponAtStart;
        }

        private void OnGameLoaded()
        {
            if (!IsEnabled) return;

            if (IsNoDeathChecked) _playerViewModel.IsNoDeathEnabled = true;
            if (IsNoDamageChecked) _playerViewModel.IsNoDamageEnabled = true;
            if (IsInfiniteStaminaChecked) _playerViewModel.IsInfiniteStaminaEnabled = true;
            if (IsNoGoodsConsumeChecked) _playerViewModel.IsNoGoodsConsumeEnabled = true;
            if (IsInfiniteFpChecked) _playerViewModel.IsInfiniteFpEnabled = true;
            if (IsInfiniteDurabilityChecked) _playerViewModel.IsInfiniteDurabilityEnabled = true;
            if (IsOneShotChecked) _playerViewModel.IsOneShotEnabled = true;
            if (IsInvisibleChecked) _playerViewModel.IsInvisibleEnabled = true;
            if (IsSilentChecked) _playerViewModel.IsSilentEnabled = true;
            if (IsNoAmmoConsumeChecked) _playerViewModel.IsNoAmmoConsumeEnabled = true;
            if (IsInfinitePoiseChecked) _playerViewModel.IsInfinitePoiseEnabled = true;
            if (IsNoHitChecked) _playerViewModel.IsNoHitEnabled = true;
            if (IsHealOverTimeChecked) _playerViewModel.IsHotEnabled = true;
            if (IsFpRegenChecked) _playerViewModel.IsFpRegenEnabled = true;
            if (IsNoRollChecked) _playerViewModel.IsNoRollEnabled = true;
            if (IsAutoNewGameSevenChecked) _playerViewModel.IsAutoSetNewGameSevenEnabled = true;

            if (IsTargetOptionsChecked) _targetViewModel.IsTargetOptionsEnabled = true;

            if (IsUnlockFpsChecked)
            {
                _utilityViewModel.Fps = LaunchFps;
                _utilityViewModel.IsDbgFpsEnabled = true;
            }

            // Must be set here (State.Loaded) rather than on State.OnNewGameStart: ItemViewModel's own
            // OnNewGameStart handler reads AutoSpawnEnabled on that same event, and Loaded always fires
            // before OnNewGameStart within a single load, so this guarantees the flag is set in time.
            if (IsSpawnWeaponAtStartChecked) _itemViewModel.AutoSpawnEnabled = true;
        }

        private void OnNewGameStart()
        {
            if (!IsEnabled) return;

            if (IsUnlockBonfiresChecked) _travelViewModel.UnlockBonfiresCommand.Execute(null);
        }
    }
}
