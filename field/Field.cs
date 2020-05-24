using Godot;
using System;
using System.Collections.Generic;
public class Field : Area2D
{
    
    private Dictionary<string, string> textures;

    private String type;


    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        textures = new Dictionary<string, string> {
            {"field", "res://assets/field.png"},
            {"figureGreen", "res://assets/figure_green.png"},
            {"figureYellow", "res://assets/figure_yellow.png"}
            };
    }

    public Field(String _type, Vector2 pos) {
        type = _type;
        Position = pos;
    }

    public void CHANGEME() {
        Vector2 coordinates = GetParent().GetNode<TileMap>("Fields").WorldToMap(Position);
        GD.Print(coordinates);
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
