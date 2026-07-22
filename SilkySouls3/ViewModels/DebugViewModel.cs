using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;
using System.Windows.Media;
using SilkySouls3.Core;
using SilkySouls3.Enums;
using SilkySouls3.Interfaces;

namespace SilkySouls3.ViewModels
{
    public class DebugViewModel : BaseViewModel
    {
        private readonly IPlayerService _playerService;
        private readonly IEventService _eventService;
        private readonly IGameTickService _gameTickService;

        private const int MaxFlagScanRange = 2000;

        private static readonly string ExportPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "SilkySouls3",
            "boss_revive_export.csv");

        private static readonly string FlagDiffExportPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "SilkySouls3",
            "boss_revive_flag_diff.csv");

        private Dictionary<int, bool> _flagSnapshot;

        public DebugViewModel(IPlayerService playerService, IEventService eventService,
            IGameTickService gameTickService, IStateService stateService)
        {
            _playerService = playerService;
            _eventService = eventService;
            _gameTickService = gameTickService;

            CheckEventCommand = new DelegateCommand(CheckEvent);
            ExportCommand = new DelegateCommand(Export);
            SnapshotBeforeCommand = new DelegateCommand(SnapshotBefore);
            SnapshotAfterCommand = new DelegateCommand(SnapshotAfterAndDiff);

            stateService.Subscribe(State.Loaded, OnGameLoaded);
            stateService.Subscribe(State.NotLoaded, OnGameNotLoaded);
        }

        #region Commands

        public ICommand CheckEventCommand { get; }
        public ICommand ExportCommand { get; }
        public ICommand SnapshotBeforeCommand { get; }
        public ICommand SnapshotAfterCommand { get; }

        #endregion

        #region Properties

        private int _currentBlockId;

        public int CurrentBlockId
        {
            get => _currentBlockId;
            set => SetProperty(ref _currentBlockId, value);
        }

        private string _eventFlagId;

        public string EventFlagId
        {
            get => _eventFlagId;
            set => SetProperty(ref _eventFlagId, value);
        }

        private string _eventFlagStatusText;

        public string EventFlagStatusText
        {
            get => _eventFlagStatusText;
            set => SetProperty(ref _eventFlagStatusText, value);
        }

        private Brush _eventFlagStatusColor;

        public Brush EventFlagStatusColor
        {
            get => _eventFlagStatusColor;
            set => SetProperty(ref _eventFlagStatusColor, value);
        }

        private bool _isDrawEventsEnabled;

        public bool IsDrawEventsEnabled
        {
            get => _isDrawEventsEnabled;
            set
            {
                if (!SetProperty(ref _isDrawEventsEnabled, value)) return;
                _eventService.ToggleDrawEvents(_isDrawEventsEnabled);
            }
        }

        private string _bossName;

        public string BossName
        {
            get => _bossName;
            set => SetProperty(ref _bossName, value);
        }

        private string _exportStatusText;

        public string ExportStatusText
        {
            get => _exportStatusText;
            set => SetProperty(ref _exportStatusText, value);
        }

        private string _flagRangeFrom;

        public string FlagRangeFrom
        {
            get => _flagRangeFrom;
            set => SetProperty(ref _flagRangeFrom, value);
        }

        private string _flagRangeTo;

        public string FlagRangeTo
        {
            get => _flagRangeTo;
            set => SetProperty(ref _flagRangeTo, value);
        }

        private string _flagScanStatusText;

        public string FlagScanStatusText
        {
            get => _flagScanStatusText;
            set => SetProperty(ref _flagScanStatusText, value);
        }

        public ObservableCollection<string> FlagDiffResults { get; } = new ObservableCollection<string>();

        #endregion

        #region Private Methods

        private void OnGameLoaded()
        {
            _gameTickService.Subscribe(Tick);
            if (IsDrawEventsEnabled) _eventService.ToggleDrawEvents(true);
        }

        private void OnGameNotLoaded()
        {
            _gameTickService.Unsubscribe(Tick);
            CurrentBlockId = 0;
        }

        private void Tick() => CurrentBlockId = _playerService.GetCurrentBlockId();

        private void CheckEvent()
        {
            if (string.IsNullOrWhiteSpace(EventFlagId))
                return;

            if (!int.TryParse(EventFlagId.Trim(), out int flagIdValue) || flagIdValue <= 0)
                return;

            if (_eventService.GetEvent(flagIdValue))
            {
                EventFlagStatusText = "True";
                EventFlagStatusColor = Brushes.Chartreuse;
            }
            else
            {
                EventFlagStatusText = "False";
                EventFlagStatusColor = Brushes.Red;
            }
        }

        private void Export()
        {
            try
            {
                var directory = Path.GetDirectoryName(ExportPath);
                if (!string.IsNullOrEmpty(directory)) Directory.CreateDirectory(directory);

                var isNewFile = !File.Exists(ExportPath);
                using (var writer = new StreamWriter(ExportPath, append: true))
                {
                    if (isNewFile) writer.WriteLine("BossName,EventFlagId,BlockId");

                    var bossName = string.IsNullOrWhiteSpace(BossName) ? "" : BossName.Trim().Replace(",", " ");
                    writer.WriteLine($"{bossName},{EventFlagId},{CurrentBlockId}");
                }

                ExportStatusText = $"Exported to {ExportPath}";
            }
            catch (Exception ex)
            {
                ExportStatusText = $"Export failed: {ex.Message}";
            }
        }

        private bool TryParseFlagRange(out int from, out int to)
        {
            from = to = 0;

            if (!int.TryParse(FlagRangeFrom?.Trim(), out from) || !int.TryParse(FlagRangeTo?.Trim(), out to))
            {
                FlagScanStatusText = "Enter valid numeric From/To event IDs.";
                return false;
            }

            if (from > to)
            {
                FlagScanStatusText = "From must be less than or equal to To.";
                return false;
            }

            if (to - from + 1 > MaxFlagScanRange)
            {
                FlagScanStatusText = $"Range too large - max {MaxFlagScanRange} IDs per scan.";
                return false;
            }

            return true;
        }

        private Dictionary<int, bool> ScanFlagRange(int from, int to)
        {
            var results = new Dictionary<int, bool>();
            for (var id = from; id <= to; id++)
            {
                results[id] = _eventService.GetEvent(id);
            }

            return results;
        }

        private void SnapshotBefore()
        {
            if (!TryParseFlagRange(out var from, out var to)) return;

            FlagDiffResults.Clear();
            _flagSnapshot = ScanFlagRange(from, to);
            FlagScanStatusText =
                $"Snapshot captured: {_flagSnapshot.Count} flags ({from}-{to}). Defeat the boss, then click Snapshot After & Save Diff.";
        }

        private void SnapshotAfterAndDiff()
        {
            if (_flagSnapshot == null)
            {
                FlagScanStatusText = "Take a Snapshot Before first.";
                return;
            }

            if (!TryParseFlagRange(out var from, out var to)) return;

            var after = ScanFlagRange(from, to);
            var changed = new List<(int Id, bool OldValue, bool NewValue)>();

            foreach (var kvp in after)
            {
                if (_flagSnapshot.TryGetValue(kvp.Key, out var oldValue) && oldValue != kvp.Value)
                    changed.Add((kvp.Key, oldValue, kvp.Value));
            }

            FlagDiffResults.Clear();
            foreach (var change in changed)
                FlagDiffResults.Add($"{change.Id}: {change.OldValue} -> {change.NewValue}");

            _flagSnapshot = null;

            if (changed.Count == 0)
            {
                FlagScanStatusText = "No flags changed in this range.";
                return;
            }

            try
            {
                SaveFlagDiff(changed);
                FlagScanStatusText = $"{changed.Count} flag(s) changed - saved to {FlagDiffExportPath}";
            }
            catch (Exception ex)
            {
                FlagScanStatusText = $"{changed.Count} flag(s) changed, but save failed: {ex.Message}";
            }
        }

        private void SaveFlagDiff(List<(int Id, bool OldValue, bool NewValue)> changed)
        {
            var directory = Path.GetDirectoryName(FlagDiffExportPath);
            if (!string.IsNullOrEmpty(directory)) Directory.CreateDirectory(directory);

            var isNewFile = !File.Exists(FlagDiffExportPath);
            using (var writer = new StreamWriter(FlagDiffExportPath, append: true))
            {
                if (isNewFile) writer.WriteLine("BossName,EventFlagId,OldValue,NewValue,BlockId");

                var bossName = string.IsNullOrWhiteSpace(BossName) ? "" : BossName.Trim().Replace(",", " ");
                foreach (var change in changed)
                    writer.WriteLine($"{bossName},{change.Id},{change.OldValue},{change.NewValue},{CurrentBlockId}");
            }
        }

        #endregion
    }
}
