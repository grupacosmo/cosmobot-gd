using Godot;
using System;

public partial class Options : Control
{
	private void _on_back_pressed()
	{
		GetTree().ChangeSceneToFile("res://scenes/menu.tscn");
	}
	private void _on_volume_pressed()
	{
		GetTree().ChangeSceneToFile("res://scenes/volume_options.tscn");
	}
}
