// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Reflection;
using System.ComponentModel;
using System.Drawing.Design;
using System.IO;
using NeoAxis.Editor;

namespace NeoAxis
{
	/// <summary>
	/// Abstract object in the scene.
	/// </summary>
	[ResourceFileExtension( "objectInSpace" )]
	[EditorDocumentWindow( typeof( Component_ObjectInSpace_Editor ), true )]
	[EditorPreviewControl( typeof( Component_ObjectInSpace_Preview ) )]
	[EditorPreviewImage( typeof( Component_ObjectInSpace_PreviewImage ) )]
	public class Component_ObjectInSpace : Component, IComponent_VisibleInHierarchy, IComponent_CanBeSelectedInHierarchy//, Component_Scene_DocumentWindow.ICanDropToScene
	{
		Component_Scene parentSceneCached;

		//scene octree
		//internal int sceneOctreeGroup;
		internal int sceneOctreeIndex = -1;

		/////////////////////////////////////////

		//!!!!
		//!!!!тут ли? он уж какой-то особо особый тут. хотя нужен многим
		//!!!!reference?
		//!!!!как вариант: сделать обычным параметром. при оббегании для рисовании  отсекать невидимые (НЕ ТАК, сцене граф же), глубже не идти.
		//!!!!!!!!тогда Visible станет самым обычным параметром
		//!!!!тут надо эвент и референс, т.к. довольно таки очевидный флаг видимости объекта в зависимости от стейта
		//!!!!с другой стороны может стать уж слишком медленно? если не юзать ссылки то не станет? хотя они обновляются иерархически же
		//!!!!!а тут ли? может это в Component_ObjectInScene?
		//bool visible = true;
		//bool visibleInHierarchy;//!!!!? = true;

		/// <summary>
		/// Whether the object is visible in the scene.
		/// </summary>
		[Serialize]
		[DefaultValue( true )]
		public Reference<bool> Visible
		{
			get { if( _visible.BeginGet() ) Visible = _visible.Get( this ); return _visible.value; }
			set
			{
				if( _visible.BeginSet( ref value ) )
				{
					try
					{
						VisibleChanged?.Invoke( this );

						//!!!!
						//OnVisibleChanged();

						//_UpdateVisibleInHierarchy( false );

						//transform = new Reference<Transform>( new Transform( visible, rotation, scale ), transform.GetByReference );
						//VisibleChanged?.Invoke( this );
						//TransformChanged?.Invoke( this );
					}
					finally { _visible.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Visible"/> property value changes.</summary>
		public event Action<Component_ObjectInSpace> VisibleChanged;
		ReferenceField<bool> _visible = true;


		//!!!!надо?
		//protected virtual void OnVisibleChanged() { }

		//!!!!надо?
		//protected virtual void OnVisibleInHierarchyChanged()
		//{
		//	//notify components
		//	Component[] array = new Component[ components.Count ];
		//	components.CopyTo( array, 0 );

		//	foreach( Component c in array )
		//	{
		//		if( c.Parent == this )
		//			c._UpdateVisibleInHierarchy( false );
		//	}
		//}

		//public event Action<Component> VisibleChanged;

		//!!!!было
		//public event Action<Component> VisibleInHierarchyChanged;

		[Browsable( false )]
		public bool VisibleInHierarchy
		{
			get
			{
				//!!!!slowly

				if( !Visible )
					return false;

				var p = Parent as IComponent_VisibleInHierarchy;
				if( p != null )
					return p.VisibleInHierarchy;
				else
					return true;
				//var p = Parent as Component_ObjectInSpace;
				//if( p != null )
				//	return p.VisibleInHierarchy;
				//else
				//	return true;

				//return visibleInHierarchy;
			}
		}

		//!!!!
		//internal void _UpdateVisibleInHierarchy( bool forceDisableBeforeRemove )
		//{
		//	bool demand;
		//	if( Visible && !forceDisableBeforeRemove )
		//	{
		//		if( parent != null )
		//			demand = parent.VisibleInHierarchy;
		//		else
		//		{
		//			if( hierarchyController != null )
		//				demand = hierarchyController.HierarchyVisible;
		//			else
		//				demand = false;
		//		}
		//	}
		//	else
		//		demand = false;

		//	if( visibleInHierarchy != demand )
		//	{
		//		visibleInHierarchy = demand;

		//		OnVisibleInHierarchyChanged();
		//		VisibleInHierarchyChanged?.Invoke( this );
		//	}
		//}

		//!!!!

		/// <summary>
		/// Whether the object is selectable in the scene view.
		/// </summary>
		[Serialize]
		[DefaultValue( true )]
		public Reference<bool> CanBeSelected
		{
			get { if( _canBeSelected.BeginGet() ) CanBeSelected = _canBeSelected.Get( this ); return _canBeSelected.value; }
			set { if( _canBeSelected.BeginSet( ref value ) ) { try { CanBeSelectedChanged?.Invoke( this ); } finally { _canBeSelected.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CanBeSelected"/> property value changes.</summary>
		public event Action<Component_ObjectInSpace> CanBeSelectedChanged;
		ReferenceField<bool> _canBeSelected = true;

		[Browsable( false )]
		public bool CanBeSelectedInHierarchy
		{
			get
			{
				if( !CanBeSelected )
					return false;

				var p = Parent as IComponent_CanBeSelectedInHierarchy;
				if( p != null )
					return p.CanBeSelectedInHierarchy;
				else
					return true;
				//var p = Parent as Component_ObjectInSpace;
				//if( p != null )
				//	return p.CanBeSelectedInHierarchy;
				//else
				//	return true;
			}
		}

		protected virtual void OnTransformUpdating( ref Reference<Transform> value )
		{
		}
		public delegate void TransformUpdatingEventDelegate( Component_ObjectInSpace obj, ref Reference<Transform> value );
		public event TransformUpdatingEventDelegate TransformUpdatingEvent;

		/// <summary>
		/// The position, rotation and scale of the object.
		/// </summary>
		[Serialize]
		//[DefaultValue( "0 0 0" )]
		public Reference<Transform> Transform
		{
			get
			{
				//if( !EngineApp._DebugCapsLock )
				//{
				//	//!!!!try, catch
				//	//!!!!быстрая проверка на строку
				//	if( _transform.value.GetByReference == "this:$Collision Body\\Transform" )
				//	{
				//		var body = GetComponent<Component_RigidBody>();
				//		if( body.Name == "Collision Body" )
				//		{
				//			//Transform = body.Transform;
				//			return new Reference<Transform>( body.Transform.Value, _transform.value.GetByReference );
				//		}
				//	}
				//}

				if( _transform.BeginGet() ) Transform = _transform.Get( this ); return _transform.value;
			}
			set
			{
				//!!!!threading. надо ли. как. где еще так

				//!!!!slowly?
				//!!!!!!возможно выставлять без проверок. SetTransformNoChecks. или с помощью переменной игнорировать тут проверку

				//!!!!new. так?
				//fix invalid value
				if( value.Value == null )
					value = new Reference<Transform>( NeoAxis.Transform.Identity, value.GetByReference );

				OnTransformUpdating( ref value );
				TransformUpdatingEvent?.Invoke( this, ref value );

				if( _transform.BeginSet( ref value ) )
				{
					try
					{
						TransformChanged?.Invoke( this );
						OnTransformChanged();
					}
					finally { _transform.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Transform"/> property value changes.</summary>
		public event Action<Component_ObjectInSpace> TransformChanged;
		ReferenceField<Transform> _transform = new Transform( Vector3.Zero, Quaternion.Identity, Vector3.One );

		protected virtual void OnTransformChanged()
		{
			//!!!!slowly?
			SpaceBoundsUpdate();
		}

		/// <summary>
		/// Simplier form of Transform.Value.
		/// </summary>
		[Browsable( false )]
		public Transform TransformV
		{
			get { return Transform.Value; }
			set { Transform = value; }
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		//!!!!

		//!!!!!а может лучше через виртуальную функцию? измениться в любое время тогда может
		//!!!!
		SpaceBounds spaceBoundsOverride;
		static SpaceBounds spaceBoundsDefault = new SpaceBounds( null, new Sphere( Vector3.Zero, .5 ) );
		SpaceBounds spaceBounds = spaceBoundsDefault;
		////!!!!!надо ли?
		//int sceneGraphGroup;
		//internal int sceneGraphIndex = -1;

		//!!!!!bool sceneGraphEndLeaf;

		//static Component_ObjectInSpace()
		//{
		//	spaceBoundsDefault = new SpaceBoundsType();
		//	spaceBoundsDefault.boundingSphere = new Sphere( Vec3.Zero, .5 );
		//}

		//public Component_ObjectInSpace()
		//{
		//	spaceBounds = NeoAxis.SpaceBounds.Default;
		//}

		[Browsable( false )]
		public virtual SpaceBounds SpaceBoundsOverride
		{
			get { return spaceBoundsOverride; }
			set
			{
				if( spaceBoundsOverride == value )
					return;
				spaceBoundsOverride = value;
				SpaceBoundsUpdate();
			}
		}

		[Browsable( false )]
		public SpaceBounds SpaceBounds
		{
			get { return spaceBounds; }
		}

		protected virtual void OnSpaceBoundsUpdate( ref SpaceBounds newBounds ) { }

		public delegate void SpaceBoundsUpdateEventDelegate( Component_ObjectInSpace obj, ref SpaceBounds newBounds );
		public event SpaceBoundsUpdateEventDelegate SpaceBoundsUpdateEvent;

		protected virtual bool OnSpaceBoundsUpdateIncludeChildren() { return false; }

		public delegate void SpaceBoundsUpdateIncludeChildrenEventDelegate( Component_ObjectInSpace obj, ref bool include );
		public event SpaceBoundsUpdateIncludeChildrenEventDelegate SpaceBoundsUpdateIncludeChildrenEvent;

		protected virtual void OnSpaceBoundsChanged() { }
		/// <summary>Occurs when the <see cref="SpaceBounds"/> property value changes.</summary>
		public event Action<Component_ObjectInSpace> SpaceBoundsChanged;

		//!!!!

		//!!!!вызывать
		public void SpaceBoundsUpdate()
		{
			//!!!!threading. типа как подготавливать цепочку для расчета. какие тогда ограничения?
			//!!!!!!!как вариант для размышлений - делать мультипоточность выше, на уровне объектов

			//!!!!или обновлять при запросе

			var oldBounds = spaceBounds;

			//calculate
			var newBounds = spaceBoundsOverride;
			if( newBounds == null )
			{
				OnSpaceBoundsUpdate( ref newBounds );
				SpaceBoundsUpdateEvent?.Invoke( this, ref newBounds );

				//include children
				{
					var include = OnSpaceBoundsUpdateIncludeChildren();
					SpaceBoundsUpdateIncludeChildrenEvent?.Invoke( this, ref include );
					if( include )
					{
						foreach( var child in GetComponents<Component_ObjectInSpace>( onlyEnabledInHierarchy: true ) )
						{
							//touch Transform to update
							var tr = child.Transform.Value;

							newBounds = SpaceBounds.Merge( newBounds, child.SpaceBounds );
						}
					}
				}

				//!!!!тут?
				if( newBounds == null )
				{
					//!!!!от чилдов еще которые Component_ObjectInSpace?
					//!!!!!так медленее. целесообразности нет. может полезно выбирать чилдовые, тогда даже вредно.

					//!!!!так?
					Sphere sphere = new Sphere( Transform.Value.Position, 0.001 );// MathEx.Epsilon );
																				  //Sphere sphere = new Sphere( Transform.Value.Position, .5 );

					newBounds = new SpaceBounds( sphere );

					//Vec3[] points = new Bounds( -.5, -.5, -.5, .5, .5, .5 ).ToPoints();
					//var mat4 = Transform.Value.ToMat4();
					//Bounds b = Bounds.Cleared;
					//foreach( var p in points )
					//	b.Add( mat4 * p );
					//newBounds = new SpaceBounds( b );

					//newBounds = new Bounds( Position, Position );
					//newBounds.Expand( .5 );
				}
			}

			spaceBounds = newBounds;
			if( oldBounds != spaceBounds )
			{
				if( EnabledInHierarchy )
				{
					//!!!!slowly ParentScene?
					ParentScene?.ObjectsInSpace_ObjectUpdate( this );
				}

				OnSpaceBoundsChanged();
				SpaceBoundsChanged?.Invoke( this );
			}

			//!!!!что-то где-то обновлять. но когда
		}

		//!!!!
		//bool SceneGraphContainer;

		//!!!!в этом классе эвенты всякие от карты?

		//!!!!!!если интерполяцию делать на уровне объектов сцены, то тут не нужно. как бы это было?
		////TransformPreviousTick
		//Reference<Transform> transformPreviousTick = new Transform( Vec3.Zero, Quat.Identity, Vec3.One );
		//[Serialize]
		////[DefaultValue( "0 0 0" )]//!!!!!
		//public virtual Reference<Transform> TransformPreviousTick
		//{
		//	get
		//	{
		//		if( !string.IsNullOrEmpty( transformPreviousTick.ReferenceName ) )
		//			TransformPreviousTick = GetReferenceValue<Transform>( this, transformPreviousTick.ReferenceName );
		//		return transformPreviousTick;
		//	}
		//	set
		//	{
		//		if( transformPreviousTick == value ) return;
		//		transformPreviousTick = value;
		//		TransformPreviousTickChanged?.Invoke( this );
		//	}
		//}
		//public event Action<Component_ObjectInScene> TransformPreviousTickChanged;

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		//!!!!или одним свойством? сложнее обращаться, как бы два уровня. хотя можно выдавать в виде "Transform.Position"

		//!!!!!как меняется скорость. bool Gravity. как проверять летаемость. ray cast или нет

		//!!!!!
		////Velocity
		//Reference<Vec3> velocityLinear = Vec3.Zero;
		//[Serialize]
		//[DefaultValue( "0 0 0" )]
		//public virtual Reference<Vec3> VelocityLinear
		//{
		//	get
		//	{
		//		if( !string.IsNullOrEmpty( velocityLinear.GetByReference ) )
		//			VelocityLinear = velocityLinear.GetValue( this );
		//		return velocityLinear;
		//	}
		//	set
		//	{
		//		if( velocityLinear == value ) return;
		//		velocityLinear = value;
		//		VelocityLinearChanged?.Invoke( this );
		//	}
		//}
		//public event Action<Component_ObjectInSpace> VelocityLinearChanged;


		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		////!!!!
		//[Browsable( false )]
		//public Component_ObjectInSpace ParentObjectInSpace
		//{
		//	get
		//	{
		//		return Parent as Component_ObjectInSpace;
		//	}
		//}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		protected override void OnEnabledInHierarchyChanged()
		{
			base.OnEnabledInHierarchyChanged();

			if( EnabledInHierarchy )
				parentSceneCached = ParentScene;
			else
				parentSceneCached = null;

			//!!!!так?
			if( EnabledInHierarchy )
				SpaceBoundsUpdate();

			//!!!!тут?
			if( EnabledInHierarchy )
				ParentScene?.ObjectsInSpace_ObjectUpdate( this );
			else
			{
				ParentScene?.ObjectsInSpace_ObjectRemove( this );
				sceneOctreeIndex = -1;
			}
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		protected virtual bool OnEnabledSelectionByCursor()
		{
			return true;
		}

		[Browsable( false )]
		public bool EnabledSelectionByCursor
		{
			get
			{
				if( EnabledInHierarchy && VisibleInHierarchy && CanBeSelectedInHierarchy && ParentScene != null && OnEnabledSelectionByCursor() )
					return true;
				return false;
			}
		}

		//!!!!name
		/// <summary>
		/// Provides data for <see cref="CheckSelectionByRay(CheckSelectionByRayContext)"/> method.
		/// </summary>
		public class CheckSelectionByRayContext
		{
			//!!!!public

			public Viewport viewport;
			public Vector2 screenPosition;
			public Ray ray;

			//!!!!
			public bool thisObjectWasChecked;
			public double thisObjectResultRayScale;
			//!!!!screen label?
		}

		//!!!!name
		protected virtual void OnCheckSelectionByRay( CheckSelectionByRayContext context ) { }

		public void CheckSelectionByRay( CheckSelectionByRayContext context )
		{
			OnCheckSelectionByRay( context );
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		//!!!!new
		//!!!!тут?
		//!!!!так?
		/// <summary>
		/// Object to control the process of displaying objects in the scene.
		/// </summary>
		public class RenderingContext
		{
			public Viewport viewport;

			//!!!!maybe only Component? типа high level. кому надо тот делает компоненту
			public ESet<object> selectedObjects = new ESet<object>();
			public ESet<object> canSelectObjects = new ESet<object>();
			public Component objectToCreate;


			public int displayObjectInSpaceBoundsMax = 1000;
			//public int displayObjectInSpaceBoundsCounter;

			public int displayBillboardsMax = 1000;
			public int displayBillboardsCounter;

			public int displayLabelsMax = 1000;
			public int displayLabelsCounter;

			public int displayLightsMax = 70;
			public int displayLightsCounter;

			public int displayDecalsMax = 200;
			public int displayDecalsCounter;

			public int displayReflectionProbesMax = 70;
			public int displayReflectionProbesCounter;

			public int displayCamerasMax = 70;
			public int displayCamerasCounter;

			public int displaySensorsMax = 200;
			public int displaySensorsCounter;

			public int displayPhysicalObjectsMaxCount = 500;
			public int displayPhysicalObjectsMaxVertices = 100000;
			//public int displayPhysicalObjectsCounter;

			public int displaySoundSourcesMax = 70;
			public int displaySoundSourcesCounter;


			//!!!!может: class ThisObjecData. переменной это всё

			//!!!!так?
			public bool disableShowingLabelForThisObject;
			//public bool thisObjectWasDisplayed;
			//public bool disableShowingThisObject;


			//!!!!maybe only Component? типа high level. кому надо тот делает компоненту
			//public Ray? thisObjectRaySelectionDetalization_Ray;
			//public double thisObjectRaySelectionDetalization_RayScaleResult;
			////ray selection detalization
			//public class RaySelectionDetalizationValue
			//{
			//	public double rayScale;
			//}
			//public Ray? raySelectionDetalization_Ray;
			//public EDictionary<object, RaySelectionDetalizationValue> raySelectionDetalization_Objects;

			//

			public RenderingContext( Viewport viewport )
			{
				this.viewport = viewport;
			}
		}

		void AddObjectScreenLabel( ViewportRenderingContext context )//, out Rectangle labelScreenRectangle )
		{
			var context2 = context.objectInSpaceRenderingContext;
			var viewport = context2.viewport;
			var t = Transform.Value;
			var pos = t.Position;

			if( viewport.CameraSettings.ProjectToScreenCoordinates( pos, out Vector2 screenPosition ) )
			{
				if( new Rectangle( 0, 0, 1, 1 ).Contains( ref screenPosition ) )
				{
					var settings = ProjectSettings.Get;
					var maxSize = settings.ScreenLabelMaxSize.Value;
					var minSize = settings.ScreenLabelMinSizeFactor.Value * maxSize;
					var maxDistance = settings.ScreenLabelMaxDistance.Value;

					double distance = ( pos - viewport.CameraSettings.Position ).Length();
					if( distance < maxDistance )
					{
						Vector2 sizeInPixels = Vector2.Lerp( new Vector2( maxSize, maxSize ), new Vector2( minSize, minSize ), distance / maxDistance );
						Vector2 screenSize = sizeInPixels / viewport.SizeInPixels.ToVector2();

						var rect = new Rectangle( screenPosition - screenSize * .5, screenPosition + screenSize * .5 ).ToRectangleF();

						ColorValue color;
						if( context2.selectedObjects.Contains( this ) )
							color = ProjectSettings.Get.SelectedColor;
						else if( context2.canSelectObjects.Contains( this ) )
							color = ProjectSettings.Get.CanSelectColor;
						else
							color = ProjectSettings.Get.ScreenLabelColor;

						context2.displayLabelsCounter++;

						var item = new Viewport.LastFrameScreenLabelItem();
						item.Object = this;
						item.DistanceToCamera = (float)( Transform.Value.Position - context.Owner.CameraSettings.Position ).Length();
						item.ScreenRectangle = rect;
						item.Color = color;

						//remove display in corner
						if( viewport.LastFrameScreenLabelByObjectInSpace.ContainsKey( this ) )
						{
							foreach( var item2 in viewport.LastFrameScreenLabels )
							{
								if( item2.Object == this )
								{
									viewport.LastFrameScreenLabels.Remove( item2 );
									break;
								}
							}
						}

						viewport.LastFrameScreenLabels.AddLast( item );
						viewport.LastFrameScreenLabelByObjectInSpace[ this ] = item;

						//var texture = ResourceManager.LoadResource<Component_Image>( "Base\\UI\\Images\\Circle.png" );
						//viewport.CanvasRenderer.AddQuad( rect, new RectangleF( 0, 0, 1, 1 ), texture, color, true );
						////viewport.CanvasRenderer.AddFillEllipse( rect, 32, color );
						////viewport.CanvasRenderer.AddText( "FS", rect.GetCenter(), EHorizontalAlignment.Center, EVerticalAlignment.Center, new ColorValue( 0.8, 0.8, 0.8 ) );
						////GuiRenderer.AddQuad( rect, new ColorValue( 1, 1, 0 ) );

						//return true;
					}
				}
			}

			//labelScreenRectangle = Rectangle.Zero;
			//return false;
		}

		//protected virtual void OnRender( RenderingContext context )
		//{
		//	//!!!!может порядок рисовани другой. например, сначала включенную физику, уже потом bounds

		//	////!!!!за экраном не нужно. может уже в контексте какие-то данные
		//	//if( ParentScene.GetDisplayDevelopmentDataInThisApplication() && ParentScene.DisplayObjectInSpaceBounds && context.viewport.Simple3DRenderer != null )
		//	//{
		//	//	ColorValue color = ProjectSettings.Get.SceneShowObjectInSpaceBoundsColor;
		//	//	context.viewport.Simple3DRenderer.SetColor( color, color * ProjectSettings.Get.HiddenByOtherObjectsColorMultiplier );
		//	//	context.viewport.Simple3DRenderer.AddBounds( SpaceBounds.CalculatedBoundingBox );
		//	//}
		//}

		//public delegate void RenderEventDelegate( Component_ObjectInSpace sender, RenderingContext context );
		//public event RenderEventDelegate RenderEvent;

		//internal void PerformRender( RenderingContext context )
		//{
		//	OnRender( context );
		//	RenderEvent?.Invoke( this, context );
		//}

		/////////////////////////////////////////

		[Browsable( false )]
		public Component_Scene ParentScene
		{
			get
			{
				if( parentSceneCached != null )
					return parentSceneCached;
				return FindParent<Component_Scene>();
			}
		}

		/////////////////////////////////////////

		internal int _internalRenderSceneIndex = -1;

		public virtual void OnGetRenderSceneData( ViewportRenderingContext context, GetRenderSceneDataMode mode, Component_Scene.GetObjectsInSpaceItem modeGetObjectsItem ) { }

		public delegate void GetRenderSceneDataDelegate( Component_ObjectInSpace sender, ViewportRenderingContext context, GetRenderSceneDataMode mode, Component_Scene.GetObjectsInSpaceItem modeGetObjectsItem );
		public event GetRenderSceneDataDelegate GetRenderSceneDataBefore;
		public event GetRenderSceneDataDelegate GetRenderSceneData;

		internal void PerformGetRenderSceneData( ViewportRenderingContext context, GetRenderSceneDataMode mode, Component_Scene.GetObjectsInSpaceItem modeGetObjectsItem )
		{
			//reset this object parameters
			context.objectInSpaceRenderingContext.disableShowingLabelForThisObject = false;

			GetRenderSceneDataBefore?.Invoke( this, context, mode, modeGetObjectsItem );
			OnGetRenderSceneData( context, mode, modeGetObjectsItem );
			GetRenderSceneData?.Invoke( this, context, mode, modeGetObjectsItem );

			//object space bounds, render screen label if not displayed
			if( mode == GetRenderSceneDataMode.InsideFrustum )
			{
				var context2 = context.objectInSpaceRenderingContext;
				var viewport = context.Owner;
				var scene = viewport.AttachedScene;
				if( scene != null && scene.GetDisplayDevelopmentDataInThisApplication() )
				{
					////object scene bounds
					//if( ParentScene.DisplayObjectInSpaceBounds && viewport.Simple3DRenderer != null && context2.displayObjectInSpaceBoundsCounter < context2.displayObjectInSpaceBoundsMax )
					//{
					//	context2.displayObjectInSpaceBoundsCounter++;

					//	ColorValue color = ProjectSettings.Get.SceneShowObjectInSpaceBoundsColor;
					//	viewport.Simple3DRenderer.SetColor( color, color * ProjectSettings.Get.HiddenByOtherObjectsColorMultiplier );

					//	var bounds = SpaceBounds.CalculatedBoundingBox;

					//	double lineThickness = 0;
					//	//precalculate line thickness
					//	if( bounds.GetSize().MaxComponent() < 10 )
					//		lineThickness = viewport.Simple3DRenderer.GetThicknessByPixelSize( bounds.GetCenter(), ProjectSettings.Get.LineThickness );

					//	viewport.Simple3DRenderer.AddBounds( bounds, false, lineThickness );
					//}

					//label
					var display = ScreenLabel.Value;

					if( scene.DisplayLabels && EnabledSelectionByCursor && ( !context2.disableShowingLabelForThisObject || display == ScreenLabelEnum.AlwaysDisplay ) && display != ScreenLabelEnum.NeverDisplay && viewport.AllowRenderScreenLabels && context2.viewport.CanvasRenderer != null && context2.displayLabelsCounter < context2.displayLabelsMax )
					{
						AddObjectScreenLabel( context );

						//if( DrawObjectScreenLabel( context2, out Rectangle labelScreenRectangle ) )
						//{
						//	context2.displayLabelsCounter++;

						//	xx xx;

						//	var item = new Viewport.LastFrameScreenLabelItem();
						//	item.Obj = this;
						//	item.ScreenRectangle = labelScreenRectangle;

						//	item.Color = xxx;

						//	viewport.LastFrameScreenLabels.Add( item );
						//}
					}
				}
			}
		}

		/////////////////////////////////////////

		public void SetPosition( Vector3 value )
		{
			var objectToTransform = Component_ObjectInSpace_Utility.CalculateObjectToTransform( this );
			if( objectToTransform == null )
				objectToTransform = this;

			var tr = objectToTransform.TransformV;
			objectToTransform.Transform = new Transform( value, tr.Rotation, tr.Scale );
		}

		public void SetPosition( double x, double y, double z )
		{
			SetPosition( new Vector3( x, y, z ) );
		}

		public void SetRotation( Quaternion value )
		{
			var objectToTransform = Component_ObjectInSpace_Utility.CalculateObjectToTransform( this );
			if( objectToTransform == null )
				objectToTransform = this;

			var tr = objectToTransform.TransformV;
			objectToTransform.Transform = new Transform( tr.Position, value, tr.Scale );
		}

		public void SetRotation( Angles value )
		{
			SetRotation( value.ToQuaternion() );
		}

		public void SetRotation( double roll, double pitch, double yaw )
		{
			SetRotation( new Angles( roll, pitch, yaw ).ToQuaternion() );
		}

		public void SetScale( Vector3 value )
		{
			var objectToTransform = Component_ObjectInSpace_Utility.CalculateObjectToTransform( this );
			if( objectToTransform == null )
				objectToTransform = this;

			var tr = objectToTransform.TransformV;
			objectToTransform.Transform = new Transform( tr.Position, tr.Rotation, value );
		}

		public void SetScale( double x, double y, double z )
		{
			SetScale( new Vector3( x, y, z ) );
		}

		public void SetScale( double value )
		{
			SetScale( new Vector3( value, value, value ) );
		}

		public void LookAt( Vector3 target, Vector3 up )
		{
			var direction = target - TransformV.Position;
			if( direction == Vector3.Zero )
				direction = Vector3.XAxis;
			direction.Normalize();
			SetRotation( Quaternion.LookAt( direction, up ) );
		}

		public void LookAt( Component_ObjectInSpace target, Vector3 up )
		{
			LookAt( target.TransformV.Position, up );
		}

		public void LookAt( Vector3 target )
		{
			LookAt( target, Vector3.ZAxis );
		}

		public void LookAt( Component_ObjectInSpace target )
		{
			LookAt( target, Vector3.ZAxis );
		}
	}
}
