// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using NeoAxis.Editor;
using System.IO;


//!!!!child of scene for shared data
//public class CharacterMakerManager : Component


namespace NeoAxis
{
	/// <summary>
	/// A component to generate character meshes.
	/// </summary>
	[AddToResourcesWindow( @"Base\3D\Character Maker", -8989 )]
	[ResourceFileExtension( "charactermaker" )]
#if !DEPLOY
	[EditorControl( typeof( CharacterMakerEditor ) )]
	//[Preview( typeof( CharacterMakerPreview ) )]
	//[PreviewImage( typeof( CharacterMakerPreviewImage ) )]
	[SettingsCell( typeof( CharacterMakerSettingsCell ) )]
#endif
	public class CharacterMaker : Component
	{
		bool needUpdateCreatedMesh;
		Mesh createdMesh;
		//bool createdApplyToParent;

		/// <summary>
		/// The height of the character.
		/// </summary>
		//[Category( "Basic" )]
		[DefaultValue( 1.8 )]
		public Reference<double> Height
		{
			get { if( _height.BeginGet() ) Height = _height.Get( this ); return _height.value; }
			set { if( _height.BeginSet( this, ref value ) ) { try { HeightChanged?.Invoke( this ); NeedUpdateCreatedMesh(); } finally { _height.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Height"/> property value changes.</summary>
		public event Action<CharacterMaker> HeightChanged;
		ReferenceField<double> _height = 1.8;

		//!!!!
		/// <summary>
		/// The color multiplier.
		/// </summary>
		[DefaultValue( "0.1 0.7 0.1" )]
		public Reference<ColorValue> Color
		{
			get { if( _color.BeginGet() ) Color = _color.Get( this ); return _color.value; }
			set { if( _color.BeginSet( this, ref value ) ) { try { ColorChanged?.Invoke( this ); NeedUpdateCreatedMesh(); } finally { _color.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Color"/> property value changes.</summary>
		public event Action<CharacterMaker> ColorChanged;
		ReferenceField<ColorValue> _color = new ColorValue( 0.1, 0.7, 0.1 );

		/// <summary>
		/// Whether to generate mesh internally inside the component. Use CreatedMesh property to get access.
		/// </summary>
		[Category( "Output" )]
		[DefaultValue( false )]
		public Reference<bool> CreateMeshComponent
		{
			get { if( _createMeshComponent.BeginGet() ) CreateMeshComponent = _createMeshComponent.Get( this ); return _createMeshComponent.value; }
			set { if( _createMeshComponent.BeginSet( this, ref value ) ) { try { CreateMeshComponentChanged?.Invoke( this ); } finally { _createMeshComponent.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CreateMeshComponent"/> property value changes.</summary>
		public event Action<CharacterMaker> CreateMeshComponentChanged;
		ReferenceField<bool> _createMeshComponent = false;

		[Browsable( false )]
		internal bool createMeshComponentForEditor;

		///// <summary>
		///// Whether to configure parent character via CharacterType property. When disabled CharacterType component will be created as a child.
		///// </summary>
		//[Category( "Output" )]
		//[DefaultValue( true )]
		//public Reference<bool> ApplyToParent
		//{
		//	get { if( _applyToParent.BeginGet() ) ApplyToParent = _applyToParent.Get( this ); return _applyToParent.value; }
		//	set { if( _applyToParent.BeginSet( this, ref value ) ) { try { ApplyToParentChanged?.Invoke( this ); NeedUpdate(); } finally { _applyToParent.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="ApplyToParent"/> property value changes.</summary>
		//public event Action<CharacterMaker> ApplyToParentChanged;
		//ReferenceField<bool> _applyToParent = true;

		///////////////////////////////////////////////

		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			base.OnMetadataGetMembersFilter( context, member, ref skip );

			//if( member is Metadata.Property )
			//{
			//	switch( member.Name )
			//	{
			//	case nameof( ApplyToParent ):
			//		if( Parent as Character == null )
			//			skip = true;
			//		break;
			//	}
			//}
		}

		[Browsable( false )]
		public Mesh CreatedMesh
		{
			get { return createdMesh; }
		}

		void UpdateCreatedMeshIfNeed()
		{
			if( needUpdateCreatedMesh )
				UpdateCreatedMesh( out _ );
		}

		protected override void OnEnabledInHierarchyChanged()
		{
			base.OnEnabledInHierarchyChanged();

			if( EnabledInHierarchyAndIsInstance )
			{
				needUpdateCreatedMesh = true;
				UpdateCreatedMeshIfNeed();
			}
			else
				DeleteCreatedMesh();
		}

		protected override void OnUpdate( float delta )
		{
			base.OnUpdate( delta );

			//!!!!slowly for cache?
			if( EnabledInHierarchyAndIsInstance )
				UpdateCreatedMeshIfNeed();
		}

		//!!!!when no sense to call this method?
		public void NeedUpdateCreatedMesh()
		{
			needUpdateCreatedMesh = true;
		}

		public bool UpdateCreatedMesh( out string error )
		{
			error = "";

			needUpdateCreatedMesh = false;

			if( !CreateMeshComponent && !createMeshComponentForEditor )
			{
				DeleteCreatedMesh();
				return false;
			}

			try
			{
				Mesh mesh;
				if( createdMesh != null )
				{
					mesh = createdMesh;
					mesh.Enabled = false;
					mesh.RemoveAllComponents( false );
				}
				else
					mesh = ComponentUtility.CreateComponent<Mesh>( null, true, false );

				GenerateMesh( mesh, false );
				mesh.Enabled = true;

				createdMesh = mesh;
				//createdApplyToParent = ApplyToParent;

				////update parent character
				//if( createdApplyToParent )
				//{
				//	var character = Parent as Character;
				//	if( character != null )
				//		character.CharacterType = createdType;
				//}
			}
			catch( Exception e )
			{
				error = e.Message;
				return false;
			}


			//DeleteCreatedMesh();
			//needUpdateCreatedMesh = false;

			//if( !CreateMeshComponent && !createMeshComponentForEditor )
			//	return false;

			//try
			//{
			//	var mesh = ComponentUtility.CreateComponent<Mesh>( null, true, false );
			//	GenerateMesh( mesh, false );
			//	mesh.Enabled = true;

			//	createdMesh = mesh;
			//	//createdApplyToParent = ApplyToParent;

			//	////update parent character
			//	//if( createdApplyToParent )
			//	//{
			//	//	var character = Parent as Character;
			//	//	if( character != null )
			//	//		character.CharacterType = createdType;
			//	//}
			//}
			//catch( Exception e )
			//{
			//	error = e.Message;
			//	return false;
			//}

			return true;
		}

		public void DeleteCreatedMesh()
		{
			var createdMesh2 = createdMesh;
			//var createdApplyToParent2 = createdApplyToParent;

			createdMesh = null;
			//createdApplyToParent = false;

			if( createdMesh2 != null )
			{
				////restore parent character
				//if( createdApplyToParent2 )
				//{
				//	var character = Parent as Character;
				//	if( character != null )
				//		character.CharacterType = null;
				//}

				createdMesh2.Dispose();
			}
		}

		public delegate void GenerateMeshEventDelegate( CharacterMaker sender, Mesh mesh, bool baking, ref bool handled );
		public event GenerateMeshEventDelegate GenerateMeshEvent;

		protected virtual void GenerateMesh( Mesh mesh, bool baking )
		{
			var handled = false;
			GenerateMeshEvent?.Invoke( this, mesh, baking, ref handled );
			if( handled )
				return;

			var sourceSkeleton = ResourceManager.LoadResource<Skeleton>( @"Base\Components\Character Maker\Template.skeleton" );
			var sourceHeight = 1.947;
			//var sourceHeightOfRootBone = 1.0;

			sourceSkeleton.GetBones( false, out var sourceBones, out var sourceBoneByName, out var sourceBoneParents );

			mesh.Name = "Mesh";

			//create a mesh geometry
			var geometry = mesh.CreateComponent<MeshGeometry>();
			{
				//!!!!can be smaller
				var vertexStructure = StandardVertex.MakeStructure( StandardVertex.Components.All, true, out int vertexSize );
				//var vertexStructure = StandardVertex.MakeStructure( StandardVertex.Components.StaticOneTexCoord, true, out int vertexSize );

				var lines = new List<(Line3, int)>();

				//foreach( var bone in sourceSkeleton.GetComponents<SkeletonBone>( checkChildren: true ) )
				//foreach( var bone in sourceBones )
				for( int nBone = 0; nBone < sourceBones.Length; nBone++ )
				{
					var bone = sourceBones[ nBone ];

					var parentBone = bone.Parent as SkeletonBone;
					if( parentBone != null )
					{
						var line = new Line3( parentBone.Transform.Value.Position, bone.Transform.Value.Position );
						lines.Add( (line, nBone) );
						//lines.Add( new Line3( parentBone.Transform.Value.Position, bone.Transform.Value.Position ) );
					}
				}

				var totalVertices = 24 * lines.Count;
				var totalIndices = 36 * lines.Count;

				var vertices = new byte[ vertexSize * totalVertices ];
				var indices = new int[ totalIndices ];

				unsafe
				{
					fixed( byte* pVertices = vertices )
					{
						StandardVertex* pVertices2 = (StandardVertex*)pVertices;
						//StandardVertex.StaticOneTexCoord* pVertices2 = (StandardVertex.StaticOneTexCoord*)pVertices;
						var currentVertex = 0;
						var currentIndex = 0;

						for( int nBox = 0; nBox < lines.Count; nBox++ )
						{
							var item = lines[ nBox ];
							var line = item.Item1;
							var boneIndex = item.Item2;

							var rot = Quaternion.FromDirectionZAxisUp( ( line.End - line.Start ).GetNormalize() );
							var length = ( line.End - line.Start ).Length();

							var thickness = Height.Value * 0.02;
							//var localBounds = new Bounds( -thickness, -thickness, -thickness, length + thickness, thickness, thickness );

							SimpleMeshGenerator.GenerateBox( new Vector3( length + thickness, thickness, thickness ), false, out var positions, out Vector3F[] normals, out var tangents, out var texCoords, out var indices2, out _ );
							for( int n = 0; n < positions.Length; n++ )
								positions[ n ] = positions[ n ] + new Vector3F( (float)length * 0.5f, 0, 0 );

							//SimpleMeshGenerator.GenerateBox( new Vector3( length + thickness * 2, thickness, thickness ), false, out var positions, out Vector3F[] normals, out var tangents, out var texCoords, out var indices2, out _ );

							var tr = new Transform( line.Start, rot, Vector3.One );
							var trRotate = new Transform( Vector3.Zero, rot, Vector3.One );

							var indexOffset = currentVertex;

							for( int n = 0; n < positions.Length; n++ )
							{
								var pVertex = pVertices2 + currentVertex;

								pVertex->Position = ( tr * positions[ n ] ).ToVector3F();
								pVertex->Normal = ( trRotate * normals[ n ] ).GetNormalize().ToVector3F();
								pVertex->Tangent = new Vector4F( ( trRotate * tangents[ n ].ToVector3F() ).GetNormalize().ToVector3F(), tangents[ n ].W );
								pVertex->Color = new ColorValue( 1, 1, 1, 1 );
								pVertex->TexCoord0 = texCoords[ n ];
								pVertex->TexCoord1 = texCoords[ n ];
								pVertex->TexCoord2 = texCoords[ n ];
								pVertex->TexCoord3 = texCoords[ n ];

								//!!!!может тут указывать родительский bone index

								pVertex->BlendIndices = new Vector4I( boneIndex, -1, -1, -1 );
								pVertex->BlendWeights = new Vector4F( 1, 0, 0, 0 );

								currentVertex++;
							}

							for( int n = 0; n < indices2.Length; n++ )
								indices[ currentIndex++ ] = indices2[ n ] + indexOffset;
						}
					}
				}



				//var boxes = new Bounds[ 8 ];
				//boxes[ 0 ] = new Bounds( -0.06, -0.13, 0.4, 0.06, 0.13, 0.8 );
				//boxes[ 1 ] = new Bounds( -0.08, -0.08, 0.8, 0.08, 0.08, 1.0 );
				//boxes[ 2 ] = new Bounds( -0.04, -0.04 - 0.05, 0.0, 0.04, 0.04 - 0.05, 0.4 );
				//boxes[ 3 ] = new Bounds( -0.04, -0.04 + 0.05, 0.0, 0.04, 0.04 + 0.05, 0.4 );
				//boxes[ 4 ] = new Bounds( -0.04, -0.04 - 0.05, 0.0, 0.10, 0.04 - 0.05, 0.05 );
				//boxes[ 5 ] = new Bounds( -0.04, -0.04 + 0.05, 0.0, 0.10, 0.04 + 0.05, 0.05 );
				//boxes[ 6 ] = new Bounds( -0.04, -0.04 - 0.17, 0.40, 0.04, 0.04 - 0.17, 0.75 );
				//boxes[ 7 ] = new Bounds( -0.04, -0.04 + 0.17, 0.40, 0.04, 0.04 + 0.17, 0.75 );

				////scale depending Height
				//for( int n = 0; n < boxes.Length; n++ )
				//{
				//	ref var b = ref boxes[ n ];
				//	b.Minimum *= Height.Value;
				//	b.Maximum *= Height.Value;
				//}

				//var totalVertices = 24 * boxes.Length;
				//var totalIndices = 36 * boxes.Length;

				//var vertices = new byte[ vertexSize * totalVertices ];
				//var indices = new int[ totalIndices ];

				//unsafe
				//{
				//	fixed( byte* pVertices = vertices )
				//	{
				//		//StandardVertex* pVertices2 = (StandardVertex*)pVertices;
				//		StandardVertex.StaticOneTexCoord* pVertices2 = (StandardVertex.StaticOneTexCoord*)pVertices;
				//		var currentVertex = 0;
				//		var currentIndex = 0;

				//		for( int nBox = 0; nBox < boxes.Length; nBox++ )
				//		{
				//			var b = boxes[ nBox ];

				//			SimpleMeshGenerator.GenerateBox( b.GetSize(), false, out var positions, out Vector3F[] normals, out var tangents, out var texCoords, out var indices2, out _ );

				//			var center = b.GetCenter().ToVector3F();

				//			var indexOffset = currentVertex;

				//			for( int n = 0; n < positions.Length; n++ )
				//			{
				//				var pVertex = pVertices2 + currentVertex;

				//				pVertex->Position = positions[ n ] + center;
				//				pVertex->Normal = normals[ n ];
				//				pVertex->Tangent = tangents[ n ];
				//				pVertex->Color = new ColorValue( 1, 1, 1, 1 );
				//				pVertex->TexCoord0 = texCoords[ n ];

				//				//!!!!
				//				//pVertex->TexCoord1 = texCoords[ n ];
				//				//pVertex->TexCoord2 = texCoords[ n ];
				//				//pVertex->TexCoord3 = texCoords[ n ];

				//				////!!!!
				//				//pVertex->BlendIndices = new Vector4I( 3, 0, 0, 0 );
				//				//pVertex->BlendWeights = new Vector4F( 1, 0, 0, 0 );

				//				currentVertex++;
				//			}

				//			for( int n = 0; n < indices2.Length; n++ )
				//				indices[ currentIndex++ ] = indices2[ n ] + indexOffset;
				//		}
				//	}
				//}


				////var vertexStructure = StandardVertex.MakeStructure( StandardVertex.Components.StaticOneTexCoord, true, out int vertexSize );

				////var size = new Vector3( Height / 6, Height / 3, Height );
				////SimpleMeshGenerator.GenerateBox( size, false, out var positions, out Vector3F[] normals, out var tangents, out var texCoords, out var indices, out _ );

				////var vertices = new byte[ vertexSize * positions.Length ];
				////unsafe
				////{
				////	fixed( byte* pVertices = vertices )
				////	{
				////		StandardVertex.StaticOneTexCoord* pVertex = (StandardVertex.StaticOneTexCoord*)pVertices;

				////		for( int n = 0; n < positions.Length; n++ )
				////		{
				////			pVertex->Position = positions[ n ] + new Vector3F( 0, 0, (float)Height.Value / 2 );
				////			pVertex->Normal = normals[ n ];
				////			pVertex->Tangent = tangents[ n ];
				////			pVertex->Color = new ColorValue( 1, 1, 1, 1 );
				////			pVertex->TexCoord0 = texCoords[ n ];

				////			pVertex++;
				////		}
				////	}
				////}


				geometry.Name = "Mesh Geometry";
				geometry.VertexStructure = vertexStructure;
				geometry.Vertices = vertices;
				geometry.Indices = indices;
			}

			//create a material
			var material = geometry.CreateComponent<Material>();
			{
				//!!!!make shader graph?
				//material.NewObjectSetDefaultConfiguration( false );

				material.Name = "Material";
				material.BaseColor = Color;

				if( baking )
					geometry.Material = ReferenceUtility.MakeThisReference( geometry, material );
				else
					geometry.Material = material;
			}

			//create a skeleton
			var skeleton = (Skeleton)sourceSkeleton.Clone();
			{
				skeleton.Name = "Skeleton";

				//apply scale to transform

				var scale = Height / sourceHeight;
				//var positionOffset = new Vector3( 0, 0, scale - 1.0 );

				foreach( var bone in skeleton.GetComponents<SkeletonBone>( checkChildren: true ) )
				{
					var tr = bone.Transform.Value;
					var tr2 = tr.UpdatePosition( tr.Position * scale );// + positionOffset );
					bone.Transform = tr2;
				}

				mesh.AddComponent( skeleton );

				if( baking )
					mesh.Skeleton = ReferenceUtility.MakeThisReference( mesh, skeleton );
				else
					mesh.Skeleton = skeleton;
			}
		}

		public bool WriteMesh( string writeToFolder, bool getFileNamesMode, List<string> fileNames, out string error )
		{
			error = "";

			try
			{
				var typeFileName = ComponentUtility.GetOwnedFileNameOfComponent( this );
				var realFileName = Path.Combine( writeToFolder, Path.GetFileNameWithoutExtension( typeFileName ) + ".mesh" );

				if( getFileNamesMode )
				{
					//only get file names
					fileNames.Add( realFileName );
				}
				else
				{
					var mesh = ComponentUtility.CreateComponent<Mesh>( null, true, false );
					GenerateMesh( mesh, true );
					mesh.Enabled = true;

					try
					{
						if( !ComponentUtility.SaveComponentToFile( mesh, realFileName, null, out error ) )
							return false;
					}
					finally
					{
						mesh.Dispose();
					}
				}
			}
			catch( Exception e )
			{
				error = e.Message;
				return false;
			}

			return true;
		}
	}
}
