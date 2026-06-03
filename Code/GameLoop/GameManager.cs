using System.Collections.Generic;

public sealed partial class GameManager : GameObjectSystem<GameManager>
{
	[Property] public GameObject PlayerPrefab { get; set; }
	[Sync] public List<City> allCities { get; set; } = new List<City>();
	public bool IsBuilding { get; set; }
	public Dictionary<Connection, Player> ConnectionToPlayer { get; set; } = new Dictionary<Connection, Player>();

	public GameManager( Scene scene ) : base( scene ) { }
}
