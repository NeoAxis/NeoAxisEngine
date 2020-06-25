// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Reflection;
using System.IO;
using System.Linq;
using NeoAxis.Editor;

namespace NeoAxis
{
	//!!!!summary: or material.

	/// <summary>
	/// Represents an area that filled by surface.
	/// </summary>
	[EditorSettingsCell( typeof( Component_SurfaceArea_SettingsCell ) )]
	[ObjectCreationMode( typeof( CreationModeSurfaceArea ) )]
	public class Component_SurfaceArea : Component_Area
	{
		bool needUpdate;
		Vector3[] updatedForPointPositions;

		/// <summary>
		/// Surface to fill.
		/// </summary>
		[DefaultValue( null )]
		public Reference<Component_Surface> Surface
		{
			get { if( _surface.BeginGet() ) Surface = _surface.Get( this ); return _surface.value; }
			set { if( _surface.BeginSet( ref value ) ) { try { SurfaceChanged?.Invoke( this ); NeedUpdate(); } finally { _surface.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Surface"/> property value changes.</summary>
		public event Action<Component_SurfaceArea> SurfaceChanged;
		ReferenceField<Component_Surface> _surface = null;

		//[DefaultValue( null )]
		//public Reference<Component_Material> Material
		//{
		//	get { if( _material.BeginGet() ) Material = _material.Get( this ); return _material.value; }
		//	set { if( _material.BeginSet( ref value ) ) { try { MaterialChanged?.Invoke( this ); } finally { _material.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="Material"/> property value changes.</summary>
		//public event Action<Component_SurfaceArea> MaterialChanged;
		//ReferenceField<Component_Material> _material = null;

		/// <summary>
		/// The intensity of drawing surface.
		/// </summary>
		[DefaultValue( 1.0 )]
		[Range( 0.0, 2.0 )]
		public Reference<double> Strength
		{
			get { if( _strength.BeginGet() ) Strength = _strength.Get( this ); return _strength.value; }
			set { if( _strength.BeginSet( ref value ) ) { try { StrengthChanged?.Invoke( this ); NeedUpdate(); } finally { _strength.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Strength"/> property value changes.</summary>
		public event Action<Component_SurfaceArea> StrengthChanged;
		ReferenceField<double> _strength = 1.0;

		/// <summary>
		/// Saturation fade length near to the area borders.
		/// </summary>
		[DefaultValue( 1.0 )]
		[Range( 0, 30, RangeAttribute.ConvenientDistributionEnum.Exponential, 3 )]
		public Reference<double> FadeLength
		{
			get { if( _fadeLength.BeginGet() ) FadeLength = _fadeLength.Get( this ); return _fadeLength.value; }
			set { if( _fadeLength.BeginSet( ref value ) ) { try { FadeLengthChanged?.Invoke( this ); NeedUpdate(); } finally { _fadeLength.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="FadeLength"/> property value changes.</summary>
		public event Action<Component_SurfaceArea> FadeLengthChanged;
		ReferenceField<double> _fadeLength = 1.0;

		//[DefaultValue( 0.5 )]
		//[Range( 0, 1 )]
		//public Reference<double> FadeHardness
		//{
		//	get { if( _fadeHardness.BeginGet() ) FadeHardness = _fadeHardness.Get( this ); return _fadeHardness.value; }
		//	set { if( _fadeHardness.BeginSet( ref value ) ) { try { FadeHardnessChanged?.Invoke( this ); NeedUpdate(); } finally { _fadeHardness.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="FadeHardness"/> property value changes.</summary>
		//public event Action<Component_SurfaceArea> FadeHardnessChanged;
		//ReferenceField<double> _fadeHardness = 0.5;

		/// <summary>
		/// The initial value of the random number generator when filling.
		/// </summary>
		[DefaultValue( 0 )]
		[Range( 0, 20 )]
		public Reference<int> RandomSeed
		{
			get { if( _randomSeed.BeginGet() ) RandomSeed = _randomSeed.Get( this ); return _randomSeed.value; }
			set { if( _randomSeed.BeginSet( ref value ) ) { try { RandomSeedChanged?.Invoke( this ); NeedUpdate(); } finally { _randomSeed.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="RandomSeed"/> property value changes.</summary>
		public event Action<Component_SurfaceArea> RandomSeedChanged;
		ReferenceField<int> _randomSeed = 0;

		/// <summary>
		/// Whether to do auto update after changes.
		/// </summary>
		[DefaultValue( true )]
		public Reference<bool> AutoUpdate
		{
			get { if( _autoUpdate.BeginGet() ) AutoUpdate = _autoUpdate.Get( this ); return _autoUpdate.value; }
			set { if( _autoUpdate.BeginSet( ref value ) ) { try { AutoUpdateChanged?.Invoke( this ); } finally { _autoUpdate.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="AutoUpdate"/> property value changes.</summary>
		public event Action<Component_SurfaceArea> AutoUpdateChanged;
		ReferenceField<bool> _autoUpdate = true;

		/////////////////////////////////////////

		/// <summary>
		/// A class for providing the creation of a <see cref="Component_SurfaceArea"/> in the editor.
		/// </summary>
		public class CreationModeSurfaceArea : CreationModeArea
		{
			public CreationModeSurfaceArea( DocumentWindowWithViewport documentWindow, Component creatingObject )
				: base( documentWindow, creatingObject )
			{
			}

			public new Component_SurfaceArea CreatingObject
			{
				get { return (Component_SurfaceArea)base.CreatingObject; }
			}

			protected override bool CalculatePointPosition( Viewport viewport, out Vector3 position, out Component_ObjectInSpace collidedWith )
			{
				var result = base.CalculatePointPosition( viewport, out position, out collidedWith );

				if( result )
				{
					if( collidedWith != null )
						CreatingObject.AddBaseObject( collidedWith );
				}

				return result;
			}

			public override void Finish( bool cancel )
			{
				base.Finish( cancel );

				if( !cancel )
					CreatingObject.Surface = ReferenceUtility.MakeReference( @"Base\Surfaces\Default.surface" );
			}
		}

		/////////////////////////////////////////

		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			base.OnMetadataGetMembersFilter( context, member, ref skip );

			//var p = member as Metadata.Property;
			//if( p != null )
			//{
			//	switch( p.Name )
			//	{
			//	case nameof( Material ):
			//		if( Surface.Value != null )
			//			skip = true;
			//		break;

			//	case nameof( Surface ):
			//		if( Material.Value != null )
			//			skip = true;
			//		break;
			//	}
			//}
		}

		public bool IsDataInitialized()
		{
			if( Surface.Value != null )//!!!!|| Material.Value != null )
				return true;
			return false;
		}

		public Component_GroupOfObjects GetOrCreateGroupOfObjects()
		{
			var result = GetComponent( "Group Of Objects" ) as Component_GroupOfObjects;
			if( result == null )
			{
				result = CreateComponent<Component_GroupOfObjects>();
				result.Name = "Group Of Objects";
			}

			result.EditorAllowUsePaintBrush = false;

			return result;
		}

		public delegate void UpdateOutputEventDelegate( Component_SurfaceArea sender, ref Component_GroupOfObjects.Object[] objects );
		public event UpdateOutputEventDelegate UpdateOutputEvent;

		protected virtual void OnUpdateOutput( ref Component_GroupOfObjects.Object[] objects )
		{
		}

		Bounds GetPointPositionsBounds()
		{
			var b = Bounds.Cleared;
			foreach( var p in GetPointPositions() )
				b.Add( p );
			return b;
		}

		Component_GroupOfObjects.Object[] UpdateOutputDefault( Component_Surface surface, Component_GroupOfObjects groupOfObjects )
		{
			var pointPositions = GetPointPositions();
			var pointPositions2 = pointPositions.Select( a => a.ToVector2() ).ToArray();
			var pointBounds = GetPointPositionsBounds();

			var lines = new List<Line2>();
			for( int n = 0; n < pointPositions2.Length; n++ )
				lines.Add( new Line2( pointPositions2[ n ], pointPositions2[ ( n + 1 ) % pointPositions2.Length ] ) );

			var random = new Random( RandomSeed.Value );

			double GetFadeFactor( Vector2 point )
			{
				var minLength = double.MaxValue;
				foreach( var line in lines )
				{
					var d = Math.Min( ( line.Start - point ).Length(), ( line.End - point ).Length() );

					var projected = MathAlgorithms.ProjectPointToLine( line.Start, line.End, point );

					Rectangle b = new Rectangle( line.Start );
					b.Add( line.End );
					if( b.Contains( projected ) )
					{
						var d2 = ( projected - point ).Length();
						if( d2 < d )
							d = d2;
					}

					if( d < minLength )
						minLength = d;
				}

				if( minLength >= FadeLength )
					return 1;
				return minLength / FadeLength;
			}

			if( pointPositions.Length >= 3 )
			{
				//calculate object count
				int count;
				{
					double polygonArea;
					{
						var points = new List<Vector2>( pointPositions2 );
						points.Add( points[ 0 ] );

						polygonArea = Math.Abs( points.Take( points.Count - 1 )
						   .Select( ( p, i ) => ( points[ i + 1 ].X - p.X ) * ( points[ i + 1 ].Y + p.Y ) )
						   .Sum() / 2 );
					}

					//!!!!среднее от всех групп
					double minDistanceBetweenObjects;
					{
						var groups = surface.GetComponents<Component_SurfaceGroupOfElements>();
						if( groups.Length != 0 )
						{
							minDistanceBetweenObjects = 0;
							foreach( var group in groups )
								minDistanceBetweenObjects += group.MinDistanceBetweenObjects;
							minDistanceBetweenObjects /= groups.Length;
						}
						else
							minDistanceBetweenObjects = 1;
					}

					double radius = minDistanceBetweenObjects / 2;
					double objectArea = Math.PI * radius * radius;
					if( objectArea < 0.1 )
						objectArea = 0.1;

					double maxCount = polygonArea / objectArea;

					count = (int)( Strength * (double)maxCount );
					count = Math.Max( count, 0 );
				}

				var data = new List<Component_GroupOfObjects.Object>( count );

				var scene = groupOfObjects.FindParent<Component_Scene>();
				if( scene != null )
				{
					var destinationCachedBaseObjects = groupOfObjects.GetBaseObjects();

					var pointContainerFindFreePlace = new PointContainer3D( pointBounds, 100 );

					for( int n = 0; n < count; n++ )
					{
						surface.GetRandomVariation( new Component_Surface.GetRandomVariationOptions(), random, out var groupIndex, out var elementIndex, out var positionZ, out var rotation, out var scale );
						var surfaceGroup = surface.GetGroup( groupIndex );

						Vector3? position = null;

						int counter = 0;
						while( counter < 20 )
						{
							Vector2 position2 = Vector2.Zero;
							position2.X = MathEx.Lerp( pointBounds.Minimum.X, pointBounds.Maximum.X, random.NextFloat() );
							position2.Y = MathEx.Lerp( pointBounds.Minimum.Y, pointBounds.Maximum.Y, random.NextFloat() );

							if( MathAlgorithms.IsPointInPolygon( pointPositions2, position2 ) )
							{
								double factor = 1;
								if( FadeLength > 0 )
									factor = GetFadeFactor( position2 );

								if( factor >= 1 || random.NextDouble() <= factor )
								{
									var result = Component_Scene_Utility.CalculateObjectPositionZ( scene, groupOfObjects, pointBounds.GetCenter().Z, position2, destinationCachedBaseObjects );
									if( result.found || destinationCachedBaseObjects.Count == 0 )
									{
										var p = new Vector3( position2, destinationCachedBaseObjects.Count != 0 ? result.positionZ : pointBounds.GetCenter().Z );

										//check by MinDistanceBetweenObjects
										if( surfaceGroup == null || !pointContainerFindFreePlace.Contains( new Sphere( p, surfaceGroup.MinDistanceBetweenObjects ) ) )
										{
											//found place to create
											position = p;
											break;
										}
									}
								}
							}

							counter++;
						}

						if( position != null )
						{
							var obj = new Component_GroupOfObjects.Object();
							obj.Element = 0;// (ushort)element.Index.Value;
							obj.VariationGroup = groupIndex;
							obj.VariationElement = elementIndex;
							obj.Flags = Component_GroupOfObjects.Object.FlagsEnum.Enabled | Component_GroupOfObjects.Object.FlagsEnum.Visible;
							obj.Position = position.Value + new Vector3( 0, 0, positionZ );
							obj.Rotation = rotation;
							obj.Scale = scale;
							obj.Color = ColorValue.One;
							data.Add( obj );

							//add to point container
							pointContainerFindFreePlace.Add( ref obj.Position );
						}
					}
				}

				return data.ToArray();
			}
			else
				return new Component_GroupOfObjects.Object[ 0 ];
		}

		public void UpdateOutput()
		{
			var groupOfObjects = GetOrCreateGroupOfObjects();
			if( groupOfObjects != null )
			{
				var wasEnabled = groupOfObjects.Enabled;
				if( wasEnabled )
					groupOfObjects.Enabled = false;

				Component_GroupOfObjects.Object[] objects;

				var surface = Surface.Value;
				if( surface != null )
				{
					var element = groupOfObjects.GetComponent<Component_GroupOfObjectsElement_Surface>();
					if( element == null )
					{
						element = groupOfObjects.CreateComponent<Component_GroupOfObjectsElement_Surface>();
						element.Name = "Surface";
						//!!!!?
						element.AutoAlign = false;
						element.Surface = ReferenceUtility.MakeThisReference( element, this, "Surface" );
					}

					objects = UpdateOutputDefault( surface, groupOfObjects );
				}
				else
					objects = new Component_GroupOfObjects.Object[ 0 ];

				OnUpdateOutput( ref objects );
				UpdateOutputEvent?.Invoke( this, ref objects );

				groupOfObjects.ObjectsSet( objects );

				updatedForPointPositions = GetPointPositions();

				if( wasEnabled )
					groupOfObjects.Enabled = true;
			}
		}

		protected override void OnUpdate( float delta )
		{
			base.OnUpdate( delta );

			//check for point positions are updated
			if( AutoUpdate && EngineApp.ApplicationType == EngineApp.ApplicationTypeEnum.Editor )
			{
				var pointPositions = GetPointPositions();
				if( updatedForPointPositions == null || !updatedForPointPositions.SequenceEqual( pointPositions ) )
					needUpdate = true;
			}

			//update group of objects
			if( needUpdate && AutoUpdate )
			{
				UpdateOutput();
				needUpdate = false;
			}
		}

		public void NeedUpdate()
		{
			needUpdate = true;
		}

		public void AddBaseObject( Component_ObjectInSpace obj )
		{
			var groupOfObjects = GetOrCreateGroupOfObjects();
			if( groupOfObjects != null )
			{
				if( !groupOfObjects.BaseObjects.Contains( (Component)obj ) )
					groupOfObjects.BaseObjects.Add( ReferenceUtility.MakeRootReference<Component>( obj ) );
			}
		}
	}
}
