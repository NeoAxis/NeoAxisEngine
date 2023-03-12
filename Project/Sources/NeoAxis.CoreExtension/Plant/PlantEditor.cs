// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#if !DEPLOY
using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace NeoAxis.Editor
{
	public class PlantEditor : CanvasBasedEditor
	{
		//bool firstCameraUpdate = true;
		//bool needResultCompile;

		//

		public PlantType PlantType
		{
			get { return (PlantType)ObjectOfEditor; }
		}

		protected override void OnCreate()
		{
			base.OnCreate();

			var scene = CreateScene( false );
			//if( PlantType != null )
			//{
			//	PlantTypeInSpace objInSpace = scene.CreateComponent<PlantTypeInSpace>();
			//	objInSpace.Mesh = PlantType;
			//}
			scene.Enabled = true;

			//if( Document != null )
			//	Document.UndoSystem.ListOfActionsChanged += UndoSystem_ListOfActionsChanged;

			if( ObjectOfEditor != null )
				SelectObjects( new object[] { ObjectOfEditor } );
		}

		protected override void OnDestroy()
		{
			//if( Document != null )
			//	Document.UndoSystem.ListOfActionsChanged -= UndoSystem_ListOfActionsChanged;

			base.OnDestroy();
		}

		protected override void OnViewportCreated()
		{
			base.OnViewportCreated();

			ViewportControl.Viewport.AllowRenderScreenLabels = false;
		}

		protected override void OnSceneViewportUpdateGetCameraSettings( ref bool processed )
		{
			base.OnSceneViewportUpdateGetCameraSettings( ref processed );

			//if( firstCameraUpdate && Scene.CameraEditor.Value != null )
			//{
			//	InitCamera();
			//	Viewport.CameraSettings = new Viewport.CameraSettingsClass( Viewport, Scene.CameraEditor );
			//}

			//if( Scene.CameraEditor.Value != null )
			//	PlantType.EditorCameraTransform = Scene.CameraEditor.Value.Transform;

			//firstCameraUpdate = false;
		}

		PlantMaterial GetSelectedMaterial()
		{
			if( SelectedObjects.Length == 1 )
				return SelectedObjects[ 0 ] as PlantMaterial;
			return null;
		}

		protected override void OnViewportUpdateBeforeOutput2()
		{
			base.OnViewportUpdateBeforeOutput2();

			if( PlantType != null )
			{
				var material = GetSelectedMaterial();
				if( material != null )
					DrawPlantMaterial( material );
			}
		}

		//static string Translate( string text )
		//{
		//	return EditorLocalization.Translate( "PlantEditor", text );
		//}

		protected override void OnGetTextInfoLeftTopCorner( List<string> lines )
		{
			base.OnGetTextInfoLeftTopCorner( lines );

			if( PlantType != null )
			{
				if( !string.IsNullOrEmpty( PlantType.Name ) )
					lines.Add( PlantType.Name );
				else
					lines.Add( "Plant generation tool" );

				var fileNames = new List<string>();
				PlantType.ExportToMeshes( "", true, fileNames, out _ );

				lines.Add( "" );
				lines.Add( string.Format( "{0} output meshes", fileNames.Count ) );
				foreach( var fileName in fileNames )
					lines.Add( fileName );
			}
		}

		//private void UndoSystem_ListOfActionsChanged( object sender, EventArgs e )
		//{
		//	needResultCompile = true;
		//}

		//ImageComponent GetBaseColorTexture( Material material )
		//{
		//	material.BaseColor.GetMember( material, out var outObject, out _ );

		//	var sample = outObject as ShaderTextureSample;
		//	if( sample != null )
		//		return sample.Texture;

		//	return null;
		//}

		ImageComponent GetOpacityTexture( Material material )
		{
			material.Opacity.GetMember( material, out var outObject, out _ );

			var sample = outObject as ShaderTextureSample;
			if( sample != null )
				return sample.Texture;

			return null;
		}

		void DrawPlantMaterial( PlantMaterial material )
		{
			Vector2F screenMultiplier = new Vector2F( Viewport.CanvasRenderer.AspectRatioInv * 0.95f, 0.95f );

			Vector2F Convert( Vector2 v )
			{
				var z = v - new Vector2( 0.5, 0.5 );
				z *= screenMultiplier;
				z += new Vector2( 0.5, 0.5 );
				return z.ToVector2F();
			};

			RectangleF ConvertRectangle( Rectangle v )
			{
				return new RectangleF( Convert( v.LeftTop ), Convert( v.RightBottom ) );
			};

			var renderer = Viewport.CanvasRenderer;


			ImageComponent texture = null;
			{
				var m = material.Material.Value;
				if( m != null )
				{
					//texture = GetBaseColorTexture( m );
					//if( texture == null )
					texture = GetOpacityTexture( m );
				}
			}

			//!!!!это надо плейн в 3D рисовать

			renderer.AddQuad( new RectangleF( Convert( Vector2F.Zero ), Convert( Vector2F.One ) ), new RectangleF( 0, 0, 1, 1 ), texture, new ColorValue( 1, 1, 1, 0.5 ) );
			//renderer.AddRectangle( new RectangleF( Convert( Vector2F.Zero ), Convert( Vector2F.One ) ), new ColorValue( 1, 1, 1, 0.3 ) );

			var partType = material.PartType.Value;

			if( partType == PlantMaterial.PartTypeEnum.Bark )
			{
				switch( material.UVMode.Value )
				{
				case PlantMaterial.UVModeEnum.All:
					{
						var color = new ColorValue( 0, 1, 0 );

						var p0 = new Vector2( 0, 0 );
						var p1 = new Vector2( 1, 0 );
						var p2 = new Vector2( 1, 1 );
						var p3 = new Vector2( 0, 1 );

						renderer.AddLine( Convert( p0 ), Convert( p1 ), color );
						renderer.AddLine( Convert( p1 ), Convert( p2 ), color );
						renderer.AddLine( Convert( p2 ), Convert( p3 ), color );
						renderer.AddLine( Convert( p3 ), Convert( p0 ), color );
					}
					break;

				case PlantMaterial.UVModeEnum.Point:
					{
						var pos = material.UVFrontPosition.Value;

						var color = new ColorValue( 0, 1, 0 );

						var r = new Rectangle( pos );
						r.Expand( 0.005 );

						renderer.AddQuad( ConvertRectangle( r ), color );
					}
					break;
				}
			}

			if( partType == PlantMaterial.PartTypeEnum.BranchWithLeaves || partType == PlantMaterial.PartTypeEnum.Leaf )
			{
				{
					var pos = material.UVFrontPosition.Value;
					var dirAngle = material.UVFrontDirection.Value.InRadians();
					var rot = Quaternion.FromRotateByZ( -dirAngle );

					var dirForward = ( rot * new Vector3( 1, 0, 0 ) ).ToVector2();
					var dirRight = ( rot * new Vector3( 0, 1, 0 ) ).ToVector2();

					var color = new ColorValue( 0, 1, 0 );

					renderer.AddLine( Convert( pos - dirForward * 0.01 ), Convert( pos + dirForward * 0.02 ), color );
					renderer.AddLine( Convert( pos - dirRight * 0.01 ), Convert( pos + dirRight * 0.01 ), color );

					var lengthRange = material.UVLengthRange.Value;
					var width = material.UVWidth.Value;

					var p0 = pos + ( rot * new Vector3( lengthRange.Maximum, -width / 2, 0 ) ).ToVector2();
					var p1 = pos + ( rot * new Vector3( lengthRange.Maximum, width / 2, 0 ) ).ToVector2();
					var p2 = pos + ( rot * new Vector3( lengthRange.Minimum, width / 2, 0 ) ).ToVector2();
					var p3 = pos + ( rot * new Vector3( lengthRange.Minimum, -width / 2, 0 ) ).ToVector2();

					renderer.AddLine( Convert( p0 ), Convert( p1 ), color );
					renderer.AddLine( Convert( p1 ), Convert( p2 ), color );
					renderer.AddLine( Convert( p2 ), Convert( p3 ), color );
					renderer.AddLine( Convert( p3 ), Convert( p0 ), color );
				}

				if( material.UVBack )
				{
					var pos = material.UVBackPosition.Value;
					var dirAngle = material.UVBackDirection.Value.InRadians();
					var rot = Quaternion.FromRotateByZ( -dirAngle );

					var dirForward = ( rot * new Vector3( 1, 0, 0 ) ).ToVector2();
					var dirRight = ( rot * new Vector3( 0, 1, 0 ) ).ToVector2();

					var color = new ColorValue( 0, 0, 1 );

					renderer.AddLine( Convert( pos - dirForward * 0.01 ), Convert( pos + dirForward * 0.02 ), color );
					renderer.AddLine( Convert( pos - dirRight * 0.01 ), Convert( pos + dirRight * 0.01 ), color );

					var lengthRange = material.UVLengthRange.Value;
					var width = material.UVWidth.Value;

					var p0 = pos + ( rot * new Vector3( lengthRange.Maximum, -width / 2, 0 ) ).ToVector2();
					var p1 = pos + ( rot * new Vector3( lengthRange.Maximum, width / 2, 0 ) ).ToVector2();
					var p2 = pos + ( rot * new Vector3( lengthRange.Minimum, width / 2, 0 ) ).ToVector2();
					var p3 = pos + ( rot * new Vector3( lengthRange.Minimum, -width / 2, 0 ) ).ToVector2();

					renderer.AddLine( Convert( p0 ), Convert( p1 ), color );
					renderer.AddLine( Convert( p1 ), Convert( p2 ), color );
					renderer.AddLine( Convert( p2 ), Convert( p3 ), color );
					renderer.AddLine( Convert( p3 ), Convert( p0 ), color );
				}
			}

			if( partType == PlantMaterial.PartTypeEnum.Flower )
			{
				{
					var pos = material.UVFrontPosition.Value;
					var radius = material.UVRadius.Value;

					var color = new ColorValue( 0, 1, 0 );

					var rect = new Rectangle( pos );
					rect.Expand( radius );

					renderer.AddEllipse( ConvertRectangle( rect ), 128, color );
				}

				//!!!!UVBack impl
				if( material.UVBack )
				{
					var pos = material.UVBackPosition.Value;
					var radius = material.UVRadius.Value;

					var color = new ColorValue( 0, 0, 1 );

					var rect = new Rectangle( pos );
					rect.Expand( radius );

					renderer.AddEllipse( ConvertRectangle( rect ), 128, color );
				}
			}

		}
	}
}
#endif