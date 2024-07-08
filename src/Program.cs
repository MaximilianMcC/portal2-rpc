using DiscordRPC;

class Program
{
	public static DiscordRpcClient Client;
	public static RichPresence Presence;

	public static void Main(string[] args)
	{
		// Give a fancy title thingy
		Console.Title = "Portal 2 RPC";
		Console.WriteLine("     Portal 2 Discord Rich Presence");
		Console.WriteLine("--------------------------------------------");

		// Instructions
		Console.WriteLine("1. Launch Portal 2");
		Console.WriteLine("2. Open the developer console");
		Console.WriteLine("3. Run 'con_logfile console.log'");
		Console.WriteLine("4. Press any key on this window to start the Rich Presence");
		Console.ReadKey(true);
		Console.WriteLine("--------------------------------------------");

		// Start and enable the rich presence 
		Client = new DiscordRpcClient("1258253632785350787");
		Client.Initialize();

		// Set the initial presence
		Presence = new RichPresence()
		{
			Assets = new Assets() { LargeImageKey = "aperture_logo_blue" },
			Timestamps = new Timestamps() { Start = DateTime.UtcNow },
			Details = "Debugulations rn"
		};
		Client.SetPresence(Presence);
		Console.WriteLine("Rich presence started!");

		// Setup the file watcher so that every time the
		// config file is edited we can do something
		// TODO: Don't hardcode the path
		string logFilePath = @"D:\games\steam\steamapps\common\Portal 2\portal2\";
		FileSystemWatcher fileWatcher = new FileSystemWatcher(logFilePath, "console.log");
		fileWatcher.Changed += AttemptToUpdateStatus;
		fileWatcher.EnableRaisingEvents = true;

		Console.ReadKey();
		Shutdown(fileWatcher.Path);
	}

	private static void AttemptToUpdateStatus(object sender, FileSystemEventArgs e)
	{
		// Read the new file contents
		string[] output;
		try { output = File.ReadAllLines(e.FullPath); }
		catch (IOException)
		{
			// File is being written to by the game (wait a sec before retrying)
			Thread.Sleep(100);
			output = File.ReadAllLines(e.FullPath);
		}

		// Parse the output to determine where
		// the user currently is in the game
		string status = "In the Main Menu";
		foreach (string line in output)
		{
			if (line.Contains("Host_NewGame on map")) status = "Loading a chamber";
			if (line.Contains("==== calling mapspawn.nut")) status = "Playing a chamber";
			if (line.Contains("from server (Server shutting down)") || line.Contains("Dropped ")) status = "In the Main Menu";
			if (line.Contains("Host_WriteConfiguration: Wrote cfg/config.cfg")) Shutdown(e.FullPath);
		}

		// Update the RPC status if it has changed
		if (Presence.Details == status) return;
		Console.WriteLine($"[{Console.ForegroundColor = ConsoleColor.Cyan}{DateTime.UtcNow}{Console.ResetColor}] {status}");
		Presence.Details = status;
		Client.SetPresence(Presence);
	}

	private static void Shutdown(string logFilePath)
	{
		// Turn off the presence
		Client.Dispose();

		// Clear the file so that we don't have a
		// massive log file and so that we keep clean
		File.Delete(logFilePath);

	}
}