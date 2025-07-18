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
using System.Diagnostics;

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
                txtStatus.Text = $"✅ Converted {count} .fui files to .swf";
            }
        }

        // Convert .swf files to .fui
        private async void BtnConvertToFui_Click(object sender, RoutedEventArgs e)
        {
            if (Directory.Exists(selectedFolder))
            {
                int count = await RenameFilesAsync(selectedFolder, ".swf", ".fui");
                txtStatus.Text = $"✅ Converted {count} .swf files to .fui";
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
            progressTimer.Interval = TimeSpan.FromMilliseconds(02); // speed of progress update
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
                    logBox.ScrollToEnd();
                }
                else
                {
                    logBox.AppendText($"Skipped (already exists): {newPath}\n");
                    logBox.ScrollToEnd();
                }

                progressBar.Value = ((double)(i + 1) / totalFiles) * 100;
                await Task.Yield();
            }

            return renamedCount;
        }

        // Link click handler
        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
            e.Handled = true;
        }

        // About link click handler
        private void AboutLink_Click(object sender, RoutedEventArgs e)
        {
            var aboutWindow = new AboutWindow();
            aboutWindow.Owner = this;
            aboutWindow.ShowDialog();
        }


    }
}