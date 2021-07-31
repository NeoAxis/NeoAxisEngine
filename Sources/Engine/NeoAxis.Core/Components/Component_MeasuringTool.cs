// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;

namespace NeoAxis
{
	/// <summary>
	/// The component to measure and visualize length.
	/// </summary>
	[Editor.AddToResourcesWindow( @"Base\Scene objects\Additional\Measuring Tool", 0 )]
	public class Component_MeasuringTool : Component_ObjectInSpace
	{
		/// <summary>
		/// The color of the line.
		/// </summary>
		[DefaultValue( "0 0 1" )]
		public Reference<ColorValue> Color
		{
			get { if( _color.BeginGet() ) Color = _color.Get( this ); return _color.value; }
			set { if( _color.BeginSet( ref value ) ) { try { ColorChanged?.Invoke( this ); } finally { _color.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Color"/> property value changes.</summary>
		public event Action<Component_MeasuringTool> ColorChanged;
		ReferenceField<ColorValue> _color = new ColorValue( 0, 0, 1 );

		/// <summary>
		/// The thickness of the line.
		/// </summary>
		[DefaultValue( 0.05 )]
		[Range( 0, 1, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		public Reference<double> Thickness
		{
			get { if( _thickness.BeginGet() ) Thickness = _thickness.Get( this ); return _thickness.value; }
			set { if( _thickness.BeginSet( ref value ) ) { try { ThicknessChanged?.Invoke( this ); } finally { _thickness.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Thickness"/> property value changes.</summary>
		public event Action<Component_MeasuringTool> ThicknessChanged;
		ReferenceField<double> _thickness = 0.05;

		public enum CapStyleEnum
		{
			Arrow,
			Flat,
			Sphere,
		}

		/// <summary>
		/// The style of start cap.
		/// </summary>
		[DefaultValue( CapStyleEnum.Arrow )]
		public Reference<CapStyleEnum> StartCapStyle
		{
			get { if( _startCapStyle.BeginGet() ) StartCapStyle = _startCapStyle.Get( this ); return _startCapStyle.value; }
			set { if( _startCapStyle.BeginSet( ref value ) ) { try { StartCapStyleChanged?.Invoke( this ); } finally { _startCapStyle.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="StartCapStyle"/> property value changes.</summary>
		public event Action<Component_MeasuringTool> StartCapStyleChanged;
		ReferenceField<CapStyleEnum> _startCapStyle = CapStyleEnum.Arrow;

		/// <summary>
		/// The style of end cap.
		/// </summary>
		[DefaultValue( CapStyleEnum.Arrow )]
		public Reference<CapStyleEnum> EndCapStyle
		{
			get { if( _endCapStyle.BeginGet() ) EndCapStyle = _endCapStyle.Get( this ); return _endCapStyle.value; }
			set { if( _endCapStyle.BeginSet( ref value ) ) { try { EndCapStyleChanged?.Invoke( this ); } finally { _endCapStyle.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="EndCapStyle"/> property value changes.</summary>
		public event Action<Component_MeasuringTool> EndCapStyleChanged;
		ReferenceField<CapStyleEnum> _endCapStyle = CapStyleEnum.Arrow;

		/// <summary>
		/// Whether to display information text.
		/// </summary>
		[DefaultValue( true )]
		public Reference<bool> DisplayText
		{
			get { if( _displayText.BeginGet() ) DisplayText = _displayText.Get( this ); return _displayText.value; }
			set { if( _displayText.BeginSet( ref value ) ) { try { DisplayTextChanged?.Invoke( this ); } finally { _displayText.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DisplayText"/> property value changes.</summary>
		public event Action<Component_MeasuringTool> DisplayTextChanged;
		ReferenceField<bool> _displayText = true;

		public enum MeasureEnum
		{
			Units,
			Meters,
			Centimeters,
			Inches,
		}

		/// <summary>
		/// Units of display text.
		/// </summary>
		[DefaultValue( MeasureEnum.Units )]
		public Reference<MeasureEnum> MeasureOfDisplayedLength
		{
			get { if( _measureOfDisplayedLength.BeginGet() ) MeasureOfDisplayedLength = _measureOfDisplayedLength.Get( this ); return _measureOfDisplayedLength.value; }
			set { if( _measureOfDisplayedLength.BeginSet( ref value ) ) { try { MeasureOfDisplayedLengthChanged?.Invoke( this ); } finally { _measureOfDisplayedLength.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="MeasureOfDisplayedLength"/> property value changes.</summary>
		public event Action<Component_MeasuringTool> MeasureOfDisplayedLengthChanged;
		ReferenceField<MeasureEnum> _measureOfDisplayedLength = MeasureEnum.Units;

		///////////////////////////////////////////////

		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			base.OnMetadataGetMembersFilter( context, member, ref skip );

			//if( member is Metadata.Property )
			//{
			//	switch( member.Name )
			//	{
			//	case nameof( RoundedLineCurvatureRadius ):
			//		if( CurveTypePosition.Value != CurveTypeEnum.RoundedLine )
			//			skip = true;
			//		break;
			//	}
			//}
		}

		public override void NewObjectSetDefaultConfiguration( bool createdFromNewObjectWindow )
		{
			var point = CreateComponent<Component_MeasuringToolEndPoint>();
			point.Name = point.BaseType.GetUserFriendlyNameForInstance();
			point.Transform = new Transform( Transform.Value.Position + new Vector3( 1, 0, 0 ), Quaternion.Identity );

			var text = CreateComponent<Component_Text2D>();
			text.Name = text.BaseType.GetUserFriendlyNameForInstance();
			text.Back = true;
			text.Transform = new Transform( GetCenterPosition() );
		}

		public override void NewObjectSetDefaultConfigurationUpdate()
		{
			var point = GetComponent<Component_MeasuringToolEndPoint>();
			if( point != null )
				point.Transform = new Transform( Transform.Value.Position + new Vector3( 1, 0, 0 ), Quaternion.Identity );

			UpdateTextComponent();
		}

		public Component_MeasuringToolEndPoint GetEndPoint()
		{
			return GetComponent<Component_MeasuringToolEndPoint>();
		}

		public Vector3 GetStartPosition()
		{
			return TransformV.Position;
		}

		public double GetStartScale()
		{
			return TransformV.Scale.MaxComponent();
		}

		public Vector3 GetEndPosition()
		{
			var point = GetEndPoint();
			if( point != null )
				return point.TransformV.Position;
			else
				return TransformV * new Vector3( 1, 0, 0 );
		}

		public Vector3 GetCenterPosition()
		{
			return ( GetStartPosition() + GetEndPosition() ) * 0.5;
		}

		public double GetLength()
		{
			return ( GetEndPosition() - GetStartPosition() ).Length();
		}

		public double GetEndScale()
		{
			var point = GetEndPoint();
			if( point != null )
				return point.TransformV.Scale.MaxComponent();
			else
				return TransformV.Scale.MaxComponent();
		}

		public Cylinder GetCylinder()
		{
			return new Cylinder( GetStartPosition(), GetEndPosition(), Thickness.Value * 0.5 );
		}

		protected override void OnSpaceBoundsUpdate( ref SpaceBounds newBounds )
		{
			base.OnSpaceBoundsUpdate( ref newBounds );

			var cylinder = GetCylinder();
			var b = cylinder.ToBounds();
			//add arrows thickness
			b.Expand( Thickness );
			newBounds = new SpaceBounds( b );
		}

		public delegate void UpdateTextDelegate( Component_MeasuringTool sender, ref string text );
		public event UpdateTextDelegate UpdateText;

		void UpdateTextComponent()
		{
			var text = GetComponent<Component_Text2D>();
			if( text != null )
			{
				text.Visible = DisplayText;
				text.Transform = new Transform( GetCenterPosition() );

				string value = "";

				switch( MeasureOfDisplayedLength.Value )
				{
				case MeasureEnum.Units:
					value = GetLength().ToString( "F2" );
					break;
				case MeasureEnum.Meters:
					value = GetLength().ToString( "F2" ) + " m";
					break;
				case MeasureEnum.Centimeters:
					value = ( GetLength() / 0.01 ).ToString( "F2" ) + " cm";
					break;
				case MeasureEnum.Inches:
					value = ( GetLength() / 0.0254 ).ToString( "F2" ) + " in";
					break;
				}

				UpdateText?.Invoke( this, ref value );

				text.Text = value;
			}
		}

		protected override void OnUpdate( float delta )
		{
			base.OnUpdate( delta );

			UpdateTextComponent();
		}

		public override void OnGetRenderSceneData( ViewportRenderingContext context, GetRenderSceneDataMode mode, Component_Scene.GetObjectsInSpaceItem modeGetObjectsItem )
		{
			base.OnGetRenderSceneData( context, mode, modeGetObjectsItem );

			if( mode == GetRenderSceneDataMode.InsideFrustum )
			{
				var context2 = context.objectInSpaceRenderingContext;

				if( !ParentScene.GetDisplayDevelopmentDataInThisApplication() )
					context2.disableShowingLabelForThisObject = true;

				var renderer = context.Owner.Simple3DRenderer;
				if( renderer != null )
				{
					var start = GetStartPosition();
					var startScale = GetStartScale();
					var end = GetEndPosition();
					var endScale = GetEndScale();

					if( end != start )
					{
						var center = ( start + end ) * 0.5;
						var dir = ( end - start ).GetNormalize();

						//!!!!transparent rendering

						renderer.SetColor( Color );

						switch( StartCapStyle.Value )
						{
						case CapStyleEnum.Arrow:
							renderer.AddArrow( center, start, Thickness * 5 * startScale, Thickness * 1.5 * startScale, true, Thickness );
							break;
						case CapStyleEnum.Flat:
							renderer.AddLine( center - dir * Thickness * 0.5, start + dir * Thickness * 0.5, Thickness );
							break;
						case CapStyleEnum.Sphere:
							renderer.AddLine( center - dir * Thickness * 0.5, start + dir * Thickness * 0.5, Thickness );
							renderer.AddSphere( start, Thickness * 2 * startScale, solid: true );
							break;
						}

						switch( EndCapStyle.Value )
						{
						case CapStyleEnum.Arrow:
							renderer.AddArrow( center, end, Thickness * 5 * endScale, Thickness * 1.5 * endScale, true, Thickness );
							break;
						case CapStyleEnum.Flat:
							renderer.AddLine( center + dir * Thickness * 0.5, end - dir * Thickness * 0.5, Thickness );
							break;
						case CapStyleEnum.Sphere:
							renderer.AddLine( center + dir * Thickness * 0.5, end - dir * Thickness * 0.5, Thickness );
							renderer.AddSphere( end, Thickness * 2 * endScale, solid: true );
							break;
						}

					}
				}
			}
		}

		public override ScreenLabelInfo GetScreenLabelInfo()
		{
			return new ScreenLabelInfo( "MeasuringTool" );
		}
	}

	/// <summary>
	/// End point for <see cref="Component_MeasuringTool"/>.
	/// </summary>
	public class Component_MeasuringToolEndPoint : Component_ObjectInSpace
	{
		protected override void OnTransformChanged()
		{
			base.OnTransformChanged();

			var start = Parent as Component_MeasuringTool;
			start?.SpaceBoundsUpdate();
		}

		protected override void OnSpaceBoundsUpdate( ref SpaceBounds newBounds )
		{
			base.OnSpaceBoundsUpdate( ref newBounds );

			var b = new Bounds( TransformV.Position );
			b.Expand( 0.05 );
			////add arrows thickness
			//b.Expand( Thickness );
			newBounds = new SpaceBounds( b );
		}

		public override void OnGetRenderSceneData( ViewportRenderingContext context, GetRenderSceneDataMode mode, Component_Scene.GetObjectsInSpaceItem modeGetObjectsItem )
		{
			base.OnGetRenderSceneData( context, mode, modeGetObjectsItem );

			if( mode == GetRenderSceneDataMode.InsideFrustum )
			{
				var context2 = context.objectInSpaceRenderingContext;

				if( !ParentScene.GetDisplayDevelopmentDataInThisApplication() )
					context2.disableShowingLabelForThisObject = true;
			}
		}

		public override ScreenLabelInfo GetScreenLabelInfo()
		{
			return new ScreenLabelInfo( "MeasuringToolEndPoint" );
		}
	}

}
