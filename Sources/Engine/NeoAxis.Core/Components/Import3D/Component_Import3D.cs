// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Reflection;
using System.IO;
using System.Linq;
using NeoAxis.Import;
using NeoAxis.Editor;

namespace NeoAxis
{
	/// <summary>
	/// The component for import of 3D content.
	/// </summary>
	[EditorDocumentWindow( typeof( Component_Import3D_DocumentWindow ) )]
	[EditorPreviewControl( typeof( Component_Import3D_PreviewControl ) )]
	[EditorSettingsCell( typeof( Component_Import3D_SettingsCell ) )]
	public class Component_Import3D : Component
	{
		bool insideDoUpdate;

		public enum ModeEnum
		{
			Auto,
			OneMesh,
			Meshes,
			//!!!!impl
			//Scene,
		}
		/// <summary>
		/// The mode of the import.
		/// </summary>
		[DefaultValue( ModeEnum.Auto )]
		[Serialize]
		[Category( "Import Settings" )]
		public Reference<ModeEnum> Mode
		{
			get { if( _mode.BeginGet() ) Mode = _mode.Get( this ); return _mode.value; }
			set { if( _mode.BeginSet( ref value ) ) { try { ModeChanged?.Invoke( this ); } finally { _mode.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Mode"/> property value changes.</summary>
		public event Action<Component_Import3D> ModeChanged;
		ReferenceField<ModeEnum> _mode = ModeEnum.Auto;

		/// <summary>
		/// Specifies a result position offset of imported 3D models.
		/// </summary>
		[DefaultValue( "0 0 0" )]
		[Category( "Import Settings" )]
		public Reference<Vector3> Position
		{
			get { if( _position.BeginGet() ) Position = _position.Get( this ); return _position.value; }
			set { if( _position.BeginSet( ref value ) ) { try { PositionChanged?.Invoke( this ); } finally { _position.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Position"/> property value changes.</summary>
		public event Action<Component_Import3D> PositionChanged;
		ReferenceField<Vector3> _position;

		/// <summary>
		/// Specifies a result rotation of imported 3D models.
		/// </summary>
		[DefaultValue( "0 0 0 1" )]
		[Serialize]
		[Category( "Import Settings" )]
		public Reference<Quaternion> Rotation
		{
			get { if( _rotation.BeginGet() ) Rotation = _rotation.Get( this ); return _rotation.value; }
			set { if( _rotation.BeginSet( ref value ) ) { try { RotationChanged?.Invoke( this ); } finally { _rotation.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Rotation"/> property value changes.</summary>
		public event Action<Component_Import3D> RotationChanged;
		ReferenceField<Quaternion> _rotation = Quaternion.Identity;

		/// <summary>
		/// Specifies a result scale of imported 3D models.
		/// </summary>
		[DefaultValue( 1.0 )]
		[Serialize]
		[Category( "Import Settings" )]
		public Reference<double> Scale
		{
			get { if( _scale.BeginGet() ) Scale = _scale.Get( this ); return _scale.value; }
			set { if( _scale.BeginSet( ref value ) ) { try { ScaleChanged?.Invoke( this ); } finally { _scale.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Scale"/> property value changes.</summary>
		public event Action<Component_Import3D> ScaleChanged;
		ReferenceField<double> _scale = 1.0;

		/// <summary>
		/// Whether to change a coordinate system to engine system.
		/// </summary>
		[DefaultValue( true )]
		[Serialize]
		[DisplayName( "Force Front X Axis" )]
		[Category( "Import Settings" )]
		public Reference<bool> ForceFrontXAxis
		{
			get { if( _forceFrontXAxis.BeginGet() ) ForceFrontXAxis = _forceFrontXAxis.Get( this ); return _forceFrontXAxis.value; }
			set { if( _forceFrontXAxis.BeginSet( ref value ) ) { try { ForceFrontXAxisChanged?.Invoke( this ); } finally { _forceFrontXAxis.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ForceFrontXAxis"/> property value changes.</summary>
		public event Action<Component_Import3D> ForceFrontXAxisChanged;
		ReferenceField<bool> _forceFrontXAxis = true;

		/// <summary>
		/// The range of levels of detail to import.
		/// </summary>
		[DefaultValue( "0 10" )]
		[Range( 0, 10 )]
		[DisplayName( "LOD Range" )]
		[Category( "Import Settings" )]
		public Reference<RangeI> LODRange
		{
			get { if( _lODRange.BeginGet() ) LODRange = _lODRange.Get( this ); return _lODRange.value; }
			set { if( _lODRange.BeginSet( ref value ) ) { try { LODRangeChanged?.Invoke( this ); } finally { _lODRange.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="LODRange"/> property value changes.</summary>
		public event Action<Component_Import3D> LODRangeChanged;
		ReferenceField<RangeI> _lODRange = new RangeI( 0, 10 );

		/// <summary>
		/// The distance from the previous to the next level of detail. To apply changes need to do re-import.
		/// </summary>
		[DisplayName( "LOD Distance" )]
		[Category( "Import Settings" )]
		[DefaultValue( 10.0 )]
		[Range( 1, 50, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		public Reference<double> LODDistance
		{
			get { if( _lODDistance.BeginGet() ) LODDistance = _lODDistance.Get( this ); return _lODDistance.value; }
			set { if( _lODDistance.BeginSet( ref value ) ) { try { LODDistanceChanged?.Invoke( this ); } finally { _lODDistance.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="LODDistance"/> property value changes.</summary>
		public event Action<Component_Import3D> LODDistanceChanged;
		ReferenceField<double> _lODDistance = 10.0;

		/// <summary>
		/// Whether to merge geometries with the same format.
		/// </summary>
		[DefaultValue( true )]
		[Serialize]
		[Category( "Import Settings" )]
		public Reference<bool> MergeMeshGeometries
		{
			get { if( _mergeMeshGeometries.BeginGet() ) MergeMeshGeometries = _mergeMeshGeometries.Get( this ); return _mergeMeshGeometries.value; }
			set { if( _mergeMeshGeometries.BeginSet( ref value ) ) { try { MergeMeshGeometriesChanged?.Invoke( this ); } finally { _mergeMeshGeometries.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="MergeMeshGeometries"/> property value changes.</summary>
		public event Action<Component_Import3D> MergeMeshGeometriesChanged;
		ReferenceField<bool> _mergeMeshGeometries = true;

		/// <summary>
		/// Whether to flip UV coordinates by vertical axis.
		/// </summary>
		[DefaultValue( false )]
		[Serialize]
		[Category( "Import Settings" )]
		public Reference<bool> FlipUVs
		{
			get { if( _flipUVs.BeginGet() ) FlipUVs = _flipUVs.Get( this ); return _flipUVs.value; }
			set { if( _flipUVs.BeginSet( ref value ) ) { try { FlipUVsChanged?.Invoke( this ); } finally { _flipUVs.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="FlipUVs"/> property value changes.</summary>
		public event Action<Component_Import3D> FlipUVsChanged;
		ReferenceField<bool> _flipUVs;

		/// <summary>
		/// Whether to delete unused materials.
		/// </summary>
		[DefaultValue( true )]
		[Serialize]
		[Category( "Import Settings" )]
		public Reference<bool> DeleteUnusedMaterials
		{
			get { if( _deleteUnusedMaterials.BeginGet() ) DeleteUnusedMaterials = _deleteUnusedMaterials.Get( this ); return _deleteUnusedMaterials.value; }
			set { if( _deleteUnusedMaterials.BeginSet( ref value ) ) { try { DeleteUnusedMaterialsChanged?.Invoke( this ); } finally { _deleteUnusedMaterials.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DeleteUnusedMaterials"/> property value changes.</summary>
		public event Action<Component_Import3D> DeleteUnusedMaterialsChanged;
		ReferenceField<bool> _deleteUnusedMaterials = true;

		/// <summary>
		/// Whether to import Displacement channel for materials.
		/// </summary>
		[DefaultValue( false )]
		[Category( "Import Settings" )]
		public Reference<bool> MaterialDisplacement
		{
			get { if( _materialDisplacement.BeginGet() ) MaterialDisplacement = _materialDisplacement.Get( this ); return _materialDisplacement.value; }
			set { if( _materialDisplacement.BeginSet( ref value ) ) { try { MaterialDisplacementChanged?.Invoke( this ); } finally { _materialDisplacement.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="MaterialDisplacement"/> property value changes.</summary>
		public event Action<Component_Import3D> MaterialDisplacementChanged;
		ReferenceField<bool> _materialDisplacement = false;

		/////////////////////////////////////////

		protected override void OnEnabledInHierarchyChanged()
		{
			base.OnEnabledInHierarchyChanged();

			if( !insideDoUpdate && EnabledInHierarchy && IsNeedUpdate() )
			{
				if( !DoUpdate( null, out string error ) )
				{
					var virtualFileName = ParentRoot.HierarchyController?.CreatedByResource?.Owner.Name;
					if( string.IsNullOrEmpty( virtualFileName ) )
						virtualFileName = "NO FILE NAME";
					var error2 = $"Unable to load or import resource \"{virtualFileName}\".\r\n\r\n" + error;
					Log.Error( error2 );
					return;
				}

				//save to file
				if( HierarchyController != null && HierarchyController.CreatedByResource != null &&
					HierarchyController.CreatedByResource.InstanceType == Resource.InstanceType.Resource &&
					HierarchyController.CreatedByResource.Owner.LoadFromFile )
				{
					var resource = HierarchyController.CreatedByResource.Owner;
					string virtualPath = resource.Name + resource.GetSaveAddFileExtension();
					string realPath = VirtualPathUtility.GetRealPathByVirtual( virtualPath );

					if( !File.Exists( realPath ) && Components.Count != 0 )
					{
						if( !ComponentUtility.SaveComponentToFile( this, realPath, null, out error ) )
						{
							Log.Warning( error );
							return;
						}
					}
				}
			}
		}

		public bool IsNeedUpdate()
		{
			if( Parent == null )
			{
				//!!!!good?
				if( Components.Count == 0 )
					return true;

				//!!!!хотя проверять тоже долго ведь. варианты: в дейплойменте сорцы удалить (сделать пустые файлы)
			}

			return false;
		}

		/// <summary>
		/// Represents settings for reimporting data of <see cref="Component_Import3D"/>.
		/// </summary>
		public class ReimportSettings
		{
			[DefaultValue( true )]
			[Category( "Options" )]
			//[Description( "" )]
			public bool UpdateMaterials { get; set; } = true;

			[DefaultValue( true )]
			[Category( "Options" )]
			public bool UpdateMeshes { get; set; } = true;

			[DefaultValue( true )]
			[Category( "Options" )]
			public bool UpdateObjectsInSpace { get; set; } = true;
		}

		public delegate void UpdatePostProcessDelegate( Component_Import3D import, ImportGeneral.Settings settings );
		public static event UpdatePostProcessDelegate UpdatePostProcess;

		public bool DoUpdate( ReimportSettings reimportSettings, out string error )
		{
			try
			{
				insideDoUpdate = true;

				if( reimportSettings == null )
					reimportSettings = new ReimportSettings();

				error = "";

				//bool createDocumentConfig = Components.Count == 0;

				//disable _Import3D
				bool wasEnabled = Enabled;
				Enabled = false;
				try
				{
					var settings = new ImportGeneral.Settings();
					settings.updateMaterials = reimportSettings.UpdateMaterials;
					settings.updateMeshes = reimportSettings.UpdateMeshes;
					settings.updateObjectsInSpace = reimportSettings.UpdateObjectsInSpace;

					//remove old objects
					if( settings.updateObjectsInSpace )
					{
						var c = GetComponent( "Object In Space" );
						if( c != null )
							RemoveComponent( c, false );
						c = GetComponent( "Objects In Space" );
						if( c != null )
							RemoveComponent( c, false );
					}
					if( settings.updateMeshes )
					{
						var c = GetComponent( "Mesh" );
						if( c != null )
							RemoveComponent( c, false );
						c = GetComponent( "Meshes" );
						if( c != null )
							RemoveComponent( c, false );
					}
					if( settings.updateMaterials )
					{
						var c = GetComponent( "Material" );
						if( c != null )
							RemoveComponent( c, false );
						c = GetComponent( "Materials" );
						if( c != null )
							RemoveComponent( c, false );
					}

					if( Parent != null || ParentRoot.HierarchyController == null || ParentRoot.HierarchyController.CreatedByResource == null ||
						!ParentRoot.HierarchyController.CreatedByResource.Owner.LoadFromFile )
					{
						error = "This component must be root of the resource.";
						return false;
					}
					var virtualFileName = ParentRoot.HierarchyController.CreatedByResource.Owner.Name;

					settings.component = this;
					settings.virtualFileName = virtualFileName;

					if( Path.GetExtension( virtualFileName ).ToLower() == ".json" )
					{
						//Quixel Megascans

						ImportMegascans.DoImport( settings, out error );
						if( !string.IsNullOrEmpty( error ) )
							return false;
					}
					else if( Path.GetExtension( virtualFileName ).ToLower() == ".fbx" )
					{
						//FBX

						//settings.loadAnimations = true;
						//!!!!to options?
						settings.frameStep = .25;

						Import.FBX.ImportFBX.DoImport( settings, out error );
						if( !string.IsNullOrEmpty( error ) )
							return false;
					}
					else
					{
						//Assimp

						//settings.loadAnimations = false;

						ImportAssimp.DoImport( settings, out error );
						if( !string.IsNullOrEmpty( error ) )
							return false;
					}

					UpdatePostProcess?.Invoke( this, settings );

					//DeleteUnusedMaterials
					if( settings.updateMaterials && DeleteUnusedMaterials && !settings.disableDeletionUnusedMaterials )
					{
						var usedMaterials = new ESet<Component_Material>();
						foreach( var meshGeometry in GetComponents<Component_MeshGeometry>( false, true ) )
						{
							var material = meshGeometry.Material.Value;
							if( material != null )
								usedMaterials.AddWithCheckAlreadyContained( material );
						}

						again:
						foreach( var material in GetComponents<Component_Material>( false, true ) )
						{
							if( !usedMaterials.Contains( material ) )
							{
								material.RemoveFromParent( false );
								material.Dispose();
								goto again;
							}
						}
					}
				}
				finally
				{
					//enable _Import3D
					if( wasEnabled )
						Enabled = true;
				}

				//set EditorDocumentConfiguration
				//if( createDocumentConfig )
				{
					//!!!!что еще при Mode = Scene

					var toSelect = new List<Component>();

					//root object
					toSelect.Add( this );

					//mesh
					var mesh = GetComponent( "Mesh" ) as Component_Mesh;
					if( mesh != null )
						toSelect.Add( mesh );

					//materials
					var materials = GetComponents<Component_Material>( false, true );
					if( materials.Length <= 4 )
					{
						foreach( var material in materials )
						{
							var graph = material.GetComponent<Component_FlowGraph>();
							if( graph != null && graph.TypeSettingsIsPublic() )
								toSelect.Add( graph );
						}
					}

					//select window with mesh or select window with root object
					var selectObject = mesh != null ? (Component)mesh : this;

					EditorDocumentConfiguration = KryptonConfigGenerator.CreateEditorDocumentXmlConfiguration( toSelect, selectObject );

					//update windows
					//!!!!какие-то еще проверки?
					if( EngineApp.ApplicationType == EngineApp.ApplicationTypeEnum.Editor )
					{
						var document = EditorAPI.GetDocumentByObject( this );
						if( document != null )
						{
							////close old windows
							//EditorAPI.CloseAllDocumentWindowsOnSecondLevel( document );

							//open windows
							foreach( var obj in toSelect )
							{
								if( obj != this )
									EditorAPI.OpenDocumentWindowForObject( document, obj );
							}

							//delete old windows
							EditorAPI.CloseAllDocumentWindowsOnSecondLevelWithDeletedObjects( document );

							//select window
							var windows = EditorAPI.FindDocumentWindowsWithObject( selectObject );
							if( windows.Count != 0 )
								EditorAPI.SelectDockWindow( windows[ 0 ] );
						}

						//!!!!or restore window configuration from EditorDocumentConfiguration
					}

				}
			}
			finally
			{
				insideDoUpdate = false;
			}

			return true;
		}

		[Browsable( false )]
		public long VersionForPreviewDisplay
		{
			get;
			set;
		}

		static void FixRootReferences( Component_Import3D import3D )
		{
			bool Fix( IReference reference, out string newReference )
			{
				if( reference.ReferenceSpecified )
				{
					var referenceValue = reference.GetByReference;
					if( referenceValue.Length >= "root:".Length && referenceValue.Substring( 0, 5 ) == "root:" )
					{
						newReference = referenceValue.Insert( 5, "$Import3D\\" );
						return true;
					}
				}
				newReference = null;
				return false;
			}

			//Component_MeshGeometry.Material
			foreach( var meshGeometry in import3D.GetComponents<Component_MeshGeometry>( false, true ) )
			{
				if( Fix( meshGeometry.Material, out var newReference ) )
					meshGeometry.Material = ReferenceUtility.MakeReference<Component_Material>( null, newReference );
			}

			//Component_MeshInSpace.Mesh
			foreach( var meshInSpace in import3D.GetComponents<Component_MeshInSpace>( false, true ) )
			{
				if( Fix( meshInSpace.Mesh, out var newReference ) )
					meshInSpace.Mesh = ReferenceUtility.MakeReference<Component_Mesh>( null, newReference );
			}
		}

		public Component_Import3D CreateForPreviewDisplay( Component_Scene scene, out bool onlyOneMaterial, out Dictionary<Component_Mesh, Transform> transformBySourceMesh )
		{
			onlyOneMaterial = false;
			transformBySourceMesh = new Dictionary<Component_Mesh, Transform>();

			var createdObject = (Component_Import3D)Clone();
			createdObject.Name = "Import3D";
			FixRootReferences( createdObject );
			scene.AddComponent( createdObject );

			//create MeshInSpace for one mesh
			{
				var mesh = createdObject.GetComponent( "Mesh" ) as Component_Mesh;
				if( mesh != null )
				{
					var objectInSpace = createdObject.CreateComponent<Component_MeshInSpace>();
					objectInSpace.Mesh = ReferenceUtility.MakeReference<Component_Mesh>( null, ReferenceUtility.CalculateThisReference( objectInSpace, mesh ) );
				}
			}

			//create meshes in space for Meshes
			{
				var sourceMeshesGroup = GetComponent( "Meshes" );
				Component_Mesh[] sourceMeshes = null;
				if( sourceMeshesGroup != null )
					sourceMeshes = sourceMeshesGroup.GetComponents<Component_Mesh>();

				var createdMeshesGroup = createdObject.GetComponent( "Meshes" );
				if( createdMeshesGroup != null )
				{
					var createdMeshes = createdMeshesGroup.GetComponents<Component_Mesh>();

					var positions = new Vector3[ createdMeshes.Length ];
					{
						if( sourceMeshesGroup != null )
						{
							double maxSizeY = 0;
							foreach( var sourceMesh in sourceMeshesGroup.GetComponents<Component_Mesh>() )
							{
								if( sourceMesh.Result != null )
								{
									var bounds = sourceMesh.Result.SpaceBounds.CalculatedBoundingBox;
									var sizeY = bounds.GetSize().Y;
									if( sizeY > maxSizeY )
										maxSizeY = sizeY;
								}
							}
							var step = maxSizeY * 1.2;
							var totalSize = (double)( createdMeshes.Length - 1 ) * step;

							for( int n = 0; n < createdMeshes.Length; n++ )
							{
								var positionY = (double)n * step - totalSize / 2;
								positions[ n ] = new Vector3( 0, positionY, 0 );
							}
						}
					}

					for( int n = 0; n < createdMeshes.Length; n++ )
					{
						var createdMesh = createdMeshes[ n ];

						var transform = new Transform( positions[ n ], Quaternion.Identity );

						var objectInSpace = createdObject.CreateComponent<Component_MeshInSpace>();
						objectInSpace.Transform = transform;
						objectInSpace.Mesh = ReferenceUtility.MakeReference<Component_Mesh>( null, ReferenceUtility.CalculateThisReference( objectInSpace, createdMesh ) );

						if( sourceMeshes != null && sourceMeshes.Length == createdMeshes.Length )
							transformBySourceMesh[ sourceMeshes[ n ] ] = transform;
					}
				}
			}

			//create MeshInSpace for only one material
			{
				var material = createdObject.GetComponent( "Material" ) as Component_Material;
				if( material != null )
				{
					var mesh = ProjectSettings.Get.MaterialPreviewMesh.Value;
					if( mesh != null )
					{
						var objectInSpace = createdObject.CreateComponent<Component_MeshInSpace>();
						objectInSpace.Mesh = mesh;

						objectInSpace.ReplaceMaterial = ReferenceUtility.MakeReference<Component_Material>( null, ReferenceUtility.CalculateThisReference( objectInSpace, material ) );

						onlyOneMaterial = true;
					}
				}
			}

			return createdObject;
		}
	}
}
