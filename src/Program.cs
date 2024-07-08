using DiscordRPC;
using TinyDialogsNet;

class Program
{
	public static DiscordRpcClient Client;
	public static RichPresence Presence;

	public static void Main(string[] args)
	{
		// Give a fancy title thingy
		Console.Title = "Portal 2 RPC config settings thing";

		// Config stuff
		ConfigHandler.Initialize();

		// Check for if they have setup everything. If not
		// then guide them through the setup process
		Setup();

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
		Print("Rich presence started!");

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

	private static void Setup()
	{
		// Keep track of if we've shown the instructions
		// already so we don't flood the console with them
		// a million times if they cancel
		bool shownInstructions = false;

		// Keep on asking them for a location
		while (ConfigHandler.Config.CompletedSetup == false)
		{
			if (shownInstructions == false)
			{
				// Say what we're going to do and give some instructions
				Console.WriteLine("\nThis works by reading console output from");
				Console.WriteLine("Portal 2 as you play. Please select portal2.exe");
				Console.WriteLine("so that we know where to read your game status from.\n");
				shownInstructions = true;
			}

			// Open a file dialogue so that they can show
			// what directory portal 2 is stored in
			FileFilter filter = new FileFilter("Portal 2 game", ["portal2.exe"]);
			string defaultSteamInstallationDirectory = @"C:\Program Files (x86)\Steam\steamapps\common\Portal 2\";
			var (cancelled, paths) = TinyDialogs.OpenFileDialog("Select portal 2 executable (portal2.exe)", defaultSteamInstallationDirectory, false, filter);

			// Get the answer
			string portal2ExePath = "";
			if (cancelled || paths == null)
			{
				Console.WriteLine("You've cancelled the search. Press any key to try again...");
				Console.ReadKey(true);
				continue;
			}
			using (IEnumerator<string> enumerator = paths.GetEnumerator())
			{
				if (enumerator.MoveNext()) portal2ExePath = enumerator.Current;
			}

			// Save the portal 2 location to the config
			ConfigHandler.Config.Portal2Directory = Path.Join(Path.GetDirectoryName(portal2ExePath), "portal2");
			Console.WriteLine();
			Print("Wrote game path to config.");

			// Create or edit the autoexec file so that we can
			// automatically run the command to get the console output
			string autoexecFilePath = Path.Combine(ConfigHandler.Config.Portal2Directory, "cfg", "autoexec.cfg");
			using (StreamWriter writer = new StreamWriter(autoexecFilePath, true))
			{
				writer.WriteLine("\n\n# Save the console output to a file. Used for Discord RPC");
				writer.WriteLine("con_logfile console.log\n\n");
			}
			Print("Wrote autoexec file.");

			// Say that we've completed setup
			ConfigHandler.Config.CompletedSetup = true;
			ConfigHandler.Save(ConfigHandler.Config);
		}
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
		Print(status);
		Presence.Details = status;
		Client.SetPresence(Presence);
	}

	private static void Shutdown(string logFilePath)
	{
		// Turn off the presence if its not already off
		if (Client.IsDisposed) return;
		Client.Dispose();

		// Clear the file so that we don't have a
		// massive log file and so that we keep clean
		File.Delete(logFilePath);

		Print("Shutting down RPC");
	}



	// Fancy print thingy with a timestamp and whatnot
	public static void Print(string status)
	{
		Console.ForegroundColor = ConsoleColor.DarkGray;
		Console.Write("[");
		Console.ForegroundColor = ConsoleColor.Cyan;
		Console.Write(DateTime.UtcNow);
		Console.ForegroundColor = ConsoleColor.DarkGray;
		Console.Write("] ");
		Console.ResetColor();
		Console.WriteLine(status);
	}
}