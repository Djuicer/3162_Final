using Godot;
using System.Collections.Generic;  // 用于 List<T>

public partial class SaveManager : Node
{
	public static SaveManager Instance { get; private set; }

	public override void _EnterTree()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		else
		{
			QueueFree();
		}
	}
	
	private const string SaveDir = "user://saves/";
	private const int MaxSlots = 3;

	// 使用 Godot.Collections.Dictionary
	public Godot.Collections.Dictionary GetDefaultSaveData(string playerName)
	{
		return new Godot.Collections.Dictionary
		{
			{ "player_name", playerName },
			{ "current_day", 1 },
			{ "knowledge", 1 },
			{ "coding_skill", 1 },
			{ "energy", 5 },
			{ "confidence", 2 },
			{ "portfolio_progress", 0 },
			{ "has_seen_opening", false },
			{ "scene_path", "res://scenes/Domitory/dormitory.tscn" }
		};
	}

	private void EnsureSaveDir()
	{
		if (!DirAccess.DirExistsAbsolute(SaveDir))
			DirAccess.MakeDirRecursiveAbsolute(SaveDir);
	}

	private string GetSavePath(int slot) => $"{SaveDir}save_{slot}.json";

	public bool HasSave(int slot) => FileAccess.FileExists(GetSavePath(slot));

	// 保存：参数使用 Godot.Collections.Dictionary
	public Error SaveGame(int slot, Godot.Collections.Dictionary data)
	{
		EnsureSaveDir();
		var path = GetSavePath(slot);
		using var file = FileAccess.Open(path, FileAccess.ModeFlags.Write);
		if (file == null) return Error.Failed;
		string json = Json.Stringify(data, "\t");
		file.StoreString(json);
		return Error.Ok;
	}

	// 加载：返回 Godot.Collections.Dictionary
	public Godot.Collections.Dictionary LoadGame(int slot)
	{
		string path = GetSavePath(slot);
		if (!FileAccess.FileExists(path)) 
			return new Godot.Collections.Dictionary();
		
		using var file = FileAccess.Open(path, FileAccess.ModeFlags.Read);
		string content = file.GetAsText();
		var data = Json.ParseString(content);
		
		if (data.VariantType == Variant.Type.Nil || data.AsGodotDictionary().Count == 0)
			return new Godot.Collections.Dictionary();
		
		return data.AsGodotDictionary();
	}

	public List<SlotInfo> GetAllSlotsInfo()
	{
		var info = new List<SlotInfo>();
		for (int i = 0; i < MaxSlots; i++)
		{
			bool has = HasSave(i);
			string displayText;
			if (has)
			{
				var data = LoadGame(i);
				string name = data["player_name"].AsString();
				int day = data.ContainsKey("current_day") ? data["current_day"].AsInt32() : 1;
				int portfolio = data.ContainsKey("portfolio_progress") ? data["portfolio_progress"].AsInt32() : 0;
				displayText = $"{name}  Day {day}  Portfolio {portfolio}%";
			}
			else
			{
				displayText = "empty save";
			}
			info.Add(new SlotInfo { HasData = has, DisplayText = displayText });
		}
		return info;
	}

	public Godot.Collections.Dictionary CreateNewSave(int slot, string playerName)
	{
		var newData = GetDefaultSaveData(playerName);
		SaveGame(slot, newData);
		return newData;
	}

	public struct SlotInfo
	{
		public bool HasData;
		public string DisplayText;
	}
}
