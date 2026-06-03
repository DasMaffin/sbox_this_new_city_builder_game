using Sandbox;
using Sandbox.Utility;
using System;

namespace TNCBG;

public sealed class MaffinSpawnPoint : Component
{
	[Property, Change( nameof( Snap ) )] public bool RefreshInEditor { get; set; }

	protected override void OnAwake()
	{
		Snap();
	}

	private void Snap()
	{
		var go = this.GameObject;
		var from = go.WorldPosition.WithZ( 10000f );
		var to = go.WorldPosition.WithZ( -10000f );

		var hit = Scene.Trace.Ray( from, to )
			.WithTag( "terrain" )
			.Run();

		if ( hit.Hit )
			go.WorldPosition = hit.HitPosition;
	}
}
