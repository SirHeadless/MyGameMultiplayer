using Godot;
using System;

public class FigurePlayer2 : Area2D
{
	// Declare member variables here. Examples:
	// private int a = 2;
	// private string b = "text";

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
	}
	
	public FigurePlayer2 Init(Vector2 pos) {
		Position = pos;
		return this;
	}

	public FigurePlayer2(){
		
	}

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
