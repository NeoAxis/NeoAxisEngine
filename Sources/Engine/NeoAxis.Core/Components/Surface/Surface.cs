// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using NeoAxis.Editor;
using System.Runtime.CompilerServices;

namespace NeoAxis
{
	/// <summary>
	/// A component is defined how to apply of objects and material on the surface.
	/// </summary>
	[ResourceFileExtension( "surface" )]
#if !DEPLOY
	[EditorControl( "NeoAxis.Editor.SurfaceEditor" )]
	[Preview( "NeoAxis.Editor.SurfacePreview" )]
	[PreviewImage( "NeoAxis.Editor.SurfacePreviewImage" )]
#endif
	public class Surface : ResultCompile<Surface.CompiledSurfaceData>//, IEditorUpdateWhenDocumentModified
	{
		/// <summary>
		/// An optional material of the surface.
		/// </summary>
		[DefaultValue( null )]
		public Reference<Material> Material
		{
			get { if( _material.BeginGet() ) Material = _material.Get( this ); return _material.value; }
			set { if( _material.BeginSet( this, ref value ) ) { try { MaterialChanged?.Invoke( this ); ShouldRecompile = true; } finally { _material.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Material"/> property value changes.</summary>
		public event Action<Surface> MaterialChanged;
		ReferenceField<Material> _material = null;

		/// <summary>
		/// The number of UV tiles per unit for texture coordinates 0.
		/// </summary>
		[DefaultValue( "1 1" )]
		[DisplayName( "Material UV 0" )]
		[Range( 0, 10, RangeAttribute.ConvenientDistributionEnum.Exponential, 3 )]
		public Reference<Vector2> MaterialUV0
		{
			get { if( _materialUV0.BeginGet() ) MaterialUV0 = _materialUV0.Get( this ); return _materialUV0.value; }
			set { if( _materialUV0.BeginSet( this, ref value ) ) { try { MaterialUV0Changed?.Invoke( this ); ShouldRecompile = true; } finally { _materialUV0.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="MaterialUV0"/> property value changes.</summary>
		public event Action<Surface> MaterialUV0Changed;
		ReferenceField<Vector2> _materialUV0 = new Vector2( 1, 1 );

		/// <summary>
		/// The number of UV tiles per unit for texture coordinates 1.
		/// </summary>
		[DefaultValue( "1 1" )]
		[DisplayName( "Material UV 1" )]
		[Range( 0, 10, RangeAttribute.ConvenientDistributionEnum.Exponential, 3 )]
		public Reference<Vector2> MaterialUV1
		{
			get { if( _materialUV1.BeginGet() ) MaterialUV1 = _materialUV1.Get( this ); return _materialUV1.value; }
			set { if( _materialUV1.BeginSet( this, ref value ) ) { try { MaterialUV1Changed?.Invoke( this ); ShouldRecompile = true; } finally { _materialUV1.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="MaterialUV1"/> property value changes.</summary>
		public event Action<Surface> MaterialUV1Changed;
		ReferenceField<Vector2> _materialUV1 = new Vector2( 1, 1 );

		//!!!!painting mask cell size

		[Serialize]
		[Browsable( false )]
		public Transform EditorCameraTransform;

		/////////////////////////////////////////

		public class CompiledSurfaceData
		{
			public Surface Owner { get; }
			FillPatternClass fillPattern;
			GroupItem[] groups;

			/////////////////////

			public class GroupElementItem
			{
				public SurfaceElement Element;

				//SurfaceElement
				public bool Enabled;
				public float Probability;

				//SurfaceElement_Mesh
				public bool IsMeshElement;
				public Mesh Mesh;
				public Material ReplaceMaterial;
				public float VisibilityDistanceFactor;
				public bool CastShadows;
				public bool ReceiveDecals;
				public float MotionBlurFactor;
			}

			/////////////////////

			public class GroupItem
			{
				public SurfaceGroupOfElements Group;

				public bool Enabled;
				public float Probability;
				public float OccupiedAreaRadius;
				public RangeF PositionZRange;
				public bool RotateBySurfaceNormal;
				public bool RotateAroundItsAxis;
				public RadianF MaxIncline;
				public RangeF ScaleRange;
				public GroupElementItem[] Elements;
			}

			/////////////////////

			public class FillPatternClass
			{
				public Vector2 Size;
				public List<ObjectItem> Objects = new List<ObjectItem>();

				public struct ObjectItem
				{
					public Vector2 Position;
					public byte Group;

					public ObjectItem( Vector2 position, byte group )
					{
						Position = position;
						Group = group;
					}
				}
			}

			/////////////////////

			public CompiledSurfaceData( Surface owner )
			{
				Owner = owner;
			}

			public FillPatternClass FillPattern
			{
				get
				{
					if( fillPattern == null )
						CalculateDefaultFillPattern();
					return fillPattern;
				}
				set { fillPattern = value; }
			}

			void CalculateDefaultFillPattern()
			{
				fillPattern = new FillPatternClass();

				try
				{
					var surface = Owner;

					double maxOccupiedAreaRadius;
					double averageOccupiedAreaRadius;
					{
						var groups = surface.GetComponents<SurfaceGroupOfElements>();
						if( groups.Length != 0 )
						{
							maxOccupiedAreaRadius = 0;
							averageOccupiedAreaRadius = 0;
							foreach( var group in groups )
							{
								if( group.OccupiedAreaRadius > maxOccupiedAreaRadius )
									maxOccupiedAreaRadius = group.OccupiedAreaRadius;
								averageOccupiedAreaRadius += group.OccupiedAreaRadius;
							}
							averageOccupiedAreaRadius /= groups.Length;
						}
						else
						{
							maxOccupiedAreaRadius = 1;
							averageOccupiedAreaRadius = 1;
						}
					}

					fillPattern.Size = new Vector2( maxOccupiedAreaRadius * 10, maxOccupiedAreaRadius * 10 );

					var bounds = new Rectangle( 0, 0, fillPattern.Size.X, fillPattern.Size.Y );

					var random = new FastRandom( 0 );

					//calculate object count
					int count;
					{
						var toolSquare = bounds.Size.X * bounds.Size.Y;

						double radius = averageOccupiedAreaRadius;//maxOccupiedAreaRadius;
						double objectSquare = Math.PI * radius * radius;
						if( objectSquare < 0.1 )
							objectSquare = 0.1;

						double maxCount = toolSquare / objectSquare;
						//maxCount /= 10;

						count = (int)maxCount;
						count = Math.Max( count, 1 );

						count *= 4;
						//count *= 20;
					}

					var totalBounds = new Bounds( new Vector3( -fillPattern.Size, 0 ), new Vector3( fillPattern.Size * 2, 0 ) );
					totalBounds.Expand( maxOccupiedAreaRadius * 4.01 );

					var initSettings = new OctreeContainer.InitSettings();
					initSettings.InitialOctreeBounds = totalBounds;
					initSettings.OctreeBoundsRebuildExpand = Vector3.Zero;
					initSettings.MinNodeSize = totalBounds.GetSize() / 40;
					var octree = new OctreeContainer( initSettings );

					var octreeOccupiedAreas = new List<Sphere>( 256 );


					for( int n = 0; n < count; n++ )
					{
						surface.GetRandomVariation( new GetRandomVariationOptions(), random, out var groupIndex, out _, out _, out _, out _ );
						var surfaceGroup = surface.GetGroup( groupIndex );

						Vector2? position = null;

						for( var nRadiusMultiplier = 0; nRadiusMultiplier < 3; nRadiusMultiplier++ )
						{
							var radiusMultiplier = 1.0;
							switch( nRadiusMultiplier )
							{
							case 0: radiusMultiplier = 4; break;
							case 1: radiusMultiplier = 2; break;
							case 2: radiusMultiplier = 1; break;
							}

							int counter = 0;
							while( counter < 10 )
							{
								var position2 = new Vector2( random.Next( fillPattern.Size.X ), random.Next( fillPattern.Size.Y ) );

								var p = new Vector3( position2, 0 );// result.positionZ );

								var objSphere = new Sphere( p, surfaceGroup.OccupiedAreaRadius );
								objSphere.ToBounds( out var objBounds );

								var occupied = false;

								foreach( var index in octree.GetObjects( objBounds, 0xFFFFFFFF, OctreeContainer.ModeEnum.All ) )
								{
									var sphere = octreeOccupiedAreas[ index ];
									sphere.Radius *= 0.25;//back to original
									sphere.Radius *= radiusMultiplier;//multiply

									if( ( p - sphere.Center ).LengthSquared() < ( sphere.Radius + objSphere.Radius ) * ( sphere.Radius + objSphere.Radius ) )
									{
										occupied = true;
										break;
									}
								}

								if( !occupied )
								{
									//found place to create
									position = position2;
									goto end;
								}

								counter++;
							}
						}

end:;

						if( position != null )
						{
							//add object

							fillPattern.Objects.Add( new FillPatternClass.ObjectItem( position.Value, groupIndex ) );

							//add to the octree
							for( int y = -1; y <= 1; y++ )
							{
								for( int x = -1; x <= 1; x++ )
								{
									var position3 = new Vector3( position.Value, 0 );
									position3.X += (double)x * fillPattern.Size.X;
									position3.Y += (double)y * fillPattern.Size.Y;

									octreeOccupiedAreas.Add( new Sphere( position3, surfaceGroup.OccupiedAreaRadius ) );

									var b = new Bounds( position3 );
									b.Expand( surfaceGroup.OccupiedAreaRadius * 4 );
									octree.AddObject( ref b, 1 );
								}
							}
						}
					}

					octree.Dispose();
				}
				catch( Exception e )
				{
					Log.Warning( "Surface: CalculateDefaultFillPattern: " + e.Message );
				}
			}

			public GroupItem[] Groups
			{
				get
				{
					if( groups == null )
					{
						var components = Owner.GetComponents<SurfaceGroupOfElements>();
						groups = new GroupItem[ components.Length ];
						for( int nGroup = 0; nGroup < groups.Length; nGroup++ )
						{
							var group = components[ nGroup ];

							var groupItem = new GroupItem();
							groupItem.Group = group;
							groupItem.Enabled = group.Enabled;
							groupItem.Probability = (float)group.Probability.Value;
							groupItem.OccupiedAreaRadius = (float)group.OccupiedAreaRadius.Value;
							groupItem.PositionZRange = group.PositionZRange.Value.ToRangeF();
							groupItem.RotateBySurfaceNormal = group.RotateBySurfaceNormal;
							groupItem.RotateAroundItsAxis = group.RotateAroundItsAxis;
							groupItem.MaxIncline = group.MaxIncline.Value.InRadians().ToRadianF();
							groupItem.ScaleRange = group.ScaleRange.Value.ToRangeF();

							var elements = group.GetComponents<SurfaceElement>();

							groupItem.Elements = new GroupElementItem[ elements.Length ];
							for( int nElement = 0; nElement < elements.Length; nElement++ )
							{
								var element = elements[ nElement ];

								var elementItem = new GroupElementItem();
								elementItem.Element = element;
								elementItem.Enabled = element.Enabled;
								elementItem.Probability = (float)element.Probability.Value;

								//SurfaceElement_Mesh
								var elementMesh = element as SurfaceElement_Mesh;
								if( elementMesh != null )
								{
									elementItem.IsMeshElement = true;
									elementItem.Mesh = elementMesh.Mesh;
									elementItem.ReplaceMaterial = elementMesh.ReplaceMaterial;
									elementItem.VisibilityDistanceFactor = (float)elementMesh.VisibilityDistanceFactor.Value;
									elementItem.CastShadows = elementMesh.CastShadows;
									elementItem.ReceiveDecals = elementMesh.ReceiveDecals;
									elementItem.MotionBlurFactor = (float)elementMesh.MotionBlurFactor.Value;
								}

								groupItem.Elements[ nElement ] = elementItem;
							}

							groups[ nGroup ] = groupItem;
						}
					}
					return groups;
				}
			}

			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			public void GetMesh( int variationGroup, int variationElement, out bool enabled, out Mesh mesh, out Material replaceMaterial, out float visibilityDistanceFactor, out bool castShadows, out bool receiveDecals, out float motionBlurFactor )
			{
				var groups = Groups;
				if( variationGroup < groups.Length )
				{
					var group = groups[ variationGroup ];
					if( group.Enabled )
					{
						var elements = group.Elements;
						if( variationElement < elements.Length )
						{
							var element = elements[ variationElement ];
							if( element.Enabled )
							{
								//enabled = true;
								mesh = element.Mesh;
								enabled = mesh != null;
								replaceMaterial = element.ReplaceMaterial;
								visibilityDistanceFactor = element.VisibilityDistanceFactor;
								castShadows = element.CastShadows;
								receiveDecals = element.ReceiveDecals;
								motionBlurFactor = element.MotionBlurFactor;
								return;
							}
						}
					}
				}

				enabled = false;
				mesh = null;
				replaceMaterial = null;
				visibilityDistanceFactor = 1;//0;
				castShadows = false;
				receiveDecals = false;
				motionBlurFactor = 0;
			}

			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			public void GetMesh( int variationGroup, int variationElement, out bool enabled, out Mesh mesh )
			{
				var groups = Groups;
				if( variationGroup < groups.Length )
				{
					var group = groups[ variationGroup ];
					if( group.Enabled )
					{
						var elements = group.Elements;
						if( variationElement < elements.Length )
						{
							var element = elements[ variationElement ];
							if( element.Enabled )
							{
								enabled = true;
								mesh = element.Mesh;
								return;
							}
						}
					}
				}

				enabled = false;
				mesh = null;
			}

			//public float GetMaxVisibilityDistance()
			//{
			//	float result = 0;

			//	foreach( var group in Groups )
			//	{
			//		if( group.Enabled )
			//		{
			//			foreach( var element in group.Elements )
			//			{
			//				if( element.Enabled )
			//					result = Math.Max( result, element.VisibilityDistance );
			//			}
			//		}
			//	}

			//	return result;
			//}

			public virtual void GetRandomVariation( GetRandomVariationOptions options, FastRandom random, out byte groupIndex, out byte elementIndex, out double positionZ, out QuaternionF rotation, out Vector3F scale )
			{
				if( options.SetGroup.HasValue )
					groupIndex = options.SetGroup.Value;
				else
					groupIndex = 0;
				elementIndex = 0;
				positionZ = 0;
				rotation = QuaternionF.Identity;
				scale = Vector3F.One;

				bool handled = false;
				Owner.GetRandomVariationEvent?.Invoke( Owner, options, ref handled, ref groupIndex, ref elementIndex, ref positionZ, ref rotation, ref scale );
				if( handled )
					return;

				var groups = Groups;
				if( groups.Length != 0 )
				{
					if( !options.SetGroup.HasValue && groups.Length > 1 )
					{
						unsafe
						{
							var groupProbabilities = stackalloc double[ groups.Length ];
							for( int n = 0; n < groups.Length; n++ )
							{
								var group = groups[ n ];
								if( group.Enabled )//Group.Enabled )
									groupProbabilities[ n ] = group.Probability;// Group.Probability;
								else
									groupProbabilities[ n ] = 0;
							}
							groupIndex = (byte)GetRandomIndex( random, groupProbabilities, groups.Length );
						}
					}

					if( groupIndex < groups.Length )
					{
						var groupItem = groups[ groupIndex ];
						var group = groupItem.Group;

						var groupElements = groupItem.Elements;
						if( groupElements.Length > 1 )
						{
							unsafe
							{
								var elementProbabilities = stackalloc double[ groupElements.Length ];
								for( int n = 0; n < groupElements.Length; n++ )
								{
									var element = groupElements[ n ];
									if( element.Enabled )
										elementProbabilities[ n ] = element.Probability;
									else
										elementProbabilities[ n ] = 0;
								}
								elementIndex = (byte)GetRandomIndex( random, elementProbabilities, groupElements.Length );
							}
						}

						//PositionZRange
						var positionZRange = groupItem.PositionZRange;
						if( positionZRange.Minimum != positionZRange.Maximum )
							positionZ = random.Next( positionZRange.Minimum, positionZRange.Maximum );
						else
							positionZ = positionZRange.Minimum;

						//RotateBySurfaceNormal
						if( groupItem.RotateBySurfaceNormal && options.SurfaceNormal != null )
						{
							//!!!!not versatile? locked to Z

							var normal = options.SurfaceNormal.Value;

							var xAngle = MathEx.Atan2( -normal.X, normal.Z );
							var yAngle = MathEx.Atan2( normal.Y, normal.Z );

							var r = QuaternionF.FromRotateByY( xAngle ) * QuaternionF.FromRotateByX( yAngle );
							rotation *= r;
						}

						//RotateAroundItsAxis
						if( groupItem.RotateAroundItsAxis )
							rotation *= Quaternion.FromRotateByZ( random.Next( 0, MathEx.PI * 2 ) ).ToQuaternionF();

						//MaxIncline
						if( groupItem.MaxIncline != 0 )
						{
							var inclineX = random.Next( groupItem.MaxIncline );
							rotation *= QuaternionF.FromRotateByX( (float)inclineX );

							//!!!!new
							var inclineY = random.Next( groupItem.MaxIncline );
							rotation *= QuaternionF.FromRotateByY( (float)inclineY );
						}

						//ScaleRange
						var scaleRange = groupItem.ScaleRange;
						float scaleV;
						if( scaleRange.Minimum != scaleRange.Maximum )
							scaleV = (float)random.Next( scaleRange.Minimum, scaleRange.Maximum );
						else
							scaleV = (float)scaleRange.Minimum;
						scale = new Vector3F( scaleV, scaleV, scaleV );
					}
				}
			}

			public List<Mesh> GetAllMeshes()
			{
				var list = new List<Mesh>();

				foreach( var group in Groups )
				{
					foreach( var element in group.Elements )
					{
						var elementMesh = element.Element as SurfaceElement_Mesh;
						if( elementMesh != null )
						{
							var mesh = elementMesh.Mesh.Value;
							if( mesh != null )
								list.Add( mesh );
						}
					}
				}

				return list;
			}

			public virtual double GetMaxScale()
			{
				var result = 0.0;
				foreach( var group in Groups )
					result = Math.Max( result, group.ScaleRange.Maximum );
				return result;
			}

			public virtual double GetMaxVisibilityDistanceFactor()
			{
				var result = 0.0;
				foreach( var group in Groups )
				{
					foreach( var element in group.Elements )
						result = Math.Max( result, element.VisibilityDistanceFactor );
				}
				return result;
			}
		}

		/////////////////////////////////////////

		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			base.OnMetadataGetMembersFilter( context, member, ref skip );

			var p = member as Metadata.Property;
			if( p != null )
			{
				switch( p.Name )
				{
				case nameof( MaterialUV0 ):
				case nameof( MaterialUV1 ):
					if( !Material.ReferenceSpecified )
						skip = true;
					break;
				}
			}
		}

		//static int GetRandomIndex( FastRandom random, double[] probabilities )
		//{
		//	return RandomUtility.GetRandomIndexByProbabilities( random, probabilities );
		//}

		static unsafe int GetRandomIndex( FastRandom random, double* probabilities, int length )
		{
			return RandomUtility.GetRandomIndexByProbabilities( random, probabilities, length );
		}

		public struct GetRandomVariationOptions
		{
			public byte? SetGroup;
			public Vector3F? SurfaceNormal;

			public GetRandomVariationOptions( byte? setGroup, Vector3F? surfaceNormal )
			{
				SetGroup = setGroup;
				SurfaceNormal = surfaceNormal;
			}
		}

		public delegate void GetRandomVariationEventDelegate( Surface sender, GetRandomVariationOptions options, ref bool handled, ref byte groupIndex, ref byte elementIndex, ref double positionZ/*, out bool rotateByBaseNormal*/, ref QuaternionF rotation, ref Vector3F scale );
		/// <summary>
		/// The event to override <see cref="GetRandomVariation"/> method. Can be called from a background thread.
		/// </summary>
		public event GetRandomVariationEventDelegate GetRandomVariationEvent;

		public virtual void GetRandomVariation( GetRandomVariationOptions options, FastRandom random, out byte groupIndex, out byte elementIndex, out double positionZ, out QuaternionF rotation, out Vector3F scale )
		{
			var result = Result;
			if( result != null )
				result.GetRandomVariation( options, random, out groupIndex, out elementIndex, out positionZ, out rotation, out scale );
			else
			{
				if( options.SetGroup.HasValue )
					groupIndex = options.SetGroup.Value;
				else
					groupIndex = 0;
				elementIndex = 0;
				positionZ = 0;
				rotation = QuaternionF.Identity;
				scale = Vector3F.One;
			}
		}

		public SurfaceGroupOfElements GetGroup( int index )
		{
			var result = Result;
			if( result == null )
				result = new CompiledSurfaceData( this );

			var groups = result.Groups;
			if( index >= 0 && index < groups.Length )
				return groups[ index ].Group;
			return null;
		}

		public override void NewObjectSetDefaultConfiguration( bool createdFromNewObjectWindow = false )
		{
			var group = CreateComponent<SurfaceGroupOfElements>();
			group.Name = "Group 1";

			var element = group.CreateComponent<SurfaceElement_Mesh>();
			element.Name = "Mesh 1";
		}

		//!!!!вызывать
		public void TouchPropertiesToUpdateResult()
		{
			//!!!!тойчить по идее все свойства

			var groups = GetComponents<SurfaceGroupOfElements>();
			foreach( var group in groups )
			{
				foreach( var element in group.GetComponents<SurfaceElement>() )
				{
					var elementMesh = element as SurfaceElement_Mesh;
					if( elementMesh != null )
					{
						var m = elementMesh.Mesh.Value;
					}
				}
			}
		}

		protected virtual CompiledSurfaceData Compile()
		{
			var result = new CompiledSurfaceData( this );
			return result;
		}

		protected override void OnResultCompile()
		{
			base.OnResultCompile();
			Result = Compile();
		}
	}
}
