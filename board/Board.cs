using Godot;
using System;
using System.Threading;
using System.Collections.Generic;

public class Board : Node2D
{
	public enum Player {
		PlayerGreen=0,
		PlayerYellow=1

	}

	private static readonly int ROWS = 10;
	private static readonly int MAXCOLUMNS = 7;
	
	[Export]
	public PackedScene FieldGreen;
	[Export]
	public PackedScene FieldYellow;

	private string server;

	private WsClient client;

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

			/* Change logic 
			1. Message is received
			2. MessageHandler handles messages and might start action to change state
			3. State will be "rerendered"
			*/
			// Parse newly received messages
			cqueue.TryDequeue(out msg);
			GD.Print("**************************************");
			GD.Print(msg);
			char[] charSeparators = new char[] { ',' };
			string[] result = msg.Split(charSeparators, StringSplitOptions.None);
			if (result[0] == "move") {
				try
				{
					int x = result[1].ToInt();
					int y = result[2].ToInt();
					Player player = (Player) result[3].ToInt();

					GD.Print(new Vector2(x, y));
					GD.Print( player);
					displayMoveOnField(new Vector2(x, y), player);
				}
				catch (Exception ex)
				{
					GD.Print(msg);
					GD.Print(ex);
				}
			} 
			else if (result[0] == "valences") {
				try {
					List<List<int>> valenceMatrix = valenceMessageToValences(result);
					displayValencesOnField(valenceMatrix);
				} catch (Exception ex) {
					GD.Print(msg);
					GD.Print(ex);
				}
			}
			else if (result[0] == "captured") {
				try {
					Player player = (Player) result[1].ToInt();
					displayCapturedFieldsOnField(result, player);

				} catch (Exception ex) {
					GD.Print(msg);
					GD.Print(ex);
				}
			}
			else if (result[0] == "winner") {
				try {
					GD.Print("************** RESULT *******************");
					GD.Print("Player " + result[1] + " has " + result[2] + "points");
					GD.Print("Player " + result[3] + " has " + result[4] + "points");

				} catch (Exception ex) {
					GD.Print(msg);
					GD.Print(ex);
				}
			}


			GD.Print("**************************************");
		}
	}

	private List<List<int>> valenceMessageToValences(String[] valenceMessage) {
		GD.Print(valenceMessage);
		List<List<int>> valences = new List<List<int>>(); 
		for (int i = 0; i < MAXCOLUMNS ; i++) {
			int offset = i % 2;
			valences.Add(new List<int>());
			for (int j = 0; j < ROWS - offset; j++) {
				GD.Print(i + j + 1);
				valences[i].Add(valenceMessage[i + j + 1].ToInt());
			}
		}
		return valences;
	}

	public void displayMoveOnField(Vector2 coordinates, Player player)
	{
		// AddChild();
		Vector2 pos = GetNode<TileMap>("Fields").MapToWorld(coordinates);
		pos = pos + GetNode<TileMap>("Fields").CellSize / 2;
		Area2D field = InstantiateFieldArea2D(pos, player);
		field.ZIndex = 2;
		AddChild(field);
		GD.Print(coordinates);
		GD.Print(pos);
		GD.Print("===================");
	}

	public void displayValencesOnField(List<List<int>> valences)
	{
		TileMap valenceTileMap = GetNode<TileMap>("Valence");

		for (int i = 0; i < MAXCOLUMNS; i++) {
			int offset = i % 2;
			for (int j = 0; j < ROWS - offset; j++) {
				valenceTileMap.SetCellv(new Vector2(j,i), valences[i][j] - 1 );
			}
		}
	}

	public void displayCapturedFieldsOnField(String[] caputredFieldPositions, Player player)
	{
		GD.Print("ccccccccccccccc CAPTURED FIELDS ccccccccccccccccccccccc");
		String tileName = (player == Player.PlayerGreen) ? "captured_green" : "captured_yellow";
		TileMap figuresTileMap = GetNode<TileMap>("Figures");
		GD.Print(caputredFieldPositions);
		for (int i = 2; i <= caputredFieldPositions.Length; i=i+2) {
			GD.Print(i);
			figuresTileMap.SetCellv(new Vector2(caputredFieldPositions[i].ToInt(), caputredFieldPositions[i+1].ToInt()), figuresTileMap.TileSet.FindTileByName(tileName));
		}
		GD.Print("cccccccccccccccccccccccccccccccccccccccccccccccccccccc");

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
