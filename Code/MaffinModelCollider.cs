using System;
using System.Collections.Generic;

namespace TNCBG;

public sealed class MaffinModelCollider : ModelCollider, Component.DontExecuteOnServer
{
	private bool ShouldExecute
	{
		get
		{
			if ( Scene == null )
			{
				return false;
			}

			if ( Scene.IsEditor && !(this is ExecuteInEditor) )
			{
				return false;
			}

			if ( Application.IsDedicatedServer && this is DontExecuteOnServer )
			{
				return false;
			}

			return true;
		}
	}

	protected override void OnValidate()
	{
		if ( !ShouldExecute ) return;

		base.OnValidate();

		Model source;
		if ( Model != null && !Model.IsError )
		{
			source = Model;
		}
		else
		{
			source = GetComponent<ModelRenderer>(true).Model;
		}
		if ( source == null ) { Log.Warning( $"A model collider on {GameObject.Name} is missing a model. No physics are generated." ); return; }

		Vertex[] verts = source.GetVertices();
		int[] intIndices = Array.ConvertAll( source.GetIndices(), i => (int)i );

		Mesh mesh = new Mesh( Material.Load( "materials/default.vmat" ) );

		mesh.CreateVertexBuffer( verts.Length, verts.AsSpan() );
		mesh.CreateIndexBuffer( intIndices.Length, intIndices.AsSpan() );

		List<Vector3> positions = Array.ConvertAll( verts, v => v.Position ).ToList();
		List<int> collisionIndices = intIndices.ToList();

		Model model = Model.Builder
			.AddMesh( mesh )
			.AddCollisionMesh( positions, collisionIndices )
			.Create();

		Model = model;		
	}
}
