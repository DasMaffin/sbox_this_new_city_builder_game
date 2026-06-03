using SWB.Player;
using SWB.Shared;
using static Sandbox.ModelPhysics;

namespace TNCBG;

public class EnemyController : Component, Sandbox.Component.IDamageable
{
	[Property] public GameObject Body { get; set; }
	[Property, Sync] private float Health { get; set; } = 100f;

	public void OnDamage( in Sandbox.DamageInfo info )
	{
		if ( info is not SWB.Shared.DamageInfo )
		{
			TakeDamage( SWB.Shared.DamageInfo.FromDamageInfo( info ) );
			return;
		}

		info.Shape = null;
		info.Hitbox = null;
		TakeDamage( info as SWB.Shared.DamageInfo );
	}

	[Rpc.Broadcast]
	public virtual void TakeDamage( SWB.Shared.DamageInfo info )
	{
		if ( !this.IsValid() || IsProxy /*|| !IsAlive || GodMode*/ )
			return;

		if ( info.Tags.Has( "head" ) )
			info.Damage *= 2;

		Health -= (int)info.Damage;

		if ( info.HitFlinch > 0 )
		{
			//DoHitFlinch( info.HitFlinch );
		}

		if ( info.MovementImpact.Duration > 0 )
		{
			//ApplyMovementImpact( info.MovementImpact );
		}

		if ( Health <= 0 )
		{
			Log.Info( "I should die!" );
			OnDeath( info );
		}
	}

	[Rpc.Broadcast]
	public virtual void OnDeath( SWB.Shared.DamageInfo info )
	{
		if ( !IsValid ) return;
		var attackerGO = info.Attacker;

		if ( attackerGO is not null && !attackerGO.IsProxy )
		{
			var attacker = attackerGO.Components.Get<PlayerBase>();

			if ( attacker is not null )
				attacker.Kills++;
		}

		if ( IsProxy ) return;

		//Ragdoll( info.Force, info.Origin, CharacterController.Velocity );
		//CharacterController.Velocity = 0;
		//Inventory.Clear();
		DestroyWithDelay( 0 );
	}
	public async virtual void DestroyWithDelay( float delay )
	{
		await GameTask.DelaySeconds( delay );
		GameObject.Destroy();
	}

	[Property] SkinnedModelRenderer Renderer;
	[Property] Rigidbody Rigidbody;

	protected override void OnEnabled()
	{
		base.OnEnabled();

		if ( Renderer == null )
		{
			Renderer = GetOrAddComponent<SkinnedModelRenderer>();
		}
		if ( Rigidbody == null )
		{
			Rigidbody = GetOrAddComponent<Rigidbody>();
		}

		Renderer.Set( "RunMode", 2 );
	}

	//protected override void OnFixedUpdate()
	//{
	//	Rigidbody.Sleeping = false;
	//	Vector3 movePos = new Vector3( Renderer.RootMotion.Position * GameObject.WorldRotation.Normal );
	//	movePos.z = 0;
	//	GameObject.WorldPosition = GameObject.WorldPosition + movePos;
	//	GameObject.WorldRotation *= Renderer.RootMotion.Rotation;
	//}
}
