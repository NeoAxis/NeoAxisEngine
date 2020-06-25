// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Runtime.InteropServices;

namespace NeoAxis
{
	public sealed partial class Viewport
	{
		/// <summary>
		/// Camera settings for rendering to viewport.
		/// </summary>
		public class CameraSettingsClass
		{
			Viewport viewport;
			//Component_Camera sourceCamera;
			bool frustumCullingTest;

			double aspectRatio;
			Degree fieldOfView;
			double nearClipDistance;
			double farClipDistance;
			Vector3 position;
			internal Vector3 direction;
			Vector3 fixedUp;
			ProjectionType projection;
			double height;
			double exposure;
			double emissiveFactor;
			bool reflectionEnabled;
			Plane reflectionPlane;
			Component_RenderingPipeline renderingPipelineOverride;

			Quaternion rotation;
			internal Vector3 up;
			internal Vector3 right;

			Frustum frustum;
			Matrix4 viewMatrix;
			Matrix4 projectionMatrix;

			bool inverseViewProjectionNeedUpdate = true;
			Matrix4 inverseViewProjection;

			//

			public CameraSettingsClass( Viewport viewport, Component_Camera camera, bool frustumCullingTest = false )
			{
				//!!!!reflection

				Transform t = camera.Transform;

				Init( viewport, /*camera, */frustumCullingTest, camera.AspectRatio, camera.FieldOfView, camera.NearClipPlane, camera.FarClipPlane, t.Position, t.Rotation.GetForward()/* camera.Direction*/, camera.FixedUp, camera.Projection, camera.Height, camera.Exposure, camera.EmissiveFactor, false, new Plane(), camera.RenderingPipelineOverride );
			}

			public CameraSettingsClass( Viewport viewport, double aspectRatio, Degree fieldOfView, double nearClipDistance, double farClipDistance,
				Vector3 position, Vector3 direction, Vector3 fixedUp, ProjectionType projection, double height, double exposure, double emissiveFactor, bool reflectionEnabled = false, Plane reflectionPlane = new Plane(), bool frustumCullingTest = false, Component_RenderingPipeline renderingPipelineOverride = null )
			{
				Init( viewport, /*null, */frustumCullingTest, aspectRatio, fieldOfView, nearClipDistance, farClipDistance, position, direction, fixedUp, projection, height, exposure, emissiveFactor, reflectionEnabled, reflectionPlane, renderingPipelineOverride );
			}

			//public CameraSettingsClass( Viewport viewport, double aspectRatio, Degree fieldOfView, double nearClipDistance, double farClipDistance,
			//	Vector3 position, Vector3 direction, Vector3 fixedUp, ProjectionType projection = ProjectionType.Perspective,
			//	double orthographicHeight = 1, bool reflectionEnabled = false, Plane reflectionPlane = new Plane() )
			//{
			//	Init( viewport, /*null, */false, aspectRatio, fieldOfView, nearClipDistance, farClipDistance, position, direction, fixedUp,
			//		projection, orthographicHeight, reflectionEnabled, reflectionPlane );
			//}

			//public CameraSettingsClass( Viewport viewport, double aspectRatio, Degree fieldOfView, double nearClipDistance, double farClipDistance,
			//	Vector3 position, Vector3 direction, Vector3 fixedUp, ProjectionType projection = ProjectionType.Perspective,
			//	double height = 1, bool reflectionEnabled = false, Plane reflectionPlane = new Plane() )
			//{
			//	Init( viewport, /*null, */false, aspectRatio, fieldOfView, nearClipDistance, farClipDistance, position, direction, fixedUp,
			//		projection, height, reflectionEnabled, reflectionPlane );
			//}

			void Init( Viewport viewport, /*Component_Camera sourceCamera, */bool frustumCullingTest, double aspectRatio, Degree fieldOfView, double nearClipDistance, double farClipDistance, Vector3 position, Vector3 direction, Vector3 fixedUp, ProjectionType projection, double height, double exposure, double emissiveFactor, bool reflectionEnabled, Plane reflectionPlane, Component_RenderingPipeline renderingPipelineOverride )
			{
				this.viewport = viewport;
				//this.sourceCamera = sourceCamera;
				this.frustumCullingTest = frustumCullingTest;

				//fix invalid parameters
				//!!!!!!
				if( direction.Length() < .0001 )
					direction = new Vector3( fixedUp.Y, fixedUp.X, fixedUp.Z );
				fixedUp.Normalize();
				direction.Normalize();
				if( direction.Equals( fixedUp, .0001 ) )
					direction = new Vector3( fixedUp.Y, fixedUp.X, fixedUp.Z );

				this.aspectRatio = aspectRatio;
				if( this.aspectRatio <= 0 )
					this.aspectRatio = (double)viewport.SizeInPixels.X / (double)viewport.SizeInPixels.Y;

				this.fieldOfView = fieldOfView;
				this.nearClipDistance = nearClipDistance;
				this.farClipDistance = farClipDistance;
				this.position = position;
				//!!!!this.derivedPosition = position;
				this.direction = direction;
				//!!!!this.derivedDirection = direction;
				this.fixedUp = fixedUp;
				this.projection = projection;
				this.height = height;
				this.exposure = exposure;
				this.emissiveFactor = emissiveFactor;
				this.reflectionEnabled = reflectionEnabled;
				this.reflectionPlane = reflectionPlane;
				this.renderingPipelineOverride = renderingPipelineOverride;

				//!!!!!!
				if( this.reflectionEnabled )
					Log.Fatal( "impl reflectionEnabled" );

				//!!!!new
				this.rotation = Quaternion.LookAt( direction, this.fixedUp );
				//this.rotation = Quat._Remove_FromDirectionZAxisUp( direction );

				//!!!!
				this.direction = rotation.GetForward();
				up = rotation.GetUp();
				right = Vector3.Cross( this.direction, up );

				CalculateFrustum();
				CalculateMatrices();
			}

			public Viewport Viewport
			{
				get { return viewport; }
			}

			//public Component_Camera SourceCamera
			//{
			//	get { return sourceCamera; }
			//}

			//public bool FrustumCullingTest
			//{
			//	get { return frustumCullingTest; }
			//}

			/// <summary>
			/// Gets or sets the aspect ratio for the frustum viewport.
			/// </summary>
			/// <remarks>
			/// <para>
			/// The ratio between the x and y dimensions of the rectangular area visible through the frustum
			/// is known as aspect ratio: aspect = width / height.
			/// </para>
			/// </remarks>
			public double AspectRatio
			{
				get { return aspectRatio; }
			}

			public Degree FieldOfView
			{
				get { return fieldOfView; }
			}

			/// <summary>
			/// Gets the position of the near clipping plane.
			/// </summary>
			/// <remarks>
			/// The position of the near clipping plane is the distance from the frustums position to the screen
			/// on which the world is projected. The near plane distance, combined with the field-of-view and the
			/// aspect ratio, determines the size of the viewport through which the world is viewed (in world
			/// co-ordinates). Note that this world viewport is different to a screen viewport, which has it's
			/// dimensions expressed in pixels. The frustums viewport should have the same aspect ratio as the
			/// screen viewport it renders into to avoid distortion.
			/// </remarks>
			public double NearClipDistance
			{
				get { return nearClipDistance; }
			}

			/// <summary>
			/// Gets the distance to the far clipping plane.
			/// </summary>
			/// <remarks>
			/// <para>
			/// The view frustrum is a pyramid created from the frustum 
			/// position and the edges of the viewport.
			/// This method sets the distance for the far end of that pyramid. 
			/// Different applications need different values: e.g. a flight sim
			/// needs a much further far clipping plane than a first-person 
			/// shooter. An important point here is that the larger the ratio 
			/// between near and far clipping planes, the lower the accuracy of
			/// the Z-buffer used to depth-cue pixels. This is because the
			/// Z-range is limited to the size of the Z buffer (16 or 32-bit) 
			/// and the max values must be spread over the gap between near and
			/// far clip planes. As it happens, you can affect the accuracy far 
			/// more by altering the near distance rather than the far distance, 
			/// but keep this in mind.
			/// </para>
			/// <para>
			/// far The distance to the far clipping plane from the frustum in 
			/// world coordinates.If you specify 0, this means an infinite view
			/// distance which is useful especially when projecting shadows; but
			/// be careful not to use a near distance too close.
			/// </para>
			/// </remarks>
			public double FarClipDistance
			{
				get { return farClipDistance; }
			}

			/// <summary>Gets the camera's position.</summary>
			public Vector3 Position
			{
				get { return position; }
			}

			///// <summary>
			///// Gets the derived position of the camera, including any translation inherited from a reflection matrix.
			///// </summary>
			//public Vec3 DerivedPosition
			//{
			//	get { return derivedPosition; }
			//}

			/// <summary>Gets the camera's direction.</summary>
			public Vector3 Direction
			{
				get { return direction; }
			}

			///// <summary>
			///// Gets the derived direction vector of the camera, including any rotation inherited from a reflection matrix.
			///// </summary>
			//public Vec3 DerivedDirection
			//{
			//	get { return derivedDirection; }
			//}

			/// <summary>Gets the camera's fixed up vector.</summary>
			public Vector3 FixedUp
			{
				get { return fixedUp; }
			}

			/// <summary>
			/// Gets the type of projection to use (orthographic or perspective). Default is perspective.
			/// </summary>
			public ProjectionType Projection
			{
				get { return projection; }
			}

			public double OrthographicHeight
			{
				get { return height; }
			}

			public double Exposure
			{
				get { return exposure; }
			}

			public double EmissiveFactor
			{
				get { return emissiveFactor; }
			}

			/// <summary>Returns whether this frustum is being reflected.</summary>
			public bool ReflectionEnabled
			{
				get { return reflectionEnabled; }
			}

			/// <summary>Returns the reflection plane of the frustum if appropriate.</summary>
			public Plane ReflectionPlane
			{
				get { return reflectionPlane; }
			}

			public Component_RenderingPipeline RenderingPipelineOverride
			{
				get { return renderingPipelineOverride; }
			}

			/// <summary>
			/// Gets the camera's current orientation.
			/// </summary>
			public Quaternion Rotation
			{
				get { return rotation; }
			}

			/// <summary>Gets the camera's up vector.</summary>
			public Vector3 Up
			{
				get { return up; }
			}

			public Vector3 Right
			{
				get { return right; }
			}

			///// <summary>
			///// Gets the derived orientation of the camera, including any rotation inherited from a reflection matrix.
			///// </summary>
			//public Quat DerivedRotation
			//{
			//	get { return derivedRotation; }
			//}


			//!!!!slow?
			public Matrix4 ViewMatrix
			{
				get { return viewMatrix; }
			}

			//!!!!slow?
			public Matrix4 ProjectionMatrix
			{
				get { return projectionMatrix; }
			}

			public Frustum Frustum
			{
				get { return frustum; }
			}

			void CalculateFrustum()
			{
				Vector3 cameraPosition = Position;
				Vector3 cameraDirection = Direction;
				Vector3 cameraUp = Up;
				if( ReflectionEnabled )
				{
					//!!!!!!
					Log.Fatal( "impl" );
					//Mat4 reflectedMatrix = GetReflectionMatrix();
					//Mat3 reflectedMatrix3 = reflectedMatrix.ToMat3();
					//cameraPosition = reflectedMatrix * cameraPosition;
					//cameraDirection = reflectedMatrix3 * cameraDirection;
					//cameraUp = reflectedMatrix3 * cameraUp;
				}

				Quaternion rotation = new Matrix3( cameraDirection, -Vector3.Cross( cameraDirection, cameraUp ), cameraUp ).ToQuaternion();

				double halfWidth;
				double halfHeight;
				if( Projection == ProjectionType.Perspective )
				{
					double tan = Math.Tan( FieldOfView.InRadians() / 2 );
					halfWidth = tan * FarClipDistance * AspectRatio;
					halfHeight = tan * FarClipDistance;
				}
				else
				{
					halfWidth = OrthographicHeight * .5 * AspectRatio;
					halfHeight = OrthographicHeight * .5;
				}

				if( frustumCullingTest )
				{
					halfWidth *= .5;
					halfHeight *= .5;
				}

				frustum = new Frustum( true, Projection, cameraPosition, rotation, NearClipDistance, FarClipDistance, halfWidth, halfHeight );
			}

			void CalcProjectionParameters( out double left, out double right, out double bottom, out double top )
			{
				if( projection == ProjectionType.Perspective )
				{
					Radian thetaY = FieldOfView.InRadians() * .5;
					double tanThetaY = Math.Tan( thetaY );
					double tanThetaX = tanThetaY * aspectRatio;

					double nearFocal = nearClipDistance;// / mFocalLength;
					double nearOffsetX = 0;// mFrustumOffset.x * nearFocal;
					double nearOffsetY = 0;// mFrustumOffset.y * nearFocal;

					double half_w = tanThetaX * nearClipDistance;
					double half_h = tanThetaY * nearClipDistance;

					left = -half_w + nearOffsetX;
					right = +half_w + nearOffsetX;
					bottom = -half_h + nearOffsetY;
					top = +half_h + nearOffsetY;
				}
				else
				{
					// Unknown how to apply frustum offset to orthographic camera, just ignore here
					double half_w = height * aspectRatio * 0.5;
					double half_h = height * 0.5;

					left = -half_w;
					right = half_w;
					bottom = -half_h;
					top = half_h;
				}
			}

			static Matrix4 LookAt( Vector3 eye, Vector3 target, Vector3 up )
			{
				Vector3 zaxis = ( eye - target ).GetNormalize();// The "forward" vector.
				Vector3 xaxis = ( Vector3.Cross( up, zaxis ) ).GetNormalize();// The "right" vector.
				Vector3 yaxis = Vector3.Cross( zaxis, xaxis );// The "up" vector.

				// Create a 4x4 view matrix from the right, up, forward and eye position vectors
				Matrix4 viewMatrix = new Matrix4(
					new Vector4( xaxis.X, yaxis.X, zaxis.X, 0 ),
					new Vector4( xaxis.Y, yaxis.Y, zaxis.Y, 0 ),
					new Vector4( xaxis.Z, yaxis.Z, zaxis.Z, 0 ),
					new Vector4( -Vector3.Dot( xaxis, eye ), -Vector3.Dot( yaxis, eye ), -Vector3.Dot( zaxis, eye ), 1 )
				);

				return viewMatrix;
			}

			void CalculateMatrices()
			{
				double left, right, bottom, top;
				CalcProjectionParameters( out left, out right, out bottom, out top );

				double inv_w = 1.0 / ( right - left );
				double inv_h = 1.0 / ( top - bottom );
				double inv_d = 1.0 / ( farClipDistance - nearClipDistance );

				if( projection == ProjectionType.Perspective )
				{
					// Calc matrix elements
					double A = 2.0 * nearClipDistance * inv_w;
					double B = 2.0 * nearClipDistance * inv_h;
					double C = ( right + left ) * inv_w;
					double D = ( top + bottom ) * inv_h;
					double q = -( farClipDistance + nearClipDistance ) * inv_d;
					double qn = -2.0 * ( farClipDistance * nearClipDistance ) * inv_d;

					// NB: This creates 'uniform' perspective projection matrix,
					// which depth range [-1,1], right-handed rules
					//
					// [ A   0   C   0  ]
					// [ 0   B   D   0  ]
					// [ 0   0   q   qn ]
					// [ 0   0   -1  0  ]
					//
					// A = 2 * near / (right - left)
					// B = 2 * near / (top - bottom)
					// C = (right + left) / (right - left)
					// D = (top + bottom) / (top - bottom)
					// q = - (far + near) / (far - near)
					// qn = - 2 * (far * near) / (far - near)

					projectionMatrix = Matrix4.Zero;
					projectionMatrix.Item0.X = A;
					projectionMatrix.Item0.Z = C;
					projectionMatrix.Item1.Y = B;
					projectionMatrix.Item1.Z = D;
					projectionMatrix.Item2.Z = q;
					projectionMatrix.Item2.W = qn;
					projectionMatrix.Item3.Z = -1;
					projectionMatrix.Transpose();
				}
				else
				{
					double A = 2.0 * inv_w;
					double B = 2.0 * inv_h;
					double C = -( right + left ) * inv_w;
					double D = -( top + bottom ) * inv_h;
					double q = -2.0 * inv_d;
					double qn = -( farClipDistance + nearClipDistance ) * inv_d;

					// NB: This creates 'uniform' orthographic projection matrix,
					// which depth range [-1,1], right-handed rules
					//
					// [ A   0   0   C  ]
					// [ 0   B   0   D  ]
					// [ 0   0   q   qn ]
					// [ 0   0   0   1  ]
					//
					// A = 2 * / (right - left)
					// B = 2 * / (top - bottom)
					// C = - (right + left) / (right - left)
					// D = - (top + bottom) / (top - bottom)
					// q = - 2 / (far - near)
					// qn = - (far + near) / (far - near)

					projectionMatrix = Matrix4.Zero;
					projectionMatrix.Item0.X = A;
					projectionMatrix.Item0.Z = C;
					projectionMatrix.Item1.Y = B;
					projectionMatrix.Item1.W = D;
					projectionMatrix.Item2.Z = q;
					projectionMatrix.Item2.W = qn;
					projectionMatrix.Item3.W = 1;
					projectionMatrix.Transpose();
				}

				//!!!!new. вроде всё норм работает, GetRayByScreenCoordinates тоже
				// Convert depth range from [-1,+1] to [0,1]
				projectionMatrix.Transpose();
				projectionMatrix.Item2 = ( projectionMatrix.Item2 + projectionMatrix.Item3 ) / 2;
				projectionMatrix.Transpose();


				//mProjMatrixRSDepth
				//// Convert depth range from [-1,+1] to [0,1]
				//dest[ 2 ][ 0 ] = ( dest[ 2 ][ 0 ] + dest[ 3 ][ 0 ] ) / 2;
				//dest[ 2 ][ 1 ] = ( dest[ 2 ][ 1 ] + dest[ 3 ][ 1 ] ) / 2;
				//dest[ 2 ][ 2 ] = ( dest[ 2 ][ 2 ] + dest[ 3 ][ 2 ] ) / 2;
				//dest[ 2 ][ 3 ] = ( dest[ 2 ][ 3 ] + dest[ 3 ][ 3 ] ) / 2;


				//RenderSystem* renderSystem = root->renderSystem;
				//// API specific
				//renderSystem->_convertProjectionMatrix( mProjMatrix, mProjMatrixRS );
				//// API specific for Gpu Programs
				//renderSystem->_convertProjectionMatrix( mProjMatrix, mProjMatrixRSDepth, true );

				viewMatrix = LookAt( position, position + direction, up );

				//float nearClip = GetNearClip();
				//float h = ( 1.0f / tanf( fov_ * M_DEGTORAD * 0.5f ) ) * zoom_;
				//float w = h / aspectRatio_;
				//float q, r;

				//if( openGLFormat )
				//{
				//	q = ( farClip_ + nearClip ) / ( farClip_ - nearClip );
				//	r = -2.0f * farClip_ * nearClip / ( farClip_ - nearClip );
				//}
				//else
				//{
				//	q = farClip_ / ( farClip_ - nearClip );
				//	r = -q * nearClip;
				//}

				//ret.m00_ = w;
				//ret.m02_ = projectionOffset_.x_ * 2.0f;
				//ret.m11_ = h;
				//ret.m12_ = projectionOffset_.y_ * 2.0f;
				//ret.m22_ = q;
				//ret.m23_ = r;
				//ret.m32_ = 1.0f;

				//inverseViewProjectionDirty = true;
			}

			static void MultiplyProjectWTo1( ref Matrix4 m, ref Vector3 v, out Vector3 result )
			{
				double invW = 1.0 / ( m.Item0.W * v.X + m.Item1.W * v.Y + m.Item2.W * v.Z + m.Item3.W );
				result = ( m * v ) * invW;
			}

			/// <summary>
			/// Projects world position to screen coordinates.
			/// </summary>
			/// <param name="position">The world position.</param>
			/// <param name="screenPosition">The result screen coordinates.</param>
			/// <returns>
			/// <b>true</b> if screen position successfully received; otherwise, <b>false</b>.
			/// </returns>
			public bool ProjectToScreenCoordinates( ref Vector3 position, out Vector2 screenPosition, bool backsideReturnFalse = true )
			{
				screenPosition = new Vector2( -1, -1 );

				// Don't use getViewMatrix here, incase overrided by camera and return a cull frustum view matrix
				Matrix4.Multiply( ref viewMatrix, ref position, out var eyeSpacePos );
				//Vector3 eyeSpacePos = viewMatrix * position;
				//Vec3 eyeSpacePos = viewMatrix.transformAffine( position );

				//!!!!надо ли? может возвращать значение даже если в задней полуплоскости. или опционально
				if( backsideReturnFalse )
				{
					if( eyeSpacePos.Z >= 0 )
						return false;
					// early-exit
					if( eyeSpacePos.LengthSquared() <= 0 )
						return false;
				}

				MultiplyProjectWTo1( ref projectionMatrix, ref eyeSpacePos, out var screenSpacePos );
				screenPosition = new Vector2( ( screenSpacePos.X + 1.0 ) * .5, 1.0 - ( screenSpacePos.Y + 1.0 ) * .5 );
				return true;
			}

			/// <summary>
			/// Projects world position to screen coordinates.
			/// </summary>
			/// <param name="position">The world position.</param>
			/// <param name="screenPosition">The result screen coordinates.</param>
			/// <returns>
			/// <b>true</b> if screen position successfully received; otherwise, <b>false</b>.
			/// </returns>
			public bool ProjectToScreenCoordinates( Vector3 position, out Vector2 screenPosition, bool backsideReturnFalse = true )
			{
				return ProjectToScreenCoordinates( ref position, out screenPosition, backsideReturnFalse );
			}

			/// <summary>
			/// Generates world ray from screen coordinates.
			/// </summary>
			/// <param name="screenPosition">The screen coordinates.</param>
			/// <returns>The ray.</returns>
			public void GetRayByScreenCoordinates( ref Vector2 screenPosition, out Ray result )
			{
				if( inverseViewProjectionNeedUpdate )
				{
					( projectionMatrix * viewMatrix ).GetInverse( out inverseViewProjection );
					inverseViewProjectionNeedUpdate = false;
				}

				Vector3 nearPoint;
				Vector3 midPoint;
				if( projection == ProjectionType.Perspective )
				{
					double nx = ( 2.0 * screenPosition.X ) - 1.0;
					double ny = 1.0 - ( 2.0 * screenPosition.Y );
					nearPoint = new Vector3( nx, ny, -1.0 );
					// Use midPoint rather than far point to avoid issues with infinite projection
					midPoint = new Vector3( nx, ny, 0.0 );
				}
				else
				{
					double nx = ( 2.0 * screenPosition.X ) - 1.0;
					double ny = 1.0 - ( 2.0 * screenPosition.Y );
					nearPoint = new Vector3( nx, ny, 0 );
					midPoint = new Vector3( nx, ny, 1 );
				}

				// Get ray origin and ray target on near plane in world space
				MultiplyProjectWTo1( ref inverseViewProjection/*inverseVP*/, ref nearPoint, out var rayOrigin );
				MultiplyProjectWTo1( ref inverseViewProjection/*inverseVP*/, ref midPoint, out var rayTarget );

				double distance = FarClipDistance;
				if( distance < 1 )
					distance = 1;
				Vector3 rayDirection = ( rayTarget - rayOrigin ).GetNormalize() * distance;

				result = new Ray( rayOrigin, rayDirection );
			}

			/// <summary>
			/// Generates world ray from screen coordinates.
			/// </summary>
			/// <param name="screenPosition">The screen coordinates.</param>
			/// <returns>The ray.</returns>
			public Ray GetRayByScreenCoordinates( Vector2 screenPosition )
			{
				GetRayByScreenCoordinates( ref screenPosition, out var result );
				return result;
			}
		}





		//public class CameraSettings__F
		//{
		//	//Purposes purpose;

		//	//PolygonModes polygonMode = PolygonModes.Solid;
		//	//double lodBias = 1;

		//	////transform
		//	//Vec3 position;
		//	//Vec3 derivedPosition;
		//	////!!!!!надо?
		//	////!!!!!по сути не надо т.к. есть Rotation
		//	//Vec3 direction = Vec3.XAxis;
		//	//Vec3 derivedDirection = Vec3.XAxis;
		//	//Vec3 up = Vec3.ZAxis;
		//	//Vec3 fixedUp = Vec3.ZAxis;
		//	//Quat rotation = Quat.Identity;
		//	//Quat derivedRotation = Quat.Identity;
		//	//bool reflectionEnabled;
		//	//Plane reflectionPlane = new Plane( Vec3.XAxis, 0 );

		//	//!!!!!в viewport?
		//	//!!!!!настроить всем
		//	//bool allowFrustumCullingTestMode;
		//	//bool allowZonesAndPortalsSceneManagement = true;

		//	//!!!!!!//при создании выставлять?
		//	//!!!!!настраивать
		//	//!!!!!учитывать
		//	//!!!!!слишком странный параметр, какой-то частный случай
		//	//bool applyFullScreenEffects = true;

		//	//!!!!!было
		//	//internal bool lastFrameRendered;

		//	///////////////////////////////////////////

		//	//public enum Purposes
		//	//{
		//	//   ///// <summary>
		//	//   ///// Purpose is not defined.
		//	//   ///// </summary>
		//	//   //Unknown,

		//	//   /// <summary>
		//	//   /// Camera for usual main screen generation.
		//	//   /// </summary>
		//	//   UsualScene,

		//	//   ///// <summary>
		//	//   ///// Camera for shadow map generation.
		//	//   ///// </summary>
		//	//   //ShadowMap,

		//	//   ///// <summary>
		//	//   ///// Camera for compositor (post effect).
		//	//   ///// </summary>
		//	//   //Compositor,

		//	//   /// <summary>
		//	//   /// To generate cubemaps for reflections (CubemapZone).
		//	//   /// </summary>
		//	//   CubemapEnvironment,

		//	//   /// <summary>
		//	//   /// Special purpose. Examples: Water reflection render texture.
		//	//   /// </summary>
		//	//   Special,
		//	//}

		//	///////////////////////////////////////////

		//	public CameraSettings__F()// OgreCamera* doubleObject )//, Purposes purpose )//, bool noDelete )
		//	{
		//		Set( 1.33333f, 75, .1f, 1000, Vec3.Zero, Vec3.XAxis, Vec3.ZAxis );

		//		//this.doubleObject = doubleObject;
		//		////this.purpose = purpose;

		//		////check OgreCamera*, OgreFrustum* is same pointers.
		//		//if( OgreCamera.castToFrustum( doubleObject ) != doubleObject )
		//		//	Log.Fatal( "RenderCamera: RenderCamera: OgreCamera.castToFrustum( doubleObject ) != doubleObject." );
		//		////Init( (OgreFrustum*)OgreCamera.castToFrustum( doubleObject ), MovableTypes.Camera );
		//		////this.noDelete = noDelete;

		//		//RendererWorld.cameras.Add( (IntPtr)doubleObject, this );

		//		////!!!!double
		//		//OgreFrustum.setFOVy( doubleObject, (float)fov.InRadians() );
		//		//OgreFrustum.setNearClipDistance( doubleObject, (float)nearClipDistance );
		//		//OgreFrustum.setFarClipDistance( doubleObject, (float)farClipDistance );

		//		//debugGeometry = new DebugGeometryImpl();
		//	}

		//	//unsafe internal void Dispose()
		//	//{
		//	//	if( doubleObject != null )
		//	//	{
		//	//		//if( debugGeometry != null )
		//	//		//{
		//	//		//   debugGeometry.Dispose();
		//	//		//   debugGeometry = null;
		//	//		//}

		//	//		//after shutdown check
		//	//		if( RendererWorld.Disposed )
		//	//			Log.Fatal( "Renderer: Dispose after Shutdown: {0}()", System.Reflection.MethodInfo.GetCurrentMethod().Name );

		//	//		RendererWorld.cameras.Remove( (IntPtr)doubleObject );

		//	//		//ShutdownFrustum();

		//	//		//if( !noDelete )
		//	//		//{
		//	//		//   Shutdown();
		//	//		MyOgreSceneManager.destroyCamera( SceneManager.doubleObject, doubleObject );
		//	//		//}
		//	//		//else
		//	//		//   Shutdown( true );

		//	//		doubleObject = null;
		//	//	}
		//	//}

		//	//////////////////////// RenderFrustum ////////////////////////


		//	///// <summary>
		//	///// Tests whether the given bounds is visible in the Frustum.
		//	///// </summary>
		//	///// <remarks>
		//	///// Not cull everything outside the frustum.
		//	///// </remarks>
		//	///// <param name="bounds">The bounds.</param>
		//	///// <returns><b>true</b> if the bounds is visibled; otherwise, <b>false</b>.</returns>
		//	//public bool IsIntersectsFast( MathEx.Bounds bounds )
		//	//{
		//	//   unsafe
		//	//   {
		//	//      Vec3 boundsMin = bounds.Minimum;
		//	//      Vec3 boundsMax = bounds.Maximum;
		//	//      return OgreFrustum.isVisibleBounds( doubleObject, ref boundsMin, ref boundsMax );
		//	//   }
		//	//}

		//	///// <summary>
		//	///// Tests whether the given point is visible in the Frustum.
		//	///// </summary>
		//	///// <remarks>
		//	///// Not cull everything outside the frustum.
		//	///// </remarks>
		//	///// <param name="position">The point.</param>
		//	///// <returns><b>true</b> if the point is visibled; otherwise, <b>false</b>.</returns>
		//	//public bool IsIntersectsFast( Vec3 position )
		//	//{
		//	//   unsafe
		//	//   {
		//	//      return OgreFrustum.isVisiblePoint( doubleObject, ref position );
		//	//   }
		//	//}

		//	//public bool IsIntersectsFast( Sphere sphere )
		//	//{
		//	//   unsafe
		//	//   {
		//	//      Vec3 center = sphere.Origin;
		//	//      return OgreFrustum.isVisibleSphere( doubleObject, ref center, sphere.Radius );
		//	//   }
		//	//}

		//	//!!!!!
		//	///// <summary>Returns the reflection matrix of the frustum if appropriate.</summary>
		//	///// <returns>The reflection matrix.</returns>
		//	//public Mat4 GetReflectionMatrix()
		//	//{
		//	//	unsafe
		//	//	{
		//	//		//!!!!точность

		//	//		OgreMatrix4* doubleMatrix = OgreFrustum.getReflectionMatrix( doubleObject );
		//	//		Mat4F result;
		//	//		OgreMatrix4.Convert( doubleMatrix, out result );
		//	//		return result.ToMat4D();
		//	//	}
		//	//}

		//	//!!!!!
		//	////!!!!проверить что как тут
		//	//public void SetClipPlanesForAllGeometry( PlaneF[] planes )
		//	//{
		//	//	unsafe
		//	//	{
		//	//		if( planes != null && planes.Length != 0 )
		//	//		{
		//	//			fixed ( PlaneF* pPlanes = planes )
		//	//			{
		//	//				OgreFrustum.setRenderClipPlanesForAll( doubleObject, pPlanes, planes.Length );
		//	//			}
		//	//		}
		//	//		else
		//	//			OgreFrustum.setRenderClipPlanesForAll( doubleObject, null, 0 );
		//	//	}
		//	//}

		//	//!!!!!
		//	///// <summary>
		//	///// Gets the projection matrix for this frustum adjusted for the current
		//	///// rendersystem specifics (may be right or left-handed, depth range
		//	///// may vary).
		//	///// </summary>
		//	///// <remarks>
		//	///// This method retrieves the rendering-API dependent version of the projection
		//	///// matrix. If you want a 'typical' projection matrix then use 
		//	///// <see cref="GetProjectionMatrix"/>.
		//	///// </remarks>
		//	///// <returns></returns>
		//	//public Mat4F GetProjectionMatrixRS()
		//	//{
		//	//	unsafe
		//	//	{
		//	//		OgreMatrix4* doubleMatrix = OgreFrustum.getProjectionMatrixRS( doubleObject );
		//	//		Mat4F result;
		//	//		OgreMatrix4.Convert( doubleMatrix, out result );
		//	//		return result;
		//	//	}
		//	//}

		//	//!!!!!
		//	///// <summary>
		//	///// Gets the depth-adjusted projection matrix for the current rendersystem,
		//	///// but one which still conforms to right-hand rules.
		//	///// </summary>
		//	///// <remarks>
		//	///// This differs from the rendering-API dependent <see cref="GetProjectionMatrix"/>
		//	///// in that it always returns a right-handed projection matrix result 
		//	///// no matter what rendering API is being used - this is required for
		//	///// vertex and fragment programs for example. However, the resulting depth
		//	///// range may still vary between render systems since D3D uses [0,1] and 
		//	///// GL uses [-1,1], and the range must be kept the same between programmable
		//	///// and fixed-function pipelines.
		//	///// </remarks>
		//	///// <returns></returns>
		//	//public Mat4F GetProjectionMatrixWithRSDepth()
		//	//{
		//	//	unsafe
		//	//	{
		//	//		OgreMatrix4* doubleMatrix = OgreFrustum.getProjectionMatrixWithRSDepth( doubleObject );
		//	//		Mat4F result;
		//	//		OgreMatrix4.Convert( doubleMatrix, out result );
		//	//		return result;
		//	//	}
		//	//}

		//	//!!!!!
		//	///// <summary>
		//	///// Gets the normal projection matrix for this frustum, ie the 
		//	///// projection matrix which conforms to standard right-handed rules and 
		//	///// uses depth range [-1,+1].
		//	///// </summary>
		//	///// <remarks>
		//	///// This differs from the rendering-API dependent <see cref="GetProjectionMatrixRS"/>
		//	///// in that it always returns a right-handed projection matrix with depth
		//	///// range [-1,+1], result no matter what rendering API is being used - this
		//	///// is required for some uniform algebra for example.
		//	///// </remarks>
		//	///// <returns></returns>
		//	//public Mat4F GetProjectionMatrix()
		//	//{
		//	//	//TO DO: cache

		//	//	unsafe
		//	//	{
		//	//		OgreMatrix4* doubleMatrix = OgreFrustum.getProjectionMatrix( doubleObject );
		//	//		Mat4F result;
		//	//		OgreMatrix4.Convert( doubleMatrix, out result );
		//	//		return result;
		//	//	}
		//	//}

		//	//!!!!!!!
		//	///// <summary>
		//	///// Gets the view matrix for this frustum.
		//	///// </summary>
		//	///// <returns></returns>
		//	//public Mat4F GetViewMatrix()
		//	//{
		//	//	//TO DO: cache

		//	//	unsafe
		//	//	{
		//	//		OgreMatrix4* doubleMatrix = OgreFrustum.getViewMatrix( doubleObject );
		//	//		Mat4F result;
		//	//		OgreMatrix4.Convert( doubleMatrix, out result );
		//	//		return result;
		//	//	}
		//	//}

		//	//public Vec2 FrustumOffset
		//	//{
		//	//   get
		//	//   {
		//	//      unsafe
		//	//      {
		//	//         float horizontal, vertical;
		//	//         OgreFrustum.getFrustumOffset( doubleObject, out horizontal, out vertical );
		//	//         return new Vec2( horizontal, vertical );
		//	//      }
		//	//   }
		//	//   set
		//	//   {
		//	//      if( FrustumOffset == value )
		//	//         return;

		//	//      unsafe
		//	//      {
		//	//         OgreFrustum.setFrustumOffset( doubleObject, value.X, value.Y );
		//	//      }

		//	//      frustumCached = null;
		//	//   }
		//	//}

		//	//////////////////////// RenderCamera ////////////////////////

		//	///// <summary>
		//	///// Projects world position to screen coordinates.
		//	///// </summary>
		//	///// <param name="position">The world position.</param>
		//	///// <param name="screenPosition">The result screen coordinates.</param>
		//	///// <returns>
		//	///// <b>true</b> if screen position successfully received; otherwise, <b>false</b>.
		//	///// </returns>
		//	//public bool ProjectToScreenCoordinates( Vec3 position, out Vec2 screenPosition )
		//	//{
		//	//   unsafe
		//	//   {
		//	//      float left, top;
		//	//      bool result = OgreCamera.projectPoint( doubleObject, ref position, out left, out top );
		//	//      screenPosition = new Vec2( ( left + 1.0f ) * .5f, 1.0f - ( top + 1.0f ) * .5f );
		//	//      return result;
		//	//   }
		//	//}

		//	///// <summary>
		//	///// Generates world ray from screen coordinates.
		//	///// </summary>
		//	///// <param name="screenPosition">The screen coordinates.</param>
		//	///// <returns>The ray.</returns>
		//	//public Ray GetCameraToViewportRay( Vec2 screenPosition )
		//	//{
		//	//   unsafe
		//	//   {
		//	//      Ray ray;
		//	//      OgreCamera.getCameraToViewportRay( doubleObject, ref screenPosition, out ray );
		//	//      Vec3 dir = Vec3.Normalize( ray.Direction ) * FarClipDistance;
		//	//      return new Ray( ray.Origin, dir );
		//	//   }
		//	//}

		//	///// <summary>
		//	///// Gets or sets the level of rendering detail required from this camera.
		//	///// </summary>
		//	///// <remarks>
		//	///// Each camera is set to render at full detail by default, that is
		//	///// with full texturing, lighting etc. This method lets you change
		//	///// that behaviour, allowing you to make the camera just render a
		//	///// wireframe view, for example.
		//	///// </remarks>
		//	//public Renderer.PolygonModes PolygonMode
		//	//{
		//	//   get { return polygonMode; }
		//	//   set
		//	//   {
		//	//      this.polygonMode = value;
		//	//      unsafe
		//	//      {
		//	//         OgreCamera.setPolygonMode( doubleObject, value );
		//	//      }
		//	//   }
		//	//}

		//	public bool AllowFrustumCullingTestMode
		//	{
		//		get { return allowFrustumCullingTestMode; }
		//		set { allowFrustumCullingTestMode = value; }
		//	}

		//	//public bool IsForShadowMapGeneration()
		//	//{
		//	//   return Purpose == Purposes.ShadowMap;
		//	//}

		//	//public bool AllowZonesAndPortalsSceneManagement
		//	//{
		//	//   get { return allowZonesAndPortalsSceneManagement; }
		//	//   set { allowZonesAndPortalsSceneManagement = value; }
		//	//}

		//	//!!!!!!
		//	//public bool ApplyFullScreenEffects
		//	//{
		//	//	get { return applyFullScreenEffects; }
		//	//	set { applyFullScreenEffects = value; }
		//	//}

		//	//public DebugGeometry DebugGeometry
		//	//{
		//	//   get { return debugGeometry; }
		//	//}

		//	//public bool UseRenderingDistance
		//	//{
		//	//   get
		//	//   {
		//	//      unsafe
		//	//      {
		//	//         return OgreCamera.getUseRenderingDistance( doubleObject );
		//	//      }
		//	//   }
		//	//   set
		//	//   {
		//	//      unsafe
		//	//      {
		//	//         OgreCamera.setUseRenderingDistance( doubleObject, value );
		//	//      }
		//	//   }
		//	//}

		//	//!!!!было. не надо видать
		//	//public int GetDirectionalLightPSSMTextureIndex()
		//	//{
		//	//   unsafe
		//	//   {
		//	//      return OgreCamera.getDirectionalLightPSSMTextureIndex( doubleObject );
		//	//   }
		//	//}

		//	//public Purposes Purpose
		//	//{
		//	//   get { return purpose; }
		//	//}

		//	//!!!!!
		//	public double LodBias
		//	{
		//		get { return lodBias; }
		//		//!!!!
		//		//set
		//		//{
		//		//	lodBias = value;
		//		//	unsafe
		//		//	{
		//		//		//!!!!
		//		//		OgreCamera.setLodBias( doubleObject, (float)value );
		//		//	}
		//		//}
		//	}

		//	///// <summary>
		//	///// Retrieves a specified plane of the frustum.
		//	///// </summary>
		//	///// <param name="index">The plane index.</param>
		//	///// <returns>The plane.</returns>
		//	//public Plane GetFrustumPlane( int index )
		//	//{
		//	//   unsafe
		//	//   {
		//	//      return *(Plane*)OgreFrustum.getFrustumPlane( doubleObject, index );
		//	//   }
		//	//}



		//}

	}
}
