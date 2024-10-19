var serverTask = StartServer(args);
var sshTask = SSH.Start();

await Task.WhenAll(serverTask, sshTask);


static Task StartServer(string[] args)
{
	return Task.Run(() =>
	{
		var host = Host.CreateDefaultBuilder(args)
			.ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); })
			.Build();

		host.Run();
	});
}