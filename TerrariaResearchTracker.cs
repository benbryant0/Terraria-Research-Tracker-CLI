using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.IO;
using Terraria;
using TMain = Terraria.Main;

public class TerrariaResearchTracker {
	public static PlayerFileInfo SelectPlayer() {
		var files = Directory.GetFiles(TMain.PlayerPath, "*.plr");
		var players = (files
			.Select(path => new PlayerFileInfo(path))
			.Where(info => {
				var player = info.Player;
				return player.loadStatus == StatusID.Ok && player.difficulty == PlayerDifficultyID.Creative;
			})
		).ToArray();
		Array.Sort(players, (a, b) => {
			return -a.LastWriteTime.CompareTo(b.LastWriteTime);
		});
		var more = -1;
		if (players.Length > 9) {
			more = players.Length - 9;
			var newPlayers = new PlayerFileInfo[9];
			for (var i = 0; i < 9; i++) newPlayers[i] = players[i];
			players = newPlayers;
		}
		if (players.Length == 0) return null;

		Console.WriteLine("Which file would you like to load?");
		for (var i = 0; i < players.Length; i++) {
			var d = players[i];
			Console.WriteLine($"[{i+1}] {d.Player.name} ({Path.GetFileName(d.Path)}, {d.LastWriteTime})");
		}
		Console.Write("> ");

		var index = -1;
		while (index < 0) {
			var c = Console.ReadKey().KeyChar;
			index = c - '1';
			if (index >= players.Length) index = -1;
		}
		Console.WriteLine();
		return players[index];
	}

	public static void Run(PlayerFileInfo playerFileInfo) {
		var player = playerFileInfo.Player;
		Console.WriteLine($"Looking at {player.name} ({Path.GetFileName(playerFileInfo.Path)})...");
		var creativeTracker = player.creativeTracker;
		var itemSacrifices = creativeTracker.ItemSacrifices;

		#region Categories
		var catMelee = new ItemInfoCat("Melee");
		var catRanged = new ItemInfoCat("Ranged");
		var catMagic = new ItemInfoCat("Magic");
		var catSentry = new ItemInfoCat("Sentry");
		var catSummon = new ItemInfoCat("Summon");
		var catTools = new ItemInfoCat("Tools");
		var catAccessory = new ItemInfoCat("Accessory");
		var catArmor = new ItemInfoCat("Armor");
		var catVanity = new ItemInfoCat("Vanity");
		var catFishing = new ItemInfoCat("Fishing");
		var catPlaceable = new ItemInfoCat("Placeable");
		var catConsumable = new ItemInfoCat("Consumable");
		var catAmmo = new ItemInfoCat("Ammo");
		var catMaterial = new ItemInfoCat("Material");
		var catDye = new ItemInfoCat("Dye");
		var catPaint = new ItemInfoCat("Paint");
		var catOther = new ItemInfoCat("Other");

		var categories = new List<ItemInfoCat>() {
			catMelee,
			catRanged,
			catMagic,
			catSentry,
			catSummon,
			catTools,
			catAccessory,
			catArmor,
			catVanity,
			catFishing,
			catPlaceable,
			catConsumable,
			catAmmo,
			catMaterial,
			catDye,
			catPaint,
			catOther,
		};
		#endregion

		//
		const string configPath = "TerrariaResearchTracker.cfg";
		Console.Write($"Loading configuration from {configPath}...");
		var config = new ResearchConfig();
		config.Load(Path.Combine(Program.ToolDir, configPath));
		Console.WriteLine(" OK!");

		//
		Console.Write("Collecting item info...");
		var item = new Item();
		var complete = 0;
		var total = 0;
		var allItems = new Dictionary<short, ItemInfo>();
		for (short id = 1; id < ItemID.Count; id++) {
			if (config.Omit.Contains(id)) continue;
			if (itemSacrifices.TryGetSacrificeNumbers(id, out var amountWeHave, out var amountNeededTotal)) {
				total += 1;
				if (amountWeHave >= amountNeededTotal) complete += 1;
				//
				item.SetDefaults(id);
				string pid = "";
				if (!ContentSamples.ItemPersistentIdsByNetIds.TryGetValue(id, out pid)) {
					Console.WriteLine($"{item.Name} (#{id}) has no persistent ID?");
				}
				var info = new ItemInfo(id, item.Name, amountWeHave, amountNeededTotal);
				allItems[id] = info;
				// figure out where it's supposed to go:
				var cat = catOther;
				if (item.vanity) cat = catVanity;
				else if (pid.EndsWith("Dye")) cat = catDye;
				else if (pid.EndsWith("Paint")) cat = catPaint;
				else if (item.ammo > 0) cat = catAmmo;
				else if (item.fishingPole > 0) cat = catFishing;
				else if (item.sentry) cat = catSentry;
				else if (item.summon) cat = catSummon;
				else if (item.accessory) cat = catAccessory;
				else if (item.pick > 0 || item.axe > 0 || item.hammer > 0) cat = catTools;
				else if (item.melee) cat = catMelee;
				else if (item.ranged) cat = catRanged;
				else if (item.magic) cat = catMagic;
				else if (item.headSlot > 0 || item.bodySlot > 0 || item.legSlot > 0) cat = catArmor;
				else if (item.material) cat = catMaterial;
				else if (item.createTile > 0 || item.createWall > 0) cat = catPlaceable;
				else if (item.consumable || item.healLife > 0 || item.healMana > 0) cat = catConsumable;
				//
				cat.Add(info);
			}
		}
		foreach (var aliases in config.Aliases) {
			var anyComplete = false;
			foreach (var id in aliases) {
				if (allItems.TryGetValue(id, out var info) && info.Complete) {
					anyComplete = true;
					break;
				}
			}
			if (anyComplete) {
				foreach (var id in aliases) {
					if (allItems.TryGetValue(id, out var info)) {
						info.Complete = true;
					}
				}
			}
		}
		Console.WriteLine(" OK!");

		//
		Console.WriteLine($"Total: {complete}/{total} ({PrintFraction(complete, total)})");
		foreach (var cat in categories) {
			Console.WriteLine(cat.ToString());
		}

		//
		var dir = Path.Combine(Path.GetDirectoryName(TMain.PlayerPath), "ResearchTracker");
		if (!Directory.Exists(dir)) {
			try {
				Directory.CreateDirectory(dir);
			} catch (Exception) {
				Console.WriteLine($"Couldn't create directory \"{dir}\".");
				Console.WriteLine("Where else would you like to save these? (enter full path)");
				dir = null;
				while (dir == null) {
					Console.Write("> ");
					dir = Console.ReadLine();
					if (!Directory.Exists(dir)) {
						Console.WriteLine("That doesn't seem to exist!");
						dir = null;
						Console.Write("> ");
					}
				}
			}
		}
		Console.Write($"Saving to \"{dir}\"...");
		foreach (var cat in categories) {
			cat.SaveTo(Path.Combine(dir, cat.Name + ".txt"));
		}
		Console.WriteLine(" OK!");
	}
	public static void Loop() {
		var slot = SelectPlayer();
		while (slot == null) {
			Console.WriteLine($"There are no player files in the directory ({TMain.PlayerPath}).");
			Console.WriteLine("You may need to temporarily disable cloud saving for Terraria in Properties.");
			Console.WriteLine("(press Enter to retry)");
			Console.ReadLine();
			slot = SelectPlayer();
		}
		//
		while (true) {
			Run(slot);
			Console.WriteLine("Press Enter to refresh");
			Console.ReadLine();
			slot.Load();
		}
	}
	public static string PrintFraction(int complete, int total) {
		return Math.Round((double)complete / total * 100_00) * 0.01 + "%";
	}
}
