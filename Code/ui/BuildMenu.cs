using Sandbox.Internal;
using Sandbox.UI;
using Sandbox.UI.Construct;
using SWB.Player;
using System;

public class BuildMenu : Panel
{
	bool isActive = true;

	public BuildMenu()
	{
		StyleSheet.Load( "/ui/BuildMenu.scss" );
	}

	public override void Tick()
	{
		if(Input.Pressed( "Menu" ) )
		{
			isActive = !isActive;
		}
		SetClass( "hide", isActive );
	}
}
