using Sandbox;
using SWB.Base;
using SWB.Player;
public class Player : PlayerBase
{
	[Property] public Connection Owner;

	[Rpc.Host]
	void GiveWeapon( string className, bool setActive = false )
	{
		var weapon = WeaponRegistry.Instance.Get( className );

		if ( weapon is null )
		{
			Log.Error( $"[SWB Demo] {className} not found in WeaponRegistry!" );
			return;
		}

		Inventory.AddClone( weapon.GameObject, setActive );
		SetAmmo( weapon.Primary.AmmoType, 360 );
	}

	public override void Respawn(Transform? respawnAt = null)
	{
		base.Respawn(respawnAt);

		GiveWeapon( "maffin_scarh", true );
		GiveWeapon( "generic_weapon_test");
	}
}
