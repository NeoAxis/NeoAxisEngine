// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#if !DEPLOY
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;

namespace NeoAxis
{
	partial class PlantGenerator
	{
		public List<Element> Trunks = new List<Element>();//Sheaths
		public List<Element> Branches = new List<Element>();
		public List<Element> Twigs = new List<Element>();
		public List<Element> Leaves = new List<Element>();//Blades
		public List<Element> LateralRoots = new List<Element>();
		public List<Element> LateralRootBranches = new List<Element>();
		public List<Element> Flowers = new List<Element>();

		///////////////////////////////////////////////

		public enum ShapeEnum
		{
			Cylinder,
			Ribbon,
			Bowl,
			//!!!!
			//sphere типа ягоды
			//может сужающийся, прямоугольный
			//хвойный. 4,6,8 плоскостей
			//что еще
		}

		///////////////////////////////////////////////

		public enum ElementTypeEnum
		{
			Trunk,
			Branch,
			Twig,
			Leaf,
			LateralRoot,
			LateralRootBranch,
			Flower,
			//!!!!more
		}

		///////////////////////////////////////////////

		public class ElementCurve
		{
			public Curve Position = new CurveCubicSpline();
			public Curve Forward = new CurveCubicSpline();
			public Curve Up = new CurveCubicSpline();
			public Curve Scale = new CurveCubicSpline();
			public double MaxTime;

			//

			public void AddPoint( double time, Transform transform )
			{
				Position.AddPoint( time, transform.Position );
				Forward.AddPoint( time, transform.Rotation.GetForward() );
				Up.AddPoint( time, transform.Rotation.GetUp() );
				Scale.AddPoint( time, transform.Scale );

				MaxTime = Math.Max( MaxTime, time );
			}

			public Transform GetTransformByTime( double time )
			{
				var pos = Vector3.Zero;
				var rot = Quaternion.Identity;
				var scl = Vector3.One;

				//if( getPosition )
				pos = Position.CalculateValueByTime( time );

				//if( getRotation )
				{
					Vector3 forward = Forward.CalculateValueByTime( time );
					Vector3 up = Up.CalculateValueByTime( time );

					rot = Quaternion.LookAt( forward, up );

					//forward.Normalize();
					//var left = Vector3.Cross( up, forward );
					//left.Normalize();
					//up = Vector3.Cross( forward, left );
					//up.Normalize();

					//var mat = new Matrix3( forward, left, up );
					//mat.ToQuaternion( out rot );
					//rot.Normalize();
				}

				//if( getScale )
				scl = Scale.CalculateValueByTime( time );

				return new Transform( pos, rot, scl );
			}

			public Transform GetTransformByTimeFactor( double timeFactor )
			{
				return GetTransformByTime( timeFactor * MaxTime );
			}

			public (Transform transform, double parentThickness) GetTransformOnSurface( double timeFactorOnParentCurve, double twistFactorOnParentCurve, Degree verticalAngle, Degree twistAngle )
			{
				var tr = GetTransformByTimeFactor( timeFactorOnParentCurve );

				var p = new Vector2(
					Math.Cos( Math.PI * 2 * twistFactorOnParentCurve ),
					Math.Sin( Math.PI * 2 * twistFactorOnParentCurve ) );

				var offset = tr.Rotation * new Vector3( 0, p.X * tr.Scale.X, p.Y * tr.Scale.Y );

				var pos = tr.Position + offset;


				Quaternion rot;
				if( offset != Vector3.Zero )
					rot = Quaternion.LookAt( tr.Rotation.GetForward(), offset.GetNormalize() );
				else
					rot = tr.Rotation;

				var localRot = Quaternion.Identity;
				if( verticalAngle != 0 )
					localRot *= Quaternion.FromRotateByY( verticalAngle.InRadians() );
				if( twistAngle != 0 )
					localRot *= Quaternion.FromRotateByX( twistAngle.InRadians() );
				rot *= localRot;


				return (new Transform( pos, rot ), tr.Scale.MaxComponent() * 2.0);
			}
		}

		///////////////////////////////////////////////

		public class SelectorByProbability
		{
			PlantGenerator generator;
			List<Item> items = new List<Item>();
			double total;

			//

			struct Item
			{
				public Element element;
				public double from;
				public double to;
			}

			public SelectorByProbability( PlantGenerator generator )
			{
				this.generator = generator;
			}

			public void AddElements( IEnumerable<Element> elements )
			{
				foreach( var element in elements )
				{
					var item = new Item();
					item.element = element;
					item.from = total;
					item.to = total + element.Length;
					items.Add( item );

					total += element.Length;
				}
			}

			public int Count
			{
				get { return items.Count; }
			}

			public Element Get()
			{
				var value = generator.Randomizer.Next( total );

				//!!!!slowly. бинарный поиск

				for( int n = 0; n < items.Count; n++ )
				{
					var item = items[ n ];
					if( value >= item.from && value < item.to )
						return item.element;
				}

				return items[ 0 ].element;
			}
		}

		///////////////////////////////////////////////

		public class Element
		{
			public Element Parent;
			public ShapeEnum Shape;
			public Transform StartTransform;
			public ElementCurve Curve = new ElementCurve();

			public PlantMaterial Material;

			public double BendingDownByLength;

			public double Length;
			public double Width;
			public double DeepOffset;

			public bool Cross;
			public Degree VolumeNormalsDegree;

			//flowers
			public double Maturity;

			public bool Enabled = true;
			public bool GenerateMeshData = true;

			//

			public Element( Element parent, ShapeEnum shape, Transform startTransform )
			{
				Parent = parent;
				Shape = shape;
				StartTransform = startTransform;
			}

			public bool IsValidRecursive()
			{
				if( !Enabled || Length <= 0.001 )
					return false;
				if( Parent != null && !Parent.IsValidRecursive() )
					return false;
				return true;
			}
		}

		///////////////////////////////////////////////

		public class CollisionChecker
		{
			double timeThreshold;
			double twistThreshold;
			List<(double time, double twist)> created = new List<(double, double)>();

			//

			public CollisionChecker( double timeThreshold = 0.05, double twistThreshold = 0.2 )
			{
				this.timeThreshold = timeThreshold;
				this.twistThreshold = twistThreshold;
			}

			public bool Intersects( double timeFactor, double twistFactor )
			{
				foreach( var item in created )
				{
					if( Math.Abs( item.time - timeFactor ) < timeThreshold )
					{
						if( Math.Abs( item.twist - twistFactor ) < twistThreshold || Math.Abs( item.twist - 1.0 - twistFactor ) < twistThreshold || Math.Abs( item.twist + 1.0 - twistFactor ) < twistThreshold )
						{
							return true;
						}
					}
				}

				return false;
			}

			public void Add( double timeFactor, double twistFactor )
			{
				created.Add( (timeFactor, twistFactor) );
			}
		}

		///////////////////////////////////////////////

		public List<Element> GetElementsListByType( ElementTypeEnum type )
		{
			switch( type )
			{
			case ElementTypeEnum.Trunk: return Trunks;
			case ElementTypeEnum.Branch: return Branches;
			case ElementTypeEnum.Twig: return Twigs;
			case ElementTypeEnum.Leaf: return Leaves;
			case ElementTypeEnum.LateralRoot: return LateralRoots;
			case ElementTypeEnum.LateralRootBranch: return LateralRootBranches;
			case ElementTypeEnum.Flower: return Flowers;
			}
			return null;
		}

		//!!!!реже вызывать
		//!!!! double? season, double? dead, double? fired
		public PlantMaterial FindSuitableMaterial( PlantMaterial.PartTypeEnum partType )//!!!!, double? age )
		{
			//filter by part type
			var list = Materials.Where( m => m.PartType.Value == partType ).ToArray();
			if( list.Length == 0 )
				return null;

			//select by closest age
			double closestAge = 0;
			double closestAgeDiff = double.MaxValue;
			foreach( var material in list )
			{
				var diff = Math.Abs( Age - material.Age.Value );
				if( diff < closestAgeDiff )
				{
					closestAge = material.Age;
					closestAgeDiff = diff;
				}
			}
			list = list.Where( m => m.Age.Value == closestAge ).ToArray();
			if( list.Length == 0 )
				return null;


			//!!!!ближний сезон и другое


			if( list.Length > 1 )
			{
				unsafe
				{
					var probabilities = stackalloc double[ list.Length ];
					for( int n = 0; n < list.Length; n++ )
					{
						var item = list[ n ];
						if( item.Enabled )
							probabilities[ n ] = item.Probability;
						else
							probabilities[ n ] = 0;
					}
					var index = RandomUtility.GetRandomIndexByProbabilities( Randomizer, probabilities, list.Length );
					//var index = Randomizer.Next( list.Length - 1 );

					return list[ index ];
				}

				//var index = Randomizer.Next( list.Length - 1 );
				//return list[ index ];
			}
			else
				return list[ 0 ];
		}

		public Element CreateElementCylinder( Element parent, PlantMaterial material, Transform startTransform, double length, double thickness, Curve1F thicknessFactor, double curvatureFrequency1, double curvatureFrequency2, double curvatureWidth, double bendingUpByLength )
		{
			var element = new Element( parent, ShapeEnum.Cylinder, startTransform );
			element.Material = material;
			element.Length = length;
			element.Width = thickness;

			var scl = new Vector3( thickness / 2, thickness / 2, thickness / 2 );
			var deepOffset = parent != null ? ( -thickness / 2 ) : ( -thickness / 3.5 );// 2.5;

			element.DeepOffset = deepOffset;

			//!!!!
			var points = 30;

			for( int n = 0; n < points; n++ )
			{
				var time = (double)n / (double)( points - 1 );

				//!!!!другой вариант через отклонение и угол
				//!!!!чтобы оно не в цилиндре было. чтобы могло уйти в сторону
				var localPos = new Vector3(
					time * length + deepOffset,
					Math.Sin( time * curvatureFrequency1 ) * curvatureWidth / 2,
					Math.Sin( time * curvatureFrequency2 ) * curvatureWidth / 2 );

				//!!!!rotation по курве

				var pos = startTransform.Position + startTransform.Rotation * localPos;

				//bendingUpByLength
				if( bendingUpByLength != 0 )
				{
					var distance = ( pos - startTransform.Position ).Length();
					pos.Z += distance * distance * bendingUpByLength;
				}

				double scaleFactor;
				if( thicknessFactor != null )
					scaleFactor = thicknessFactor.CalculateValueByTime( (float)time );
				else
				{
					//!!!!степень параметром
					scaleFactor = 1.0 - Math.Pow( time, 1 );
					//var scl2 = scl * ( 1.0 - Math.Pow( time, 2 ) );
				}

				var scl2 = scl * scaleFactor;

				element.Curve.AddPoint( time, new Transform( pos, startTransform.Rotation, scl2 ) );
			}

			//update Forward curve by Position curve
			for( int n = 0; n < element.Curve.Position.Points.Count; n++ )
			{
				var point = element.Curve.Forward.Points[ n ];

				double time1;
				double time2;
				if( n == 0 )
				{
					time1 = point.Time;
					time2 = point.Time + 0.01;
				}
				else
				{
					time1 = point.Time - 0.01;
					time2 = point.Time;
				}

				var dir = ( element.Curve.Position.CalculateValueByTime( time2 ) - element.Curve.Position.CalculateValueByTime( time1 ) ).GetNormalize();
				element.Curve.Forward.Points[ n ] = new Curve.Point( point.Time, dir );
			}

			return element;
		}

		public Element CreateElementRibbon( Element parent, PlantMaterial material, Transform startTransform, double length, double bendingDownByLength, bool cross, Degree volumeNormalsDegree )
		{
			var element = new Element( parent, ShapeEnum.Ribbon, startTransform );
			element.BendingDownByLength = bendingDownByLength;
			element.Material = material;
			element.Length = length;
			element.Cross = cross;
			element.VolumeNormalsDegree = volumeNormalsDegree;

			if( bendingDownByLength != 0 )
			{
				//!!!!оно не только вниз, т.к. если вертикальный то не то

				//!!!!

				//element.Curve.AddPoint( 0, startTransform );
				//element.Curve.AddPoint( 1, new Transform( startTransform.Position + startTransform.Rotation * new Vector3( length, 0, 0 ), startTransform.Rotation, startTransform.Scale ) );
			}
			else
			{
				double width = 0;
				if( material != null )
				{
					var texLength = material.UVLengthRange.Value.Size;
					var texWidth = material.UVWidth.Value;
					if( texLength != 0 )
						width = texWidth * length / texLength;
				}

				var scl = new Vector3( 1, width / 2, width / 2 );

				//!!!!про пивот который не от нуля

				element.Width = width;

				element.Curve.AddPoint( 0, new Transform( startTransform.Position, startTransform.Rotation, scl ) );
				element.Curve.AddPoint( 1, new Transform( startTransform.Position + startTransform.Rotation * new Vector3( length, 0, 0 ), startTransform.Rotation, scl ) );
			}

			return element;
		}

		//!!!!весь метод
		public Element CreateGrassBladeRibbon( Element parent, PlantMaterial material, double length, Degree verticalAngle, double bendingDownByLength )
		{
			//!!!!неравномерно выставлять. может тогда параметром метода
			var timeFactor = Randomizer.Next( 1.0 );

			var startTransformPair = parent.Curve.GetTransformOnSurface( timeFactor, Randomizer.Next( 1.0 ), 0, 0 );
			var startTransform = startTransformPair.transform;

			//!!!!может еще вращать вокруг оси. начальное вращение, вращение от длины

			if( verticalAngle != 0 )
			{
				//!!!!check

				var rot = startTransform.Rotation * Quaternion.FromRotateByY( verticalAngle.InRadians() );
				startTransform = new Transform( startTransform.Position, rot );
			}

			return CreateElementRibbon( parent, material, startTransform, length, bendingDownByLength, false, 0 );
		}

		public Element CreateElementBowl( Element parent, PlantMaterial material, Transform startTransform, double length, double width, double maturity )
		{
			var element = new Element( parent, ShapeEnum.Bowl, startTransform );
			element.Material = material;
			element.Length = length;
			element.Width = width;
			element.Maturity = maturity;
			return element;
		}

		//Curve1F CreateLineThicknessCurve( double startThickness )
		//{
		//	var result = new CurveSpline1F();
		//	result.AddPoint( new Curve1F.Point( 0, (float)startThickness ) );
		//	result.AddPoint( new Curve1F.Point( 1, 0 ) );
		//	return result;
		//}

	}
}
#endif