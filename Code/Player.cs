using System;

public class Player : Component, ILookTrace
{
	public Guid MyCityId { get; set; }
	public SteamId SteamId { get; set; }

	public bool AddToCity(City city)
	{
		if(MyCityId == Guid.Empty )
		{
			city.AddCitizen( this );
			MyCityId = city.Id;
			return true;
		}

		return false;
	}

	public void OnBuildingBuilt(BuildingController bc)
	{
		switch ( bc.buildingType )
		{
			case BuildingType.TownHall:
				City city = new City( this );
				foreach ( Connection connection in Connection.All )
				{
					if ( connection.PartyId == Connection.Local.PartyId )
						GameManager.Current.ConnectionToPlayer[connection].AddToCity( city );
				}
				GameManager.Current.allCities.Add( city );
				break;
			default:
				break;
		}
	}

	public void OnTraceHit( SceneTraceResult traceResult )
	{
		if(Input.Pressed("Use") && traceResult.HasTag( "door" ) )
		{
			Log.Info( "Döör!" );
		}
	}
}
