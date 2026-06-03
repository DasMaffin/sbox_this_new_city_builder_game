using SWB.Base;
using SWB.Player;
using SWB.Shared;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public enum BuildingType
{
	None,
	TownHall,
	Residential, 
	Defensive,
	Production
}

public sealed class BuildingController : Component, Component.ITriggerListener, ILookTrace, IWeaponSwitch
{
	[Property] public BuildingType buildingType;

	public event Action<BuildingController> onBuildingBuilt;
	public Player Builder {  get; set; }
	public RadialMenuComponent bcm;

	private List<Collider> myColliders;
	private List<Collider> MyColliders
	{
		get
		{
			if(myColliders == null)
				myColliders = GetComponents<Collider>().ToList();

			return myColliders; 
		}
	}

	private ModelRenderer myRenderer;
	private ModelRenderer MyRenderer
	{
		get
		{
			if ( myRenderer == null )
				myRenderer = GetComponent<ModelRenderer>();

			return myRenderer;
		}
	}

	private bool isBuilt;
	public bool IsBuilt 
	{
		get => isBuilt;
		set
		{
			foreach(Collider col in MyColliders )
			{
				col.IsTrigger = !value;
			}
			GameManager.Current.IsBuilding = !value;
			isBuilt = value;
		}
	}

	private bool canBeBuilt;
	public bool CanBeBuilt
	{
		get => canBeBuilt;
		set
		{
			canBeBuilt = value;
			if(value)
				MyRenderer.Tint = Color.Blue;
			else
				MyRenderer.Tint = Color.Red;
		}
	}

	private ObservableCollection<GameObject> collidingObjects = new ObservableCollection<GameObject>();

	protected override void OnEnabled()
	{
		if(buildingType == BuildingType.None )
		{
			Log.Error($"Trying to spawn building with no type. Please tell the developer to assign a type to the building \"{GameObject.Name}\"");
			GameObject.Enabled = false;
		}
	}

	protected override void OnAwake()
	{
		collidingObjects.CollectionChanged += CollidingObjects_CollectionChanged;
		if ( !IsBuilt ) CollidingObjects_CollectionChanged(collidingObjects, null);
	}

	protected override void OnUpdate()
	{
		if ( Input.Pressed( "SlotNext" ) || Input.Pressed( "SlotPrev" ) || Input.MouseWheel.y != 0 )
		{
			this.GameObject.WorldRotation *= Rotation.From( 0, Input.MouseWheel.y * 5, 0 );
		}
		if (!CanBeBuilt) return;
		if ( Input.Pressed( "Attack1" ) )
		{
			IsBuilt = true;
			MyRenderer.Tint = Color.White;
			onBuildingBuilt?.Invoke(this);
		}
	}

	private void CollidingObjects_CollectionChanged( object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e )
	{
		if ( sender is ObservableCollection<GameObject> s)
		{
			bool filtered = s.Any( go => go.Tags.HasAny( "foliage", "building", "player" ) );
			CanBeBuilt = !filtered;
		}
	}

	void ILookTrace.OnTraceHit( SceneTraceResult result )
	{
		if ( IsBuilt ) return;

		this.GameObject.WorldPosition = result.EndPosition;
	}

	void ITriggerListener.OnTriggerEnter( GameObject other )
	{
		collidingObjects.Add( other );
	}

	void ITriggerListener.OnTriggerExit( GameObject other )
	{
		collidingObjects.Remove( other );
	}

	public void OnWeaponSwitched( IInventory inventory, GameObject weapon, IInventoryItem item )
	{
		if ( isBuilt || weapon == null ) return;

		GameObject.Destroy();
	}
}
