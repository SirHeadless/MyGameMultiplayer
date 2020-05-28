using Godot;
using System;

public class Board : Node2D
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    private Boolean processingInput = false;

    [Export]
    public PackedScene FieldGreen;
    [Export]
    public PackedScene FieldYellow;

    private string server;
    // WebSocket Client
    private WsClient client;


    /* Attempt to use Godot.Websocketclient
        private WebsocketService wsService = new WebsocketService();

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {

            Godot.WebSocketClient ws = new Godot.WebSocketClient();

            ws.ConnectToUrl("ws://127.0.0.1:8080/ws-chat/123?name=Hostname");

            GD.Print("CONNECTED");
        }
    */

    /* Attempt to use own client */
    public override void _Ready()
    {
        server = "ws://127.0.0.1:8080/ws-chat/123?name=Hostname" + System.Guid.NewGuid();
        client = new WsClient(server);
        ConnectToServer();

    }


    public async void ConnectToServer()
    {
        await client.Connect();
    }

    // public override void _Input(InputEvent @event) {
        
    // }

    private ClickedInput getInput()
    {
        if (processingInput) return new ClickedInput();
        if (Input.IsActionPressed("left_click"))
        {
            processingInput = false;
            return new ClickedInput(GetGlobalMousePosition());
        }
        return new ClickedInput();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        ClickedInput clickedInput = getInput();
        if (clickedInput.gotInput())
        {
            GD.Print(clickedInput.ClickedPosition);
            Vector2 coordinates = GetNode<TileMap>("Fields").WorldToMap(clickedInput.ClickedPosition);

            client.Send(coordinates.x + "," + coordinates.y);

            // displayOnField(coordinates);




        }

        var cqueue = client.receiveQueue;
        string msg;
        while (cqueue.TryPeek(out msg))
        {
            // Parse newly received messages
            cqueue.TryDequeue(out msg);
            GD.Print("**************************************");
            GD.Print(msg);
            char[] charSeparators = new char[] { ',' };
            string[] result = msg.Split(charSeparators, StringSplitOptions.None);
            try
            {
                GD.Print(new Vector2(result[0].ToFloat(), result[1].ToFloat()));
                displayOnField(new Vector2(result[0].ToFloat(), result[1].ToFloat()));
            }
            catch (Exception ex)
            {
                GD.Print(msg);
                GD.Print(ex);
            }
            GD.Print("**************************************");
        }
    }

    public void displayOnField(Vector2 coordinates)
    {
        // AddChild();
        Vector2 pos = GetNode<TileMap>("Fields").MapToWorld(coordinates);
        pos = pos + GetNode<TileMap>("Fields").CellSize / 2;
        FieldGreen fieldGreen = (FieldGreen)FieldGreen.Instance();
        fieldGreen.Init(pos);
        AddChild(fieldGreen);
        processingInput = false;
        GD.Print(coordinates);
        GD.Print(pos);
        GD.Print("===================");
    }



}
