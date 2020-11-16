// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Collections;
using System.Diagnostics;
using System.ComponentModel;
using System.Reflection;
using System.Drawing;
using System.Drawing.Design;
using NeoAxis.Editor;
using NeoAxis.Input;

namespace NeoAxis
{
	/// <summary>
	/// Base class of all UI controls.
	/// </summary>
	[ResourceFileExtension( "ui" )]
	[EditorDocumentWindow( typeof( UIControl_DocumentWindow ) )]
	//[EditorNewObjectSettings( typeof( UIControl_NewObjectSettings ) )]
	public class UIControl : Component, IComponent_VisibleInHierarchy//, IComponent_CanBeSelectedInHierarchy
	{
		//!!!!всё тут

		double createTime;

		//!!!!было
		////!!!!!? лучше потом заняться обновлением GUI системы. пока просто, чтобы заработало то, что есть.
		//Vec2 oldScreenSize = new Vec2( 1, 1 );
		//bool needResetOldScreenSize;

		//ColorValue backColor;
		////!!!!
		////!!!!backImage. тоже ссылкой и т.д. где еще. всё тут
		//string backTexture = "";
		////Texture backTexture;
		//Rect backTextureCoord = new Rect( 0, 0, 1, 1 );
		//bool backTextureTile;
		//CanvasRenderer.TextureFilteringMode backTextureFiltering = CanvasRenderer.TextureFilteringMode.Linear;

		//!!!!
		internal bool lastMousePositionInsideBounds;

		//!!!!!
		//!!!!!не инитится. было в UIControlWorld.CreateControlFromFile
		//string fileNameDeclared;
		//string fileNameCreated;

		////!!!!
		//bool lockEditorResizing;

		Rectangle? screenClipRectangle;
		bool pushedClipRectangleToGuiRenderer;

		bool pushedColorMultiplier;

		//!!!!!
		bool canClone = true;

		//string uiControlRole = "";

		//!!!!!!
		//bool parentControlManagerInitialized;
		//UIControlContainer parentControlManager;

		//special shader
		CanvasRenderer.ShaderItem specialShader;
		bool specialShaderApplyToChildren;
		bool pushedSpecialShaderToGuiRenderer_ApplyToChildren;
		bool pushedSpecialShaderToGuiRenderer_NotApplyToChildren;

		//cached screen position and size
		Rectangle? cachedScreenRectangle;
		//Vec2? cachedScreenPosition;
		//Vec2? cachedScreenSize;

		internal int cachedIndexInHierarchyToImplementCovering;

		///////////////////////////////////////////

		/// <summary>
		/// The text value of the control.
		/// </summary>
		[DefaultValue( "" )]
		[Category( "Common" )]
		public Reference<string> Text
		{
			get { if( _text.BeginGet() ) Text = _text.Get( this ); return _text.value; }
			set
			{
				if( value.Value == null )
					value.Value = "";
				if( _text.BeginSet( ref value ) ) { try { TextChanged?.Invoke( this ); } finally { _text.EndSet(); } }
			}
		}
		public event Action<UIControl> TextChanged;
		ReferenceField<string> _text = "";

		/// <summary>
		/// Whether the control is visible.
		/// </summary>
		[DefaultValue( true )]
		[Category( "Common" )]
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
		public event Action<UIControl> VisibleChanged;
		ReferenceField<bool> _visible = true;

		/// <summary>
		/// Whether the control is read-only.
		/// </summary>
		[DefaultValue( false )]
		[Category( "Common" )]
		public Reference<bool> ReadOnly
		{
			get { if( _readOnly.BeginGet() ) ReadOnly = _readOnly.Get( this ); return _readOnly.value; }
			set { if( _readOnly.BeginSet( ref value ) ) { try { ReadOnlyChanged?.Invoke( this ); } finally { _readOnly.EndSet(); } } }
		}
		public event Action<UIControl> ReadOnlyChanged;
		ReferenceField<bool> _readOnly = false;

		/// <summary>
		/// Whether the object is selectable in editor view.
		/// </summary>
		[DefaultValue( true )]
		[Category( "Common" )]//!!!!
		public Reference<bool> CanBeSelected
		{
			get { if( _canBeSelected.BeginGet() ) CanBeSelected = _canBeSelected.Get( this ); return _canBeSelected.value; }
			set { if( _canBeSelected.BeginSet( ref value ) ) { try { CanBeSelectedChanged?.Invoke( this ); } finally { _canBeSelected.EndSet(); } } }
		}
		public event Action<UIControl> CanBeSelectedChanged;
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
			}
		}

		public enum CoverOtherControlsEnum
		{
			None,
			AllPreviousInHierarchy,
			OnlyBehind,
		}

		[Browsable( false )]
		[Serialize( false )]
		public virtual CoverOtherControlsEnum CoverOtherControls
		{
			get { return coverOtherControls; }
			set { coverOtherControls = value; }
		}
		CoverOtherControlsEnum coverOtherControls = CoverOtherControlsEnum.None;

		////!!!!
		//[Browsable( false )]
		//protected virtual bool MouseCover
		//{
		//	get { return false; }
		//}
		////!!!!name: InputCover?
		//[DefaultValue( false )]
		//[Category( "Not Refactored" )]//[Category( "Common" )]
		//public Reference<bool> MouseCover
		//{
		//	get { if( _mouseCover.BeginGet() ) MouseCover = _mouseCover.Get( this ); return _mouseCover.value; }
		//	set { if( _mouseCover.BeginSet( ref value ) ) { try { MouseCoverChanged?.Invoke( this ); } finally { _mouseCover.EndSet(); } } }
		//}
		//public event Action<UIControl> MouseCoverChanged;
		//ReferenceField<bool> _mouseCover = false;

		//[DefaultValue( "Parent 1 1" )]
		/// <summary>
		/// The size of the control.
		/// </summary>
		[Category( "Layout" )]
		public Reference<UIMeasureValueVector2> Size
		{
			get { if( _size.BeginGet() ) Size = _size.Get( this ); return _size.value; }
			set
			{
				if( _size.BeginSet( ref value ) )
				{
					try
					{
						SizeChanged?.Invoke( this );
						ResetCachedScreenRectangleRecursive();
					}
					finally { _size.EndSet(); }
				}
			}
		}
		public event Action<UIControl> SizeChanged;
		ReferenceField<UIMeasureValueVector2> _size = new UIMeasureValueVector2( UIMeasure.Parent, 1, 1 );

		//[DefaultValue( ScaleType.Parent )]
		//[Category( "Layout" )]
		//public Reference<ScaleType> SizeType
		//{
		//	get { if( _sizeType.BeginGet() ) SizeType = _sizeType.Get( this ); return _sizeType.value; }
		//	set
		//	{
		//		if( _sizeType.BeginSet( ref value ) )
		//		{
		//			try
		//			{
		//				SizeTypeChanged?.Invoke( this );
		//				ResetCachedScreenPositionAndSizeRecursive();
		//			}
		//			finally { _sizeType.EndSet(); }
		//		}
		//	}
		//}
		//public event Action<UIControl> SizeTypeChanged;
		//ReferenceField<ScaleType> _sizeType = ScaleType.Parent;

		//[DefaultValue( "1 1" )]
		//[Category( "Layout" )]
		//public Reference<Vec2> SizeValue
		//{
		//	get { if( _sizeValue.BeginGet() ) SizeValue = _sizeValue.Get( this ); return _sizeValue.value; }
		//	set
		//	{
		//		if( _sizeValue.BeginSet( ref value ) )
		//		{
		//			try
		//			{
		//				SizeValueChanged?.Invoke( this );
		//				ResetCachedScreenPositionAndSizeRecursive();
		//			}
		//			finally { _sizeValue.EndSet(); }
		//		}
		//	}
		//}
		//public event Action<UIControl> SizeValueChanged;
		//ReferenceField<Vec2> _sizeValue = Vec2.One;

		/// <summary>
		/// The horizontal alignment of the control.
		/// </summary>
		[DefaultValue( EHorizontalAlignment.Left )]
		[Category( "Layout" )]
		public Reference<EHorizontalAlignment> HorizontalAlignment
		{
			get { if( _horizontalAlign.BeginGet() ) HorizontalAlignment = _horizontalAlign.Get( this ); return _horizontalAlign.value; }
			set
			{
				if( _horizontalAlign.BeginSet( ref value ) )
				{
					try
					{
						HorizontalAlignChanged?.Invoke( this );
						ResetCachedScreenRectangleRecursive();
					}
					finally { _horizontalAlign.EndSet(); }
				}
			}
		}
		public event Action<UIControl> HorizontalAlignChanged;
		ReferenceField<EHorizontalAlignment> _horizontalAlign = EHorizontalAlignment.Left;

		/// <summary>
		/// The vertical alignment of the control.
		/// </summary>
		[DefaultValue( EVerticalAlignment.Top )]
		[Category( "Layout" )]
		public Reference<EVerticalAlignment> VerticalAlignment
		{
			get { if( _verticalAlign.BeginGet() ) VerticalAlignment = _verticalAlign.Get( this ); return _verticalAlign.value; }
			set
			{
				if( _verticalAlign.BeginSet( ref value ) )
				{
					try
					{
						VerticalAlignChanged?.Invoke( this );
						ResetCachedScreenRectangleRecursive();
					}
					finally { _verticalAlign.EndSet(); }
				}
			}
		}
		public event Action<UIControl> VerticalAlignChanged;
		ReferenceField<EVerticalAlignment> _verticalAlign = EVerticalAlignment.Top;

		/// <summary>
		/// The margin of the control.
		/// </summary>
		[DefaultValue( "Parent 0 0 0 0" )]
		[Category( "Layout" )]
		public Reference<UIMeasureValueRectangle> Margin
		{
			get { if( _margin.BeginGet() ) Margin = _margin.Get( this ); return _margin.value; }
			set
			{
				if( _margin.BeginSet( ref value ) )
				{
					try
					{
						MarginChanged?.Invoke( this );
						ResetCachedScreenRectangleRecursive();
					}
					finally { _margin.EndSet(); }
				}
			}
		}
		public event Action<UIControl> MarginChanged;
		ReferenceField<UIMeasureValueRectangle> _margin;

		//[DefaultValue( ScaleType.Parent )]
		//[Category( "Layout" )]
		//public Reference<ScaleType> MarginType
		//{
		//	get { if( _marginType.BeginGet() ) MarginType = _marginType.Get( this ); return _marginType.value; }
		//	set
		//	{
		//		if( _marginType.BeginSet( ref value ) )
		//		{
		//			try
		//			{
		//				MarginTypeChanged?.Invoke( this );
		//				ResetCachedScreenRectangleRecursive();
		//			}
		//			finally { _marginType.EndSet(); }
		//		}
		//	}
		//}
		//public event Action<UIControl> MarginTypeChanged;
		//ReferenceField<ScaleType> _marginType = ScaleType.Parent;

		//[DefaultValue( "0 0 0 0" )]
		//[Category( "Layout" )]
		//public Reference<Rect> MarginValue
		//{
		//	get { if( _marginValue.BeginGet() ) MarginValue = _marginValue.Get( this ); return _marginValue.value; }
		//	set
		//	{
		//		if( _marginValue.BeginSet( ref value ) )
		//		{
		//			try
		//			{
		//				MarginValueChanged?.Invoke( this );
		//				ResetCachedScreenRectangleRecursive();
		//			}
		//			finally { _marginValue.EndSet(); }
		//		}
		//	}
		//}
		//public event Action<UIControl> MarginValueChanged;
		//ReferenceField<Rect> _marginValue;

		///// <summary>
		///// Specifies the the ordering of overlapping controls.
		///// </summary>
		//[DefaultValue( 0 )]
		//[DisplayName( "Z Order" )]
		//[Category( "Layout" )]
		//public Reference<int> ZOrder
		//{
		//	get { if( _zOrder.BeginGet() ) ZOrder = _zOrder.Get( this ); return _zOrder.value; }
		//	set { if( _zOrder.BeginSet( ref value ) ) { try { ZOrderChanged?.Invoke( this ); } finally { _zOrder.EndSet(); } } }
		//}
		//public event Action<UIControl> ZOrderChanged;
		//ReferenceField<int> _zOrder = 0;

		/// <summary>
		/// Whether the control is rendered on the top.
		/// </summary>
		[DefaultValue( false )]
		[Category( "Layout" )]
		public Reference<bool> TopMost
		{
			get { if( _topMost.BeginGet() ) TopMost = _topMost.Get( this ); return _topMost.value; }
			set { if( _topMost.BeginSet( ref value ) ) { try { TopMostChanged?.Invoke( this ); } finally { _topMost.EndSet(); } } }
		}
		public event Action<UIControl> TopMostChanged;
		ReferenceField<bool> _topMost = false;

		///////////////////////////////////////////

		//[Editor( typeof( UIControlScaleValueEditor ), typeof( UITypeEditor ) )]
		//[TypeConverter( typeof( ExpandableObjectConverter ) )]
		//public struct ScaleValue
		//{
		//	ScaleType type;
		//	internal Vec2 value;

		//	public ScaleValue( ScaleType type, Vec2 value )
		//	{
		//		this.type = type;
		//		this.value = value;
		//	}

		//	public ScaleType Type
		//	{
		//		get { return type; }
		//		set { type = value; }
		//	}

		//	public Vec2 Value
		//	{
		//		get { return this.value; }
		//		set { this.value = value; }
		//	}

		//	public static ScaleValue Parse( string text )
		//	{
		//		if( text == null || text.Length == 0 )
		//			throw new ArgumentNullException( "The parsable Text parameter cannot be null or zero length." );

		//		string[] vals = text.Split( ' ' );
		//		if( vals.Length != 3 )
		//			throw new FormatException( string.Format( "Cannot parse the text '{0}' because it does not have 3 parts separated by spaces in the form (type x y) with optional parenthesis.", text ) );

		//		try
		//		{
		//			ScaleType t;
		//			//!!!!
		//			if( vals[ 0 ] == "Texture" )//backwards compatibility
		//				t = ScaleType.ScaleByResolution;
		//			else
		//				t = (ScaleType)Enum.Parse( typeof( ScaleType ), vals[ 0 ] );

		//			double x = double.Parse( vals[ 1 ] );
		//			double y = double.Parse( vals[ 2 ] );

		//			return new ScaleValue( t, new Vec2( x, y ) );

		//			//return new ScaleValue( (ScaleType)Enum.Parse( typeof( ScaleType ), vals[ 0 ] ),
		//			//   Vec2.Parse( vals[ 1 ] + " " + vals[ 2 ] ) );
		//		}
		//		catch( Exception )
		//		{
		//			throw new FormatException( "Invalid format." );
		//		}
		//	}

		//	public override string ToString()
		//	{
		//		return type.ToString() + " " + value.ToString();
		//	}

		//	public override bool Equals( object obj )
		//	{
		//		return ( obj is ScaleValue && this == (ScaleValue)obj );
		//	}

		//	public override int GetHashCode()
		//	{
		//		return ( type.GetHashCode() ^ value.GetHashCode() );
		//	}

		//	public static bool operator ==( ScaleValue v1, ScaleValue v2 )
		//	{
		//		return ( v1.type == v2.type && v1.value == v2.value );
		//	}

		//	public static bool operator !=( ScaleValue v1, ScaleValue v2 )
		//	{
		//		return ( v1.type != v2.type || v1.value != v2.value );
		//	}
		//}

		///////////////////////////////////////////

		//[Flags]
		//public enum AnchorTypes
		//{
		//	None = 0,
		//	Left = 1,
		//	Bottom = 2,
		//	Top = 4,
		//	Right = 8,
		//}

		///////////////////////////////////////////

		//public sealed class StandardChildSlotItem
		//{
		//	string name;
		//	UIControl control;

		//	public StandardChildSlotItem( string name, UIControl control )
		//	{
		//		this.name = name;
		//		this.control = control;
		//	}

		//	public string Name
		//	{
		//		get { return name; }
		//	}

		//	public UIControl Control
		//	{
		//		get { return control; }
		//	}
		//}

		///////////////////////////////////////////

		//public class AvailableUIControlRoleItem
		//{
		//	string name = "";
		//	string description = "";

		//	public AvailableUIControlRoleItem( string name, string description )
		//	{
		//		this.name = name;
		//		this.description = description;
		//	}

		//	public string Name
		//	{
		//		get { return name; }
		//	}

		//	public string Description
		//	{
		//		get { return description; }
		//	}

		//	public override string ToString()
		//	{
		//		return string.Format( "{0} - {1}", Name, Description );
		//	}
		//}

		///////////////////////////////////////////

		public UIControl()
		{
			ResetCreateTime();
		}

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
		//public event Action<Component> VisibleInHierarchyChanged;

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

		//!!!!!так? или как Enabled, Visible?
		/// <summary>
		/// Whether the control is read-only in hierarchy.
		/// </summary>
		[Browsable( false )]
		public bool ReadOnlyInHierarchy
		{
			get
			{
				UIControl c = this;
				do
				{
					if( c.ReadOnly )
						return true;
					c = c.Parent as UIControl;
				} while( c != null );
				return false;
			}
		}

		[Browsable( false )]
		public double Time
		{
			get
			{
				if( EngineApp.Instance == null )
					return 0;
				return EngineApp.EngineTime - createTime;
			}
		}

		public void ResetCreateTime()
		{
			if( EngineApp.Instance != null )
				createTime = EngineApp.EngineTime;
		}

		public delegate void ControlDefaultEventDelegate( UIControl sender );

		/// <summary>
		/// The style of the control.
		/// </summary>
		[DefaultValue( null )]
		[Category( "Appearance" )]
		public Reference<UIStyle> Style
		{
			get { if( _style.BeginGet() ) Style = _style.Get( this ); return _style.value; }
			set { if( _style.BeginSet( ref value ) ) { try { StyleChanged?.Invoke( this ); } finally { _style.EndSet(); } } }
		}
		public event Action<UIControl> StyleChanged;
		ReferenceField<UIStyle> _style = null;

		/// <summary>
		/// The background color of the control.
		/// </summary>
		[DefaultValue( "0 0 0 0" )]
		[Category( "Appearance" )]
		public Reference<ColorValue> BackgroundColor
		{
			get { if( _backgroundColor.BeginGet() ) BackgroundColor = _backgroundColor.Get( this ); return _backgroundColor.value; }
			set { if( _backgroundColor.BeginSet( ref value ) ) { try { BackgroundColorChanged?.Invoke( this ); } finally { _backgroundColor.EndSet(); } } }
		}
		public event Action<UIControl> BackgroundColorChanged;
		ReferenceField<ColorValue> _backgroundColor = ColorValue.Zero;

		/// <summary>
		/// The extra color multiplier applied to the control.
		/// </summary>
		[DefaultValue( "1 1 1" )]
		[Category( "Appearance" )]
		public Reference<ColorValue> ColorMultiplier
		{
			get { if( _colorMultiplier.BeginGet() ) ColorMultiplier = _colorMultiplier.Get( this ); return _colorMultiplier.value; }
			set { if( _colorMultiplier.BeginSet( ref value ) ) { try { ColorMultiplierChanged?.Invoke( this ); } finally { _colorMultiplier.EndSet(); } } }
		}
		public event Action<UIControl> ColorMultiplierChanged;
		ReferenceField<ColorValue> _colorMultiplier = new ColorValue( 1, 1, 1 );

		///// <summary>
		///// The position of the control.
		///// </summary>
		//[Category( "Layout" )]
		//public ScaleValue Position
		//{
		//	get { return position; }
		//	set
		//	{
		//		if( position == value )
		//			return;

		//		position = value;

		//		ResetCachedScreenPositionAndSizeRecursive();
		//	}
		//}

		///// <summary>
		///// The horizontal alignment of the control.
		///// </summary>
		//[DefaultValue( EHorizontalAlignment.Left )]
		//[Category( "Layout" )]
		//public EHorizontalAlignment HorizontalAlign
		//{
		//	get { return horizontalAlign; }
		//	set
		//	{
		//		if( horizontalAlign == value )
		//			return;

		//		horizontalAlign = value;

		//		ResetCachedScreenPositionAndSizeRecursive();
		//	}
		//}

		///// <summary>
		///// The vertical alignment of the control.
		///// </summary>
		//[DefaultValue( EVerticalAlignment.Top )]
		//[Category( "Layout" )]
		//public EVerticalAlignment VerticalAlign
		//{
		//	get { return verticalAlign; }
		//	set
		//	{
		//		if( verticalAlign == value )
		//			return;

		//		verticalAlign = value;

		//		ResetCachedScreenPositionAndSizeRecursive();
		//	}
		//}

		//public event ControlDefaultEventDelegate SizeChanged;

		///// <summary>
		///// The size of the control.
		///// </summary>
		//[Category( "Layout" )]
		//public ScaleValue Size
		//{
		//	get { return size; }
		//	set
		//	{
		//		if( size == value )
		//			return;

		//		oldScreenSize = GetScreenSize();
		//		size = value;

		//		ResetCachedScreenPositionAndSizeRecursive();

		//		if( needResetOldScreenSize )
		//			oldScreenSize = GetScreenSize();

		//		OnResize();
		//		if( SizeChanged != null )
		//			SizeChanged( this );
		//	}
		//}

		///// <summary>
		///// The alignment anchor of the control.
		///// </summary>
		//[DefaultValue( AnchorTypes.None )]
		//[Category( "Layout" )]
		//[Editor( typeof( UIControlAnchorEditor ), typeof( UITypeEditor ) )]
		//public AnchorTypes Anchor
		//{
		//	get { return anchor; }
		//	set
		//	{
		//		if( anchor == value )
		//			return;

		//		anchor = value;

		//		ResetCachedScreenPositionAndSizeRecursive();
		//	}
		//}

		public void GetScreenRectangle( out Rectangle result )
		{
			if( cachedScreenRectangle == null )
			{
				var parent = ParentControl;

				//Vec2 position;
				Vector2 size;

				switch( Size.Value.Measure )
				{
				case UIMeasure.Parent:
					if( parent != null )
					{
						parent.GetScreenSize( out var parentSize );
						size = Size.Value.Value * parentSize;
						//Vec2.Multiply( ref SizeValue, ref parentSize, out s );
					}
					else
						size = Size.Value.Value;
					break;

				case UIMeasure.Screen:
					size = Size.Value.Value;
					break;

				case UIMeasure.Units:
					GetParentContainerSizeInUnits( out var baseSize );
					size = Size.Value.Value / baseSize;
					//Vec2.Divide( ref size.value, ref baseSize, out s );
					break;

				case UIMeasure.Pixels:
					Vector2 screenPixelSize = GetParentContainerSizeInPixels();
					size = Size.Value.Value / screenPixelSize;
					//Vec2.Divide( ref size.value, ref screenPixelSize, out s );
					break;

				default:
					size = Vector2.Zero;
					break;
				}

				var marginMeasure = Margin.Value.Measure;
				var marginValue = Margin.Value.Value;

				double positionX = 0;
				double positionY = 0;

				//if( marginType == ScaleType.Screen )
				//{
				//	switch( HorizontalAlignment.Value )
				//	{
				//	case EHorizontalAlignment.Left:
				//		positionX = marginValue.Left;
				//		break;
				//	case EHorizontalAlignment.Center:
				//		positionX = ( marginValue.Left + ( 1.0 - marginValue.Right ) ) / 2 - size.X / 2;
				//		break;
				//	case EHorizontalAlignment.Right:
				//		positionX = 1.0 - marginValue.Right - size.X;
				//		break;
				//	case EHorizontalAlignment.Stretch:
				//		positionX = marginValue.Left;
				//		size.X = 1.0 - marginValue.Right - positionX;
				//		break;
				//	}

				//	switch( VerticalAlignment.Value )
				//	{
				//	case EVerticalAlignment.Top:
				//		positionY = marginValue.Top;
				//		break;
				//	case EVerticalAlignment.Center:
				//		positionY = ( marginValue.Top + ( 1.0 - marginValue.Bottom ) ) / 2 - size.Y / 2;
				//		break;
				//	case EVerticalAlignment.Bottom:
				//		positionY = 1.0 - marginValue.Bottom - size.Y;
				//		break;
				//	case EVerticalAlignment.Stretch:
				//		positionY = marginValue.Top;
				//		size.Y = 1.0 - marginValue.Bottom - positionY;
				//		break;
				//	}
				//}
				//else
				//{
				var posFrom = parent != null ? parent.GetScreenPosition() : Vector2.Zero;
				var posTo = parent != null ? parent.GetScreenPosition() + parent.GetScreenSize() : Vector2.One;

				switch( marginMeasure )
				{
				case UIMeasure.Parent:
					if( parent != null )
					{
						parent.GetScreenSize( out var s );
						posFrom += marginValue.LeftTop * s;
						posTo -= marginValue.RightBottom * s;
					}
					break;

				case UIMeasure.Screen:
					posFrom = marginValue.LeftTop;
					posTo = Vector2.One - marginValue.RightBottom;
					break;

				case UIMeasure.Units:
					posFrom += marginValue.LeftTop / GetParentContainerSizeInUnits();
					posTo -= marginValue.RightBottom / GetParentContainerSizeInUnits();
					break;

				case UIMeasure.Pixels:
					posFrom += marginValue.LeftTop / GetParentContainerSizeInPixels();
					posTo -= marginValue.RightBottom / GetParentContainerSizeInPixels();
					break;
				}

				switch( HorizontalAlignment.Value )
				{
				case EHorizontalAlignment.Left:
					positionX = posFrom.X;
					break;
				case EHorizontalAlignment.Center:
					positionX = ( posFrom.X + posTo.X ) / 2 - size.X / 2;
					//positionX = ( posFrom.X + ( 1.0 - posTo.X ) ) / 2 - size.X / 2;
					break;
				case EHorizontalAlignment.Right:
					positionX = posTo.X - size.X;
					//positionX = 1.0 - posTo.X - size.X;
					break;
				case EHorizontalAlignment.Stretch:
					positionX = posFrom.X;
					size.X = posTo.X - positionX;
					//size.X = 1.0 - posTo.X - positionX;
					break;
				}

				switch( VerticalAlignment.Value )
				{
				case EVerticalAlignment.Top:
					positionY = posFrom.Y;
					break;
				case EVerticalAlignment.Center:
					positionY = ( posFrom.Y + posTo.Y ) / 2 - size.Y / 2;
					//positionY = ( posFrom.Y + ( 1.0 - posTo.Y ) ) / 2 - size.Y / 2;
					break;
				case EVerticalAlignment.Bottom:
					positionY = posTo.Y - size.Y;
					//positionY = 1.0 - posTo.Y - size.Y;
					break;
				case EVerticalAlignment.Stretch:
					positionY = posFrom.Y;
					size.Y = posTo.Y - positionY;
					//size.Y = 1.0 - posTo.Y - positionY;
					break;
				}
				//}

				cachedScreenRectangle = new Rectangle( new Vector2( positionX, positionY ), new Vector2( positionX, positionY ) + size );
			}

			result = cachedScreenRectangle.Value;
		}

		public Rectangle GetScreenRectangle()
		{
			GetScreenRectangle( out var rect );
			return rect;
		}

		//internal void GetScreenPosition2( out Vec2 result )
		//{
		//	if( cachedScreenPosition == null )
		//	{
		//		UIControl parent = ParentControl;

		//		Vec2 pos;

		//		//position
		//		switch( position.Type )
		//		{
		//		case ScaleType.Parent:
		//			pos = position.value;
		//			if( parent != null )
		//			{
		//				parent.GetScreenSize( out var s );
		//				parent.GetScreenPosition( out var p );
		//				pos = new Vec2( pos.X * s.X + p.X, pos.Y * s.Y + p.Y );
		//				//p *= parent.GetScreenSize();
		//				//p += parent.GetScreenPosition();
		//			}
		//			break;

		//		case ScaleType.Screen:
		//			pos = position.Value;
		//			break;

		//		case ScaleType.ScaleByResolution:
		//			pos = position.Value / GetScreenTextureBaseSize();
		//			if( parent != null )
		//				pos += parent.GetScreenPosition();
		//			break;

		//		case ScaleType.Pixel:
		//			pos = position.Value / GetScreenPixelSize();
		//			if( parent != null )
		//				pos += parent.GetScreenPosition();
		//			break;

		//		default:
		//			pos = Vec2.Zero;
		//			break;
		//		}

		//		if( horizontalAlign != EHorizontalAlignment.Left || verticalAlign != EVerticalAlignment.Top )
		//		{
		//			Vec2 parentSize;
		//			Vec2 s;

		//			switch( size.Type )
		//			{
		//			case ScaleType.Parent:
		//				parentSize = new Vec2( 1, 1 );
		//				s = ( parent != null ) ? Size.Value : new Vec2( 1, 1 );
		//				break;

		//			case ScaleType.Screen:
		//				parentSize = ( parent != null ) ? parent.GetScreenSize() : new Vec2( 1, 1 );
		//				s = GetScreenSize();
		//				break;

		//			case ScaleType.ScaleByResolution:
		//				parentSize = ( parent != null ) ? parent.GetScreenSize() : new Vec2( 1, 1 );
		//				s = GetScreenSize();
		//				break;

		//			case ScaleType.Pixel:
		//				parentSize = ( parent != null ) ? parent.GetScreenSize() : new Vec2( 1, 1 );
		//				s = GetScreenSize();
		//				break;

		//			default:
		//				throw new Exception( "GetScreenPosition invalid scaleType" );
		//			}

		//			Vec2 p = Vec2.Zero;

		//			if( horizontalAlign == EHorizontalAlignment.Center )
		//				p.X += parentSize.X * .5f - s.X * .5f;
		//			else if( horizontalAlign == EHorizontalAlignment.Right )
		//				p.X += parentSize.X - s.X;
		//			if( verticalAlign == EVerticalAlignment.Center )
		//				p.Y += parentSize.Y * .5f - s.Y * .5f;
		//			else if( verticalAlign == EVerticalAlignment.Bottom )
		//				p.Y += parentSize.Y - s.Y;

		//			switch( size.Type )
		//			{
		//			case ScaleType.Parent:
		//				if( parent != null )
		//					p *= parent.GetScreenSize();
		//				break;
		//			}

		//			pos += p;
		//		}

		//		cachedScreenPosition = pos;
		//	}

		//	result = cachedScreenPosition.Value;
		//}

		//internal void GetScreenSize2( out Vec2 result )
		//{
		//	if( cachedScreenSize == null )
		//	{
		//		UIControl parent = ParentControl;

		//		Vec2 s;

		//		switch( SizeType.Value )
		//		{
		//		case ScaleType.Parent:
		//			if( parent != null )
		//			{
		//				parent.GetScreenSize( out var parentSize );
		//				s = SizeValue * parentSize;
		//				//Vec2.Multiply( ref SizeValue, ref parentSize, out s );
		//			}
		//			else
		//				s = SizeValue;
		//			break;

		//		case ScaleType.Screen:
		//			s = SizeValue;
		//			break;

		//		case ScaleType.ScaleByResolution:
		//			GetScreenTextureBaseSize( out var baseSize );
		//			s = SizeValue / baseSize;
		//			//Vec2.Divide( ref size.value, ref baseSize, out s );
		//			break;

		//		case ScaleType.Pixel:
		//			Vec2 screenPixelSize = GetScreenPixelSize();
		//			s = SizeValue / screenPixelSize;
		//			//Vec2.Divide( ref size.value, ref screenPixelSize, out s );
		//			break;

		//		default:
		//			s = Vec2.Zero;
		//			break;
		//		}

		//		cachedScreenSize = s;
		//	}

		//	result = cachedScreenSize.Value;
		//}

		internal void GetScreenPosition( out Vector2 result )
		{
			GetScreenRectangle( out var rect );
			result = rect.LeftTop;
		}

		public Vector2 GetScreenPosition()
		{
			GetScreenRectangle( out var rect );
			return rect.LeftTop;
		}

		internal void GetScreenSize( out Vector2 result )
		{
			GetScreenRectangle( out var rect );
			result = rect.Size;
		}

		public Vector2 GetScreenSize()
		{
			GetScreenRectangle( out var rect );
			return rect.Size;
		}

		/////////////////////////////////////////

		public Vector2 ConvertScreenToLocal( Vector2 point )
		{
			Vector2 pos = point - GetScreenPosition();
			GetScreenSize( out var screenSize );
			if( screenSize.X != 0 )
				pos.X /= screenSize.X;
			else
				pos.X = 0;
			if( screenSize.Y != 0 )
				pos.Y /= screenSize.Y;
			else
				pos.Y = 0;
			return pos;
		}

		public Vector2 ConvertLocalToScreen( Vector2 point )
		{
			GetScreenRectangle( out var rectangle );
			var p = rectangle.LeftTop;
			var s = rectangle.Size;
			//GetScreenPosition( out var p );
			//GetScreenSize( out var s );
			return new Vector2( point.X * s.X + p.X, point.Y * s.Y + p.Y );
		}

		public Rectangle ConvertScreenToLocal( Rectangle rectangle )
		{
			return new Rectangle(
				ConvertScreenToLocal( rectangle.LeftTop ),
				ConvertScreenToLocal( rectangle.RightBottom ) );
		}

		public Rectangle ConvertLocalToScreen( Rectangle rectangle )
		{
			return new Rectangle(
				ConvertLocalToScreen( rectangle.LeftTop ),
				ConvertLocalToScreen( rectangle.RightBottom ) );
		}

		/////////////////////////////////////////

		//public Vector2 GetLocalOffsetByValue( UIMeasureValueVector2 sizeValue )
		//{
		//	if( sizeValue.Value == Vector2.Zero )
		//		return Vector2.Zero;

		//	switch( sizeValue.Measure )
		//	{
		//	case UIMeasure.Parent:
		//		return sizeValue.Value;

		//	case UIMeasure.Screen:
		//		{
		//			Vector2 v = sizeValue.Value;
		//			GetScreenSize( out var screenSize );

		//			if( screenSize.X != 0 )
		//				v.X /= screenSize.X;
		//			else
		//				v.X = 0;
		//			if( screenSize.Y != 0 )
		//				v.Y /= screenSize.Y;
		//			else
		//				v.Y = 0;

		//			return v;
		//		}

		//	case UIMeasure.Units:
		//		{
		//			Vector2 v = sizeValue.Value;
		//			GetScreenSize( out var screenSize );

		//			if( screenSize.X != 0 )
		//				v.X /= screenSize.X;
		//			else
		//				v.X = 0;
		//			if( screenSize.Y != 0 )
		//				v.Y /= screenSize.Y;
		//			else
		//				v.Y = 0;

		//			return v / GetParentContainerSizeInUnits();
		//		}

		//	case UIMeasure.Pixels:
		//		{
		//			Vector2 v = sizeValue.Value;
		//			GetScreenSize( out var screenSize );

		//			if( screenSize.X != 0 )
		//				v.X /= screenSize.X;
		//			else
		//				v.X = 0;
		//			if( screenSize.Y != 0 )
		//				v.Y /= screenSize.Y;
		//			else
		//				v.Y = 0;

		//			return v / GetParentContainerSizeInPixels();
		//		}

		//	default:
		//		Trace.Assert( false );
		//		return Vector2.Zero;
		//	}
		//}

		////!!!!new
		//public Vector2 GetScreenOffsetByValue( UIMeasureValueVector2 sizeValue )
		//{
		//	if( sizeValue.Value == Vector2.Zero )
		//		return Vector2.Zero;

		//	//!!!!xx
		//	{
		//		switch( sizeValue.Measure )
		//		{
		//		case UIMeasure.Screen:
		//			return sizeValue.Value;

		//		case UIMeasure.Units:
		//			return sizeValue.Value / GetParentContainerSizeInUnits();
		//		}
		//	}

		//	return ConvertLocalToScreen( GetLocalOffsetByValue( sizeValue ) );
		//}

		//public Vector2 ConvertLocalOffsetToSpecifiedMeasure( Vector2 localOffset, UIMeasure measure )
		//{
		//	switch( measure )
		//	{
		//	case UIMeasure.Parent:
		//		return localOffset;
		//	case UIMeasure.Screen:
		//		return localOffset * GetScreenSize();
		//	case UIMeasure.Units:
		//		return localOffset * GetScreenSize() * GetParentContainerSizeInUnits();
		//	case UIMeasure.Pixels:
		//		return localOffset * GetScreenSize() * GetParentContainerSizeInPixels();
		//	default:
		//		Trace.Assert( false );
		//		return Vector2.Zero;
		//	}
		//}

		Vector2 DivideWithZeroCheck( Vector2 v1, Vector2 v2 )
		{
			Vector2 v = Vector2.Zero;
			if( v2.X != 0 )
				v.X = v1.X / v2.X;
			if( v2.Y != 0 )
				v.Y = v1.Y / v2.Y;
			return v;
		}

		public Vector2 ConvertOffset( UIMeasureValueVector2 value, UIMeasure toMeasure )
		{
			if( value.Value == Vector2.Zero )
				return Vector2.Zero;

			Vector2 screen = Vector2.Zero;

			//from
			switch( value.Measure )
			{
			case UIMeasure.Parent:
				screen = value.Value * GetScreenSize();
				break;
			case UIMeasure.Screen:
				screen = value.Value;
				break;
			case UIMeasure.Units:
				screen = DivideWithZeroCheck( value.Value, GetParentContainerSizeInUnits() );
				break;
			case UIMeasure.Pixels:
				screen = DivideWithZeroCheck( value.Value, GetParentContainerSizeInPixels() );
				break;
			}

			//to
			switch( toMeasure )
			{
			case UIMeasure.Parent:
				return DivideWithZeroCheck( screen, GetScreenSize() );
			case UIMeasure.Screen:
				return screen;
			case UIMeasure.Units:
				return screen * GetParentContainerSizeInUnits();
			case UIMeasure.Pixels:
				return screen * GetParentContainerSizeInPixels();
			default:
				return Vector2.Zero;
			}
		}

		public double ConvertOffsetX( UIMeasureValueDouble value, UIMeasure toMeasure )
		{
			return ConvertOffset( new UIMeasureValueVector2( value.Measure, value.Value, 0 ), toMeasure ).X;
		}

		public double ConvertOffsetY( UIMeasureValueDouble value, UIMeasure toMeasure )
		{
			return ConvertOffset( new UIMeasureValueVector2( value.Measure, 0, value.Value ), toMeasure ).Y;
		}

		public Vector2 GetLocalOffsetByValue( UIMeasureValueVector2 value )
		{
			return ConvertOffset( value, UIMeasure.Parent );
		}

		public Vector2 GetScreenOffsetByValue( UIMeasureValueVector2 value )
		{
			return ConvertOffset( value, UIMeasure.Screen );
		}

		public double GetScreenOffsetByValueX( UIMeasureValueDouble value )
		{
			return ConvertOffsetX( value, UIMeasure.Screen );
		}

		public double GetScreenOffsetByValueY( UIMeasureValueDouble value )
		{
			return ConvertOffsetY( value, UIMeasure.Screen );
		}

		/////////////////////////////////////////

		////!!!!
		//internal UIControl GetTopMouseCoversControlRecursive()
		//{
		//	//!!!!GetComponentsSortedByZOrder
		//	foreach( UIControl control in GetComponents<UIControl>( true ) )
		//	{
		//		if( control.EnabledInHierarchy && !control.RemoveFromParentQueued && control.Visible )
		//		{
		//			UIControl r = control.GetTopMouseCoversControlRecursive();
		//			if( r != null )
		//				return r;
		//		}
		//	}

		//	//if( CoverOtherControls == CoverOtherControlsEnum.OnlyBehind && ( new Rectangle( 0, 0, 1, 1 ).Contains( MousePosition ) || MouseRelativeMode ) )
		//	//	return true;
		//	//if( CoverOtherControls == CoverOtherControlsEnum.AllPreviousInHierarchy )
		//	//	return true;

		//	//!!!!
		//	if( MouseCover && new Rectangle( 0, 0, 1, 1 ).Contains( MousePosition ) )
		//		return this;

		//	return null;
		//}

		UIControl[] GetControls( bool reverse )//UIControl[] GetComponentsSortedByZOrder( bool reverse )
		{
			return GetComponents<UIControl>( reverse );

			//var controls = GetComponents<UIControl>();

			//CollectionUtility.InsertionSort( controls, delegate ( UIControl c1, UIControl c2 )
			//{
			//	var order1 = c1.ZOrder.Value;
			//	var order2 = c2.ZOrder.Value;

			//	if( reverse )
			//	{
			//		if( order1 > order2 )
			//			return -1;
			//		if( order1 < order2 )
			//			return 1;
			//	}
			//	else
			//	{
			//		if( order1 < order2 )
			//			return -1;
			//		if( order1 > order2 )
			//			return 1;
			//	}

			//	return 0;
			//} );

			//return controls;
		}

		/////////////////////////////////////////

		protected virtual bool OnKeyDownBefore( KeyEvent e ) { return false; }
		protected virtual bool OnKeyDown( KeyEvent e ) { return false; }

		public delegate void KeyDownDelegate( UIControl sender, KeyEvent e, ref bool handled );
		public event KeyDownDelegate KeyDownBefore;
		public event KeyDownDelegate KeyDown;

		internal bool CallKeyDown( KeyEvent e )
		{
			foreach( UIControl control in GetControls( true ) )
			{
				//!!!!!так проверять?
				if( control.EnabledInHierarchy && !control.RemoveFromParentQueued && control.Visible )
					if( control.CallKeyDown( e ) )
						return true;
			}

			if( OnKeyDownBefore( e ) )
				return true;

			bool handled = false;
			KeyDownBefore?.Invoke( this, e, ref handled );
			if( handled )
				return true;

			if( OnKeyDown( e ) )
				return true;

			KeyDown?.Invoke( this, e, ref handled );
			if( handled )
				return true;

			return false;
		}

		/////////////////////////////////////////

		protected virtual bool OnKeyPressBefore( KeyPressEvent e ) { return false; }
		protected virtual bool OnKeyPress( KeyPressEvent e ) { return false; }

		public delegate void KeyPressDelegate( UIControl sender, KeyPressEvent e, ref bool handled );
		public event KeyPressDelegate KeyPressBefore;
		public event KeyPressDelegate KeyPress;

		internal bool CallKeyPress( KeyPressEvent e )
		{
			foreach( UIControl control in GetControls( true ) )
			{
				if( control.EnabledInHierarchy && !control.RemoveFromParentQueued && control.Visible )
					if( control.CallKeyPress( e ) )
						return true;
			}

			if( OnKeyPressBefore( e ) )
				return true;

			bool handled = false;
			KeyPressBefore?.Invoke( this, e, ref handled );
			if( handled )
				return true;

			if( OnKeyPress( e ) )
				return true;

			KeyPress?.Invoke( this, e, ref handled );
			if( handled )
				return true;

			return false;
		}

		/////////////////////////////////////////

		protected virtual bool OnKeyUpBefore( KeyEvent e ) { return false; }
		protected virtual bool OnKeyUp( KeyEvent e ) { return false; }

		public delegate void KeyUpDelegate( UIControl sender, KeyEvent e, ref bool handled );
		public event KeyUpDelegate KeyUpBefore;
		public event KeyUpDelegate KeyUp;

		internal bool CallKeyUp( KeyEvent e )
		{
			foreach( UIControl control in GetControls( true ) )
			{
				if( control.EnabledInHierarchy && !control.RemoveFromParentQueued && control.Visible )
					if( control.CallKeyUp( e ) )
						return true;
			}

			if( OnKeyUpBefore( e ) )
				return true;

			bool handled = false;
			KeyUpBefore?.Invoke( this, e, ref handled );
			if( handled )
				return true;

			if( OnKeyUp( e ) )
				return true;

			KeyUp?.Invoke( this, e, ref handled );
			if( handled )
				return true;

			return false;
		}

		/////////////////////////////////////////

		protected virtual bool OnMouseDownBefore( EMouseButtons button ) { return false; }
		protected virtual bool OnMouseDown( EMouseButtons button ) { return false; }

		public delegate void MouseButtonDelegate( UIControl sender, EMouseButtons button, ref bool handled );
		public event MouseButtonDelegate MouseDownBefore;
		public event MouseButtonDelegate MouseDown;

		internal bool CallMouseDown( EMouseButtons button )
		{
			foreach( UIControl control in GetControls( true ) )
			{
				if( control.EnabledInHierarchy && !control.RemoveFromParentQueued && control.Visible )
					if( control.CallMouseDown( button ) )
						return true;
			}

			if( new Rectangle( 0, 0, 1, 1 ).Contains( MousePosition ) || MouseRelativeMode )
			{
				if( OnMouseDownBefore( button ) )
					return true;

				//if( new Rectangle( 0, 0, 1, 1 ).Contains( MousePosition ) )
				//{
				bool handled = false;
				MouseDownBefore?.Invoke( this, button, ref handled );
				if( handled )
					return true;
				//}

				if( OnMouseDown( button ) )
					return true;

				//if( new Rectangle( 0, 0, 1, 1 ).Contains( MousePosition ) )
				//{
				//bool handled = false;
				MouseDown?.Invoke( this, button, ref handled );
				if( handled )
					return true;
			}

			if( CoverOtherControls == CoverOtherControlsEnum.OnlyBehind && ( new Rectangle( 0, 0, 1, 1 ).Contains( MousePosition ) || MouseRelativeMode ) )
				return true;
			if( CoverOtherControls == CoverOtherControlsEnum.AllPreviousInHierarchy )
				return true;

			return false;
		}

		/////////////////////////////////////////

		protected virtual bool OnMouseUpBefore( EMouseButtons button ) { return false; }
		protected virtual bool OnMouseUp( EMouseButtons button ) { return false; }

		public delegate void MouseUpDelegate( UIControl sender, EMouseButtons button, ref bool handled );
		public event MouseUpDelegate MouseUpBefore;
		public event MouseUpDelegate MouseUp;

		internal bool CallMouseUp( EMouseButtons button )
		{
			foreach( UIControl control in GetControls( true ) )
			{
				if( control.EnabledInHierarchy && !control.RemoveFromParentQueued && control.Visible )
					if( control.CallMouseUp( button ) )
						return false;
			}

			if( OnMouseUpBefore( button ) )
				return true;

			bool handled = false;
			MouseUpBefore?.Invoke( this, button, ref handled );
			if( handled )
				return true;

			if( OnMouseUp( button ) )
				return true;

			MouseUp?.Invoke( this, button, ref handled );
			if( handled )
				return true;

			return false;
		}

		/////////////////////////////////////////

		protected virtual bool OnMouseDoubleClickBefore( EMouseButtons button ) { return false; }
		protected virtual bool OnMouseDoubleClick( EMouseButtons button ) { return false; }

		public event MouseButtonDelegate MouseDoubleClickBefore;
		public event MouseButtonDelegate MouseDoubleClick;

		internal bool CallMouseDoubleClick( EMouseButtons button )
		{
			foreach( UIControl control in GetControls( true ) )
			{
				if( control.EnabledInHierarchy && !control.RemoveFromParentQueued && control.Visible )
					if( control.CallMouseDoubleClick( button ) )
						return true;
			}

			if( new Rectangle( 0, 0, 1, 1 ).Contains( MousePosition ) || MouseRelativeMode )
			{
				if( OnMouseDoubleClickBefore( button ) )
					return true;

				//if( new Rectangle( 0, 0, 1, 1 ).Contains( MousePosition ) )
				//{
				bool handled = false;
				MouseDoubleClickBefore?.Invoke( this, button, ref handled );
				if( handled )
					return true;
				//}

				if( OnMouseDoubleClick( button ) )
					return true;

				//if( new Rectangle( 0, 0, 1, 1 ).Contains( MousePosition ) )
				//{
				//bool handled = false;
				MouseDoubleClick?.Invoke( this, button, ref handled );
				if( handled )
					return true;
			}

			if( CoverOtherControls == CoverOtherControlsEnum.OnlyBehind && ( new Rectangle( 0, 0, 1, 1 ).Contains( MousePosition ) || MouseRelativeMode ) )
				return true;
			if( CoverOtherControls == CoverOtherControlsEnum.AllPreviousInHierarchy )
				return true;

			return false;
		}

		/////////////////////////////////////////

		public event ControlDefaultEventDelegate MouseEnter;
		public event ControlDefaultEventDelegate MouseLeave;

		//!!!!BeforeMouseMove? bool handled

		protected virtual void OnMouseMove( Vector2 mouse ) { }

		public delegate void MouseMoveDelegate( UIControl sender );
		public event MouseMoveDelegate MouseMove;

		internal void CallMouseMove( Vector2 mouse )
		{
			foreach( UIControl control in GetControls( false ) )
			{
				if( control.EnabledInHierarchy && !control.RemoveFromParentQueued )
					control.CallMouseMove( mouse );
			}

			OnMouseMove( mouse );

			//MouseMove event
			MouseMove?.Invoke( this );

			//MouseEnter, MouseLeave events
			if( MouseEnter != null || MouseLeave != null )
			{
				bool insideBounds;
				{
					if( MouseRelativeMode )
					//if( ParentContainer != null && ParentContainer is UIContainerScreen &&
					//	( (UIContainerScreen)ParentContainer ).Viewport.MouseRelativeMode )
					{
						insideBounds = false;
					}
					else
					{
						Vector2 mouse2 = MousePosition;
						insideBounds = mouse2.X >= 0 && mouse2.X <= 1 && mouse2.Y >= 0 && mouse2.Y <= 1;
					}
				}

				if( insideBounds != lastMousePositionInsideBounds )
				{
					if( insideBounds )
						MouseEnter?.Invoke( this );
					else
						MouseLeave?.Invoke( this );
					lastMousePositionInsideBounds = insideBounds;
				}
			}
		}

		/////////////////////////////////////////

		protected virtual bool OnMouseWheelBefore( int delta ) { return false; }
		protected virtual bool OnMouseWheel( int delta ) { return false; }

		public delegate void MouseWheelDelegate( UIControl sender, int delta, ref bool handled );
		public event MouseWheelDelegate MouseWheelBefore;
		public event MouseWheelDelegate MouseWheel;

		internal bool CallMouseWheel( int delta )
		{
			foreach( UIControl control in GetControls( true ) )
			{
				if( control.EnabledInHierarchy && !control.RemoveFromParentQueued && control.Visible )
					if( control.CallMouseWheel( delta ) )
						return true;
			}

			if( OnMouseWheelBefore( delta ) )
				return true;

			bool handled = false;
			MouseWheelBefore?.Invoke( this, delta, ref handled );
			if( handled )
				return true;

			if( OnMouseWheel( delta ) )
				return true;

			MouseWheel?.Invoke( this, delta, ref handled );
			if( handled )
				return true;

			return false;
		}

		/////////////////////////////////////////

		//OnJoystickEventBefore
		protected virtual bool OnJoystickEvent( JoystickInputEvent e ) { return false; }

		public delegate void JoystickEventDelegate( UIControl sender, JoystickInputEvent e, ref bool handled );
		//public event JoystickEventDelegate JoystickEventBefore;
		public event JoystickEventDelegate JoystickEvent;

		internal bool CallJoystickEvent( JoystickInputEvent e )
		{
			foreach( UIControl control in GetControls( true ) )
			{
				if( control.EnabledInHierarchy && !control.RemoveFromParentQueued && control.Visible )
					if( control.CallJoystickEvent( e ) )
						return true;
			}

			if( OnJoystickEvent( e ) )
				return true;

			bool handled = false;
			JoystickEvent?.Invoke( this, e, ref handled );
			if( handled )
				return true;

			return false;
		}

		/////////////////////////////////////////

		protected virtual bool OnTouch( TouchData e ) { return false; }

		public delegate void TouchDelegate( UIControl sender, TouchData e, ref bool handled );
		//public event TouchDelegate TouchBefore;
		public event TouchDelegate Touch;

		internal bool CallTouch( TouchData e )
		{
			foreach( UIControl control in GetControls( true ) )
			{
				if( control.EnabledInHierarchy && !control.RemoveFromParentQueued && control.Visible )
					if( control.CallTouch( e ) )
						return true;
			}

			if( OnTouch( e ) )
				return true;

			bool handled = false;
			Touch?.Invoke( this, e, ref handled );
			if( handled )
				return true;

			return false;
		}

		/////////////////////////////////////////

		//OnSpecialInputDeviceEventBefore
		protected virtual bool OnSpecialInputDeviceEvent( InputEvent e ) { return false; }

		public delegate void SpecialInputDeviceEventDelegate( UIControl sender, InputEvent e, ref bool handled );
		//public event TouchEventDelegate TouchEventBefore;
		public event SpecialInputDeviceEventDelegate SpecialInputDeviceEvent;

		internal bool CallSpecialInputDeviceEvent( InputEvent e )
		{
			foreach( UIControl control in GetControls( true ) )
			{
				if( control.EnabledInHierarchy && !control.RemoveFromParentQueued && control.Visible )
					if( control.CallSpecialInputDeviceEvent( e ) )
						return true;
			}

			if( OnSpecialInputDeviceEvent( e ) )
				return true;

			bool handled = false;
			SpecialInputDeviceEvent?.Invoke( this, e, ref handled );
			if( handled )
				return true;

			return false;
		}

		//UITick

		////!!!!rename UITick?
		//public delegate void UITickDelegate( UIControl sender );
		//public event UITickDelegate UITick;

		//protected virtual void OnUITick( double delta )
		//{
		//	foreach( UIControl control in GetComponentsSortedByZOrder( false ) )
		//	{
		//		if( control.EnabledInHierarchy && !control.RemoveFromParentQueued )
		//			control.OnUITick( delta );
		//	}

		//	//!!!!не тут
		//	UITick?.Invoke( this );
		//}

		//Render

		public delegate void BeforeRenderUIWithChildrenEventDelegate( UIControl sender, CanvasRenderer renderer );
		public event BeforeRenderUIWithChildrenEventDelegate BeforeRenderUIWithChildren;
		public delegate void AfterRenderUIWithChildrenEventDelegate( UIControl sender, CanvasRenderer renderer );
		public event AfterRenderUIWithChildrenEventDelegate AfterRenderUIWithChildren;

		protected virtual void OnBeforeRenderUIWithChildren( CanvasRenderer renderer )
		{
			BeforeRenderUIWithChildren?.Invoke( this, renderer );

			//push settings to GuiRenderer
			{
				//customShaderMode
				if( specialShader != null && specialShaderApplyToChildren )
				{
					renderer.PushShader( specialShader );
					pushedSpecialShaderToGuiRenderer_ApplyToChildren = true;
				}

				if( screenClipRectangle != null )
				{
					renderer.PushClipRectangle( screenClipRectangle.Value );
					pushedClipRectangleToGuiRenderer = true;
				}

				var color = ColorMultiplier.Value;
				if( color != ColorValue.One )
				{
					renderer.PushColorMultiplier( color );
					pushedColorMultiplier = true;
				}
			}
		}

		protected virtual void OnAfterRenderUIWithChildren( CanvasRenderer renderer )
		{
			//pop settings from GuiRenderer
			{
				if( pushedColorMultiplier )
				{
					renderer.PopColorMultiplier();
					pushedColorMultiplier = false;
				}

				if( pushedClipRectangleToGuiRenderer )
				{
					renderer.PopClipRectangle();
					pushedClipRectangleToGuiRenderer = false;
				}

				//customShaderMode
				if( pushedSpecialShaderToGuiRenderer_ApplyToChildren )
				{
					renderer.PopShader();
					pushedSpecialShaderToGuiRenderer_ApplyToChildren = false;
				}
			}

			AfterRenderUIWithChildren?.Invoke( this, renderer );
		}

		void DoRenderUI( CanvasRenderer renderer )
		{
			//customShaderMode
			if( specialShader != null && !specialShaderApplyToChildren )
			{
				renderer.PushShader( specialShader );
				pushedSpecialShaderToGuiRenderer_NotApplyToChildren = true;
			}

			OnRenderUI( renderer );
			RenderUI?.Invoke( this, renderer );

			//customShaderMode
			if( pushedSpecialShaderToGuiRenderer_NotApplyToChildren )
			{
				renderer.PopShader();
				pushedSpecialShaderToGuiRenderer_NotApplyToChildren = false;
			}
		}

		internal void OnRenderUIWithChildren_NonTopMostMode( CanvasRenderer renderer )
		{
			if( TopMost )
				return;

			OnBeforeRenderUIWithChildren( renderer );

			DoRenderUI( renderer );

			foreach( UIControl control in GetControls( false ) )
			{
				if( control.EnabledInHierarchy && !control.RemoveFromParentQueued && control.Visible )
					control.OnRenderUIWithChildren_NonTopMostMode( renderer );
			}

			OnAfterRenderUIWithChildren( renderer );
		}

		internal void OnRenderUIWithChildren_TopMostMode( CanvasRenderer renderer, bool topMostControlFound )
		{
			bool topMostControlFound2 = topMostControlFound;
			if( !topMostControlFound2 && TopMost )
				topMostControlFound2 = true;

			OnBeforeRenderUIWithChildren( renderer );

			if( topMostControlFound2 )
				DoRenderUI( renderer );

			foreach( UIControl control in GetControls( false ) )
			{
				if( control.EnabledInHierarchy && !control.RemoveFromParentQueued && control.Visible )
					control.OnRenderUIWithChildren_TopMostMode( renderer, topMostControlFound2 );
			}

			OnAfterRenderUIWithChildren( renderer );
		}

		protected virtual void OnRenderUI( CanvasRenderer renderer )
		{
			//bool backColorZero = backColor == new ColorValue( 0, 0, 0, 0 );

			////!!!!неправильной текстурой рисовать, чтобы видно было что ошибка? везде так
			////!!!!!заранее при загрузке?

			////!!!!Wait
			//Component_Texture backTex = ResourceManager.LoadResource<Component_Texture>( BackTexture );

			////!!!!?
			//GpuTexture gpuTexture = ResourceUtils.GetTextureCompiledData( backTex );
			//if( gpuTexture == null )
			//	gpuTexture = ResourceUtils.GetTextureCompiledData( ResourceUtils.WhiteTexture2D );

			//if( backColor.Alpha > 0 || ( backTex != null && backColorZero ) )
			//{
			//	Rect texCoord = backTextureCoord;

			//	if( backTextureTile && backTex != null )
			//	{
			//		double baseHeight = UIControlsWorld.ScaleByResolutionBaseHeight;
			//		//!!!!!!slow
			//		Vec2 tileCount = new Vec2( baseHeight * renderer.AspectRatio, baseHeight ) / gpuTexture.SourceSize.ToVec2F() * GetScreenSize();
			//		texCoord = new Rect( -tileCount * .5f, tileCount * .5f ) + new Vec2( .5f, .5f );
			//	}

			//	ColorValue color = ( backColorZero ? new ColorValue( 1, 1, 1 ) : backColor ) * GetTotalColorMultiplier();
			//	color.Clamp( new ColorValue( 0, 0, 0, 0 ), new ColorValue( 1, 1, 1, 1 ) );

			//	Rect rect;
			//	GetScreenRectangle( out rect );

			//	if( backTextureFiltering != CanvasRenderer.TextureFilteringMode.Linear )
			//		renderer.PushTextureFilteringMode( CanvasRenderer.TextureFilteringMode.Point );

			//	renderer.AddQuad( rect, texCoord, backTex, color, backTextureTile ? false : true );

			//	if( backTextureFiltering != CanvasRenderer.TextureFilteringMode.Linear )
			//		renderer.PopTextureFilteringMode();
			//}

			////BackgroundColor
			//var backgroundColor = BackgroundColor.Value;
			//if( backgroundColor.Alpha > 0 )
			//{
			//	GetScreenRectangle( out var rect );
			//	var color = ( backgroundColor * GetTotalColorMultiplier() ).GetSaturate();
			//	if( color.Alpha > 0 )
			//		renderer.AddQuad( rect, color );
			//}

			//render control
			GetStyle().PerformRenderComponent( this, renderer );
		}

		public delegate void RenderUIDelegate( UIControl sender, CanvasRenderer renderer );
		public event RenderUIDelegate RenderUI;

		//static bool IsValidName( string name, out string error )
		//{
		//	error = null;
		//	foreach( char c in name )
		//	{
		//		if( c == '\\' || c == '/' )
		//		{
		//			error = "A name can't contain any of the following characters: \\ /";
		//			return false;
		//		}
		//	}
		//	return true;
		//}

		//[DefaultValue( typeof( ColorValue ), "0 0 0 0" )]
		//[Category( "Background" )]
		//public ColorValue BackColor
		//{
		//	get { return backColor; }
		//	set { backColor = value; }
		//}

		///// <summary>
		///// The background texture of the control.
		///// </summary>
		//[Category( "Background" )]
		////!!!!!![Editor( typeof( EditorTextureUITypeEditor ), typeof( UITypeEditor ) )]
		////[EditorTextureLoadParameters( Texture.Type.Type2D, -1 )]
		//[DefaultValue( null )]
		//public string BackTexture
		////public Texture BackTexture
		//{
		//	get { return backTexture; }
		//	set { backTexture = value; }
		//}

		///// <summary>
		///// The background texture uv coordinates of the control.
		///// </summary>
		//[DefaultValue( typeof( Rect ), "0 0 1 1" )]
		//[Category( "Background" )]
		//public Rect BackTextureCoord
		//{
		//	get { return backTextureCoord; }
		//	set { backTextureCoord = value; }
		//}

		//[DefaultValue( false )]
		//[Category( "Background" )]
		//public bool BackTextureTile
		//{
		//	get { return backTextureTile; }
		//	set { backTextureTile = value; }
		//}

		///// <summary>
		///// The background texture filtering mode of the control.
		///// </summary>
		//[DefaultValue( CanvasRenderer.TextureFilteringMode.Linear )]
		//[Category( "Background" )]
		//public CanvasRenderer.TextureFilteringMode BackTextureFiltering
		//{
		//	get { return backTextureFiltering; }
		//	set { backTextureFiltering = value; }
		//}

		/// <summary>
		/// The current mouse pointer position.
		/// </summary>
		[Browsable( false )]
		public Vector2 MousePosition
		{
			get
			{
				var container = ParentContainer;
				if( container != null )
					return ConvertScreenToLocal( container.ContainerGetMousePosition() );
				else
					return Vector2.Zero;
			}
		}

		[Browsable( false )]
		public virtual bool MouseRelativeMode
		{
			get
			{
				var container = ParentContainer;
				if( container != null )
					return container.MouseRelativeMode;
				return false;
			}
		}

		[Browsable( false )]
		public bool Capture
		{
			get
			{
				if( ParentContainer == null )
					return false;
				return ParentContainer.capturedControl == this;
			}
			set
			{
				if( ParentContainer == null )
					return;
				ParentContainer.capturedControl = value ? this : null;
			}
		}

		//[DefaultValue( "" )]
		//public string UIControlRole
		//{
		//	get { return uiControlRole; }
		//	set { uiControlRole = value; }
		//}

		/// <summary>
		/// The parent container of the control.
		/// </summary>
		[Browsable( false )]
		public UIContainer ParentContainer
		{
			get
			{
				//!!!!!slowly
				//!!!!!так?

				Component c = this;
				do
				{
					//!!!!!так?
					if( !( c is UIControl ) )
						return null;

					UIContainer container = c as UIContainer;
					if( container != null )
						return container;
					c = c.Parent;
				} while( c != null );

				return null;


				//return RootParent as UIControlContainer;

				//if( !parentControlManagerInitialized )
				//{
				//	if( this is UIControlContainer )
				//	{
				//		parentControlManager = (UIControlContainer)this;
				//	}
				//	else
				//	{
				//		if( parent != null )
				//			parentControlManager = parent.ControlManager;
				//		else
				//			parentControlManager = null;
				//	}
				//	parentControlManagerInitialized = true;
				//}
				//return parentControlManager;
			}
		}

		//public virtual object Clone()
		//{
		//	//!!!!!!что тут
		//	//!!!!!ЭТО метод компоненты по сути
		//	Log.Fatal( "impl" );
		//	return null;

		//	//!!!!было

		//	//Type controlType = GetType();
		//	//ConstructorInfo constructorInfo = controlType.GetConstructor( new Type[ 0 ] );
		//	//UIControl control = (UIControl)constructorInfo.Invoke( null );

		//	//control.needResetOldScreenSize = true;

		//	//List<UIControl> childControlsAdded = new List<UIControl>();

		//	////Set properties
		//	//ControlCachedSerializeProperties.ClassItem properties =
		//	//	ControlCachedSerializeProperties.GetPropertiesByClass( control.GetType() );

		//	//foreach( PropertyInfo property in properties.GetProperties() )
		//	//{
		//	//	if( !property.CanRead || !property.CanWrite )
		//	//		continue;

		//	//	Type propertyType = property.PropertyType;

		//	//	if( SimpleTypesUtils.IsSimpleType( propertyType ) ||
		//	//		propertyType == typeof( ScaleValue ) ||
		//	//		propertyType == typeof( Texture ) ||
		//	//		propertyType == typeof( EngineFont ) )
		//	//	{
		//	//		property.SetValue( control, property.GetValue( this, null ), null );
		//	//	}
		//	//	else if( typeof( UIControl ).IsAssignableFrom( propertyType ) )
		//	//	{
		//	//		UIControl c = (UIControl)property.GetValue( this, null );

		//	//		if( c != null )//&& Controls.Contains( c ) )
		//	//		{
		//	//			if( c.CanClone )
		//	//			{
		//	//				UIControl newC = (UIControl)c.Clone();
		//	//				property.SetValue( control, newC, null );

		//	//				childControlsAdded.Add( c );
		//	//			}
		//	//		}
		//	//	}
		//	//	else
		//	//	{
		//	//		Log.Error( "UIControl: Clone: No copied property \"{0}\"", property.ToString() );
		//	//	}
		//	//}

		//	////Child controls
		//	//int insertIndex = 0;

		//	//foreach( UIControl childControl in Controls )
		//	//{
		//	//	if( childControl.CanClone )
		//	//	{
		//	//		if( !childControlsAdded.Contains( childControl ) )
		//	//		{
		//	//			control.Controls.Insert( insertIndex, (UIControl)childControl.Clone() );
		//	//			insertIndex++;
		//	//		}
		//	//	}
		//	//}

		//	//control.FileNameDeclared = FileNameDeclared;
		//	//control.FileNameCreated = FileNameCreated;

		//	//control.needResetOldScreenSize = false;

		//	//return control;
		//}

		//!!!!было
		//internal void AfterUpdateAspectRatioRecursive()
		//{
		//	oldScreenSize = GetScreenSize();

		//	foreach( UIControl control in GetComponents<UIControl>( false ) )
		//		control.AfterUpdateAspectRatioRecursive();

		//	UpdateControlsPositionAndSizeByAnchor();
		//}

		//!!!!было
		//internal void UpdateControlsPositionAndSizeByAnchor()
		//{
		//	foreach( UIControl control in GetComponents<UIControl>( false ) )
		//	{
		//		AnchorTypes anc = control.anchor;

		//		bool horizontal;
		//		bool vertical;

		//		//change size

		//		horizontal = ( anc & AnchorTypes.Left ) != 0 && ( anc & AnchorTypes.Right ) != 0;
		//		vertical = ( anc & AnchorTypes.Top ) != 0 && ( anc & AnchorTypes.Bottom ) != 0;

		//		if( horizontal || vertical )
		//		{
		//			Vec2 screenDiffSize = GetScreenSize() - oldScreenSize;
		//			if( !horizontal )
		//				screenDiffSize.X = 0;
		//			if( !vertical )
		//				screenDiffSize.Y = 0;

		//			if( screenDiffSize != Vec2.Zero )
		//			{
		//				switch( control.Size.Type )
		//				{
		//				case UIControl.ScaleType.Parent:
		//					control.Size = new UIControl.ScaleValue( control.Size.Type,
		//						control.Size.Value + control.ParentControl.GetLocalOffsetByValue(
		//						new UIControl.ScaleValue( UIControl.ScaleType.Screen, screenDiffSize ) ) );
		//					break;

		//				case UIControl.ScaleType.ScaleByResolution:
		//					control.Size = new UIControl.ScaleValue( control.Size.Type,
		//						control.Size.Value + screenDiffSize * GetScreenTextureBaseSize() );
		//					break;

		//				case UIControl.ScaleType.Pixel:
		//					control.Size = new UIControl.ScaleValue( control.Size.Type,
		//						control.Size.Value + screenDiffSize * GetScreenPixelSize() );
		//					break;
		//				}
		//			}
		//		}

		//		//change position

		//		horizontal = ( anc & AnchorTypes.Left ) == 0 && ( anc & AnchorTypes.Right ) != 0;
		//		vertical = ( anc & AnchorTypes.Top ) == 0 && ( anc & AnchorTypes.Bottom ) != 0;

		//		if( horizontal || vertical )
		//		{
		//			Vec2 screenDiffPosition = GetScreenSize() - oldScreenSize;
		//			if( !horizontal )
		//				screenDiffPosition.X = 0;
		//			if( !vertical )
		//				screenDiffPosition.Y = 0;

		//			if( screenDiffPosition != Vec2.Zero )
		//			{
		//				switch( control.Position.Type )
		//				{
		//				case UIControl.ScaleType.Parent:
		//					control.Position = new UIControl.ScaleValue( control.Position.Type,
		//						control.Position.Value + control.ParentControl.GetLocalOffsetByValue(
		//						new UIControl.ScaleValue( UIControl.ScaleType.Screen, screenDiffPosition ) ) );
		//					break;

		//				case UIControl.ScaleType.ScaleByResolution:
		//					control.Position = new UIControl.ScaleValue( control.Position.Type,
		//						control.Position.Value + screenDiffPosition * GetScreenTextureBaseSize() );
		//					break;

		//				case UIControl.ScaleType.Pixel:
		//					control.Position = new UIControl.ScaleValue( control.Position.Type,
		//						control.Position.Value + screenDiffPosition * GetScreenPixelSize() );
		//					break;
		//				}
		//			}
		//		}
		//	}
		//}

		//!!!!
		protected virtual void OnResize()
		{
			//!!!!было
			//UpdateControlsPositionAndSizeByAnchor();
		}

		/// <summary>
		/// Whether the control is focused or not.
		/// </summary>
		[Browsable( false )]
		public bool Focused
		{
			get
			{
				UIContainer manager = ParentContainer;
				if( manager == null )
					return false;
				return manager.focusedControl == this;
			}
		}

		/// <summary>
		/// Whether control can be focused.
		/// </summary>
		[Browsable( false )]
		public virtual bool CanFocus
		{
			get { return false; }
		}

		public bool Focus()
		{
			if( !CanFocus )
				return false;
			if( RemoveFromParentQueued )
				return false;
			UIContainer manager = ParentContainer;
			if( manager == null )
				return false;
			manager.focusedControl = this;
			return true;
		}

		public void Unfocus()
		{
			UIContainer manager = ParentContainer;
			if( manager == null )
				return;
			if( manager.focusedControl == this )
				manager.focusedControl = null;
		}

		////!!!!
		//[Category( "Not Refactored" )]//[Category( "Layout" )]
		//[DefaultValue( false )]
		//public bool LockEditorResizing
		//{
		//	get { return lockEditorResizing; }
		//	set { lockEditorResizing = value; }
		//}

		public override void RemoveFromParent( bool queued )
		{
			//!!!!!раньше было только если queued
			Capture = false;
			if( Focused && ParentContainer != null )
				ParentContainer.focusedControl = null;

			base.RemoveFromParent( queued );
		}

		/// <summary>
		/// The clip rectangle of the control.
		/// </summary>
		[Browsable( false )]
		public Rectangle? ScreenClipRectangle
		{
			get { return screenClipRectangle; }
			set { screenClipRectangle = value; }
		}

		//public Rectangle GetTotalScreenClipRectangle()
		//{
		//	Rectangle result = Rectangle.Cleared;
		//	bool found = false;

		//	UIControl control = this;
		//	do
		//	{
		//		if( control.screenClipRectangle != null )
		//		{
		//			if( found )
		//			{
		//				result = result.Intersection( control.screenClipRectangle.Value );
		//			}
		//			else
		//			{
		//				result = control.screenClipRectangle.Value;
		//				found = true;
		//			}
		//		}

		//		control = control.ParentControl;
		//	} while( control != null );

		//	return result;
		//}

		//!!!!!
		/// <summary>
		/// Whether the control is cloneable.
		/// </summary>
		[Browsable( false )]
		public bool CanClone
		{
			get { return canClone; }
			set { canClone = value; }
		}

		/// <summary>
		/// The special shader of the control.
		/// </summary>
		[Browsable( false )]
		public CanvasRenderer.ShaderItem SpecialShader
		{
			get { return specialShader; }
			set { specialShader = value; }
		}

		/// <summary>
		/// Whether special shader is applied to children.
		/// </summary>
		[Browsable( false )]
		public bool SpecialShaderApplyToChildren
		{
			get { return specialShaderApplyToChildren; }
			set { specialShaderApplyToChildren = value; }
		}

		/////////////////////////////////////////

		double GetParentContainerAspectRatio()
		{
			UIContainer container = ParentContainer;
			if( container != null )
				return container.AspectRatio;
			return 1;
		}

		internal void GetParentContainerSizeInUnits( out Vector2 result )
		{
			double baseHeight = UIControlsWorld.ScaleByResolutionBaseHeight;
			result = new Vector2( baseHeight * GetParentContainerAspectRatio(), baseHeight );
		}

		public Vector2 GetParentContainerSizeInUnits()
		{
			GetParentContainerSizeInUnits( out var result );
			return result;
		}

		public Vector2 GetParentContainerSizeInPixels()
		{
			UIContainer container = ParentContainer;
			if( container != null )
				return container.Viewport.SizeInPixels.ToVector2();// GetSizeInPixels();
			else
				return new Vector2( 1024, 768 );
		}

		/////////////////////////////////////////

		//!!!!!?
		//public void EnableSpecialShader( GuiRenderer.SpecialShaderItem specialShader, bool applyToChildren )
		//{
		//	specialShader = true;
		//	specialShaderApplyToChildren = applyToChildren;
		//	customShaderModeSourceFileName = sourceFileName;

		//	if( additionalTextures != null )
		//	{
		//		customShaderModeAdditionalTextures = new GuiRenderer.CustomShaderModeTexture[
		//			additionalTextures.Count ];
		//		for( int n = 0; n < additionalTextures.Count; n++ )
		//			customShaderModeAdditionalTextures[ n ] = additionalTextures[ n ];
		//	}
		//	else
		//		customShaderModeAdditionalTextures = null;

		//	if( parameters != null )
		//	{
		//		customShaderModeParameters = new GuiRenderer.CustomShaderModeParameter[
		//			parameters.Count ];
		//		for( int n = 0; n < parameters.Count; n++ )
		//			customShaderModeParameters[ n ] = parameters[ n ];
		//	}
		//	else
		//		customShaderModeParameters = null;
		//}

		//public void DisableSpecialShaderMode()
		//{
		//	specialShader = false;
		//	specialShaderApplyToChildren = false;
		//	customShaderModeSourceFileName = null;
		//	customShaderModeAdditionalTextures = null;
		//	customShaderModeParameters = null;
		//}

		internal void ResetCachedScreenRectangleRecursive()
		{
			cachedScreenRectangle = null;
			//cachedScreenPosition = null;
			//cachedScreenSize = null;

			foreach( var control in GetComponents<UIControl>( false ) )
				control.ResetCachedScreenRectangleRecursive();
		}

		//!!!!!!?
		[Browsable( false )]
		public UIControl ParentControl
		{
			get { return Parent as UIControl; }
		}

		protected override void OnAddedToParent()
		{
			cachedScreenRectangle = null;
			//cachedScreenPosition = null;
			//cachedScreenSize = null;
			//!!!!!
			//parentControlManagerInitialized = false;
			//parentControlManager = null;

			base.OnAddedToParent();

			//UIControl parent = Parent as UIControl;
			//if( parent != null && copyTextFromParent )
			//	Text = parent.Text;
		}

		protected override void OnRemovedFromParent( Component oldParent )
		{
			cachedScreenRectangle = null;
			//cachedScreenPosition = null;
			//cachedScreenSize = null;
			//!!!!!
			//parentControlManagerInitialized = false;
			//parentControlManager = null;

			base.OnRemovedFromParent( oldParent );
		}

		protected override void OnComponentRemoved( Component component )
		{
			UIControl child2 = component as UIControl;
			if( child2 != null )
			{
				child2.Capture = false;
				if( child2.Focused && child2.ParentContainer != null )
					child2.ParentContainer.focusedControl = null;
			}

			base.OnComponentRemoved( component );
		}

		/// <summary>
		/// The editor scroll position of the control.
		/// </summary>
		[Browsable( false )]
		[Serialize]
		[DefaultValue( typeof( Vector2 ), "0 0" )]
		public Vector2 EditorScrollPosition { get; set; }

		[Browsable( false )]
		[Serialize]
		[DefaultValue( 8 )]
		public int EditorZoomIndex { get; set; } = 8;

		public UIStyle GetStyle() //GetStyleFromThisOrFromParents
		{
			var v = Style.Value;
			if( v != null )
				return v;

			if( Parent != null && Parent is UIControl parent2 )
				return parent2.GetStyle();
			else
				return UIStyle.Default;
		}

		internal delegate void EnumerateChildrenDelegate( UIControl control, ref bool stopEnumerate );

		//!!!!public?
		internal bool EnumerateChildrenRecursive( bool reverse, bool onlyEnabledInHierarchy, bool onlyVisible, EnumerateChildrenDelegate action )
		{
			foreach( var control in GetControls( reverse ) )
			{
				if( ( !onlyEnabledInHierarchy || control.EnabledInHierarchy ) && !control.RemoveFromParentQueued && ( !onlyVisible || control.Visible ) )
				{
					if( !control.EnumerateChildrenRecursive( reverse, onlyEnabledInHierarchy, onlyVisible, action ) )
						return false;
				}
			}

			bool stopEnumerate = false;
			action( this, ref stopEnumerate );
			if( stopEnumerate )
				return false;

			return true;
		}

	}
}
