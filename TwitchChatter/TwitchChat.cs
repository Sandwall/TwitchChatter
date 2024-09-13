using System;
using System.Net.Sockets;
using System.Net.Security;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Interop;

namespace TwitchChatter
{
	public static class TwitchChat
	{
		private const string ip = "irc.chat.twitch.tv";
		private const int port = 6697; //6667 is the normal port, this is the SSL one
		private static string twitchChannel = "";
		private static string botUsername = "";
		private static string password = "";

		private static StreamReader? streamReader;
		private static StreamWriter? streamWriter;

		public static bool isConnected = false;

		public static MessagePool messagePool;
		public static MainWindow windowRef;

		private static bool ValidateServerCertificate(object sender, X509Certificate? certificate, X509Chain? chain, SslPolicyErrors sslPolicyErrors)
		{
			return sslPolicyErrors == SslPolicyErrors.None;
		}

		public static async Task Connect()
		{
			if (!File.Exists("./twitch.chat")) return;

			StreamReader sr = new StreamReader("./twitch.chat");
			String[] lines = sr.ReadToEnd().Split('\n');
			sr.Close();
			if (lines.Length < 3) return;

			twitchChannel = lines[0];
			botUsername = lines[1];
			password = lines[2];

			//Assumes isConnected is checked
			var tcpClient = new TcpClient();
			await tcpClient.ConnectAsync(ip, port);
			SslStream sslStream = new SslStream(tcpClient.GetStream(), false, ValidateServerCertificate, null);

			await sslStream.AuthenticateAsClientAsync(ip);

			streamReader = new StreamReader(sslStream);
			streamWriter = new StreamWriter(sslStream) { NewLine = "\r\n", AutoFlush = true };

			await streamWriter.WriteLineAsync($"PASS {password}");
			await streamWriter.WriteLineAsync($"NICK {botUsername}");
			await streamWriter.WriteLineAsync($"JOIN #{twitchChannel}");

			await SendMessage("I have connected.");

			isConnected = true;
			//Assumes isConnected is checked
		}

		public static async Task Disconnect()
		{
			//Assumes isConnected was checked
			await streamWriter.WriteLineAsync($"PART #{twitchChannel}");

			isConnected = false;
			Close();
		}

		public static void Close()
		{
			if (streamReader != null) { streamReader.Close(); }
			if (streamWriter != null) { streamWriter.Close(); }
		}

		public static async Task Run()
		{
			//Should be true if Connect was run before
			while (isConnected)
			{
				string line = await streamReader.ReadLineAsync();
				//Console.WriteLine(line);

				if(line != null)
				{
					string[] split = line.Split(" ");
					//PING :tmi.twitch.tv
					//Respond with PONG :tmi.twitch.tv
					switch (split[0].ToUpper())
					{
						case "PING":
							{
								Console.WriteLine("PING");
								await streamWriter.WriteLineAsync($"PONG {split[1]}");
							}
							break;
						case "NOTICE":
							{
								Console.WriteLine("NOTICE");
							}
							break;
					}
					if (split.Length > 1 && split[1].ToUpper() == "PRIVMSG")
					{
						int exclamationPointPosition = split[0].IndexOf("!");
						string username = split[0].Substring(1, exclamationPointPosition - 1);
						int secondColonPosition = line.IndexOf(':', 1);//the 1 here is what skips the first character
						string message = line.Substring(secondColonPosition + 1);//Everything past the second colon
						messagePool.Add(new Message(message, username, "#FF00FF"));
						windowRef.UpdateMessagePool();
					}
					
				}
			}
		}

		public static async void SendMessage(string msg)
		{
			if(isConnected)
			{
				messagePool.Add(new Message(msg, botUsername, "#00FF00"));
				await streamWriter.WriteLineAsync($"PRIVMSG #{twitchChannel} :{msg}");
				windowRef.UpdateMessagePool();
			}
		}
	}
}
