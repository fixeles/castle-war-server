using System.Diagnostics;

public static class SSH
{
	private static Process? _process;

	public static Task Start()
	{
		KillSshProcesses();
		return Task.Run(() =>
		{
			var processInfo = new ProcessStartInfo
			{
				FileName = "ssh",
				Arguments = $"-R {Constants.SSH}",
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				UseShellExecute = false,
				CreateNoWindow = true
			};

			_process = new Process { StartInfo = processInfo };
			_process.OutputDataReceived += (sender, e) =>
			{
				if (!string.IsNullOrEmpty(e.Data))
				{
					Console.WriteLine("OUTPUT: " + e.Data);
				}
			};

			_process.ErrorDataReceived += (sender, e) =>
			{
				if (!string.IsNullOrEmpty(e.Data))
				{
					Console.WriteLine("ERROR: " + e.Data);
				}
			};

			_process.Start();
			_process.BeginOutputReadLine();
			_process.BeginErrorReadLine();


			Console.ReadLine();
			StopSshTunnel();
		});
	}

	private static void KillSshProcesses()
	{
		var processInfo = new ProcessStartInfo
		{
			FileName = "taskkill",
			Arguments = "/F /IM ssh.exe",
			RedirectStandardOutput = true,
			RedirectStandardError = true,
			UseShellExecute = false,
			CreateNoWindow = true
		};

		using var process = new Process();
		process.StartInfo = processInfo;
		process.Start();
		process.WaitForExit();
	}

	private static void StopSshTunnel()
	{
		Console.WriteLine("SSH tunnel was closed");
		if (_process is { HasExited: false })
		{
			_process.Kill();
			_process.Dispose();
		}
	}
}