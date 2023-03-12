// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using NeoAxis.Editor;

namespace NeoAxis
{
	/// <summary>
	/// A logical element of road system to connect roads. It indended to make crossroads and other road elements.
	/// </summary>
	public class RoadNode : ObjectInSpace
	{
		bool sceneIsReady;

		LogicalData logicalData;
		//bool logicalDataUpdating;
		RoadUtility.VisualData visualData;
		bool needUpdate;
		bool needUpdateAfterEndModifyingTransformTool;

		///////////////////////////////////////////////

		//!!!!при обновлении может только удалять visual data
		/// <summary>
		/// The base color and opacity multiplier.
		/// </summary>
		[DefaultValue( "1 1 1" )]
		[Category( "Display" )]
		public Reference<ColorValue> ColorMultiplier
		{
			get { if( _colorMultiplier.BeginGet() ) ColorMultiplier = _colorMultiplier.Get( this ); return _colorMultiplier.value; }
			set { if( _colorMultiplier.BeginSet( ref value ) ) { try { ColorMultiplierChanged?.Invoke( this ); DataWasChanged(); } finally { _colorMultiplier.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ColorMultiplier"/> property value changes.</summary>
		public event Action<RoadNode> ColorMultiplierChanged;
		ReferenceField<ColorValue> _colorMultiplier = ColorValue.One;

		[DefaultValue( true )]
		public Reference<bool> Collision
		{
			get { if( _collision.BeginGet() ) Collision = _collision.Get( this ); return _collision.value; }
			set { if( _collision.BeginSet( ref value ) ) { try { CollisionChanged?.Invoke( this ); DataWasChanged(); } finally { _collision.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Collision"/> property value changes.</summary>
		public event Action<RoadNode> CollisionChanged;
		ReferenceField<bool> _collision = true;

		///////////////////////////////////////////////

		public class LogicalData
		{
			public RoadNode Owner;
			public Scene Scene;

			public List<Scene.PhysicsWorldClass.Body> CollisionBodies = new List<Scene.PhysicsWorldClass.Body>();

			/////////////////////

			public LogicalData( RoadNode owner )
			{
				Owner = owner;
				Scene = Owner.ParentScene;
			}

			public virtual void OnDelete()
			{
				DestroyCollisionBodies();
			}

			internal void UpdateCollisionBodies()
			{
				DestroyCollisionBodies();

				if( !Owner.Collision.Value )
					return;
				if( RoadUtility.IsAnyTransformToolInModifyingMode() )
				{
					Owner.needUpdateAfterEndModifyingTransformTool = true;
					return;
				}

				var manager = Scene.GetComponent<RoadManager>();
				if( manager == null || manager.Collision )
				{
					Owner.OnCalculateCollisionBodies();
				}
			}

			public void DestroyCollisionBodies()
			{
				if( CollisionBodies.Count != 0 )
				{
					foreach( var body in CollisionBodies )
						body.Dispose();
					CollisionBodies.Clear();
				}
			}

			public virtual void OnConnectedRoadDelete( Road.RoadData roadData )
			{
			}

			public virtual void GetModifiersForRoad( Road.RoadData roadData, List<Road.Modifier> modifiers )
			{
			}
		}

		///////////////////////////////////////////////

		protected virtual LogicalData OnCalculateLogicalData()
		{
			var logicalData = new LogicalData( this );
			return logicalData;
		}

		protected virtual void OnCalculateCollisionBodies() { }

		public LogicalData GetLogicalData( bool canCreate = true )
		{
			if( logicalData == null && EnabledInHierarchyAndIsInstance && canCreate && ParentScene != null )
			//if( !logicalDataUpdating /*logicalData == null*/ && EnabledInHierarchyAndIsInstance && canCreate && ParentScene != null )
			{
				//logicalDataUpdating = true;
				//try
				//{
				logicalData = OnCalculateLogicalData();

				if( sceneIsReady )
					logicalData?.UpdateCollisionBodies();
				//}
				//finally
				//{
				//	logicalDataUpdating = false;
				//}
			}
			return logicalData;
		}

		void DeleteLogicalData()
		{
			if( logicalData != null )
			{
				logicalData.OnDelete();
				logicalData = null;
			}
		}

		protected virtual void OnCalculateVisualData( RoadUtility.VisualData visualData )
		{
		}

		public RoadUtility.VisualData GetVisualData()
		{
			if( visualData == null )
			{
				GetLogicalData();
				if( logicalData != null && ParentScene != null )
				{
					if( !RoadUtility.UpdateVisualDataInModifyingMode && RoadUtility.IsAnyTransformToolInModifyingMode() )
					{
						needUpdateAfterEndModifyingTransformTool = true;
						return null;
					}

					visualData = new RoadUtility.VisualData( this );

					var manager = logicalData.Scene.GetComponent<RoadManager>();
					if( manager == null || manager.Display )
					{
						OnCalculateVisualData( visualData );
					}
				}
			}
			return visualData;
		}

		void DeleteVisualData()
		{
			visualData?.Dispose();
			visualData = null;
		}

		protected override void OnEnabledInHierarchyChanged()
		{
			base.OnEnabledInHierarchyChanged();

			var scene = ParentScene;
			if( scene != null )
			{
				if( EnabledInHierarchyAndIsInstance )
				{
					scene.EnabledInHierarchyChanged += Scene_EnabledInHierarchyChanged;
					scene.ViewportUpdateBefore += Scene_ViewportUpdateBefore;
#if !DEPLOY
					TransformTool.AllInstances_ModifyCommit += TransformTool_AllInstances_ModifyCommit;
					TransformTool.AllInstances_ModifyCancel += TransformTool_AllInstances_ModifyCancel;
#endif

					if( logicalData == null )
						Update();
				}
				else
				{
					scene.EnabledInHierarchyChanged -= Scene_EnabledInHierarchyChanged;
					scene.ViewportUpdateBefore -= Scene_ViewportUpdateBefore;
#if !DEPLOY
					TransformTool.AllInstances_ModifyCommit -= TransformTool_AllInstances_ModifyCommit;
					TransformTool.AllInstances_ModifyCancel -= TransformTool_AllInstances_ModifyCancel;
#endif

					Update();
				}
			}
		}

#if !DEPLOY
		private void TransformTool_AllInstances_ModifyCommit( TransformTool sender )
		{
			if( needUpdateAfterEndModifyingTransformTool )
			{
				NeedUpdate();
				//Update();
				needUpdateAfterEndModifyingTransformTool = false;
			}
		}

		private void TransformTool_AllInstances_ModifyCancel( TransformTool sender )
		{
			if( needUpdateAfterEndModifyingTransformTool )
			{
				NeedUpdate();
				//Update();
				needUpdateAfterEndModifyingTransformTool = false;
			}
		}
#endif

		public void Update()
		{
			//!!!!
			//var size = Size.Value;
			//var totalCells = size.X * size.Y * size.Z;
			//if( totalCells > 500 )
			//{
			//	if( IsAnyTransformToolInModifyingMode() )
			//	{
			//		needUpdateAfterEndModifyingTransformTool = true;
			//		return;
			//	}
			//}

			DeleteVisualData();
			DeleteLogicalData();

			if( EnabledInHierarchyAndIsInstance )
			{
				GetLogicalData();
				SpaceBoundsUpdate();
			}

			needUpdate = false;
		}

		protected override void OnUpdate( float delta )
		{
			base.OnUpdate( delta );

			if( EnabledInHierarchyAndIsInstance )
			{
				if( needUpdate )
					Update();
			}
		}

		private void Scene_EnabledInHierarchyChanged( Component obj )
		{
			if( obj.EnabledInHierarchyAndIsInstance )
			{
				if( needUpdate )
					Update();

				var logicalData = GetLogicalData();
				logicalData?.UpdateCollisionBodies();

				sceneIsReady = true;
			}
		}

		private void Scene_ViewportUpdateBefore( Scene scene, Viewport viewport, Viewport.CameraSettingsClass overrideCameraSettings )
		{
			//!!!!лучше всё же в OnGetRenderSceneData
			//хотя SpaceBounds видимо больше должен быть, учитывать габариты мешей
			//когда долго не видим объект, данные можно удалять. лишь бы не было что создается/удаляется каждый раз

			//!!!!может VisibleInHierarchy менять флаг в объектах

			if( VisibleInHierarchy )
				GetVisualData();
			else
				DeleteVisualData();
		}

		protected override void OnGetRenderSceneData( ViewportRenderingContext context, GetRenderSceneDataMode mode, Scene.GetObjectsInSpaceItem modeGetObjectsItem )
		{
			base.OnGetRenderSceneData( context, mode, modeGetObjectsItem );

			//!!!!
			////!!!!here?
			//visualData?.groupOfObjects?.EndUpdate();

			//!!!!тут?
			////!!!!не сразу обновляет если только тут
			//GetVisualData();

			if( EnabledInHierarchy && VisibleInHierarchy && mode == GetRenderSceneDataMode.InsideFrustum )
			{
				//debug lines
				var visualData = GetVisualData();
				if( visualData != null )
				{
					var renderer = context.Owner.Simple3DRenderer;

					if( visualData.DebugLines.Count != 0 )
					{
						renderer.SetColor( new ColorValue( 1, 0, 0 ) );
						foreach( var line in visualData.DebugLines )
							renderer.AddLine( line );
					}
				}
			}
		}

		protected override void OnTransformChanged()
		{
			base.OnTransformChanged();

			NeedUpdate();
		}

		public void NeedUpdate()
		{
			needUpdate = true;
		}

		public override void NewObjectSetDefaultConfiguration( bool createdFromNewObjectWindow = false )
		{
			SetScale( 20, 20, 2 );
		}

		public override ScreenLabelInfo GetScreenLabelInfo()
		{
			return new ScreenLabelInfo( "Volume" );
		}

		public virtual void DataWasChanged()
		{
			if( EnabledInHierarchyAndIsInstance )
				needUpdate = true;
		}
	}
}
