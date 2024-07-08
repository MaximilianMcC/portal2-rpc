using System.Text.Json;

class ConfigHandler
{
	public static readonly string FilePath = "./config.json";
	public static Config Config { get; private set; }

	public static void Initialize()
	{
		// See if the json file exists. If it doesn't then create it
		if (File.Exists(FilePath) == false)
		{
			// Get a default copy of the data
			// then write it to the config file
			Save(new Config());
			Program.Print("Wrote new config file");
		}

		// Load the config from the JSON file
		string configFile = File.ReadAllText(FilePath);
		Config = JsonSerializer.Deserialize<Config>(configFile);
		Program.Print("Loaded config file");
	}

	public static void Save(Config config)
	{
		// Format the json with indentation because
		// manually editing the file will probably be
		// the way its done for a while
		JsonSerializerOptions options = new JsonSerializerOptions();
		options.WriteIndented = true;

		// Serialize the config to json then write it
		string json = JsonSerializer.Serialize(config, options);
		File.WriteAllText(FilePath, json);
	}
}

class Config
{
	public ulong ClientId { get; set; } = 1258253632785350787;

	public bool CompletedSetup { get; set; } = false;
	public string Portal2Directory { get; set; } = null;

	public bool RpcEnabled { get; set; } = true;
	public bool ShowMapStatus { get; set; } = true;
	public bool ShowPortalgunStatus { get; set; } = true;

	public PortalGunStatus PortalGunStatuses { get; set; } = new PortalGunStatus()
	{
		NoneText = "",
		NoneSmallImage = "",

		SingleText = "Single portal device",
		SingleSmallImage = "single_portal_crosshair",

		DualText = "Dual portal device",
		DualSmallImage = "dual_portal_crosshair"
	};

	public GenericStatus MenuStatus { get; set; } = new GenericStatus()
	{
		TopLine = "In the menu",
		BottomLine = "",
		LargeImage = "modern_aperture_logo"
	};

	public GenericStatus LoadingStatus { get; set; } = new GenericStatus()
	{
		TopLine = "Loading",
		BottomLine = "",
		LargeImage = "modern_aperture_logo"
	};

	public GenericStatus CommunityChamberStatus { get; set; } = new GenericStatus()
	{
		TopLine = "Playing a community chamber",
		BottomLine = "",
		LargeImage = "modern_aperture_logo"
	};

	public SingleplayerStoryStatus SingleplayerStoryStatus { get; set; } = new SingleplayerStoryStatus()
	{
		TopLine = "Playing the story",
		BottomLine = "{chapter} - {title}",

		Chapter1LargeImage = "run_down_modern_aperture_logo",
		Chapter2LargeImage = "run_down_modern_aperture_logo",
		Chapter3LargeImage = "run_down_modern_aperture_logo",
		Chapter4LargeImage = "modern_aperture_logo",
		Chapter5LargeImage = "modern_aperture_logo",
		Chapter6LargeImage = "1950s_aperture_logo",
		Chapter7LargeImage = "1970s_aperture_logo",
		Chapter9LargeImage = "wheatley_aperture_logo"
	};

	public CooperativeStoryStatus CoopStoryStatus { get; set; } = new CooperativeStoryStatus()
	{
		TopLine = "Playing the co-op story",
		BottomLine = "{course} - {title}",

		Course1LargeImage = "modern_aperture_logo",
		Course2LargeImage = "run_down_modern_aperture_logo",
		Course3LargeImage = "run_down_modern_aperture_logo",
		Course4LargeImage = "modern_aperture_logo",
		Course5LargeImage = "1970s_aperture_logo",
		Course6LargeImage = "1970s_aperture_logo"
	};
}

class PortalGunStatus
{
	public string NoneText { get; set; }
	public string NoneSmallImage { get; set; }

	public string SingleText { get; set; }
	public string SingleSmallImage { get; set; }

	public string DualText { get; set; }
	public string DualSmallImage { get; set; }
}

class GenericStatus
{
	public string TopLine { get; set; }
	public string BottomLine { get; set; }
	public string LargeImage { get; set; }
}

class SingleplayerStoryStatus
{
	public string TopLine { get; set; }
	public string BottomLine { get; set; }

	public string Chapter1LargeImage { get; set; }
	public string Chapter2LargeImage { get; set; }
	public string Chapter3LargeImage { get; set; }
	public string Chapter4LargeImage { get; set; }
	public string Chapter5LargeImage { get; set; }
	public string Chapter6LargeImage { get; set; }
	public string Chapter7LargeImage { get; set; }
	public string Chapter9LargeImage { get; set; }
}

class CooperativeStoryStatus
{
	public string TopLine { get; set; }
	public string BottomLine { get; set; }

	public string Course1LargeImage { get; set; }
	public string Course2LargeImage { get; set; }
	public string Course3LargeImage { get; set; }
	public string Course4LargeImage { get; set; }
	public string Course5LargeImage { get; set; }
	public string Course6LargeImage { get; set; }
}