// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;

//!!!!Builder support

//!!!!collision

//!!!!smooth normals

//!!!!multiline

namespace NeoAxis
{
	/// <summary>
	/// A scene object displaying 3D text in the scene.
	/// </summary>
	[NewObjectDefaultName( "Text 3D" )]
	[AddToResourcesWindow( @"Base\Scene objects\Additional\Text 3D", 0 )]
	public class Text3D : MeshInSpace
	{
		GeneratedData generatedData;
		bool needUpdate;

		//Font.Contour[] _contours;

		//

		/// <summary>
		/// The text to display.
		/// </summary>
		[DefaultValue( "Text" )]
		[Category( "Text 3D" )]
		public Reference<string> Text
		{
			get { if( _text.BeginGet() ) Text = _text.Get( this ); return _text.value; }
			set { if( _text.BeginSet( this, ref value ) ) { try { TextChanged?.Invoke( this ); DataWasChanged(); } finally { _text.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Text"/> property value changes.</summary>
		public event Action<Text3D> TextChanged;
		ReferenceField<string> _text = "Text";

		//!!!!

		///// <summary>
		///// Whether to display a text in multiline mode. Use '\n' to specify returns in the <see cref="Text"/> property.
		///// </summary>
		//[DefaultValue( true )]
		//public Reference<bool> Multiline
		//{
		//	get { if( _multiline.BeginGet() ) Multiline = _multiline.Get( this ); return _multiline.value; }
		//	set { if( _multiline.BeginSet( this, ref value ) ) { try { MultilineChanged?.Invoke( this ); DataWasChanged(); } finally { _multiline.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="Multiline"/> property value changes.</summary>
		//public event Action<Text3D> MultilineChanged;
		//ReferenceField<bool> _multiline = true;

		///// <summary>
		///// The horizontal alignment of the multiline text.
		///// </summary>
		//[DefaultValue( EHorizontalAlignment.Center )]
		//public Reference<EHorizontalAlignment> TextHorizontalAlignment
		//{
		//	get { if( _textHorizontalAlignment.BeginGet() ) TextHorizontalAlignment = _textHorizontalAlignment.Get( this ); return _textHorizontalAlignment.value; }
		//	set { if( _textHorizontalAlignment.BeginSet( this, ref value ) ) { try { TextHorizontalAlignmentChanged?.Invoke( this ); DataWasChanged(); } finally { _textHorizontalAlignment.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="TextHorizontalAlignment"/> property value changes.</summary>
		//public event Action<Text3D> TextHorizontalAlignmentChanged;
		//ReferenceField<EHorizontalAlignment> _textHorizontalAlignment = EHorizontalAlignment.Center;

		///// <summary>
		///// Vertical space between lines in multiline mode.
		///// </summary>
		//[DefaultValue( "Units 0" )]
		//public Reference<UIMeasureValueDouble> VerticalIndention
		//{
		//	get { if( _verticalIndention.BeginGet() ) VerticalIndention = _verticalIndention.Get( this ); return _verticalIndention.value; }
		//	set { if( _verticalIndention.BeginSet( this, ref value ) ) { try { VerticalIndentionChanged?.Invoke( this ); DataWasChanged(); } finally { _verticalIndention.EndSet(); } } }
		//}
		//public event Action<Text3D> VerticalIndentionChanged;
		//ReferenceField<UIMeasureValueDouble> _verticalIndention = new UIMeasureValueDouble( UIMeasure.Units, 0 );

		/// <summary>
		/// The font of rendered text.
		/// </summary>
		[DefaultValue( null )]
		[Category( "Text 3D" )]
		public Reference<FontComponent> Font
		{
			get { if( _font.BeginGet() ) Font = _font.Get( this ); return _font.value; }
			set { if( _font.BeginSet( this, ref value ) ) { try { FontChanged?.Invoke( this ); DataWasChanged(); } finally { _font.EndSet(); } } }
		}
		public event Action<Text3D> FontChanged;
		ReferenceField<FontComponent> _font = null;

		/// <summary>
		/// The thickness of the generated mesh.
		/// </summary>
		//[Category( "Text" )]
		[DefaultValue( 0.1 )]
		[Range( 0, 1, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		[Category( "Text 3D" )]
		public Reference<double> Thickness
		{
			get { if( _thickness.BeginGet() ) Thickness = _thickness.Get( this ); return _thickness.value; }
			set { if( _thickness.BeginSet( this, ref value ) ) { try { ThicknessChanged?.Invoke( this ); DataWasChanged(); } finally { _thickness.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Thickness"/> property value changes.</summary>
		public event Action<Text3D> ThicknessChanged;
		ReferenceField<double> _thickness = 0.1;

		/// <summary>
		/// The level of tessellation of rounded silhouettes.
		/// </summary>
		[DefaultValue( 1 )]
		[Range( 0, 3 )]
		[Category( "Text 3D" )]
		public Reference<int> Tesselation
		{
			get { if( _tesselation.BeginGet() ) Tesselation = _tesselation.Get( this ); return _tesselation.value; }
			set { if( _tesselation.BeginSet( this, ref value ) ) { try { TesselationChanged?.Invoke( this ); DataWasChanged(); } finally { _tesselation.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Tesselation"/> property value changes.</summary>
		public event Action<Text3D> TesselationChanged;
		ReferenceField<int> _tesselation = 1;

		/// <summary>
		/// The horizontal alignment on the screen.
		/// </summary>
		[DefaultValue( EHorizontalAlignment.Center )]
		[Category( "Text 3D" )]
		public Reference<EHorizontalAlignment> HorizontalAlignment
		{
			get { if( _horizontalAlignment.BeginGet() ) HorizontalAlignment = _horizontalAlignment.Get( this ); return _horizontalAlignment.value; }
			set { if( _horizontalAlignment.BeginSet( this, ref value ) ) { try { HorizontalAlignmentChanged?.Invoke( this ); DataWasChanged(); } finally { _horizontalAlignment.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="HorizontalAlignment"/> property value changes.</summary>
		public event Action<Text3D> HorizontalAlignmentChanged;
		ReferenceField<EHorizontalAlignment> _horizontalAlignment = EHorizontalAlignment.Center;

		/// <summary>
		/// The vertical alignment on the screen.
		/// </summary>
		[DefaultValue( EVerticalAlignment.Center )]
		[Category( "Text 3D" )]
		public Reference<EVerticalAlignment> VerticalAlignment
		{
			get { if( _verticalAlignment.BeginGet() ) VerticalAlignment = _verticalAlignment.Get( this ); return _verticalAlignment.value; }
			set { if( _verticalAlignment.BeginSet( this, ref value ) ) { try { VerticalAlignmentChanged?.Invoke( this ); DataWasChanged(); } finally { _verticalAlignment.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="VerticalAlignment"/> property value changes.</summary>
		public event Action<Text3D> VerticalAlignmentChanged;
		ReferenceField<EVerticalAlignment> _verticalAlignment = EVerticalAlignment.Center;

		/////////////////////////////////////////

		class GeneratedData
		{
			public List<Mesh> meshesToDispose = new List<Mesh>();
		}

		/////////////////////////////////////////

		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			base.OnMetadataGetMembersFilter( context, member, ref skip );

			var p = member as Metadata.Property;
			if( p != null )
			{
				//switch( p.Name )
				//{

				//!!!!

				//case nameof( TextHorizontalAlignment ):
				//case nameof( VerticalIndention ):
				//	if( !Multiline )
				//		skip = true;
				//	break;
				//}
			}
		}

		protected override void OnGetRenderSceneData( ViewportRenderingContext context, GetRenderSceneDataMode mode, Scene.GetObjectsInSpaceItem modeGetObjectsItem )
		{
			base.OnGetRenderSceneData( context, mode, modeGetObjectsItem );

			//!!!!
			//if( _contours != null )
			//{
			//	var renderer = context.objectInSpaceRenderingContext.viewport.Simple3DRenderer;
			//	var tr = TransformV;

			//	foreach( var contour in _contours )
			//	{

			//		var points = new Vector3[ contour.Points.Count ];
			//		for( int n = 0; n < points.Length; n++ )
			//		{
			//			var point = contour.Points[ n ];
			//			points[ n ] = new Vector3( 0, point.X, point.Y );
			//		}

			//		for( int n = 0; n < points.Length; n++ )
			//		{
			//			var p = points[ n ];

			//			renderer.SetColor( new ColorValue( 1, 1, 1 ) );

			//			if( n == 0 )
			//				renderer.SetColor( new ColorValue( 1, 0, 0 ) );
			//			if( n == 1 )
			//				renderer.SetColor( new ColorValue( 0, 1, 0 ) );
			//			if( n == 2 )
			//				renderer.SetColor( new ColorValue( 0, 0, 1 ) );
			//			if( n == 3 )
			//				renderer.SetColor( new ColorValue( 1, 1, 0 ) );


			//			renderer.AddSphere( new Sphere( tr * p, 0.005 ), 8 );
			//		}

			//		//c.Points
			//	}
			//}
		}

		GeneratedData GetGeneratedData()
		{
			if( generatedData == null )
			{
				generatedData = new GeneratedData();

				var contourDatas = GetContours();
				if( contourDatas.Length != 0 )
				{
					var mesh = ComponentUtility.CreateComponent<Mesh>( null, true, false );
					//var mesh = CreateComponent<Mesh>( enabled: false );
					//mesh.Name = "Mesh";

					GenerateMeshData( mesh, null, contourDatas );
					mesh.Enabled = true;

					generatedData.meshesToDispose.Add( mesh );

					Mesh = mesh;

					SpaceBoundsUpdate();
				}

				//_contours = contours;
			}

			return generatedData;
		}

		void DeleteGeneratedData()
		{
			if( generatedData != null )
			{
				//_contours = null;

				Mesh = null;

				foreach( var mesh in generatedData.meshesToDispose )
					mesh.Dispose();

				generatedData = null;
			}
		}

		protected override void OnEnabledInHierarchyChanged()
		{
			base.OnEnabledInHierarchyChanged();

			var scene = ParentScene;
			if( scene != null )
			{
				if( EnabledInHierarchyAndIsInstance )
				{
					//scene.ViewportUpdateBefore += Scene_ViewportUpdateBefore;
					//TransformTool.AllInstances_ModifyCommit += TransformTool_AllInstances_ModifyCommit;
					//TransformTool.AllInstances_ModifyCancel += TransformTool_AllInstances_ModifyCancel;
					if( generatedData == null )
						Update();
				}
				else
				{
					//scene.ViewportUpdateBefore -= Scene_ViewportUpdateBefore;
					//TransformTool.AllInstances_ModifyCommit -= TransformTool_AllInstances_ModifyCommit;
					//TransformTool.AllInstances_ModifyCancel -= TransformTool_AllInstances_ModifyCancel;
					Update();
				}
			}
		}

		//private void TransformTool_AllInstances_ModifyCommit( TransformTool sender )
		//{
		//	if( needUpdateAfterEndModifyingTransformTool )
		//	{
		//		Update();
		//		needUpdateAfterEndModifyingTransformTool = false;
		//	}
		//}

		//private void TransformTool_AllInstances_ModifyCancel( TransformTool sender )
		//{
		//	if( needUpdateAfterEndModifyingTransformTool )
		//	{
		//		Update();
		//		needUpdateAfterEndModifyingTransformTool = false;
		//	}
		//}

		public void Update()
		{
			DeleteGeneratedData();

			if( EnabledInHierarchyAndIsInstance )
			{
				GetGeneratedData();
				SpaceBoundsUpdate();
			}

			needUpdate = false;
		}

		protected override void OnUpdate( float delta )
		{
			base.OnUpdate( delta );

			if( EnabledInHierarchyAndIsInstance && needUpdate )
				Update();
		}

		void DataWasChanged()
		{
			if( EnabledInHierarchyAndIsInstance )
				needUpdate = true;
		}

		FontComponent GetFont()
		{
			var font = Font.Value;

			if( font == null && RenderingSystem.ApplicationRenderTarget.Viewports.Count != 0 )
			{
				var mainViewport = RenderingSystem.ApplicationRenderTarget.Viewports[ 0 ];
				var renderer = mainViewport.CanvasRenderer;
				if( renderer != null )
					font = renderer.DefaultFont;
			}

			if( font == null || font.Disposed )
				return null;

			return font;
		}

		FontComponent.CharacterContourData[] GetContours()
		{
			var font = GetFont();
			if( font != null )
			{
				var result = new List<FontComponent.CharacterContourData>();
				foreach( var character in Text.Value )
				{
					var contourData = font.GetCharacterContourData( character, Tesselation );
					if( contourData != null )
						result.Add( contourData );
				}
				return result.ToArray();
			}
			else
				return new FontComponent.CharacterContourData[ 0 ];
		}

		Rectangle GetContoursBounds( FontComponent.CharacterContourData[] contourDatas )
		{
			var currentAdvance = 0.0;

			var b = Rectangle.Cleared;
			foreach( var data in contourDatas )
			{
				foreach( var c in data.Add )
				{
					foreach( var p in c.Points )
						b.Add( p + new Vector2( currentAdvance, 0 ) );
				}

				currentAdvance += data.Advance;
			}
			return b;
		}

		void GenerateMeshData( Mesh mesh, CollisionShape_Mesh collisionShape, FontComponent.CharacterContourData[] contourDatas )
		{
			//alignment
			var offset = Vector2.Zero;
			{
				var b = GetContoursBounds( contourDatas );

				if( HorizontalAlignment.Value == EHorizontalAlignment.Center )
					offset.X = -b.GetCenter().X;
				else if( HorizontalAlignment.Value == EHorizontalAlignment.Right )
					offset.X = -b.Right;
				else
					offset.X = -b.Left;

				if( VerticalAlignment.Value == EVerticalAlignment.Center )
					offset.Y = -b.GetCenter().Y;
				else if( VerticalAlignment.Value == EVerticalAlignment.Bottom )
					offset.Y = -b.Top;
				else
					offset.Y = -b.Bottom;
			}

			var resultPositions = new List<Vector3F>( 512 );
			var resultNormals = new List<Vector3F>( 512 );
			var resultTangents = new List<Vector4F>( 512 );
			var resultIndices = new List<int>( 1024 );

			var thickness = (float)Thickness.Value;

			var currentAdvance = 0.0;

			foreach( var contourData in contourDatas )
			{
				contourData.GetShape( out var positions, out var normals, out var tangents, out var indices );

				//add character data to result data
				{
					var startIndex = resultPositions.Count;

					for( int n = 0; n < positions.Count; n++ )
					{
						var pos = positions[ n ];
						pos.X *= thickness;
						pos.Y += (float)offset.X;
						pos.Z += (float)offset.Y;

						resultPositions.Add( pos + new Vector3F( 0, (float)currentAdvance, 0 ) );
						resultNormals.Add( normals[ n ] );
						resultTangents.Add( tangents[ n ] );
					}

					foreach( var index in indices )
						resultIndices.Add( startIndex + index );
				}

				currentAdvance += contourData.Advance;
			}

			//if( currentIndex != indexCount )
			//	Log.Fatal( "Text3D: GenerateMeshData: currentIndex != indexCount." );
			foreach( var index in resultIndices )
				if( index < 0 || index >= resultPositions.Count )
					Log.Fatal( "Text3D: GenerateMeshData: index < 0 || index >= positions.Count." );

			//init objects
			if( collisionShape != null )
			{
				collisionShape.Vertices = resultPositions.ToArray();
				collisionShape.Indices = resultIndices.ToArray();
			}
			else
			{
				var vertexStructure = StandardVertex.MakeStructure( StandardVertex.Components.StaticOneTexCoord, true, out int vertexSize );

				var vertices = new byte[ vertexSize * resultPositions.Count ];
				unsafe
				{
					fixed( byte* pVertices = vertices )
					{
						StandardVertex.StaticOneTexCoord* pVertex = (StandardVertex.StaticOneTexCoord*)pVertices;

						for( int n = 0; n < resultPositions.Count; n++ )
						{
							pVertex->Position = resultPositions[ n ];
							pVertex->Normal = resultNormals[ n ];
							pVertex->Tangent = resultTangents[ n ];
							pVertex->Color = new ColorValue( 1, 1, 1, 1 );
							pVertex->TexCoord0 = new Vector2F( 0, 0 );
							//pVertex->TexCoord0 = resultTexCoords[ n ];

							pVertex++;
						}
					}
				}

				//create mesh geometry
				if( vertexStructure.Length != 0 && vertices.Length != 0 && resultIndices.Count != 0 )
				{
					var meshGeometry = mesh.CreateComponent<MeshGeometry>();
					meshGeometry.Name = "Mesh Geometry";
					meshGeometry.VertexStructure = vertexStructure;
					meshGeometry.Vertices = vertices;
					meshGeometry.Indices = resultIndices.ToArray();
				}
			}
		}

		public override void NewObjectSetDefaultConfiguration( bool createdFromNewObjectWindow = false )
		{
			base.NewObjectSetDefaultConfiguration( createdFromNewObjectWindow );

			ReplaceMaterial = ReferenceUtility.MakeReference( @"Base\Materials\White.material" );
		}
	}
}
