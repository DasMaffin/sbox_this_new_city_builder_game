using Sandbox.Diagnostics;

public sealed partial class Initialize : GameObjectSystem<Initialize>, Component.INetworkListener, ISceneStartup
{
	public Initialize( Scene scene ) : base( scene ) { }

	void ISceneStartup.OnHostInitialize()
	{
		if ( !Networking.IsActive )
		{
			Networking.CreateLobby( new Sandbox.Network.LobbyConfig() { Privacy = Sandbox.Network.LobbyPrivacy.Public, MaxPlayers = 128, Name = "This new City Builder Game", DestroyWhenHostLeaves = true } );
		}
	}

	void Component.INetworkListener.OnActive( Connection channel )
	{
		SpawnPlayer( channel );
	}

	public void SpawnPlayer( Connection c )
	{
		Assert.True( Networking.IsHost, $"Client tried to SpawnPlayer: " );

		if ( Scene.GetAll<Player>().Any( x => x.Network.Owner?.Id == c.Id ) )
			return;

		// Spawn this object and make the client the owner
		var playerGO = GameManager.Current.PlayerPrefab.Clone();
		playerGO.Name = "Player";
		playerGO.NetworkSpawn( c );

		Player player = GameManager.Current.ConnectionToPlayer.FirstOrDefault( p => p.Value.SteamId == c.SteamId ).Value;
		if ( player == null )
		{
			GameManager.Current.ConnectionToPlayer.Add( c, playerGO.GetOrAddComponent<Player>() );
			GameManager.Current.ConnectionToPlayer[c].SteamId = c.SteamId;
		}
		else
		{
			Player p = playerGO.GetOrAddComponent<Player>();
			p = player;
		}
	}
}
