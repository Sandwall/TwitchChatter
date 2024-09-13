using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;

namespace TwitchChatter
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private double screenPadding = 20.0;
		
		public enum TCorner
		{
			tLeft,
			bLeft,
			tRight,
			bRight
		}

		TCorner screenCorner = TCorner.tLeft; 

		private void SetWindowPositionCorner(TCorner corner, double sWidth, double sHeight)
		{
			switch(corner)
			{
				case TCorner.tLeft:
					this.Top = screenPadding;
					this.Left = screenPadding;
					break;
				case TCorner.bLeft:
					this.Top = sHeight - (this.Height + screenPadding);
					this.Left = screenPadding;
					break;
				case TCorner.tRight:
					this.Top = screenPadding;
					this.Left = sWidth - (this.Width + screenPadding);
					break;
				case TCorner.bRight:
					this.Top = sHeight - (this.Height + screenPadding);
					this.Left = sWidth - (this.Width + screenPadding);
					break;
			}
		}

		private IntPtr hWnd;
		private SettingsWindow settingsWindow;

		private void SetWindowProperties()
		{
			this.Opacity = TwitchChatter.Properties.Settings.Default.opacity;

			hWnd = new WindowInteropHelper(GetWindow(this)).EnsureHandle();
			WindowUtil.SetRoundedWindow(hWnd);
		}

		public MainWindow()
		{
			InitializeComponent();
			SetWindowPositionCorner(TCorner.tLeft, System.Windows.SystemParameters.PrimaryScreenWidth, System.Windows.SystemParameters.PrimaryScreenHeight);
			TwitchChat.messagePool = new MessagePool(TwitchChatter.Properties.Settings.Default.maxNumMessages);
			TwitchChat.windowRef = this;

			this.SetWindowProperties();
		}

		private void closeButton_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}

		private void captionButton1_Click(object sender, RoutedEventArgs e)
		{
			
			screenCorner = TCorner.tLeft;
			SetWindowPositionCorner(screenCorner, System.Windows.SystemParameters.PrimaryScreenWidth, System.Windows.SystemParameters.PrimaryScreenHeight);
		}

		private void captionButton2_Click(object sender, RoutedEventArgs e)
		{
			screenCorner = TCorner.tRight;
			SetWindowPositionCorner(screenCorner, System.Windows.SystemParameters.PrimaryScreenWidth, System.Windows.SystemParameters.PrimaryScreenHeight);
		}

		private void captionButton3_Click(object sender, RoutedEventArgs e)
		{
			screenCorner = TCorner.bRight;
			SetWindowPositionCorner(screenCorner, System.Windows.SystemParameters.PrimaryScreenWidth, System.Windows.SystemParameters.PrimaryScreenHeight);
		}

		private void captionButton4_Click(object sender, RoutedEventArgs e)
		{
			screenCorner = TCorner.bLeft;
			SetWindowPositionCorner(screenCorner, System.Windows.SystemParameters.PrimaryScreenWidth, System.Windows.SystemParameters.PrimaryScreenHeight);
		}

		private void chatBox_KeyDown(object sender, KeyEventArgs e)
		{
			if(e.Key == Key.Enter)
			{
				TwitchChat.SendMessage(chatBox.Text.TrimEnd('\n'));
				chatBox.Text = "";
			}
		}

		public void UpdateMessagePool()
		{
			this.messageStackPanel.Children.Clear();
			foreach (Message m in TwitchChat.messagePool.messages)
			{
				StackPanel sP = new StackPanel();
				TextBlock name = new TextBlock();
				TextBlock message = new TextBlock();
				name.Text = m.Sender;
				name.Foreground = new BrushConverter().ConvertFromString(m.Color) as SolidColorBrush;
				name.FontWeight = FontWeights.Bold;
				message.Text = m.Content;
				message.Foreground = Brushes.White;
				sP.Orientation = Orientation.Horizontal;
				sP.Children.Add(name); sP.Children.Add(message);
				sP.VerticalAlignment = VerticalAlignment.Center;
				this.messageStackPanel.Children.Insert(this.messageStackPanel.Children.Count, sP);
			}
		}

		private void clearEntriesMenuItem_Click(object sender, RoutedEventArgs e)
		{
			this.messageStackPanel.Children.Clear();
		}

		private async void connectMenuItem_Click(object sender, RoutedEventArgs e)
		{
			if(!TwitchChat.isConnected)
			{
				await TwitchChat.Connect();
				if (TwitchChat.isConnected)
				{   // .Connect sets isConnected, don't want to run if we failed
					connectMenuItem.Header = "Disconnect";
					await TwitchChat.Run();
				} else
				{
					MessageBox.Show("Was unable to read login details. Ensure there is a file next to the binary called 'twitch.chat', where the first line is the channel you want to join, second line is the name of the account you want to login to, and third line is the oauth password of that account.");
				}
			}
			else
			{
				await TwitchChat.Disconnect();
				connectMenuItem.Header = "Connect";
			}
		}

		private void settingsMenuItem_Click(object sender, RoutedEventArgs e)
		{
			settingsWindow = new SettingsWindow();
			settingsWindow.Owner = this;
			settingsWindow.ShowDialog();
		}

		private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			SetWindowPositionCorner(TCorner.tLeft, System.Windows.SystemParameters.PrimaryScreenWidth, System.Windows.SystemParameters.PrimaryScreenHeight);
		}
		private void menuButton_Click(object sender, RoutedEventArgs e)
		{
			this.menuButton.ContextMenu.PlacementTarget = sender as Button;
			this.menuButton.ContextMenu.IsOpen = true;
		}
	}
}
