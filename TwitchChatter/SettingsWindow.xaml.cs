using System;
using System.Windows;
using System.Windows.Interop;

namespace TwitchChatter
{
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
            IntPtr hWnd = new WindowInteropHelper(GetWindow(this)).EnsureHandle();
            WindowUtil.UseImmersiveDarkMode(hWnd, true);
            WindowUtil.SetRoundedWindow(hWnd);

            opacitySlider.Value = TwitchChatter.Properties.Settings.Default.opacity * 100.0;
            numOfMessagesBox.Text = TwitchChatter.Properties.Settings.Default.maxNumMessages.ToString();
        }

        private void applyButton_Click(object sender, RoutedEventArgs e)
        {
            //Opacity
            TwitchChatter.Properties.Settings.Default.opacity = (opacitySlider.Value / 100.0);
            Application.Current.MainWindow.Opacity = TwitchChatter.Properties.Settings.Default.opacity;

            TwitchChatter.Properties.Settings.Default.maxNumMessages = int.Parse(numOfMessagesBox.Text);
            TwitchChat.messagePool.ChangeCapacity(TwitchChatter.Properties.Settings.Default.maxNumMessages);
            //Application.Current.MainWindow.UpdateMessagePool();

            TwitchChatter.Properties.Settings.Default.Save();
        }

        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            applyButton_Click(sender, e);
            this.Close();
        }
    }
}
