using System;
using System.Collections.Generic;

public class City
{
	public Guid Id { get; set; }
	public HashSet<Player> Citizens { get; set; } = new HashSet<Player>();
	public City()
	{
		Id = Guid.CreateVersion7();
	}

	public City( Player player )
	{
		Citizens.Add( player );
		Id = Guid.CreateVersion7();
	}

	public void AddCitizen(Player p)
	{
		Citizens.Add(p);
	}
}
