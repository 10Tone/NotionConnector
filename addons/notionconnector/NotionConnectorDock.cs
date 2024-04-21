using Godot;
using System;

[Tool]
public partial class NotionConnectorDock : Control
{
	[Signal] public delegate void ConnectPressedEventHandler();
	[Export] private NodePath _connectBtnPath;
	
	private Button _connectBtn;

	public override void _EnterTree()
	{
		base._EnterTree();
		_connectBtn = GetNode<Button>(_connectBtnPath);
		_connectBtn.Pressed += OnConnectPressed;
	}
	
	private void OnConnectPressed()
	{

		EmitSignal(SignalName.ConnectPressed);
	}
}
