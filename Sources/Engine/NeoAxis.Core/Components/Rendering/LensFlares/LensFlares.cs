// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using NeoAxis.Editor;

namespace NeoAxis
{
	/// <summary>
	/// Lens flares effect of the light. The component must be a child of Light.
	/// </summary>
#if !DEPLOY
	[NewObjectSettings( "NeoAxis.Editor.LensFlaresNewObjectSettings" )]
#endif
	public class LensFlares : Component//, ILightChild
	{
		/// <summary>
		/// The color multiplier of the flares.
		/// </summary>
		[DefaultValue( "1 1 1" )]
		public Reference<ColorValue> Color
		{
			get { if( _color.BeginGet() ) Color = _color.Get( this ); return _color.value; }
			set { if( _color.BeginSet( this, ref value ) ) { try { ColorChanged?.Invoke( this ); } finally { _color.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Color"/> property value changes.</summary>
		public event Action<LensFlares> ColorChanged;
		ReferenceField<ColorValue> _color = ColorValue.One;

		/////////////////////////////////////////

		public void CreateDefaultFlares()
		{
			LensFlare flare;

			flare = CreateComponent<LensFlare>();
			flare.Name = ComponentUtility.GetNewObjectUniqueName( flare );
			flare.Image = ReferenceUtility.MakeReference( @"Base\Images\Lens flares\hexangle.png" );
			flare.Color = new ColorValue( 1, 0.4980392f, 0.4980392f, 0.05882353f );
			flare.Size = new Vector2( 0.05f, 0.05f );
			flare.Position = -0.1f;

			flare = CreateComponent<LensFlare>();
			flare.Name = ComponentUtility.GetNewObjectUniqueName( flare );
			flare.Image = ReferenceUtility.MakeReference( @"Base\Images\Lens flares\hexangle.png" );
			flare.Color = new ColorValue( 0.4980392f, 1, 0.4980392f, 0.05882353f );
			flare.Size = new Vector2( 0.05f, 0.05f );
			flare.Position = -2;

			flare = CreateComponent<LensFlare>();
			flare.Name = ComponentUtility.GetNewObjectUniqueName( flare );
			flare.Image = ReferenceUtility.MakeReference( @"Base\Images\Lens flares\hexangle.png" );
			flare.Color = new ColorValue( 1, 1, 1, 0.05882353f );
			flare.Size = new Vector2( 0.08f, 0.08f );
			flare.Position = -1.5f;

			flare = CreateComponent<LensFlare>();
			flare.Name = ComponentUtility.GetNewObjectUniqueName( flare );
			flare.Image = ReferenceUtility.MakeReference( @"Base\Images\Lens flares\hexangle.png" );
			flare.Color = new ColorValue( 1, 1, 1, 0.05882353f );
			flare.Size = new Vector2( 0.1f, 0.1f );
			flare.Position = -1.7f;

			flare = CreateComponent<LensFlare>();
			flare.Name = ComponentUtility.GetNewObjectUniqueName( flare );
			flare.Image = ReferenceUtility.MakeReference( @"Base\Images\Lens flares\hexangle.png" );
			flare.Color = new ColorValue( 1, 0.4980392f, 0.4980392f, 0.05882353f );
			flare.Size = new Vector2( 0.06f, 0.06f );
			flare.Position = -3;

			flare = CreateComponent<LensFlare>();
			flare.Name = ComponentUtility.GetNewObjectUniqueName( flare );
			flare.Image = ReferenceUtility.MakeReference( @"Base\Images\Lens flares\hexangle.png" );
			flare.Color = new ColorValue( 1, 1, 0.4980392f, 0.05882353f );
			flare.Size = new Vector2( 0.05f, 0.05f );
			flare.Position = -.5f;

			flare = CreateComponent<LensFlare>();
			flare.Name = ComponentUtility.GetNewObjectUniqueName( flare );
			flare.Image = ReferenceUtility.MakeReference( @"Base\Images\Lens flares\hexangle.png" );
			flare.Color = new ColorValue( 1, 1, 1, 0.05882353f );
			flare.Size = new Vector2( 0.09f, 0.09f );
			flare.Position = -2.1f;

			flare = CreateComponent<LensFlare>();
			flare.Name = ComponentUtility.GetNewObjectUniqueName( flare );
			flare.Image = ReferenceUtility.MakeReference( @"Base\Images\Lens flares\hexangle.png" );
			flare.Color = new ColorValue( 1, 1, 1, 0.05882353f );
			flare.Size = new Vector2( 0.08f, 0.08f );
			flare.Position = -.65f;

			flare = CreateComponent<LensFlare>();
			flare.Name = ComponentUtility.GetNewObjectUniqueName( flare );
			flare.Image = ReferenceUtility.MakeReference( @"Base\Images\Lens flares\hexangle.png" );
			flare.Color = new ColorValue( 0.4980392f, 1, 0.4980392f, 0.05882353f );
			flare.Size = new Vector2( 0.05f, 0.05f );
			flare.Position = -.86f;

			flare = CreateComponent<LensFlare>();
			flare.Name = ComponentUtility.GetNewObjectUniqueName( flare );
			flare.Image = ReferenceUtility.MakeReference( @"Base\Images\Lens flares\hexangle.png" );
			flare.Color = new ColorValue( 1, 1, 1, 0.05882353f );
			flare.Size = new Vector2( 0.12f, 0.12f );
			flare.Position = -5;

			flare = CreateComponent<LensFlare>();
			flare.Name = ComponentUtility.GetNewObjectUniqueName( flare );
			flare.Image = ReferenceUtility.MakeReference( @"Base\Images\Lens flares\hexangle.png" );
			flare.Color = new ColorValue( 1, 1, 1, 0.05882353f );
			flare.Size = new Vector2( 0.04f, 0.04f );
			flare.Position = .3f;

			flare = CreateComponent<LensFlare>();
			flare.Name = ComponentUtility.GetNewObjectUniqueName( flare );
			flare.Image = ReferenceUtility.MakeReference( @"Base\Images\Lens flares\hexangle.png" );
			flare.Color = new ColorValue( 0.4980392f, 1, 0.4980392f, 0.05882353f );
			flare.Size = new Vector2( 0.03f, 0.03f );
			flare.Position = .6f;

			flare = CreateComponent<LensFlare>();
			flare.Name = ComponentUtility.GetNewObjectUniqueName( flare );
			flare.Image = ReferenceUtility.MakeReference( @"Base\Images\Lens flares\hexangle.png" );
			flare.Color = new ColorValue( 1, 1, 1, 0.05882353f );
			flare.Size = new Vector2( 0.06f, 0.06f );
			flare.Position = .1f;

			flare = CreateComponent<LensFlare>();
			flare.Name = ComponentUtility.GetNewObjectUniqueName( flare );
			flare.Image = ReferenceUtility.MakeReference( @"Base\Images\Lens flares\hexangle.png" );
			flare.Color = new ColorValue( 1, 1, 1, 0.05882353f );
			flare.Size = new Vector2( 0.04f, 0.04f );
			flare.Position = -.35f;

			flare = CreateComponent<LensFlare>();
			flare.Name = ComponentUtility.GetNewObjectUniqueName( flare );
			flare.Image = ReferenceUtility.MakeReference( @"Base\Images\Lens flares\sparkle_blurred.png" );
			flare.Color = new ColorValue( 0.9570981, 1, 0.49, 1.3 );
			flare.Size = new Vector2( 3, 3 );
			flare.SizeFadeByDistance = true;
			flare.Position = 1;
		}

		public virtual void RenderUI( ViewportRenderingContext context, CanvasRenderer renderer, RenderingPipeline.RenderSceneData.LightItem lightItem, Viewport viewport, double intensity, bool occlusionDepthCheck )
		{
			var lightPosition = lightItem.Position;

			//!!!!override real position
			//{
			//	if( Sun.Instances.Count > 0 )
			//	{
			//		//get first sun entity on the map.
			//		Sun sun = Sun.Instances[ 0 ];

			//		Vec3 direction;
			//		if( sun.BillboardOverridePosition != Vec3.Zero )
			//			direction = sun.BillboardOverridePosition.GetNormalize();
			//		else
			//			direction = -sun.Rotation.GetForward();
			//		position = camera.Position + direction * 100000;

			//		return true;
			//	}

			//	position = Vec3.Zero;
			//	return false;
			//}

			if( viewport.CameraSettings.ProjectToScreenCoordinates( lightPosition, out var screenLightPosition ) )
			{
				//!!!!GC
				//get enabled flares
				var flares = GetComponents<LensFlare>( onlyEnabledInHierarchy: true );

				//sort by position
				CollectionUtility.SelectionSort( flares, delegate ( LensFlare f1, LensFlare f2 )
				{
					if( f1.Position > f2.Position )
						return -1;
					if( f1.Position < f2.Position )
						return 1;
					return 0;
				} );

				//for( int n = 0; n < flares.Count; n++ )
				//	sortedFlares[ n ].tempSmoothIntensityFactor = cameraInfo.SmoothIntensityFactors[ n ];

				var cameraPosition = viewport.CameraSettings.Position;
				var pipeline = context.RenderingPipeline;
				var minimumVisibleSizeOfObjects = pipeline.MinimumVisibleSizeOfObjects.Value;

				//render
				foreach( var flare in flares )
				{
					var image = flare.Image.Value;

					var flareVector = screenLightPosition - new Vector2( 0.5, 0.5 );
					var flarePosition = new Vector2( 0.5, 0.5 ) + flareVector * flare.Position;

					var size = flare.Size.Value;
					if( flare.SizeFadeByDistance )
					{
						Vector3.Lerp( ref cameraPosition, ref lightPosition, flare.Position, out var flarePosition3D );

						double distance = ( flarePosition3D - viewport.CameraSettings.Position ).Length();
						if( distance != 0 )
							size *= 1.0 / distance;
					}

					var flareSize = new Vector2( size.X * renderer.AspectRatioInv, size.Y );
					//var flareSize = new Vector2( size.X, size.Y * renderer.AspectRatio );

					//cull by size
					var flareSizeInPixels = flareSize * viewport.SizeInPixels.ToVector2();
					if( flareSizeInPixels.MaxComponent() >= minimumVisibleSizeOfObjects )
					{
						var rectangle = new Rectangle( flarePosition - flareSize * 0.5, flarePosition + flareSize * 0.5 );

						var flareColor = Color.Value * flare.Color * new ColorValue( 1, 1, 1, intensity );
						// * new ColorValue( 1, 1, 1, item.tempSmoothIntensityFactor );

						if( flareColor.Alpha > 0 )
						{
							if( occlusionDepthCheck )
							{
								//!!!!
								var screenSize = flareSize.Y / 20;

								var compareDepth = ( cameraPosition - lightPosition ).Length() - flare.DepthCheckOffset.Value;
								renderer.PushOcclusionDepthCheck( screenLightPosition.ToVector2F(), (float)screenSize, (float)compareDepth );
							}

							renderer.PushBlendingType( flare.Blending.Value );
							renderer.AddQuad( rectangle, new RectangleF( 0, 0, 1, 1 ), image, flareColor, true );
							renderer.PopBlendingType();

							if( occlusionDepthCheck )
								renderer.PopOcclusionDepthCheck();
						}
					}
				}
			}
		}

		public override void NewObjectSetDefaultConfiguration( bool createdFromNewObjectWindow = false )
		{
			base.NewObjectSetDefaultConfiguration( createdFromNewObjectWindow );

			if( !createdFromNewObjectWindow )
				CreateDefaultFlares();
		}
	}
}
