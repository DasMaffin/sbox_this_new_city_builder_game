using Sandbox.Diagnostics;
using SWB.Demo;
using System;
using System.Runtime.InteropServices;

public sealed partial class GameManager : GameObjectSystem<GameManager>, Component.INetworkListener, ISceneStartup
{
	public GameManager( Scene scene ) : base( scene ) { }

	void ISceneStartup.OnHostInitialize()
	{
		if ( !Networking.IsActive )
		{
			Networking.CreateLobby( new Sandbox.Network.LobbyConfig() { Privacy = Sandbox.Network.LobbyPrivacy.Public, MaxPlayers = 32, Name = "Sandbox", DestroyWhenHostLeaves = true } );
		}
	}

	void Component.INetworkListener.OnActive( Connection channel )
	{
		channel.CanSpawnObjects = false;

		SpawnPlayer(channel);
	}

	public void SpawnPlayer(Connection c)
	{
		Assert.True( Networking.IsHost, $"Client tried to SpawnPlayer: " );

		if ( Scene.GetAll<Player>().Any( x => x.Network.Owner?.Id == c.Id) )
			return;

		// Find a spawn location for this player
		Transform startLocation = FindSpawnLocation().WithScale( 1 );

		// Spawn this object and make the client the owner
		GameObject playerGo = GameObject.Clone( "/prefabs/player.prefab", new CloneConfig { Name = "Maffin", StartEnabled = false, Transform = startLocation } );

		DemoPlayer player = playerGo.GetComponent<DemoPlayer>( true );
		//player.Owner = c;
		playerGo.NetworkSpawn(c);
	}

	/// <summary>
	/// Find the most appropriate place to respawn
	/// </summary>
	Transform FindSpawnLocation()
	{
		//
		// If we have any SpawnPoint components in the scene, then use those
		//
		SpawnPoint[] spawnPoints = Scene.GetAllComponents<SpawnPoint>().ToArray();

		if ( spawnPoints.Length == 0 )
		{
			return Transform.Zero;
		}

		return Random.Shared.FromArray( spawnPoints ).Transform.World;
	}
}
