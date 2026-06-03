using Sandbox;
using Sandbox.Mapping;
using Sandbox.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using static Sandbox.PhysicsGroupDescription.BodyPart;

[Title( "Low Poly Terrain" )]
public class LowPolyTerrain : Component, Component.ExecuteInEditor
{
	[Property, Change("GenerateMesh")] public int Width { get; set; } = 20;
	[Property, Change( "GenerateMesh" )] public int Depth { get; set; } = 20;
	[Property, Change( "GenerateMesh" )] public float Scale { get; set; } = 3f; 
	[Property, Change("GenerateMesh")] public float QuadSize { get; set; } = 16f;
	[Property, Change( "GenerateMesh" )] public float HeightMultiplier { get; set; } = 2f;
	[Property, Range( 0f, 1f ), Change( "GenerateMesh" )] public float ShadingIntensity { get; set; } = 0.3f;
	[Property, Change( "GenerateMesh" )] public Color TerrainColor { get; set; } = new Color( 0.29f, 0.48f, 0.21f );
	[Property, Change( "GenerateMesh" )] public Vector2 Offset { get; set; } = Vector2.Zero;
	[Property, Change( "GenerateMesh" )] public Material Material { get; set; } = Material.Load( "materials/default/vertex_color.vmat" );
	[Property, Change( "GenerateMesh" )] public Color Tint{ get; set; } = new Color();

	protected override void OnEnabled()
	{
		GenerateMesh();
	}

	protected override void OnDisabled()
	{
		_sceneObject?.Delete();
		_sceneObject = null;
	}

	protected override void OnValidate()
	{
		GenerateMesh();
	}
	private SceneObject _sceneObject;
	void GenerateMesh()
	{
		Log.Info( "Generating ground mesh" );
		Vector3[] grid = new Vector3[(Width + 1) * (Depth + 1)];

		for (int x = 0; x <= Width; x++ )
		{
			for (int y = 0; y <= Depth; y++ )
			{
				float nx = (x + Offset.x) / Scale;
				float nz = (y + Offset.y) / Scale;
				float z = Noise.Perlin( nx, nz ) * HeightMultiplier * QuadSize;
				grid[y * (Width + 1) + x] = new Vector3( x * QuadSize, y * QuadSize, z );
			}
		}

		List<Vertex> vertices = new List<Vertex>();
		List<int> indices = new List<int>();

		for ( int z = 0; z < Depth; z++ )
		{
			for ( int x = 0; x < Width; x++ )
			{
				var a = grid[z * (Width + 1) + x];
				var b = grid[(z + 1) * (Width + 1) + x];
				var c = grid[z * (Width + 1) + (x + 1)];
				var d = grid[(z + 1) * (Width + 1) + (x + 1)];

				AddFlatTri( vertices, indices, a, c, b );
				AddFlatTri( vertices, indices, c, d, b );
			}
		}

		Mesh mesh = new Mesh();
		mesh.CreateVertexBuffer( vertices.Count, vertices.ToArray() );
		mesh.CreateIndexBuffer( indices.Count, indices.ToArray() );
		// Meshes now render via explicit sub-mesh draw calls. Setting mesh.Material alone
		// no longer registers one, so post-update the terrain drew nothing — add a sub-mesh
		// covering the whole index buffer instead.
		mesh.AddSubMesh( Material, 0, indices.Count );

		ModelBuilder mb = new ModelBuilder();
		mb.AddMesh( mesh );
		List<int> collisionIndices = new List<int>();

		for ( int z = 0; z < Depth; z++ )
		for ( int x = 0; x < Width; x++ )
		{
			int a = z       * (Width + 1) + x;
			int b = (z + 1) * (Width + 1) + x;
			int c = z       * (Width + 1) + (x + 1);
			int d = (z + 1) * (Width + 1) + (x + 1);

			collisionIndices.AddRange( new[] { a, c, b } );
			collisionIndices.AddRange( new[] { c, d, b } );
		}

		mb.AddCollisionMesh( grid, collisionIndices.ToArray() );

		Model model = mb.Create();

		// Using SceneObject instead of ModelRenderer because the clutterer doesnt work with procedurally generated models when it is applied to one.
		_sceneObject?.Delete();
		_sceneObject = new SceneObject( Scene.SceneWorld, model );
		_sceneObject.Transform = WorldTransform; 
		_sceneObject.SetMaterialOverride( Material );
		_sceneObject.ColorTint = Tint;

		Components.GetOrCreate<ModelCollider>().Model = model;
	}

	void AddFlatTri( List<Vertex> verts, List<int> indices, Vector3 v0, Vector3 v1, Vector3 v2 )
	{
		Vector3 normal = Vector3.Cross( v1 - v0, v2 - v0 ).Normal;
		float dot = MathF.Max( 0f, Vector3.Dot( normal, Vector3.Up ) );
		float shade = MathX.Lerp( 1f, dot, ShadingIntensity );
		Color col = new Color( TerrainColor.r * shade, TerrainColor.g * shade, TerrainColor.b * shade );

		int i = verts.Count;
		verts.Add( new Vertex( v0, normal, new Vector4( 1, 0, 0, 1 ), Vector2.Zero ) { Color = col } );
		verts.Add( new Vertex( v1, normal, new Vector4( 1, 0, 0, 1 ), Vector2.Zero ) { Color = col } );
		verts.Add( new Vertex( v2, normal, new Vector4( 1, 0, 0, 1 ), Vector2.Zero ) { Color = col } );
		indices.AddRange( new int[] { i, i + 1, i + 2 } );
	}
}
