using System.Threading.Tasks;

namespace SWB.Demo;

[Group( "SWB" )]
[Title( "Demo NetworkManager" )]
public class DemoNetworkManager : Component, Component.INetworkListener, ISceneStartup
{
	[Property] public SceneFile MainScene { get; set; }
	[Property] public PrefabScene PlayerPrefab { get; set; }
	[Property] public PrefabScene BotPrefab { get; set; }

	//protected override Task OnLoad()
	//{
	//	if ( !Networking.IsActive )
	//		Networking.CreateLobby( new() );

	//	return base.OnLoad();
	//}

	void ISceneStartup.OnHostInitialize()
	{
		if ( !Networking.IsActive )
		{
			Networking.CreateLobby( new Sandbox.Network.LobbyConfig() { Privacy = Sandbox.Network.LobbyPrivacy.Public, MaxPlayers = 128, Name = "This new City Builder Game", DestroyWhenHostLeaves = true } );
		}
	}

	// Called on host
	void INetworkListener.OnActive( Connection connection )
	{
		var playerGO = PlayerPrefab.Clone();
		playerGO.Name = "Player";
		playerGO.NetworkSpawn( connection );
	}
}
