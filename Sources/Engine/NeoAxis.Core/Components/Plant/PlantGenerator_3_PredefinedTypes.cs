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

		//now it is in the scripts inside .planttype files



		//void GenerateStage_QuercusRobur( ElementTypeEnum stage )
		//{
		//	var minBranchTwigLength = Height / 50.0;


		//	//береза:
		//	//бранч в 7 раз тоньше чем родитель
		//	//ствол, бранчи, ветки ровные
		//	//мало бранчей, веток тоже
		//	//сверху больше растительности

		//	//дуб:
		//	//больше изогнутость чем береза
		//	//внизу тоже много растительности

		//	//ель:
		//	//веток нет, одни бранчи?

		//	//высота
		//	//толщина сплайной
		//	//

		//	switch( stage )
		//	{
		//	case ElementTypeEnum.Trunk:
		//		{
		//			var material = FindSuitableMaterial( PlantMaterial.PartTypeEnum.Bark );
		//			var startTransform = new Transform( Vector3.Zero, Quaternion.LookAt( Vector3.ZAxis, Vector3.XAxis ) );
		//			var length = Height * Randomizer.Next( 0.8, 1.2 );
		//			var thickness = length / 20.0;

		//			Trunks.Add( CreateElementCylinder( null, material, startTransform, length, thickness, null, 15.0, 24.0, thickness * 0.2, 0 ) );
		//		}
		//		break;

		//	case ElementTypeEnum.Branch:
		//		{
		//			var count = 40;// 40;
		//			if( Age < PlantType.MatureAge )
		//				count = (int)( (double)count * Math.Pow( Age / PlantType.MatureAge, 2 ) );

		//			var parent = Trunks[ 0 ];
		//			var material = FindSuitableMaterial( PlantMaterial.PartTypeEnum.Bark );

		//			var collisionChecker = new CollisionChecker();

		//			var added = 0;
		//			for( int n = 0; n < count * 10; n++ )
		//			{
		//				var timeFactor = Randomizer.Next( 0.2, 0.95 );
		//				var twistFactor = Randomizer.Next( 1.0 );

		//				if( !collisionChecker.Intersects( timeFactor, twistFactor ) )
		//				{
		//					var verticalAngle = Randomizer.Next( 45, 100 );
		//					var twistAngle = Randomizer.Next( -45, 45 );
		//					var startTransform = parent.Curve.GetTransformOnSurface( timeFactor, twistFactor, verticalAngle, twistAngle );

		//					var thickness = startTransform.parentThickness * Randomizer.Next( 0.3, 0.5 );

		//					var length = thickness * 20.0;
		//					if( length >= minBranchTwigLength )
		//					{
		//						Branches.Add( CreateElementCylinder( parent, material, startTransform.transform, length, thickness, null, 10.0, 14.0, thickness * 0.5, 0.05 ) );// 0.1 ) );

		//						collisionChecker.Add( timeFactor, twistFactor );

		//						added++;
		//						if( added >= count )
		//							break;
		//					}
		//				}
		//			}
		//		}
		//		break;

		//	case ElementTypeEnum.Twig:
		//		{
		//			var selector = new SelectorByProbability( this );
		//			selector.AddElements( Branches.Where( b => b.Length >= minBranchTwigLength ) );

		//			if( selector.Count != 0 )
		//			{
		//				var count = 300;// 400;// 200;
		//				if( Age < PlantType.MatureAge )
		//					count = (int)( (double)count * Math.Pow( Age / PlantType.MatureAge, 2 ) );

		//				var material = FindSuitableMaterial( PlantMaterial.PartTypeEnum.Bark );

		//				var added = 0;
		//				for( int n = 0; n < count * 10; n++ )
		//				{
		//					var parent = selector.Get();

		//					var timeFactorOnParentCurve = Randomizer.Next( 0.25, 0.95 );
		//					var verticalAngle = Randomizer.Next( 45.0, 100.0 );
		//					var twistAngle = Randomizer.Next( -45.0, 45.0 );
		//					var startTransform = parent.Curve.GetTransformOnSurface( timeFactorOnParentCurve, Randomizer.Next( 1.0 ), verticalAngle, twistAngle );

		//					var thickness = startTransform.parentThickness * Randomizer.Next( 0.3, 0.5 );

		//					var length = thickness * 20.0;
		//					if( length >= minBranchTwigLength )
		//					{
		//						Twigs.Add( CreateElementCylinder( parent, material, startTransform.transform, length, thickness, null, 10.0, 14.0, thickness * 0.5, 0.2 ) );

		//						added++;
		//						if( added >= count )
		//							break;
		//					}
		//				}
		//			}
		//		}
		//		break;

		//	case ElementTypeEnum.Leaf:
		//		if( Branches.Count != 0 || Twigs.Count != 0 )
		//		{
		//			var selector = new SelectorByProbability( this );
		//			selector.AddElements( Branches );
		//			selector.AddElements( Twigs );
		//			selector.AddElements( Trunks );

		//			//!!!!распределять в зависимости от длины

		//			//!!!!равномерно распределять. бранчи, ветки тоже

		//			//!!!!применять LeafCount

		//			var count = 2000;// 1500;// 2000;// 2500;
		//			if( Age < PlantType.MatureAge )
		//				count = (int)( (double)count * Math.Pow( Age / PlantType.MatureAge, 2.5 ) );

		//			//if( LOD >= 2 )
		//			//	count /= 2;
		//			//if( LOD >= 3 )
		//			//	count /= 6;

		//			for( int n = 0; n < count; n++ )
		//			{
		//				var parent = selector.Get();

		//				var material = FindSuitableMaterial( PlantMaterial.PartTypeEnum.BranchWithLeaves );

		//				//!!!!поворачивать по горизонтали?

		//				//!!!!распределение

		//				//!!!!ориентация относительно солнца/верха

		//				var verticalAngle = Randomizer.Next( 90.0 - 45.0, 90.0 + 45.0 );
		//				var twistAngle = Randomizer.Next( -45.0, 45.0 );

		//				var startTransform = parent.Curve.GetTransformOnSurface( Randomizer.Next( 0.3, 0.97 ), Randomizer.Next( 1.0 ), verticalAngle, twistAngle );

		//				//!!!!tiltAngle

		//				var length = 1.0;
		//				if( material != null )
		//					length = material.RealLength * Randomizer.Next( 0.8, 1.2 );

		//				//if( LOD >= 2 )
		//				//	length *= 1.5;
		//				//if( LOD >= 3 )
		//				//	length *= 1.5;

		//				Leaves.Add( CreateElementRibbon( parent, material, startTransform.transform, length, 0, true, 45 ) );
		//			}

		//			//!!!!
		//			//проверять материал есть ли ветка.
		//			//если нет тогда делать листья. есть есть тогда всю ветку риббоном.

		//		}
		//		break;
		//	}
		//}

		///////////////////////////////////////////////

		//void GenerateStage_MatricariaChamomilla( ElementTypeEnum stage )
		//{
		//	switch( stage )
		//	{
		//	case ElementTypeEnum.Trunk:
		//		{
		//			var material = FindSuitableMaterial( PlantMaterial.PartTypeEnum.Bark );
		//			var startTransform = new Transform( Vector3.Zero, Quaternion.LookAt( Vector3.ZAxis, Vector3.XAxis ) );
		//			var length = Height * Randomizer.Next( 0.8, 1.2 );
		//			var thickness = length / 60.0;

		//			var thicknessFactor = new CurveCubicSpline1F();
		//			thicknessFactor.AddPoint( new Curve1F.Point( 0, 1 ) );
		//			thicknessFactor.AddPoint( new Curve1F.Point( 1, 0.33f ) );
		//			//thicknessFactor.AddPoint( new Curve1F.Point( 0.95f, 0.33f ) );
		//			//thicknessFactor.AddPoint( new Curve1F.Point( 1, 0 ) );

		//			Trunks.Add( CreateElementCylinder( null, material, startTransform, length, thickness, thicknessFactor, 10, 13, thickness * 0.5, 0 ) );
		//		}
		//		break;

		//	case ElementTypeEnum.Branch:
		//		{
		//			var count = 7;
		//			if( Age < PlantType.MatureAge )
		//				count = (int)( (double)count * Math.Pow( Age / PlantType.MatureAge, 2 ) );

		//			var parent = Trunks[ 0 ];
		//			var material = FindSuitableMaterial( PlantMaterial.PartTypeEnum.Bark );

		//			var added = 0;
		//			for( int n = 0; n < count * 10; n++ )
		//			{
		//				var timeFactorOnParentCurve = Randomizer.Next( 0.2, 0.65 );
		//				var verticalAngle = Randomizer.Next( 20.0, 50.0 );
		//				var twistAngle = Randomizer.Next( -45.0, 45.0 );
		//				var startTransform = parent.Curve.GetTransformOnSurface( timeFactorOnParentCurve, Randomizer.Next( 1.0 ), verticalAngle, twistAngle );

		//				var thickness = startTransform.parentThickness * Randomizer.Next( 0.8, 1.0 );

		//				var minBranchTwigLength = Height / 150.0;

		//				var length = thickness * 35.0;
		//				if( length >= minBranchTwigLength )
		//				{
		//					var thicknessFactor = new CurveCubicSpline1F();
		//					thicknessFactor.AddPoint( new Curve1F.Point( 0, 1 ) );
		//					thicknessFactor.AddPoint( new Curve1F.Point( 1, 0.33f ) );

		//					Branches.Add( CreateElementCylinder( parent, material, startTransform.transform, length, thickness, thicknessFactor, 10.0, 13.0, thickness * 0.5, 3.0 ) );

		//					added++;
		//					if( added >= count )
		//						break;
		//				}
		//			}
		//		}
		//		break;

		//	//case ElementTypeEnum.Twig:
		//	//	break;

		//	case ElementTypeEnum.Flower:
		//		{
		//			for( int n = 0; n < Trunks.Count + Branches.Count; n++ )
		//			{
		//				var maturity = Age / PlantType.MatureAge.Value * Randomizer.Next( 0.8, 1.2 );
		//				if( maturity > 0.33 )
		//				{
		//					Element parent;
		//					if( n < Trunks.Count )
		//						parent = Trunks[ n ];
		//					else
		//						parent = Branches[ n - Trunks.Count ];

		//					var material = FindSuitableMaterial( PlantMaterial.PartTypeEnum.Flower );

		//					//!!!!twist random

		//					var transform1 = parent.Curve.GetTransformByTimeFactor( 1 );

		//					var direction = ( transform1.Position - parent.Curve.GetTransformByTimeFactor( 0.99 ).Position ).GetNormalize();
		//					var rotation = Quaternion.FromDirectionZAxisUp( direction );

		//					var transform = new Transform( transform1.Position, rotation );

		//					var length = material != null ? material.RealLength.Value : Height / 15.0;
		//					length *= Randomizer.Next( 0.8, 1.2 );
		//					if( maturity < 1 )
		//						length *= maturity;

		//					var width = length;

		//					Flowers.Add( CreateElementBowl( parent, material, transform, length, width, maturity ) );
		//				}
		//			}
		//		}
		//		break;

		//	case ElementTypeEnum.Leaf:
		//		if( Branches.Count != 0 || Twigs.Count != 0 )
		//		{
		//			var selector = new SelectorByProbability( this );
		//			selector.AddElements( Branches );
		//			//selector.AddElements( Twigs );
		//			selector.AddElements( Trunks );

		//			//!!!!распределять в зависимости от длины

		//			//!!!!равномерно распределять. бранчи, ветки тоже

		//			//!!!!применять LeafCount

		//			var material = FindSuitableMaterial( PlantMaterial.PartTypeEnum.BranchWithLeaves );

		//			var count = 50;
		//			if( Age < PlantType.MatureAge )
		//				count = (int)( (double)count * Math.Pow( Age / PlantType.MatureAge, 2 ) );

		//			//if( LOD >= 2 )
		//			//	count /= 2;
		//			//if( LOD >= 3 )
		//			//	count /= 6;

		//			for( int n = 0; n < count; n++ )
		//			{
		//				var parent = selector.Get();

		//				//!!!!поворачивать по горизонтали?

		//				//!!!!распределение

		//				//!!!!ориентация относительно солнца/верха

		//				var verticalAngle = Randomizer.Next( -30.0, 30.0 );
		//				var twistAngle = Randomizer.Next( -90.0, 90.0 );

		//				var startTransform = parent.Curve.GetTransformOnSurface( Randomizer.Next( 0.01, 0.65 ), Randomizer.Next( 1.0 ), verticalAngle, twistAngle );

		//				//!!!!tiltAngle

		//				var length = 0.1;
		//				if( material != null )
		//				{
		//					var maturity = Math.Min( Age / PlantType.MatureAge.Value, 1.0 );
		//					length = material.RealLength * maturity * Randomizer.Next( 0.8, 1.2 );
		//				}

		//				//!!!!cross?
		//				Leaves.Add( CreateElementRibbon( parent, material, startTransform.transform, length, 0, false, 0 ) );
		//			}

		//			//!!!!
		//			//проверять материал есть ли ветка.
		//			//если нет тогда делать листья. есть есть тогда всю ветку риббоном.

		//		}
		//		break;
		//	}
		//}

		///////////////////////////////////////////////

		//void GenerateStage_DoronicumGrandiflorum( ElementTypeEnum stage )
		//{
		//	switch( stage )
		//	{
		//	case ElementTypeEnum.Trunk:
		//		{
		//			var material = FindSuitableMaterial( PlantMaterial.PartTypeEnum.Bark );
		//			var startTransform = new Transform( Vector3.Zero, Quaternion.LookAt( Vector3.ZAxis, Vector3.XAxis ) );
		//			var length = Height * Randomizer.Next( 0.8, 1.2 );
		//			var thickness = length / 60.0;

		//			var thicknessFactor = new CurveCubicSpline1F();
		//			thicknessFactor.AddPoint( new Curve1F.Point( 0, 1 ) );
		//			thicknessFactor.AddPoint( new Curve1F.Point( 1, 0.33f ) );
		//			//thicknessFactor.AddPoint( new Curve1F.Point( 0.95f, 0.33f ) );
		//			//thicknessFactor.AddPoint( new Curve1F.Point( 1, 0 ) );

		//			Trunks.Add( CreateElementCylinder( null, material, startTransform, length, thickness, thicknessFactor, 10, 13, thickness * 0.5, 0 ) );
		//		}
		//		break;

		//	case ElementTypeEnum.Branch:
		//		{
		//			var count = 7;
		//			if( Age < PlantType.MatureAge )
		//				count = (int)( (double)count * Math.Pow( Age / PlantType.MatureAge, 2 ) );

		//			var parent = Trunks[ 0 ];
		//			var material = FindSuitableMaterial( PlantMaterial.PartTypeEnum.Bark );

		//			var added = 0;
		//			for( int n = 0; n < count * 10; n++ )
		//			{
		//				var timeFactorOnParentCurve = Randomizer.Next( 0.2, 0.65 );
		//				var verticalAngle = Randomizer.Next( 20.0, 50.0 );
		//				var twistAngle = Randomizer.Next( -45.0, 45.0 );
		//				var startTransform = parent.Curve.GetTransformOnSurface( timeFactorOnParentCurve, Randomizer.Next( 1.0 ), verticalAngle, twistAngle );

		//				var thickness = startTransform.parentThickness * Randomizer.Next( 0.8, 1.0 );

		//				var minBranchTwigLength = Height / 150.0;

		//				var length = thickness * 35.0;
		//				if( length >= minBranchTwigLength )
		//				{
		//					var thicknessFactor = new CurveCubicSpline1F();
		//					thicknessFactor.AddPoint( new Curve1F.Point( 0, 1 ) );
		//					thicknessFactor.AddPoint( new Curve1F.Point( 1, 0.33f ) );

		//					Branches.Add( CreateElementCylinder( parent, material, startTransform.transform, length, thickness, thicknessFactor, 10.0, 13.0, thickness * 0.5, 3.0 ) );

		//					added++;
		//					if( added >= count )
		//						break;
		//				}
		//			}
		//		}
		//		break;

		//	//case ElementTypeEnum.Twig:
		//	//	break;

		//	case ElementTypeEnum.Flower:
		//		{
		//			for( int n = 0; n < Trunks.Count + Branches.Count; n++ )
		//			{
		//				var maturity = Age / PlantType.MatureAge.Value * Randomizer.Next( 0.8, 1.2 );
		//				if( maturity > 0.33 )
		//				{
		//					Element parent;
		//					if( n < Trunks.Count )
		//						parent = Trunks[ n ];
		//					else
		//						parent = Branches[ n - Trunks.Count ];

		//					var material = FindSuitableMaterial( PlantMaterial.PartTypeEnum.Flower );

		//					//!!!!twist random

		//					var transform1 = parent.Curve.GetTransformByTimeFactor( 1 );

		//					var direction = ( transform1.Position - parent.Curve.GetTransformByTimeFactor( 0.99 ).Position ).GetNormalize();
		//					var rotation = Quaternion.FromDirectionZAxisUp( direction );

		//					var transform = new Transform( transform1.Position, rotation );

		//					var length = material != null ? material.RealLength.Value : Height / 15.0;
		//					length *= Randomizer.Next( 0.8, 1.2 );
		//					if( maturity < 1 )
		//						length *= maturity;

		//					var width = length;

		//					Flowers.Add( CreateElementBowl( parent, material, transform, length, width, maturity ) );
		//				}
		//			}
		//		}
		//		break;

		//	case ElementTypeEnum.Leaf:
		//		if( Branches.Count != 0 || Twigs.Count != 0 )
		//		{
		//			var selector = new SelectorByProbability( this );
		//			selector.AddElements( Branches );
		//			//selector.AddElements( Twigs );
		//			selector.AddElements( Trunks );

		//			//!!!!распределять в зависимости от длины

		//			//!!!!равномерно распределять. бранчи, ветки тоже

		//			//!!!!apply LeafCount

		//			var material = FindSuitableMaterial( PlantMaterial.PartTypeEnum.BranchWithLeaves );

		//			var count = 30;// 50;
		//			if( Age < PlantType.MatureAge )
		//				count = (int)( (double)count * Math.Pow( Age / PlantType.MatureAge, 2 ) );

		//			//if( LOD >= 2 )
		//			//	count /= 2;
		//			//if( LOD >= 3 )
		//			//	count /= 6;

		//			for( int n = 0; n < count; n++ )
		//			{
		//				var parent = selector.Get();

		//				//!!!!rotate by horizontal?

		//				//!!!!distribution

		//				//!!!!sun/up orintation

		//				var verticalAngle = Randomizer.Next( 40, 60.0 );
		//				var twistAngle = Randomizer.Next( -10.0, 10.0 );

		//				var startTransform = parent.Curve.GetTransformOnSurface( Randomizer.Next( 0.1, 0.6 ), Randomizer.Next( 1.0 ), verticalAngle, twistAngle );
		//				//var startTransform = parent.Curve.GetTransformOnSurface( Randomizer.Next( 0.1, 0.8 ), Randomizer.Next( 1.0 ), verticalAngle, twistAngle );

		//				//!!!!tiltAngle

		//				var length = 0.1;
		//				if( material != null )
		//				{
		//					var maturity = Math.Min( Age / PlantType.MatureAge.Value, 1.0 );
		//					length = material.RealLength * maturity * Randomizer.Next( 0.8, 1.2 );
		//				}

		//				Leaves.Add( CreateElementRibbon( parent, material, startTransform.transform, length, 0, false, 0 ) );
		//			}

		//			//!!!!
		//			//проверять материал есть ли ветка.
		//			//если нет тогда делать листья. есть есть тогда всю ветку риббоном.

		//		}
		//		break;
		//	}
		//}

		///////////////////////////////////////////////

		//void GenerateStage_AlopecurusPratensis( ElementTypeEnum stage )
		//{
		//	switch( stage )
		//	{
		//	case ElementTypeEnum.Trunk:
		//		{
		//			var material = FindSuitableMaterial( PlantMaterial.PartTypeEnum.BranchWithLeaves );

		//			var length = Height;

		//			{
		//				var transform = new Transform( Vector3.Zero, Matrix3.LookAt( Vector3.ZAxis, Vector3.XAxis ).ToQuaternion() );
		//				Trunks.Add( CreateElementRibbon( null, material, transform, length, 0, false, 0 ) );
		//			}

		//			{
		//				var transform = new Transform( Vector3.Zero, Matrix3.LookAt( Vector3.ZAxis, Vector3.YAxis ).ToQuaternion() );
		//				Trunks.Add( CreateElementRibbon( null, material, transform, length, 0, false, 0 ) );
		//			}
		//		}
		//		break;
		//	}
		//}

		///////////////////////////////////////////////

		//void GenerateStage_PoaAnnua( ElementTypeEnum stage )
		//{
		//	switch( stage )
		//	{
		//	case ElementTypeEnum.Trunk:
		//		{
		//			var material = FindSuitableMaterial( PlantMaterial.PartTypeEnum.BranchWithLeaves );

		//			var length = Height;

		//			{
		//				var transform = new Transform( Vector3.Zero, Matrix3.LookAt( Vector3.ZAxis, Vector3.XAxis ).ToQuaternion() );
		//				Trunks.Add( CreateElementRibbon( null, material, transform, length, 0, false, 0 ) );
		//			}

		//			{
		//				var transform = new Transform( Vector3.Zero, Matrix3.LookAt( Vector3.ZAxis, Vector3.YAxis ).ToQuaternion() );
		//				Trunks.Add( CreateElementRibbon( null, material, transform, length, 0, false, 0 ) );
		//			}
		//		}
		//		break;
		//	}
		//}

		///////////////////////////////////////////////

		//!!!!
		//void GenerateStage_CyperusEsculentus( ElementTypeEnum stage )
		//{
		//	//!!!!всё тут

		//	//switch( stage )
		//	//{
		//	//case ElementTypeEnum.Trunk:
		//	//	{
		//	//		Trunks.Add( CreateTrunk( 0.01, 0.01, 0 ) );

		//	//		//hide
		//	//		Trunks[ 0 ].GenerateMeshData = false;
		//	//	}
		//	//	break;

		//	//case ElementTypeEnum.Leaf:
		//	//	{
		//	//		var count = 20;

		//	//		for( int n = 0; n < count; n++ )
		//	//		{
		//	//			var length = Height * Randomizer.Next( 0.8, 1.2 );
		//	//			var material = FindSuitableMaterial( PlantMaterial.PartTypeEnum.Leaf );

		//	//			//!!!!
		//	//			//про удлинение из-за изгиба
		//	//			//изгиб делать шейдером

		//	//			//!!!!
		//	//			//double width;
		//	//			//if( material != null && material.UVWidth.Value != 0 )
		//	//			//	width = length * material.UVLengthRange.Value.Size / material.UVWidth.Value;
		//	//			//else
		//	//			//	width = length / 20;

		//	//			Leaves.Add( CreateGrassBladeRibbon( Trunks[ 0 ], material, length, Randomizer.Next( 10, 45 ), 0.5 ) );
		//	//		}
		//	//	}
		//	//	break;
		//	//}
		//}

		///////////////////////////////////////////////

		protected virtual void GenerateStructure()
		{
			foreach( ElementTypeEnum elementType in Enum.GetValues( typeof( ElementTypeEnum ) ) )
			{
				////predefined configurations
				//switch( PlantType.PredefinedConfiguration.Value )
				//{

				//case PlantType.PredefinedConfigurationEnum.QuercusRobur: GenerateStage_QuercusRobur( elementType ); break;
				//case PlantType.PredefinedConfigurationEnum.MatricariaChamomilla: GenerateStage_MatricariaChamomilla( elementType ); break;
				//case PlantType.PredefinedConfigurationEnum.DoronicumGrandiflorum: GenerateStage_DoronicumGrandiflorum( elementType ); break;
				//case PlantType.PredefinedConfigurationEnum.AlopecurusPratensis: GenerateStage_AlopecurusPratensis( elementType ); break;
				//case PlantType.PredefinedConfigurationEnum.PoaAnnua: GenerateStage_PoaAnnua( elementType ); break;

				//!!!!
				//case PlantType.PredefinedConfigurationEnum.TanacetumCoccineum: GenerateStage_MatricariaChamomilla( elementType ); break;

				//case PlantType.PredefinedConfigurationEnum.CommonPoppy: GenerateStage_CommonPoppy( elementType ); break;
				//case PlantType.PredefinedConfigurationEnum.CyperusEsculentus: GenerateStage_CyperusEsculentus( elementType ); break;

				//}

				//event for scripts
				PlantType.PerformGenerateStage( this, elementType );
			}
		}
	}
}
#endif