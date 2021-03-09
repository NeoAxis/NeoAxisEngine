// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Reflection;
using System.IO;
using System.Drawing.Design;
using SharpBgfx;
using NeoAxis.Editor;

namespace NeoAxis
{
	/// <summary>
	/// Implements the sky is using cubemap or panoramic image.
	/// </summary>
	[EditorSettingsCell( typeof( Component_Skybox_SettingsCell ) )]
	[EditorDocumentWindow( typeof( Component_Skybox_DocumentWindow ) )]
	[EditorPreviewControl( typeof( Component_Skybox_PreviewControl ) )]
	[EditorPreviewImage( typeof( Component_Skybox_PreviewImage ) )]
	[WhenCreatingShowWarningIfItAlreadyExists]
	public class Component_Skybox : Component_Sky
	{
		static GpuMaterialPass materialPassCube;
		static GpuMaterialPass materialPass2D;

		static Component_Mesh mesh;
		//Component_Image createdCubemap;
		//bool createdCubemapNeedUpdate = true;

		bool processedCubemapNeedUpdate = true;
		Component_Image processedEnvironmentCubemap;
		Component_Image processedIrradianceCubemap;

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// The texture used by the skybox.
		/// </summary>
		[DefaultValueReference( @"Base\Environments\Forest.image" )]//[DefaultValue( null )]
		public Reference<Component_Image> Cubemap
		{
			get { if( _cubemap.BeginGet() ) Cubemap = _cubemap.Get( this ); return _cubemap.value; }
			set
			{
				if( _cubemap.BeginSet( ref value ) )
				{
					try
					{
						CubemapChanged?.Invoke( this );
						//createdCubemapNeedUpdate = true;
						processedCubemapNeedUpdate = true;
					}
					finally { _cubemap.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Cubemap"/> property value changes.</summary>
		public event Action<Component_Skybox> CubemapChanged;
		ReferenceField<Component_Image> _cubemap = new Reference<Component_Image>( null, @"Base\Environments\Forest.image" );

		///// <summary>
		///// Positive X side of the texture used by the skybox.
		///// </summary>
		//[DefaultValue( null )]
		//[DisplayName( "Cubemap X+" )]
		//public Reference<ReferenceValueType_Resource> CubemapPositiveX
		//{
		//	get { if( _cubemapPositiveX.BeginGet() ) CubemapPositiveX = _cubemapPositiveX.Get( this ); return _cubemapPositiveX.value; }
		//	set
		//	{
		//		if( _cubemapPositiveX.BeginSet( ref value ) )
		//		{
		//			try
		//			{
		//				CubemapPositiveXChanged?.Invoke( this );
		//				createdCubemapNeedUpdate = true;
		//			}
		//			finally { _cubemapPositiveX.EndSet(); }
		//		}
		//	}
		//}
		//public event Action<Component_Skybox> CubemapPositiveXChanged;
		//ReferenceField<ReferenceValueType_Resource> _cubemapPositiveX;

		///// <summary>
		///// Negative X side of the texture used by the skybox.
		///// </summary>
		//[DefaultValue( null )]
		//[DisplayName( "Cubemap X-" )]
		//public Reference<ReferenceValueType_Resource> CubemapNegativeX
		//{
		//	get { if( _cubemapNegativeX.BeginGet() ) CubemapNegativeX = _cubemapNegativeX.Get( this ); return _cubemapNegativeX.value; }
		//	set
		//	{
		//		if( _cubemapNegativeX.BeginSet( ref value ) )
		//		{
		//			try
		//			{
		//				CubemapNegativeXChanged?.Invoke( this );
		//				createdCubemapNeedUpdate = true;
		//			}
		//			finally { _cubemapNegativeX.EndSet(); }
		//		}
		//	}
		//}
		//public event Action<Component_Skybox> CubemapNegativeXChanged;
		//ReferenceField<ReferenceValueType_Resource> _cubemapNegativeX;

		///// <summary>
		///// Positive Y side of the texture used by the skybox.
		///// </summary>
		//[DefaultValue( null )]
		//[DisplayName( "Cubemap Y+" )]
		//public Reference<ReferenceValueType_Resource> CubemapPositiveY
		//{
		//	get { if( _cubemapPositiveY.BeginGet() ) CubemapPositiveY = _cubemapPositiveY.Get( this ); return _cubemapPositiveY.value; }
		//	set
		//	{
		//		if( _cubemapPositiveY.BeginSet( ref value ) )
		//		{
		//			try
		//			{
		//				CubemapPositiveYChanged?.Invoke( this );
		//				createdCubemapNeedUpdate = true;
		//			}
		//			finally { _cubemapPositiveY.EndSet(); }
		//		}
		//	}
		//}
		//public event Action<Component_Skybox> CubemapPositiveYChanged;
		//ReferenceField<ReferenceValueType_Resource> _cubemapPositiveY;

		///// <summary>
		///// Negative Y side of the texture used by the skybox.
		///// </summary>
		//[DefaultValue( null )]
		//[DisplayName( "Cubemap Y-" )]
		//public Reference<ReferenceValueType_Resource> CubemapNegativeY
		//{
		//	get { if( _cubemapNegativeY.BeginGet() ) CubemapNegativeY = _cubemapNegativeY.Get( this ); return _cubemapNegativeY.value; }
		//	set
		//	{
		//		if( _cubemapNegativeY.BeginSet( ref value ) )
		//		{
		//			try
		//			{
		//				CubemapNegativeYChanged?.Invoke( this );
		//				createdCubemapNeedUpdate = true;
		//			}
		//			finally { _cubemapNegativeY.EndSet(); }
		//		}
		//	}
		//}
		//public event Action<Component_Skybox> CubemapNegativeYChanged;
		//ReferenceField<ReferenceValueType_Resource> _cubemapNegativeY;

		///// <summary>
		///// Positive Z side of the texture used by the skybox.
		///// </summary>
		//[DefaultValue( null )]
		//[DisplayName( "Cubemap Z+" )]
		//public Reference<ReferenceValueType_Resource> CubemapPositiveZ
		//{
		//	get { if( _cubemapPositiveZ.BeginGet() ) CubemapPositiveZ = _cubemapPositiveZ.Get( this ); return _cubemapPositiveZ.value; }
		//	set
		//	{
		//		if( _cubemapPositiveZ.BeginSet( ref value ) )
		//		{
		//			try
		//			{
		//				CubemapPositiveZChanged?.Invoke( this );
		//				createdCubemapNeedUpdate = true;
		//			}
		//			finally { _cubemapPositiveZ.EndSet(); }
		//		}
		//	}
		//}
		//public event Action<Component_Skybox> CubemapPositiveZChanged;
		//ReferenceField<ReferenceValueType_Resource> _cubemapPositiveZ;

		///// <summary>
		///// Negative Z side of the texture used by the skybox.
		///// </summary>
		//[DefaultValue( null )]
		//[DisplayName( "Cubemap Z-" )]
		//public Reference<ReferenceValueType_Resource> CubemapNegativeZ
		//{
		//	get { if( _cubemapNegativeZ.BeginGet() ) CubemapNegativeZ = _cubemapNegativeZ.Get( this ); return _cubemapNegativeZ.value; }
		//	set
		//	{
		//		if( _cubemapNegativeZ.BeginSet( ref value ) )
		//		{
		//			try
		//			{
		//				CubemapNegativeZChanged?.Invoke( this );
		//				createdCubemapNeedUpdate = true;
		//			}
		//			finally { _cubemapNegativeZ.EndSet(); }
		//		}
		//	}
		//}
		//public event Action<Component_Skybox> CubemapNegativeZChanged;
		//ReferenceField<ReferenceValueType_Resource> _cubemapNegativeZ;

		///// <summary>
		///// The irradiance map displays somewhat like an average color or lighting display of the environment.
		///// </summary>
		//[DefaultValue( null )]
		//public Reference<Component_Image> CubemapIrradiance
		//{
		//	get { if( _cubemapIrradiance.BeginGet() ) CubemapIrradiance = _cubemapIrradiance.Get( this ); return _cubemapIrradiance.value; }
		//	set { if( _cubemapIrradiance.BeginSet( ref value ) ) { try { CubemapIrradianceChanged?.Invoke( this ); } finally { _cubemapIrradiance.EndSet(); } } }
		//}
		//public event Action<Component_Skybox> CubemapIrradianceChanged;
		//ReferenceField<Component_Image> _cubemapIrradiance;

		/// <summary>
		/// The horizontal rotation of the skybox.
		/// </summary>
		[DefaultValue( 0.0 )]
		[Range( 0, 360 )]
		public Reference<Degree> Rotation
		{
			get { if( _rotation.BeginGet() ) Rotation = _rotation.Get( this ); return _rotation.value; }
			set { if( _rotation.BeginSet( ref value ) ) { try { RotationChanged?.Invoke( this ); } finally { _rotation.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Rotation"/> property value changes.</summary>
		public event Action<Component_Skybox> RotationChanged;
		ReferenceField<Degree> _rotation;

		/// <summary>
		/// Vertical stretch multiplier.
		/// </summary>
		[DefaultValue( 1.0 )]
		[Range( 0.8, 1.2 )]
		public Reference<double> Stretch
		{
			get { if( _stretch.BeginGet() ) Stretch = _stretch.Get( this ); return _stretch.value; }
			set { if( _stretch.BeginSet( ref value ) ) { try { StretchChanged?.Invoke( this ); } finally { _stretch.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Stretch"/> property value changes.</summary>
		public event Action<Component_Skybox> StretchChanged;
		ReferenceField<double> _stretch = 1.0;

		/// <summary>
		/// A skybox color multiplier.
		/// </summary>
		[DefaultValue( "1 1 1" )]
		[ApplicableRangeColorValuePower( 0, 4 )]
		[ColorValueNoAlpha]
		public Reference<ColorValuePowered> Multiplier
		{
			get { if( _multiplier.BeginGet() ) Multiplier = _multiplier.Get( this ); return _multiplier.value; }
			set { if( _multiplier.BeginSet( ref value ) ) { try { MultiplierChanged?.Invoke( this ); } finally { _multiplier.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Multiplier"/> property value changes.</summary>
		public event Action<Component_Skybox> MultiplierChanged;
		ReferenceField<ColorValuePowered> _multiplier = new ColorValuePowered( 1, 1, 1 );

		/// <summary>
		/// Whether to affect to ambient lighting.
		/// </summary>
		[DefaultValue( 1.0 )]
		[Range( 0, 1 )]
		public Reference<double> AffectLighting
		{
			get { if( _affectLighting.BeginGet() ) AffectLighting = _affectLighting.Get( this ); return _affectLighting.value; }
			set { if( _affectLighting.BeginSet( ref value ) ) { try { AffectLightingChanged?.Invoke( this ); } finally { _affectLighting.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="AffectLighting"/> property value changes.</summary>
		public event Action<Component_Skybox> AffectLightingChanged;
		ReferenceField<double> _affectLighting = 1.0;

		/// <summary>
		/// The texture used for the reflection. When it is null, the specified cubemap at <see cref="Cubemap"/> property is used for lighting.
		/// </summary>
		[DefaultValue( null )]
		public Reference<Component_Image> LightingCubemap
		{
			get { if( _lightingCubemap.BeginGet() ) LightingCubemap = _lightingCubemap.Get( this ); return _lightingCubemap.value; }
			set
			{
				if( _lightingCubemap.BeginSet( ref value ) )
				{
					try
					{
						LightingCubemapChanged?.Invoke( this );
						//createdCubemapNeedUpdate = true;
						processedCubemapNeedUpdate = true;
					}
					finally { _lightingCubemap.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="LightingCubemap"/> property value changes.</summary>
		public event Action<Component_Skybox> LightingCubemapChanged;
		ReferenceField<Component_Image> _lightingCubemap = null;

		/// <summary>
		/// The horizontal rotation of the lighting cubemap.
		/// </summary>
		[DefaultValue( 0.0 )]
		[Range( 0, 360 )]
		public Reference<Degree> LightingCubemapRotation
		{
			get { if( _lightingCubemapRotation.BeginGet() ) LightingCubemapRotation = _lightingCubemapRotation.Get( this ); return _lightingCubemapRotation.value; }
			set { if( _lightingCubemapRotation.BeginSet( ref value ) ) { try { LightingCubemapRotationChanged?.Invoke( this ); } finally { _lightingCubemapRotation.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="LightingCubemapRotation"/> property value changes.</summary>
		public event Action<Component_Skybox> LightingCubemapRotationChanged;
		ReferenceField<Degree> _lightingCubemapRotation;

		/// <summary>
		/// A skybox color multiplier that affects ambient lighting.
		/// </summary>
		[DefaultValue( "1 1 1" )]
		[ApplicableRangeColorValuePower( 0, 4 )]
		[ColorValueNoAlpha]
		public Reference<ColorValuePowered> LightingMultiplier
		{
			get { if( _lightingMultiplier.BeginGet() ) LightingMultiplier = _lightingMultiplier.Get( this ); return _lightingMultiplier.value; }
			set { if( _lightingMultiplier.BeginSet( ref value ) ) { try { LightingMultiplierChanged?.Invoke( this ); } finally { _lightingMultiplier.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="LightingMultiplier"/> property value changes.</summary>
		public event Action<Component_Skybox> LightingMultiplierChanged;
		ReferenceField<ColorValuePowered> _lightingMultiplier = new ColorValuePowered( 1, 1, 1 );

		/// <summary>
		/// Whether to use the processed cubemap for the background instead of the original image.
		/// </summary>
		[Category( "Advanced" )]
		[DefaultValue( false )]
		public Reference<bool> AlwaysUseProcessedCubemap
		{
			get { if( _alwaysUseProcessedCubemap.BeginGet() ) AlwaysUseProcessedCubemap = _alwaysUseProcessedCubemap.Get( this ); return _alwaysUseProcessedCubemap.value; }
			set { if( _alwaysUseProcessedCubemap.BeginSet( ref value ) ) { try { AlwaysUseProcessedCubemapChanged?.Invoke( this ); } finally { _alwaysUseProcessedCubemap.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="AlwaysUseProcessedCubemap"/> property value changes.</summary>
		public event Action<Component_Skybox> AlwaysUseProcessedCubemapChanged;
		ReferenceField<bool> _alwaysUseProcessedCubemap = false;

		/// <summary>
		/// Whether to allow processing the specified cubemap to 6-sided cubemap.
		/// </summary>
		[Category( "Advanced" )]
		[DefaultValue( true )]
		public Reference<bool> AllowProcessEnvironmentCubemap
		{
			get { if( _allowProcessEnvironmentCubemap.BeginGet() ) AllowProcessEnvironmentCubemap = _allowProcessEnvironmentCubemap.Get( this ); return _allowProcessEnvironmentCubemap.value; }
			set { if( _allowProcessEnvironmentCubemap.BeginSet( ref value ) ) { try { AllowProcessEnvironmentCubemapChanged?.Invoke( this ); } finally { _allowProcessEnvironmentCubemap.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="AllowProcessEnvironmentCubemap"/> property value changes.</summary>
		public event Action<Component_Skybox> AllowProcessEnvironmentCubemapChanged;
		ReferenceField<bool> _allowProcessEnvironmentCubemap = true;

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		//ReferenceValueType_Resource GetCubemapSide( int face )
		//{
		//	switch( face )
		//	{
		//	case 0: return CubemapPositiveX;
		//	case 1: return CubemapNegativeX;
		//	case 2: return CubemapPositiveY;
		//	case 3: return CubemapNegativeY;
		//	case 4: return CubemapPositiveZ;
		//	case 5: return CubemapNegativeZ;
		//	}
		//	return null;
		//}

		//bool AnyCubemapSideIsSpecified()
		//{
		//	for( int n = 0; n < 6; n++ )
		//		if( GetCubemapSide( n ) != null )
		//			return true;
		//	return false;
		//}

		//bool AllCubemapSidesAreaSpecified()
		//{
		//	for( int n = 0; n < 6; n++ )
		//		if( GetCubemapSide( n ) == null )
		//			return false;
		//	return true;
		//}

		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			base.OnMetadataGetMembersFilter( context, member, ref skip );

			var p = member as Metadata.Property;
			if( p != null )
			{
				switch( p.Name )
				{
				case nameof( LightingCubemap ):
				case nameof( LightingMultiplier ):
					if( AffectLighting.Value == 0 )
						skip = true;
					break;

				case nameof( LightingCubemapRotation ):
					if( !LightingCubemap.ReferenceSpecified && LightingCubemap.Value == null )
						skip = true;
					if( AffectLighting.Value == 0 )
						skip = true;
					break;

					//case nameof( Cubemap ):
					//	if( AnyCubemapSideIsSpecified() )
					//		skip = true;
					//	break;

					//case nameof( CubemapPositiveX ):
					//case nameof( CubemapNegativeX ):
					//case nameof( CubemapPositiveY ):
					//case nameof( CubemapNegativeY ):
					//case nameof( CubemapPositiveZ ):
					//case nameof( CubemapNegativeZ ):
					//	if( Cubemap.Value != null )
					//		skip = true;
					//	break;
				}
			}
		}

		[Browsable( false )]
		public static Component_Mesh Mesh
		{
			get { return mesh; }
		}

		protected override void OnEnabled()
		{
			base.OnEnabled();

			if( mesh == null )
			{
				mesh = ComponentUtility.CreateComponent<Component_Mesh>( null, true, false );
				mesh.CreateComponent<Component_MeshGeometry_Box>();
				mesh.Enabled = true;
			}

			processedCubemapNeedUpdate = true;
		}

		protected override void OnDisabled()
		{
			//mesh?.Dispose();
			//mesh = null;

			//CreatedCubemapDispose();

			base.OnDisabled();
		}

		//void CreatedCubemapUpdate()
		//{
		//	if( AllCubemapSidesAreaSpecified() )
		//	{
		//		if( createdCubemap == null || createdCubemapNeedUpdate )
		//		{
		//			CreatedCubemapDispose();

		//			//need set ShowInEditor = false before AddComponent
		//			createdCubemap = ComponentUtility.CreateComponent<Component_Image>( null, false, false );
		//			createdCubemap.DisplayInEditor = false;
		//			AddComponent( createdCubemap );
		//			//createdCubemap = CreateComponent<Component_Texture>( enable: false );

		//			createdCubemap.SaveSupport = false;
		//			createdCubemap.CloneSupport = false;

		//			createdCubemap.LoadCubePositiveX = GetCubemapSide( 0 );
		//			createdCubemap.LoadCubeNegativeX = GetCubemapSide( 1 );
		//			createdCubemap.LoadCubePositiveY = GetCubemapSide( 2 );
		//			createdCubemap.LoadCubeNegativeY = GetCubemapSide( 3 );
		//			createdCubemap.LoadCubePositiveZ = GetCubemapSide( 4 );
		//			createdCubemap.LoadCubeNegativeZ = GetCubemapSide( 5 );

		//			createdCubemap.Enabled = true;

		//			createdCubemapNeedUpdate = false;
		//		}
		//	}
		//	else
		//		CreatedCubemapDispose();
		//}

		//void CreatedCubemapDispose()
		//{
		//	createdCubemap?.Dispose();
		//	createdCubemap = null;
		//}

		protected override void OnUpdate( float delta )
		{
			base.OnUpdate( delta );

			UpdateProcessedCubemaps();
		}

		unsafe public override void Render( Component_RenderingPipeline pipeline, ViewportRenderingContext context, Component_RenderingPipeline.IFrameData frameData )
		{
			UpdateProcessedCubemaps();

			if( mesh != null )
			{
				////!!!!
				//CreatedCubemapUpdate();

				//!!!!double
				Matrix4F worldMatrix = Matrix4.FromTranslate( context.Owner.CameraSettings.Position ).ToMatrix4F();

				foreach( var item in mesh.Result.MeshData.RenderOperations )
				{
					ParameterContainer generalContainer = new ParameterContainer();
					generalContainer.Set( "multiplier", Multiplier.Value.ToColorValue() );
					GetRotationMatrix( out var rotation );
					generalContainer.Set( "rotation", rotation );// Matrix3.FromRotateByZ( Rotation.Value.InRadians() ).ToMatrix3F() );
					generalContainer.Set( "stretch", (float)Stretch.Value );

					Component_Image tex = null;
					////!!!!hack. by idea need mirror 6-sided loaded cubemaps
					//bool flipCubemap = false;

					if( AlwaysUseProcessedCubemap )
						tex = processedEnvironmentCubemap;
					if( tex == null )
					{
						tex = Cubemap;
						//if( tex != null && tex.AnyCubemapSideIsSpecified() )
						//	flipCubemap = true;
					}
					if( tex == null )
						tex = processedEnvironmentCubemap;
					if( tex == null )
						tex = ResourceUtility.BlackTextureCube;

					//generalContainer.Set( "flipCubemap", new Vector4F( flipCubemap ? -1 : 1, 0, 0, 0 ) );

					////!!!!сделать GetResultEnvironmentCubemap()?
					////!!!!!!где еще его использовать
					//if( processedEnvironmentCubemap != null )
					//	tex = processedEnvironmentCubemap;
					////else if( AnyCubemapSideIsSpecified() )
					////	tex = createdCubemap;
					//else
					//	tex = Cubemap;
					//if( tex == null )
					//	tex = ResourceUtility.BlackTextureCube;

					if( tex.Result != null )
					{
						var cube = tex.Result.TextureType == Component_Image.TypeEnum.Cube;

						var pass = GetMaterialPass( cube );
						if( pass != null )
						{
							TextureAddressingMode addressingMode;
							if( cube )
								addressingMode = TextureAddressingMode.Wrap;
							else
								addressingMode = TextureAddressingMode.WrapU | TextureAddressingMode.ClampV;

							context.BindTexture( new ViewportRenderingContext.BindTextureData( 0/*"skyboxTexture"*/, tex, addressingMode, FilterOption.Linear, FilterOption.Linear, FilterOption.Linear ) );

							var containers = new List<ParameterContainer>();
							containers.Add( generalContainer );

							Bgfx.SetTransform( (float*)&worldMatrix );
							( (Component_RenderingPipeline_Basic)pipeline ).RenderOperation( context, item, pass, containers );
						}
					}
				}
			}
		}

		public void GetRotationMatrix( out Matrix3F result )
		{
			Matrix3.FromRotateByZ( Rotation.Value.InRadians() ).ToMatrix3F( out result );
		}

		public void GetLightingCubemapRotationMatrix( out Matrix3F result )
		{
			Matrix3.FromRotateByZ( LightingCubemapRotation.Value.InRadians() ).ToMatrix3F( out result );
		}

		static GpuMaterialPass GetMaterialPass( bool cube )
		{
			if( cube && materialPassCube == null || !cube && materialPass2D == null )
			{
				//generate compile arguments
				var generalDefines = new List<(string, string)>();

				if( !cube )
					generalDefines.Add( ("USE_2D", "") );

				//vertex program
				GpuProgram vertexProgram = GpuProgramManager.GetProgram( $"Skybox_Vertex_", GpuProgramType.Vertex,
					$@"Base\Shaders\Skybox_vs.sc", generalDefines, out var error );
				if( !string.IsNullOrEmpty( error ) )
				{
					Log.Warning( error );
					return null;
				}

				//fragment program
				GpuProgram fragmentProgram = GpuProgramManager.GetProgram( $"Skybox_Fragment_", GpuProgramType.Fragment,
					$@"Base\Shaders\Skybox_fs.sc", generalDefines, out error );
				if( !string.IsNullOrEmpty( error ) )
				{
					Log.Warning( error );
					return null;
				}

				var pass = new GpuMaterialPass( vertexProgram, fragmentProgram );
				pass.CullingMode = CullingMode.None;
				pass.DepthCheck = true;
				pass.DepthWrite = false;

				if( cube )
					materialPassCube = pass;
				else
					materialPass2D = pass;
			}

			return cube ? materialPassCube : materialPass2D;
		}

		void UpdateProcessedCubemaps()
		{
			if( processedEnvironmentCubemap != null && processedEnvironmentCubemap.Disposed )
			{
				processedEnvironmentCubemap = null;
				processedCubemapNeedUpdate = true;
			}
			if( processedIrradianceCubemap != null && processedIrradianceCubemap.Disposed )
			{
				processedIrradianceCubemap = null;
				processedCubemapNeedUpdate = true;
			}

			if( processedCubemapNeedUpdate && AllowProcessEnvironmentCubemap )
			{
				processedCubemapNeedUpdate = false;
				ProcessCubemaps();
			}
		}

		void ProcessCubemaps()
		{
			processedEnvironmentCubemap = null;
			processedIrradianceCubemap = null;

			var useCubemap = LightingCubemap;
			if( !useCubemap.ReferenceSpecified && useCubemap.Value == null )
				useCubemap = Cubemap;

			var sourceFileName = useCubemap.Value?.LoadFile.Value?.ResourceName;
			if( string.IsNullOrEmpty( sourceFileName ) )
			{
				var getByReference = useCubemap.GetByReference;
				if( !string.IsNullOrEmpty( getByReference ) )
				{
					try
					{
						if( Path.GetExtension( getByReference ) == ".image" )
							sourceFileName = getByReference;
					}
					catch { }
				}
			}

			if( !string.IsNullOrEmpty( sourceFileName ) )
			{
				bool skip = false;
				if( sourceFileName.Length > 11 )
				{
					var s = sourceFileName.Substring( sourceFileName.Length - 11 );
					if( s == "_GenEnv.dds" || s == "_GenIrr.dds" )
						skip = true;
				}

				if( !skip )
				{
					if( !CubemapProcessing.GetOrGenerate( sourceFileName, false, 0, out var envVirtualFileName, out var irrVirtualFileName, out var error ) )
					{
						Log.Error( error );
						return;
					}

					if( VirtualFile.Exists( envVirtualFileName ) )
						processedEnvironmentCubemap = ResourceManager.LoadResource<Component_Image>( envVirtualFileName );
					if( VirtualFile.Exists( irrVirtualFileName ) )
						processedIrradianceCubemap = ResourceManager.LoadResource<Component_Image>( irrVirtualFileName );
				}
			}
		}

		//public void GetEnvironmentCubemaps( out Component_Image environmentCubemap, out Component_Image irradianceCubemap )
		//{
		//	environmentCubemap = processedEnvironmentCubemap;
		//	irradianceCubemap = processedIrradianceCubemap;
		//}

		public override bool GetEnvironmentTextureData( out Component_RenderingPipeline.EnvironmentTextureData environmentCubemap, out Component_RenderingPipeline.EnvironmentTextureData irradianceCubemap )
		{
			var affect = (float)AffectLighting.Value;
			if( affect > 0 )
			{
				Vector3F multiplier;
				Matrix3F rotation;
				if( LightingCubemap.ReferenceSpecified || LightingCubemap.Value != null )
				{
					multiplier = LightingMultiplier.Value.ToVector3F();
					GetLightingCubemapRotationMatrix( out rotation );
				}
				else
				{
					multiplier = Multiplier.Value.ToVector3F() * LightingMultiplier.Value.ToVector3F();
					GetRotationMatrix( out rotation );
				}

				if( processedEnvironmentCubemap != null )
					environmentCubemap = new Component_RenderingPipeline.EnvironmentTextureData( processedEnvironmentCubemap, affect, ref rotation, ref multiplier );
				else
					environmentCubemap = new Component_RenderingPipeline.EnvironmentTextureData( ResourceUtility.GrayTextureCube, affect );

				if( processedIrradianceCubemap != null )
					irradianceCubemap = new Component_RenderingPipeline.EnvironmentTextureData( processedIrradianceCubemap, affect, ref rotation, ref multiplier );
				else
					irradianceCubemap = new Component_RenderingPipeline.EnvironmentTextureData( ResourceUtility.GrayTextureCube, affect );

				return true;
			}

			environmentCubemap = new Component_RenderingPipeline.EnvironmentTextureData();
			irradianceCubemap = new Component_RenderingPipeline.EnvironmentTextureData();
			return false;
		}
	}
}
