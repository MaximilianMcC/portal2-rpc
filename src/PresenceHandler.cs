using DiscordRPC;

class PresenceHandler
{
	public static DiscordRpcClient Client;
	public static RichPresence Presence;

	public static void Start()
	{
		// Start and enable the rich presence
		Client = new DiscordRpcClient(ConfigHandler.Config.ClientId.ToString());
		Client.Initialize();

		// Set the default status to be loading and set the
		// timestamp to be right now (when the status started)
		Presence = new RichPresence()
		{
			Assets = new Assets()
			{
				LargeImageKey = ConfigHandler.Config.LoadingStatus.LargeImage
			},
			Details = ConfigHandler.Config.LoadingStatus.TopLine,
			State = ConfigHandler.Config.LoadingStatus.BottomLine,
			Timestamps = new Timestamps() { Start = DateTime.UtcNow },
		};
		Client.SetPresence(Presence);

		Program.Print("Rich presence started!");

		// Start the status handler so we can
		// get the actual status from the game
		StatusHandler.Start();
	}

	public static void Stop()
	{
		// If the presence isn't already
		// stopped then stop it
		if (Client.IsDisposed) return;
		Client.Dispose();
		Program.Print("Rich presence shut down.");

		// Stop getting statuses from the status handler
		StatusHandler.Stop();

		// Clear the console log so that it doesn't
		// get too massive (stay clean fr)
		File.Delete(Path.Join(ConfigHandler.Config.Portal2Directory, "console.log"));
		Program.Print("Cleaned console.log file.");
	}
}