using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace SilkySouls3.Utilities
{
    public class SettingsManager
    {
        private static SettingsManager _default;
        public static SettingsManager Default => _default ?? (_default = Load());

        public string HotkeyActionIds { get; set; } = "";
        public string HotkeyValues { get; set; } = "";
        public bool EnableHotkeys { get; set; }
        public bool StutterFix { get; set; }
        public bool AlwaysOnTop { get; set; }
        public bool DisableMenuMusic { get; set; }
        public bool HotkeyReminder { get; set; }
        public bool DefaultSoundChangeEnabled { get; set; }
        [DefaultValue(3)] public int DefaultSoundVolume { get; set; }
        public double WindowLeft { get; set; }
        public double WindowTop { get; set; }
        [DefaultValue(1.0)] public double ResistancesWindowScaleX { get; set; }
        [DefaultValue(1.0)] public double ResistancesWindowScaleY { get; set; }
        [DefaultValue(0.5)] public double ResistancesWindowOpacity { get; set; }
        public double ResistancesWindowWidth { get; set; }
        public double ResistancesWindowLeft { get; set; }
        public double ResistancesWindowTop { get; set; }
        public bool ResistancesShowCombatInfo { get; set; }
        [DefaultValue(1.0)] public double DefensesWindowScaleX { get; set; }
        [DefaultValue(1.0)] public double DefensesWindowScaleY { get; set; }
        [DefaultValue(0.5)] public double DefensesWindowOpacity { get; set; }
        public double DefensesWindowWidth { get; set; }
        public double DefensesWindowLeft { get; set; }
        public double DefensesWindowTop { get; set; }
        [DefaultValue(true)] public bool EnableUpdateChecks { get; set; }
        public bool RememberPlayerSpeed { get; set; }
        [DefaultValue(1f)] public float PlayerSpeed { get; set; }
        public bool RememberGameSpeed { get; set; }
        [DefaultValue(1f)] public float GameSpeed { get; set; }
        public string SaveCustomHp { get; set; } = "";
        public double SpEffectWindowLeft { get; set; }
        public double SpEffectWindowTop { get; set; }
        public bool SpEffectAlwaysOnTop { get; set; }

        public bool ActivateOnLaunchEnabled { get; set; }
        public bool ActivateOnLaunchNoDeath { get; set; }
        public bool ActivateOnLaunchNoDamage { get; set; }
        public bool ActivateOnLaunchInfiniteStamina { get; set; }
        public bool ActivateOnLaunchNoGoodsConsume { get; set; }
        public bool ActivateOnLaunchInfiniteFp { get; set; }
        public bool ActivateOnLaunchInfiniteDurability { get; set; }
        public bool ActivateOnLaunchOneShot { get; set; }
        public bool ActivateOnLaunchInvisible { get; set; }
        public bool ActivateOnLaunchSilent { get; set; }
        public bool ActivateOnLaunchNoAmmoConsume { get; set; }
        public bool ActivateOnLaunchInfinitePoise { get; set; }
        public bool ActivateOnLaunchNoHit { get; set; }
        public bool ActivateOnLaunchHealOverTime { get; set; }
        public bool ActivateOnLaunchFpRegen { get; set; }
        public bool ActivateOnLaunchNoRoll { get; set; }
        public bool ActivateOnLaunchAutoNewGameSeven { get; set; }
        public bool ActivateOnLaunchTargetOptions { get; set; }
        public bool ActivateOnLaunchUnlockFps { get; set; }
        [DefaultValue(75)] public int ActivateOnLaunchFps { get; set; }
        public bool ActivateOnLaunchUnlockBonfires { get; set; }
        public bool ActivateOnLaunchSpawnWeaponAtStart { get; set; }

        private static string SettingsPath => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "SilkySouls3",
            "settings.json");

        public void Save()
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(SettingsPath)!);
                var lines = new List<string>();

                foreach (var prop in GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    var value = prop.GetValue(this);
                    var stringValue = value switch
                    {
                        double d => d.ToString(CultureInfo.InvariantCulture),
                        float f => f.ToString(CultureInfo.InvariantCulture),
                        _ => value?.ToString() ?? ""
                    };
                    lines.Add($"{prop.Name}={stringValue}");
                }

                File.WriteAllLines(SettingsPath, lines);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving settings: {ex.Message}");
            }
        }

        private static SettingsManager Load()
        {
            var settings = new SettingsManager();

            foreach (var prop in typeof(SettingsManager).GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var defaultAttr = prop.GetCustomAttribute<DefaultValueAttribute>();
                if (defaultAttr != null)
                    prop.SetValue(settings, defaultAttr.Value);
            }

            if (!File.Exists(SettingsPath))
                return settings;

            try
            {
                var props = new Dictionary<string, PropertyInfo>();
                foreach (var prop in typeof(SettingsManager).GetProperties(BindingFlags.Public | BindingFlags.Instance))
                    props[prop.Name] = prop;

                foreach (var line in File.ReadAllLines(SettingsPath))
                {
                    var parts = line.Split(new[] { '=' }, 2);
                    if (parts.Length != 2) continue;

                    var key = parts[0];
                    var value = parts[1];

                    if (!props.TryGetValue(key, out var prop)) continue;

                    object parsed = prop.PropertyType switch
                    {
                        { } t when t == typeof(byte) =>
                            byte.TryParse(value, out var by) ? by : (byte)0,
                        { } t when t == typeof(double) =>
                            double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var d) ? d : 0.0,
                        { } t when t == typeof(float) =>
                            float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var f) ? f : 0f,
                        { } t when t == typeof(int) =>
                            int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var i) ? i : 0,
                        { } t when t == typeof(bool) =>
                            bool.TryParse(value, out var b) && b,
                        { } t when t == typeof(string) => value,
                        _ => null
                    };

                    if (parsed != null)
                        prop.SetValue(settings, parsed);
                }
            }
            catch
            {
                // Return default settings on error
            }

            return settings;
        }
    }
}
