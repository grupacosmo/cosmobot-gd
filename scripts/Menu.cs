using Godot;
using System;

public partial class Menu : Control
{
	public void _on_new_game_pressed()
	{
		GetTree().ChangeSceneToFile("res://scenes/level1.tscn");
	}


	public void _on_load_game_pressed()
	{
		// Replace with function body.
	}


	public void _on_options_pressed()
	{
		GetTree().ChangeSceneToFile("res://scenes/options.tscn");
	}


	public void _on_quit_pressed()
	{
		GetTree().Quit();
	}
}
