public class ItemInfo {
	public short ID;
	public string Name;
	public int Have;
	public int Needed;
	public bool Complete {
		get => Have >= Needed;
		set => Have = value ? Needed : 0;
	}
	public ItemInfo(short id, string name, int have, int needed) {
		ID = id;
		Name = name;
		Have = have;
		Needed = needed;
	}
}