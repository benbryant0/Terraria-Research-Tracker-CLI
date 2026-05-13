# Terraria Research Tracker (CLI edition)
**Quick links:** [itch.io](https://yellowafterlife.itch.io/terraria-research-tracker) (pre-built binaries)

This is a simpler re-creation of [my older web-based tool](https://yal.cc/game-tools/terraria-research/)
that works for Terraria 1.4.5.x and will hopefully work for future game versions with few to no changes.

## How to use this

1. Compile the tool from source code or download a pre-built binary from itch.io
2. Extract the ZIP (if downloaded) or copy the files from `bin` directory (if compiled) to the Terraria folder so that `TerrariaResearchTracker.exe` sits next to `Terraria.exe`
3. Good to use!
4. Resulting files go in Terraria's folder in My Documents.

## How to compile this
You'll want to set a `TERRARIA_PATH` environment variable to point at where the game folder resides (where `Terraria.exe` is, no trailing backslash).

You can then open the Visual Studio (VS2019 or newer should work) and build/run the project.

## Motivation
When I made the web-based Research Tracker in now-distant 2021,
I knew that it would not be as popular as Terrasavr,
but even then I have slightly overestimated public interest -
in the year leading up to 1.4.5 release, the tool had just about 200 times fewer visits than Terrasavr.

This isn't an issue on its own -
many of my works are obscure at best -
but being a web-based tool that needs up-to-date game info
*and* needs to be able to read data from a binary save file
means that it has a visible upkeep cost.

These issues were then made worse by the ongoing war in my country,
which has been slowly eroding both my availability and my motivation
to work on personal projects.

So anyway, this quick rewrite aims to address a few of these problems:
1. Being a downloadable tool means that it can ask the game to load your player file, or for up-to-date item information.
2. It has fewer moving parts (e.g. I'm not extracting textures) so there are fewer things that can break.
3. It is open-source so someone might be able to fix it up if I don't have time to.
4. Small tweaks like excluding unobtainable items can be done in a `.cfg` file without touching the source code.

## Mini-FAQ

### Mac & Linux
You can run the program with the Windows version of Terraria through WINE or Proton.

It is likely possible to build this for Mac/Linux natively,
but personally I'm unfamiliar with .NET/Mono specifics.

### Mobile
If someone figures out how to compile this for Linux,
someone could set up a little web app
that takes player files and hands them to the tool + Terraria Server.

Until then... could ask someone to run the tool for you on PC?

I don't think anyone has ever mentioned anything to me
in regards of using the original web-based tool on mobile
so I cannot tell whether this is a real point of concern.

### Trust
This is a C# + .NET application so you can decompile it with ILSpy/etc.

## Credits
A tool by YellowAfterlife.

This one is **not** written in Haxe, just your regular C#.