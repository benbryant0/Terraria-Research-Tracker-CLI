using System.Collections.Generic;
using System.IO;
using System.Text;

public class ItemInfoCat {
	public string Name;
	public List<ItemInfo> Items = new List<ItemInfo>();
	public int Complete = 0;
	public int Total = 0;
	public string Percent {
		get => TerrariaResearchTracker.PrintFraction(Complete, Total);
	}
	public ItemInfoCat(string name) {
		Name = name;
	}
	public void Add(ItemInfo item) {
		Items.Add(item);
		Total += 1;
		if (item.Complete) Complete += 1;
	}
	public string Print() {
		var b = new StringBuilder();
		for (var step = 0; step < 2; step++) {
			var complete = step == 1;
			var first = true;
			foreach (var item in Items) {
				if (item.Complete != complete) continue;
				if (first) {
					first = false;
					b.AppendLine(complete ? "Complete" : "Incomplete");
					b.AppendLine("  ID\tGot\tTgt\tItem");
				}
				b.AppendLine($"{item.ID,4}\t{item.Have}\t{item.Needed}\t{item.Name}");
			}
			if (first) {
				if (!complete) {
					b.AppendLine("All done!");
				}
			}
			if (step == 0) {
				b.AppendLine("");
			}
		}
		return b.ToString();
	}
	public void SaveTo(string path) {
		File.WriteAllText(path, Print());
	}
	override public string ToString() {
		return $"{Name}: {Complete}/{Total} ({Percent})";
	}
}
