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
        private readonly ActivateOnLaunchManager _aol;

        public ActivateOnLaunchViewModel(PlayerViewModel playerViewModel, TargetViewModel targetViewModel,
            UtilityViewModel utilityViewModel, TravelViewModel travelViewModel, ItemViewModel itemViewModel,
            ActivateOnLaunchManager activateOnLaunchManager, IStateService stateService)
        {
            _playerViewModel = playerViewModel;
            _targetViewModel = targetViewModel;
            _utilityViewModel = utilityViewModel;
            _travelViewModel = travelViewModel;
            _itemViewModel = itemViewModel;
            _aol = activateOnLaunchManager;

            RegisterActions();

            stateService.Subscribe(State.Loaded, OnGameLoaded);
            stateService.Subscribe(State.OnNewGameStart, OnNewGameStart);
        }

        #region Properties

        // Master toggle
        private bool _isEnabled = SettingsManager.Default.ActivateOnLaunchEnabled;

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

        // Helper macros
        private bool Get(string id) => _aol.GetBool(id);
        private void Set(string id, bool value) => _aol.SetBool(id, value);

        private bool _isNoDeathChecked;

        public bool IsNoDeathChecked
        {
            get => _isNoDeathChecked;
            set
            {
                if (SetProperty(ref _isNoDeathChecked, value)) Set(nameof(IsNoDeathChecked), value);
            }
        }

        private bool _isNoDamageChecked;

        public bool IsNoDamageChecked
        {
            get => _isNoDamageChecked;
            set
            {
                if (SetProperty(ref _isNoDamageChecked, value)) Set(nameof(IsNoDamageChecked), value);
            }
        }

        private bool _isInfiniteStaminaChecked;

        public bool IsInfiniteStaminaChecked
        {
            get => _isInfiniteStaminaChecked;
            set
            {
                if (SetProperty(ref _isInfiniteStaminaChecked, value)) Set(nameof(IsInfiniteStaminaChecked), value);
            }
        }

        private bool _isNoGoodsConsumeChecked;

        public bool IsNoGoodsConsumeChecked
        {
            get => _isNoGoodsConsumeChecked;
            set
            {
                if (SetProperty(ref _isNoGoodsConsumeChecked, value)) Set(nameof(IsNoGoodsConsumeChecked), value);
            }
        }

        private bool _isInfiniteFpChecked;

        public bool IsInfiniteFpChecked
        {
            get => _isInfiniteFpChecked;
            set
            {
                if (SetProperty(ref _isInfiniteFpChecked, value)) Set(nameof(IsInfiniteFpChecked), value);
            }
        }

        private bool _isInfiniteDurabilityChecked;

        public bool IsInfiniteDurabilityChecked
        {
            get => _isInfiniteDurabilityChecked;
            set
            {
                if (SetProperty(ref _isInfiniteDurabilityChecked, value)) Set(nameof(IsInfiniteDurabilityChecked), value);
            }
        }

        private bool _isOneShotChecked;

        public bool IsOneShotChecked
        {
            get => _isOneShotChecked;
            set
            {
                if (SetProperty(ref _isOneShotChecked, value)) Set(nameof(IsOneShotChecked), value);
            }
        }

        private bool _isInvisibleChecked;

        public bool IsInvisibleChecked
        {
            get => _isInvisibleChecked;
            set
            {
                if (SetProperty(ref _isInvisibleChecked, value)) Set(nameof(IsInvisibleChecked), value);
            }
        }

        private bool _isSilentChecked;

        public bool IsSilentChecked
        {
            get => _isSilentChecked;
            set
            {
                if (SetProperty(ref _isSilentChecked, value)) Set(nameof(IsSilentChecked), value);
            }
        }

        private bool _isNoAmmoConsumeChecked;

        public bool IsNoAmmoConsumeChecked
        {
            get => _isNoAmmoConsumeChecked;
            set
            {
                if (SetProperty(ref _isNoAmmoConsumeChecked, value)) Set(nameof(IsNoAmmoConsumeChecked), value);
            }
        }

        private bool _isInfinitePoiseChecked;

        public bool IsInfinitePoiseChecked
        {
            get => _isInfinitePoiseChecked;
            set
            {
                if (SetProperty(ref _isInfinitePoiseChecked, value)) Set(nameof(IsInfinitePoiseChecked), value);
            }
        }

        private bool _isNoHitChecked;

        public bool IsNoHitChecked
        {
            get => _isNoHitChecked;
            set
            {
                if (SetProperty(ref _isNoHitChecked, value)) Set(nameof(IsNoHitChecked), value);
            }
        }

        private bool _isHealOverTimeChecked;

        public bool IsHealOverTimeChecked
        {
            get => _isHealOverTimeChecked;
            set
            {
                if (SetProperty(ref _isHealOverTimeChecked, value)) Set(nameof(IsHealOverTimeChecked), value);
            }
        }

        private bool _isFpRegenChecked;

        public bool IsFpRegenChecked
        {
            get => _isFpRegenChecked;
            set
            {
                if (SetProperty(ref _isFpRegenChecked, value)) Set(nameof(IsFpRegenChecked), value);
            }
        }

        private bool _isNoRollChecked;

        public bool IsNoRollChecked
        {
            get => _isNoRollChecked;
            set
            {
                if (SetProperty(ref _isNoRollChecked, value)) Set(nameof(IsNoRollChecked), value);
            }
        }

        private bool _isAutoNewGameSevenChecked;

        public bool IsAutoNewGameSevenChecked
        {
            get => _isAutoNewGameSevenChecked;
            set
            {
                if (SetProperty(ref _isAutoNewGameSevenChecked, value)) Set(nameof(IsAutoNewGameSevenChecked), value);
            }
        }

        private bool _isTargetOptionsChecked;

        public bool IsTargetOptionsChecked
        {
            get => _isTargetOptionsChecked;
            set
            {
                if (SetProperty(ref _isTargetOptionsChecked, value)) Set(nameof(IsTargetOptionsChecked), value);
            }
        }

        private bool _isUnlockFpsChecked;

        public bool IsUnlockFpsChecked
        {
            get => _isUnlockFpsChecked;
            set
            {
                if (SetProperty(ref _isUnlockFpsChecked, value)) Set(nameof(IsUnlockFpsChecked), value);
            }
        }

        private int _launchFps;

        public int LaunchFps
        {
            get => _launchFps;
            set
            {
                if (SetProperty(ref _launchFps, value))
                    _aol.SetInt(nameof(LaunchFps), value);
            }
        }

        private bool _isUnlockBonfiresChecked;

        public bool IsUnlockBonfiresChecked
        {
            get => _isUnlockBonfiresChecked;
            set
            {
                if (SetProperty(ref _isUnlockBonfiresChecked, value)) Set(nameof(IsUnlockBonfiresChecked), value);
            }
        }

        private bool _isSpawnWeaponAtStartChecked;

        public bool IsSpawnWeaponAtStartChecked
        {
            get => _isSpawnWeaponAtStartChecked;
            set
            {
                if (SetProperty(ref _isSpawnWeaponAtStartChecked, value)) Set(nameof(IsSpawnWeaponAtStartChecked), value);
            }
        }

        #endregion

        private void RegisterActions()
        {
            _isNoDeathChecked = Get(nameof(IsNoDeathChecked));
            _isNoDamageChecked = Get(nameof(IsNoDamageChecked));
            _isInfiniteStaminaChecked = Get(nameof(IsInfiniteStaminaChecked));
            _isNoGoodsConsumeChecked = Get(nameof(IsNoGoodsConsumeChecked));
            _isInfiniteFpChecked = Get(nameof(IsInfiniteFpChecked));
            _isInfiniteDurabilityChecked = Get(nameof(IsInfiniteDurabilityChecked));
            _isOneShotChecked = Get(nameof(IsOneShotChecked));
            _isInvisibleChecked = Get(nameof(IsInvisibleChecked));
            _isSilentChecked = Get(nameof(IsSilentChecked));
            _isNoAmmoConsumeChecked = Get(nameof(IsNoAmmoConsumeChecked));
            _isInfinitePoiseChecked = Get(nameof(IsInfinitePoiseChecked));
            _isNoHitChecked = Get(nameof(IsNoHitChecked));
            _isHealOverTimeChecked = Get(nameof(IsHealOverTimeChecked));
            _isFpRegenChecked = Get(nameof(IsFpRegenChecked));
            _isNoRollChecked = Get(nameof(IsNoRollChecked));
            _isAutoNewGameSevenChecked = Get(nameof(IsAutoNewGameSevenChecked));
            _isTargetOptionsChecked = Get(nameof(IsTargetOptionsChecked));
            _isUnlockFpsChecked = Get(nameof(IsUnlockFpsChecked));
            _launchFps = _aol.GetInt(nameof(LaunchFps), defaultValue: 75);
            _isUnlockBonfiresChecked = Get(nameof(IsUnlockBonfiresChecked));
            _isSpawnWeaponAtStartChecked = Get(nameof(IsSpawnWeaponAtStartChecked));
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
