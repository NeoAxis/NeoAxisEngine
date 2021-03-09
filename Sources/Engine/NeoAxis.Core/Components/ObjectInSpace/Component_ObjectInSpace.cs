// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
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

	////[Editor( typeof( Editor.SelectMapObjectUITypeEditor ), typeof( UITypeEditor ) )]//!!!!
	//public class Component_MapObject : Component
	//{
	//	//internal Map ParentMap;//!!!!!attachedToMap? или в SceneObject переименовать?
	//	////!!!!!
	//	//internal double createTime;

	//	//!!!!пока без uin
	//	//internal uint uin;
	//	//internal uint networkUIN;

	//	//!!!!!было
	//	//internal bool created;
	//	//internal bool mustDestroy;
	//	//internal bool destroyed;

	//	//!!!!
	//	////Value - count of subscriptions
	//	////!!!!!!странная всё же хрень. будет жеж struct ObjectReference
	//	//internal Dictionary<MapObject, int> subscriptionsToDeletionEvent;

	//	////for Map.MapObjects
	//	//internal LinkedListNode<MapObject> allObjectsLinkListNode;
	//	//internal LinkedListNode<MapObject> objectsDeletionQueueLinkListNode;

	//	//!!!!!было
	//	//через ESet?
	//	//for Tick events
	//	//LinkedListNode<MapObject> objectsTimerLinkListNode;
	//	//int subscriptionToTickEventCounter;

	//	//!!!!!в Component? rename? или еще куда запихать?
	//	//можно по идее общий метод для редактора?
	//	//!!!!оно ведь может еще рассчиываться. тогда методом еще
	//	bool editorSelectable = true;

	//	string layerName = "";
	//	//Map.LayerClass layer;

	//	//!!!!!!компонентой?
	//	//!!!!!оно тут не в тему. слишком специфично
	//	//[FieldSerialize( "remainingLifetime" )]
	//	//[DefaultValue( 0.0f )]
	//	//internal double remainingLifetime;
	//	//bool remainingLifetime_subscribedToTick;

	//	//Transform
	//	xx xx;
	//	xx xx;//одним параметром. одним изменением.
	//	bool transformEnabled = true;
	//	Vec3 position;
	//	Quat rotation = Quat.Identity;
	//	Vec3 scale = new Vec3( 1, 1, 1 );

	//	//!!!!!!
	//	//bool transformAllowCallOnSetTransformAndUpdatePhysicsModel;

	//	//Transform interpolation
	//	Vec3 oldPosition;
	//	Quat oldRotation = Quat.Identity;
	//	Vec3 oldScale = new Vec3( 1, 1, 1 );
	//	//!!!!check
	//	bool oldTransformIsDifferent;
	//	double oldTransformUpdateTime;

	//	//Scene graph
	//	Bounds worldBounds;
	//	//!!!!!надо ли?
	//	int sceneGraphGroup;
	//	internal int sceneGraphIndex = -1;

	//	////Physics model
	//	//[FieldSerialize( "physicsModelName" )]
	//	//string physicsModelName = "";
	//	//PhysicsModel physicsModelInstance;
	//	//bool physicsModelManualControlPushedToWorldProperty;

	//	//!!!!!иначе?
	//	bool editorTransformModifyActivated;

	//	//!!!!!
	//	//internal CreationTypes creationType = CreationTypes.Creation;

	//	//!!!!!!через LoadingContext
	//	//internal TextBlock lastLoadingTextBlock;

	//	//!!!!было в новом
	//	//bool canEnabled;

	//	//!!!!!!? компонентой?
	//	////Auto alignment
	//	//public enum AutoVerticalAlignments
	//	//{
	//	//	None,
	//	//	ByBounds,
	//	//	ByBoundsWithRotation,
	//	//	ByCenter,
	//	//	//!!!!!!specified by developer?
	//	//}
	//	//[FieldSerialize( "autoVerticalAlignment" )]
	//	//[DefaultValue( AutoVerticalAlignments.None )]
	//	//AutoVerticalAlignments autoVerticalAlignment = AutoVerticalAlignments.None;

	//	//!!!!!!
	//	//this object is attached to another object by means MapObjectComponent_AttachObject
	//	//internal MapObject thisObjectIsAttachedTo_Object;
	//	//internal Joint thisObjectIsAttachedTo_ObjectJoint;

	//	//!!!!!!
	//	//static MapObjectAttachedMesh.MeshBoneSlot[] emptyBoneSlots = new MapObjectAttachedMesh.MeshBoneSlot[ 0 ];
	//	//internal Dictionary<string, MapObjectAttachedMesh.MeshBoneSlot> boneSlots;

	//	//!!!!!нужно new SDK
	//	//internal uint[] client_attachedMapObjectsInfo;

	//	//!!!!!
	//	//static RemoteEntityWorld[] tempRemoteEntityWorldArray1 = new RemoteEntityWorld[ 1 ];


	//	///////////////////////////////////////////

	//	//!!!!!
	//	//public enum CreationTypes
	//	//{
	//	//	Creation,
	//	//	Recreation,
	//	//	Loading,
	//	//}

	//	///////////////////////////////////////////

	//	//!!!!!
	//	//enum NetworkMessages
	//	//{
	//	//	PostCreateToClient,
	//	//	MustDestroyToClient,
	//	//}

	//	///////////////////////////////////////////

	//	//!!!!!
	//	//public enum NetworkDirections
	//	//{
	//	//	ToClient,
	//	//	ToServer,
	//	//}

	//	///////////////////////////////////////////

	//	//!!!!!
	//	//[AttributeUsage( AttributeTargets.Method )]
	//	//public sealed class NetworkReceiveAttribute : Attribute
	//	//{
	//	//	NetworkDirections direction;
	//	//	ushort messageIdentifier;

	//	//	public NetworkReceiveAttribute( NetworkDirections direction, ushort messageIdentifier )
	//	//	{
	//	//		this.direction = direction;
	//	//		this.messageIdentifier = messageIdentifier;
	//	//	}

	//	//	public NetworkDirections Direction
	//	//	{
	//	//		get { return direction; }
	//	//	}

	//	//	public ushort MessageIdentifier
	//	//	{
	//	//		get { return messageIdentifier; }
	//	//	}
	//	//}

	//	/////////////////////////////////////////////

	//	////!!!!!тут?
	//	//public class TagInfo
	//	//{
	//	//	[FieldSerialize( "name" )]
	//	//	string name = "";
	//	//	[FieldSerialize( "value" )]
	//	//	string value = "";

	//	//	public TagInfo()
	//	//	{
	//	//	}

	//	//	public TagInfo( string name, string value )
	//	//	{
	//	//		this.name = name;
	//	//		this.value = value;
	//	//	}

	//	//	public string Name
	//	//	{
	//	//		get { return name; }
	//	//		set { name = value; }
	//	//	}

	//	//	public string Value
	//	//	{
	//	//		get { return value; }
	//	//		set { this.value = value; }
	//	//	}

	//	//	public override string ToString()
	//	//	{
	//	//		return string.Format( "{0} = {1}", name, value );
	//	//	}
	//	//}

	//	///////////////////////////////////////////

	//	//protected MapObject()
	//	//{
	//	//allObjectsLinkListNode = new LinkedListNode<MapObject>( this );
	//	//}

	//	//!!!!!
	//	////!!!!rename? а почему это не в компоненте?
	//	///// <summary>
	//	///// Gets the type of this object.
	//	///// </summary>
	//	//[Description( "The type of this object." )]
	//	//public MapObjectTypeInfo Type
	//	//{
	//	//	get { return type; }
	//	//}

	//	///// <summary>
	//	///// Gets the parent of entity.
	//	///// </summary>
	//	//[Browsable( false )]
	//	//public Entity Parent
	//	//{
	//	//   get { return parent; }
	//	//}

	//	//!!!!!
	//	///// <summary>
	//	///// Gets internal unique identifier.
	//	///// </summary>
	//	//[Browsable( false )]
	//	//public uint UIN
	//	//{
	//	//	get { return uin; }
	//	//}

	//	//!!!!!
	//	///// <summary>
	//	///// Gets internal unique network identifier.
	//	///// </summary>
	//	//[Browsable( false )]
	//	//public uint NetworkUIN
	//	//{
	//	//	get { return networkUIN; }
	//	//}

	//	//!!!!!
	//	///// <summary>
	//	///// Gets create time.
	//	///// </summary>
	//	//[Browsable( false )]
	//	//public double CreateTime
	//	//{
	//	//	get { return createTime; }
	//	//}

	//	///// <summary>
	//	///// Puts the object to queue of the objects for deletion.
	//	///// </summary>
	//	//public void MustDestroy()
	//	//{
	//	//	if( !mustDestroy )
	//	//	{
	//	//		mustDestroy = true;

	//	//		//add to deletion queue
	//	//		if( !ParentMap.MapObjects.objectsDeletionQueue.Contains( this ) )
	//	//			ParentMap.MapObjects.objectsDeletionQueue.Add( this );

	//	//		//!!!!!
	//	//		////send network message to all clients
	//	//		//if( Type.NetworkType == EntityNetworkTypes.Synchronized && ParentMap.SimulationType_IsServer() )
	//	//		//{
	//	//		//	SendDataWriter writer = BeginNetworkMessage( typeof( MapObject ), (ushort)NetworkMessages.MustDestroyToClient );
	//	//		//	//writer.Write( false );
	//	//		//	EndNetworkMessage();
	//	//		//}
	//	//	}
	//	//}

	//	//!!!!
	//	//[NetworkReceive( NetworkDirections.ToClient, (ushort)NetworkMessages.MustDestroyToClient )]
	//	//void Client_ReceiveMustDestroyToClient( RemoteEntityWorld sender, ReceiveDataReader reader )
	//	//{
	//	//	//if( !reader.Complete() )
	//	//	//   return;
	//	//	MustDestroy();
	//	//}

	//	///// <summary>
	//	///// Gets or sets the name of the object.
	//	///// </summary>
	//	///// <remarks>
	//	///// <para>The name of the object is always unique on the map.</para>
	//	///// </remarks>
	//	//[Description( "The name of the object. The name of the object is always unique on the map." )]
	//	//public string Name
	//	//{
	//	//	get { return name; }
	//	//	set
	//	//	{
	//	//		if( string.IsNullOrEmpty( value ) )
	//	//			throw new InvalidOperationException( "The empty name is forbidden." );
	//	//		if( name == value )
	//	//			return;

	//	//		if( ParentMap != null )
	//	//		{
	//	//			MapObject obj = ParentMap.mapObjects.GetByName( value );
	//	//			//!!!!!!?
	//	//			if( obj != null && obj != this )
	//	//				throw new InvalidOperationException( $"The name \'{value}\' is already occupied by \'{obj.ToString()}\'." );
	//	//			ParentMap.mapObjects.objectByName.Remove( name );
	//	//		}

	//	//		this.name = value;

	//	//		if( ParentMap != null )
	//	//			ParentMap.mapObjects.objectByName[ name ] = this;

	//	//		NameChanged?.Invoke( this );
	//	//		AllMapObjects_NameChanged?.Invoke( this );
	//	//	}
	//	//}

	//	//!!!!!
	//	///// <summary>
	//	///// Subscribes to deletion event of another entity.
	//	///// </summary>
	//	///// <param name="entity">The other entity.</param>
	//	///// <seealso cref="UnsubscribeToDeletionEvent(MapObject)"/>
	//	///// <seealso cref="NeoAxis.MapObject.OnDeleteSubscribedToDeletionEvent(MapObject)"/>
	//	///// <seealso cref="NeoAxis.MapObject.DeleteSubscribedToDeletionEvent"/>
	//	//public void SubscribeToDeletionEvent( MapObject entity )
	//	//{
	//	//	//!!!!!погонять

	//	//	if( entity == this )
	//	//		Log.Fatal( "Entity: SubscribeToDeletionEvent: entity == this." );

	//	//	if( subscriptionsToDeletionEvent == null )
	//	//		subscriptionsToDeletionEvent = new Dictionary<MapObject, int>();
	//	//	int value;
	//	//	if( subscriptionsToDeletionEvent.TryGetValue( entity, out value ) )
	//	//		subscriptionsToDeletionEvent[ entity ] = value + 1;
	//	//	else
	//	//		subscriptionsToDeletionEvent.Add( entity, 1 );

	//	//	if( entity.subscriptionsToDeletionEvent == null )
	//	//		entity.subscriptionsToDeletionEvent = new Dictionary<MapObject, int>();
	//	//	if( entity.subscriptionsToDeletionEvent.TryGetValue( this, out value ) )
	//	//		entity.subscriptionsToDeletionEvent[ this ] = value + 1;
	//	//	else
	//	//		entity.subscriptionsToDeletionEvent.Add( this, 1 );
	//	//}

	//	//!!!!!
	//	///// <summary>
	//	///// Unsubscribes to deletion event of another entity.
	//	///// </summary>
	//	///// <param name="entity">The other entity.</param>
	//	///// <seealso cref="NeoAxis.MapObject.SubscribeToDeletionEvent(MapObject)"/>
	//	///// <seealso cref="NeoAxis.MapObject.OnDeleteSubscribedToDeletionEvent(MapObject)"/>
	//	///// <seealso cref="NeoAxis.MapObject.DeleteSubscribedToDeletionEvent"/>
	//	//public void UnsubscribeToDeletionEvent( MapObject entity )
	//	//{
	//	//	if( subscriptionsToDeletionEvent != null )
	//	//	{
	//	//		int value;
	//	//		if( subscriptionsToDeletionEvent.TryGetValue( entity, out value ) )
	//	//		{
	//	//			if( value > 1 )
	//	//				subscriptionsToDeletionEvent[ entity ] = value - 1;
	//	//			else
	//	//				subscriptionsToDeletionEvent.Remove( entity );
	//	//		}
	//	//	}

	//	//	if( entity.subscriptionsToDeletionEvent != null )
	//	//	{
	//	//		int value;
	//	//		if( entity.subscriptionsToDeletionEvent.TryGetValue( this, out value ) )
	//	//		{
	//	//			if( value > 1 )
	//	//				entity.subscriptionsToDeletionEvent[ this ] = value - 1;
	//	//			else
	//	//				entity.subscriptionsToDeletionEvent.Remove( this );
	//	//		}
	//	//	}
	//	//}

	//	//!!!!!

	//	//MapObjectDefaultEventDelegate SimulationTickEventHandlers;

	//	//public event MapObjectDefaultEventDelegate SimulationTick
	//	//{
	//	//	add
	//	//	{
	//	//		SimulationTickEventHandlers += value;
	//	//		//!!!!!!
	//	//		//SubscribeToTickEvent();
	//	//	}
	//	//	remove
	//	//	{
	//	//		//!!!!!!
	//	//		//UnsubscribeToTickEvent();
	//	//		SimulationTickEventHandlers -= value;
	//	//	}
	//	//}

	//	//internal void PerformSimulationTick()
	//	//{
	//	//	//OnTick();
	//	//	if( SimulationTickEventHandlers != null )
	//	//		SimulationTickEventHandlers( this );
	//	//}

	//	//!!!!
	//	//internal void Client_CallTick()
	//	//{
	//	//	Client_OnTick();
	//	//	//!!!!!event?
	//	//}

	//	//!!!!!
	//	//void RemainingLifetime_Tick()
	//	//{
	//	//	if( remainingLifetime != 0 )
	//	//	{
	//	//		remainingLifetime -= TickDelta;
	//	//		if( remainingLifetime < 0 )
	//	//		{
	//	//			remainingLifetime = 0;
	//	//			MustDestroy();
	//	//		}
	//	//	}
	//	//}

	//	//!!!!!!нужен код внутри
	//	void __OnTick()
	//	{
	//		//!!!!!
	//		//!!!!networking
	//		//RemainingLifetime_Tick();

	//		if( oldTransformUpdateTime != ParentMap.SimulationTickTime )
	//			OnUpdateOldTransform();

	//		//!!!!!
	//		//UpdatePositionAndRotationByPhysics( false );
	//	}

	//	//!!!!!
	//	//protected virtual void Client_OnTick()
	//	//{
	//	//	//!!!!!
	//	//	//!!!!networking
	//	//	//RemainingLifetime_Tick();

	//	//	if( oldTransformUpdateTime != ParentMap.mapObjects.TickTime )
	//	//		OnUpdateOldTransform();
	//	//}

	//	//!!!!
	//	/////// <summary>
	//	/////// Increases the counter of signing by timer events.
	//	/////// </summary>
	//	/////// <remarks>
	//	/////// <para>If the counter is not becomes equal to zero timer events will be caused.</para>
	//	/////// </remarks>
	//	/////// <seealso cref="NeoAxis.MapObject.UnsubscribeToTickEvent()"/>
	//	/////// <seealso cref="NeoAxis.MapObject.OnTick()"/>
	//	/////// <seealso cref="NeoAxis.MapObject.TickDelta"/>
	//	/////// <seealso cref="NeoAxis.MapObject.Tick"/>
	//	/////// <seealso cref="NeoAxis.MapSystemWorld.TicksPerSecond"/>
	//	/////// <seealso cref="NeoAxis.MapSystem.MapSystemWorld.Simulation"/>
	//	//public void SubscribeToTickEvent()
	//	//{
	//	//	если родитель меняется, то что?

	//	//	if( subscriptionToTickEventCounter == 0 )
	//	//	{
	//	//		if( objectsTimerLinkListNode == null )
	//	//			objectsTimerLinkListNode = new LinkedListNode<MapObject>( this );

	//	//		ParentMap.mapObjects.objectsSubscribedToTicks.AddLast( objectsTimerLinkListNode );
	//	//		//Entities.Instance.timerEntityListEntityRemoved = true;
	//	//	}

	//	//	subscriptionToTickEventCounter++;
	//	//}

	//	//!!!!!
	//	/////// <summary>
	//	/////// Reduces the counter of signing by timer events.
	//	/////// </summary>
	//	/////// <remarks>
	//	/////// <para>If the counter becomes equal to zero timer events will not be caused.</para>
	//	/////// </remarks>
	//	/////// <seealso cref="NeoAxis.MapObject.SubscribeToTickEvent()"/>
	//	/////// <seealso cref="NeoAxis.MapObject.OnTick()"/>
	//	/////// <seealso cref="NeoAxis.MapObject.TickDelta"/>
	//	/////// <seealso cref="NeoAxis.MapObject.Tick"/>
	//	/////// <seealso cref="NeoAxis.MapSystemWorld.TicksPerSecond"/>
	//	/////// <seealso cref="NeoAxis.MapSystem.MapSystemWorld.Simulation"/>
	//	//public void UnsubscribeToTickEvent()
	//	//{
	//	//	//!!!!!было
	//	//	//!!!!!!надо ли? может это вызывается из эвента дважды
	//	//	//if( subscriptionToTickEventCounter <= 0 )
	//	//	//   Log.Warning( string.Format( "Entity: UnsubscribeToTickEvent: Amount of subscribtions to the timer <= 0. Entity name: \"{0}\".", Name ) );

	//	//	subscriptionToTickEventCounter--;

	//	//	if( subscriptionToTickEventCounter <= 0 )
	//	//	{
	//	//		ParentMap.mapObjects.objectsSubscribedToTicks.Remove( objectsTimerLinkListNode );
	//	//		//Entities.Instance.timerEntityListEntityRemoved = true;
	//	//		subscriptionToTickEventCounter = 0;
	//	//	}
	//	//}

	//	//!!!!!
	//	//void RemoveAllSubscribtionsToTickEvents()
	//	//{
	//	//	if( subscriptionToTickEventCounter != 0 )
	//	//		ParentMap.mapObjects.objectsSubscribedToTicks.Remove( objectsTimerLinkListNode );
	//	//	subscriptionToTickEventCounter = 0;
	//	//}

	//	//!!!!!
	//	///// <summary>
	//	///// Represents the method that handles a <see cref="NeoAxis.MapObject.DeleteSubscribedToDeletionEvent"/> event.
	//	///// </summary>
	//	///// <param name="entity">The source of the event.</param>
	//	///// <param name="deletedEntity">The deleted entity.</param>
	//	//public delegate void DeleteSubscribedToDeletionEventDelegate( MapObject entity, MapObject deletedEntity );

	//	//!!!!!
	//	///// <summary>
	//	///// Occurs when when the entity related with this entity is deleted.
	//	///// </summary>
	//	///// <seealso cref="NeoAxis.MapObject.SubscribeToDeletionEvent(MapObject)"/>
	//	///// <seealso cref="NeoAxis.MapObject.UnsubscribeToDeletionEvent(MapObject)"/>
	//	///// <seealso cref="NeoAxis.MapObject.OnDeleteSubscribedToDeletionEvent(MapObject)"/>
	//	//public event DeleteSubscribedToDeletionEventDelegate DeleteSubscribedToDeletionEvent;

	//	//!!!!!
	//	//internal void CallDeleteSubscribedToDeletionEvent( MapObject entity )
	//	//{
	//	//	OnDeleteSubscribedToDeletionEvent( entity );
	//	//	if( DeleteSubscribedToDeletionEvent != null )
	//	//		DeleteSubscribedToDeletionEvent( this, entity );
	//	//}

	//	//!!!!!
	//	///// <summary>
	//	///// Called when the entity related with this entity is deleted.
	//	///// </summary>
	//	///// <param name="entity">The deleted entity.</param>
	//	///// <seealso cref="NeoAxis.MapObject.SubscribeToDeletionEvent(MapObject)"/>
	//	///// <seealso cref="NeoAxis.MapObject.UnsubscribeToDeletionEvent(MapObject)"/>
	//	///// <seealso cref="NeoAxis.MapObject.DeleteSubscribedToDeletionEvent"/>
	//	//protected virtual void OnDeleteSubscribedToDeletionEvent( MapObject entity )
	//	//{
	//	//	//!!!!было
	//	//	//for( int n = 0; n < components.Count; n++ )
	//	//	//components[ n ].OnDeleteSubscribedToDeletionEvent( entity );
	//	//}

	//	/// <summary>
	//	/// Called while entity type is loading.
	//	/// </summary>
	//	/// <param name="block">The text block from which data of entity will be loaded.</param>
	//	/// <param name="error"></param>
	//	/// <returns><b>true</b> if the entity successfully loaded; otherwise, <b>false</b>.</returns>
	//	protected override bool OnLoad( TextBlock block, ref string error )
	//	{
	//		if( !base.OnLoad( block, ref error ) )
	//			return false;

	//		//!!!!было. теперь в компонентах будет
	//		////attributes
	//		//{
	//		//	//!!!!!свойства тоже?
	//		//	//для UIControl тоже?

	//		//	Type classType = ( Type != null ) ? Type.ClassType : GetType();
	//		//	Map.MapObjectClassesClass.ClassInfo loopClassInfo = Map.MapObjectClassesClass.RegisterOrGetClassInfo( classType );
	//		//	while( loopClassInfo != null )
	//		//	{
	//		//		foreach( Map.MapObjectClassesClass.ClassInfo.SerializableFieldItem item in loopClassInfo.SerializableFields )
	//		//		{
	//		//			if( MapSystemWorld.IsSupportedFieldSerializationAttribute( ParentMap, item.SupportedSerializationModes ) )
	//		//			{
	//		//				string errorString2;
	//		//				if( !FieldsSerialization.LoadFieldValue( ParentMap, true, this, item.Field, block, out errorString2 ) )
	//		//				{
	//		//					errorString = $"Object \'{this}\' can't be loaded. " + errorString2;
	//		//					return false;
	//		//				}
	//		//			}
	//		//		}
	//		//		loopClassInfo = loopClassInfo.BaseClassInfo;
	//		//	}
	//		//}

	//		//!!!!
	//		////subscriptionsToDeletionEvent
	//		//if( block.IsAttributeExist( "subscriptionsToDeletionEvent" ) )
	//		//{
	//		//	//!!!!проверить
	//		//	Log.Fatal( "check subscriptionsToDeletionEvent" );

	//		//	string text = block.GetAttribute( "subscriptionsToDeletionEvent" );
	//		//	string[] strItems = text.Split( new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries );
	//		//	subscriptionsToDeletionEvent = new Dictionary<MapObject, int>( strItems.Length / 2 );
	//		//	for( int n = 0; n < strItems.Length; n += 2 )
	//		//	{
	//		//		uint uin = uint.Parse( strItems[ n ] );
	//		//		int count = int.Parse( strItems[ n + 1 ] );
	//		//		MapObject e = ParentMap.mapObjects.GetLoadingObjectBySerializedUIN( uin );
	//		//		if( e != null && count != 0 )
	//		//			subscriptionsToDeletionEvent.Add( e, count );
	//		//	}
	//		//}

	//		return true;
	//	}

	//	//!!!!!!было
	//	//protected internal virtual void OnBeforeLoadChildEntity( EntityType childType, TextBlock childBlock, ref bool needLoad )
	//	//{
	//	//   if( ParentMap.SimulationType == Map.SimulationTypes.DedicatedServer )
	//	//   {
	//	//      if( childType.NetworkType == EntityNetworkTypes.ClientOnly )
	//	//         needLoad = false;
	//	//   }
	//	//   if( ParentMap.SimulationType == Map.SimulationTypes.ClientOnly )
	//	//   {
	//	//      if( childType.NetworkType == EntityNetworkTypes.ServerOnly )
	//	//         needLoad = false;
	//	//      if( childType.NetworkType == EntityNetworkTypes.Synchronized )
	//	//         needLoad = false;
	//	//   }
	//	//}

	//	protected override bool OnSave( TextBlock block, ref bool canSave, out string error )
	//	{
	//		error = "";

	//		//!!!!!
	//		//if( Type != null )
	//		//{
	//		//	//!!!!relative paths
	//		//	Log.Fatal( "check" );
	//		//	block.SetAttribute( "type", Type.Name );
	//		//}
	//		//else
	//		//{
	//		//	//!!!!
	//		//	Log.Fatal( "check" );
	//		//	block.SetAttribute( "class", GetType().Name );
	//		//}

	//		//!!!!!
	//		//block.SetAttribute( "uin", uin.ToString() );

	//		//!!!!было
	//		//block.SetAttribute( "classPrompt", Type.ClassInfo.entityClassType.Name );

	//		//if( layer != null && layer != ParentMap.RootLayer )
	//		//	block.SetAttribute( "layer", layer.GetPath() );

	//		if( !base.OnSave( block, ref canSave, out error ) )
	//			return false;

	//		//!!!!!
	//		////subscriptionsToDeletionEvent
	//		//if( subscriptionsToDeletionEvent != null && subscriptionsToDeletionEvent.Count != 0 )
	//		//{
	//		//	StringBuilder text = new StringBuilder();
	//		//	foreach( KeyValuePair<MapObject, int> pair in subscriptionsToDeletionEvent )
	//		//	{
	//		//		if( text.Length != 0 )
	//		//			text.Append( " " );
	//		//		text.Append( pair.Key.UIN.ToString() );
	//		//		text.Append( " " );
	//		//		text.Append( pair.Value.ToString() );
	//		//	}
	//		//	block.SetAttribute( "subscriptionsToDeletionEvent", text.ToString() );
	//		//}

	//		//!!!!!
	//		////attributes
	//		//{
	//		//	Type classType = ( Type != null ) ? Type.ClassType : GetType();
	//		//	Map.MapObjectClassesClass.ClassInfo loopClassInfo = Map.MapObjectClassesClass.RegisterOrGetClassInfo( classType );
	//		//	while( loopClassInfo != null )
	//		//	{
	//		//		foreach( Map.MapObjectClassesClass.ClassInfo.SerializableFieldItem item in loopClassInfo.SerializableFields )
	//		//		{
	//		//			if( MapSystemWorld.IsSupportedFieldSerializationAttribute( ParentMap, item.SupportedSerializationModes ) )
	//		//			{
	//		//				object defaultValue = null;
	//		//				{
	//		//					DefaultValueAttribute[] attributes = (DefaultValueAttribute[])item.Field.GetCustomAttributes(
	//		//						typeof( DefaultValueAttribute ), true );
	//		//					if( attributes.Length != 0 )
	//		//						defaultValue = attributes[ 0 ].Value;
	//		//				}

	//		//				string errorString2;
	//		//				if( !FieldsSerialization.SaveFieldValue( ParentMap, true, this, item.Field, block, defaultValue, out errorString2 ) )
	//		//				{
	//		//					errorString = $"Object \'{this}\' can't be saved. " + errorString2;
	//		//					return false;
	//		//				}
	//		//			}
	//		//		}
	//		//		loopClassInfo = loopClassInfo.BaseClassInfo;
	//		//	}
	//		//}

	//		//!!!!!
	//		//!!!!только ли после симуляции? может в редакторе два стейта, исходный и какой-то? да ну бред какой-то
	//		////physics model data
	//		//if( ParentMap.SimulationStatus == Map.SimulationStatuses.AlreadySimulated )
	//		//{
	//		//	if( PhysicsModelInstance != null && PhysicsModelInstance.ModelDeclaration != null )
	//		//		PhysicsModel.SaveWorldSerialization( block );
	//		//}

	//		return true;
	//	}

	//	/// <summary>
	//	/// Returns a string containing the type name and name of the entity.
	//	/// </summary>
	//	/// <returns>A string containing the type class name and name of the entity.</returns>
	//	public override string ToString()
	//	{
	//		//!!!!!!!
	//		return base.ToString();
	//		//return string.Format( "{0} ({1})", name, ( Type != null ) ? Type.Name : GetType().Name );
	//	}

	//	///// <summary>
	//	///// Receives constant time inverted game fps.
	//	///// </summary>
	//	///// <remarks>
	//	///// <para>This value is constant during simulation.</para>
	//	///// <para>This value is equal 1 / <see cref="MapSystemWorld.SimulationTicksPerSecond"/>.</para>
	//	///// </remarks>
	//	///// <seealso cref="SimulationTick"/>
	//	///// <seealso cref="MapSystemWorld.SimulationTicksPerSecond"/>
	//	//[Browsable( false )]
	//	//public static double SimulationTickDelta
	//	//{
	//	//	get { return simulationTickDelta; }
	//	//}

	//	//internal bool IsAllowSaveRecursive()
	//	//{
	//	//	MapObject entity = this;
	//	//	while( entity != null )
	//	//	{
	//	//		if( !entity.CanSave )
	//	//			return false;
	//	//		entity = entity.Parent;
	//	//	}
	//	//	return true;
	//	//}

	//	/// <summary>
	//	/// Gets or sets a value indicating whether ability to select entity in the editor.
	//	/// </summary>
	//	[Browsable( false )]
	//	public virtual bool EditorSelectable
	//	{
	//		get { return editorSelectable; }
	//		set { editorSelectable = value; }
	//	}

	//	//!!!!!было Clone

	//	//void CopyBrowsableProperties( MapObject source )
	//	//{
	//	//	PropertyInfo[] properties = source.GetType().GetProperties();
	//	//	foreach( PropertyInfo property in properties )
	//	//	{
	//	//		if( !property.CanWrite )
	//	//			continue;

	//	//		BrowsableAttribute[] browsableAttributes = (BrowsableAttribute[])property.
	//	//			GetCustomAttributes( typeof( BrowsableAttribute ), true );
	//	//		if( browsableAttributes.Length != 0 )
	//	//		{
	//	//			bool browsable = true;
	//	//			foreach( BrowsableAttribute browsableAttribute in browsableAttributes )
	//	//			{
	//	//				if( !browsableAttribute.Browsable )
	//	//				{
	//	//					browsable = false;
	//	//					break;
	//	//				}
	//	//			}
	//	//			if( !browsable )
	//	//				continue;
	//	//		}

	//	//		if( property.Name == "Name" )
	//	//			continue;

	//	//		
	//	//		в типе этого нет
	//	//		if( property.Name == "Components" )
	//	//			continue;

	//	//		object value = property.GetValue( source, null );
	//	//		property.SetValue( this, value, null );
	//	//	}
	//	//}

	//	//public delegate void CloneEventDelegate( MapObject cloned, bool copyBrowsableProperties, bool cloneComponents, bool cloneChildren,
	//	//	bool callPostCreate, bool callPostCreateForChildren );
	//	//public event CloneEventDelegate CloneEvent;

	//	///// <summary>
	//	///// Called when the entity is cloned.
	//	///// </summary>
	//	///// <param name="source">Entity with source data.</param>
	//	//protected virtual void OnClone( MapObject cloned, bool copyBrowsableProperties, bool cloneComponents, bool cloneChildren, bool callPostCreate,
	//	//	bool callPostCreateForChildren )
	//	//{ }

	//	///// <summary>
	//	///// Creates a new entity that is a copy of the current instance. Copying only browsable properties.
	//	///// </summary>
	//	///// <param name="cloneChildren">Whether to clone children of entity.</param>
	//	///// <param name="parent">The parameter sets the parent for the cloned entity.</param>
	//	///// <param name="callPostCreate"></param>
	//	///// <returns>A new entity that is a copy of this instance.</returns>
	//	//public MapObject Clone( bool copyBrowsableProperties, bool cloneComponents, bool cloneChildren, bool callPostCreate,
	//	//	bool callPostCreateForChildren )
	//	//{
	//	//	MapObject cloned = parent.ParentMap.entities.Create( Type );

	//	//	if( copyBrowsableProperties )
	//	//		cloned.CopyBrowsableProperties( this );
	//	//	//!!!!!в типе нет
	//	//	if( cloneComponents )
	//	//		cloned.Components_CloneAll( this );

	//	//	if( cloneChildren )
	//	//	{
	//	//		foreach( MapObject child in Children )
	//	//			child.Clone( cloned, copyBrowsableProperties, cloneComponents, cloneChildren, callPostCreateForChildren, callPostCreateForChildren );
	//	//	}

	//	//	OnClone( cloned, copyBrowsableProperties, cloneComponents, cloneChildren, callPostCreate, callPostCreateForChildren );
	//	//	if( CloneEvent != null )
	//	//		CloneEvent( cloned, copyBrowsableProperties, cloneComponents, cloneChildren, callPostCreate, callPostCreateForChildren );

	//	//	if( callPostCreate )
	//	//		cloned.PostCreate();

	//	//	return cloned;
	//	//}

	//	//было, но не надо
	//	//public void _CallOnClone( Entity source )
	//	//{
	//	//   OnClone( source );
	//	//}

	//	//!!!!в Component?
	//	//!!!!!непонятный метод? может больше инфы выдавать, типа как: GetInfoForEditor
	//	//!!!!!эвент
	//	/// <summary>
	//	/// Receives the information which will be shown in NeoAxis Engine when entity is selected.
	//	/// </summary>
	//	/// <returns>Returns information string.</returns>
	//	public virtual void Editor_GetInformationToShowOnScreen( List<Pair<string, ColorValue>> lines ) { }

	//	//!!!!!!
	//	//protected SendDataWriter BeginNetworkMessage( IList<RemoteEntityWorld> toRemoteEntityWorlds, Type messageEntityClassType, ushort messageIdentifier )
	//	//{
	//	//if( !typeof( Entity ).IsAssignableFrom( messageEntityClassType ) )
	//	//   Log.Fatal( "Entity: BeginNetworkMessage: \"messageEntityClassType\" is not entity class." );

	//	//if( Type.NetworkType != EntityNetworkTypes.Synchronized )
	//	//{
	//	//   Log.Fatal( "Entity: BeginNetworkMessage: This entity \"{0}\" is not support network " +
	//	//      "synchronization. (Type.NetworkType != ClientServerSynchronized).", ToString() );
	//	//}

	//	//if( NetworkUIN == 0 )
	//	//{
	//	//   Log.Fatal( "Entity: BeginNetworkMessage: This entity \"{0}\" has a not " +
	//	//      "initialized NetworkUIN.", ToString() );
	//	//}

	//	//if( !EntitySystemWorld.Instance.IsServer() && !EntitySystemWorld.Instance.IsClientOnly() )
	//	//{
	//	//   Log.Fatal( "Entity: BeginNetworkMessage: This Entity System world is not support " +
	//	//      "networking. (WorldSimulationType = \"{0}\".",
	//	//      EntitySystemWorld.Instance.WorldSimulationType );
	//	//}

	//	//if( EntitySystemWorld.Instance.networkingInterface == null )
	//	//   Log.Fatal( "Entity: BeginNetworkMessage: Networking inteface is not initialized." );

	//	//return EntitySystemWorld.Instance.networkingInterface.BeginEntitySynchronizationMessage(
	//	//   toRemoteEntityWorlds, this, messageEntityClassType, messageIdentifier );
	//	//}

	//	//!!!!!!
	//	//protected SendDataWriter BeginNetworkMessage( RemoteEntityWorld toRemoteEntityWorld, Type messageEntityClassType, ushort messageIdentifier )
	//	//{
	//	//	if( toRemoteEntityWorld == null )
	//	//		Log.Fatal( "Entity: BeginNetworkMessage: toRemoteEntityWorld == null." );

	//	//	tempRemoteEntityWorldArray1[ 0 ] = toRemoteEntityWorld;
	//	//	SendDataWriter writer = BeginNetworkMessage( tempRemoteEntityWorldArray1, messageEntityClassType, messageIdentifier );
	//	//	tempRemoteEntityWorldArray1[ 0 ] = null;
	//	//	return writer;
	//	//}

	//	//!!!!!!
	//	////!!!!!префиксы Networking_?
	//	//protected SendDataWriter BeginNetworkMessage( Type messageEntityClassType, ushort messageIdentifier )
	//	//{
	//	//	//!!!!!!!было
	//	//	return null;
	//	//	//return BeginNetworkMessage( EntitySystemWorld.Instance.RemoteEntityWorlds, messageEntityClassType, messageIdentifier );
	//	//}

	//	//!!!!!!
	//	//protected void EndNetworkMessage()
	//	//{
	//	//	//!!!!!!!было
	//	//	//EntitySystemWorld.Instance.networkingInterface.EndEntitySystemMessage();
	//	//}

	//	//!!!!!!
	//	//protected internal virtual void Server_OnClientConnectedBeforePostCreate( RemoteEntityWorld remoteEntityWorld )
	//	//{
	//	//}
	//	//protected internal virtual void Server_OnClientConnectedAfterPostCreate( RemoteEntityWorld remoteEntityWorld )
	//	//{
	//	//}
	//	//protected internal virtual void Server_OnClientDisconnected( RemoteEntityWorld remoteEntityWorld )
	//	//{
	//	//}

	//	//!!!!!
	//	///// <summary>
	//	///// Gets or sets the remaining time before the object will destroyed. When set to 0, the timer of deletion is disabled.
	//	///// </summary>
	//	//[DefaultValue( 0.0f )]
	//	//public double RemainingLifetime
	//	//{
	//	//	get { return remainingLifetime; }
	//	//	set
	//	//	{
	//	//		if( remainingLifetime == value )
	//	//			return;
	//	//		remainingLifetime = value;

	//	//		if( remainingLifetime > 0 && !remainingLifetime_subscribedToTick )
	//	//		{
	//	//			SubscribeToTickEvent();
	//	//			remainingLifetime_subscribedToTick = true;
	//	//		}
	//	//	}
	//	//}

	//	//!!!!!?
	//	//OnParentMapChanged(Map oldMap);

	//	[Browsable( false )]
	//	public Component_Map ParentMap
	//	{
	//		get
	//		{
	//			//!!!!!slowly?
	//			return RootParent as Component_Map;
	//		}
	//	}

	//	//!!!!!
	//	///// <summary>
	//	///// Gets or sets a value indicating whether the object is enabled.
	//	///// </summary>
	//	//[DefaultValue( true )]
	//	////[Category( "General" )]
	//	//[Description( "A value indicating whether the object is enabled." )]
	//	//public new bool Enabled
	//	//{
	//	//	get { return base.Enabled; }
	//	//	set { base.Enabled = value; }
	//	//}

	//	//!!!!!
	//	////!!!!!показывать в редакторе?
	//	///// <summary>
	//	///// Gets or sets a value indicating whether the object is visible.
	//	///// </summary>
	//	//[DefaultValue( true )]
	//	////[Category( "General" )]
	//	//[Description( "A value indicating whether the object is visible." )]
	//	//public new bool Visible
	//	//{
	//	//	get { return base.Visible; }
	//	//	set { base.Visible = value; }
	//	//}
	//	///// <summary>
	//	///// Gets or sets a value indicating whether the entity is visible.
	//	///// </summary>
	//	//[Browsable( false )]
	//	//public new bool Visible
	//	//{
	//	//	get { return visible; }
	//	//	set
	//	//	{
	//	//		if( visible == value )
	//	//			return;
	//	//		visible = value;

	//	//		CallVisibleUpdate();
	//	//	}
	//	//}

	//	///////////////////////////////////////////

	//	//!!!!!
	//	//enum NetworkMessages
	//	//{
	//	//    AttachedMapObjectsInfoToClient,
	//	//}

	//	///////////////////////////////////////////

	//	/// <summary>
	//	/// Gets or sets the position of the object in the world.
	//	/// </summary>
	//	[Description( "The world position of the object." )]
	//	[DefaultValue( typeof( Vec3 ), "0 0 0" )]
	//	[Serialize]//!!!!!!лучше обфускатор поправить. или отнаследоваться от Serializable
	//	public Vec3 Position
	//	{
	//		get { return position; }
	//		set
	//		{
	//			if( position == value )
	//				return;

	//			//!!!!!!
	//			//if( transformAllowCallOnSetTransformAndUpdatePhysicsModel )
	//			{
	//				//!!!!!!

	//				//if( PhysicsModelInstance != null )
	//				//{
	//				//	//!!!!почему джоинты нельзя с моторами перетаскивать? надо рассмотреть это
	//				//	bool containsJointsOrMotors = PhysicsModelInstance.Joints.Length != 0 || PhysicsModelInstance.Motors.Length != 0;
	//				//	if( physicsModelInstance.PushedToWorld && containsJointsOrMotors && !physicsModelManualControlPushedToWorldProperty )
	//				//		physicsModelInstance.PushedToWorld = false;

	//				//	if( PhysicsModelInstance.ModelDeclaration != null )
	//				//	{
	//				//		physicsModelInstance.ResetTransformFromModelDeclaration( ref value, ref rotation );
	//				//	}
	//				//	else
	//				//	{
	//				//		//!!!!!как тут?
	//				//		foreach( Body body in PhysicsModelInstance.Bodies )
	//				//			body.Position = value;
	//				//	}

	//				//	if( !physicsModelManualControlPushedToWorldProperty )
	//				//		physicsModelInstance.PushedToWorld = true;
	//				//}

	//				TransformUpdateProcess( ref value, ref rotation, ref scale );
	//			}
	//			//!!!!!!
	//			//else
	//			//{
	//			//	position = value;
	//			//	oldPosition = position;
	//			//	oldTransformIsDifferent = oldPosition != position || oldRotation != rotation || oldScale != scale;
	//			//}

	//			if( ParentMap.SimulationType == Component_Map.SimulationTypes.Editor )
	//				OldPosition = Position;
	//		}
	//	}

	//	/// <summary>
	//	/// Gets or sets the rotation of the object in the world.
	//	/// </summary>
	//	[Description( "The world rotation of the object." )]
	//	[DefaultValue( typeof( Quat ), "0 0 0" )]
	//	[Serialize]
	//	public Quat Rotation
	//	{
	//		get { return rotation; }
	//		set
	//		{
	//			if( rotation == value )
	//				return;

	//			//!!!!!!
	//			//if( transformAllowCallOnSetTransformAndUpdatePhysicsModel )
	//			{
	//				//!!!!

	//				//if( PhysicsModelInstance != null )
	//				//{
	//				//	bool containsJointsOrMotors = PhysicsModelInstance.Joints.Length != 0 || PhysicsModelInstance.Motors.Length != 0;
	//				//	if( PhysicsModelInstance.PushedToWorld && containsJointsOrMotors && !physicsModelManualControlPushedToWorldProperty )
	//				//		physicsModelInstance.PushedToWorld = false;

	//				//	if( PhysicsModelInstance.ModelDeclaration != null )
	//				//	{
	//				//		physicsModelInstance.ResetTransformFromModelDeclaration( ref position, ref value );
	//				//	}
	//				//	else
	//				//	{
	//				//		//!!!!!как тут?
	//				//		Body[] bodies = PhysicsModelInstance.Bodies;
	//				//		foreach( Body body in bodies )
	//				//			body.Rotation = value;
	//				//	}

	//				//	if( !physicsModelManualControlPushedToWorldProperty )
	//				//		physicsModelInstance.PushedToWorld = true;
	//				//}

	//				TransformUpdateProcess( ref position, ref value, ref scale );
	//			}
	//			//!!!!!!
	//			//else
	//			//{
	//			//	rotation = value;
	//			//	oldRotation = rotation;
	//			//	oldTransformIsDifferent = oldPosition != position || oldRotation != rotation || oldScale != scale;
	//			//}

	//			if( ParentMap.SimulationType == Component_Map.SimulationTypes.Editor )
	//				OldRotation = Rotation;
	//		}
	//	}

	//	/// <summary>
	//	/// Gets or sets the scaling of the object.
	//	/// </summary>
	//	[Description( "The scaling of the object." )]
	//	[DefaultValue( typeof( Vec3 ), "1 1 1" )]
	//	[Serialize]
	//	public Vec3 Scale
	//	{
	//		get { return scale; }
	//		set
	//		{
	//			if( scale == value )
	//				return;

	//			//!!!!!тут иначе видать, чтобы было как в Position, Rotation

	//			string reason;
	//			if( value != new Vec3( 1, 1, 1 ) && !IsAllowToChangeScale( out reason ) )
	//				return;

	//			//!!!!!!
	//			//if( transformAllowCallOnSetTransformAndUpdatePhysicsModel )
	//			{
	//				TransformUpdateProcess( ref position, ref rotation, ref value );
	//			}
	//			//!!!!!!
	//			//else
	//			//{
	//			//	scale = value;
	//			//	oldScale = scale;
	//			//	oldTransformIsDifferent = oldPosition != position || oldRotation != rotation || oldScale != scale;
	//			//}

	//			if( ParentMap.SimulationType == Component_Map.SimulationTypes.Editor )
	//				OldScale = Scale;
	//		}
	//	}

	//	/// <summary>
	//	/// Sets the transformation of the object.
	//	/// </summary>
	//	/// <remarks>
	//	/// <para>
	//	/// The same effect as set of three properties (<see cref="Position"/>, 
	//	/// <see cref="Rotation"/>, <see cref="Scale"/>) has this method.
	//	/// The difference consists what to cause one method more quickly, than to set three properties.
	//	/// </para>
	//	/// </remarks>
	//	/// <param name="pos">The object position.</param>
	//	/// <param name="rot">The object rotation.</param>
	//	/// <param name="scl">The object scale.</param>
	//	public void SetTransform( Vec3 pos, Quat rot, Vec3 scl )
	//	{
	//		SetTransform( ref pos, ref rot, ref scl );
	//	}

	//	internal void SetTransform( ref Vec3 pos, ref Quat rot, ref Vec3 scl )
	//	{
	//		//!!!!!всё свести к этому вызову? чтобы не было 4 раза код 

	//		//!!!!!
	//		//if( transformAllowCallOnSetTransformAndUpdatePhysicsModel )
	//		{
	//			//!!!!!!

	//			//if( PhysicsModelInstance != null )
	//			//{
	//			//	bool containsJointsOrMotors = PhysicsModelInstance.Joints.Length != 0 || PhysicsModelInstance.Motors.Length != 0;
	//			//	if( PhysicsModelInstance.PushedToWorld && containsJointsOrMotors && !physicsModelManualControlPushedToWorldProperty )
	//			//		PhysicsModelInstance.PushedToWorld = false;

	//			//	if( PhysicsModelInstance.ModelDeclaration != null )
	//			//	{
	//			//		physicsModelInstance.ResetTransformFromModelDeclaration( ref pos, ref rot );
	//			//	}
	//			//	else
	//			//	{
	//			//		//!!!!!как тут?
	//			//		Body[] bodies = PhysicsModelInstance.Bodies;
	//			//		foreach( Body body in bodies )
	//			//			body.SetTransform( pos, rot );
	//			//	}

	//			//	if( !physicsModelManualControlPushedToWorldProperty )
	//			//		PhysicsModelInstance.PushedToWorld = true;
	//			//}

	//			TransformUpdateProcess( ref pos, ref rot, ref scl );
	//		}
	//		//!!!!!
	//		//else
	//		//{
	//		//	position = pos;
	//		//	rotation = rot;
	//		//	scale = scl;
	//		//}

	//		//!!!!!
	//		//if( !transformAllowCallOnSetTransformAndUpdatePhysicsModel )
	//		//{
	//		//	oldPosition = position;
	//		//	oldRotation = rotation;
	//		//	oldScale = scale;
	//		//	oldTransformIsDifferent = oldPosition != position || oldRotation != rotation || oldScale != scale;
	//		//}

	//		if( ParentMap.SimulationType == Component_Map.SimulationTypes.Editor )
	//		{
	//			//new SDK:
	//			SetOldTransform( Position, Rotation, Scale );
	//			//OldPosition = Position;
	//			//OldRotation = Rotation;
	//			//OldScale = Scale;
	//		}
	//	}

	//	/// <summary>
	//	/// Called when the transformation of object durpng update.
	//	/// </summary>
	//	/// <param name="pos">New position.</param>
	//	/// <param name="rot">New rotation.</param>
	//	/// <param name="scl">New scale.</param>
	//	protected virtual void OnTransformUpdateBegin( ref Vec3 pos, ref Quat rot, ref Vec3 scl ) { }

	//	protected virtual void OnTransformUpdateEnd() { }

	//	/// <summary>
	//	/// Represents the method that handles a <see cref="TransformUpdateBegin"/> event.
	//	/// </summary>
	//	/// <param name="obj"></param>
	//	/// <param name="pos"></param>
	//	/// <param name="rot"></param>
	//	/// <param name="scl"></param>
	//	public delegate void TransformUpdateBeginDelegate( Component_MapObject obj, ref Vec3 pos, ref Quat rot, ref Vec3 scl );
	//	/// <summary>
	//	/// Occurs when the new transformation of object is calculated. The developer can change values.
	//	/// </summary>
	//	public event TransformUpdateBeginDelegate TransformUpdateBegin;

	//	/// <summary>
	//	/// Occurs when the transformation of object is changed.
	//	/// </summary>
	//	public event WorldBoundsUpdateEndDelegate TransformUpdateEnd;

	//	void TransformUpdateProcess( ref Vec3 pos, ref Quat rot, ref Vec3 scl )
	//	{
	//		//!!!!!if(EnabledInHierarchy)?

	//		OnTransformUpdateBegin( ref pos, ref rot, ref scl );
	//		TransformUpdateBegin?.Invoke( this, ref pos, ref rot, ref scl );

	//		//!!!!!тут?
	//		if( ParentMap != null && oldTransformUpdateTime != ParentMap.SimulationTickTime )
	//			OnUpdateOldTransform();

	//		position = pos;
	//		rotation = rot;
	//		scale = scl;

	//		//!!!!!
	//		////Component: OnTransformChangeUpdate
	//		//for( int n = 0; n < Components.Count; n++ )
	//		//	( Components[ n ] as Component_MapObjectChild )?.OnTransformChangeUpdate();

	//		OnTransformUpdateEnd();
	//		TransformUpdateEnd?.Invoke( this );

	//		WorldBoundsUpdate();
	//	}

	//	/// <summary>
	//	/// Gets or sets the old position of object.
	//	/// </summary>
	//	[Browsable( false )]
	//	public Vec3 OldPosition
	//	{
	//		get { return oldPosition; }
	//		set
	//		{
	//			//new: early exit
	//			if( oldPosition == value )
	//				return;
	//			oldPosition = value;
	//			oldTransformIsDifferent = oldPosition != position || oldRotation != rotation || oldScale != scale;

	//			//!!!!!
	//			//if( transformAllowCallOnSetTransformAndUpdatePhysicsModel )
	//			//{
	//			//	for( int nComponent = 0; nComponent < Components.Count; nComponent++ )
	//			//	{
	//			//		Component_MapObjectChild component = Components[ nComponent ] as Component_MapObjectChild;
	//			//		if( component != null )
	//			//			component.OnOldTransformWasChangedManually();
	//			//	}
	//			//}
	//		}
	//	}

	//	/// <summary>
	//	/// Gets or sets the old rotation of object.
	//	/// </summary>
	//	[Browsable( false )]
	//	public Quat OldRotation
	//	{
	//		get { return oldRotation; }
	//		set
	//		{
	//			//new: early exit
	//			if( oldRotation == value )
	//				return;
	//			oldRotation = value;
	//			oldTransformIsDifferent = oldPosition != position || oldRotation != rotation || oldScale != scale;

	//			//!!!!!
	//			//if( transformAllowCallOnSetTransformAndUpdatePhysicsModel )
	//			//{
	//			//	for( int nComponent = 0; nComponent < Components.Count; nComponent++ )
	//			//	{
	//			//		Component_MapObjectChild component = Components[ nComponent ] as Component_MapObjectChild;
	//			//		if( component != null )
	//			//			component.OnOldTransformWasChangedManually();
	//			//	}
	//			//}
	//		}
	//	}

	//	/// <summary>
	//	/// Gets or sets the old scale of object.
	//	/// </summary>
	//	[Browsable( false )]
	//	public Vec3 OldScale
	//	{
	//		get { return oldScale; }
	//		set
	//		{
	//			//new: early exit
	//			if( oldScale == value )
	//				return;
	//			oldScale = value;
	//			oldTransformIsDifferent = oldPosition != position || oldRotation != rotation || oldScale != scale;

	//			//!!!!!
	//			//if( transformAllowCallOnSetTransformAndUpdatePhysicsModel )
	//			//{
	//			//	for( int nComponent = 0; nComponent < Components.Count; nComponent++ )
	//			//	{
	//			//		Component_MapObjectChild component = Components[ nComponent ] as Component_MapObjectChild;
	//			//		if( component != null )
	//			//			component.OnOldTransformWasChangedManually();
	//			//	}
	//			//}
	//		}
	//	}

	//	[Browsable( false )]
	//	public bool OldTransformIsDifferent
	//	{
	//		get { return oldTransformIsDifferent; }
	//	}

	//	//!!!!!!
	//	internal void SetOldTransform( ref Vec3 pos, ref Quat rot, ref Vec3 scl )
	//	{
	//		//new: early exit
	//		if( pos == oldPosition && rot == oldRotation && scl == oldScale )
	//			return;
	//		oldPosition = pos;
	//		oldRotation = rot;
	//		oldScale = scl;
	//		oldTransformIsDifferent = oldPosition != position || oldRotation != rotation || oldScale != scale;

	//		//!!!!!
	//		//if( transformAllowCallOnSetTransformAndUpdatePhysicsModel )
	//		//{
	//		//	for( int nComponent = 0; nComponent < Components.Count; nComponent++ )
	//		//	{
	//		//		Component_MapObjectChild component = Components[ nComponent ] as Component_MapObjectChild;
	//		//		if( component != null )
	//		//			component.OnOldTransformWasChangedManually();
	//		//	}
	//		//}
	//	}

	//	/// <summary>
	//	/// Sets the old transformation of object.
	//	/// </summary>
	//	public void SetOldTransform( Vec3 pos, Quat rot, Vec3 scl )
	//	{
	//		SetOldTransform( ref pos, ref rot, ref scl );
	//	}

	//	//было, но какой-то хлам что-то
	//	///// <summary>
	//	///// Copies parameters of transformation from other object.
	//	///// </summary>
	//	///// <remarks>
	//	///// The method copies both current and old transformation.
	//	///// </remarks>
	//	///// <param name="from">The source object.</param>
	//	//public void CopyTransform( MapObject from )
	//	//{
	//	//   OnCopyTransform( from );
	//	//}

	//	//было, но какой-то хлам что-то
	//	//зачем virtual?
	//	///// <summary>
	//	///// Called when the method <see cref="CopyTransform(MapObject)"/> is called.
	//	///// </summary>
	//	///// <param name="from">The source object.</param>
	//	//protected virtual void OnCopyTransform( MapObject from )
	//	//{
	//	//   SetTransform( from.Position, from.Rotation, from.Scale );
	//	//   SetOldTransform( from.OldPosition, from.OldRotation, from.OldScale );
	//	//}

	//	///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	//	/// <summary>
	//	/// Gets the world bounds of the object.
	//	/// </summary>
	//	[Browsable( false )]
	//	public Bounds WorldBounds
	//	{
	//		get { return worldBounds; }
	//	}

	//	/// <summary>
	//	/// Called when the world bounds during update.
	//	/// </summary>
	//	/// <remarks>
	//	/// By means of this method it is possible to change default alghoritm of calculation bounding box.
	//	/// </remarks>
	//	/// <param name="bounds">The bounds of object.</param>
	//	protected virtual void OnWorldBoundsUpdateBegin( ref Bounds bounds ) { }
	//	protected virtual void OnWorldBoundsUpdateEnd() { }

	//	public delegate void WorldBoundsUpdateBeginDelegate( Component_MapObject obj, ref Bounds bounds );
	//	public event WorldBoundsUpdateBeginDelegate WorldBoundsUpdateBegin;
	//	public delegate void WorldBoundsUpdateEndDelegate( Component_MapObject obj );
	//	public event WorldBoundsUpdateEndDelegate WorldBoundsUpdateEnd;

	//	//!!!!!вызывать
	//	/// <summary>
	//	/// Calcylates world bounds of the objects. More: <see cref="WorldBounds"/>.
	//	/// </summary>
	//	/// <remarks>
	//	/// By default the object bounds calculation is completely automatic, but in special cases need update manually.
	//	/// </remarks>
	//	public void WorldBoundsUpdate()
	//	{
	//		//!!!!!if(EnabledInHierarchy)?
	//		//!!!!!проверить можно ли обновлять
	//		//!!!!!TransformEnabled
	//		//!!!!!EnabledInHiearrchy

	//		//!!!!!
	//		//if( transformAllowCallOnSetTransformAndUpdatePhysicsModel )
	//		{
	//			//!!!!!if(transformEnabled), а если нет то что ставить?

	//			Bounds newBounds;
	//			//!!!!было
	//			//if( PhysicsModelInstance != null )
	//			//{
	//			//	PhysicsModelInstance.GetGlobalBounds( out newBounds );
	//			//	newBounds.Add( Position );
	//			//}
	//			//else
	//			newBounds = new Bounds( Position );

	//			//!!!!было
	//			//for( int n = 0; n < Components.Count; n++ )
	//			//{
	//			//	MapObjectComponent component2 = Components[ n ] as MapObjectComponent;
	//			//	component2?.OnMapBoundsUpdating( ref newBounds );
	//			//}

	//			OnWorldBoundsUpdateBegin( ref newBounds );
	//			WorldBoundsUpdateBegin?.Invoke( this, ref newBounds );

	//			worldBounds = newBounds;

	//			UpdateInSceneGraph();

	//			OnWorldBoundsUpdateEnd();
	//			WorldBoundsUpdateEnd?.Invoke( this );
	//		}
	//	}

	//	///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	//	//!!!!!было
	//	//public void UpdatePositionAndRotationByPhysics( bool forceUpdate )
	//	//{
	//	//	//!!!!!а также не подписываться на тики бы?
	//	//	//!!!!!!slow?

	//	//	bool updated = false;
	//	//	Body mainBody = null;

	//	//	if( PhysicsModelInstance != null )
	//	//	{
	//	//		Body[] list = PhysicsModelInstance.Bodies;

	//	//		//get mainBody
	//	//		if( list.Length != 0 )
	//	//			mainBody = list[ 0 ];

	//	//		if( !forceUpdate )
	//	//		{
	//	//			//check for update
	//	//			foreach( Body body in list )
	//	//			{
	//	//				if( body._TransformHasChangedByLibrary )
	//	//				{
	//	//					updated = true;
	//	//					break;
	//	//				}
	//	//			}
	//	//		}
	//	//		else
	//	//			updated = true;
	//	//	}

	//	//	if( updated )
	//	//	{
	//	//		Vec3 p;
	//	//		Quat r;

	//	//		if( mainBody != null )
	//	//		{
	//	//			PhysicsModel modelDeclaration = PhysicsModelInstance.ModelDeclaration;
	//	//			if( modelDeclaration != null )
	//	//			{
	//	//				Body declBody = null;
	//	//				if( modelDeclaration.Bodies.Length != 0 )
	//	//					declBody = modelDeclaration.Bodies[ 0 ];

	//	//				if( declBody != null )
	//	//				{
	//	//					Vec3 declPosition = declBody.Position;
	//	//					Quat declRotation = declBody.Rotation;

	//	//					if( declPosition != Vec3.Zero || declRotation != Quat.Identity )
	//	//					{
	//	//						p = mainBody.Position - declPosition * mainBody.Rotation;
	//	//						r = mainBody.Rotation * declRotation.GetInverse();
	//	//					}
	//	//					else
	//	//					{
	//	//						p = mainBody.Position;
	//	//						r = mainBody.Rotation;
	//	//					}
	//	//				}
	//	//				else
	//	//				{
	//	//					p = mainBody.Position;
	//	//					r = mainBody.Rotation;
	//	//				}
	//	//			}
	//	//			else
	//	//			{
	//	//				p = mainBody.Position;
	//	//				r = mainBody.Rotation;
	//	//			}
	//	//		}
	//	//		else
	//	//		{
	//	//			p = Vec3.Zero;
	//	//			r = Quat.Identity;
	//	//		}

	//	//		Vec3 s = new Vec3( 1, 1, 1 );
	//	//		TransformChange( ref p, ref r, ref s );

	//	//		foreach( Body body in PhysicsModelInstance.Bodies )
	//	//			body._TransformHasChangedByLibrary = false;
	//	//	}
	//	//}

	//	void OnUpdateOldTransform()
	//	{
	//		oldPosition = position;
	//		oldRotation = rotation;
	//		oldScale = scale;
	//		oldTransformIsDifferent = oldPosition != position || oldRotation != rotation || oldScale != scale;
	//		oldTransformUpdateTime = ParentMap.SimulationTickTime;

	//		//!!!!эвентом? надо ли такой эвент? какой был бы лучше?

	//		//!!!!!
	//		////!!!!!slowly?
	//		//for( int n = 0; n < Components.Count; n++ )
	//		//{
	//		//	Component_MapObjectChild component = Components[ n ] as Component_MapObjectChild;
	//		//	if( component != null )
	//		//		component.OnUpdateOldTransform();
	//		//}
	//	}

	//	//!!!!!
	//	//public void GetInterpolatedTransform( xx xx xx);

	//	/// <summary>
	//	/// Returns the interpolated position of object.
	//	/// </summary>
	//	/// <returns>The interpolated position.</returns>
	//	public Vec3 GetInterpolatedPosition()
	//	{
	//		if( oldTransformIsDifferent && position != oldPosition )
	//		{
	//			double diffTime = ParentMap.LastRenderTime - oldTransformUpdateTime;
	//			if( diffTime < Component_Map.SimulationTickDelta )
	//			{
	//				double time = diffTime * MapSystemWorld.SimulationTicksPerSecond;
	//				if( time < 0 )
	//					time = 0;
	//				Vec3 pos;
	//				Vec3.Lerp( ref oldPosition, ref position, time, out pos );
	//				//pos = oldPosition * ( 1.0f - time ) + position * time;
	//				return pos;
	//			}
	//		}

	//		return position;
	//	}

	//	/// <summary>
	//	/// Returns the interpolated rotation of object.
	//	/// </summary>
	//	/// <returns>The interpolated rotation.</returns>
	//	public Quat GetInterpolatedRotation()
	//	{
	//		if( oldTransformIsDifferent && rotation != oldRotation )
	//		{
	//			double diffTime = ParentMap.LastRenderTime - oldTransformUpdateTime;
	//			if( diffTime < Component_Map.SimulationTickDelta )
	//			{
	//				double time = diffTime * MapSystemWorld.SimulationTicksPerSecond;
	//				if( time < 0 )
	//					time = 0;
	//				Quat rot;
	//				Quat.Slerp( ref oldRotation, ref rotation, time, out rot );
	//				//rot = Quat.Slerp( oldRotation, rotation, time );
	//				return rot;
	//			}
	//		}
	//		return rotation;
	//	}

	//	/// <summary>
	//	/// Returns the interpolated scale of object.
	//	/// </summary>
	//	/// <returns>The interpolated scale.</returns>
	//	public Vec3 GetInterpolatedScale()
	//	{
	//		if( oldTransformIsDifferent && scale != oldScale )
	//		{
	//			double diffTime = ParentMap.LastRenderTime - oldTransformUpdateTime;
	//			if( diffTime < Component_Map.SimulationTickDelta )
	//			{
	//				double time = diffTime * MapSystemWorld.SimulationTicksPerSecond;
	//				if( time < 0 )
	//					time = 0;
	//				Vec3 scl;
	//				Vec3.Lerp( ref oldScale, ref scale, time, out scl );
	//				//scl = oldScale * ( 1.0f - time ) + scale * time;
	//				return scl;
	//			}
	//		}
	//		return scale;
	//	}

	//	/// <summary>
	//	/// Returns the interpolated transformation of object.
	//	/// </summary>
	//	/// <param name="pos">The interpolated position.</param>
	//	/// <param name="rot">The interpolated rotation.</param>
	//	/// <param name="scl">The interpolated scale.</param>
	//	public void GetInterpolatedTransform( out Vec3 pos, out Quat rot, out Vec3 scl )
	//	{
	//		if( oldTransformIsDifferent )
	//		{
	//			double diffTime = ParentMap.LastRenderTime - oldTransformUpdateTime;
	//			if( diffTime < Component_Map.SimulationTickDelta )
	//			{
	//				double time = diffTime * MapSystemWorld.SimulationTicksPerSecond;
	//				if( time < 0 )
	//					time = 0;

	//				Vec3.Lerp( ref oldPosition, ref position, time, out pos );
	//				//pos = oldPosition * ( 1.0f - time ) + position * time;
	//				Quat.Slerp( ref oldRotation, ref rotation, time, out rot );
	//				//rot = Quat.Slerp( oldRotation, rotation, time );
	//				Vec3.Lerp( ref oldScale, ref scale, time, out scl );
	//				//scl = oldScale * ( 1.0f - time ) + scale * time;
	//				return;
	//			}
	//		}
	//		pos = position;
	//		rot = rotation;
	//		scl = scale;
	//	}

	//	//!!!!!нужно networking
	//	//protected virtual bool OnIsNeedToAttachObjectFromType( MapObjectTypeAttachedObject typeObject )
	//	//{
	//	//   //check for attached map object (synchronized via network)
	//	//   if( EntitySystemWorld.Instance.IsClientOnly() &&
	//	//      Type.NetworkType == EntityNetworkTypes.Synchronized )
	//	//   {
	//	//      MapObjectTypeAttachedMapObject typeObjectMapObject = typeObject as
	//	//         MapObjectTypeAttachedMapObject;

	//	//      if( typeObjectMapObject != null &&
	//	//         typeObjectMapObject.Type.NetworkType == EntityNetworkTypes.Synchronized )
	//	//      {
	//	//         int index = Array.IndexOf<MapObjectTypeAttachedObject>( Type.AttachedObjects,
	//	//            typeObject );

	//	//         if( client_attachedMapObjectsInfo == null )
	//	//            return false;

	//	//         uint networkUIN = client_attachedMapObjectsInfo[ index ];
	//	//         if( networkUIN == 0 )
	//	//            return false;

	//	//         if( Entities.Instance.GetByNetworkUIN( networkUIN ) == null )
	//	//            return false;
	//	//      }
	//	//   }

	//	//   return true;
	//	//}

	//	//!!!!!!
	//	///// <summary>
	//	///// Gets the physics model of object.
	//	///// </summary>
	//	//[Browsable( false )]
	//	//public PhysicsModel PhysicsModelInstance
	//	//{
	//	//	get { return physicsModelInstance; }
	//	//	set { physicsModelInstance = value; }
	//	//}

	//	//!!!!review names
	//	///// <summary>
	//	///// Gets the object to which is attached the this object. This object can be attached to another object by means MapObjectComponent_AttachObject.
	//	///// </summary>
	//	//[Browsable( false )]
	//	//public MapObject ThisObjectIsAttachedTo_Object
	//	//{
	//	//	get { return thisObjectIsAttachedTo_Object; }
	//	//}

	//	//!!!!review names
	//	///// <summary>
	//	///// Gets the joint of the object to which is attached the this object. This object can be attached to another object by means MapObjectComponent_AttachObject.
	//	///// </summary>
	//	//[Browsable( false )]
	//	//public Joint ThisObjectIsAttachedTo_ObjectJoint
	//	//{
	//	//	get { return thisObjectIsAttachedTo_ObjectJoint; }
	//	//}

	//	//!!!!!

	//	///// <summary>
	//	///// Represents the method that handles a <see cref="ThisObjectIsAttachedTo_Attach"/> event.
	//	///// </summary>
	//	///// <param name="entity">The object.</param>
	//	///// <param name="attachedToJoint"></param>
	//	///// <param name="attachedToObject"></param>
	//	//public delegate void ThisObjectIsAttachedTo_AttachDelegate( MapObject entity, MapObject attachedToObject, Joint attachedToJoint );

	//	////!!!!!name. надо ли?
	//	///// <summary>
	//	///// Occurs after object has attached to another object by means Attach Object component.
	//	///// </summary>
	//	//public event ThisObjectIsAttachedTo_AttachDelegate ThisObjectIsAttachedTo_Attach;

	//	///// <summary>
	//	///// Represents the method that handles <see cref="ThisObjectIsAttachedTo_Detach"/> event.
	//	///// </summary>
	//	///// <param name="entity">The object.</param>
	//	//public delegate void ThisObjectIsAttachedTo_DetachDelegate( MapObject entity );

	//	////!!!!!name. надо ли?
	//	///// <summary>
	//	///// Occurs before object has detached from another object by means Attach Object component.
	//	///// </summary>
	//	//public event ThisObjectIsAttachedTo_DetachDelegate ThisObjectIsAttachedTo_Detach;

	//	//!!!!!

	//	///// <summary>
	//	///// Caused after object has attached to another object by means Attach Object component.
	//	///// </summary>
	//	//protected internal virtual void ThisObjectIsAttachedTo_OnAttach()
	//	//{
	//	//	if( ThisObjectIsAttachedTo_Attach != null )
	//	//		ThisObjectIsAttachedTo_Attach( this, ThisObjectIsAttachedTo_Object, ThisObjectIsAttachedTo_ObjectJoint );
	//	//}

	//	////!!!!!
	//	///// <summary>
	//	///// Caused before object has detached from another object by means Attach Object component.
	//	///// </summary>
	//	//protected internal virtual void ThisObjectIsAttachedTo_OnDetach()
	//	//{
	//	//	if( ThisObjectIsAttachedTo_Detach != null )
	//	//		ThisObjectIsAttachedTo_Detach( this );
	//	//}

	//	/// <summary>
	//	/// Gets the transform as <see cref="Box"/> structure.
	//	/// </summary>
	//	/// <returns></returns>
	//	public Box GetTransformAsBox()
	//	{
	//		Mat3 rot;
	//		rotation.ToMat3( out rot );
	//		return new Box( position, new Vec3( scale.X * .5f, scale.Y * .5f, scale.Z * .5f ), rot );
	//	}

	//	/// <summary>
	//	/// Calculates the transformation of object as Matrix.
	//	/// </summary>
	//	/// <returns>The transformation of object as Matrix.</returns>
	//	public Mat4 GetTransformAsMatrix()
	//	{
	//		Mat3 rot;
	//		rotation.ToMat3( out rot );

	//		Mat3 scl;
	//		Mat3.FromScale( ref scale, out scl );

	//		Mat3 mat;
	//		Mat3.Multiply( ref rot, ref scl, out mat );

	//		return new Mat4( mat, position );

	//		//return new Mat4( Rotation.ToMat3() * Mat3.FromScale( Scale ), Position );
	//	}

	//	/// <summary>
	//	/// Returns Position, Rotation, Scale.
	//	/// </summary>
	//	public void GetTransform( out Vec3 pos, out Quat rot, out Vec3 scl )
	//	{
	//		pos = Position;
	//		rot = Rotation;
	//		scl = Scale;
	//	}

	//	//!!!!!!путается с LastRenderTime
	//	/// <summary>
	//	/// Gets the last Tick time.
	//	/// </summary>
	//	[Browsable( false )]
	//	public double OldTransformUpdateTime//!!!!было: LastTickTime
	//	{
	//		get { return oldTransformUpdateTime; }
	//	}

	//	//!!!!!Visible During Environment Cubemap Generation.

	//	/// <summary>
	//	/// Gets or sets the group for the scene manager. This property gives the ability to filter objects during call Map.GetObjects methods and similar.
	//	/// </summary>
	//	[Browsable( false )]
	//	public int SceneGraphGroup
	//	{
	//		get { return sceneGraphGroup; }
	//		set
	//		{
	//			if( value < 0 || value >= 32 )
	//				Log.Fatal( "MapObject: SceneGraphGroup set: value < 0 || value >= 32." );

	//			if( sceneGraphGroup == value )
	//				return;
	//			sceneGraphGroup = value;

	//			UpdateInSceneGraph();
	//		}
	//	}

	//	///////////////////////////////////////////

	//	public class EditorCheckSelectionByRayResult
	//	{
	//		double rayScale;
	//		object obj;
	//		double priority;

	//		public double RayScale
	//		{
	//			get { return rayScale; }
	//			set { rayScale = value; }
	//		}

	//		/// <summary>
	//		/// MapObject or MapObjectComponent.
	//		/// </summary>
	//		public object Obj
	//		{
	//			get { return obj; }
	//			set { obj = value; }
	//		}

	//		/// <summary>
	//		/// Priority of selection. It is by default equal <b>0.5</b>.
	//		/// The object with the highest priority will be selected.
	//		/// </summary>
	//		public double Priority
	//		{
	//			get { return priority; }
	//			set { priority = value; }
	//		}

	//		/// <summary>
	//		/// </summary>
	//		/// <param name="rayScale"></param>
	//		/// <param name="obj">MapObject or MapObjectComponent.</param>
	//		/// <param name="priority">
	//		/// Priority of selection. It is by default equal <b>0.5</b>.
	//		/// The object with the highest priority will be selected.
	//		/// </param>
	//		public EditorCheckSelectionByRayResult( double rayScale, object obj, double priority = .5f )
	//		{
	//			this.rayScale = rayScale;
	//			this.obj = obj;
	//			this.priority = priority;
	//		}
	//	}

	//	//!!!!
	//	//public delegate void EditorCheckSelectionByRayEventDelegate( MapObject obj, Ray ray, ref bool _checked, List<EditorCheckSelectionByRayResult> list );
	//	//public event EditorCheckSelectionByRayEventDelegate EditorCheckSelectionByRayEvent;

	//	/// <summary>
	//	/// Caused when the user tries to select object the mouse.
	//	/// </summary>
	//	/// <param name="ray">The selection ray.</param>
	//	/// <returns>Returns <b>true</b> the object is selected; otherwise, <b>false</b>.</returns>
	//	///// <param name="rayScale">The position of intersection on the ray.</param>
	//	///// <param name="priority">
	//	///// Priority of selection. It is by default equal <b>0.5</b>.
	//	///// The object with the highest priority will be selected.
	//	///// </param>
	//	public virtual EditorCheckSelectionByRayResult[] EditorCheckSelectionByRay( Ray ray )
	//	{
	//		//!!!!!
	//		Log.Fatal( "impl" );
	//		return null;

	//		//bool _checked = false;
	//		//List<EditorCheckSelectionByRayResult> list = new List<EditorCheckSelectionByRayResult>();

	//		////components
	//		//for( int nComponent = 0; nComponent < Components.Count; nComponent++ )
	//		//{
	//		//	Component_MapObjectChild component = Components[ nComponent ] as Component_MapObjectChild;
	//		//	if( component != null && component.Visible )
	//		//		component.EditorCheckSelectionByRay( ray, ref _checked, list );
	//		//}

	//		////event
	//		//EditorCheckSelectionByRayEvent?.Invoke( this, ray, ref _checked, list );

	//		////check by MapBounds
	//		//if( !_checked && list.Count == 0 )
	//		//{
	//		//	double scale;
	//		//	if( WorldBounds.RayIntersection( ray, out scale ) )
	//		//		list.Add( new EditorCheckSelectionByRayResult( (double)scale, this ) );
	//		//}

	//		////sort by RayScale
	//		//ListUtils.MergeSort( list, delegate ( EditorCheckSelectionByRayResult r1, EditorCheckSelectionByRayResult r2 )
	//		//{
	//		//	//!!!!
	//		//	Log.Fatal( "check" );

	//		//	if( r1.Priority < r2.Priority )
	//		//		return -1;
	//		//	if( r1.Priority > r2.Priority )
	//		//		return 1;
	//		//	return 0;
	//		//} );

	//		//return list.ToArray();
	//	}

	//	public enum EditorRenderSelectionMode
	//	{
	//		Selected,
	//		CanSelect,
	//	}

	//	//!!!!
	//	//public delegate void EditorRenderSelectionBorderEventDelegate( MapObject obj, Viewport viewport, EditorRenderSelectionMode mode, bool simpleVisualization, DynamicMeshManager.MaterialData dynamicMeshManagerMaterial, ref bool hasBeenDrawn );
	//	//public event EditorRenderSelectionBorderEventDelegate EditorRenderSelectionBorderEvent;

	//	public virtual void EditorRenderSelectionBorder( Viewport viewport, EditorRenderSelectionMode mode, bool simpleVisualization, DynamicMeshManager.MaterialData dynamicMeshManagerMaterial )
	//	{
	//		//!!!!!!
	//		Log.Fatal( "impl" );

	//		//bool hasBeenDrawn = false;

	//		//if( !simpleVisualization )
	//		//{
	//		//	//components
	//		//	for( int nComponent = 0; nComponent < Components.Count; nComponent++ )
	//		//	{
	//		//		Component_MapObjectChild component = Components[ nComponent ] as Component_MapObjectChild;
	//		//		if( component != null && component.Visible )
	//		//			component.EditorRenderSelectionBorder( viewport, mode, simpleVisualization, dynamicMeshManagerMaterial, ref hasBeenDrawn );
	//		//	}
	//		//}

	//		////event
	//		//EditorRenderSelectionBorderEvent?.Invoke( this, viewport, mode, simpleVisualization, dynamicMeshManagerMaterial, ref hasBeenDrawn );

	//		////render MapBounds
	//		//if( !hasBeenDrawn )
	//		//{
	//		//	//!!!!
	//		//	Log.Fatal( "impl" );

	//		//	//DynamicMeshManager manager = ParentMap.SceneObjects.DynamicMeshManager;

	//		//	//DynamicMeshManager.Block block = manager.GetBlockFromCacheOrCreate( "MapObject.Editor_RenderSelectionBorder: Box" );
	//		//	//block.AddBox( false, new BoxD( Vec3D.Zero, Vec3D.One, Mat3D.Identity ), null );

	//		//	//BoundsD bounds = WorldBounds;
	//		//	//Vec3D boundsCenter = bounds.GetCenter();
	//		//	//manager.AddBlockToScene( block, bounds.GetCenter(), Quat.Identity, ( bounds.Maximum - boundsCenter ).ToVec3(), false, dynamicMeshManagerMaterial );
	//		//}
	//	}

	//	public enum EditorTransformModifyAction
	//	{
	//		Begin,
	//		Commit,
	//		Cancel,
	//	}
	//	public delegate void EditorTransformModifyStatusChangedDelegate( Component_MapObject obj, EditorTransformModifyAction action );
	//	public event EditorTransformModifyStatusChangedDelegate EditorTransformModifyStatusChanged;

	//	public virtual void EditorChangeTransformModifyStatus( EditorTransformModifyAction action )
	//	{
	//		editorTransformModifyActivated = action == EditorTransformModifyAction.Begin;

	//		//for( int n = 0; n < Components.Count; n++ )
	//		//{
	//		//	MapObjectComponent component2 = Components[ n ] as MapObjectComponent;
	//		//	if( component2 != null )
	//		//		component2.Editor_TransformModify( action );
	//		//}

	//		EditorTransformModifyStatusChanged?.Invoke( this, action );
	//	}

	//	[Browsable( false )]
	//	public bool EditorTransformModifyActivated
	//	{
	//		get { return editorTransformModifyActivated; }
	//	}

	//	//!!!!!!
	//	///// <summary>
	//	///// Gets or sets the mode of automatic alignment by Z axis. Using this property developer can configure automatic alignment of the object vertically.
	//	///// </summary>
	//	//[Description( "The mode of automatic alignment by Z axis. Using this property developer can configure automatic alignment of the object vertically." )]
	//	//[DefaultValue( AutoVerticalAlignments.None )]
	//	//[Category( "Editor" )]
	//	//public AutoVerticalAlignments AutoVerticalAlignment
	//	//{
	//	//	get { return autoVerticalAlignment; }
	//	//	set
	//	//	{
	//	//		if( autoVerticalAlignment == value )
	//	//			return;
	//	//		autoVerticalAlignment = value;
	//	//	}
	//	//}

	//	//!!!!!!нужно
	//	//protected internal override void Server_OnClientConnectedBeforePostCreate( RemoteEntityWorld remoteEntityWorld )
	//	//{
	//	//	base.Server_OnClientConnectedBeforePostCreate( remoteEntityWorld );

	//	//	//Server_SendAttachedMapObjectsInfoToClients( new RemoteEntityWorld[] { remoteEntityWorld } );
	//	//}

	//	//!!!!!!нужно
	//	//void Server_SendAttachedMapObjectsInfoToClients( IList<RemoteEntityWorld> remoteEntityWorlds )
	//	//{
	//	//   SendDataWriter writer = null;

	//	//   foreach( MapObjectAttachedObject attachedObject in AttachedObjects )
	//	//   {
	//	//      MapObjectAttachedMapObject attachedMapObject = attachedObject as
	//	//         MapObjectAttachedMapObject;
	//	//      if( attachedMapObject == null )
	//	//         continue;
	//	//      if( attachedMapObject.TypeObject == null )
	//	//         continue;

	//	//      MapObject mapObject = attachedMapObject.MapObject;

	//	//      if( mapObject.Type.NetworkType != EntityNetworkTypes.Synchronized )
	//	//         continue;

	//	//      int index = Array.IndexOf<MapObjectTypeAttachedObject>( Type.AttachedObjects,
	//	//         attachedMapObject.TypeObject );
	//	//      if( index == -1 )
	//	//         Log.Fatal( "MapObject: Server_SendAttachedMapObjectsInfoToClients: index == -1." );

	//	//      //begin network message
	//	//      if( writer == null )
	//	//      {
	//	//         writer = BeginNetworkMessage( remoteEntityWorlds, typeof( MapObject ),
	//	//            (ushort)NetworkMessages.AttachedMapObjectsInfoToClient );
	//	//      }

	//	//      writer.WriteVariableUInt32( mapObject.NetworkUIN );
	//	//      writer.WriteVariableUInt32( (uint)index );
	//	//   }

	//	//   if( writer != null )
	//	//      EndNetworkMessage();
	//	//}

	//	//!!!!!!нужно
	//	//[NetworkReceive( NetworkDirections.ToClient, (ushort)NetworkMessages.AttachedMapObjectsInfoToClient )]
	//	//void Client_ReceiveAttachedMapObjectsInfo( RemoteEntityWorld sender, ReceiveDataReader reader )
	//	//{
	//	//   client_attachedMapObjectsInfo = new uint[ Type.AttachedObjects.Length ];

	//	//   while( reader.BitPosition < reader.EndBitPosition )
	//	//   {
	//	//      uint networkUIN = reader.ReadVariableUInt32();
	//	//      int index = (int)reader.ReadVariableUInt32();

	//	//      if( reader.Overflow )
	//	//         return;

	//	//      client_attachedMapObjectsInfo[ index ] = networkUIN;
	//	//   }
	//	//}

	//	//public static MapObject GetMapObjectBySceneNode( SceneNode sceneNode )
	//	//{
	//	//	return sceneNode.Owner as MapObject;
	//	//}

	//	//!!!!!review
	//	public virtual bool IsAllowToChangeScale( out string reason )
	//	{
	//		//!!!!!надо для физической модели поддержать скейлинг
	//		//!!!!было
	//		//if( physicsModelInstance != null && !string.IsNullOrEmpty( Type.PhysicsModel ) )
	//		//{
	//		//	reason = ToolsLocalization.Translate( "Various", "Objects with loaded physics model do not support scaling." );
	//		//	return false;
	//		//}

	//		reason = null;
	//		return true;
	//	}

	//	//!!!!
	//	////!!!!!возможность скрыть в редактор карт, чтобы нельзя менять было. например для перса или тачки
	//	///// <summary>
	//	///// Gets or sets the file name of physics model.
	//	///// </summary>
	//	//[Description( "The file name of physics model." )]
	//	//[DefaultValue( "" )]
	//	//[Editor( typeof( EditorPhysicsModelUITypeEditor ), typeof( UITypeEditor ) )]
	//	//[SupportRelativePath]
	//	//public string PhysicsModelName
	//	//{
	//	//	get { return physicsModelName; }
	//	//	set
	//	//	{
	//	//		if( physicsModelName == value )
	//	//			return;
	//	//		physicsModelName = value;

	//	//		if( !string.IsNullOrEmpty( physicsModelName ) )
	//	//			PhysicsModelLoad( physicsModelName, true, !physicsModelManualControlPushedToWorldProperty );
	//	//		else
	//	//			PhysicsModelDestroy();
	//	//	}
	//	//}

	//	//!!!!!
	//	//string GetFullPath( string path )
	//	//{
	//	//	if( Type != null )
	//	//		return VirtualPathUtils.ConvertRelativePathToFull( Path.GetDirectoryName( Type.Name ), path );
	//	//	else
	//	//		return path;
	//	//}

	//	//!!!!!
	//	//public string PhysicsModelName_GetFullPath()
	//	//{
	//	//	return GetFullPath( PhysicsModelName );
	//	//}

	//	//!!!!!
	//	//[Browsable( false )]
	//	//public bool PhysicsModelManualControlPushedToWorldProperty
	//	//{
	//	//	get { return physicsModelManualControlPushedToWorldProperty; }
	//	//	set { physicsModelManualControlPushedToWorldProperty = value; }
	//	//}

	//	//!!!!!
	//	///// <summary>
	//	///// Creates the empty physics model of object.
	//	///// </summary>
	//	//public virtual void PhysicsModelCreate( bool destroyCurrentPhysicsModel )
	//	//{
	//	//	if( destroyCurrentPhysicsModel )
	//	//		PhysicsModelDestroy();

	//	//	physicsModelInstance = ParentMap.PhysicsScene.CreatePhysicsModel( this );
	//	//}

	//	//!!!!!
	//	//public virtual void PhysicsModelChange( PhysicsModel physicsModel, bool destroyCurrentPhysicsModel, bool pushToWorld )
	//	//{
	//	//	//remove or destroy old
	//	//	if( destroyCurrentPhysicsModel )
	//	//		PhysicsModelDestroy();

	//	//	//change field
	//	//	this.physicsModelInstance = physicsModel;

	//	//	//set contact groups
	//	//	foreach( Body body in physicsModel.Bodies )
	//	//	{
	//	//		foreach( Shape shape in body.Shapes )
	//	//		{
	//	//			int group = shape.ContactGroup;
	//	//			if( group == 0 )
	//	//				shape.ContactGroup = (int)ContactGroup.Dynamic;
	//	//		}
	//	//	}

	//	//	physicsModel.Owner = this;

	//	//	//update bodies positions and push to world.
	//	//	if( !IsDestroyed )
	//	//	{
	//	//		if( PhysicsModelInstance.ModelDeclaration != null )
	//	//		{
	//	//			physicsModel.ResetTransformFromModelDeclaration( ref position, ref rotation );
	//	//		}
	//	//		else
	//	//		{
	//	//			foreach( Body body in PhysicsModelInstance.Bodies )
	//	//				body.SetTransform( position, rotation );
	//	//		}

	//	//		if( pushToWorld )
	//	//			physicsModel.PushedToWorld = true;

	//	//		//update body references for MapObjectCompnent components
	//	//		for( int nComponent = 0; nComponent < Components.Count; nComponent++ )
	//	//		{
	//	//			Component_MapObjectChild component = Components[ nComponent ] as Component_MapObjectChild;
	//	//			if( component != null )
	//	//				component.AttachToBody_OnPhysicsModelChange();
	//	//		}
	//	//	}
	//	//}

	//	//!!!!!
	//	///// <summary>
	//	///// Loads a new physics model.
	//	///// </summary>
	//	///// <param name="fileName"></param>
	//	///// <param name="destroyCurrentPhysicsModel"></param>
	//	///// <param name="pushToWorld"></param>
	//	///// <remarks>
	//	///// Need reset Body properties for attached objects before call this method.
	//	///// </remarks>
	//	//public virtual void PhysicsModelLoad( string fileName, bool destroyCurrentPhysicsModel, bool pushToWorld )
	//	//{
	//	//	if( destroyCurrentPhysicsModel )
	//	//		PhysicsModelDestroy();

	//	//	//load model
	//	//	PhysicsModel model = ParentMap.PhysicsScene.LoadPhysicsModel( fileName, this );
	//	//	if( model == null )
	//	//	{
	//	//		Log.Warning( "MapObject: Unable to load physics model with name \"{0}\".", fileName );
	//	//		return;
	//	//	}

	//	//	PhysicsModelChange( model, destroyCurrentPhysicsModel, pushToWorld );
	//	//}

	//	//!!!!!
	//	////!!!!event?
	//	///// <summary>
	//	///// Destroys the physics model of object.
	//	///// </summary>
	//	//public virtual void PhysicsModelDestroy()
	//	//{
	//	//	if( physicsModelInstance != null )
	//	//	{
	//	//		physicsModelInstance.Dispose();
	//	//		physicsModelInstance = null;
	//	//	}
	//	//}

	//	void AddToSceneGraph()
	//	{
	//		//add object to the scene graph
	//		sceneGraphIndex = ParentMap.mapObjectsSceneGraphContainer.AddObject( WorldBounds, SceneGraphGroup );

	//		//add to sceneGraphObjects
	//		while( ParentMap.mapObjectsSceneGraphObjectsList.Count <= sceneGraphIndex )
	//			ParentMap.mapObjectsSceneGraphObjectsList.Add( null );
	//		ParentMap.mapObjectsSceneGraphObjectsList[ sceneGraphIndex ] = this;

	//		//MapObjectAddToSceneGraph?.Invoke( obj );
	//	}

	//	void RemoveFromSceneGraph()
	//	{
	//		if( sceneGraphIndex != -1 )
	//		{
	//			//MapObjectRemoveFromSceneGraph?.Invoke( obj );

	//			ParentMap.mapObjectsSceneGraphObjectsList[ sceneGraphIndex ] = null;
	//			ParentMap.mapObjectsSceneGraphContainer.RemoveObject( sceneGraphIndex );
	//			sceneGraphIndex = -1;
	//		}
	//	}

	//	void UpdateInSceneGraph()
	//	{
	//		if( sceneGraphIndex != -1 && transformEnabled )
	//		{
	//			//MapObjectRemoveFromSceneGraph?.Invoke( obj );
	//			ParentMap.mapObjectsSceneGraphContainer.UpdateObject( sceneGraphIndex, WorldBounds, SceneGraphGroup );
	//			//MapObjectAddToSceneGraph?.Invoke( obj );
	//		}
	//	}


	//	//!!!!!редактировать в редакторе?
	//	//[Browsable( false )]
	//	///// <summary>
	//	///// Gets or sets the position of the object in the world.
	//	///// </summary>
	//	//[Description( "The position of the object in the world." )]
	//	[DefaultValue( true )]
	//	[Serialize]
	//	public bool TransformEnabled
	//	{
	//		get { return transformEnabled; }
	//		set
	//		{
	//			if( transformEnabled == value )
	//				return;

	//			if( EnabledInHierarchy )
	//			{
	//				RemoveFromSceneGraph();
	//				ParentMap.mapObjectsWithDisabledTransform.Remove( this );
	//			}

	//			transformEnabled = value;

	//			if( EnabledInHierarchy )
	//			{
	//				if( transformEnabled )
	//					AddToSceneGraph();
	//				else
	//					ParentMap.mapObjectsWithDisabledTransform.Add( this );
	//			}
	//		}
	//	}

	//	[Browsable( false )]
	//	[Serialize]
	//	public string LayerName
	//	{
	//		get { return layerName; }
	//		set { layerName = value; }
	//	}
	//	//!!!!!
	//	///// <summary>
	//	///// Gets or sets the layer where the object is placed. Layers are configured in the Objects window.
	//	///// </summary>
	//	//[Description( "The layer where the object is placed. Layers can be configured in the Objects window." )]
	//	//[Editor( typeof( Editor.MapLayerTypeEditor ), typeof( UITypeEditor ) )]
	//	//[Category( "Editor" )]
	//	//public Map.LayerClass Layer
	//	//{
	//	//	get { return layer; }
	//	//	set { layer = value; }
	//	//}

	//	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	//	//!!!!!!нужны эвенты, чтобы локально вместе зацепленные для этого объекта были. и поэтому по сути не всегда нужно отписываться, т.к. они вместе удалятся

	//	public delegate void ViewportUpdateDelegate( Component_MapObject obj, Viewport viewport );
	//	public event ViewportUpdateDelegate ViewportUpdateBegin;
	//	public event ViewportUpdateDelegate ViewportUpdateEnd;

	//	protected virtual void OnViewportUpdateBegin( Viewport viewport )
	//	{
	//		//!!!!хорошо бы чтобы камера уже была выставлена. т.к. это значительный эвент наружу. люди будут юзать разное.
	//		//!!!!!!!!может тогда лучше, раз эвенты, в эвент уже в обновленном виде вызывать. т.е. внутри всё обновлено.

	//		//!!!!!
	//		////debug geometry
	//		//if( EngineSettings.Debug.Enabled )
	//		//{
	//		//	//!!!!!!!было: if( viewport.Camera.Purpose == RenderCamera.Purposes.UsualScene )
	//		//	{
	//		//		const double drawAsBoundsDistance = 100;

	//		//		RenderCamera camera = viewport.Camera;

	//		//		//draw static physics
	//		//		if( EngineSettings.Debug.DrawStaticPhysics )
	//		//		{
	//		//			double distanceSqr = WorldBounds.GetPointDistanceSqr( camera.Position );
	//		//			double farClipDistance = camera.FarClipDistance;
	//		//			if( distanceSqr < farClipDistance * farClipDistance )
	//		//			{
	//		//				viewport.DebugGeometry.SetSpecialDepthSettings( false, true );

	//		//				//!!!!!другие компоненты почему так
	//		//				foreach( MapObjectComponent_Mesh component in GetAllComponentsByClass<MapObjectComponent_Mesh>() )
	//		//				{
	//		//					Body body = component.CollisionBody;
	//		//					if( body != null )
	//		//					{
	//		//						if( distanceSqr < drawAsBoundsDistance * drawAsBoundsDistance )
	//		//						{
	//		//							body.DebugRender( viewport.DebugGeometry, 0, .5f, true, false, ColorValue.Zero );
	//		//						}
	//		//						else
	//		//						{
	//		//							viewport.DebugGeometry.Color = body.Sleeping ? new ColorValue( 0, 0, 1, .5f ) : new ColorValue( 0, 1, 0, .5f );
	//		//							viewport.DebugGeometry.AddBounds( body.GetWorldBounds() );
	//		//						}
	//		//					}
	//		//				}

	//		//				viewport.DebugGeometry.RestoreDefaultDepthSettings();
	//		//			}
	//		//		}

	//		//		//draw dynamic physics
	//		//		if( EngineSettings.Debug.DrawDynamicPhysics )
	//		//		{
	//		//			if( PhysicsModelInstance != null )
	//		//			{
	//		//				double distanceSqr = WorldBounds.GetPointDistanceSqr( camera.Position );
	//		//				double farClipDistance = camera.FarClipDistance;
	//		//				if( distanceSqr < farClipDistance * farClipDistance )
	//		//				{
	//		//					viewport.DebugGeometry.SetSpecialDepthSettings( false, true );

	//		//					if( distanceSqr < drawAsBoundsDistance * drawAsBoundsDistance )
	//		//					{
	//		//						double renderTime = ParentMap.LastRenderTime;
	//		//						//!!!!!!так?
	//		//						double timeInterpolate = ( renderTime - ParentMap.mapObjects.SimulationTickTime ) * MapSystemWorld.SimulationTicksPerSecond;
	//		//						MathEx.Clamp( ref timeInterpolate, 0, 1 );

	//		//						foreach( Body body in PhysicsModelInstance.Bodies )
	//		//							body.DebugRender( viewport.DebugGeometry, timeInterpolate, .5f, true, false, ColorValue.Zero );
	//		//						foreach( Joint joint in PhysicsModelInstance.Joints )
	//		//							joint.DebugRender( viewport.DebugGeometry, timeInterpolate, .5f, true, false, ColorValue.Zero );
	//		//					}
	//		//					else
	//		//					{
	//		//						foreach( Body body in PhysicsModelInstance.Bodies )
	//		//						{
	//		//							viewport.DebugGeometry.Color = body.Sleeping ? new ColorValue( 0, 0, 1, .5f ) : new ColorValue( 0, 1, 0, .5f );
	//		//							viewport.DebugGeometry.AddBounds( body.GetWorldBounds() );
	//		//						}
	//		//					}

	//		//					viewport.DebugGeometry.RestoreDefaultDepthSettings();
	//		//				}
	//		//			}
	//		//		}

	//		//		//draw map object bounds
	//		//		if( EngineSettings.Debug.DrawMapObjectBounds )
	//		//		{
	//		//			double distanceSqr = WorldBounds.GetPointDistanceSqr( camera.Position );
	//		//			double farClipDistance = camera.FarClipDistance;
	//		//			if( distanceSqr < farClipDistance * farClipDistance )
	//		//			{
	//		//				viewport.DebugGeometry.Color = new ColorValue( 1, 1, 0 );
	//		//				viewport.DebugGeometry.AddBounds( WorldBounds + ( GetInterpolatedPosition() - Position ) );
	//		//			}
	//		//		}
	//		//	}
	//		//}
	//	}

	//	protected virtual void OnViewportUpdateEnd( Viewport viewport )
	//	{
	//	}

	//	//!!!!name
	//	//!!!!так?
	//	//!!!!internal?
	//	internal void _PerformViewportUpdateBegin( Viewport viewport )
	//	{
	//		//!!!!везде посмотреть, чтобы эвенты были позже On метода

	//		OnViewportUpdateBegin( viewport );
	//		ViewportUpdateBegin?.Invoke( this, viewport );
	//	}

	//	//!!!!!
	//	internal void _PerformViewportUpdateEnd( Viewport viewport )
	//	{
	//		OnViewportUpdateEnd( viewport );
	//		ViewportUpdateEnd?.Invoke( this, viewport );
	//	}

	//	//!!!!
	//	//[Browsable( false )]
	//	//public CreationTypes CreationType
	//	//{
	//	//	get { return creationType; }
	//	//}

	//	//!!!!!сделать MapObject.OnLoad( MapLoadingContext context ). в нем можно получить TextBlock объекта. а также по id получить реальный.
	//	///// <summary>
	//	///// Gets the text block with the loading data from last load.
	//	///// </summary>
	//	//[Browsable( false )]
	//	//public TextBlock LastLoadingTextBlock
	//	//{
	//	//	get { return lastLoadingTextBlock; }
	//	//}



	//	protected override void OnEnabledInHierarchyChanged()
	//	{
	//		//!!!!!было. надо?
	//		//!!!!!почему тут?
	//		//if( EnabledInHierarchy )
	//		//{
	//		//	if( !string.IsNullOrEmpty( physicsModelName ) )
	//		//		PhysicsModelLoad( physicsModelName, true, true );

	//		//	SetTransform( ref position, ref rotation, ref scale );
	//		//	SetOldTransform( ref position, ref rotation, ref scale );

	//		//	transformAllowCallOnSetTransformAndUpdatePhysicsModel = true;
	//		//}

	//		base.OnEnabledInHierarchyChanged();

	//		//!!!!так? тут?
	//		if( EnabledInHierarchy )
	//		{
	//			//!!!!!так?
	//			ParentMap.mapObjectsAll.Add( this );

	//			//!!!!!!
	//			OnUpdateOldTransform();

	//			//!!!!!тут? может еще где?
	//			WorldBoundsUpdate();

	//			//add to scene graph
	//			if( transformEnabled )
	//				AddToSceneGraph();
	//			else
	//				ParentMap.mapObjectsWithDisabledTransform.Add( this );
	//		}
	//		else
	//		{
	//			//!!!!!
	//			//ParentMap точно должен быть заинитить тут ведь? проверить и тогда проверки-фаталы наставить

	//			//remove from scene graph
	//			if( transformEnabled )
	//				RemoveFromSceneGraph();
	//			else
	//			{
	//				if( ParentMap != null )
	//					ParentMap.mapObjectsWithDisabledTransform.Remove( this );
	//			}

	//			//!!!!!так?
	//			if( ParentMap != null )
	//				ParentMap.mapObjectsAll.Remove( this );

	//			//!!!!!
	//			//if( physicsModelInstance != null )
	//			//{
	//			//	physicsModelInstance.Dispose();
	//			//	physicsModelInstance = null;
	//			//}

	//			//!!!!!
	//			////for undo support
	//			//transformAllowCallOnSetTransformAndUpdatePhysicsModel = false;
	//		}
	//	}



	//	/////// <summary>
	//	/////// Adds entity in the world. This method is necessary call after entity is created and parameters are set.
	//	/////// </summary>
	//	/////// <example>This example shows creation of entity and his addition in the world.
	//	/////// <code>
	//	/////// MapObject obj = (MapObject)Entities.Instance.Create( "Box", Map.Instance );
	//	/////// obj.Position = new Vec3( 0, 0, 10 );
	//	/////// obj.EndCreation();
	//	/////// </code>
	//	/////// </example>
	//	/////// <seealso cref="NeoAxis.Map.MapObjectsClass.Create(EntityType,Entity)"/>
	//	/////// <seealso cref="NeoAxis.MapSystem.Map.MapObjectsClass.Create(string,MapObject)"/>
	//	/////// <seealso cref="NeoAxis.MapObject.OnBeginCreation(bool)"/>
	//	/////// <seealso cref="NeoAxis.MapObject.EndCreation()"/>
	//	/////// <seealso cref="NeoAxis.MapObject.PostCreated"/>
	//	//public void FinishCreation()
	//	//{
	//	//	//!!!!проверять где-то, чтобы был досоздан

	//	//	//if( IsCreated )
	//	//	//	Log.Fatal( "MapObject: EndCreation: This object is already created." );
	//	//	//if( IsDestroyed )
	//	//	//	Log.Fatal( "MapObject: EndCreation: This object is destroyed." );

	//	//	////!!!!!!так? имя сразу назначается? лучше поскорее
	//	//	//UpdateNameAfterCreation();

	//	//	//PerformCreationProcess();
	//	//	//created = true;

	//	//	//additional actions after creation
	//	//	//!!!!!в самое конце норм вызвать, почему бы и нет. с другой стороны, может кто-то при ините захочет заюзать.
	//	//	WorldBoundsCallUpdate();

	//	//	//PerformCreationFinished();

	//	//	//!!!!было
	//	//	//if( ParentMap.SimulationType_IsServer() && Type.NetworkType == EntityNetworkTypes.Synchronized )
	//	//	//   Server_SendPostCreateToClients( EntitySystemWorld.Instance.RemoteEntityWorlds );
	//	//}

	//	//!!!!!
	//	//internal void Server_SendPostCreateToClients( IList<RemoteEntityWorld> remoteEntityWorlds )
	//	//{
	//	//	SendDataWriter writer = BeginNetworkMessage( remoteEntityWorlds,
	//	//		typeof( MapObject ), (ushort)NetworkMessages.PostCreateToClient );
	//	//	EndNetworkMessage();
	//	//}

	//	//!!!!!
	//	//[NetworkReceive( NetworkDirections.ToClient, (ushort)NetworkMessages.PostCreateToClient )]
	//	//void Client_ReceivePostCreate( RemoteEntityWorld sender, ReceiveDataReader reader )
	//	//{
	//	//	if( !reader.Complete() )
	//	//		return;

	//	//	//for map loading, because has post created during loading
	//	//	if( IsPostCreated )
	//	//		return;

	//	//	PostCreate();
	//	//}

	//	//!!!!!
	//	///// <summary>
	//	///// Gets a value indicating whether the object the state of creation.
	//	///// </summary>
	//	//[Browsable( false )]
	//	//public bool IsCreated
	//	//{
	//	//	get { return created; }
	//	//}

	//	//!!!!!
	//	///// <summary>
	//	///// Gets a value indicating whether the object was added to queue for deletion.
	//	///// </summary>
	//	//[Browsable( false )]
	//	//public bool IsMustDestroy
	//	//{
	//	//	get { return mustDestroy; }
	//	//}

	//	//!!!!!
	//	///// <summary>
	//	///// Gets a value indicating whether the object is deleted.
	//	///// </summary>
	//	//[Browsable( false )]
	//	//public bool IsDestroyed
	//	//{
	//	//	get { return destroyed; }
	//	//}

	//	//!!!!!было
	//	//public event MapObjectDefaultEventDelegate CreationBegin;
	//	//public event MapObjectDefaultEventDelegate CreationProcess;
	//	//public event MapObjectDefaultEventDelegate CreationFinished;
	//	//public event MapObjectDefaultEventDelegate DestroyBegin;
	//	//public event MapObjectDefaultEventDelegate DestroyProcess;
	//	//public event MapObjectDefaultEventDelegate DestroyFinished;
	//	//public static event MapObjectDefaultEventDelegate AllMapObjects_CreationBegin;
	//	//public static event MapObjectDefaultEventDelegate AllMapObjects_CreationProcess;
	//	//public static event MapObjectDefaultEventDelegate AllMapObjects_CreationFinished;
	//	//public static event MapObjectDefaultEventDelegate AllMapObjects_DestroyBegin;
	//	//public static event MapObjectDefaultEventDelegate AllMapObjects_DestroyProcess;
	//	//public static event MapObjectDefaultEventDelegate AllMapObjects_DestroyFinished;

	//	///// <summary>
	//	///// Called when entity is created, but before initialization of resources.
	//	///// </summary>
	//	/////// <seealso cref="NeoAxis.MapObject.OnBeginCreation(bool)"/>
	//	/////// <seealso cref="NeoAxis.MapObject.OnPostCreate(bool)"/>
	//	/////// <seealso cref="NeoAxis.MapObject.PostCreate()"/>
	//	//protected virtual void OnCreationBegin()
	//	//{
	//	//	//layer = ParentMap.RootLayer;

	//	//	//!!!!!было
	//	//	//Log.Fatal( "impl. что с типами?" );
	//	//	////load fields from the type
	//	//	////!!!!!loaded?
	//	//	////if( !loaded && Type != null )
	//	//	//if( Type != null )
	//	//	//{
	//	//	//	if( Type.Data.IsAttributeExist( "transformEnabled" ) )
	//	//	//		transformEnabled = bool.Parse( Type.Data.GetAttribute( "transformEnabled" ) );
	//	//	//	if( Type.Data.IsAttributeExist( "physicsModelName" ) )
	//	//	//		physicsModelName = Type.Data.GetAttribute( "physicsModelName" );
	//	//	//}

	//	//	//!!!!!!тут?
	//	//	for( int n = 0; n < Components.Count; n++ )
	//	//		( Components[ n ] as Component_MapObjectChild )?.OnParentCreationBegin();
	//	//}

	//	//protected virtual void OnCreationProcess()
	//	//{
	//	//	////layer
	//	//	//if( CreationType == CreationTypes.Loading )
	//	//	//{
	//	//	//	string layerName = LastLoadingTextBlock.GetAttribute( "layer" );
	//	//	//	if( !string.IsNullOrEmpty( layerName ) )
	//	//	//		layer = ParentMap.GetLayerByPath( layerName );
	//	//	//}

	//	//	////destroy resources for the object and it's components
	//	//	////canEnabled = true;
	//	//	//NotifyEnabledInHierarchyChanged();

	//	//	//!!!!!было
	//	//	//if( !excludeFromWorld_excluded )
	//	//	//{
	//	//	//	Components_CreateDefinedInEntityType();
	//	//	//}

	//	//	//!!!!!!тут?
	//	//	for( int n = 0; n < Components.Count; n++ )
	//	//		( Components[ n ] as Component_MapObjectChild )?.OnParentCreationProcess();

	//	//	//!!!!!
	//	//	//if( !excludeFromWorld_excluded )
	//	//	//{
	//	//	//	if( remainingLifetime > 0 && !remainingLifetime_subscribedToTick )
	//	//	//	{
	//	//	//		SubscribeToTickEvent();
	//	//	//		remainingLifetime_subscribedToTick = true;
	//	//	//	}
	//	//	//}

	//	//	//transformAllowCallOnSetTransformAndUpdatePhysicsModel = true;

	//	//	//SetTransform( ref position, ref rotation, ref scale );
	//	//	//SetOldTransform( ref position, ref rotation, ref scale );

	//	//	//CreateAttachedObjectsFromType();

	//	//	//!!!!!было
	//	//	////!!!!!так?
	//	//	////load PhysicsModel data
	//	//	//if( ParentMap.SimulationStatus == Map.SimulationStatuses.AlreadySimulated && LastLoadingTextBlock != null )
	//	//	//{
	//	//	//	if( CreationType == CreationTypes.Loading && physicsModelInstance != null && physicsModelInstance.ModelDeclaration != null )
	//	//	//		physicsModelInstance.LoadWorldSerialization( LastLoadingTextBlock );
	//	//	//}

	//	//	//!!!!!тут?
	//	//	OnUpdateOldTransform();

	//	//	//!!!!было
	//	//	////!!!!!тут?
	//	//	////!!!!!slowly? we can update it not in the OnTick()
	//	//	//if( PhysicsModelInstance != null )
	//	//	//{
	//	//	//	foreach( Body body in PhysicsModelInstance.Bodies )
	//	//	//	{
	//	//	//		if( !body.Static )
	//	//	//		{
	//	//	//			SubscribeToTickEvent();
	//	//	//			break;
	//	//	//		}
	//	//	//	}
	//	//	//}

	//	//	//!!!!!networking. нужно
	//	//	//if( EntitySystemWorld.Instance.IsServer() &&
	//	//	//   Type.NetworkType == EntityNetworkTypes.Synchronized )
	//	//	//{
	//	//	//   Server_SendAttachedMapObjectsInfoToClients( EntitySystemWorld.Instance.RemoteEntityWorlds );
	//	//	//}

	//	//	//!!!!!нужно
	//	//	//client_attachedMapObjectsInfo = null;
	//	//}

	//	//protected virtual void OnCreationFinished()
	//	//{
	//	//	//!!!!!!тут?
	//	//	for( int n = 0; n < Components.Count; n++ )
	//	//		( Components[ n ] as Component_MapObjectChild )?.OnParentCreationFinished();
	//	//}

	//	//protected virtual void OnDestroyBegin()
	//	//{
	//	//	//!!!!!!тут?
	//	//	for( int n = 0; n < Components.Count; n++ )
	//	//		( Components[ n ] as Component_MapObjectChild )?.OnParentDestroyBegin();
	//	//}

	//	//protected virtual void OnDestroyProcess()
	//	//{
	//	//	xx xx;//название какое-то дурацкое

	//	//	//destroy resources for the object and it's components
	//	//	//canEnabled = false;
	//	//	NotifyEnabledInHierarchyChanged();

	//	//	//if( thisObjectIsAttachedTo_Object != null )
	//	//	//{
	//	//	//   for( int nComponent = 0; nComponent < Components.Count; nComponent++ )
	//	//	//   {
	//	//	//      MapObjectComponent_AttachObject component = thisObjectIsAttachedTo_Object.Components[ nComponent ] as MapObjectComponent_AttachObject;
	//	//	//      if( component != null )
	//	//	//      {
	//	//	//         if( component.GetObject() == this )
	//	//	//         {
	//	//	//				ff;
	//	//	//            break;
	//	//	//         }
	//	//	//      }
	//	//	//   }
	//	//	//   foreach( MapObjectAttachedObject attachedObject in thisObjectIsAttachedTo_Object.AttachedObjects )
	//	//	//   {
	//	//	//      MapObjectAttachedMapObject attachedMapObjectObject = attachedObject as MapObjectAttachedMapObject;
	//	//	//      if( attachedMapObjectObject != null )
	//	//	//      {
	//	//	//         if( attachedMapObjectObject.MapObject == this )
	//	//	//         {
	//	//	//            thisObjectIsAttachedTo_Object.Detach( attachedMapObjectObject );
	//	//	//            break;
	//	//	//         }
	//	//	//      }
	//	//	//   }
	//	//	//}

	//	//	//if( sceneGraphCanUseSceneGraph && hasTransform )
	//	//	//	ParentMap.SceneGraph_RemoveObject( this );

	//	//	//if( physicsModelInstance != null )
	//	//	//{
	//	//	//	physicsModelInstance.Dispose();
	//	//	//	physicsModelInstance = null;
	//	//	//}

	//	//	////for undo support
	//	//	//transformAllowCallOnSetTransformAndUpdatePhysicsModel = false;

	//	//	//sceneGraphCanUseSceneGraph = false;

	//	//	//!!!!!тут?
	//	//	for( int n = 0; n < Components.Count; n++ )
	//	//		( Components[ n ] as Component_MapObjectChild )?.OnParentDestroyProcess();

	//	//	//!!!!!не тут
	//	//	////remove from name dictionary
	//	//	//{
	//	//	//	MapObject obj;
	//	//	//	if( ParentMap.mapObjects.objectByName.TryGetValue( Name, out obj ) )
	//	//	//	{
	//	//	//		if( obj == this )
	//	//	//			ParentMap.mapObjects.objectByName.Remove( Name );
	//	//	//	}
	//	//	//}

	//	//	//!!!!!было: if( !excludeFromWorld_excluded )
	//	//	//{
	//	//	//	//subscriptions to deletion event
	//	//	//	if( subscriptionsToDeletionEvent != null && subscriptionsToDeletionEvent.Count != 0 )
	//	//	//	{
	//	//	//		//!!!!как быть с undo?
	//	//	//		//!!!!проверить

	//	//	//		MapObject[] objects = new MapObject[ subscriptionsToDeletionEvent.Count ];
	//	//	//		subscriptionsToDeletionEvent.Keys.CopyTo( objects, 0 );

	//	//	//		foreach( MapObject obj in objects )
	//	//	//			obj.CallDeleteSubscribedToDeletionEvent( this );

	//	//	//		foreach( MapObject obj in objects )
	//	//	//		{
	//	//	//			subscriptionsToDeletionEvent.Remove( obj );
	//	//	//			if( obj.subscriptionsToDeletionEvent != null )
	//	//	//				obj.subscriptionsToDeletionEvent.Remove( this );
	//	//	//		}
	//	//	//	}

	//	//	//	RemoveAllSubscribtionsToTickEvents();
	//	//	//}
	//	//}

	//	//protected virtual void OnDestroyFinished()
	//	//{
	//	//	//!!!!так?
	//	//	for( int n = 0; n < Components.Count; n++ )
	//	//		( Components[ n ] as Component_MapObjectChild )?.OnParentDestroyFinished();
	//	//}

	//	//internal void PerformCreationBegin()
	//	//{
	//	//	OnCreationBegin();
	//	//	CreationBegin?.Invoke( this );
	//	//	AllMapObjects_CreationBegin?.Invoke( this );
	//	//}

	//	//internal void PerformCreationProcess()
	//	//{
	//	//	OnCreationProcess();
	//	//	CreationProcess?.Invoke( this );
	//	//	AllMapObjects_CreationProcess?.Invoke( this );
	//	//}

	//	//internal void PerformCreationFinished()
	//	//{
	//	//	OnCreationFinished();
	//	//	CreationFinished?.Invoke( this );
	//	//	AllMapObjects_CreationFinished?.Invoke( this );
	//	//}

	//	//internal void PerformDestroyBegin()
	//	//{
	//	//	OnDestroyBegin();
	//	//	DestroyBegin?.Invoke( this );
	//	//	AllMapObjects_DestroyBegin?.Invoke( this );
	//	//}

	//	//internal void PerformDestroyProcess()
	//	//{
	//	//	OnDestroyProcess();
	//	//	DestroyProcess?.Invoke( this );
	//	//	AllMapObjects_DestroyProcess?.Invoke( this );
	//	//}

	//	//internal void PerformDestroyFinished()
	//	//{
	//	//	OnDestroyFinished();
	//	//	DestroyFinished?.Invoke( this );
	//	//	AllMapObjects_DestroyFinished?.Invoke( this );
	//	//}

	//	//!!!!!было
	//	//internal void UpdateNameAfterCreation()
	//	//{
	//	//	//fix empty name
	//	//	if( string.IsNullOrEmpty( Name ) && !string.IsNullOrEmpty( Type.Name ) )
	//	//		Name = ParentMap.mapObjects.GetUniqueObjectName( Path.GetFileName( Type.Name ) + "_", false );

	//	//	//check errors
	//	//	{
	//	//		MapObject obj;
	//	//		ParentMap.mapObjects.objectByName.TryGetValue( Name, out obj );
	//	//		if( obj != null )
	//	//		{
	//	//			if( obj != this )
	//	//			{
	//	//				//!!!!!было
	//	//				//if( loading && EntitySystemWorld.Instance.SerializationMode == SerializationModes.MapSceneFile )
	//	//				//{
	//	//				//   Log.Info( "Entity: Error: Another entity has duplicate name \"{0}\".", Name );
	//	//				//   name = "";
	//	//				//}
	//	//				//else

	//	//				//!!!!!good? может автоматом разруливать?
	//	//				Log.Error( "Entity: Another object has same name \"{0}\".", Name );
	//	//			}
	//	//			else
	//	//				return;
	//	//		}
	//	//	}

	//	//	ParentMap.mapObjects.objectByName[ Name ] = this;
	//	//}


	//	protected override void OnVisibleInHierarchyChanged()
	//	{
	//		base.OnVisibleInHierarchyChanged();

	//		//!!!!!!
	//	}
	//}
}
