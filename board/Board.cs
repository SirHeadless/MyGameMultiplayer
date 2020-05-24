using Godot;
using System;

public class Board : Node2D
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    private Boolean processInput = false;

    [Export]
    public PackedScene FieldGreen;
    [Export]
    public PackedScene FieldYellow;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {

    }


    private ClickedInput getInput()
    {
        if (processInput) return new ClickedInput();
        if (Input.IsActionPressed("left_click"))
        {
            processInput = true;
            return new ClickedInput(GetGlobalMousePosition());
        }
        return new ClickedInput();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        ClickedInput clickedInput = getInput();
        if (clickedInput.gotInput()) {
            GD.Print(clickedInput.ClickedPosition);
            Vector2 coordinates = GetNode<TileMap>("Fields").WorldToMap(clickedInput.ClickedPosition);


            
            // AddChild();
            Vector2 pos = GetNode<TileMap>("Fields").MapToWorld(coordinates);
            pos = pos + GetNode<TileMap>("Fields").CellSize /2;
            FieldGreen fieldGreen = (FieldGreen) FieldGreen.Instance();
            fieldGreen.Init(pos);
            AddChild(fieldGreen);
            processInput = false;
            GD.Print(coordinates);
            GD.Print(pos);
            GD.Print("===================");
        }



    }


}
