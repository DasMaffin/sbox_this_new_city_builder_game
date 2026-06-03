using Sandbox.UI;
using System;

public class CircleSegment : PolygonPanel
{
	private readonly float _innerRadius, _outerRadius, _degrees, _startAngle;
	private readonly int _resolution;
	Color _color, _hoverColor;
	public CircleSegment( float innerRadius, float outerRadius, float degrees, float startAngle, Color color, int resolution = 32, Color hoverColor = new Color())
	{
		_innerRadius = innerRadius;
		_outerRadius = outerRadius;
		_degrees = degrees;
		_startAngle = startAngle;
		_color = color;
		_resolution = resolution;
		_hoverColor = hoverColor == new Color() ? color : hoverColor;

		OnScreenSizeChanged += CircleSegment_OnScreenSizeChanged;
		OnHoverEnter += CircleSegment_OnHoverEnter;
		OnHoverExit += CircleSegment_OnHoverExit;
	}

	private void CircleSegment_OnHoverExit(MousePanelEvent mpe)
	{
		polygon.SetColor( _color );
	}

	private void CircleSegment_OnHoverEnter(MousePanelEvent mpe)
	{
		polygon.SetColor( _hoverColor );
	}

	private void CircleSegment_OnScreenSizeChanged()
	{
		initInternal( BuildTriangles( _innerRadius, _outerRadius, _degrees, _startAngle, _resolution, ScaleToScreen ), _color );
		Texture tex = Texture.Load( "wuthering_waves_lynae_build_guide_best_weapons_echoes_and_teams_hero_f40ffa2cd8.png" );

		polygon.SetTexture( tex );
	}

	private Triangle[] BuildTriangles( float innerRadius, float outerRadius, float degrees, float startAngle, int resolution, float ScaleToScreen )
	{
		var triangles = new Triangle[resolution * 2];
		Vector2 center = (Screen.Size / 2f) / ScaleToScreen;
		for ( int i = 0; i < resolution; i++ )
		{
			float a0 = (startAngle + degrees * (i / (float)resolution)) * MathF.PI / 180f;
			float a1 = (startAngle + degrees * ((i + 1) / (float)resolution)) * MathF.PI / 180f;

			Vector2 outerA = center + new Vector2( MathF.Cos( a0 ), MathF.Sin( a0 ) ) * outerRadius;
			Vector2 outerB = center + new Vector2( MathF.Cos( a1 ), MathF.Sin( a1 ) ) * outerRadius;
			Vector2 innerA = center + new Vector2( MathF.Cos( a0 ), MathF.Sin( a0 ) ) * innerRadius;
			Vector2 innerB = center + new Vector2( MathF.Cos( a1 ), MathF.Sin( a1 ) ) * innerRadius;

			triangles[i * 2 + 0] = new Triangle( outerA, outerB, innerA );
			triangles[i * 2 + 1] = new Triangle( innerA, outerB, innerB );
		}

		return triangles;
	}
}
