using Godot;
using Player;
using System;

public partial class DebugTest : Control
{
    private Script serverScript = GD.Load<Script>("res://Scripts/Managers/Server.cs");
	private Script clientScript = GD.Load<Script>("res://Scripts/Managers/Client.cs");

    public void host() {
        Server.Server server = new Server.Server();        
        AddChild(server);
    }

    public void join() {
        Client client = new Client();
        AddChild(client);
        GD.Print("?");
    }
}
