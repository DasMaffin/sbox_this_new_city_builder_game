using System;

public interface ILookTrace : ISceneEvent<ILookTrace>
{
	void OnTraceHit( SceneTraceResult traceResult);
}

public sealed class ForwardTraceHandler : Component
{
	[Property] private string[] WithAnyTags { get; set; }
	[Property] private string[] WithoutTags { get; set; }

	protected override void OnUpdate()
	{
		SceneTraceResult tr = Scene.Trace.Ray( this.GameObject.WorldPosition, this.GameObject.WorldPosition + this.GameObject.WorldRotation.Forward.Normal * 1000f )
			.WithAnyTags( WithAnyTags )
			.WithoutTags( WithoutTags )
			.Run();

		if ( tr.Hit )
		{
			ILookTrace.Post(x => x.OnTraceHit(tr));
		}
	}
}
