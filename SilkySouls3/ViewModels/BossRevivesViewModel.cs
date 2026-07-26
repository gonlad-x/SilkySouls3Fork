using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using SilkySouls3.Core;
using SilkySouls3.Enums;
using SilkySouls3.Interfaces;
using SilkySouls3.Models;
using SilkySouls3.Utilities;

namespace SilkySouls3.ViewModels
{
    public class BossRevivesViewModel : BaseViewModel
    {
        private readonly IEventService _eventService;
        private readonly ITravelService _travelService;
        private readonly IPlayerService _playerService;
        private readonly IDlcService _dlcService;
        private readonly IItemService _itemService;
        private readonly IStateService _stateService;

        private const string BossStatusDead = "Dead";
        private const string BossStatusAlive = "Alive";
        private const string BossStatusFirstEncounter = "Alive, First Encounter";

        private static readonly SolidColorBrush BossStatusDeadBrush =
            (SolidColorBrush)new BrushConverter().ConvertFrom("#e74c3c");

        private static readonly SolidColorBrush BossStatusAliveBrush =
            (SolidColorBrush)new BrushConverter().ConvertFrom("#2ecc71");

        private static readonly SolidColorBrush BossStatusDefaultBrush = Brushes.White;

        private Dictionary<string, List<BossRevive>> _bossDict;
        private List<BossRevive> _allBosses;

        private string _preSearchArea;

        public BossRevivesViewModel(IEventService eventService, ITravelService travelService,
            IPlayerService playerService, IDlcService dlcService, IStateService stateService,
            IItemService itemService)
        {
            _eventService = eventService;
            _travelService = travelService;
            _playerService = playerService;
            _dlcService = dlcService;
            _itemService = itemService;
            _stateService = stateService;

            ReviveBossCommand = new DelegateCommand(ReviveBoss);
            ReviveBossFirstEncounterCommand = new DelegateCommand(ReviveBossFirstEncounter);

            stateService.Subscribe(State.Loaded, OnGameLoaded);
            stateService.Subscribe(State.NotLoaded, OnGameNotLoaded);

            LoadBosses();
        }

        #region Commands

        public ICommand ReviveBossCommand { get; }
        public ICommand ReviveBossFirstEncounterCommand { get; }

        #endregion

        #region Properties

        private bool _areOptionsEnabled;

        public bool AreOptionsEnabled
        {
            get => _areOptionsEnabled;
            set => SetProperty(ref _areOptionsEnabled, value);
        }

        private ObservableCollection<string> _areas = new();

        public ObservableCollection<string> Areas
        {
            get => _areas;
            private set => SetProperty(ref _areas, value);
        }

        private ObservableCollection<BossRevive> _areaBosses = new();

        public ObservableCollection<BossRevive> AreaBosses
        {
            get => _areaBosses;
            private set => SetProperty(ref _areaBosses, value);
        }

        private string _selectedArea;

        public string SelectedArea
        {
            get => _selectedArea;
            set
            {
                if (!SetProperty(ref _selectedArea, value) || value == null) return;

                if (IsSearchActive)
                {
                    IsSearchActive = false;
                    _searchText = string.Empty;
                    OnPropertyChanged(nameof(SearchText));
                    _preSearchArea = null;
                }

                UpdateAreaBossesList();
            }
        }

        private BossRevive _selectedBoss;

        public BossRevive SelectedBoss
        {
            get => _selectedBoss;
            set
            {
                if (!SetProperty(ref _selectedBoss, value)) return;
                RefreshSelectedBossStatus();
            }
        }

        private bool _isSearchActive;

        public bool IsSearchActive
        {
            get => _isSearchActive;
            private set => SetProperty(ref _isSearchActive, value);
        }

        private string _searchText = string.Empty;

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (!SetProperty(ref _searchText, value)) return;

                if (string.IsNullOrEmpty(value))
                {
                    IsSearchActive = false;
                    if (_preSearchArea != null)
                    {
                        _selectedArea = _preSearchArea;
                        OnPropertyChanged(nameof(SelectedArea));
                        UpdateAreaBossesList();
                        _preSearchArea = null;
                    }
                }
                else
                {
                    if (!IsSearchActive)
                    {
                        _preSearchArea = SelectedArea;
                        IsSearchActive = true;
                    }

                    ApplyFilter();
                }
            }
        }

        private bool _isRestOnReviveEnabled;

        public bool IsRestOnReviveEnabled
        {
            get => _isRestOnReviveEnabled;
            set => SetProperty(ref _isRestOnReviveEnabled, value);
        }

        private string _selectedBossStatus = string.Empty;

        public string SelectedBossStatus
        {
            get => _selectedBossStatus;
            private set => SetProperty(ref _selectedBossStatus, value);
        }

        private SolidColorBrush _selectedBossStatusColor = BossStatusDefaultBrush;

        public SolidColorBrush SelectedBossStatusColor
        {
            get => _selectedBossStatusColor;
            private set => SetProperty(ref _selectedBossStatusColor, value);
        }

        #endregion

        #region Private Methods

        private void LoadBosses()
        {
            _bossDict = DataLoader.GetBossRevives();
            _allBosses = _bossDict.Values.SelectMany(x => x).ToList();

            Areas = new ObservableCollection<string>(_bossDict.Keys);
            SelectedArea = Areas.FirstOrDefault();
        }

        private void UpdateAreaBossesList()
        {
            if (string.IsNullOrEmpty(SelectedArea) || !_bossDict.ContainsKey(SelectedArea))
            {
                AreaBosses = new ObservableCollection<BossRevive>();
                return;
            }

            AreaBosses = new ObservableCollection<BossRevive>(_bossDict[SelectedArea]);
            SelectedBoss = AreaBosses.FirstOrDefault();
        }

        private void ApplyFilter()
        {
            var searchLower = SearchText.ToLower();
            var matches = _allBosses.Where(b =>
                b.BossName.ToLower().Contains(searchLower) ||
                b.Area.ToLower().Contains(searchLower));

            AreaBosses = new ObservableCollection<BossRevive>(matches);
            SelectedBoss = AreaBosses.FirstOrDefault();
        }

        private void RefreshSelectedBossStatus()
        {
            if (!AreOptionsEnabled || SelectedBoss == null)
            {
                SelectedBossStatus = string.Empty;
                SelectedBossStatusColor = BossStatusDefaultBrush;
                return;
            }

            SelectedBossStatus = GetBossStatus(SelectedBoss);
            SelectedBossStatusColor = GetBossStatusColor(SelectedBossStatus);
        }

        private string GetBossStatus(BossRevive bossRevive)
        {
            if (bossRevive.BossFlags == null || bossRevive.BossFlags.Count == 0) return string.Empty;

            var isDead = _eventService.GetEvent(bossRevive.BossFlags[0].EventId);
            if (isDead) return BossStatusDead;

            if (bossRevive.FirstEncounterFlags is { Count: > 0 })
            {
                var alreadyEncountered = bossRevive.FirstEncounterFlags.All(f => _eventService.GetEvent(f.EventId));
                return alreadyEncountered ? BossStatusAlive : BossStatusFirstEncounter;
            }

            return BossStatusAlive;
        }

        private static SolidColorBrush GetBossStatusColor(string status) => status switch
        {
            BossStatusDead => BossStatusDeadBrush,
            BossStatusAlive => BossStatusAliveBrush,
            BossStatusFirstEncounter => BossStatusAliveBrush,
            _ => BossStatusDefaultBrush
        };

        private void SetBossFlags(BossRevive bossRevive, bool isFirstEncounter)
        {
            if (isFirstEncounter && bossRevive.FirstEncounterFlags != null)
            {
                foreach (var flag in bossRevive.FirstEncounterFlags)
                    _eventService.SetEvent(flag.EventId, flag.Value);
            }

            foreach (var flag in bossRevive.BossFlags)
                _eventService.SetEvent(flag.EventId, flag.Value);
        }

        private void ReviveBoss() => Revive(isFirstEncounter: false);

        private void ReviveBossFirstEncounter() => Revive(isFirstEncounter: true);

        private void Revive(bool isFirstEncounter)
        {
            var bossRevive = SelectedBoss;
            if (bossRevive == null || !_dlcService.MeetsRequirement(bossRevive.DlcRequirement)) return;

            SetBossFlags(bossRevive, isFirstEncounter);
            RefreshSelectedBossStatus();

            if (isFirstEncounter && bossRevive.FirstEncounterItemId.HasValue)
                _itemService.SpawnItem(bossRevive.FirstEncounterItemId.Value, 1, true, 1);

            bool isInBossArea = _playerService.GetCurrentBlockId() == bossRevive.BlockId;
            if (!isInBossArea && !IsRestOnReviveEnabled) return;

            _ = Task.Run(() =>
            {
                if (isInBossArea)
                {
                    if (bossRevive.Coords.HasValue)
                        _travelService.WarpWithCoords(bossRevive.Coords.Value, bossRevive.Angle,
                            bossRevive.BonfireId);
                    else
                        _travelService.Warp(bossRevive.BonfireId);
                }

                if (IsRestOnReviveEnabled)
                {
                    if (isInBossArea)
                    {
                        int start = Environment.TickCount;
                        while (!_stateService.IsFadedIn() && Environment.TickCount - start < 10000)
                            Thread.Sleep(50);
                    }

                    _playerService.Rest();
                }
            });
        }

        private void OnGameLoaded()
        {
            AreOptionsEnabled = true;
            RefreshSelectedBossStatus();
        }

        private void OnGameNotLoaded()
        {
            AreOptionsEnabled = false;
            SelectedBossStatus = string.Empty;
            SelectedBossStatusColor = BossStatusDefaultBrush;
        }

        #endregion
    }
}
