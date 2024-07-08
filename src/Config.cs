class Config
{
	public static readonly string FilePath = "./settings.txt";

	// TODO: Just use stj bruh (would have been done by now and probably faster)
	public static void Initialize()
	{
		// Make a new settings file if there isn't one already
		if (File.Exists(FilePath) == false)
		{
			// Get all the default settings
			// TODO: Could just spam the SetValue method also
			string[] defaultSettings = new string[]
			{
				GetOptionString("setup", false),
				GetOptionString("enabled", true),
				GetOptionString("showMapType", true),
				GetOptionString("showPortalgunType", true),
				GetOptionString("portalLocation", null),
			};

			// Make the file, and write all the default settings to it
			File.WriteAllLines(FilePath, defaultSettings);
		}
	}

	// Get an option as a string for writing to the file
	public static string GetOptionString(string key, object value)
	{
		return $"{key}: {value}";
	}

	// Get a value as a boolean
	public static bool GetBoolean(string key)
	{
		// Open the file and loop through every line
		// until the specified key is found
		string[] config = File.ReadAllLines(FilePath);
		foreach (string line in config)
		{
			// Split up the line into a key and value
			string[] segments = line.Split(": ");

			// Return the value if its the one we're looking for
			if (segments[0] != key) continue;
			return Boolean.Parse(segments[1]);
		}

		// No value was found. Defaulting to false
		return false;
	}

	// Get a value as a string
	public static string GetString(string key)
	{
		// Open the file and loop through every line
		// until the specified key is found
		string[] config = File.ReadAllLines(FilePath);
		foreach (string line in config)
		{
			// Split up the line into a key and value
			string[] segments = line.Split(": ");

			// Return the value if its the one we're looking for
			if (segments[0] != key) continue;
			return segments[1];
		}

		// No value was found. Defaulting to null
		return null;
	}

	// Set a value
	public static void SetValue(string key, object value)
	{
		// Open the file and loop through every line
		// until the specified key is found
		string[] config = File.ReadAllLines(FilePath);
		for (int i = 0; i < config.Length; i++)
		{
			// Split up the line into a key and value
			string[] segments = config[i].Split(": ");

			// Assign the value if its the one we're looking for
			if (segments[0] != key) continue;
			config[i] = GetOptionString(key, value);

			// Write it to the file to save the changes
			File.WriteAllLines(FilePath, config);
			return;
		}
	}
}