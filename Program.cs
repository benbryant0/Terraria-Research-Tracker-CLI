using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Loader;
using TMain = Terraria.Main;

public class Program {
	public static string ToolDir;
	public static string GameDir;

	static void Main(string[] args) {
		AssemblyLoadContext.Default.Resolving += ResolveFromGac;

		// The tool is supposed to be placed next to the game executables, so this is fine for now
		ToolDir = AppContext.BaseDirectory;
		GameDir = AppContext.BaseDirectory;

		// Explicitly load since modern .NET is more conservative in how it probes for assemblies
		Assembly.LoadFrom("TerrariaServer.exe");

		Start();
	}

	// Kept out of Main so that the JIT doesn't try to load Terraria before GAC resolving is set up
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void Start() {
        const string title = "Research Tracker";
		Console.Title = title;

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

    private const string gacRoot = @"C:\Windows\Microsoft.NET\assembly";
    private static readonly string[] GacSubDirs = ["GAC_MSIL", "GAC_32", "GAC_64"];
    private static Assembly ResolveFromGac(AssemblyLoadContext context, AssemblyName name) {
		if (name.Name is null || name.Version is null) {
			return null;
		}

        var token = name.GetPublicKeyToken();
        var tokenStr = token is { Length: > 0 }
            ? Convert.ToHexString(token).ToLowerInvariant()
            : "";

        foreach (var subdir in GacSubDirs) {
            var versionDir = $"v4.0_{name.Version}{(tokenStr.Length > 0 ? $"__{tokenStr}" : "")}";
            var path = Path.Combine(gacRoot, subdir, name.Name, versionDir, $"{name.Name}.dll");

			if (File.Exists(path)) {
				return context.LoadFromAssemblyPath(path);
			}
        }

        return null;
    }
}
