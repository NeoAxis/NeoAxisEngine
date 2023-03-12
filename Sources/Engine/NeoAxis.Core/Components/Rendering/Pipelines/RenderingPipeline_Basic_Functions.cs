// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;

namespace NeoAxis
{
	public partial class RenderingPipeline_Basic
	{
		double[] GetShadowCascadeSplitDistances( ViewportRenderingContext context )
		{
			var result = new double[ ShadowDirectionalLightCascades + 1 ];

			int index = 0;
			result[ index++ ] = context.Owner.CameraSettings.NearClipDistance;

			if( ShadowDirectionalLightCascades >= 2 )
			{
				double[] values = new double[ ShadowDirectionalLightCascades ];
				values[ 0 ] = 1;
				for( int n = 1; n < values.Length; n++ )
					values[ n ] = values[ n - 1 ] * ShadowDirectionalLightCascadeDistribution;

				double total = 0;
				foreach( var v in values )
					total += v;
				if( total <= 0.01 )
					total = 0.01;

				double current = 0;
				for( int n = 0; n < values.Length - 1; n++ )
				{
					current += values[ n ];
					result[ index++ ] = ShadowFarDistance * current / total;
				}
			}

			//if( ShadowDirectionalLightCascades >= 2 )
			//	result[ index++ ] = ShadowDirectionalLightCascadesSplit1.Value * ShadowFarDistance;
			//if( ShadowDirectionalLightCascades >= 3 )
			//	result[ index++ ] = ShadowDirectionalLightCascadesSplit2.Value * ShadowFarDistance;
			//if( ShadowDirectionalLightCascades >= 4 )
			//	result[ index++ ] = ShadowDirectionalLightCascadesSplit3.Value * ShadowFarDistance;

			result[ index++ ] = ShadowFarDistance;

			return result;
		}

		const double directionalLightShadowsCascadeOverlapping = 1.1;

		Frustum GetDirectionalLightShadowsFrustum( ViewportRenderingContext context, int cascadeIndex )
		{
			var splitDistances = GetShadowCascadeSplitDistances( context );

			var nearDistance = splitDistances[ cascadeIndex ];
			var farDistance = splitDistances[ cascadeIndex + 1 ] * directionalLightShadowsCascadeOverlapping;
			//var farDistance = splitDistances[ cascadeIndex + 1 ];
			if( nearDistance < .0001 )
				nearDistance = .0001;
			if( farDistance < nearDistance + .01 )
				farDistance = nearDistance + .01;

			var frustum = context.Owner.CameraSettings.Frustum.Clone( false );
			frustum.NearDistance = nearDistance;
			frustum.MoveFarDistance( farDistance );

			return frustum;
		}

		Vector3[] GetDirectionalLightShadowsCameraCornerPoints( ViewportRenderingContext context, int cascadeIndex )
		{
			var splitDistances = GetShadowCascadeSplitDistances( context );

			var nearDistance = splitDistances[ cascadeIndex ];
			var farDistance = splitDistances[ cascadeIndex + 1 ] * directionalLightShadowsCascadeOverlapping;
			//var farDistance = splitDistances[ cascadeIndex + 1 ];
			if( nearDistance < .0001 )
				nearDistance = .0001;
			if( farDistance < nearDistance + .01 )
				farDistance = nearDistance + .01;

			var frustum = context.Owner.CameraSettings.Frustum.Clone( false );
			frustum.NearDistance = nearDistance;
			frustum.MoveFarDistance( farDistance );

			if( nearDistance < 1 )
			{
				var array = new Vector3[ 5 ];
				array[ 0 ] = frustum.Origin;
				for( int n = 0; n < 4; n++ )
					array[ 1 + n ] = frustum.Points[ 4 + n ];
				return array;
			}
			else
				return frustum.Points;

			//bool lastTextureIndex = cascadeIndex == ShadowDirectionalLightCascades - 1;
			//return GetDirectionalLightShadowsCameraCornerPoints( context, nearDistance, farDistance, lastTextureIndex );
		}

		static Vector3 GetDirectionalLightCameraDestinationPoint( ViewportRenderingContext context, Vector3[] cornerPoints )
		{
			if( context.Owner.CameraSettings.Projection == ProjectionType.Perspective )
			{
				//perspective camera

				Ray cameraDirectionAsRay = new Ray( context.Owner.CameraSettings.Position, context.Owner.CameraSettings.Rotation.GetForward() );

				Vector3 nearPoint = cornerPoints[ 0 ];
				Vector3 farPoint = cornerPoints[ 4 ];

				Vector3 projectedPoint = MathAlgorithms.ProjectPointToLine( cameraDirectionAsRay.Origin,
					cameraDirectionAsRay.Origin + cameraDirectionAsRay.Direction, farPoint );

				if( ( projectedPoint - farPoint ).Length() >= ( projectedPoint - nearPoint ).Length() )
				{
					return projectedPoint;
				}
				else
				{
					Vector3 centerBetweenPoints = ( nearPoint + farPoint ) / 2;

					Vector3 normal = ( farPoint - centerBetweenPoints ).GetNormalize();
					Plane plane = Plane.FromPointAndNormal( centerBetweenPoints, normal );

					plane.Intersects( cameraDirectionAsRay, out double scale );
					return cameraDirectionAsRay.GetPointOnRay( scale );
				}
			}
			else
			{
				//orthographic camera

				Vector3 destinationPoint = Vector3.Zero;
				foreach( Vector3 point in cornerPoints )
					destinationPoint += point;
				destinationPoint /= (float)cornerPoints.Length;

				return destinationPoint;
			}
		}

		////!!!!
		///// <summary>Computes the convex hull of a polygon, in clockwise order in a Y-up 
		///// coordinate system (counterclockwise in a Y-down coordinate system).</summary>
		///// <remarks>Uses the Monotone Chain algorithm, a.k.a. Andrew's Algorithm.</remarks>
		//public static List<Vector2> ComputeConvexHull( IEnumerable<Vector2> points )
		//{
		//	var list = new List<Vector2>( points );
		//	return ComputeConvexHull( list, true );
		//}
		//public static List<Vector2> ComputeConvexHull( List<Vector2> points, bool sortInPlace )//= false )
		//{
		//	if( !sortInPlace )
		//		points = new List<Vector2>( points );

		//	points.Sort( ( a, b ) => a.X == b.X ? a.Y.CompareTo( b.Y ) : ( a.X > b.X ? 1 : -1 ) );

		//	// Importantly, DList provides O(1) insertion at beginning and end
		//	List<Vector2> hull = new List<Vector2>();
		//	int L = 0, U = 0; // size of lower and upper hulls

		//	// Builds a hull such that the output polygon starts at the leftmost point.
		//	for( int i = points.Count - 1; i >= 0; i-- )
		//	{
		//		Vector2 p = points[ i ], p1;

		//		// build lower hull (at end of output list)
		//		while( L >= 2 && ( p1 = hull.Last ).Sub( hull[ hull.Count - 2 ] ).Cross( p.Sub( p1 ) ) >= 0 )
		//		{
		//			hull.RemoveAt( hull.Count - 1 );
		//			L--;
		//		}
		//		hull.PushLast( p );
		//		L++;

		//		// build upper hull (at beginning of output list)
		//		while( U >= 2 && ( p1 = hull.First ).Sub( hull[ 1 ] ).Cross( p.Sub( p1 ) ) <= 0 )
		//		{
		//			hull.RemoveAt( 0 );
		//			U--;
		//		}
		//		if( U != 0 ) // share the point added above, except in the first iteration
		//			hull.PushFirst( p );
		//		U++;
		//	}
		//	hull.RemoveAt( hull.Count - 1 );
		//	return hull;
		//}

		void GetDirectionalLightShadowsCascadeHullPlanes( ViewportRenderingContext context, LightItem lightItem, int cascadeIndex, out Plane[] planes, out Bounds bounds )
		{
			var cornerPoints = GetDirectionalLightShadowsCameraCornerPoints( context, cascadeIndex );

			var inputVertices = new List<Vector3>( cornerPoints.Length * 2 );
			inputVertices.AddRange( cornerPoints );
			foreach( var point in cornerPoints )
				inputVertices.Add( point - lightItem.data.Rotation.ToQuaternion().GetForward() * ShadowDirectionalLightExtrusionDistance );

			var projectMatrix = lightItem.data.Rotation.ToQuaternion().GetInverse();
			var unprojectMatrix = lightItem.data.Rotation.ToQuaternion();

			var projected2D = new Vector2[ cornerPoints.Length ];
			for( int n = 0; n < projected2D.Length; n++ )
			{
				var p = projectMatrix * cornerPoints[ n ];
				projected2D[ n ] = new Vector2( p.Y, p.Z );
			}

			var convex = MathAlgorithms.GetConvexByPoints( projected2D );

			var planesList = new List<Plane>( convex.Count );
			for( int n = 0; n < convex.Count; n++ )
			{
				var p1 = convex[ n ];
				var p2 = convex[ ( n + 1 ) % convex.Count ];

				var u1 = unprojectMatrix * new Vector3( 0, p1.X, p1.Y );
				var u2 = unprojectMatrix * new Vector3( 0, p2.X, p2.Y );
				var u3 = unprojectMatrix * new Vector3( 1, p2.X, p2.Y );

				planesList.Add( Plane.FromPoints( u1, u3, u2 ) );
			}
			planes = planesList.ToArray();

			////!!!!глючит
			//ConvexHullAlgorithm.Create( inputVertices.ToArray(), out planes );

			bounds = Bounds.Cleared;
			foreach( var p in inputVertices )
				bounds.Add( p );
		}

		//static Vec3[] GetDirectionalLightShadowsCameraCornerPoints( ViewportRenderingContext context, double initialNearDistance,
		//	double initialFarDistance, bool clipByShadowFarDistance )
		//{
		//	double nearDistance = initialNearDistance;
		//	double farDistance = initialFarDistance;

		//	var frustum = context.Owner.CameraSettings.Frustum.Clone( false );

		//	////clip by shadow far distance sphere (only for perspective camera)
		//	//if( context.Owner.CameraSettings.Projection == ProjectionType.Perspective && clipByShadowFarDistance )
		//	//{
		//	//	Vec3[] points = frustum.Points;

		//	//	Sphere sphere = new Sphere( context.Owner.CameraSettings.Position, farDistance );

		//	//	Vec3[] intersections = new Vec3[ 3 ];
		//	//	for( int n = 0; n < 3; n++ )
		//	//	{
		//	//		Vec3 pointEnd = points[ n + 4 ];

		//	//		Ray ray = new Ray( context.Owner.CameraSettings.Position, pointEnd - context.Owner.CameraSettings.Position );
		//	//		sphere.Intersects( ray, out double scale1, out double scale2 );
		//	//		double scale = Math.Max( scale1, scale2 );

		//	//		intersections[ n ] = ray.GetPointOnRay( scale );
		//	//	}

		//	//	Plane farPlane = Plane.FromPoints( intersections[ 0 ], intersections[ 1 ], intersections[ 2 ] );
		//	//	Ray cameraDirectionAsRay = new Ray( context.Owner.CameraSettings.Position, context.Owner.CameraSettings.Rotation.GetForward() );
		//	//	farPlane.Intersects( cameraDirectionAsRay, out Vec3 pointByDirection );

		//	//	farDistance = ( pointByDirection - context.Owner.CameraSettings.Position ).Length();
		//	//	if( nearDistance + 5 > farDistance )
		//	//		farDistance = nearDistance + 5;
		//	//}

		//	if( nearDistance < .0001 )
		//		nearDistance = .0001;
		//	if( farDistance < nearDistance + .01 )
		//		farDistance = nearDistance + .01;

		//	frustum.NearDistance = nearDistance;
		//	frustum.MoveFarDistance( farDistance );

		//	return frustum.Points;
		//}

		//public Plane[] GetClipPlanesForDirectionalLightShadowGeneration( ViewportRenderingContext context, LightItem lightItem, int pssmTextureIndex )
		//{
		//	Viewport viewportOwner = context.Owner;
		//	var lightDirection = lightItem.data.transform.Rotation.GetForward();

		//	var newFrustum = viewportOwner.CameraSettings.Frustum.Clone( false );

		//	//!!!!надо
		//	//!!!!настроить pssmTextureIndex
		//	//if( ShadowDirectionalLightCascades != 1 )
		//	//{
		//	//float[] splitDistances = SceneManager.Instance.ShadowDirectionalLightSplitDistances;

		//	//float nearSplitDistance = splitDistances[ pssmTextureIndex ];
		//	//float farSplitDistance = splitDistances[ pssmTextureIndex + 1 ];

		//	//const float splitPadding = 1;

		//	//float splitCount = splitDistances.Length - 1;
		//	//if( pssmTextureIndex > 0 )
		//	//	nearSplitDistance -= splitPadding;
		//	//if( pssmTextureIndex < splitCount - 1 )
		//	//	farSplitDistance += splitPadding;

		//	//if( nearSplitDistance < mainCamera.NearClipDistance )
		//	//	nearSplitDistance = mainCamera.NearClipDistance;
		//	//if( farSplitDistance <= nearSplitDistance + .001f )
		//	//	farSplitDistance = nearSplitDistance + .001f;

		//	//mainFrustum.NearDistance = nearSplitDistance;
		//	//mainFrustum.MoveFarDistance( farSplitDistance );
		//	//}
		//	//else
		//	//{
		//	if( ShadowFarDistance > newFrustum.NearDistance )
		//		newFrustum.MoveFarDistance( ShadowFarDistance );
		//	//}

		//	List<Plane> clipPlanes = new List<Plane>( 64 );

		//	{
		//		Quat lightRotation = Quat.FromDirectionZAxisUp( lightDirection );

		//		Vec3[] frustumPoints = newFrustum.Points;

		//		Vec3 frustumCenterPoint;
		//		{
		//			frustumCenterPoint = Vec3.Zero;
		//			foreach( Vec3 point in frustumPoints )
		//				frustumCenterPoint += point;
		//			frustumCenterPoint /= frustumPoints.Length;
		//		}

		//		//calculate frustum points projected to 2d from light direction.
		//		Vec2[] projectedFrustumPoints = new Vec2[ frustumPoints.Length ];
		//		{
		//			//Quat invertFrustumAxis = lightCameraFrustum.Rotation.GetInverse();
		//			Quat lightRotationInv = lightRotation.GetInverse();
		//			Vec3 translate = frustumCenterPoint - lightDirection * 1000;

		//			for( int n = 0; n < frustumPoints.Length; n++ )
		//			{
		//				Vec3 point = frustumPoints[ n ] - translate;
		//				Vec3 localPoint = lightRotationInv * point;
		//				projectedFrustumPoints[ n ] = new Vec2( localPoint.Y, localPoint.Z );
		//			}
		//		}

		//		int[] edges = ConvexPolygon.GetFromPoints( projectedFrustumPoints, .001f );

		//		for( int n = 0; n < edges.Length; n++ )
		//		{
		//			Vec3 point1 = frustumPoints[ edges[ n ] ];
		//			Vec3 point2 = frustumPoints[ edges[ ( n + 1 ) % edges.Length ] ];

		//			Plane plane = Plane.FromVectors( lightDirection,
		//				( point2 - point1 ).GetNormalize(), point1 );

		//			clipPlanes.Add( plane );
		//		}
		//	}

		//	//add main frustum clip planes
		//	foreach( Plane plane in newFrustum.Planes )
		//	{
		//		if( Vec3.Dot( plane.Normal, lightDirection ) < 0 )
		//			continue;

		//		clipPlanes.Add( plane );
		//	}

		//	//add directionalLightExtrusionDistance clip plane
		//	{
		//		Quat rot = Quat.FromDirectionZAxisUp( lightDirection );
		//		Vec3 p = viewportOwner.CameraSettings.Position - lightDirection * ShadowDirectionalLightExtrusionDistance;
		//		Plane plane = Plane.FromVectors( rot * Vec3.ZAxis, rot * Vec3.YAxis, p );
		//		clipPlanes.Add( plane );
		//	}

		//	return clipPlanes.ToArray();
		//}

		enum PointLightFace
		{
			PositiveX,
			NegativeX,
			PositiveY,
			NegativeY,
			PositiveZ,
			NegativeZ,
		}

		static Plane[] GetPlanesForPointLightShadowGeneration( /*Vec3 lightPosition, */PointLightFace face )
		{
			Vector3[] points = new Vector3[ 4 ];

			switch( face )
			{
			case PointLightFace.PositiveX:
				points[ 0 ] = new Vector3( 1, -1, -1 );
				points[ 1 ] = new Vector3( 1, -1, 1 );
				points[ 2 ] = new Vector3( 1, 1, 1 );
				points[ 3 ] = new Vector3( 1, 1, -1 );
				break;
			case PointLightFace.NegativeX:
				points[ 0 ] = new Vector3( -1, 1, -1 );
				points[ 1 ] = new Vector3( -1, 1, 1 );
				points[ 2 ] = new Vector3( -1, -1, 1 );
				points[ 3 ] = new Vector3( -1, -1, -1 );
				break;
			case PointLightFace.PositiveY:
				points[ 0 ] = new Vector3( 1, 1, -1 );
				points[ 1 ] = new Vector3( 1, 1, 1 );
				points[ 2 ] = new Vector3( -1, 1, 1 );
				points[ 3 ] = new Vector3( -1, 1, -1 );
				break;
			case PointLightFace.NegativeY:
				points[ 0 ] = new Vector3( -1, -1, -1 );
				points[ 1 ] = new Vector3( -1, -1, 1 );
				points[ 2 ] = new Vector3( 1, -1, 1 );
				points[ 3 ] = new Vector3( 1, -1, -1 );
				break;
			case PointLightFace.PositiveZ:
				points[ 0 ] = new Vector3( -1, 1, 1 );
				points[ 1 ] = new Vector3( 1, 1, 1 );
				points[ 2 ] = new Vector3( 1, -1, 1 );
				points[ 3 ] = new Vector3( -1, -1, 1 );
				break;
			case PointLightFace.NegativeZ:
				points[ 0 ] = new Vector3( -1, -1, -1 );
				points[ 1 ] = new Vector3( 1, -1, -1 );
				points[ 2 ] = new Vector3( 1, 1, -1 );
				points[ 3 ] = new Vector3( -1, 1, -1 );
				break;
			}

			Plane[] clipPlanes = new Plane[ 4 ];
			for( int n = 0; n < 4; n++ )
				clipPlanes[ n ] = Plane.FromPoints( Vector3.Zero, points[ ( n + 1 ) % 4 ], points[ n ] );
			//clipPlanes[ n ] = Plane.FromPoints( lightPosition, lightPosition + points[ ( n + 1 ) % 4 ], lightPosition + points[ n ] );
			return clipPlanes;
		}

		static Plane[][] pointLightShadowGenerationPlanes;

		void PointLightShadowGenerationCheckAddFaces( ref Vector3 lightPosition, ref Sphere meshBoundingSphere, bool[] result )
		{
			if( pointLightShadowGenerationPlanes == null )
			{
				pointLightShadowGenerationPlanes = new Plane[ 6 ][];
				for( int nFace = 0; nFace < 6; nFace++ )
				{
					PointLightFace face = PointLightFace.PositiveX;

					//flipped
					switch( nFace )
					{
					case 0: face = PointLightFace.NegativeY; break;
					case 1: face = PointLightFace.PositiveY; break;
					case 2: face = PointLightFace.PositiveZ; break;
					case 3: face = PointLightFace.NegativeZ; break;
					case 4: face = PointLightFace.PositiveX; break;
					case 5: face = PointLightFace.NegativeX; break;
					}

					//PC: OriginBottomLeft = false, HomogeneousDepth = false
					//Android: OriginBottomLeft = true, HomogeneousDepth = true
					var caps = RenderingSystem.Capabilities;
					if( caps.OriginBottomLeft )
					{
						switch( nFace )
						{
						case 2: face = PointLightFace.NegativeZ; break;
						case 3: face = PointLightFace.PositiveZ; break;
						}
					}

					var planes = GetPlanesForPointLightShadowGeneration(/* Vec3.Zero, */face );
					pointLightShadowGenerationPlanes[ nFace ] = planes;
				}
			}

			var sphereLocalOrigin = meshBoundingSphere.Center - lightPosition;

			for( int nFace = 0; nFace < 6; nFace++ )
			{
				var planes = pointLightShadowGenerationPlanes[ nFace ];

				bool add = true;
				for( int n = 0; n < planes.Length; n++ )
				{
					double distance = planes[ n ].GetDistance( sphereLocalOrigin );// meshSphere.Origin );
					if( distance < -meshBoundingSphere.Radius )
					{
						add = false;
						break;
					}
				}

				result[ nFace ] = add;
			}
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		public enum DownscalingModeEnum
		{
			Auto,
			Manual,
		}

		public class GaussianBlurSettings
		{
			public ImageComponent SourceTexture;
			//public RenderingEffect ForEffect;
			public double BlurFactor = 1;
			public DownscalingModeEnum DownscalingMode = DownscalingModeEnum.Auto;
			public int DownscalingValue;
			public double StandardDeviation = 3;

			public ImageComponent BlendResultWithTexture;
			public double BlendResultWithTextureIntensity;
		}

		public virtual ImageComponent GaussianBlur( ViewportRenderingContext context, GaussianBlurSettings settings )
		{
			ImageComponent currentTexture = settings.SourceTexture;

			//downscaling

			int downscalingValue2;
			if( settings.DownscalingMode == DownscalingModeEnum.Auto )
			{
				downscalingValue2 = (int)( settings.BlurFactor / 2.0 + 0.999 );
				if( settings.BlurFactor <= 1 )
					downscalingValue2 = 0;
				//downscalingValue2 = (int)( Math.Pow( blurFactor, 1.0 / 1.1 ) ) - 1;
				//downscalingValue2 = (int)( Math.Sqrt( blurFactor ) ) - 1;
				//downscalingValue2 = (int)( blurFactor / 1.5 );
			}
			else
				downscalingValue2 = settings.DownscalingValue;

			for( int n = 0; n < downscalingValue2; n++ )
			{
				var size = currentTexture.Result.ResultSize / 2;
				if( size.X < 1 ) size.X = 1;
				if( size.Y < 1 ) size.Y = 1;

				var texture = context.RenderTarget2D_Alloc( size, currentTexture.Result.ResultFormat );

				context.SetViewport( texture.Result.GetRenderTarget().Viewports[ 0 ] );

				var shader = new CanvasRenderer.ShaderItem();
				shader.VertexProgramFileName = @"Base\Shaders\EffectsCommon_vs.sc";
				shader.FragmentProgramFileName = @"Base\Shaders\Effects\Downscale2_fs.sc";

				shader.Parameters.Set( "sourceSizeInv", new Vector2F( 1, 1 ) / currentTexture.Result.ResultSize.ToVector2F() );

				//shader.Parameters.Set( "0"/*"sourceTexture"*/, new GpuMaterialPass.TextureParameterValue( actualTexture,
				//	TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.Point ) );
				shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 0/*"sourceTexture"*/, currentTexture,
					TextureAddressingMode.Clamp, FilterOption.Linear, FilterOption.Linear, FilterOption.Point ) );

				context.RenderQuadToCurrentViewport( shader );

				if( currentTexture != settings.SourceTexture )
					context.DynamicTexture_Free( currentTexture );
				currentTexture = texture;
			}

			//horizontal blur
			//if( dimensions == BlurDimensionsEnum.HorizontalAndVertical || dimensions == BlurDimensionsEnum.Horizontal )
			{
				var texture = context.RenderTarget2D_Alloc( settings.SourceTexture.Result.ResultSize, currentTexture.Result.ResultFormat );
				//var texture = context.RenderTarget2D_Alloc( currentTexture.Result.Size, currentTexture.Result.Format );
				{
					context.SetViewport( texture.Result.GetRenderTarget().Viewports[ 0 ] );

					var shader = new CanvasRenderer.ShaderItem();
					shader.VertexProgramFileName = @"Base\Shaders\EffectsCommon_vs.sc";
					shader.FragmentProgramFileName = @"Base\Shaders\Effects\Blur_fs.sc";

					shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 0/*"s_sourceTexture"*/, currentTexture,
						TextureAddressingMode.Clamp, FilterOption.Linear, FilterOption.Linear, FilterOption.Point ) );

					var values = GaussianBlurMath.Calculate15( texture.Result.ResultSize, true, settings.BlurFactor, settings.StandardDeviation );//, settings.Intensity );
					shader.Parameters.Set( "sampleOffsets", values.SampleOffsetsAsVector4Array );
					shader.Parameters.Set( "sampleWeights", values.SampleWeights );

					context.RenderQuadToCurrentViewport( shader );
				}

				if( currentTexture != settings.SourceTexture )
					context.DynamicTexture_Free( currentTexture );
				currentTexture = texture;
			}

			//vertical blur
			//if( dimensions == BlurDimensionsEnum.HorizontalAndVertical || dimensions == BlurDimensionsEnum.Vertical )
			{
				var texture = context.RenderTarget2D_Alloc( settings.SourceTexture.Result.ResultSize, currentTexture.Result.ResultFormat );
				//var texture = context.RenderTarget2D_Alloc( currentTexture.Result.Size, currentTexture.Result.Format );
				{
					context.SetViewport( texture.Result.GetRenderTarget().Viewports[ 0 ] );

					var shader = new CanvasRenderer.ShaderItem();
					shader.VertexProgramFileName = @"Base\Shaders\EffectsCommon_vs.sc";
					shader.FragmentProgramFileName = @"Base\Shaders\Effects\Blur_fs.sc";

					shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 0/*"s_sourceTexture"*/, currentTexture, TextureAddressingMode.Clamp, FilterOption.Linear, FilterOption.Linear, FilterOption.Point ) );

					var values = GaussianBlurMath.Calculate15( texture.Result.ResultSize, false, settings.BlurFactor, settings.StandardDeviation );//, settings.Intensity );
					shader.Parameters.Set( "sampleOffsets", values.SampleOffsetsAsVector4Array );
					shader.Parameters.Set( "sampleWeights", values.SampleWeights );

					if( settings.BlendResultWithTexture != null )
					{
						shader.Defines.Add( new CanvasRenderer.ShaderItem.DefineItem( "BLEND_WITH_TEXTURE" ) );

						shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 1/*"s_blendWithTexture"*/, settings.BlendResultWithTexture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.None ) );

						shader.Parameters.Set( "intensity", (float)settings.BlendResultWithTextureIntensity );
					}

					context.RenderQuadToCurrentViewport( shader );
				}

				if( currentTexture != settings.SourceTexture )
					context.DynamicTexture_Free( currentTexture );
				currentTexture = texture;
			}

			return currentTexture;
		}

		public ImageComponent GaussianBlur( ViewportRenderingContext context, ImageComponent sourceTexture, double blurFactor, DownscalingModeEnum downscalingMode, int downscalingValue )
		{
			var settings = new GaussianBlurSettings();
			settings.SourceTexture = sourceTexture;
			settings.BlurFactor = blurFactor;
			settings.DownscalingMode = downscalingMode;
			settings.DownscalingValue = downscalingValue;

			return GaussianBlur( context, settings );
		}

		//void GaussianBlurShadowVSM( ViewportRenderingContext context, Light.TypeEnum lightType, ImageComponent shadowTexture, double blurFactor/*, DownscalingModeEnum downscalingMode, int downscalingValue*/ )
		//{
		//	int iterationCount = 1;
		//	//!!!!
		//	//if( lightData.Type == Light.TypeEnum.Point )
		//	//	iterationCount = 6;
		//	if( lightType == Light.TypeEnum.Directional )
		//		iterationCount = ShadowDirectionalLightCascades;

		//	for( int nIteration = 0; nIteration < iterationCount; nIteration++ )
		//	{
		//		var tempTexture = context.RenderTarget2D_Alloc( shadowTexture.Result.ResultSize, shadowTexture.Result.ResultFormat );

		//		//horizontal blur
		//		{
		//			context.SetViewport( tempTexture.Result.GetRenderTarget().Viewports[ 0 ] );

		//			var shader = new CanvasRenderer.ShaderItem();
		//			shader.VertexProgramFileName = @"Base\Shaders\EffectsCommon_vs.sc";
		//			shader.FragmentProgramFileName = @"Base\Shaders\Effects\Blur_fs.sc";

		//			if( lightType == Light.TypeEnum.Directional )
		//				shader.Defines.Add( new CanvasRenderer.ShaderItem.DefineItem( "BLUR_SAMPLE2D_FROM_ARRAY", nIteration.ToString() ) );

		//			shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 0/*"sourceTexture"*/, shadowTexture, TextureAddressingMode.Clamp, FilterOption.Linear, FilterOption.Linear, FilterOption.Point ) );

		//			var values = GaussianBlurMath.Calculate15( tempTexture.Result.ResultSize, true, blurFactor );
		//			shader.Parameters.Set( "sampleOffsets", values.SampleOffsetsAsVector4Array );
		//			shader.Parameters.Set( "sampleWeights", values.SampleWeights );

		//			context.RenderQuadToCurrentViewport( shader );
		//		}

		//		//vertical blur
		//		{
		//			context.SetViewport( shadowTexture.Result.GetRenderTarget( 0, nIteration ).Viewports[ 0 ] );

		//			var shader = new CanvasRenderer.ShaderItem();
		//			shader.VertexProgramFileName = @"Base\Shaders\EffectsCommon_vs.sc";
		//			shader.FragmentProgramFileName = @"Base\Shaders\Effects\Blur_fs.sc";

		//			shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 0/*"sourceTexture"*/, tempTexture, TextureAddressingMode.Clamp, FilterOption.Linear, FilterOption.Linear, FilterOption.Point ) );

		//			var values = GaussianBlurMath.Calculate15( tempTexture.Result.ResultSize, false, blurFactor );
		//			shader.Parameters.Set( "sampleOffsets", values.SampleOffsetsAsVector4Array );
		//			shader.Parameters.Set( "sampleWeights", values.SampleWeights );

		//			context.RenderQuadToCurrentViewport( shader );
		//		}

		//		context.DynamicTexture_Free( tempTexture );
		//	}
		//}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		public virtual void ConvertToLDR( ViewportRenderingContext context, ref ImageComponent actualTexture )
		{
			//!!!!может другие тоже форматы? проверять на FloatXX?
			var demandFormat = PixelFormat.A8R8G8B8;

			if( actualTexture.Result.ResultFormat != demandFormat )
			{
				var newTexture = context.RenderTarget2D_Alloc( actualTexture.Result.ResultSize, demandFormat );

				context.SetViewport( newTexture.Result.GetRenderTarget().Viewports[ 0 ] );

				CanvasRenderer.ShaderItem shader = new CanvasRenderer.ShaderItem();
				shader.VertexProgramFileName = @"Base\Shaders\EffectsCommon_vs.sc";
				shader.FragmentProgramFileName = @"Base\Shaders\Effects\ToLDR_fs.sc";

				shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 0/*"sourceTexture"*/, actualTexture,
					TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.None ) );

				//var size = context.owner.DimensionsInPixels.Size;
				//shader.Parameters.Set( "viewportSize", new Vec4( size.X, size.Y, 1.0 / (double)size.X, 1.0 / (double)size.Y ).ToVec4F() );

				//Mat4F identity = Mat4F.Identity;
				//shader.Parameters.Set( "worldViewProjMatrix", ParameterType.Matrix4x4, 1, &identity, sizeof( Mat4F ) );

				context.RenderQuadToCurrentViewport( shader );

				//free old texture
				context.DynamicTexture_Free( actualTexture );

				actualTexture = newTexture;
			}
		}
	}
}
