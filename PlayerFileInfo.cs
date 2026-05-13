using System;
using System.IO;
using Terraria;
using Terraria.IO;

public class PlayerFileInfo {
	public string Path;
	public DateTime LastWriteTime;
	public PlayerFileData Data = null;
	public Player Player = null;
	public PlayerFileInfo(string path) {
		Path = path;
		LastWriteTime = File.GetLastWriteTimeUtc(path);
		Load();
	}
	public void Load() {
		Data = Player.LoadPlayer(Path, false);
		Player = Data.Player;
	}
}