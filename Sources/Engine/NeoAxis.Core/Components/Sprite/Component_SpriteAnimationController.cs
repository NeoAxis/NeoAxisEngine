// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using NeoAxis.Editor;

namespace NeoAxis
{
	/// <summary>
	/// An animation controller for the sprite.
	/// </summary>
	[AddToResourcesWindow( @"Base\2D\Sprite Animation Controller", -7999 )]
	public class Component_SpriteAnimationController : Component
	{
		double currentAnimationTime;
		double currentEngineTime;

		/////////////////////////////////////////

		[Browsable( false )]
		public Component_Sprite ParentSprite
		{
			get { return Parent as Component_Sprite; }
		}

		/// <summary>
		/// The animation used by the controller.
		/// </summary>
		public Reference<Component_Animation> PlayAnimation
		{
			get { if( _playAnimation.BeginGet() ) PlayAnimation = _playAnimation.Get( this ); return _playAnimation.value; }
			set { if( _playAnimation.BeginSet( ref value ) ) { try { PlayAnimationChanged?.Invoke( this ); ResetTime(); } finally { _playAnimation.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="PlayAnimation"/> property value changes.</summary>
		public event Action<Component_SpriteAnimationController> PlayAnimationChanged;
		ReferenceField<Component_Animation> _playAnimation;

		/// <summary>
		/// Animation speed multiplier.
		/// </summary>
		[Range( 0.0, 10, RangeAttribute.ConvenientDistributionEnum.Exponential, 3 )]
		[DefaultValue( 1.0 )]
		public Reference<double> Speed
		{
			get { if( _speed.BeginGet() ) Speed = _speed.Get( this ); return _speed.value; }
			set { if( _speed.BeginSet( ref value ) ) { try { SpeedChanged?.Invoke( this ); } finally { _speed.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Speed"/> property value changes.</summary>
		public event Action<Component_SpriteAnimationController> SpeedChanged;
		ReferenceField<double> _speed = 1.0;

		/// <summary>
		/// Whether to rewind to the start when playing ended.
		/// </summary>
		[DefaultValue( true )]
		public Reference<bool> AutoRewind
		{
			get { if( _autoRewind.BeginGet() ) AutoRewind = _autoRewind.Get( this ); return _autoRewind.value; }
			set { if( _autoRewind.BeginSet( ref value ) ) { try { AutoRewindChanged?.Invoke( this ); } finally { _autoRewind.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="AutoRewind"/> property value changes.</summary>
		public event Action<Component_SpriteAnimationController> AutoRewindChanged;
		ReferenceField<bool> _autoRewind = true;

		/////////////////////////////////////////

		protected override void OnEnabledInHierarchyChanged()
		{
			base.OnEnabledInHierarchyChanged();

			if( ParentSprite != null )
			{
				if( EnabledInHierarchy )
				{
					ParentSprite.MeshOutputOverride += ParentSprite_MeshOutputOverride;
					//ParentSprite.GetRenderSceneDataBefore += ParentSprite_GetRenderSceneDataBefore;
					ParentSprite.GetRenderSceneDataAddToFrameData += ParentSprite_GetRenderSceneDataAddToFrameData;
				}
				else
				{
					ParentSprite.MeshOutputOverride -= ParentSprite_MeshOutputOverride;
					//ParentSprite.GetRenderSceneDataBefore -= ParentSprite_GetRenderSceneDataBefore;
					ParentSprite.GetRenderSceneDataAddToFrameData -= ParentSprite_GetRenderSceneDataAddToFrameData;
				}
			}
		}

		private void ParentSprite_MeshOutputOverride( Component_MeshInSpace sender, ref Component_Mesh result )
		{
			var animation = PlayAnimation.Value as Component_SpriteAnimation;
			if( animation != null )
			{
				UpdateAnimationTime();

				var frame = animation.GetFrameByTime( currentAnimationTime );
				if( frame != null )
					result = SpriteMeshManager.GetMesh( frame.UV.Value.ToRectangleF() );
			}
		}

		//void ParentSprite_GetRenderSceneDataBefore( Component_ObjectInSpace sender, ViewportRenderingContext context, GetRenderSceneDataMode mode )
		//{
		//}

		private void ParentSprite_GetRenderSceneDataAddToFrameData( Component_MeshInSpace sender, ViewportRenderingContext context, GetRenderSceneDataMode mode, ref Component_RenderingPipeline.RenderSceneData.MeshItem item )
		{
			Component_Material material = null;

			var animation = PlayAnimation.Value as Component_SpriteAnimation;
			if( animation != null )
			{
				UpdateAnimationTime();

				material = animation.Material;

				var frame = animation.GetFrameByTime( currentAnimationTime );
				if( frame != null )
				{
					var m = frame.Material.Value;
					if( m != null )
						material = m;
				}
			}

			item.ReplaceMaterial = material;
			item.ReplaceMaterialSelectively = null;
		}

		void ResetTime()
		{
			currentEngineTime = EngineApp.EngineTime;
			currentAnimationTime = 0;
		}

		void UpdateAnimationTime()
		{
			double t = EngineApp.EngineTime;
			if( currentEngineTime != t )
			{
				double increment = t - currentEngineTime;
				currentEngineTime = t;
				currentAnimationTime += increment * Speed;

				var animation = PlayAnimation.Value as Component_Animation;
				if( animation != null )
				{
					double animationStartTime = 0;
					double animationLength = animation.Length;
					if( animationLength > 0 )
					{
						if( AutoRewind )
						{
							while( currentAnimationTime > animationStartTime + animationLength )
								currentAnimationTime -= animationLength;
							while( currentAnimationTime < animationStartTime )
								currentAnimationTime += animationLength;
						}
						else
							MathEx.Clamp( ref currentAnimationTime, animationStartTime, animationStartTime + animationLength );
					}
					else
						currentAnimationTime = 0;
				}
				else
					currentAnimationTime = 0;
			}
		}

		[Browsable( false )]
		public double CurrentAnimationTime
		{
			get { return currentAnimationTime; }
		}
	}
}
