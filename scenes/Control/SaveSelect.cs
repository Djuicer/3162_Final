using Godot;
using System.Collections.Generic;

public partial class SaveSelect : Control
{
	[Export] private Button[] slotButtons; // 在编辑器中将三个按钮拖入此数组

	private List<SaveManager.SlotInfo> slotsInfo;

	public override void _Ready()
	{
		// 为每个槽位按钮绑定点击事件，并传递索引
		for (int i = 0; i < slotButtons.Length; i++)
		{
			int slotIndex = i;
			slotButtons[i].Pressed += () => OnSlotPressed(slotIndex);
		}

		RefreshSlots();
	}

	private void RefreshSlots()
	{
		var saveMgr = GetNode<SaveManager>("/root/SaveManager");
		slotsInfo = saveMgr.GetAllSlotsInfo();
		var global = GetNode<GlobalVars>("/root/GVars");
		bool isLoadMode = global.SaveSelectMode == "load";

		for (int i = 0; i < slotButtons.Length; i++)
		{
			slotButtons[i].Text = slotsInfo[i].DisplayText;
			if (isLoadMode && !slotsInfo[i].HasData)
				slotButtons[i].Disabled = true;
			else
				slotButtons[i].Disabled = false;
		}
	}

	private void OnSlotPressed(int slot)
	{
		var global = GetNode<GlobalVars>("/root/GVars");
		if (global.SaveSelectMode == "start")
		{
			if (slotsInfo[slot].HasData)
				AskOverwrite(slot);
			else
				AskNewNameAndStart(slot);
		}
		else // load mode
		{
			if (slotsInfo[slot].HasData)
				LoadExistingGame(slot);
		}
	}

	private void AskOverwrite(int slot)
	{
		var confirm = new ConfirmationDialog
		{
			Title = "Cover Saving",
			DialogText = "There was a saving here, cover it and start a new one?",
			OkButtonText = "Cover",
			CancelButtonText = "Cancel"
		};
		AddChild(confirm);
		confirm.Confirmed += () => AskNewNameAndStart(slot);
		confirm.PopupCentered();
	}

	private void AskNewNameAndStart(int slot)
	{
		var nameDialog = new AcceptDialog { Title = "New Game" };
		var vbox = new VBoxContainer();
		var label = new Label { Text = "Please enter your name:" };
		var lineEdit = new LineEdit { PlaceholderText = "Your Name" };
		vbox.AddChild(label);
		vbox.AddChild(lineEdit);
		nameDialog.AddChild(vbox);
		nameDialog.OkButtonText = "Begin";
		AddChild(nameDialog);
		nameDialog.PopupCentered();
		nameDialog.Confirmed += () =>
		{
			string playerName = lineEdit.Text.Trim();
			if (string.IsNullOrEmpty(playerName)) playerName = "User";
			var saveMgr = GetNode<SaveManager>("/root/SaveManager");
			saveMgr.CreateNewSave(slot, playerName);
			StartGame(slot);
		};
	}

	private void StartGame(int slot)
	{
		var global = GetNode<GlobalVars>("/root/GVars");
		global.CurrentSlot = slot;
		var saveMgr = GetNode<SaveManager>("/root/SaveManager");
		var data = saveMgr.LoadGame(slot);
		GetTree().ChangeSceneToFile("res://scenes/Domitory/dormitory.tscn");
	}

	private void LoadExistingGame(int slot) => StartGame(slot);

	private void OnBackPressed()
	{
		GetTree().ChangeSceneToFile("res://scenes/Control/MainMenu.tscn");
	}
}
