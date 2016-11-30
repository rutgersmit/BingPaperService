using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Timers;
using System.Xml.Linq;

namespace BingPaperService
{
	public class BingPaper
	{
		private readonly Timer _timer;

		private const int INTERVAL = 10000;// 43200000;
		private const int SPI_SETDESKWALLPAPER = 20;
		private const int SPIF_UPDATEINIFILE = 0x01;
		private const int SPIF_SENDWININICHANGE = 0x02;

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		private static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

		
		public BingPaper()
		{
			Print("Initializing");
			_timer = new Timer(INTERVAL) { AutoReset = true };
			_timer.Elapsed += OnTimerOnElapsed;

			OnTimerOnElapsed(null, null);
			//SetWallpaper();
		}

		private void OnTimerOnElapsed(object sender, ElapsedEventArgs e)
		{
			Print("Set the current wallpaper");
			SetWallpaper();
			Print("Next refresh at " + DateTime.Now.Add(new TimeSpan(0,0,0,0, INTERVAL)).ToString("yyyy-MM-dd HH:mm:ss"));
		}


		public void Start()
		{
			Print("Starting timer");
			_timer.Start();
		}

		public void Stop()
		{
			Print("Stopping timer");
			_timer.Stop();
		}

		public void SetWallpaper()
		{
			try
			{
				string address = "http://www.bing.com/hpimagearchive.aspx?format=xml&idx=0&n=20&mbl=1&mkt=en-ww"; // TODO: make region configurable?
				string xml = "";

				var webClient = new WebClient();
				webClient.DownloadFileCompleted += WebClient_DownloadFileCompleted;
				xml = webClient.DownloadString(address);

				var doc = XDocument.Parse(xml).Descendants("image");
				foreach (var n in doc)
				{
					Print("Getting the first wallpaper");
					var imgurl = n.Descendants("urlBase").FirstOrDefault().Value;
					webClient.DownloadFileAsync(new Uri("http://www.bing.com" + imgurl + "_1920x1200.jpg"), Path.GetTempPath() + "\\bingwallpaper.jpg");
					break; // TODO: tidy up this for loop
				}

			}
			catch
			{

			}
		}

		private void WebClient_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
		{
			Print("Setting wallpaper");
			if (File.Exists(Path.GetTempPath() + "\\bingwallpaper.jpg"))
				SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, System.IO.Path.GetTempPath() + "\\bingwallpaper.jpg", SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
			Print("Wallpaper set");
		}

		private void Print(string msg)
		{
			Console.WriteLine("{0} > {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), msg);
		}
	}
}
