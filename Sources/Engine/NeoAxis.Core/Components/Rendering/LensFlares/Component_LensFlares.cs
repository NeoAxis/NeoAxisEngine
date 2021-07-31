// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using NeoAxis.Editor;

namespace NeoAxis
{
	/// <summary>
	/// Lens flares effect of the light. The component must be a child of Light.
	/// </summary>
	[EditorNewObjectSettings( typeof( NewObjectSettingsLensFlares ) )]
	public class Component_LensFlares : Component//, IComponent_LightChild
	{
		/// <summary>
		/// The color multiplier of the flares.
		/// </summary>
		[DefaultValue( "1 1 1" )]
		public Reference<ColorValue> Color
		{
			get { if( _color.BeginGet() ) Color = _color.Get( this ); return _color.value; }
			set { if( _color.BeginSet( ref value ) ) { try { ColorChanged?.Invoke( this ); } finally { _color.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Color"/> property value changes.</summary>
		public event Action<Component_LensFlares> ColorChanged;
		ReferenceField<ColorValue> _color = ColorValue.One;

		//!!!!

		//[FieldSerialize( "fadeSpeed" )]
		//float fadeSpeed = 3;

		//[FieldSerialize( "visibilityCheckStartDistance" )]
		//float visibilityCheckStartDistance = 1;


		//!!!!
		//!!!!нужен некий CameraInstance. это не только для флар по идее
		//Dictionary<GuiRenderer, CameraInfo> cameraItems = new Dictionary<GuiRenderer, CameraInfo>();

		/////////////////////////////////////////

		/// <summary>
		/// A set of settings for <see cref="Component_LensFlares"/> creation in the editor.
		/// </summary>
		public class NewObjectSettingsLensFlares : NewObjectSettings
		{
			[DefaultValue( true )]
			[Category( "Options" )]
			public bool CreateDefaultFlares { get; set; } = true;

			public override bool Creation( NewObjectCell.ObjectCreationContext context )
			{
				var newObject2 = (Component_LensFlares)context.newObject;

				if( CreateDefaultFlares )
					newObject2.CreateDefaultFlares();

				return base.Creation( context );
			}
		}

		/////////////////////////////////////////

		public void CreateDefaultFlares()
		{
			Component_LensFlare flare;

			flare = CreateComponent<Component_LensFlare>();
			flare.Name = ComponentUtility.GetNewObjectUniqueName( flare );
			flare.Image = ReferenceUtility.MakeReference( @"Base\Images\Lens flares\hexangle.png" );
			flare.Color = new ColorValue( 1, 0.4980392f, 0.4980392f, 0.05882353f );
			flare.Size = new Vector2( 0.05f, 0.05f );
			flare.Position = -0.1f;

			flare = CreateComponent<Component_LensFlare>();
			flare.Name = ComponentUtility.GetNewObjectUniqueName( flare );
			flare.Image = ReferenceUtility.MakeReference( @"Base\Images\Lens flares\hexangle.png" );
			flare.Color = new ColorValue( 0.4980392f, 1, 0.4980392f, 0.05882353f );
			flare.Size = new Vector2( 0.05f, 0.05f );
			flare.Position = -2;

			flare = CreateComponent<Component_LensFlare>();
			flare.Name = ComponentUtility.GetNewObjectUniqueName( flare );
			flare.Image = ReferenceUtility.MakeReference( @"Base\Images\Lens flares\hexangle.png" );
			flare.Color = new ColorValue( 1, 1, 1, 0.05882353f );
			flare.Size = new Vector2( 0.08f, 0.08f );
			flare.Position = -1.5f;

			flare = CreateComponent<Component_LensFlare>();
			flare.Name = ComponentUtility.GetNewObjectUniqueName( flare );
			flare.Image = ReferenceUtility.MakeReference( @"Base\Images\Lens flares\hexangle.png" );
			flare.Color = new ColorValue( 1, 1, 1, 0.05882353f );
			flare.Size = new Vector2( 0.1f, 0.1f );
			flare.Position = -1.7f;

			flare = CreateComponent<Component_LensFlare>();
			flare.Name = ComponentUtility.GetNewObjectUniqueName( flare );
			flare.Image = ReferenceUtility.MakeReference( @"Base\Images\Lens flares\hexangle.png" );
			flare.Color = new ColorValue( 1, 0.4980392f, 0.4980392f, 0.05882353f );
			flare.Size = new Vector2( 0.06f, 0.06f );
			flare.Position = -3;

			flare = CreateComponent<Component_LensFlare>();
			flare.Name = ComponentUtility.GetNewObjectUniqueName( flare );
			flare.Image = ReferenceUtility.MakeReference( @"Base\Images\Lens flares\hexangle.png" );
			flare.Color = new ColorValue( 1, 1, 0.4980392f, 0.05882353f );
			flare.Size = new Vector2( 0.05f, 0.05f );
			flare.Position = -.5f;

			flare = CreateComponent<Component_LensFlare>();
			flare.Name = ComponentUtility.GetNewObjectUniqueName( flare );
			flare.Image = ReferenceUtility.MakeReference( @"Base\Images\Lens flares\hexangle.png" );
			flare.Color = new ColorValue( 1, 1, 1, 0.05882353f );
			flare.Size = new Vector2( 0.09f, 0.09f );
			flare.Position = -2.1f;

			flare = CreateComponent<Component_LensFlare>();
			flare.Name = ComponentUtility.GetNewObjectUniqueName( flare );
			flare.Image = ReferenceUtility.MakeReference( @"Base\Images\Lens flares\hexangle.png" );
			flare.Color = new ColorValue( 1, 1, 1, 0.05882353f );
			flare.Size = new Vector2( 0.08f, 0.08f );
			flare.Position = -.65f;

			flare = CreateComponent<Component_LensFlare>();
			flare.Name = ComponentUtility.GetNewObjectUniqueName( flare );
			flare.Image = ReferenceUtility.MakeReference( @"Base\Images\Lens flares\hexangle.png" );
			flare.Color = new ColorValue( 0.4980392f, 1, 0.4980392f, 0.05882353f );
			flare.Size = new Vector2( 0.05f, 0.05f );
			flare.Position = -.86f;

			flare = CreateComponent<Component_LensFlare>();
			flare.Name = ComponentUtility.GetNewObjectUniqueName( flare );
			flare.Image = ReferenceUtility.MakeReference( @"Base\Images\Lens flares\hexangle.png" );
			flare.Color = new ColorValue( 1, 1, 1, 0.05882353f );
			flare.Size = new Vector2( 0.12f, 0.12f );
			flare.Position = -5;

			flare = CreateComponent<Component_LensFlare>();
			flare.Name = ComponentUtility.GetNewObjectUniqueName( flare );
			flare.Image = ReferenceUtility.MakeReference( @"Base\Images\Lens flares\hexangle.png" );
			flare.Color = new ColorValue( 1, 1, 1, 0.05882353f );
			flare.Size = new Vector2( 0.04f, 0.04f );
			flare.Position = .3f;

			flare = CreateComponent<Component_LensFlare>();
			flare.Name = ComponentUtility.GetNewObjectUniqueName( flare );
			flare.Image = ReferenceUtility.MakeReference( @"Base\Images\Lens flares\hexangle.png" );
			flare.Color = new ColorValue( 0.4980392f, 1, 0.4980392f, 0.05882353f );
			flare.Size = new Vector2( 0.03f, 0.03f );
			flare.Position = .6f;

			flare = CreateComponent<Component_LensFlare>();
			flare.Name = ComponentUtility.GetNewObjectUniqueName( flare );
			flare.Image = ReferenceUtility.MakeReference( @"Base\Images\Lens flares\hexangle.png" );
			flare.Color = new ColorValue( 1, 1, 1, 0.05882353f );
			flare.Size = new Vector2( 0.06f, 0.06f );
			flare.Position = .1f;

			flare = CreateComponent<Component_LensFlare>();
			flare.Name = ComponentUtility.GetNewObjectUniqueName( flare );
			flare.Image = ReferenceUtility.MakeReference( @"Base\Images\Lens flares\hexangle.png" );
			flare.Color = new ColorValue( 1, 1, 1, 0.05882353f );
			flare.Size = new Vector2( 0.04f, 0.04f );
			flare.Position = -.35f;
		}

		public virtual void RenderUI( CanvasRenderer renderer, Component_RenderingPipeline.RenderSceneData.LightItem lightItem, Viewport viewport, double intensity )
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
				//get enabled flares
				var flares = GetComponents<Component_LensFlare>( onlyEnabledInHierarchy: true );

				//sort by position
				CollectionUtility.SelectionSort( flares, delegate ( Component_LensFlare f1, Component_LensFlare f2 )
				{
					if( f1.Position > f2.Position )
						return -1;
					if( f1.Position < f2.Position )
						return 1;
					return 0;
				} );

				//for( int n = 0; n < flares.Count; n++ )
				//	sortedFlares[ n ].tempSmoothIntensityFactor = cameraInfo.SmoothIntensityFactors[ n ];

				//render
				foreach( var flare in flares )
				{
					var image = flare.Image.Value;

					var flareVector = screenLightPosition - new Vector2( 0.5, 0.5 );
					var flarePosition = new Vector2( 0.5, 0.5 ) + flareVector * flare.Position;
					var size = flare.Size.Value;
					var flareSize = new Vector2( size.X, size.Y * renderer.AspectRatio );
					var rectangle = new Rectangle( flarePosition - flareSize * 0.5, flarePosition + flareSize * 0.5 );

					var flareColor = Color.Value * flare.Color * new ColorValue( 1, 1, 1, intensity );
					// * new ColorValue( 1, 1, 1, item.tempSmoothIntensityFactor );

					if( flareColor.Alpha > 0 )
					{
						renderer.PushBlendingType( flare.Blending.Value );
						renderer.AddQuad( rectangle, new RectangleF( 0, 0, 1, 1 ), image, flareColor, true );
						renderer.PopBlendingType();
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
