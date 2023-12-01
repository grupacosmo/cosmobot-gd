using Godot;
using System;

public partial class VolumeOptions : Control
{
	private void _on_back_pressed()
	{
		GetTree().ChangeSceneToFile("res://scenes/options.tscn");
	}
}
