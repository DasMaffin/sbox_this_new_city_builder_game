using Sandbox;
using Sandbox.Rendering;

public class DrawableShape
{
	private GpuBuffer<Vertex> _buffer;
	private Vertex[] _verts;
	private int _vertCount;
	private Triangle[] triangles;
	private Triangle[] bounds;
	private Color color; 
	private Texture _texture;
	public DrawableShape( Triangle[] triangles, Color color )
	{
		this.triangles = triangles;
		this.bounds = triangles;
		this.color = color;
		_vertCount = triangles.Length * 3;
		_buffer = new GpuBuffer<Vertex>( _vertCount, GpuBuffer.UsageFlags.Vertex );
		_verts = new Vertex[_vertCount];
	}

	public DrawableShape( Triangle[] triangles, Triangle[] bounds, Color color )
	{
		this.triangles = triangles;
		this.bounds = bounds;
		this.color = color;
		_vertCount = triangles.Length * 3;
		_buffer = new GpuBuffer<Vertex>( _vertCount, GpuBuffer.UsageFlags.Vertex );
		_verts = new Vertex[_vertCount];
	}

	public void SetTexture( Texture texture )
	{
		_texture = texture;
	}

	public void SetColor(Color c )
	{
		color = c;
	}

	public void Draw( in CommandList cl, float scale )
	{
		Vector2 min = new Vector2( float.MaxValue, float.MaxValue );
		Vector2 max = new Vector2( float.MinValue, float.MinValue );
		foreach ( var t in triangles )
		{
			min = Vector2.Min( min, new Vector2( t.A.x, t.A.y ) );
			min = Vector2.Min( min, new Vector2( t.B.x, t.B.y ) );
			min = Vector2.Min( min, new Vector2( t.C.x, t.C.y ) );
			max = Vector2.Max( max, new Vector2( t.A.x, t.A.y ) );
			max = Vector2.Max( max, new Vector2( t.B.x, t.B.y ) );
			max = Vector2.Max( max, new Vector2( t.C.x, t.C.y ) );
		}
		Vector2 size = max - min;

		Vector2 uvScale = Vector2.One;
		Vector2 uvOffset = Vector2.Zero;
		if ( _texture != null )
		{
			float shapeAspect = size.x / size.y;
			float texAspect = (float)_texture.Width / _texture.Height;
			if ( shapeAspect > texAspect )
				uvScale = new Vector2( 1f, texAspect / shapeAspect );
			else
				uvScale = new Vector2( shapeAspect / texAspect, 1f );
			uvOffset = (Vector2.One - uvScale) / 2f;
		}

		Color32 c32 = color.ToColor32();
		for ( int i = 0; i < triangles.Length; i++ )
		{
			var t = triangles[i];
			Vector2 uvA = uvOffset + ((new Vector2( t.A.x, t.A.y ) - min) / size) * uvScale;
			Vector2 uvB = uvOffset + ((new Vector2( t.B.x, t.B.y ) - min) / size) * uvScale;
			Vector2 uvC = uvOffset + ((new Vector2( t.C.x, t.C.y ) - min) / size) * uvScale;

			_verts[i * 3 + 0] = new Vertex( t.A * scale, (Vector4)uvA, c32 );
			_verts[i * 3 + 1] = new Vertex( t.B * scale, (Vector4)uvB, c32 );
			_verts[i * 3 + 2] = new Vertex( t.C * scale, (Vector4)uvC, c32 );
		}

		_buffer.SetData( _verts );
		if ( _texture != null )
		{
			cl.Attributes.SetCombo( "D_BACKGROUND_IMAGE", 1 );
			cl.Attributes.Set( "Texture", _texture );
			cl.Attributes.Set( "BoxPosition", Vector2.Zero );
			cl.Attributes.Set( "BoxSize", new Vector2( _texture.Width, _texture.Height ) );
			cl.Attributes.Set( "BgPos", new Vector4( 0, 0, _texture.Width, _texture.Height ) );
			cl.Attributes.Set( "BgTint", new Vector4( color.r, color.g, color.b, color.a ) );
			cl.Attributes.Set( "BgRepeat", 3 );
		}
		else
		{
			cl.Attributes.SetCombo( "D_BACKGROUND_IMAGE", 0 );
			cl.Attributes.Set( "BoxPosition", Vector2.Zero );
			cl.Attributes.Set( "BoxSize", new Vector2( 1, 1 ) );
		}

		cl.Draw( _buffer, Material.UI.Box, 0, _vertCount );
	}

	public bool ContainsPoint( Vector2 point )
	{
		foreach ( var t in bounds )
		{
			if ( PointInTriangle( point,
				new Vector2( t.A.x, t.A.y ),
				new Vector2( t.B.x, t.B.y ),
				new Vector2( t.C.x, t.C.y ) ) )
				return true;
		}
		return false;
	}

	private static bool PointInTriangle( Vector2 p, Vector2 a, Vector2 b, Vector2 c )
	{
		float d1 = Sign( p, a, b );
		float d2 = Sign( p, b, c );
		float d3 = Sign( p, c, a );

		bool hasNeg = d1 < 0 || d2 < 0 || d3 < 0;
		bool hasPos = d1 > 0 || d2 > 0 || d3 > 0;

		return !(hasNeg && hasPos);
	}

	private static float Sign( Vector2 p1, Vector2 p2, Vector2 p3 )
	{
		return (p1.x - p3.x) * (p2.y - p3.y) - (p2.x - p3.x) * (p1.y - p3.y);
	}

	public void Dispose()
	{
		_buffer?.Dispose();
		_buffer = null;
	}

	internal void UpdateTriangles( Triangle[] triangles )
	{
		this.triangles = triangles;
	}
}
