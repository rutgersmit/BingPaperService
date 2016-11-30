using Topshelf;

namespace BingPaperService
{
	class Program
	{
		static void Main(string[] args)
		{
			HostFactory.Run(hostConfigurator =>
			{
				hostConfigurator.Service<BingPaper>(serviceConfigurator =>
				{
					serviceConfigurator.ConstructUsing(() => new BingPaper());
					serviceConfigurator.WhenStarted(myService => myService.Start());
					serviceConfigurator.WhenStopped(myService => myService.Stop());
				});

				hostConfigurator.RunAsLocalSystem();

				hostConfigurator.SetDisplayName("BingPaper");
				hostConfigurator.SetDescription("BingPaper Service");
				hostConfigurator.SetServiceName("BingPaperSvc");
			});
		}
	}
}
