using Sandbox.UI;
using System;
using System.Collections.Generic;

public class PolygonContainer : Panel
{
	//Don't question it.
	private bool isActive = true;
	private bool IsHidden
	{
		set
		{
			isActive = value;
			OnHiddenChanged?.Invoke( isActive );
		}
		get
		{
			return isActive;
		}
	}

	public event Action<bool> OnHiddenChanged;

	private List<PolygonPanel> _polygons = new();

	public PolygonContainer()
	{
		StyleSheet.Load( "/ui/PolygonUI/PolygonContainer.scss" );
		Style.Set( "position:absolute;width:100%;height:100%;pointer-events:all;" );
	}

	public T AddPolygon<T>( T polygon ) where T : PolygonPanel
	{
		_polygons.Add( polygon );
		AddChild( polygon );
		polygon.Style.Set( "position:absolute;width:0;height:0;pointer-events:none;" );
		return polygon;
	}

	public void RemovePolygon( PolygonPanel polygon )
	{
		_polygons.Remove( polygon );
		polygon?.Delete();
	}

	protected override void OnMouseMove( MousePanelEvent e )
	{
		foreach ( var polygon in _polygons )
			polygon.HandleMouseMove( e );
	}

	protected override void OnMouseDown( MousePanelEvent e )
	{
		foreach ( var polygon in _polygons )
			polygon.HandleMouseDown( e );
	}

	public override void Tick()
	{
		if ( Input.Pressed( "Menu" ) )
		{
			IsHidden = !IsHidden;
		}
		SetClass( "hide", IsHidden );
	}

	public void Disable()
	{
		IsHidden = true;
		SetClass( "hide", IsHidden );
	}
}
