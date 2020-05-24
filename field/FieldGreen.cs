using Godot;
using System;

public class FieldGreen : Area2D
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
    }
    
    public FieldGreen Init(Vector2 pos) {
        Position = pos;
        return this;
    }

    public FieldGreen(){
        
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
