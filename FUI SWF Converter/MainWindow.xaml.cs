using Ookii.Dialogs.Wpf;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Microsoft.Win32;

// FUI SWF Converter
// A simple tool to convert .fui files to .swf and vice versa.
// Originally designed to assist with UI file format conversions for Operation Flashpoint: Dragon Rising.

// © Noah Domingues. Last updated: 17 July 2025.

// See LICENSE file in the project root for full license information.

namespace FUI_SWF_Converter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string selectedFolder = "";
        private DispatcherTimer progressTimer;
        private double targetProgress = 0;
        private double visualProgress = 0;


        public MainWindow()
        {
            InitializeComponent();
            bool isDark = IsSystemInDarkMode();
            themeToggle.IsChecked = isDark;
            LoadTheme(isDark);
        }

        // Select folder button click event
        private void BtnSelectFolder_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new VistaFolderBrowserDialog();
            if (dialog.ShowDialog() == true)
            {
                selectedFolder = dialog.SelectedPath;
                txtSelectedFolder.Text = selectedFolder;
            }
        }

        // Convert .fui files to .swf
        private async void BtnConvertToSwf_Click(object sender, RoutedEventArgs e)
        {
            if (Directory.Exists(selectedFolder))
            {
                int count = await RenameFilesAsync(selectedFolder, ".fui", ".swf");
                txtStatus.Text = $"✅ Renamed {count} .fui files to .swf";
            }
        }

        // Convert .swf files to .fui
        private async void BtnConvertToFui_Click(object sender, RoutedEventArgs e)
        {
            if (Directory.Exists(selectedFolder))
            {
                int count = await RenameFilesAsync(selectedFolder, ".swf", ".fui");
                txtStatus.Text = $"✅ Renamed {count} .swf files to .fui";
            }
        }

        // Rename file operation with progress reporting
        private async Task<int> RenameFilesAsync(string rootPath, string fromExt, string toExt)
        {
            var files = Directory.GetFiles(rootPath, $"*{fromExt}", SearchOption.AllDirectories);
            int totalFiles = files.Length;
            int renamedCount = 0;

            progressBar.Value = 0;
            logBox.Clear();

            visualProgress = 0;
            targetProgress = 0;
            progressBar.Value = 0;

            progressTimer = new DispatcherTimer();
            progressTimer.Interval = TimeSpan.FromMilliseconds(10); // speed of progress update
            progressTimer.Tick += (s, e) =>
            {
                if (visualProgress < targetProgress)
                {
                    visualProgress += 1;
                    progressBar.Value = visualProgress;
                }
                else if (visualProgress > targetProgress)
                {
                    visualProgress = targetProgress;
                    progressBar.Value = visualProgress;
                }
            };
            progressTimer.Start();


            for (int i = 0; i < totalFiles; i++)
            {
                targetProgress = ((double)(i + 1) / totalFiles) * 100;
                string file = files[i];
                string newPath = System.IO.Path.ChangeExtension(file, toExt);

                if (!File.Exists(newPath))
                {
                    File.Move(file, newPath);
                    renamedCount++;
                    logBox.AppendText($"Renamed {file} → {newPath}\n");
                }
                else
                {
                    logBox.AppendText($"Skipped (already exists): {newPath}\n");
                }

                progressBar.Value = ((double)(i + 1) / totalFiles) * 100;
                await Task.Yield();  // let UI breathe; otherwise it won't update live
            }

            return renamedCount;
        }

        // Detect system theme and set UI accordingly
        private bool IsSystemInDarkMode()
        {
            const string registryKey = @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(registryKey))
            {
                object value = key?.GetValue("AppsUseLightTheme");
                return value is int intValue && intValue == 0;
            }
        }

        // Window loaded event to set initial theme
        private void LoadTheme(bool useDark)
        {
            var themePath = useDark ? "Themes/DarkTheme.xaml" : "Themes/LightTheme.xaml";
            var dict = new ResourceDictionary { Source = new Uri(themePath, UriKind.Relative) };

            System.Windows.Application.Current.Resources.MergedDictionaries.Clear();
            System.Windows.Application.Current.Resources.MergedDictionaries.Add(dict);
        }

        // Theme toggle event handlers
        private void ThemeToggle_Checked(object sender, RoutedEventArgs e)
        {
            LoadTheme(true);
        }

        private void ThemeToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            LoadTheme(false);
        }

    }
}