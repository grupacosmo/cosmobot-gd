using Godot;
using System;

public partial class VolumeSlider : HSlider
{
	private String _busName;
	private int _busIndex;

	public override void _Ready()
	{
		_busIndex = AudioServer.GetBusIndex(_busName);
		Connect("value_changed", new Callable(this, nameof(_on_value_changed)));
		float value = Mathf.DbToLinear(AudioServer.GetBusVolumeDb(_busIndex));
	}
	
	public void _on_value_changed(float value)
	{
		AudioServer.SetBusVolumeDb(_busIndex, Mathf.LinearToDb(value));
	}
}
