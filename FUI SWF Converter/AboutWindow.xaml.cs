using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FUI_SWF_Converter
{
    /// <summary>
    /// Interaction logic for AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow : Window
    {
        public AboutWindow()
        {
            InitializeComponent();
        }

        // Click event handlers for social media icons
        private void DiscordIcon_Click(object sender, MouseButtonEventArgs e)
        {
            Process.Start(new ProcessStartInfo("https://discord.gg/Z88NnTgpWU") { UseShellExecute = true });
        }

        private void YouTubeIcon_Click(object sender, MouseButtonEventArgs e)
        {
            Process.Start(new ProcessStartInfo("https://www.youtube.com/@NoahDomingues?sub_confirmation=1") { UseShellExecute = true });
        }

        private void GitHubIcon_Click(object sender, MouseButtonEventArgs e)
        {
            Process.Start(new ProcessStartInfo("https://github.com/NoahDomingues/FUI-SWF-Converter") { UseShellExecute = true });
        }
        private void Logo_Click(object sender, MouseButtonEventArgs e)
        {
            Process.Start(new ProcessStartInfo("https://discord.gg/Z88NnTgpWU") { UseShellExecute = true });
        }


    }
}
