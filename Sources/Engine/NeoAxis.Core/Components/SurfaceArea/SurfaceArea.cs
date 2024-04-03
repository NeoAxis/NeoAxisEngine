// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using NeoAxis.Editor;
using System.Threading.Tasks;

namespace NeoAxis
{
	//!!!!summary: or material.

	/// <summary>
	/// Represents an area that filled by surface.
	/// </summary>
#if !DEPLOY
	[SettingsCell( "NeoAxis.Editor.SurfaceAreaSettingsCell" )]
	[ObjectCreationMode( "NeoAxis.Editor.SurfaceAreaCreationMode" )]
	[AddToResourcesWindow( @"Base\Scene objects\Areas\Surface Area", 0 )]
#endif
	public class SurfaceArea : Area
	{
		bool needUpdate;
		Vector3[] updatedForPointPositions;

		/// <summary>
		/// Surface to fill.
		/// </summary>
		[DefaultValue( null )]
		public Reference<Surface> Surface
		{
			get { if( _surface.BeginGet() ) Surface = _surface.Get( this ); return _surface.value; }
			set { if( _surface.BeginSet( this, ref value ) ) { try { SurfaceChanged?.Invoke( this ); NeedUpdate(); } finally { _surface.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Surface"/> property value changes.</summary>
		public event Action<SurfaceArea> SurfaceChanged;
		ReferenceField<Surface> _surface = null;

		//[DefaultValue( null )]
		//public Reference<Material> Material
		//{
		//	get { if( _material.BeginGet() ) Material = _material.Get( this ); return _material.value; }
		//	set { if( _material.BeginSet( this, ref value ) ) { try { MaterialChanged?.Invoke( this ); } finally { _material.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="Material"/> property value changes.</summary>
		//public event Action<SurfaceArea> MaterialChanged;
		//ReferenceField<Material> _material = null;

		/// <summary>
		/// The scale the distribution of surface objects.
		/// </summary>
		[DefaultValue( 1.0 )]
		[Range( 0.1, 10, RangeAttribute.ConvenientDistributionEnum.Exponential, 3 )]
		public Reference<double> ObjectsDistribution
		{
			get { if( _objectsDistribution.BeginGet() ) ObjectsDistribution = _objectsDistribution.Get( this ); return _objectsDistribution.value; }
			set { if( _objectsDistribution.BeginSet( this, ref value ) ) { try { ObjectsDistributionChanged?.Invoke( this ); NeedUpdate(); } finally { _objectsDistribution.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ObjectsDistribution"/> property value changes.</summary>
		public event Action<SurfaceArea> ObjectsDistributionChanged;
		ReferenceField<double> _objectsDistribution = 1.0;

		/// <summary>
		/// The scale of surface objects size.
		/// </summary>
		[DefaultValue( 1.0 )]
		[Range( 0.1, 10, RangeAttribute.ConvenientDistributionEnum.Exponential, 3 )]
		public Reference<double> ObjectsScale
		{
			get { if( _objectsScale.BeginGet() ) ObjectsScale = _objectsScale.Get( this ); return _objectsScale.value; }
			set { if( _objectsScale.BeginSet( this, ref value ) ) { try { ObjectsScaleChanged?.Invoke( this ); NeedUpdate(); } finally { _objectsScale.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ObjectsScale"/> property value changes.</summary>
		public event Action<SurfaceArea> ObjectsScaleChanged;
		ReferenceField<double> _objectsScale = 1.0;

		/// <summary>
		/// The base color and opacity multiplier of the surface objects.
		/// </summary>
		[DefaultValue( "1 1 1" )]
		public Reference<ColorValue> ObjectsColor
		{
			get { if( _objectsColor.BeginGet() ) ObjectsColor = _objectsColor.Get( this ); return _objectsColor.value; }
			set { if( _objectsColor.BeginSet( this, ref value ) ) { try { ObjectsColorChanged?.Invoke( this ); NeedUpdate(); } finally { _objectsColor.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ObjectsColor"/> property value changes.</summary>
		public event Action<SurfaceArea> ObjectsColorChanged;
		ReferenceField<ColorValue> _objectsColor = ColorValue.One;

		/// <summary>
		/// Saturation fade length near to the area borders.
		/// </summary>
		[DefaultValue( 1.0 )]
		[Range( 0, 30, RangeAttribute.ConvenientDistributionEnum.Exponential, 3 )]
		public Reference<double> FadeLength
		{
			get { if( _fadeLength.BeginGet() ) FadeLength = _fadeLength.Get( this ); return _fadeLength.value; }
			set { if( _fadeLength.BeginSet( this, ref value ) ) { try { FadeLengthChanged?.Invoke( this ); NeedUpdate(); } finally { _fadeLength.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="FadeLength"/> property value changes.</summary>
		public event Action<SurfaceArea> FadeLengthChanged;
		ReferenceField<double> _fadeLength = 1.0;

		//[DefaultValue( 0.5 )]
		//[Range( 0, 1 )]
		//public Reference<double> FadeHardness
		//{
		//	get { if( _fadeHardness.BeginGet() ) FadeHardness = _fadeHardness.Get( this ); return _fadeHardness.value; }
		//	set { if( _fadeHardness.BeginSet( this, ref value ) ) { try { FadeHardnessChanged?.Invoke( this ); NeedUpdate(); } finally { _fadeHardness.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="FadeHardness"/> property value changes.</summary>
		//public event Action<SurfaceArea> FadeHardnessChanged;
		//ReferenceField<double> _fadeHardness = 0.5;

		///// <summary>
		///// The initial value of the random number generator when filling.
		///// </summary>
		//[DefaultValue( 0 )]
		//[Range( 0, 20 )]
		//public Reference<int> RandomSeed
		//{
		//	get { if( _randomSeed.BeginGet() ) RandomSeed = _randomSeed.Get( this ); return _randomSeed.value; }
		//	set { if( _randomSeed.BeginSet( this, ref value ) ) { try { RandomSeedChanged?.Invoke( this ); NeedUpdate(); } finally { _randomSeed.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="RandomSeed"/> property value changes.</summary>
		//public event Action<SurfaceArea> RandomSeedChanged;
		//ReferenceField<int> _randomSeed = 0;

		/// <summary>
		/// Whether to do auto update after changes.
		/// </summary>
		[DefaultValue( true )]
		public Reference<bool> AutoUpdate
		{
			get { if( _autoUpdate.BeginGet() ) AutoUpdate = _autoUpdate.Get( this ); return _autoUpdate.value; }
			set { if( _autoUpdate.BeginSet( this, ref value ) ) { try { AutoUpdateChanged?.Invoke( this ); } finally { _autoUpdate.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="AutoUpdate"/> property value changes.</summary>
		public event Action<SurfaceArea> AutoUpdateChanged;
		ReferenceField<bool> _autoUpdate = true;

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

		public GroupOfObjects GetOrCreateGroupOfObjects()
		{
			var result = GetComponent( "Group Of Objects" ) as GroupOfObjects;
			if( result == null )
			{
				result = CreateComponent<GroupOfObjects>();
				result.ObjectsSerialize = false;
				result.ObjectsNetworkMode = false;
				result.Name = "Group Of Objects";
			}

			result.EditorAllowUsePaintBrush = false;

			return result;
		}

		public delegate void UpdateOutputEventDelegate( SurfaceArea sender, ref GroupOfObjects.Object[] objects );
		public event UpdateOutputEventDelegate UpdateOutputEvent;

		protected virtual void OnUpdateOutput( ref GroupOfObjects.Object[] objects )
		{
		}

		Bounds GetPointPositionsBounds()
		{
			var b = Bounds.Cleared;
			foreach( var p in GetPointPositions() )
				b.Add( p );
			return b;
		}

		GroupOfObjects.Object[] UpdateOutputDefault( Surface surface, GroupOfObjects groupOfObjects )
		{
			var pointPositions = GetPointPositions();
			var pointPositions2 = pointPositions.Select( a => a.ToVector2() ).ToArray();
			var pointBounds = GetPointPositionsBounds();
			var pointBounds2 = pointBounds.ToRectangle();

			var lines = new List<Line2>();
			for( int n = 0; n < pointPositions2.Length; n++ )
				lines.Add( new Line2( pointPositions2[ n ], pointPositions2[ ( n + 1 ) % pointPositions2.Length ] ) );

			//var random = new FastRandom( 0 );// RandomSeed.Value );

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

				////calculate object count
				//int count;
				//{
				//	double polygonArea;
				//	{
				//		var points = new List<Vector2>( pointPositions2 );
				//		points.Add( points[ 0 ] );

				//		polygonArea = Math.Abs( points.Take( points.Count - 1 )
				//		   .Select( ( p, i ) => ( points[ i + 1 ].X - p.X ) * ( points[ i + 1 ].Y + p.Y ) )
				//		   .Sum() / 2 );
				//	}

				//	double radius = averageOccupiedAreaRadius;//minDistanceBetweenObjects / 2;
				//	double objectArea = Math.PI * radius * radius;
				//	if( objectArea < 0.1 )
				//		objectArea = 0.1;

				//	double maxCount = polygonArea / objectArea;

				//	count = (int)( Strength * (double)maxCount );
				//	count = Math.Max( count, 0 );
				//}

				//!!!!512
				var data = new OpenList<GroupOfObjects.Object>( 512 );// count );

				var scene = groupOfObjects.FindParent<Scene>();
				if( scene != null )
				{
					var destinationCachedBaseObjects = groupOfObjects.GetBaseObjects();

					double objectsDistribution = ObjectsDistribution.Value;
					double objectsScale = ObjectsScale.Value;
					var objectsColor = ObjectsColor.Value;

					var fillPattern = surface.Result?.FillPattern;
					if( fillPattern != null )
					{
						var fillPatternSize = fillPattern.Size * objectsDistribution;
						if( fillPatternSize.X == 0.0 )
							fillPatternSize.X = 0.01;
						if( fillPatternSize.Y == 0.0 )
							fillPatternSize.Y = 0.01;

						//if( groupOfObjects == null )
						//	groupOfObjects = owner.GetOrCreateGroupOfObjects( true );
						//var element = GetOrCreateElement( groupOfObjects, surface );


						var min = pointBounds.Minimum.ToVector2();
						for( int n = 0; n < 2; n++ )
						{
							min[ n ] /= fillPatternSize[ n ];
							min[ n ] = Math.Floor( min[ n ] );
							min[ n ] *= fillPatternSize[ n ];
						}

						var max = pointBounds.Maximum.ToVector2();
						for( int n = 0; n < 2; n++ )
						{
							max[ n ] /= fillPatternSize[ n ];
							max[ n ] = Math.Ceiling( max[ n ] );
							max[ n ] *= fillPatternSize[ n ];
						}


						int count = 0;
						for( var y = min.Y; y < max.Y - fillPatternSize.Y * 0.5; y += fillPatternSize.Y )
							for( var x = min.X; x < max.X - fillPatternSize.X * 0.5; x += fillPatternSize.X )
								count += fillPattern.Objects.Count;

						var objectsToCreate = new (Vector2 positionXY, int groupIndex)[ count ];

						int counter = 0;
						for( var y = min.Y; y < max.Y - fillPatternSize.Y * 0.5; y += fillPatternSize.Y )
						{
							for( var x = min.X; x < max.X - fillPatternSize.X * 0.5; x += fillPatternSize.X )
							{
								for( int nObjectItem = 0; nObjectItem < fillPattern.Objects.Count; nObjectItem++ )
								{
									var objectItem = fillPattern.Objects[ nObjectItem ];

									var positionXY = new Vector2( x, y ) + objectItem.Position * objectsDistribution;
									var groupIndex = objectItem.Group;

									objectsToCreate[ counter++ ] = (positionXY, groupIndex);
								}
							}
						}

						//if( data == null && counter != 0 )
						//	data = new OpenList<GroupOfObjects.Object>( 2048 );

						var touchGroups = surface.Result.Groups;

						Parallel.For( 0, counter, delegate ( int nObjectItem )
						{
							ref var item = ref objectsToCreate[ nObjectItem ];
							ref var positionXY = ref item.positionXY;
							var groupIndex = (byte)item.groupIndex;

							if( pointBounds2.Contains( ref positionXY ) && MathAlgorithms.IsPointInPolygon( pointPositions2, positionXY ) )
							{
								var random = new FastRandom( unchecked(nObjectItem * 77 + (int)( positionXY.X * 12422.7 + positionXY.Y * 1234.2 )), true );
								//works bad for Forest template scene
								//var randomSeed = new FastRandom( unchecked(nObjectItem * 12 + (int)( positionXY.X * 11.7 + positionXY.Y * 13.2 )) );
								//var random = new FastRandom( randomSeed.NextInteger() );

								double factor = 1;
								if( FadeLength > 0 )
									factor = GetFadeFactor( positionXY );

								if( factor >= 1 || random.NextDouble() <= factor )
								{
									var result = SceneUtility.CalculateObjectPositionZ( scene, groupOfObjects, pointBounds.GetCenter().Z, positionXY, destinationCachedBaseObjects );

									var options = new Surface.GetRandomVariationOptions( groupIndex, result.normal );
									surface.GetRandomVariation( options, random, out _, out var elementIndex, out var positionZ, out var rotation, out var scale );

									if( result.found || destinationCachedBaseObjects.Count == 0 )
									{
										var obj = new GroupOfObjects.Object();
										obj.Element = 0;// (ushort)element.Index.Value;
										obj.VariationGroup = groupIndex;
										obj.VariationElement = elementIndex;
										obj.Flags = GroupOfObjects.Object.FlagsEnum.Enabled | GroupOfObjects.Object.FlagsEnum.Visible;
										obj.Position = new Vector3( positionXY, result.positionZ + positionZ );
										obj.Rotation = rotation;
										obj.Scale = scale * (float)objectsScale;
										obj.Color = objectsColor;

										lock( data )
											data.Add( ref obj );
									}
								}

							}
						} );
					}
				}

				return data.ToArray();
			}
			else
				return new GroupOfObjects.Object[ 0 ];
		}

		//GroupOfObjects.Object[] UpdateOutputDefault( Surface surface, GroupOfObjects groupOfObjects )
		//{
		//	var pointPositions = GetPointPositions();
		//	var pointPositions2 = pointPositions.Select( a => a.ToVector2() ).ToArray();
		//	var pointBounds = GetPointPositionsBounds();

		//	var lines = new List<Line2>();
		//	for( int n = 0; n < pointPositions2.Length; n++ )
		//		lines.Add( new Line2( pointPositions2[ n ], pointPositions2[ ( n + 1 ) % pointPositions2.Length ] ) );

		//	var random = new FastRandom( RandomSeed.Value );

		//	double GetFadeFactor( Vector2 point )
		//	{
		//		var minLength = double.MaxValue;
		//		foreach( var line in lines )
		//		{
		//			var d = Math.Min( ( line.Start - point ).Length(), ( line.End - point ).Length() );

		//			var projected = MathAlgorithms.ProjectPointToLine( line.Start, line.End, point );

		//			Rectangle b = new Rectangle( line.Start );
		//			b.Add( line.End );
		//			if( b.Contains( projected ) )
		//			{
		//				var d2 = ( projected - point ).Length();
		//				if( d2 < d )
		//					d = d2;
		//			}

		//			if( d < minLength )
		//				minLength = d;
		//		}

		//		if( minLength >= FadeLength )
		//			return 1;
		//		return minLength / FadeLength;
		//	}

		//	if( pointPositions.Length >= 3 )
		//	{
		//		double maxOccupiedAreaRadius;
		//		double averageOccupiedAreaRadius;
		//		{
		//			var groups = surface.GetComponents<SurfaceGroupOfElements>();
		//			if( groups.Length != 0 )
		//			{
		//				maxOccupiedAreaRadius = 0;
		//				averageOccupiedAreaRadius = 0;
		//				foreach( var group in groups )
		//				{
		//					if( group.OccupiedAreaRadius > maxOccupiedAreaRadius )
		//						maxOccupiedAreaRadius = group.OccupiedAreaRadius;
		//					averageOccupiedAreaRadius += group.OccupiedAreaRadius;
		//				}
		//				averageOccupiedAreaRadius /= groups.Length;
		//			}
		//			else
		//			{
		//				maxOccupiedAreaRadius = 1;
		//				averageOccupiedAreaRadius = 1;
		//			}
		//		}

		//		//calculate object count
		//		int count;
		//		{
		//			double polygonArea;
		//			{
		//				var points = new List<Vector2>( pointPositions2 );
		//				points.Add( points[ 0 ] );

		//				polygonArea = Math.Abs( points.Take( points.Count - 1 )
		//				   .Select( ( p, i ) => ( points[ i + 1 ].X - p.X ) * ( points[ i + 1 ].Y + p.Y ) )
		//				   .Sum() / 2 );
		//			}

		//			double radius = averageOccupiedAreaRadius;//minDistanceBetweenObjects / 2;
		//			double objectArea = Math.PI * radius * radius;
		//			if( objectArea < 0.1 )
		//				objectArea = 0.1;

		//			double maxCount = polygonArea / objectArea;

		//			count = (int)( Strength * (double)maxCount );
		//			count = Math.Max( count, 0 );
		//		}

		//		var data = new List<GroupOfObjects.Object>( count );

		//		var scene = groupOfObjects.FindParent<Scene>();
		//		if( scene != null )
		//		{
		//			var destinationCachedBaseObjects = groupOfObjects.GetBaseObjects();

		//			var initSettings = new OctreeContainer.InitSettings();
		//			initSettings.InitialOctreeBounds = pointBounds;
		//			initSettings.OctreeBoundsRebuildExpand = Vector3.Zero;
		//			initSettings.MinNodeSize = pointBounds.GetSize() / 40;
		//			var octree = new OctreeContainer( initSettings );

		//			//var boundsContainer = new BoundsContainer( pointBounds, 100 );
		//			var octreeOccupiedAreas = new List<Sphere>( 256 );

		//			for( int n = 0; n < count; n++ )
		//			{
		//				surface.GetRandomVariation( new Surface.GetRandomVariationOptions(), random, out var groupIndex, out var elementIndex, out var positionZ, out var rotation, out var scale );
		//				var surfaceGroup = surface.GetGroup( groupIndex );

		//				Vector3? position = null;

		//				for( var nRadiusMultiplier = 0; nRadiusMultiplier < 3; nRadiusMultiplier++ )
		//				{
		//					var radiusMultiplier = 1.0;
		//					switch( nRadiusMultiplier )
		//					{
		//					case 0: radiusMultiplier = 4; break;
		//					case 1: radiusMultiplier = 2; break;
		//					case 2: radiusMultiplier = 1; break;
		//					}

		//					int counter = 0;
		//					while( counter < 10 )
		//					{
		//						Vector2 position2 = Vector2.Zero;
		//						position2.X = MathEx.Lerp( pointBounds.Minimum.X, pointBounds.Maximum.X, random.NextFloat() );
		//						position2.Y = MathEx.Lerp( pointBounds.Minimum.Y, pointBounds.Maximum.Y, random.NextFloat() );

		//						if( MathAlgorithms.IsPointInPolygon( pointPositions2, position2 ) )
		//						{
		//							double factor = 1;
		//							if( FadeLength > 0 )
		//								factor = GetFadeFactor( position2 );

		//							if( factor >= 1 || random.NextDouble() <= factor )
		//							{
		//								var result = SceneUtility.CalculateObjectPositionZ( scene, groupOfObjects, pointBounds.GetCenter().Z, position2, destinationCachedBaseObjects );
		//								if( result.found || destinationCachedBaseObjects.Count == 0 )
		//								{
		//									var p = new Vector3( position2, destinationCachedBaseObjects.Count != 0 ? result.positionZ : pointBounds.GetCenter().Z );

		//									var objSphere = new Sphere( p, surfaceGroup.OccupiedAreaRadius );
		//									objSphere.ToBounds( out var objBounds );

		//									var occupied = false;

		//									//foreach( var index in boundsContainer.GetObjects( ref objBounds ) )
		//									//{
		//									//	var sphere = octreeOccupiedAreas[ index ];
		//									//	sphere.Radius *= 0.25;//back to original
		//									//	sphere.Radius *= radiusMultiplier;//multiply

		//									//	if( ( p - sphere.Origin ).LengthSquared() < ( sphere.Radius + objSphere.Radius ) * ( sphere.Radius + objSphere.Radius ) )
		//									//	{
		//									//		occupied = true;
		//									//		break;
		//									//	}
		//									//}

		//									foreach( var index in octree.GetObjects( objBounds, 0xFFFFFFFF, OctreeContainer.ModeEnum.All ) )
		//									{
		//										var sphere = octreeOccupiedAreas[ index ];
		//										sphere.Radius *= 0.25;//back to original
		//										sphere.Radius *= radiusMultiplier;//multiply

		//										if( ( p - sphere.Center ).LengthSquared() < ( sphere.Radius + objSphere.Radius ) * ( sphere.Radius + objSphere.Radius ) )
		//										{
		//											occupied = true;
		//											break;
		//										}
		//									}

		//									if( !occupied )
		//									{
		//										//found place to create
		//										position = p;
		//										goto end;
		//									}
		//								}
		//							}
		//						}

		//						counter++;
		//					}
		//				}

		//				end:;

		//				if( position != null )
		//				{
		//					var obj = new GroupOfObjects.Object();
		//					obj.Element = 0;// (ushort)element.Index.Value;
		//					obj.VariationGroup = groupIndex;
		//					obj.VariationElement = elementIndex;
		//					obj.Flags = GroupOfObjects.Object.FlagsEnum.Enabled | GroupOfObjects.Object.FlagsEnum.Visible;
		//					obj.Position = position.Value + new Vector3( 0, 0, positionZ );
		//					obj.Rotation = rotation;
		//					obj.Scale = scale;
		//					obj.Color = ColorValue.One;
		//					data.Add( obj );

		//					//add to the octree

		//					octreeOccupiedAreas.Add( new Sphere( position.Value, surfaceGroup.OccupiedAreaRadius ) );

		//					var b = new Bounds( position.Value );
		//					b.Expand( surfaceGroup.OccupiedAreaRadius * 4 );
		//					//boundsContainer.Add( ref b );
		//					octree.AddObject( b, 1 );
		//				}
		//			}

		//			octree.Dispose();
		//		}

		//		return data.ToArray();
		//	}
		//	else
		//		return new GroupOfObjects.Object[ 0 ];
		//}

		public void UpdateOutput()
		{
			var groupOfObjects = GetOrCreateGroupOfObjects();
			if( groupOfObjects != null )
			{
				var wasEnabled = groupOfObjects.Enabled;
				if( wasEnabled )
					groupOfObjects.Enabled = false;

				GroupOfObjects.Object[] objects;

				var surface = Surface.Value;
				if( surface != null )
				{
					var element = groupOfObjects.GetComponent<GroupOfObjectsElement_Surface>();
					if( element == null )
					{
						element = groupOfObjects.CreateComponent<GroupOfObjectsElement_Surface>();
						element.Name = "Surface";
						//!!!!?
						element.AutoAlign = false;
						element.Surface = ReferenceUtility.MakeThisReference( element, this, "Surface" );
					}

					objects = UpdateOutputDefault( surface, groupOfObjects );
				}
				else
					objects = new GroupOfObjects.Object[ 0 ];

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
			if( AutoUpdate && EngineApp.IsEditor )
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

		public void AddBaseObject( Component obj )
		{
			var groupOfObjects = GetOrCreateGroupOfObjects();
			if( groupOfObjects != null )
			{
				if( !groupOfObjects.BaseObjects.Contains( obj ) )
					groupOfObjects.BaseObjects.Add( ReferenceUtility.MakeRootReference<Component>( obj ) );
			}
		}
	}
}
