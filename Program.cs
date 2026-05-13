using System;
using System.IO;
using TMain = Terraria.Main;
using System.Reflection;

public class Program {
	public static string ToolPath;
	public static string ToolDir;
	public static string GamePath;
	public static string GameDir;

	static void Main(string[] args) {
		const string title = "Research Tracker";
		Console.Title = title;
		ToolPath = typeof(Program).Assembly.Location;
		ToolDir = Path.GetDirectoryName(ToolPath);
		GamePath = typeof(TMain).Assembly.Location;
		GameDir = Path.GetDirectoryName(GamePath);

		// run the game (server)!
		var entryPoint = typeof(Terraria.WindowsLaunch).GetMethod("Main", BindingFlags.NonPublic | BindingFlags.Static);
		var brokenWorld = Path.Combine(ToolDir, "TerrariaResearchTracker.wld");
		var gameArgs = new string[] { "-world", brokenWorld };
		Console.WriteLine("Welcome to YAL's Terraria Research Tracker.");
		Console.WriteLine("Press Enter to run Terraria Server.");
		Console.WriteLine("It's getting a broken world file so it should show an error message,");
		Console.WriteLine("after which you can press Enter again to run the rest of this program.");
		Console.WriteLine("This is necessary because Terraria Server needs to fill up item info.");
		Console.WriteLine("Thank you for understanding.");
		Console.ReadLine();
		entryPoint.Invoke(null, new object[] { gameArgs });
		Console.Title = title;

		// the actual program needs to be in a separate class so that referenced classes
		// don't try to initialize before the included ReLogic.dll gets loaded into memory
		TerrariaResearchTracker.Loop();
	}
}