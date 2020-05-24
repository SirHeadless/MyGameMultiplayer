using Godot;
using System;


public class ClickedInput
{
    public Boolean GotInput;
    public Vector2 ClickedPosition;

    public ClickedInput()
    {
        GotInput = false;
    }

    public ClickedInput(Vector2 _clickedPosition)
    {
        GotInput = true;
        ClickedPosition = _clickedPosition;
    }

    public Boolean gotInput()
    {
        return GotInput;
    }
}