using Sandbox;
using Sandbox.Utility;
using System;

namespace TNCBG;

public sealed class MaffinSpawnPoint : Component
{
	[Property, Change( nameof( Snap ) )] public bool RefreshInEditor { get; set; }
	[Property] public LowPolyTerrain Terrain { get; set; }

	private void Snap()
	{
		var go = this.GameObject;

		float nx = (go.WorldPosition.x + Terrain.Offset.x) / Terrain.Scale;
		float nz = (go.WorldPosition.y + Terrain.Offset.y) / Terrain.Scale;
		float z = Noise.Perlin( nx, nz ) * Terrain.HeightMultiplier * Terrain.QuadSize;

		go.WorldPosition = new Vector3( go.WorldPosition.x, go.WorldPosition.y, z );
		go.SetParent( Scene );
	}
}
