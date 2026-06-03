using Sandbox.UI;
using Sandbox.UI.Construct;
using SWB.Player;
using SWB.Shared;

public class RadialMenuComponent : PanelComponent
{
	[Property] public GameObject t1TownHall { get; set; }

	// Lazy load
	private PlayerBase playerBase;
	public PlayerBase PlayerBase 
	{
		get
		{
			if (playerBase == null) 
				playerBase = GetComponentInParent<PlayerBase>();

			return playerBase;
		}
	}

	// Lazy load
	private Player player;
	public Player Player
	{
		get
		{
			if ( player == null )
				player = GetComponentInParent<Player>();

			return player;
		}
	}

	public int RadialState { get; internal set; }

	private PolygonContainer container;
	private Label cityLabel;
	private CircleSegment firstTownhall;

	protected override void OnStart()
	{
		if ( IsProxy )
		{
			this.GameObject.Destroy();
			return;
		}

		container = new PolygonContainer();
		container.OnHiddenChanged += OnContainerHiddenChanged;
		Panel.AddChild( container );
		cityLabel = Panel.Add.Label( Player.MyCityId.ToString() );
	}

	protected override void OnUpdate()
	{
		cityLabel.Text = Player.MyCityId.ToString();
	}

	private void OnContainerHiddenChanged( bool hidden )
	{
		if ( hidden ) return;

		if ( GameManager.Current.allCities.Any( c => c.Citizens.Contains( Player ) ) && RadialState == 1 )
		{
			firstTownhall.OnMousePressed -= PreviewBuilding;
			container.RemovePolygon( firstTownhall );

			var segment = new CircleSegment( 200f, 350f, 56f, 2f, Color.White, hoverColor: Color.Red );
			container.AddPolygon( segment );
			container.AddPolygon( new CircleSegment( 200f, 350f, 56, 62f, Color.White ) );
			container.AddPolygon( new CircleSegment( 200f, 350f, 56f, 122f, Color.White ) );
			container.AddPolygon( new CircleSegment( 200f, 350f, 56f, 182f, Color.White ) );
			container.AddPolygon( new CircleSegment( 200f, 350f, 56f, 242f, Color.White ) );
			container.AddPolygon( new CircleSegment( 200f, 350f, 56f, 302f, Color.White ) );
			RadialState = 2;
		}
		else if(firstTownhall == null)
		{
			firstTownhall = new CircleSegment( 200f, 350f, 360f, 0f, Color.White, hoverColor: Color.Blue );
			firstTownhall.OnMousePressed += PreviewBuilding;
			container.AddPolygon( firstTownhall );
		}
	}

	public void PreviewBuilding( MousePanelEvent e)
	{
		container.Disable();
		GameObject buildingInstance = t1TownHall.Clone();
		buildingInstance.NetworkSpawn();
		BuildingController bc = buildingInstance.GetComponent<BuildingController>();
		bc.onBuildingBuilt += Player.OnBuildingBuilt;
		bc.onBuildingBuilt += (BuildingController bc) => { if(bc.buildingType == BuildingType.TownHall) RadialState = 1; };
		bc.IsBuilt = false;
		bc.Builder = Player;
		bc.bcm = this;

		if (PlayerBase.Inventory.Active?.Components.TryGet<IInventoryItem>( out var oldActive ) ?? false)
		{
			if ( !oldActive.CanCarryStop() ) return;
			oldActive.OnCarryStop();
		}
		PlayerBase.Inventory.Active = null;
		PlayerBase.Inventory.ActiveItem = null;
	}
}
