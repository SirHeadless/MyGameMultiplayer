using Godot;
using System;
using System.Threading;

public class Board : Node2D
{
	public enum Player {
		PlayerGreen=0,
		PlayerYellow=1

	}
	// Declare member variables here. Examples:
	// private int a = 2;
	// private string b = "text";

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
		GD.Print("First");

	}
	

	public async void ConnectToServer()
	{
		System.Threading.Thread.Sleep(2000);
		GD.Print("Second");
		await client.Connect();
	}

	public override void _Input(InputEvent @event) {
		// if (@event.IsActionReleased("left_click"))
		if (@event is InputEventMouseButton eventMouseButton && eventMouseButton.IsActionReleased("left_click"))
		{
			
			GD.Print("Mouse Click/Unclick at: ", eventMouseButton.Position);

			Vector2 coordinates = GetNode<TileMap>("Fields").WorldToMap(eventMouseButton.Position);

			client.Send(coordinates.x + "," + coordinates.y);
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(float delta)
	{

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
				GD.Print( (Player) result[2].ToInt());
				displayOnField(new Vector2(result[0].ToFloat(), result[1].ToFloat()), (Player) result[2].ToInt());
			}
			catch (Exception ex)
			{
				GD.Print(msg);
				GD.Print(ex);
			}
			GD.Print("**************************************");
		}
	}

	public void displayOnField(Vector2 coordinates, Player player)
	{
		// AddChild();
		Vector2 pos = GetNode<TileMap>("Fields").MapToWorld(coordinates);
		pos = pos + GetNode<TileMap>("Fields").CellSize / 2;
		Area2D field = InstantiateFieldArea2D(pos, player);
		AddChild(field);
		GD.Print(coordinates);
		GD.Print(pos);
		GD.Print("===================");
	}

	public Area2D InstantiateFieldArea2D(Vector2 pos, Player player) {
		switch(player) {
			case Player.PlayerGreen:
				GD.Print(FieldGreen._Bundled);
				FieldGreen fieldGreen = (FieldGreen)FieldGreen.Instance();
				fieldGreen.Init(pos);
				return fieldGreen;
			case Player.PlayerYellow: 		
				GD.Print(FieldYellow._Bundled);
				FieldYellow fieldYellow = (FieldYellow)FieldYellow.Instance();
				fieldYellow.Init(pos);
				return fieldYellow;
		}

		return null;
	}

}
