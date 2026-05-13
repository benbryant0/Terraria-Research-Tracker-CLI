using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

public class ResearchConfig {
	public HashSet<short> Omit = new HashSet<short>();
	public List<List<short>> Aliases = new List<List<short>>();
	public ResearchConfig() {}

	static readonly Regex RxID = new Regex("#(\\d+)");
	static readonly Regex RxOmit = new Regex("^omit\\b(.+)");
	static readonly Regex RxAlias = new Regex("^alias\\b(.+)");
	public void Parse(IEnumerable<string> lines) {
		foreach (var line in lines) {
			Match mt;
			if ((mt = RxOmit.Match(line)).Success) {
				foreach (Match imt in RxID.Matches(mt.First())) {
					if (short.TryParse(imt.First(), out var id)) {
						if (!Omit.Contains(id)) Omit.Add(id);
					}
				}
			} else if ((mt = RxAlias.Match(line)).Success) {
				var list = new List<short>();
				foreach (Match imt in RxID.Matches(mt.First())) {
					if (short.TryParse(imt.First(), out var id)) {
						if (!list.Contains(id)) list.Add(id);
					}
				}
				Aliases.Add(list);
			}
		}
	}
	public void Load(string path) {
		var lines = File.ReadLines(path);
		Parse(lines);
	}
}
static class RegexTools {
	public static string First(this Match mt) {
		return mt.Groups[1].Value;
	}
}