using DiscordRPC;

class StatusHandler
{
	private static FileSystemWatcher fileWatcher;
	private static int currentLine = 0;
	private static StatusType previousStatus;

	private static string currentMapName;
	private static MapType currentMapType;

	public static void Start()
	{
		// Setup a file watcher so that every time the
		// config file is edited we can read it and
		// adjust the status accordingly
		fileWatcher = new FileSystemWatcher(ConfigHandler.Config.Portal2Directory, "console.log");
		fileWatcher.Changed += AttemptToUpdateStatus;
		fileWatcher.EnableRaisingEvents = true;
		Program.Print("File watcher active. Game console is being read.");
	}

	public static void Stop()
	{
		// Shutdown the file watcher
		fileWatcher.Dispose();
		Program.Print("File watcher deactivated.");

		// Clear the console log so that it doesn't
		// get too massive (stay clean fr)
		File.Delete(fileWatcher.Path);
		Program.Print("Cleaned console.log file.");
	}

	private static void AttemptToUpdateStatus(object sender, FileSystemEventArgs e)
	{
		// Read the new file contents
		string[] output;
		while (true)
		{
			// Attempt to read the file output. If it's being
			// used by another process (game still writing to it)
			// or there is some other issue then infinity keep
			// attempting to read it
			try {
				output = File.ReadAllLines(e.FullPath);
				break;
			}
			catch (IOException) { Thread.Sleep(100); }
		}

		// Get the old presence for updating and whatnot
		RichPresence presence = PresenceHandler.Presence;

		// Loop over every line in the config output file
		// and check for specific lines that tell us whats
		// happening in the game and stuff.
		StatusType newStatus = StatusType.Unknown;
		for (int i = currentLine; i < output.Length; i++)
		{
			// Check for if we're loading a chamber
			if (output[i].StartsWith("---- Host_NewGame ----"))
			{
				// Loading a new chamber
				SetLoadingStatus(ref presence);
				newStatus = StatusType.Loading;
			}

			// Get the chambers name
			if (output[i].StartsWith("Host_NewGame on map "))
			{
				// Get the map name
				currentMapName = output[i].Split("Host_NewGame on map ")[1];
				Console.WriteLine(currentMapName);

				// Use the map name to determine what the current status
				// should be (story, workshop map, coop)
				if (currentMapName.StartsWith("workshop")) currentMapType = MapType.CommunityChamber;
				if (currentMapName.StartsWith("sp_")) currentMapType = MapType.SingleplayerStory;
				if (currentMapName.StartsWith("mp_")) currentMapType = MapType.CooperativeChamber;
			}

			// Check for if the chamber is done loading
			if (output[i].StartsWith("Redownloading all lightmaps"))
			{
				// Check for what type of map we're working with
				// and change the status according to that
				// TODO: Use switch
				if (currentMapType == MapType.CommunityChamber)
				{
					// Update the presence
					presence.Assets.LargeImageKey = ConfigHandler.Config.CommunityChamberStatus.LargeImage;
					presence.Details = ConfigHandler.Config.CommunityChamberStatus.TopLine;
					presence.State = ConfigHandler.Config.CommunityChamberStatus.BottomLine;
				}
				else if (currentMapType == MapType.SingleplayerStory)
				{
					// Using the map name, extract the map chapter and title
					//! I got chat gpt to write this is so it could be wrong.
					//! I am NOT writing it myself, or even double checking so
					//! if anyone spots an issue surely submit a pull request or 
					//! whatever the things are called. Maybe just make an issue actually
					Tuple<string, string, string>[] mapInfo = new Tuple<string, string, string>[]
					{
						new Tuple<string, string, string>("sp_a1_intro", "The Courtesy Call", "Container Ride"),
						new Tuple<string, string, string>("sp_a1_intro1", "The Courtesy Call", "Portal Carousel"),
						new Tuple<string, string, string>("sp_a1_intro2", "The Courtesy Call", "Portal Gun"),
						new Tuple<string, string, string>("sp_a1_intro3", "The Courtesy Call", "Smooth Jazz"),
						new Tuple<string, string, string>("sp_a1_intro4", "The Courtesy Call", "Cube Momentum"),
						new Tuple<string, string, string>("sp_a1_intro5", "The Courtesy Call", "Future Starter"),
						new Tuple<string, string, string>("sp_a1_intro6", "The Courtesy Call", "Secret Panel"),
						new Tuple<string, string, string>("sp_a1_intro7", "The Courtesy Call", "Wakeup"),
						new Tuple<string, string, string>("sp_a1_wakeup", "The Courtesy Call", "Incinerator"),
						new Tuple<string, string, string>("sp_a2_intro", "The Cold Boot", "Laser Intro"),
						new Tuple<string, string, string>("sp_a2_laser_intro", "The Cold Boot", "Laser Stairs"),
						new Tuple<string, string, string>("sp_a2_laser_stairs", "The Cold Boot", "Dual Lasers"),
						new Tuple<string, string, string>("sp_a2_dual_lasers", "The Cold Boot", "Laser Over Goo"),
						new Tuple<string, string, string>("sp_a2_laser_over_goo", "The Cold Boot", "Catapult Intro"),
						new Tuple<string, string, string>("sp_a2_catapult_intro", "The Cold Boot", "Trust Fling"),
						new Tuple<string, string, string>("sp_a2_trust_fling", "The Cold Boot", "Pit Flings"),
						new Tuple<string, string, string>("sp_a2_pit_flings", "The Cold Boot", "Fizzler Intro"),
						new Tuple<string, string, string>("sp_a2_fizzler_intro", "The Return", "Ceiling Catapult"),
						new Tuple<string, string, string>("sp_a2_sphere_peek", "The Return", "Ricochet"),
						new Tuple<string, string, string>("sp_a2_ricochet", "The Return", "Bridge Intro"),
						new Tuple<string, string, string>("sp_a2_bridge_intro", "The Return", "Bridge the Gap"),
						new Tuple<string, string, string>("sp_a2_bridge_the_gap", "The Return", "Turret Intro"),
						new Tuple<string, string, string>("sp_a2_turret_intro", "The Return", "Laser Relays"),
						new Tuple<string, string, string>("sp_a2_laser_relays", "The Return", "Turret Blocker"),
						new Tuple<string, string, string>("sp_a2_turret_blocker", "The Return", "Laser vs. Turret"),
						new Tuple<string, string, string>("sp_a2_laser_vs_turret", "The Return", "Pull the Rug"),
						new Tuple<string, string, string>("sp_a2_pull_the_rug", "The Surprise", "Column Blocker"),
						new Tuple<string, string, string>("sp_a2_column_blocker", "The Surprise", "Laser Chaining"),
						new Tuple<string, string, string>("sp_a2_laser_chaining", "The Surprise", "Triple Laser"),
						new Tuple<string, string, string>("sp_a2_triple_laser", "The Surprise", "Jailbreak - Escape"),
						new Tuple<string, string, string>("sp_a2_bts1", "The Escape", "Turret Factory"),
						new Tuple<string, string, string>("sp_a2_bts2", "The Escape", "Turret Sabotage"),
						new Tuple<string, string, string>("sp_a2_bts3", "The Escape", "Neurotoxin Sabotage"),
						new Tuple<string, string, string>("sp_a2_bts4", "The Escape", "Tube Ride"),
						new Tuple<string, string, string>("sp_a2_bts5", "The Escape", "Core"),
						new Tuple<string, string, string>("sp_a2_bts6", "The Fall", "Long Fall - Underground"),
						new Tuple<string, string, string>("sp_a2_core", "The Fall", "Cave Johnson"),
						new Tuple<string, string, string>("sp_a3_00", "The Fall", "Repulsion Intro"),
						new Tuple<string, string, string>("sp_a3_01", "The Fall", "Bomb Flings"),
						new Tuple<string, string, string>("sp_a3_03", "The Fall", "Crazy Box"),
						new Tuple<string, string, string>("sp_a3_jump_intro", "The Fall", "PotatOS"),
						new Tuple<string, string, string>("sp_a3_bomb_flings", "The Reunion", "Propulsion Intro"),
						new Tuple<string, string, string>("sp_a3_crazy_box", "The Reunion", "Propulsion Flings"),
						new Tuple<string, string, string>("sp_a3_transition01", "The Reunion", "Conversion Intro"),
						new Tuple<string, string, string>("sp_a3_speed_ramp", "The Reunion", "Three Gels"),
						new Tuple<string, string, string>("sp_a3_speed_flings", "The Itch", "Test"),
						new Tuple<string, string, string>("sp_a3_portal_intro", "The Itch", "Funnel Intro"),
						new Tuple<string, string, string>("sp_a3_end", "The Itch", "Ceiling Button"),
						new Tuple<string, string, string>("sp_a4_intro", "The Itch", "Wall Button"),
						new Tuple<string, string, string>("sp_a4_tb_intro", "The Itch", "Polarity"),
						new Tuple<string, string, string>("sp_a4_tb_trust_drop", "The Itch", "Funnel Catch"),
						new Tuple<string, string, string>("sp_a4_tb_wall_button", "The Itch", "Stop the Box"),
						new Tuple<string, string, string>("sp_a4_tb_polarity", "The Itch", "Laser Catapult"),
						new Tuple<string, string, string>("sp_a4_tb_catch", "The Itch", "Laser Platform"),
						new Tuple<string, string, string>("sp_a4_stop_the_box", "The Itch", "Propulsion Catch"),
						new Tuple<string, string, string>("sp_a4_laser_catapult", "The Itch", "Repulsion Polarity"),
						new Tuple<string, string, string>("sp_a4_laser_platform", "The Part Where He Kills You", "Finale 1"),
						new Tuple<string, string, string>("sp_a4_speed_tb_catch", "The Part Where He Kills You", "Finale 2"),
						new Tuple<string, string, string>("sp_a4_jump_polarity", "The Part Where He Kills You", "Finale 3"),
						new Tuple<string, string, string>("sp_a4_finale1", "The Part Where He Kills You", "Finale 4"),
						new Tuple<string, string, string>("sp_a4_finale2", "The Part Where He Kills You", "Turret Opera"),
						new Tuple<string, string, string>("sp_a4_finale3", "The Credits", "Credits"),
						new Tuple<string, string, string>("sp_a4_finale4", "The Credits", "Credits"),
						new Tuple<string, string, string>("sp_a5_credits", "The Credits", "Credits")
					};
					Tuple<string, string, string> currentMapInfo = mapInfo.FirstOrDefault(map => map.Item1 == currentMapName);
					string chapter = currentMapInfo.Item2;
					string title = currentMapInfo.Item3;

					// Format the lines to include the chapter and title stuff
					string topLine = ConfigHandler.Config.CommunityChamberStatus.TopLine
						.Replace("{chapter}", chapter)
						.Replace("{title}", title);
					string bottomLine = ConfigHandler.Config.CommunityChamberStatus.BottomLine
						.Replace("{chapter}", chapter)
						.Replace("{title}", title);

					// Update the presence
					presence.Assets.LargeImageKey = ConfigHandler.Config.CommunityChamberStatus.LargeImage;
					presence.Details = topLine;
					presence.State = bottomLine;
				}
				else if (currentMapType == MapType.CooperativeChamber)
				{
					// Using the map name, extract the map chapter and title
					//! I got chat gpt to write this is so it could be wrong.
					//! I am NOT writing it myself, or even double checking so
					//! if anyone spots an issue surely submit a pull request or 
					//! whatever the things are called. Maybe just make an issue actually
					Tuple<string, string, string>[] mapInfo = new Tuple<string, string, string>[]
					{
						new Tuple<string, string, string>("mp_coop_start", "Calibration Course + Hub", "Calibration"),
						new Tuple<string, string, string>("mp_coop_lobby_3", "Calibration Course + Hub", "Hub"),
						new Tuple<string, string, string>("mp_coop_doors", "Team Building", "Doors"),
						new Tuple<string, string, string>("mp_coop_race_2", "Team Building", "Buttons"),
						new Tuple<string, string, string>("mp_coop_laser_2", "Team Building", "Lasers"),
						new Tuple<string, string, string>("mp_coop_rat_maze", "Team Building", "Rat Maze"),
						new Tuple<string, string, string>("mp_coop_laser_crusher", "Team Building", "Laser Crusher"),
						new Tuple<string, string, string>("mp_coop_teambts", "Team Building", "Behind the Scenes"),
						new Tuple<string, string, string>("mp_coop_fling_3", "Mass and Velocity", "Flings"),
						new Tuple<string, string, string>("mp_coop_infinifling_train", "Mass and Velocity", "Infinifling"),
						new Tuple<string, string, string>("mp_coop_come_along", "Mass and Velocity", "Team Retrieval"),
						new Tuple<string, string, string>("mp_coop_fling_1", "Mass and Velocity", "Vertical Flings"),
						new Tuple<string, string, string>("mp_coop_catapult_1", "Mass and Velocity", "Catapults"),
						new Tuple<string, string, string>("mp_coop_multifling_1", "Mass and Velocity", "Multifling"),
						new Tuple<string, string, string>("mp_coop_fling_crushers", "Mass and Velocity", "Fling Crushers"),
						new Tuple<string, string, string>("mp_coop_fan", "Mass and Velocity", "Industrial Fan"),
						new Tuple<string, string, string>("mp_coop_wall_intro", "Hard-Light Surfaces", "Cooperative Bridges"),
						new Tuple<string, string, string>("mp_coop_wall_2", "Hard-Light Surfaces", "Bridge Swap"),
						new Tuple<string, string, string>("mp_coop_catapult_wall_intro", "Hard-Light Surfaces", "Fling Block"),
						new Tuple<string, string, string>("mp_coop_wall_block", "Hard-Light Surfaces", "Catapult Block"),
						new Tuple<string, string, string>("mp_coop_catapult_2", "Hard-Light Surfaces", "Bridge Fling"),
						new Tuple<string, string, string>("mp_coop_turret_walls", "Hard-Light Surfaces", "Turret Walls"),
						new Tuple<string, string, string>("mp_coop_turret_ball", "Hard-Light Surfaces", "Turret Assassin"),
						new Tuple<string, string, string>("mp_coop_wall_5", "Hard-Light Surfaces", "Bridge Testing"),
						new Tuple<string, string, string>("mp_coop_tbeam_redirect", "Excursion Funnels", "Cooperative Funnels"),
						new Tuple<string, string, string>("mp_coop_tbeam_drill", "Excursion Funnels", "Funnel Drill"),
						new Tuple<string, string, string>("mp_coop_tbeam_catch_grind_1", "Excursion Funnels", "Funnel Catch"),
						new Tuple<string, string, string>("mp_coop_tbeam_laser_1", "Excursion Funnels", "Funnel Laser"),
						new Tuple<string, string, string>("mp_coop_tbeam_polarity", "Excursion Funnels", "Cooperative Polarity"),
						new Tuple<string, string, string>("mp_coop_tbeam_polarity2", "Excursion Funnels", "Funnel Hop"),
						new Tuple<string, string, string>("mp_coop_tbeam_polarity3", "Excursion Funnels", "Advanced Polarity"),
						new Tuple<string, string, string>("mp_coop_tbeam_maze", "Excursion Funnels", "Funnel Maze"),
						new Tuple<string, string, string>("mp_coop_tbeam_end", "Excursion Funnels", "Turret Warehouse"),
						new Tuple<string, string, string>("mp_coop_paint_come_along", "Mobility Gels", "Repulsion Jumps"),
						new Tuple<string, string, string>("mp_coop_paint_redirect", "Mobility Gels", "Double Bounce"),
						new Tuple<string, string, string>("mp_coop_paint_bridge", "Mobility Gels", "Bridge Repulsion"),
						new Tuple<string, string, string>("mp_coop_paint_walljumps", "Mobility Gels", "Wall Repulsion"),
						new Tuple<string, string, string>("mp_coop_paint_speed_fling", "Mobility Gels", "Propulsion Crushers"),
						new Tuple<string, string, string>("mp_coop_paint_red_racer", "Mobility Gels", "Turret Ninja"),
						new Tuple<string, string, string>("mp_coop_paint_speed_catch", "Mobility Gels", "Propulsion Retrieval"),
						new Tuple<string, string, string>("mp_coop_paint_longjump_intro", "Mobility Gels", "Vault Entrance"),
						new Tuple<string, string, string>("mp_coop_credits", "Mobility Gels", "Credits"),
						new Tuple<string, string, string>("mp_coop_separation_1", "Art Therapy", "Separation"),
						new Tuple<string, string, string>("mp_coop_tripleaxis", "Art Therapy", "Triple Axis"),
						new Tuple<string, string, string>("mp_coop_catapult_catch", "Art Therapy", "Catapult Catch"),
						new Tuple<string, string, string>("mp_coop_2paints_1bridge", "Art Therapy", "Bridge Gels"),
						new Tuple<string, string, string>("mp_coop_paint_conversion", "Art Therapy", "Maintenance"),
						new Tuple<string, string, string>("mp_coop_bridge_catch", "Art Therapy", "Bridge Catch"),
						new Tuple<string, string, string>("mp_coop_laser_tbeam", "Art Therapy", "Double Lift"),
						new Tuple<string, string, string>("mp_coop_paint_rat_maze", "Art Therapy", "Gel Maze"),
						new Tuple<string, string, string>("mp_coop_paint_crazy_box", "Art Therapy", "Crazier Box"),
					};	
					Tuple<string, string, string> currentMapInfo = mapInfo.FirstOrDefault(map => map.Item1 == currentMapName);
					string coarse = currentMapInfo.Item2;
					string title = currentMapInfo.Item3;


				}

				newStatus = StatusType.InChamber;
			}

			// Only update the status if the status was changed
			if (previousStatus != newStatus) PresenceHandler.Client.SetPresence(presence);

			// Update the current line so that we don't need
			// to reread the entire output every time we go in
			// to check for a status update
			currentLine++;
		}
	}

	//? this is only in a method because its called multiple times
	private static void SetLoadingStatus(ref RichPresence presence)
	{
		presence.Assets.LargeImageKey = ConfigHandler.Config.LoadingStatus.LargeImage;
		presence.Details = ConfigHandler.Config.LoadingStatus.TopLine;
		presence.State = ConfigHandler.Config.LoadingStatus.BottomLine;
	}


	// TODO: Don't use an enum to do this. Seems dodgy
	private enum StatusType
	{
		Unknown,
		Loading,
		InChamber
	}

	// TODO: Add those time trial and least portal things
	private enum MapType
	{
		SingleplayerStory,
		CommunityChamber,
		CooperativeChamber
	}
}