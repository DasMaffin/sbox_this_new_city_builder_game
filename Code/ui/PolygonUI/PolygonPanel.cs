using Sandbox.Rendering;
using Sandbox.UI;
using System;

public class PolygonPanel : Panel, IPanelDraw
{
	public event Action<MousePanelEvent> OnHoverEnter;
	public event Action<MousePanelEvent> OnHoverExit;
	public event Action<MousePanelEvent> OnMousePressed;
	protected event Action OnScreenSizeChanged;
	protected DrawableShape polygon;

	private bool hovering;
	private Vector2 _lastScreenSize;

	protected PolygonPanel() { }

	public PolygonPanel( Triangle[] triangles )
	{
		initInternal( triangles, Color.Red );
	}

	public PolygonPanel( Triangle[] triangles, Color color )
	{
		initInternal( triangles, color );
	}

	protected void initInternal( Triangle[] triangles, Color color )
	{
		polygon = new DrawableShape( triangles, color );
	}

	void IPanelDraw.Draw( CommandList cl )
	{
		polygon?.Draw( in cl, ScaleToScreen );
	}

	public override void OnDeleted()
	{
		polygon.Dispose();
	}

	public void HandleMouseMove( MousePanelEvent e )
	{
		// double dipping the events because ISceneEvent doesnt exist on Panels so they wont subscribe.
		bool inside = polygon.ContainsPoint( Mouse.Position / ScaleToScreen );
		if ( inside && !hovering )
		{
			OnHoverEnter?.Invoke(e);
			hovering = true;
		}
		else if ( !inside && hovering )
		{
			OnHoverExit?.Invoke(e);
			hovering = false;
		}
	}

	public void HandleMouseDown( MousePanelEvent e )
	{
		if ( hovering ) 
			OnMousePressed?.Invoke(e);
	}

	public override void Tick()
	{
		if ( Screen.Size != _lastScreenSize )
		{
			_lastScreenSize = Screen.Size;
			OnScreenSizeChanged?.Invoke();
		}
	}
}
