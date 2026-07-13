using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SilkySouls3.Utilities
{
    public static class VersionChecker
    {
        public static async Task<(bool hasUpdate, Version currentVersion, Version webVersion, string downloadUrl)>
            CheckForUpdate()
        {
            try
            {
                var currentVersion = Assembly.GetEntryAssembly()?.GetName().Version;
                if (currentVersion == null) return (false, null, null, null);

                var client = new HttpClient();
                client.DefaultRequestHeaders.UserAgent.Add(
                    new ProductInfoHeaderValue("SilkySouls3", currentVersion.ToString()));

                var response = await client.GetStringAsync(
                    "https://api.github.com/repos/borgCode/SilkySouls3/releases/latest");

                using var doc = JsonDocument.Parse(response);
                var root = doc.RootElement;

                if (!root.TryGetProperty("tag_name", out var tagNameElement)) return (false, currentVersion, null, null);
                var tagName = tagNameElement.GetString();
                if (string.IsNullOrEmpty(tagName)) return (false, currentVersion, null, null);

                var webVersion = new Version(tagName.TrimStart('v'));

                string downloadUrl = null;
                if (root.TryGetProperty("assets", out var assets))
                {
                    foreach (var asset in assets.EnumerateArray())
                    {
                        var name = asset.GetProperty("name").GetString();
                        if (name != null && name.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
                        {
                            downloadUrl = asset.GetProperty("browser_download_url").GetString();
                            break;
                        }
                    }
                }

                return (webVersion > currentVersion, currentVersion, webVersion, downloadUrl);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Update check failed: {ex.Message}");
                return (false, null, null, null);
            }
        }

        public static async void CheckForUpdates(Window parentWindow, bool showNoUpdateMessage = false)
        {
            var (hasUpdate, currentVersion, webVersion, downloadUrl) = await CheckForUpdate();

            if (!hasUpdate || webVersion == null || currentVersion == null)
            {
                if (showNoUpdateMessage)
                {
                    MessageBox.Show(
                        "Your application is up to date.",
                        "Update Check",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
                return;
            }
            
            var updateWindow = new Window
            {
                Title = "Update Available",
                Width = 300,
                Height = 200,
                WindowStyle = WindowStyle.None,
                ResizeMode = ResizeMode.NoResize,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = parentWindow,
                Background = (SolidColorBrush)Application.Current.Resources["BackgroundBrush"],
                BorderBrush = (SolidColorBrush)Application.Current.Resources["BorderBrush"],
                BorderThickness = new Thickness(1)
            };

            var grid = new Grid();
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(30) });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(50) });

            var titleBar = new Border
            {
                Background = (SolidColorBrush)Application.Current.Resources["TitleBarBrush"],
                Child = new TextBlock
                {
                    Text = "Update Available",
                    Foreground = (SolidColorBrush)Application.Current.Resources["TextBrush"],
                    Margin = new Thickness(10, 5, 0, 0)
                }
            };
            Grid.SetRow(titleBar, 0);
            grid.Children.Add(titleBar);

            var message = new TextBlock
            {
                Text = $"A new version (v{webVersion.Major}.{webVersion.Minor}.{webVersion.Build}) is available!\nCurrent version: v{currentVersion.Major}.{currentVersion.Minor}.{currentVersion.Build}",
                Foreground = (SolidColorBrush)Application.Current.Resources["TextBrush"],
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(20, 10, 20, 10),
                VerticalAlignment = VerticalAlignment.Center
            };
            Grid.SetRow(message, 1);
            grid.Children.Add(message);
            
            var dontShowCheckbox = new CheckBox
            {
                Content = "Don't show on app launch",
                Foreground = (SolidColorBrush)Application.Current.Resources["TextBrush"],
                Margin = new Thickness(20, 5, 20, 0),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Bottom,
                IsChecked = !SettingsManager.Default.EnableUpdateChecks
            };
            Grid.SetRow(dontShowCheckbox, 1);
            grid.Children.Add(dontShowCheckbox);

            
            var buttonPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 10, 0, 0)
            };

            var downloadButton = new Button
            {
                Content = "Update Now",
                Width = 90,
                Height = 25,
                Margin = new Thickness(5)
            };

            var laterButton = new Button
            {
                Content = "Later",
                Width = 80,
                Height = 25,
                Margin = new Thickness(5)
            };

            downloadButton.Click += async (s, e) =>
            {
                if (dontShowCheckbox.IsChecked == true)
                {
                    SettingsManager.Default.EnableUpdateChecks = false;
                    SettingsManager.Default.Save();
                }

                if (string.IsNullOrEmpty(downloadUrl))
                {
                    MessageBox.Show(
                        "Could not find a downloadable release asset. Opening the releases page instead.",
                        "Update Failed",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = "https://github.com/borgCode/SilkySouls3/releases/latest",
                        UseShellExecute = true
                    });
                    updateWindow.Close();
                    return;
                }

                downloadButton.IsEnabled = false;
                laterButton.IsEnabled = false;
                message.Text = "Downloading update...";

                try
                {
                    await ApplyUpdate(downloadUrl);
                    Application.Current.Shutdown();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"Auto-update failed: {ex.Message}",
                        "Update Failed",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    downloadButton.IsEnabled = true;
                    laterButton.IsEnabled = true;
                    message.Text = $"A new version (v{webVersion.Major}.{webVersion.Minor}.{webVersion.Build}) is available!\nCurrent version: v{currentVersion.Major}.{currentVersion.Minor}.{currentVersion.Build}";
                }
            };

            laterButton.Click += (s, e) =>
            {
                if (dontShowCheckbox.IsChecked == true)
                {
                    SettingsManager.Default.EnableUpdateChecks = false;
                    SettingsManager.Default.Save();
                }
    
                updateWindow.Close();
            };

            buttonPanel.Children.Add(downloadButton);
            buttonPanel.Children.Add(laterButton);
            grid.Children.Add(buttonPanel);
            Grid.SetRow(buttonPanel, 2);

            updateWindow.Content = grid;

            titleBar.MouseLeftButtonDown += (s, e) => updateWindow.DragMove();

            updateWindow.ShowDialog();
        }

        private static async Task ApplyUpdate(string downloadUrl)
        {
            var currentExePath = Process.GetCurrentProcess().MainModule.FileName;
            var exeDirectory = Path.GetDirectoryName(currentExePath);
            var newExePath = Path.Combine(exeDirectory, "SilkySouls3.update.exe");

            byte[] bytes;
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("SilkySouls3", "1.0"));
                bytes = await client.GetByteArrayAsync(downloadUrl);
            }

            File.WriteAllBytes(newExePath, bytes);

            // The running exe can't be replaced while this process holds it open, so a throwaway
            // batch script waits for the file lock to clear (i.e. this process to exit) before swapping in
            // the new build and relaunching it.
            var scriptPath = Path.Combine(Path.GetTempPath(), "SilkySouls3Update_" + Guid.NewGuid().ToString("N") + ".bat");
            var script = "@echo off\r\n" +
                         ":retry\r\n" +
                         "move /y \"" + newExePath + "\" \"" + currentExePath + "\" >nul 2>&1\r\n" +
                         "if errorlevel 1 (\r\n" +
                         "    timeout /t 1 /nobreak >nul\r\n" +
                         "    goto retry\r\n" +
                         ")\r\n" +
                         "start \"\" \"" + currentExePath + "\"\r\n" +
                         "del \"%~f0\"\r\n";
            File.WriteAllText(scriptPath, script);

            Process.Start(new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = "/c \"" + scriptPath + "\"",
                UseShellExecute = false,
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden
            });
        }

        public static void UpdateVersionText(TextBlock appVersion)
        {
            var currentVersion = Assembly.GetEntryAssembly()?.GetName().Version;
            if (currentVersion != null)
            {
                appVersion.Text = $"v{currentVersion.Major}.{currentVersion.Minor}.{currentVersion.Build}";
            }
        }
    }
}