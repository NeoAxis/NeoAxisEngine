// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace NeoAxis
{
	/// <summary>
	/// Indirect lighting screen effect.
	/// </summary>
	[DefaultOrderOfEffect( 0.5 )]
	[Editor.WhenCreatingShowWarningIfItAlreadyExists]
	public class RenderingEffect_IndirectLighting : RenderingEffect
	{
		public static double GlobalMultiplier = 1;

		/// <summary>
		/// The intensity of the effect.
		/// </summary>
		[Serialize]
		[DefaultValue( 1.0 )]
		[Range( 0, 1 )]
		[Category( "Effect" )]
		public Reference<double> Intensity
		{
			get { if( _intensity.BeginGet() ) Intensity = _intensity.Get( this ); return _intensity.value; }
			set { if( _intensity.BeginSet( ref value ) ) { try { IntensityChanged?.Invoke( this ); } finally { _intensity.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Intensity"/> property value changes.</summary>
		public event Action<RenderingEffect_IndirectLighting> IntensityChanged;
		ReferenceField<double> _intensity = 1;

		public enum TechniqueEnum
		{
			ScreenSpace,
			Full
		}

		//!!!!default
		/// <summary>
		/// The technique for getting indirect lighting.
		/// </summary>
		[Category( "Effect" )]
		[DefaultValue( TechniqueEnum.ScreenSpace )]
		public Reference<TechniqueEnum> Technique
		{
			get { if( _technique.BeginGet() ) Technique = _technique.Get( this ); return _technique.value; }
			set { if( _technique.BeginSet( ref value ) ) { try { TechniqueChanged?.Invoke( this ); } finally { _technique.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Technique"/> property value changes.</summary>
		public event Action<RenderingEffect_IndirectLighting> TechniqueChanged;
		ReferenceField<TechniqueEnum> _technique = TechniqueEnum.ScreenSpace;


		//full mode

		//!!!!impl
		/// <summary>
		/// The effective distance of the effect. The effect can only have influence within that radius.
		/// </summary>
		[Category( "Structure" )]
		[DefaultValue( 100.0 )]
		[Range( 10, 500, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		public Reference<double> Distance
		{
			get { if( _distance.BeginGet() ) Distance = _distance.Get( this ); return _distance.value; }
			set { if( _distance.BeginSet( ref value ) ) { try { DistanceChanged?.Invoke( this ); } finally { _distance.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Distance"/> property value changes.</summary>
		public event Action<RenderingEffect_IndirectLighting> DistanceChanged;
		ReferenceField<double> _distance = 100.0;

		//!!!!default
		/// <summary>
		/// The amount of internal voxel grids. Each level two times bigger than previous level by radius. Last level distance is equal Distance property.
		/// </summary>
		[Category( "Structure" )]
		[Range( 1, 10 )]
		[DefaultValue( 4 )]
		public Reference<int> Levels
		{
			get { if( _levels.BeginGet() ) Levels = _levels.Get( this ); return _levels.value; }
			set
			{
				if( value < 1 ) value = 1;
				if( _levels.BeginSet( ref value ) ) { try { LevelsChanged?.Invoke( this ); } finally { _levels.EndSet(); } }
			}
		}
		/// <summary>Occurs when the <see cref="Levels"/> property value changes.</summary>
		public event Action<RenderingEffect_IndirectLighting> LevelsChanged;
		ReferenceField<int> _levels = 4;


		public enum GridSizeEnum
		{
			_128,
			_256,
			_512,
		}

		//!!!!default

		/// <summary>
		/// The size of the internal grid.
		/// </summary>
		[Category( "Structure" )]
		[DefaultValue( GridSizeEnum._128 )]
		public Reference<GridSizeEnum> GridSize
		{
			get { if( _gridSize.BeginGet() ) GridSize = _gridSize.Get( this ); return _gridSize.value; }
			set { if( _gridSize.BeginSet( ref value ) ) { try { GridSizeChanged?.Invoke( this ); } finally { _gridSize.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="GridSize"/> property value changes.</summary>
		public event Action<RenderingEffect_IndirectLighting> GridSizeChanged;
		ReferenceField<GridSizeEnum> _gridSize = GridSizeEnum._128;

		public int GetGridSize()
		{
			switch( GridSize.Value )
			{
			case GridSizeEnum._128: return 128;
			case GridSizeEnum._256: return 256;
			case GridSizeEnum._512: return 512;
			}
			return 128;
		}

		//!!!!need?
		//!!!!name
		[Category( "Structure" )]
		[Range( 0, 2 )]
		[DefaultValue( 1.0 )]
		public Reference<double> VoxelizationExpandFactor
		{
			get { if( _voxelizationExpandFactor.BeginGet() ) VoxelizationExpandFactor = _voxelizationExpandFactor.Get( this ); return _voxelizationExpandFactor.value; }
			set { if( _voxelizationExpandFactor.BeginSet( ref value ) ) { try { VoxelizationExpandFactorChanged?.Invoke( this ); } finally { _voxelizationExpandFactor.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="VoxelizationExpandFactor"/> property value changes.</summary>
		public event Action<RenderingEffect_IndirectLighting> VoxelizationExpandFactorChanged;
		ReferenceField<double> _voxelizationExpandFactor = 1.0;


		//!!!!коэффициент распределение сетки? типа как в тенях parallel split

		//!!!!Bounces?


		////!!!!может в самом шейдере делать этот цикл
		////!!!!name
		////!!!!need?
		//[Category( "Basic" )]
		//[DefaultValue( 1 )]
		//[Range( 1, 20 )] //!!!!
		//public Reference<int> RadianceSteps
		//{
		//	get { if( _radianceSteps.BeginGet() ) RadianceSteps = _radianceSteps.Get( this ); return _radianceSteps.value; }
		//	set { if( _radianceSteps.BeginSet( ref value ) ) { try { RadianceStepsChanged?.Invoke( this ); } finally { _radianceSteps.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="RadianceSteps"/> property value changes.</summary>
		//public event Action<RenderingEffect_IndirectLighting> RadianceStepsChanged;
		//ReferenceField<int> _radianceSteps = 1;


		[Category( "Input" )]
		[DefaultValue( 0.0 )]
		[Range( 0, 4, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		public Reference<double> AmbientLighting
		{
			get { if( _ambientLighting.BeginGet() ) AmbientLighting = _ambientLighting.Get( this ); return _ambientLighting.value; }
			set { if( _ambientLighting.BeginSet( ref value ) ) { try { AmbientLightingChanged?.Invoke( this ); } finally { _ambientLighting.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="AmbientLighting"/> property value changes.</summary>
		public event Action<RenderingEffect_IndirectLighting> AmbientLightingChanged;
		ReferenceField<double> _ambientLighting = 0.0;

		[Category( "Input" )]
		[DefaultValue( 1.0 )]
		[Range( 0, 4, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		public Reference<double> DirectLighting
		{
			get { if( _directLighting.BeginGet() ) DirectLighting = _directLighting.Get( this ); return _directLighting.value; }
			set { if( _directLighting.BeginSet( ref value ) ) { try { DirectLightingChanged?.Invoke( this ); } finally { _directLighting.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DirectLighting"/> property value changes.</summary>
		public event Action<RenderingEffect_IndirectLighting> DirectLightingChanged;
		ReferenceField<double> _directLighting = 1.0;

		[Category( "Input" )]
		[DefaultValue( 1.0 )]
		[Range( 0, 4, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		public Reference<double> EmissiveLighting
		{
			get { if( _emissiveLighting.BeginGet() ) EmissiveLighting = _emissiveLighting.Get( this ); return _emissiveLighting.value; }
			set { if( _emissiveLighting.BeginSet( ref value ) ) { try { EmissiveLightingChanged?.Invoke( this ); } finally { _emissiveLighting.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="EmissiveLighting"/> property value changes.</summary>
		public event Action<RenderingEffect_IndirectLighting> EmissiveLightingChanged;
		ReferenceField<double> _emissiveLighting = 1.0;


		//!!!!impl
		//!!!!default
		//!!!!name
		//!!!!remove?

		/// <summary>
		/// The shading model for the materials for indirect lighting calculation.
		/// </summary>
		[Category( "Input" )]
		[DefaultValue( false )]
		public Reference<bool> ShadingModelFull
		{
			get { if( _shadingModelFull.BeginGet() ) ShadingModelFull = _shadingModelFull.Get( this ); return _shadingModelFull.value; }
			set { if( _shadingModelFull.BeginSet( ref value ) ) { try { ShadingModelFullChanged?.Invoke( this ); } finally { _shadingModelFull.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ShadingModelFull"/> property value changes.</summary>
		public event Action<RenderingEffect_IndirectLighting> ShadingModelFullChanged;
		ReferenceField<bool> _shadingModelFull = false;

		/// <summary>
		/// The output multiplier of indirect diffuse lighting.
		/// </summary>
		[Category( "Output" )]
		[Range( 0, 4, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		[DefaultValue( 1.0 )]
		public Reference<double> DiffuseOutput
		{
			get { if( _diffuseOutput.BeginGet() ) DiffuseOutput = _diffuseOutput.Get( this ); return _diffuseOutput.value; }
			set { if( _diffuseOutput.BeginSet( ref value ) ) { try { DiffuseOutputChanged?.Invoke( this ); } finally { _diffuseOutput.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DiffuseOutput"/> property value changes.</summary>
		public event Action<RenderingEffect_IndirectLighting> DiffuseOutputChanged;
		ReferenceField<double> _diffuseOutput = 1.0;

		/// <summary>
		/// The output multiplier of indirect specular lighting.
		/// </summary>
		[Category( "Output" )]
		[Range( 0, 4, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		[DefaultValue( 1.0 )]
		public Reference<double> SpecularOutput
		{
			get { if( _specularOutput.BeginGet() ) SpecularOutput = _specularOutput.Get( this ); return _specularOutput.value; }
			set { if( _specularOutput.BeginSet( ref value ) ) { try { SpecularOutputChanged?.Invoke( this ); } finally { _specularOutput.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="SpecularOutput"/> property value changes.</summary>
		public event Action<RenderingEffect_IndirectLighting> SpecularOutputChanged;
		ReferenceField<double> _specularOutput = 1.0;

		[DefaultValue( ResolutionEnum.Half )]
		[Category( "Output" )]
		[DisplayName( "Resolution" )]
		public Reference<ResolutionEnum> ResolutionFull
		{
			get { if( _resolutionFull.BeginGet() ) ResolutionFull = _resolutionFull.Get( this ); return _resolutionFull.value; }
			set { if( _resolutionFull.BeginSet( ref value ) ) { try { ResolutionFullChanged?.Invoke( this ); } finally { _resolutionFull.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ResolutionFull"/> property value changes.</summary>
		public event Action<RenderingEffect_IndirectLighting> ResolutionFullChanged;
		ReferenceField<ResolutionEnum> _resolutionFull = ResolutionEnum.Half;

		//!!!!descriptions

		//!!!!default
		[Category( "Tracing" )]
		[Range( 1, 100, RangeAttribute.ConvenientDistributionEnum.Exponential, 3 )]
		[DefaultValue( 10.0 )]
		public Reference<double> TraceLength
		{
			get { if( _traceLength.BeginGet() ) TraceLength = _traceLength.Get( this ); return _traceLength.value; }
			set { if( _traceLength.BeginSet( ref value ) ) { try { TraceLengthChanged?.Invoke( this ); } finally { _traceLength.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="TraceLength"/> property value changes.</summary>
		public event Action<RenderingEffect_IndirectLighting> TraceLengthChanged;
		ReferenceField<double> _traceLength = 10.0;

		//!!!!default
		[Category( "Tracing" )]
		[Range( 0.1, 10, RangeAttribute.ConvenientDistributionEnum.Exponential, 3 )]
		[DefaultValue( 2.0 )]
		public Reference<double> TraceStartOffset
		{
			get { if( _traceStartOffset.BeginGet() ) TraceStartOffset = _traceStartOffset.Get( this ); return _traceStartOffset.value; }
			set { if( _traceStartOffset.BeginSet( ref value ) ) { try { TraceStartOffsetChanged?.Invoke( this ); } finally { _traceStartOffset.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="TraceStartOffset"/> property value changes.</summary>
		public event Action<RenderingEffect_IndirectLighting> TraceStartOffsetChanged;
		ReferenceField<double> _traceStartOffset = 2.0;

		//!!!!default
		[Category( "Tracing" )]
		[Range( 0.1, 10, RangeAttribute.ConvenientDistributionEnum.Exponential, 3 )]
		[DefaultValue( 0.2 )]
		public Reference<double> TraceStepFactor
		{
			get { if( _traceStepFactor.BeginGet() ) TraceStepFactor = _traceStepFactor.Get( this ); return _traceStepFactor.value; }
			set { if( _traceStepFactor.BeginSet( ref value ) ) { try { TraceStepFactorChanged?.Invoke( this ); } finally { _traceStepFactor.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="TraceStepFactor"/> property value changes.</summary>
		public event Action<RenderingEffect_IndirectLighting> TraceStepFactorChanged;
		ReferenceField<double> _traceStepFactor = 0.2;


		//public enum AffectObjectsEnum
		//{
		//	All,
		//	OnlyVisible,
		//}

		//!!!!impl
		///// <summary>
		///// The mode of selection objects.
		///// </summary>
		//[Category( "Basic" )]
		//[DefaultValue( AffectObjectsEnum.All )]
		//public Reference<AffectObjectsEnum> AffectObjects
		//{
		//	get { if( _affectObjects.BeginGet() ) AffectObjects = _affectObjects.Get( this ); return _affectObjects.value; }
		//	set { if( _affectObjects.BeginSet( ref value ) ) { try { AffectObjectsChanged?.Invoke( this ); } finally { _affectObjects.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="AffectObjects"/> property value changes.</summary>
		//public event Action<RenderingEffect_IndirectLighting> AffectObjectsChanged;
		//ReferenceField<AffectObjectsEnum> _affectObjects = AffectObjectsEnum.All;


		////!!!!impl
		///// <summary>
		///// The distance multiplier when determining the level of detail.
		///// </summary>
		//[Category( "Basic" )]
		//[DefaultValue( 1.0 )]
		//[DisplayName( "LOD Scale" )]
		//[Range( 0, 10, RangeAttribute.ConvenientDistributionEnum.Exponential, 3 )]
		//public Reference<double> LODScale
		//{
		//	get { if( _lODScale.BeginGet() ) LODScale = _lODScale.Get( this ); return _lODScale.value; }
		//	set { if( _lODScale.BeginSet( ref value ) ) { try { LODScaleChanged?.Invoke( this ); } finally { _lODScale.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="LODScale"/> property value changes.</summary>
		//public event Action<RenderingEffect_IndirectLighting> LODScaleChanged;
		//ReferenceField<double> _lODScale = 1.0;


		//public enum UseMaterialSettingsEnum
		//{
		//	All,
		//	OnlyEmission,
		//	//Simple,
		//}

		//!!!!impl
		///// <summary>
		///// The mode of using material settings.
		///// </summary>
		//[Category( "Basic" )]
		//[DefaultValue( UseMaterialSettingsEnum.OnlyEmission )]
		//public Reference<UseMaterialSettingsEnum> UseMaterialSettings
		//{
		//	get { if( _applyMaterialSettings.BeginGet() ) UseMaterialSettings = _applyMaterialSettings.Get( this ); return _applyMaterialSettings.value; }
		//	set { if( _applyMaterialSettings.BeginSet( ref value ) ) { try { UseMaterialSettingsChanged?.Invoke( this ); } finally { _applyMaterialSettings.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="UseMaterialSettings"/> property value changes.</summary>
		//public event Action<RenderingEffect_IndirectLighting> UseMaterialSettingsChanged;
		//ReferenceField<UseMaterialSettingsEnum> _applyMaterialSettings = UseMaterialSettingsEnum.OnlyEmission;


		////!!!!need?
		////!!!!default
		///// <summary>
		///// Whether to update scene data over time.
		///// </summary>
		//[DefaultValue( true )]
		//public Reference<bool> Dynamic
		//{
		//	get { if( _dynamic.BeginGet() ) Dynamic = _dynamic.Get( this ); return _dynamic.value; }
		//	set { if( _dynamic.BeginSet( ref value ) ) { try { DynamicChanged?.Invoke( this ); } finally { _dynamic.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="Dynamic"/> property value changes.</summary>
		//public event Action<RenderingEffect_IndirectLighting> DynamicChanged;
		//ReferenceField<bool> _dynamic = true;


		//!!!!

		//[DefaultValue( "0 0 0 0" )]
		//public Reference<Vector4F> TestParameter
		//{
		//	get { if( _testParameter.BeginGet() ) TestParameter = _testParameter.Get( this ); return _testParameter.value; }
		//	set { if( _testParameter.BeginSet( ref value ) ) { try { TestParameterChanged?.Invoke( this ); } finally { _testParameter.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="TestParameter"/> property value changes.</summary>
		//public event Action<RenderingEffect_IndirectLighting> TestParameterChanged;
		//ReferenceField<Vector4F> _testParameter;

		//[DefaultValue( 0.0f )]
		//public Reference<float> TestParameter
		//{
		//	get { if( _testParameter.BeginGet() ) TestParameter = _testParameter.Get( this ); return _testParameter.value; }
		//	set { if( _testParameter.BeginSet( ref value ) ) { try { TestParameterChanged?.Invoke( this ); } finally { _testParameter.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="TestParameter"/> property value changes.</summary>
		//public event Action<RenderingEffect_IndirectLighting> TestParameterChanged;
		//ReferenceField<float> _testParameter = 0.0f;


		public enum QualityEnum
		{
			Low,
			Medium,
			High,
			//Highest,
		}

		/// <summary>
		/// The quality of the effect.
		/// </summary>
		[DefaultValue( QualityEnum.Medium )]
		[Category( "Basic" )]
		public Reference<QualityEnum> Quality
		{
			get { if( _quality.BeginGet() ) Quality = _quality.Get( this ); return _quality.value; }
			set { if( _quality.BeginSet( ref value ) ) { try { QualityChanged?.Invoke( this ); } finally { _quality.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Quality"/> property value changes.</summary>
		public event Action<RenderingEffect_IndirectLighting> QualityChanged;
		ReferenceField<QualityEnum> _quality = QualityEnum.Medium;

		public enum ResolutionEnum
		{
			Full = 1,
			Half = 2,
			Quarter = 4,
			Eighth = 8,
		}

		/// <summary>
		/// Using lower resolution light buffer can improve performance, but can accentuate aliasing.
		/// </summary>
		[DefaultValue( ResolutionEnum.Half )]
		[Category( "Basic" )]
		[DisplayName( "Resolution" )]
		public Reference<ResolutionEnum> ResolutionScreenSpace
		{
			get { if( _resolutionScreenSpace.BeginGet() ) ResolutionScreenSpace = _resolutionScreenSpace.Get( this ); return _resolutionScreenSpace.value; }
			set { if( _resolutionScreenSpace.BeginSet( ref value ) ) { try { ResolutionScreenSpaceChanged?.Invoke( this ); } finally { _resolutionScreenSpace.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ResolutionScreenSpace"/> property value changes.</summary>
		public event Action<RenderingEffect_IndirectLighting> ResolutionScreenSpaceChanged;
		ReferenceField<ResolutionEnum> _resolutionScreenSpace = ResolutionEnum.Half;

		//!!!!remove? from screen space too?
		/// <summary>
		/// Linear multiplier of effect strength.
		/// </summary>
		[DefaultValue( 1.0 )]//[DefaultValue( 10.0 )]
		[Range( 0, 5, RangeAttribute.ConvenientDistributionEnum.Exponential )]//[Range( 0, 50, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		[Category( "Basic" )]
		public Reference<double> Multiplier
		{
			get { if( _multiplier.BeginGet() ) Multiplier = _multiplier.Get( this ); return _multiplier.value; }
			set
			{
				if( value < 0 )
					value = new Reference<double>( 0, value.GetByReference );
				if( _multiplier.BeginSet( ref value ) )
				{
					try { MultiplierChanged?.Invoke( this ); }
					finally { _multiplier.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Multiplier"/> property value changes.</summary>
		public event Action<RenderingEffect_IndirectLighting> MultiplierChanged;
		ReferenceField<double> _multiplier = 1.0;// 10.0;

		/// <summary>
		/// The linear reduction of final lighting.
		/// </summary>
		[DefaultValue( 0.05 )]
		[Range( 0, 4, RangeAttribute.ConvenientDistributionEnum.Exponential, 3 )]
		[Category( "Basic" )]
		public Reference<double> Reduction
		{
			get { if( _reduction.BeginGet() ) Reduction = _reduction.Get( this ); return _reduction.value; }
			set { if( _reduction.BeginSet( ref value ) ) { try { ReductionChanged?.Invoke( this ); } finally { _reduction.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Reduction"/> property value changes.</summary>
		public event Action<RenderingEffect_IndirectLighting> ReductionChanged;
		ReferenceField<double> _reduction = 0.05;

		/// <summary>
		/// Effective sampling radius in world space. The effect can only have influence within that radius.
		/// </summary>
		[DefaultValue( 3 )]
		[Range( 0, 30, RangeAttribute.ConvenientDistributionEnum.Exponential, 3 )]
		[Category( "Sampling" )]
		public Reference<double> Radius
		{
			get { if( _radius.BeginGet() ) Radius = _radius.Get( this ); return _radius.value; }
			set { if( _radius.BeginSet( ref value ) ) { try { RadiusChanged?.Invoke( this ); } finally { _radius.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Radius"/> property value changes.</summary>
		public event Action<RenderingEffect_IndirectLighting> RadiusChanged;
		ReferenceField<double> _radius = 3;

		/// <summary>
		/// Controls samples distribution. Exp Start is an initial multiplier on the step size, and Exp Factor is an exponent applied at each step. By using a start value < 1, and an exponent > 1, it's possible to get exponential step size.
		/// </summary>
		[DefaultValue( 1.0 )]
		[Range( 0, 5, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		[Category( "Sampling" )]
		public Reference<double> ExpStart
		{
			get { if( _expStart.BeginGet() ) ExpStart = _expStart.Get( this ); return _expStart.value; }
			set { if( _expStart.BeginSet( ref value ) ) { try { ExpStartChanged?.Invoke( this ); } finally { _expStart.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ExpStart"/> property value changes.</summary>
		public event Action<RenderingEffect_IndirectLighting> ExpStartChanged;
		ReferenceField<double> _expStart = 1.0;

		/// <summary>
		/// Controls samples distribution. Exp Start is an initial multiplier on the step size, and Exp Factor is an exponent applied at each step. By using a start value < 1, and an exponent > 1, it's possible to get exponential step size.
		/// </summary>
		[DefaultValue( 1.0 )]
		[Range( 1, 5, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		[Category( "Sampling" )]
		public Reference<double> ExpFactor
		{
			get { if( _expFactor.BeginGet() ) ExpFactor = _expFactor.Get( this ); return _expFactor.value; }
			set { if( _expFactor.BeginSet( ref value ) ) { try { ExpFactorChanged?.Invoke( this ); } finally { _expFactor.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ExpFactor"/> property value changes.</summary>
		public event Action<RenderingEffect_IndirectLighting> ExpFactorChanged;
		ReferenceField<double> _expFactor = 1.0;

		/// <summary>
		/// A factor of adding sky lighting.
		/// </summary>
		[DefaultValue( 0.0 )]
		[Range( 0, 1 )]
		[Category( "Sampling" )]
		public Reference<double> SkyLighting
		{
			get { if( _skyLighting.BeginGet() ) SkyLighting = _skyLighting.Get( this ); return _skyLighting.value; }
			set { if( _skyLighting.BeginSet( ref value ) ) { try { SkyLightingChanged?.Invoke( this ); } finally { _skyLighting.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="SkyLighting"/> property value changes.</summary>
		public event Action<RenderingEffect_IndirectLighting> SkyLightingChanged;
		ReferenceField<double> _skyLighting = 0.0;

		///// <summary>
		///// Applies some noise on sample positions to hide the banding artifacts that can occur when there is undersampling.
		///// </summary>
		//[DefaultValue( true )]
		//[Category( "Sampling" )]
		//public Reference<bool> JitterSamples
		//{
		//	get { if( _jitterSamples.BeginGet() ) JitterSamples = _jitterSamples.Get( this ); return _jitterSamples.value; }
		//	set { if( _jitterSamples.BeginSet( ref value ) ) { try { JitterSamplesChanged?.Invoke( this ); } finally { _jitterSamples.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="JitterSamples"/> property value changes.</summary>
		//public event Action<RenderingEffect_IndirectLighting> JitterSamplesChanged;
		//ReferenceField<bool> _jitterSamples = true;

		///// <summary>
		///// Bypass the dot(lightNormal, lightDirection) weighting.
		///// </summary>
		//[DefaultValue( 0.0 )]
		//[Range( 0, 1 )]
		//[Category( "GI" )]
		//public Reference<double> LnDlOffset
		//{
		//	get { if( _lnDlOffset.BeginGet() ) LnDlOffset = _lnDlOffset.Get( this ); return _lnDlOffset.value; }
		//	set { if( _lnDlOffset.BeginSet( ref value ) ) { try { LnDlOffsetChanged?.Invoke( this ); } finally { _lnDlOffset.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="LnDlOffset"/> property value changes.</summary>
		//public event Action<RenderingEffect_IndirectLighting> LnDlOffsetChanged;
		//ReferenceField<double> _lnDlOffset = 0.0;

		///// <summary>
		///// Bypass the dot(normal, lightDirection) weighting.
		///// </summary>
		//[DefaultValue( 0.0 )]
		//[Range( 0, 1 )]
		//[Category( "GI" )]
		//public Reference<double> NDlOffset
		//{
		//	get { if( _nDlOffset.BeginGet() ) NDlOffset = _nDlOffset.Get( this ); return _nDlOffset.value; }
		//	set { if( _nDlOffset.BeginSet( ref value ) ) { try { NDlOffsetChanged?.Invoke( this ); } finally { _nDlOffset.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="NDlOffset"/> property value changes.</summary>
		//public event Action<RenderingEffect_IndirectLighting> NDlOffsetChanged;
		//ReferenceField<double> _nDlOffset = 0.0;

		/// <summary>
		/// Constant thickness value of objects on the screen in world space. Is used to ignore occlusion past that thickness level, as if light can travel behind the object.
		/// </summary>
		[DefaultValue( 5.0 )]
		[Range( 0.1, 10 )]
		[Category( "Occlusion" )]
		public Reference<double> Thickness
		{
			get { if( _thickness.BeginGet() ) Thickness = _thickness.Get( this ); return _thickness.value; }
			set { if( _thickness.BeginSet( ref value ) ) { try { ThicknessChanged?.Invoke( this ); } finally { _thickness.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Thickness"/> property value changes.</summary>
		public event Action<RenderingEffect_IndirectLighting> ThicknessChanged;
		ReferenceField<double> _thickness = 5.0;

		/// <summary>
		/// Occlusion falloff relative to distance.
		/// </summary>
		[DefaultValue( 1.0 )]
		[Range( 0.1, 50, RangeAttribute.ConvenientDistributionEnum.Exponential, 3 )]
		[Category( "Occlusion" )]
		public Reference<double> Falloff
		{
			get { if( _falloff.BeginGet() ) Falloff = _falloff.Get( this ); return _falloff.value; }
			set { if( _falloff.BeginSet( ref value ) ) { try { FalloffChanged?.Invoke( this ); } finally { _falloff.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Falloff"/> property value changes.</summary>
		public event Action<RenderingEffect_IndirectLighting> FalloffChanged;
		ReferenceField<double> _falloff = 1.0;

		///// <summary>
		///// The number of frames to accumulate indirect lighting.
		///// </summary>
		//[DefaultValue( 10 )]
		//[Range( 0, 60, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		//public Reference<int> AccumulateFrames
		//{
		//	get { if( _accumulateFrames.BeginGet() ) AccumulateFrames = _accumulateFrames.Get( this ); return _accumulateFrames.value; }
		//	set { if( _accumulateFrames.BeginSet( ref value ) ) { try { AccumulateFramesChanged?.Invoke( this ); } finally { _accumulateFrames.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="AccumulateFrames"/> property value changes.</summary>
		//public event Action<RenderingEffect_IndirectLighting> AccumulateFramesChanged;
		//ReferenceField<int> _accumulateFrames = 10;

		/// <summary>
		/// The amount of the blur applied.
		/// </summary>
		[Serialize]
		[DefaultValue( 5.0 )]
		[Range( 0, 15, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		[Category( "Blur" )]
		public Reference<double> BlurFactor
		{
			get { if( _blurFactor.BeginGet() ) BlurFactor = _blurFactor.Get( this ); return _blurFactor.value; }
			set { if( _blurFactor.BeginSet( ref value ) ) { try { BlurFactorChanged?.Invoke( this ); } finally { _blurFactor.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="BlurFactor"/> property value changes.</summary>
		public event Action<RenderingEffect_IndirectLighting> BlurFactorChanged;
		ReferenceField<double> _blurFactor = 5;

		/// <summary>
		/// The blur downscaling mode used.
		/// </summary>
		[DefaultValue( RenderingPipeline_Basic.DownscalingModeEnum.Auto )]
		[Serialize]
		[Category( "Blur" )]
		public Reference<RenderingPipeline_Basic.DownscalingModeEnum> BlurDownscalingMode
		{
			get { if( _blurDownscalingMode.BeginGet() ) BlurDownscalingMode = _blurDownscalingMode.Get( this ); return _blurDownscalingMode.value; }
			set { if( _blurDownscalingMode.BeginSet( ref value ) ) { try { BlurDownscalingModeChanged?.Invoke( this ); } finally { _blurDownscalingMode.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="BlurDownscalingMode"/> property value changes.</summary>
		public event Action<RenderingEffect_IndirectLighting> BlurDownscalingModeChanged;
		ReferenceField<RenderingPipeline_Basic.DownscalingModeEnum> _blurDownscalingMode = RenderingPipeline_Basic.DownscalingModeEnum.Auto;

		/// <summary>
		/// The level of blur texture downscaling.
		/// </summary>
		[DefaultValue( 0 )]
		[Serialize]
		[Range( 0, 6 )]
		[Category( "Blur" )]
		public Reference<int> BlurDownscalingValue
		{
			get { if( _blurDownscalingValue.BeginGet() ) BlurDownscalingValue = _blurDownscalingValue.Get( this ); return _blurDownscalingValue.value; }
			set { if( _blurDownscalingValue.BeginSet( ref value ) ) { try { BlurDownscalingValueChanged?.Invoke( this ); } finally { _blurDownscalingValue.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="BlurDownscalingValue"/> property value changes.</summary>
		public event Action<RenderingEffect_IndirectLighting> BlurDownscalingValueChanged;
		ReferenceField<int> _blurDownscalingValue = 0;

		/// <summary>
		/// Enables the debug visualization of the effect.
		/// </summary>
		[Serialize]
		[DefaultValue( false )]
		[Category( "Debug" )]
		public Reference<bool> ShowIndirectLighting
		{
			get { if( _showIndirectLighting.BeginGet() ) ShowIndirectLighting = _showIndirectLighting.Get( this ); return _showIndirectLighting.value; }
			set { if( _showIndirectLighting.BeginSet( ref value ) ) { try { ShowIndirectLightingChanged?.Invoke( this ); } finally { _showIndirectLighting.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ShowIndirectLighting"/> property value changes.</summary>
		public event Action<RenderingEffect_IndirectLighting> ShowIndirectLightingChanged;
		ReferenceField<bool> _showIndirectLighting;

		//!!!!?
		///// <summary>
		///// Whether to visualize the bounds of the voxel grid.
		///// </summary>
		//[Category( "Debug" )]
		//[DefaultValue( false )]
		//public Reference<bool> DebugShowGridBounds
		//{
		//	get { if( _debugShowGridBounds.BeginGet() ) DebugShowGridBounds = _debugShowGridBounds.Get( this ); return _debugShowGridBounds.value; }
		//	set { if( _debugShowGridBounds.BeginSet( ref value ) ) { try { DebugShowGridBoundsChanged?.Invoke( this ); } finally { _debugShowGridBounds.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="DebugShowGridBounds"/> property value changes.</summary>
		//public event Action<RenderingEffect_IndirectLighting> DebugShowGridBoundsChanged;
		//ReferenceField<bool> _debugShowGridBounds = false;

		public enum DebugModeEnum
		{
			None = 0,
			Normal = 1,
			BaseColor = 2,
			Metallic = 3,
			Roughness = 4,
			//Reflectance = 5,
			//Emissive = 6,
			//SubsurfaceColor = 7,
			Radiance = 8
		}

		/// <summary>
		/// The way to visualize internal data.
		/// </summary>
		[Category( "Debug" )]
		[DefaultValue( DebugModeEnum.None )]
		public Reference<DebugModeEnum> DebugMode
		{
			get { if( _debugMode.BeginGet() ) DebugMode = _debugMode.Get( this ); return _debugMode.value; }
			set { if( _debugMode.BeginSet( ref value ) ) { try { DebugModeChanged?.Invoke( this ); } finally { _debugMode.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DebugMode"/> property value changes.</summary>
		public event Action<RenderingEffect_IndirectLighting> DebugModeChanged;
		ReferenceField<DebugModeEnum> _debugMode = DebugModeEnum.None;

		/// <summary>
		/// The level of the grid to visualize.
		/// </summary>
		[Category( "Debug" )]
		[Range( 0, 10 )]
		[DefaultValue( 0 )]
		public Reference<int> DebugModeLevel
		{
			get { if( _debugModeLevel.BeginGet() ) DebugModeLevel = _debugModeLevel.Get( this ); return _debugModeLevel.value; }
			set { if( _debugModeLevel.BeginSet( ref value ) ) { try { DebugModeLevelChanged?.Invoke( this ); } finally { _debugModeLevel.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DebugModeLevel"/> property value changes.</summary>
		public event Action<RenderingEffect_IndirectLighting> DebugModeLevelChanged;
		ReferenceField<int> _debugModeLevel = 0;

		/// <summary>
		/// The intensity of debug mode.
		/// </summary>
		[Category( "Debug" )]
		[Range( 0, 1 )]
		[DefaultValue( 1.0 )]
		public Reference<double> DebugModeIntensity
		{
			get { if( _debugModeIntensity.BeginGet() ) DebugModeIntensity = _debugModeIntensity.Get( this ); return _debugModeIntensity.value; }
			set { if( _debugModeIntensity.BeginSet( ref value ) ) { try { DebugModeIntensityChanged?.Invoke( this ); } finally { _debugModeIntensity.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DebugModeIntensity"/> property value changes.</summary>
		public event Action<RenderingEffect_IndirectLighting> DebugModeIntensityChanged;
		ReferenceField<double> _debugModeIntensity = 1.0;

		///////////////////////////////////////////////

		//only for full mode
		public class FrameData
		{
			public RenderingEffect_IndirectLighting Owner;

			public int Levels;
			public int GridResolution;
			public float[] DistanceByLevel;
			public Vector3 GridCenter;
			public float CellSizeLevel0;
			public Bounds[] BoundsByLevel;
			public Bounds TotalGridBounds;

			//

			public bool GetGridIndexes( ref Vector3 gridPosition, ref float cellSizeOfLevelInv, ref Bounds bounds, out BoundsI gridIndexes )
			//public bool GetGridIndexes( int level, ref Bounds bounds, out BoundsI gridIndexes )
			{
				//var cellSizeOfLevel = CellSizeLevel0 * MathEx.Pow( 2.0f, level );
				//var gridSizeHalf = cellSizeOfLevel * GridResolution * 0.5f;
				//var gridPosition = GridCenter - new Vector3( gridSizeHalf, gridSizeHalf, gridSizeHalf );
				//var cellSizeOfLevelInv = 1.0f / cellSizeOfLevel;

				var indexMinF = ( bounds.Minimum - gridPosition ) * cellSizeOfLevelInv;
				var indexMin = indexMinF.ToVector3I();
				var indexMaxF = ( bounds.Maximum - gridPosition ) * cellSizeOfLevelInv;
				var indexMax = indexMaxF.ToVector3I();

				gridIndexes = new BoundsI( indexMin, indexMax );

				if( indexMax.X < 0 || indexMax.Y < 0 || indexMax.Z < 0 )
					return false;
				if( indexMin.X >= GridResolution || indexMin.Y >= GridResolution || indexMin.Z >= GridResolution )
					return false;

				if( gridIndexes.Minimum.X < 0 )
					gridIndexes.Minimum.X = 0;
				if( gridIndexes.Minimum.Y < 0 )
					gridIndexes.Minimum.Y = 0;
				if( gridIndexes.Minimum.Z < 0 )
					gridIndexes.Minimum.Z = 0;
				if( gridIndexes.Maximum.X >= GridResolution )
					gridIndexes.Maximum.X = GridResolution - 1;
				if( gridIndexes.Maximum.Y >= GridResolution )
					gridIndexes.Maximum.Y = GridResolution - 1;
				if( gridIndexes.Maximum.Z >= GridResolution )
					gridIndexes.Maximum.Z = GridResolution - 1;

				return true;
			}
		}

		///////////////////////////////////////////////

		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			base.OnMetadataGetMembersFilter( context, member, ref skip );

			var p = member as Metadata.Property;
			if( p != null )
			{
				switch( p.Name )
				{
				case nameof( Quality ):
				case nameof( ResolutionScreenSpace ):
				case nameof( Multiplier ):
				case nameof( Reduction ):
				case nameof( Radius ):
				case nameof( ExpStart ):
				case nameof( ExpFactor ):
				case nameof( SkyLighting ):
				case nameof( Thickness ):
				case nameof( Falloff ):
				case nameof( ShowIndirectLighting ):
					if( Technique.Value != TechniqueEnum.ScreenSpace )
						skip = true;
					break;

				case nameof( Distance ):
				case nameof( Levels ):
				case nameof( GridSize ):
				//case nameof( RadianceSteps ):
				case nameof( VoxelizationExpandFactor ):
				//case nameof( AffectObjects ):
				//case nameof( LODScale ):
				case nameof( AmbientLighting ):
				case nameof( DirectLighting ):
				case nameof( EmissiveLighting ):
				//!!!!case nameof( DebugShowGridBounds ):
				case nameof( DebugMode ):
				case nameof( DiffuseOutput ):
				case nameof( SpecularOutput ):
				case nameof( ResolutionFull ):
				case nameof( ShadingModelFull ):
				case nameof( TraceLength ):
				case nameof( TraceStartOffset ):
				case nameof( TraceStepFactor ):
					//case nameof( Multiplier ):
					//case nameof( UseMaterialSettings ):
					//case nameof( Dynamic ):
					if( Technique.Value != TechniqueEnum.Full )
						skip = true;
					break;

				case nameof( DebugModeLevel ):
				case nameof( DebugModeIntensity ):
					if( Technique.Value != TechniqueEnum.Full )// || DebugMode.Value == DebugModeEnum.None )
						skip = true;
					break;

				case nameof( BlurDownscalingValue ):
					if( BlurDownscalingMode.Value == RenderingPipeline_Basic.DownscalingModeEnum.Auto )
						skip = true;
					break;
				}
			}
		}

		//static ImageComponent CreateAccumulationBuffer( Vector2I size, PixelFormat format )
		//{
		//	ImageComponent texture = ComponentUtility.CreateComponent<ImageComponent>( null, true, false );
		//	texture.CreateSize = size;
		//	texture.CreateFormat = format;
		//	texture.CreateUsage = ImageComponent.Usages.RenderTarget;
		//	texture.Enabled = true;

		//	RenderTexture renderTexture = texture.Result.GetRenderTarget();
		//	var viewport = renderTexture.AddViewport( false, false );

		//	viewport.RenderingPipelineCreate();
		//	viewport.RenderingPipelineCreated.UseRenderTargets = false;

		//	return texture;
		//}

		unsafe protected override void OnRender( ViewportRenderingContext context, RenderingPipeline_Basic.FrameData frameData, ref ImageComponent actualTexture )
		{
			base.OnRender( context, frameData, ref actualTexture );

			if( Intensity <= 0 )
				return;

			var multiplier = Multiplier * GlobalMultiplier;
			if( multiplier <= 0 )
				return;

			var pipeline = context.RenderingPipeline;

			//is not supported
			if( !pipeline.GetUseMultiRenderTargets() )
				return;

			if( Technique.Value == TechniqueEnum.ScreenSpace )
				RenderScreenSpaceTechnique( context, frameData, ref actualTexture, multiplier );
			if( Technique.Value == TechniqueEnum.Full )
				RenderFullTechniqueDebug( context, frameData, ref actualTexture );
		}

		///////////////////////////////////////////////

		unsafe void RenderScreenSpaceTechnique( ViewportRenderingContext context, RenderingPipeline_Basic.FrameData frameData, ref ImageComponent actualTexture, double multiplier )
		{
			var pipeline = context.RenderingPipeline;

			////get noise texture
			//const string noiseTextureDefault = @"Base\Images\Noise.png";
			//var noiseTexture = ResourceManager.LoadResource<Image>( noiseTextureDefault );
			//if( noiseTexture == null )
			//	return;

			//downscale
			//var downscaledTexture = actualTexture;
			//if( actualTexture.Result.ResultSize != context.Owner.SizeInPixels )
			//{
			//	actualTexture = context.RenderTarget2D_Alloc( context.Owner.SizeInPixels, actualTextureSource.Result.ResultFormat );

			//	//copy to scene texture with downscale
			//	context.SetViewport( actualTexture.Result.GetRenderTarget().Viewports[ 0 ] );

			//	CanvasRenderer.ShaderItem shader = new CanvasRenderer.ShaderItem();
			//	shader.VertexProgramFileName = @"Base\Shaders\EffectsCommon_vs.sc";
			//	shader.FragmentProgramFileName = @"Base\Shaders\Effects\Downscale2_fs.sc";

			//	shader.Parameters.Set( "sourceSizeInv", new Vector2F( 1, 1 ) / actualTextureSource.Result.ResultSize.ToVector2F() );

			//	shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 0/*"sourceTexture"*/,
			//		actualTextureSource, TextureAddressingMode.Clamp, FilterOption.Linear, FilterOption.Linear, FilterOption.None ) );

			//	context.RenderQuadToCurrentViewport( shader );
			//}

			////!!!!сбрасывать при резком перемещении камеры. может счетчик в viewport добавить чтобы не через эвент
			////update accumulation buffer
			//Image accumulationBuffer = null;
			//{
			//	var anyDataKey = "IndirectLighting " + GetPathFromRoot();

			//	//!!!!downscaled?
			//	var demandedSize = actualTexture/*downscaledTexture*/.Result.ResultSize;

			//	//check to destroy
			//	{
			//		context.anyImageAutoDispose.TryGetValue( anyDataKey, out var current );

			//		if( AccumulateFrames.Value == 0 || ( current != null && current.Result.ResultSize != demandedSize ) )
			//		{
			//			//destroy
			//			context.anyImageAutoDispose.Remove( anyDataKey );
			//			current?.Dispose();
			//		}
			//	}

			//	//create and get
			//	if( AccumulateFrames.Value != 0 )
			//	{
			//		context.anyImageAutoDispose.TryGetValue( anyDataKey, out var current );

			//		if( current == null )
			//		{
			//			//create
			//			current = CreateAccumulationBuffer( demandedSize, actualTexture.Result.ResultFormat );
			//			context.anyImageAutoDispose[ anyDataKey ] = current;

			//			//clear
			//			context.SetViewport( current.Result.GetRenderTarget().Viewports[ 0 ], Matrix4F.Identity, Matrix4F.Identity, FrameBufferTypes.Color, ColorValue.Zero );
			//		}

			//		accumulationBuffer = current;
			//	}
			//}


			//calculate indirect lighting

			//Image lightingTexture = null;

			//Image lightingNoAccumulationTexture = null;
			//if( accumulationBuffer != null )
			//	lightingTexture = accumulationBuffer;
			//else
			//{
			//	lightingNoAccumulationTexture = context.RenderTarget2D_Alloc( actualTexture/*downscaledTexture*/.Result.ResultSize, actualTexture/*downscaledTexture*/.Result.ResultFormat );
			//	lightingTexture = lightingNoAccumulationTexture;
			//}

			var lightingTextureSize = actualTexture/*downscaledTexture*/.Result.ResultSize / (int)ResolutionScreenSpace.Value;

			var lightingTexture = context.RenderTarget2D_Alloc( lightingTextureSize, actualTexture/*downscaledTexture*/.Result.ResultFormat );
			{
				context.SetViewport( lightingTexture.Result.GetRenderTarget().Viewports[ 0 ] );

				CanvasRenderer.ShaderItem shader = new CanvasRenderer.ShaderItem();
				shader.VertexProgramFileName = @"Base\Shaders\EffectsCommon_vs.sc";
				shader.FragmentProgramFileName = @"Base\Shaders\Effects\IndirectLighting\ScreenSpace_Lighting_fs.sc";

				context.ObjectsDuringUpdate.namedTextures.TryGetValue( "depthTexture", out var depthTexture );
				context.ObjectsDuringUpdate.namedTextures.TryGetValue( "normalTexture", out var normalTexture );

				shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 0, actualTexture/*downscaledTexture*/, TextureAddressingMode.Clamp, FilterOption.Linear, FilterOption.Linear, FilterOption.None ) );
				shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 1, depthTexture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.None ) );
				shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 2, normalTexture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.None ) );
				//shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 3, noiseTexture, TextureAddressingMode.Wrap, FilterOption.Point, FilterOption.Point, FilterOption.Point ) );
				//shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 4, accumulationBuffer ?? ResourceUtility.BlackTexture2D, TextureAddressingMode.Wrap, FilterOption.Point, FilterOption.Point, FilterOption.Point ) );


				//float aspectRatio = (float)context.Owner.CameraSettings.AspectRatio;
				//float fov = (float)context.Owner.CameraSettings.FieldOfView;
				//float zNear = (float)context.Owner.CameraSettings.NearClipDistance;
				//float zFar = (float)context.Owner.CameraSettings.FarClipDistance;

				//shader.Parameters.Set( "colorTextureSize", new Vector4F( (float)lightingTexture.Result.ResultSize.X, (float)lightingTexture.Result.ResultSize.Y, 0.0f, 0.0f ) );
				//shader.Parameters.Set( "zNear", zNear );
				//shader.Parameters.Set( "zFar", zFar );
				//shader.Parameters.Set( "fov", fov );
				//shader.Parameters.Set( "aspectRatio", aspectRatio );

				//{
				//	var size = lightingTexture.Result.ResultSize;
				//	shader.Parameters.Set( "viewportSize", new Vector4( size.X, size.Y, 1.0 / (double)size.X, 1.0 / (double)size.Y ).ToVector4F() );
				//}

				//{
				//	var size = noiseTexture.Result.ResultSize;
				//	shader.Parameters.Set( "noiseTextureSize", new Vector4( size.X, size.Y, 1.0 / (double)size.X, 1.0 / (double)size.Y ).ToVector4F() );
				//}

				int rotationCount = 3;
				int stepCount = 4;
				switch( Quality.Value )
				{
				case QualityEnum.Low: rotationCount = 2; stepCount = 3; break;
				case QualityEnum.Medium: rotationCount = 3; stepCount = 6; break;
				case QualityEnum.High: rotationCount = 4; stepCount = 9; break;
				}

				shader.Defines.Add( new CanvasRenderer.ShaderItem.DefineItem( "ROTATION_COUNT", rotationCount.ToString() ) );
				shader.Defines.Add( new CanvasRenderer.ShaderItem.DefineItem( "STEP_COUNT", stepCount.ToString() ) );


				shader.Parameters.Set( "resolution", (float)ResolutionScreenSpace.Value );
				shader.Parameters.Set( "skyLighting", (float)SkyLighting.Value );

				shader.Parameters.Set( "radius", (float)Radius.Value );
				shader.Parameters.Set( "expStart", (float)ExpStart.Value );
				shader.Parameters.Set( "expFactor", (float)ExpFactor.Value );
				//shader.Parameters.Set( "jitterSamples", (float)( JitterSamples.Value ? 1.0f : 0.0f ) );

				//shader.Parameters.Set( "lnDlOffset", (float)LnDlOffset.Value );
				//shader.Parameters.Set( "nDlOffset", (float)NDlOffset.Value );

				shader.Parameters.Set( "thickness", (float)Thickness.Value );
				shader.Parameters.Set( "falloff", (float)Falloff.Value );

				shader.Parameters.Set( "reduction", (float)Reduction.Value );

				//!!!!double
				Matrix4F itViewMatrix = ( context.Owner.CameraSettings.ViewMatrix.GetInverse().ToMatrix4F() ).GetTranspose();
				shader.Parameters.Set( "itViewMatrix", itViewMatrix );

				//Vector4F seeds = Vector4F.Zero;
				//if( AccumulateFrames.Value != 0 )
				//{
				//	var random = new Random();
				//	seeds = new Vector4F( random.NextFloat(), random.NextFloat(), random.NextFloat(), random.NextFloat() );
				//}
				//shader.Parameters.Set( "randomSeeds", seeds );

				//shader.Parameters.Set( "accumulateFrames", (float)AccumulateFrames.Value );

				//!!!!double
				Matrix4F projectionMatrix = context.Owner.CameraSettings.ProjectionMatrix.ToMatrix4F();
				Matrix4F invProjectionMatrix = projectionMatrix.GetInverse();
				//Matrix4F viewMatrix = context.Owner.CameraSettings.ViewMatrix.ToMatrix4F();
				//Matrix4F viewProjMatrix = projectionMatrix * viewMatrix;
				//Matrix4F invViewProjMatrix = viewProjMatrix.GetInverse();

				//shader.Parameters.Set( "viewProj", viewProjMatrix );
				//shader.Parameters.Set( "invViewProj", invViewProjMatrix );
				shader.Parameters.Set( "invProj", invProjectionMatrix );

				//!!!!double
				Vector3F cameraPosition = context.Owner.CameraSettings.Position.ToVector3F();
				shader.Parameters.Set( "cameraPosition", cameraPosition );

				var fov = (double)context.Owner.CameraSettings.FieldOfView.InRadians();
				double halfProjScale = (double)context.Owner.SizeInPixels.Y / ( Math.Tan( fov * 0.5f ) * 2 ) * 0.5f;
				shader.Parameters.Set( "halfProjScale", (float)halfProjScale );


				//if( accumulationBuffer != null )
				//	context.RenderQuadToCurrentViewport( shader, CanvasRenderer.BlendingType.Add );
				//else
				context.RenderQuadToCurrentViewport( shader );
			}

			////copy to accumulation buffer
			//if( accumulationBuffer != null )
			//{
			//	context.SetViewport( accumulationBuffer.Result.GetRenderTarget().Viewports[ 0 ] );
			//	( (RenderingPipeline_Basic)context.renderingPipeline ).CopyToCurrentViewport( context, lightingTexture );
			//}

			//if( downscaledTexture != actualTexture )
			//	context.DynamicTexture_Free( downscaledTexture );

			//blur
			var blurTexture = pipeline.GaussianBlur( context, /* accumulationBuffer ?? */lightingTexture, BlurFactor, BlurDownscalingMode, BlurDownscalingValue );

			if( lightingTexture != null )
				context.DynamicTexture_Free( lightingTexture );
			//if( lightingNoAccumulationTexture != null )
			//	context.DynamicTexture_Free( lightingNoAccumulationTexture );

			//final pass
			var finalTexture = context.RenderTarget2D_Alloc( actualTexture.Result.ResultSize, actualTexture.Result.ResultFormat );
			{
				context.SetViewport( finalTexture.Result.GetRenderTarget().Viewports[ 0 ] );

				CanvasRenderer.ShaderItem shader = new CanvasRenderer.ShaderItem();
				shader.VertexProgramFileName = @"Base\Shaders\EffectsCommon_vs.sc";
				shader.FragmentProgramFileName = @"Base\Shaders\Effects\IndirectLighting\ScreenSpace_Final_fs.sc";

				shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 0, actualTexture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.None ) );
				shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 1, blurTexture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.None ) );

				shader.Parameters.Set( "intensity", (float)Intensity );
				shader.Parameters.Set( "showIndirectLighting", ShowIndirectLighting.Value ? 1.0f : 0.0f );
				//!!!!не так умножать. они же в степени
				shader.Parameters.Set( "multiplier", (float)multiplier * 10.0f * (float)ResolutionScreenSpace.Value );

				//#if __TEST
				//shader.Parameters.Set( "testNew", (float)0.0f );
				//#endif

				context.RenderQuadToCurrentViewport( shader );
			}

			//free targets
			context.DynamicTexture_Free( blurTexture );
			context.DynamicTexture_Free( actualTexture );
			//!!!!какие еще?

			//update actual texture
			actualTexture = finalTexture;
		}

		///////////////////////////////////////////////

		unsafe void RenderFullTechniqueDebug( ViewportRenderingContext context, RenderingPipeline_Basic.FrameData frameData, ref ImageComponent actualTexture )
		{
			if( DebugMode.Value != DebugModeEnum.None && frameData.IndirectLightingFrameData != null && DebugModeIntensity.Value > 0 )
			{
				context.ObjectsDuringUpdate.namedTextures.TryGetValue( "giGBufferGridTexture", out ImageComponent giGBufferGridTexture );
				if( giGBufferGridTexture == null )
					return;
				context.ObjectsDuringUpdate.namedTextures.TryGetValue( "giLightingGridTexture", out ImageComponent giLightingGridTexture );
				if( giLightingGridTexture == null )
					return;
				context.ObjectsDuringUpdate.namedTextures.TryGetValue( "depthTexture", out ImageComponent depthTexture );
				if( depthTexture == null )
					return;

				var data = context.FrameData.IndirectLightingFrameData;

				//final pass
				var finalTexture = context.RenderTarget2D_Alloc( actualTexture.Result.ResultSize, actualTexture.Result.ResultFormat );
				{
					context.SetViewport( finalTexture.Result.GetRenderTarget().Viewports[ 0 ] );

					var shader = new CanvasRenderer.ShaderItem();
					shader.VertexProgramFileName = @"Base\Shaders\EffectsCommon_vs.sc";
					shader.FragmentProgramFileName = @"Base\Shaders\Effects\IndirectLighting\Full_Debug_fs.sc";

					shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 0, actualTexture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.None ) );
					shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 1, giGBufferGridTexture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.None ) );
					shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 2, giLightingGridTexture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.None ) );
					shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 3, depthTexture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.None ) );


					shader.Parameters.Set( "intensity", (float)DebugModeIntensity );

					shader.Parameters.Set( "gridParameters", new Vector4F( data.Levels, data.GridResolution, data.CellSizeLevel0, 0 ) );
					//!!!!double
					shader.Parameters.Set( "gridCenter", new Vector4F( data.GridCenter.ToVector3F(), 0 ) );

					//shader.Parameters.Set( "gridPosition", data.TotalGridBounds.Minimum.ToVector3F() );

					//shader.Parameters.Set( "gridSize", (float)indirectFrameData.GridSize );
					////!!!!double
					//shader.Parameters.Set( "gridPosition", indirectFrameData.GridPosition.ToVector3F() );
					//shader.Parameters.Set( "cellSize", (float)indirectFrameData.CellSize );

					shader.Parameters.Set( "debugModeParameters", new Vector4F( (float)DebugMode.Value, DebugModeLevel.Value, 0, 0 ) );

					//!!!!need?

					//!!!!
					//!!!!где еще заюзать

					//!!!!double
					context.Owner.CameraSettings.GetViewProjectionInverseMatrix().ToMatrix4F( out var invViewProjMatrix );

					////!!!!double
					//Matrix4F projectionMatrix = context.Owner.CameraSettings.ProjectionMatrix.ToMatrix4F();
					//Matrix4F viewMatrix = context.Owner.CameraSettings.ViewMatrix.ToMatrix4F();

					//Matrix4F viewProjMatrix = projectionMatrix * viewMatrix;
					//viewProjMatrix.GetInverse( out var invViewProjMatrix );

					//!!!!double
					shader.Parameters.Set( "cameraPosition", context.Owner.CameraSettings.Position.ToVector3F() );
					//shader.Parameters.Set( "viewProj", viewProjMatrix );
					shader.Parameters.Set( "invViewProj", invViewProjMatrix );

					context.RenderQuadToCurrentViewport( shader );
				}

				//free targets
				context.DynamicTexture_Free( actualTexture );

				//update actual texture
				actualTexture = finalTexture;
			}
		}
	}
}









/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



//static Program? compiledProgram;

//static ImageComponent texture;

////!!!!
//static Uniform? u_testParameter;


////static Dictionary<int, Program> bgfxComputePrograms = new Dictionary<int, Program>();
////static Uniform? u_skinningParameters;
////static Dictionary<int, Uniform> u_skinningBones = new Dictionary<int, Uniform>();


////!!!!

//Program? GetProgram()
////GpuProgram GetProgram()
//{
//	if( compiledProgram == null )
//	{

//		var defines = new List<(string, string)>();
//		//defines.Add( ("MAX_BONES", maxBones.ToString()) );

//		var program = GpuProgramManager.GetProgram( "IndirectLighting", GpuProgramType.Compute, @"Base\Shaders\Effects\IndirectLighting\TestCompute.sc", defines, true, out var error2 );
//		if( !string.IsNullOrEmpty( error2 ) )
//		{
//			//var error = GpuProgramManager.GetGpuProgramCompilationErrorText( this, error2 );
//			//!!!!
//			//Log.Error( error );
//			Log.Warning( error2 );


//			//bgfxProgram = Program.Invalid;
//			return null;
//		}

//		compiledProgram = new Program( program.RealObject );
//	}

//	return compiledProgram.Value;

//	//return program;
//}

////bool GetBgfxComputeProgram( int maxBones, out Program bgfxProgram )
////{
////	// Try to get program from the cache.
////	if( bgfxComputePrograms.TryGetValue( maxBones, out bgfxProgram ) )
////		return true;

////	var defines = new List<(string, string)>();
////	defines.Add( ("MAX_BONES", maxBones.ToString()) );

////	var program = GpuProgramManager.GetProgram( "MeshSkeletonAnimation",
////		GpuProgramType.Compute, @"Base\Shaders\MeshSkeletonAnimation.sc", defines, out var error2 );
////	if( !string.IsNullOrEmpty( error2 ) )
////	{
////		var error = GpuProgramManager.GetGpuProgramCompilationErrorText( this, error2 );
////		Log.Error( error );

////		bgfxProgram = Program.Invalid;
////		return false;
////	}

////	bgfxProgram = new Program( program.RealObject );
////	bgfxComputePrograms[ maxBones ] = bgfxProgram;

////	return true;
////}



////!!!!
//void Test( ViewportRenderingContext context, RenderingPipeline.IFrameData frameData, ref ImageComponent actualTexture )
//{
//	var pipeline = (RenderingPipeline_Basic)context.RenderingPipeline;


//	var computeProgram = GetProgram();
//	if( computeProgram == null )
//		return;


//	//!!!!

//	//Log.Info( computeProgram.ToString() );


//	//!!!!удалять

//	if( texture == null )
//	{
//		texture = ComponentUtility.CreateComponent<ImageComponent>( null, true, false );

//		bool mipmaps = false;

//		texture.CreateType = ImageComponent.TypeEnum._2D;
//		texture.CreateSize = new Vector2I( 16, 1 );
//		texture.CreateMipmaps = mipmaps;
//		texture.CreateFormat = PixelFormat.Float32RGBA;
//		//texture.CreateFormat = PixelFormat.Float32R;

//		//!!!!write only?
//		var usage = ImageComponent.Usages.WriteOnly | ImageComponent.Usages.ComputeWrite;
//		if( mipmaps )
//			usage |= ImageComponent.Usages.AutoMipmaps;
//		texture.CreateUsage = usage;

//		texture.Enabled = true;

//		//!!!!
//		texture.Result.PrepareNativeObject();

//	}
//	//m_aoMap = bgfx::createTexture2D( uint16_t( m_size[ 0 ] ), uint16_t( m_size[ 1 ] ), false, 1, bgfx::TextureFormat::R8, BGFX_TEXTURE_COMPUTE_WRITE | SAMPLER_POINT_CLAMP );

//	//var texture = context.RenderTarget2D_Alloc( new Vector2I( 16, 1 ), PixelFormat.Float32R );
//	//var texture = context.RenderTarget2D_Alloc( new Vector2I( 16, 1 ), PixelFormat.Float16R );


//	unsafe
//	{

//		//!!!!TEMP чтобы обновить view number counter
//		context.SetComputeView();
//		//context.SetViewport( actualTexture.Result.GetRenderTarget().Viewports[ 0 ] );



//		//var maxBones = Math.Max( MathEx.NextPowerOfTwo( bones.Length ), 64 );

//		////set bone data
//		//var boneData = new Matrix4F[ maxBones ];
//		//{
//		//	//!!!!
//		//}

//		////enumerate render operations of the mesh
//		//for( int nOper = 0; nOper < modifiableMesh.Result.MeshData.RenderOperations.Count; nOper++ )
//		//{

//		//var sourceOper = originalMesh.Result.MeshData.RenderOperations[ nOper ];
//		//var destOper = modifiableMesh.Result.MeshData.RenderOperations[ nOper ];

//		//var sourceVertexBuffer = sourceOper.VertexBuffers[ 0 ];
//		//var destVertexBuffer = destOper.VertexBuffers[ 0 ];

//		//bind buffers
//		//if( sourceVertexBuffer.Flags.HasFlag( GpuBufferFlags.Dynamic ) || sourceVertexBuffer.Flags.HasFlag( GpuBufferFlags.ComputeWrite ) )
//		//	Bgfx.SetComputeBuffer( 0, (DynamicVertexBuffer)sourceVertexBuffer.GetNativeObject(), ComputeBufferAccess.Read );
//		//else
//		//	Bgfx.SetComputeBuffer( 0, (VertexBuffer)sourceVertexBuffer.GetNativeObject(), ComputeBufferAccess.Read );



//		//!!!!
//		//Bgfx.SetComputeBuffer(

//		//!!!!
//		Bgfx.SetComputeImage( 0, texture.Result.GetNativeObject( true ), 0, ComputeBufferAccess.Write, TextureFormat.RGBA32F );
//		//Bgfx.SetComputeImage( 0, texture.Result.GetRealObject( true ), 0, ComputeBufferAccess.Write, TextureFormat.R32F );
//		// R16F );
//		//Bgfx.SetComputeImage( 0, texture.Result.GetRealObject( false ), 0, ComputeBufferAccess.Write, TextureFormat.R32F );// R16F );

//		//Bgfx.SetComputeBuffer( 0, (DynamicVertexBuffer)destVertexBuffer.GetNativeObject(), ComputeBufferAccess.Write );


//		//bind parameters
//		{
//			Vector4F testParameter = TestParameter.Value;
//			//Vector4F testParameter = new Vector4F( TestParameter.Value, 0, 0, 0 );

//			var arraySize = 1;// sizeof( SkinningParameters ) / sizeof( Vector4F );
//			if( u_testParameter == null )
//				u_testParameter = GpuProgramManager.RegisterUniform( "u_testParameter", UniformType.Vector4, arraySize );
//			Bgfx.SetUniform( u_testParameter.Value, &testParameter, arraySize );

//			//var arraySize = sizeof( SkinningParameters ) / sizeof( Vector4F );
//			//if( u_skinningParameters == null )
//			//	u_skinningParameters = GpuProgramManager.RegisterUniform( "u_skinningParameters", UniformType.Vector4, arraySize );
//			//Bgfx.SetUniform( u_skinningParameters.Value, &parameters, arraySize );
//		}

//		//!!!!
//		Bgfx.Dispatch( context.CurrentViewNumber, computeProgram.Value, 1, 1, 1, DiscardFlags.All );

//		//!!!! ( sourceVertexBuffer.VertexCount + 1023 ) / 1024
//		//Bgfx.Dispatch( context.CurrentViewNumber, bgfxProgram, 1 );
//		//Bgfx.Dispatch( context.CurrentViewNumber, bgfxProgram, ( sourceVertexBuffer.VertexCount + 1023 ) / 1024 );
//		//}

//	}


//	//final pass
//	var finalTexture = context.RenderTarget2D_Alloc( actualTexture.Result.ResultSize, actualTexture.Result.ResultFormat );
//	{
//		context.SetViewport( finalTexture.Result.GetRenderTarget().Viewports[ 0 ] );

//		CanvasRenderer.ShaderItem shader = new CanvasRenderer.ShaderItem();
//		shader.VertexProgramFileName = @"Base\Shaders\EffectsCommon_vs.sc";
//		shader.FragmentProgramFileName = @"Base\Shaders\Effects\IndirectLighting\FinalTest_fs.sc";

//		shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 0, actualTexture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.None ) );

//		//!!!!
//		shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 1, texture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.None ) );

//		//!!!!
//		//shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 1, blurTexture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.None ) );

//		shader.Parameters.Set( "intensity", (float)Intensity );
//		shader.Parameters.Set( "showIndirectLighting", ShowIndirectLighting.Value ? 1.0f : 0.0f );
//		shader.Parameters.Set( "multiplier", (float)Multiplier * (float)Resolution.Value );


//		//!!!!
//		//shader.Parameters.Set( "testNew", (float)1.0f );


//		context.RenderQuadToCurrentViewport( shader );
//	}

//	//free targets
//	//!!!!
//	//context.DynamicTexture_Free( blurTexture );
//	context.DynamicTexture_Free( actualTexture );
//	//!!!!какие еще?

//	//update actual texture
//	actualTexture = finalTexture;

//}

////#if __TEST
////			//!!!!
////			if( TestNew.Value )
////			{
////				Test( context, frameData, ref actualTexture );
////				return;
////			}
////#endif



///////////////////////////////////////////////

//public class ViewportContextData : IDisposable
//{
//	//public ImageComponent GridTexture;


//	////public ImageComponent ObjectsTexture;
//	////public int ObjectsTextureCreatedCount;

//	////public ImageComponent ObjectTypesTexture;
//	////public int ObjectTypesTextureCreatedCount;

//	////public ImageComponent DirectRadianceTexture;
//	////public int DirectRadianceTextureCreatedCount;
//	////public ImageComponent RadianceObjectIdsTexture;
//	////public ImageComponent RadianceLocalVoxelIndicesTexture;

//	////public ImageComponent AccumulationRadianceTexture;

//	//

//	public void Dispose()
//	{
//		//GridTexture?.Dispose();
//		//GridTexture = null;


//		////ObjectsTexture?.Dispose();
//		////ObjectsTexture = null;

//		////ObjectTypesTexture?.Dispose();
//		////ObjectTypesTexture = null;

//		////DirectRadianceTexture?.Dispose();
//		////DirectRadianceTexture = null;

//		////RadianceObjectIdsTexture?.Dispose();
//		////RadianceObjectIdsTexture = null;

//		////RadianceLocalVoxelIndicesTexture?.Dispose();
//		////RadianceLocalVoxelIndicesTexture = null;

//		////AccumulationRadianceTexture?.Dispose();
//		////AccumulationRadianceTexture = null;
//	}
//}

///////////////////////////////////////////////

//!!!!
//internal static byte[] GetVoxelData( RenderingPipeline.RenderSceneData.IMeshData meshData )
////internal static RenderingPipeline.RenderSceneData.MeshDataRenderOperation GetVoxelData( RenderingPipeline.RenderSceneData.IMeshData meshData )
//{
//	var meshData2 = meshData;

//	var lods = meshData.LODs;
//	if( lods != null )
//		meshData2 = lods[ lods.Length - 1 ].Mesh?.Result?.MeshData;

//	if( meshData2 != null && meshData2.RenderOperations.Count == 1 )
//	{
//		var oper = meshData2.RenderOperations[ 0 ];

//		if( oper.SourceVoxelData != null )
//		{
//			return oper.SourceVoxelData;
//			//return oper;
//		}
//	}

//	return null;
//}

//!!!!temp
//internal static MeshGeometry GetVoxelMeshGeometry( MeshInSpace obj ) //GetVoxelRenderOperation
//{
//	var mesh = obj.Mesh.Value;
//	if( mesh?.Result != null )
//	{
//		var lods = mesh.Result.MeshData.LODs;
//		if( lods != null )
//			mesh = lods[ lods.Length - 1 ].Mesh;
//	}

//	if( mesh != null )
//	{
//		//mesh.

//		//mesh.Result.MeshData;

//		foreach( var geometry in mesh.GetComponents<MeshGeometry>() )
//		{
//			if( geometry.VoxelData.Value != null )
//				return geometry;
//		}
//	}

//	return null;
//}

////!!!!temp
//internal unsafe static bool ContainsGpuVoxelData( byte[] voxelData )
//{
//	if( voxelData.Length < sizeof( MeshGeometry.VoxelDataHeader ) )
//		return false;
//	var header = new MeshGeometry.VoxelDataHeader();
//	fixed( byte* pData = voxelData )
//		NativeUtility.CopyMemory( &header, pData, sizeof( MeshGeometry.VoxelDataHeader ) );

//	var sizeInBytes = voxelData.Length - sizeof( MeshGeometry.VoxelDataHeader );

//	if( header.Version == MeshGeometry.CurrentVoxelDataVersion && sizeInBytes != 0 )
//	{
//		return true;
//	}

//	return false;
//}

////!!!!temp
//internal unsafe static byte[] GetGpuVoxelData( byte[] voxelData )
//{

//	//!!!!юзать данные напрямую из VoxelData?

//	if( voxelData.Length < sizeof( MeshGeometry.VoxelDataHeader ) )
//		return null;
//	var header = new MeshGeometry.VoxelDataHeader();
//	fixed( byte* pData = voxelData )
//		NativeUtility.CopyMemory( &header, pData, sizeof( MeshGeometry.VoxelDataHeader ) );

//	var sizeInBytes = voxelData.Length - sizeof( MeshGeometry.VoxelDataHeader );

//	if( header.Version == MeshGeometry.CurrentVoxelDataVersion && sizeInBytes != 0 )
//	{
//		//!!!!что не нужно? где еще

//		var gpuHeaderSizeInBytes = sizeof( Vector4F ) * 3;
//		//var gpuHeaderSizeInBytes = sizeof( Vector4F ) * 2;
//		var totalSizeInBytes = gpuHeaderSizeInBytes + sizeInBytes;

//		//!!!!
//		totalSizeInBytes += header.VoxelCount * 4;

//		var gpuVoxelData = new byte[ totalSizeInBytes ];


//		var voxelDataInfo = new Vector4F[ 3 ];
//		//var voxelDataInfo = new Vector4F[ 2 ];
//		float fillHolesDistanceAndFormat = header.FillHolesDistance;
//		if( fillHolesDistanceAndFormat < 0.001f )
//			fillHolesDistanceAndFormat = 0.001f;
//		if( header.Format != 0 )
//			fillHolesDistanceAndFormat *= -1.0f;
//		voxelDataInfo[ 0 ] = new Vector4F( header.GridSize.ToVector3F(), fillHolesDistanceAndFormat );
//		voxelDataInfo[ 1 ] = new Vector4F( header.BoundsMin, header.CellSize );

//		//!!!!need?
//		var voxelDataOffsetInFloats = ( gpuHeaderSizeInBytes + header.GridSize.X * header.GridSize.Y * header.GridSize.Z * 4 ) / 4;

//		//!!!!особенности
//		voxelDataInfo[ 2 ] = new Vector4F( header.VoxelCount, voxelDataOffsetInFloats, 0, 0 );

//		var gpuHeaderData = CollectionUtility.ToByteArray( voxelDataInfo );
//		Array.Copy( gpuHeaderData, 0, gpuVoxelData, 0, gpuHeaderSizeInBytes );

//		Array.Copy( voxelData, sizeof( MeshGeometry.VoxelDataHeader ), gpuVoxelData, gpuHeaderSizeInBytes, sizeInBytes );

//		//write a list of voxel grid indexes. it is a grid index by voxel index dictionary
//		var gridIndexesOffset = gpuHeaderSizeInBytes + sizeInBytes;
//		fixed( byte* pGpuVoxelData = gpuVoxelData )
//		{
//			var pList = (float*)( pGpuVoxelData + gridIndexesOffset );

//			var writeOffset = 0;

//			for( int z = 0; z < header.GridSize.Z; z++ )
//			{
//				for( int y = 0; y < header.GridSize.Y; y++ )
//				{
//					for( int x = 0; x < header.GridSize.X; x++ )
//					{
//						var gridIndex = new Vector3I( x, y, z );

//						var arrayIndex = ( z * header.GridSize.Y + y ) * header.GridSize.X + x;

//						var voxelValue = *(float*)( pGpuVoxelData + gpuHeaderSizeInBytes + arrayIndex * 4 );
//						if( voxelValue > 0.0f )
//						{
//							var gridIndex256 = MeshConvertToVoxel.GetNearestVoxelWithDataIndex256( ref gridIndex );

//							pList[ writeOffset++ ] = gridIndex256;
//						}
//					}
//				}
//			}

//			//!!!!
//			if( writeOffset != header.VoxelCount )
//				Log.Fatal( "writeOffset != header.VoxelCount." );

//		}

//		return gpuVoxelData;
//	}

//	return null;
//}

//public class FullModeData
//{
//	//public byte[] GridData;

//	public Vector3 GridPosition;
//	public float CellSize;



//	//public OpenList<byte> objectTypes = new OpenList<byte>( 1024 * 1024 * 8 );

//	//public Dictionary<byte[], int> objectTypeOffsetByVoxelData = new Dictionary<byte[], int>( 512 );
//	////public Dictionary<MeshGeometry, int> objectTypeIndexVoxelData = new Dictionary<MeshGeometry, int>();
//	////public Dictionary<MeshGeometry, int> objectTypeIndexByMeshGeometry = new Dictionary<MeshGeometry, int>();
//	////var objectTypeIndexByVoxelData = new Dictionary<byte[], int>();

//	//public OpenList<GPUObjectData> objects = new OpenList<GPUObjectData>( 1024 );
//	//public OpenList<SpaceBounds> objectsLocalSpaceBounds = new OpenList<SpaceBounds>( 1024 );
//	//public OpenList<SpaceBounds> objectsWorldSpaceBounds = new OpenList<SpaceBounds>( 1024 );

//	//public OpenList<float> directRadiance = new OpenList<float>( 1024 * 1024 * 8 );
//	////!!!!
//	//public OpenList<float> radianceObjectIds = new OpenList<float>( 1024 * 1024 * 8 );
//	//public OpenList<float> radianceLocalVoxelIndices = new OpenList<float>( 1024 * 1024 * 8 );


//	////public OpenList<byte> directRadiance = new OpenList<byte>();

//	////!!!!
//	////Dictionary<Vector3F, int> objectIdByPosition = new Dictionary<Vector3F, int>( 256 );

//	////public class ObjectItem
//	////{
//	////	public Vector3F Position;
//	////}
//}

////!!!!temp
//static ImageComponent CreateVec4Texture( ViewportRenderingContext context, byte[] data )
//{
//	int textureSize = 1;
//	while( true )
//	{
//		//!!!!необязательно в степени 2

//		var bytes = textureSize * textureSize * 16;
//		if( bytes >= data.Length )
//			break;
//		textureSize *= 2;
//	}

//	if( textureSize <= RenderingSystem.Capabilities.MaxTextureSize )
//	{
//		var gpuData = new byte[ textureSize * textureSize * 16 ];
//		Array.Copy( data, 0, gpuData, 0, data.Length );

//		//var image = context.DynamicTexture_Alloc( ViewportRenderingContext.DynamicTextureType.DynamicTexture, ImageComponent.TypeEnum._2D, new Vector2I( textureSize, textureSize ), PixelFormat.Float32RGBA, 0, false );

//		var image = ComponentUtility.CreateComponent<ImageComponent>( null, true, false );
//		image.CreateType = ImageComponent.TypeEnum._2D;
//		image.CreateSize = new Vector2I( textureSize, textureSize );
//		image.CreateMipmaps = false;
//		image.CreateFormat = PixelFormat.Float32RGBA;
//		image.CreateUsage = ImageComponent.Usages.WriteOnly;
//		image.Enabled = true;

//		var gpuTexture = image.Result;
//		if( gpuTexture != null )
//			gpuTexture.SetData( new GpuTexture.SurfaceData[] { new GpuTexture.SurfaceData( gpuData ) } );

//		return image;
//	}

//	//!!!!
//	return null;
//}

////!!!!temp
//static ImageComponent CreateFloatTexture( ViewportRenderingContext context, byte[] data )
//{
//	int textureSize = 1;
//	while( true )
//	{
//		//!!!!необязательно в степени 2. выровнить только по 16 видимо

//		var bytes = textureSize * textureSize * 4;
//		if( bytes >= data.Length )
//			break;
//		textureSize *= 2;
//	}

//	if( textureSize <= RenderingSystem.Capabilities.MaxTextureSize )
//	{
//		var gpuData = new byte[ textureSize * textureSize * 4 ];
//		Array.Copy( data, 0, gpuData, 0, data.Length );

//		//var image = context.DynamicTexture_Alloc( ViewportRenderingContext.DynamicTextureType.DynamicTexture, ImageComponent.TypeEnum._2D, new Vector2I( textureSize, textureSize ), PixelFormat.Float32R, 0, false );

//		var image = ComponentUtility.CreateComponent<ImageComponent>( null, true, false );
//		image.CreateType = ImageComponent.TypeEnum._2D;
//		image.CreateSize = new Vector2I( textureSize, textureSize );
//		image.CreateMipmaps = false;
//		image.CreateFormat = PixelFormat.Float32R;
//		image.CreateUsage = ImageComponent.Usages.WriteOnly;
//		image.Enabled = true;

//		var gpuTexture = image.Result;
//		if( gpuTexture != null )
//			gpuTexture.SetData( new GpuTexture.SurfaceData[] { new GpuTexture.SurfaceData( gpuData ) } );

//		return image;
//	}

//	//!!!!
//	return null;
//}

//!!!!temp
//static ImageComponent CreateFloatTexture3D( ViewportRenderingContext context, byte[] data )
//{
//	zzzzz;

//	int textureSize = 1;
//	while( true )
//	{
//		//!!!!необязательно в степени 2. выровнить только по 16 видимо

//		var bytes = textureSize * textureSize * 4;
//		if( bytes >= data.Length )
//			break;
//		textureSize *= 2;
//	}

//	if( textureSize <= RenderingSystem.Capabilities.MaxTextureSize )
//	{
//		var gpuData = new byte[ textureSize * textureSize * 4 ];
//		Array.Copy( data, 0, gpuData, 0, data.Length );

//		//var image = context.DynamicTexture_Alloc( ViewportRenderingContext.DynamicTextureType.DynamicTexture, ImageComponent.TypeEnum._2D, new Vector2I( textureSize, textureSize ), PixelFormat.Float32R, 0, false );

//		var image = ComponentUtility.CreateComponent<ImageComponent>( null, true, false );
//		image.CreateType = ImageComponent.TypeEnum._2D;
//		image.CreateSize = new Vector2I( textureSize, textureSize );
//		image.CreateMipmaps = false;
//		image.CreateFormat = PixelFormat.Float32R;
//		image.CreateUsage = ImageComponent.Usages.WriteOnly;
//		image.Enabled = true;

//		var gpuTexture = image.Result;
//		if( gpuTexture != null )
//			gpuTexture.SetData( new GpuTexture.SurfaceData[] { new GpuTexture.SurfaceData( gpuData ) } );

//		return image;
//	}

//	//!!!!
//	return null;
//}

//		unsafe void CalculateData( ViewportRenderingContext context, FullModeData fullModeData/*, out ImageComponent objectsTexture, out ImageComponent objectTypesTexture*/ )
//		{
//			DestroyData();

//			//!!!!

//			//Log.Info( "Calculate data start" );


//			//!!!!
//			//if( sizeof( GPUObjectData ) != 256 + 16 )
//			//	Log.Fatal( "sizeof( GPUObjectData ) != 256 + 16." );
//			//if( sizeof( GPUObjectData ) != 256 )
//			//	Log.Fatal( "sizeof( GPUObjectData ) != 256." );
//			//if( sizeof( GPUObjectData ) != 48 + 192 )
//			//	Log.Fatal( "sizeof( GPUObjectData ) != 48 + 192." );
//			//if( sizeof( GPUObjectData ) != 48 )
//			//	Log.Fatal( "sizeof( GPUObjectData ) != 48." );


//#if ___
//			//!!!!или Vector4F индексирование
//			var objectTypes = new OpenList<byte>();

//			var objectTypeIndexByMeshGeometry = new Dictionary<MeshGeometry, int>();
//			//var objectTypeIndexByVoxelData = new Dictionary<byte[], int>();
//			var objects = new OpenList<GPUObjectData>();


//			//////calculate atlas of all voxel data
//			////{
//			////	//structure: raw list of gpu voxel data. header, data, header, data

//			////	//int objectTypeIndex; //it is index in the buffer
//			////	//может быть два разных индекса. номер типа и его положение в буфере
//			////	////int positionInBuffer

//			////	//voxel header/info
//			////	//voxel data

//			////	//var objectTypeCount = 0;

//			////}

//			//calculate objects data
//			{
//				//!!!!как будет устроен буфер с объектами?
//				//!!!!карта достижимостей

//				var scene = FindParent<Scene>();
//				if( scene != null )
//				{
//					foreach( var obj in scene.GetComponents<MeshInSpace>() )
//					{
//						var tr = obj.TransformV;

//						var geometry = GetVoxelMeshGeometry( obj );
//						if( geometry == null )
//							continue;

//						var voxelData = geometry.VoxelData.Value;
//						if( voxelData == null )
//							continue;

//						var gpuVoxelData = GetGpuVoxelData( voxelData );
//						if( gpuVoxelData != null )
//						{
//							if( !objectTypeIndexByMeshGeometry.TryGetValue( geometry/* gpuVoxelData*/, out var objectTypeIndex ) )
//							{
//								objectTypeIndex = objectTypes.Count;
//								objectTypes.AddRange( gpuVoxelData );

//								objectTypeIndexByMeshGeometry[ geometry/* gpuVoxelData*/ ] = objectTypeIndex;
//							}

//							var item = new GPUObjectData();

//							//!!!!double
//							item.Position = tr.Position.ToVector3F();
//							item.Rotation = tr.Rotation.ToQuaternionF();
//							item.Scale = tr.Scale.ToVector3F();
//							item.ObjectTypeIndex = objectTypeIndex;

//							objects.Add( ref item );
//						}
//					}
//				}
//			}


//			byte[] radianceData;

//			{
//				//radiance format
//				//int objectIndex
//				//voxel count * sizeof( float )

//				//!!!!byte

//				var sizeInBytes = 0;

//				for( int n = 0; n < objects.Count; n++ )
//				{
//					ref var obj = ref objects.Data[ n ];

//					var objectTypeIndex = (int)obj.ObjectTypeIndex;

//					fixed( byte* pAtlas = objectTypes.Data )
//					{
//						var gpuHeader = (Vector4F*)( pAtlas + objectTypeIndex );

//						var voxelDataCount = (int)gpuHeader[ 2 ].X;
//						sizeInBytes += voxelDataCount * 4;
//					}
//				}


//				radianceData = new byte[ sizeInBytes ];

//				//fill initial data
//				{

//					//!!!!

//				}

//			}
//#endif


//			//create gpu buffers


//			////!!!!
//			//if( !EngineApp._DebugCapsLock )
//			//{
//			//	objectsTexture = null;
//			//	objectTypesTexture = null;
//			//	return;
//			//}


//			//!!!!


//			//{
//			//	var data = CollectionUtility.ToByteArray( fullModeData.objects.ToArray() );
//			//	var textureSize = objectsTexture2.Result.ResultSize.X;

//			//	var gpuData = new byte[ textureSize * textureSize * 16 ];
//			//	Array.Copy( data, 0, gpuData, 0, data.Length );

//			//	var gpuTexture = objectsTexture2.Result;
//			//	if( gpuTexture != null )
//			//		gpuTexture.SetData( new GpuTexture.SurfaceData[] { new GpuTexture.SurfaceData( gpuData ) } );
//			//}

//			//{
//			//	var data = CollectionUtility.ToByteArray( fullModeData.objectTypes.ToArray() );
//			//	var textureSize = objectTypesTexture2.Result.ResultSize.X;

//			//	var gpuData = new byte[ textureSize * textureSize * 4 ];
//			//	Array.Copy( data, 0, gpuData, 0, data.Length );

//			//	var gpuTexture = objectTypesTexture2.Result;
//			//	if( gpuTexture != null )
//			//		gpuTexture.SetData( new GpuTexture.SurfaceData[] { new GpuTexture.SurfaceData( gpuData ) } );
//			//}

//			//{
//			//	var data = CollectionUtility.ToByteArray( fullModeData.directRadiance.ToArray() );
//			//	var textureSize = radianceTexture2.Result.ResultSize.X;

//			//	var gpuData = new byte[ textureSize * textureSize * 4 ];
//			//	Array.Copy( data, 0, gpuData, 0, data.Length );

//			//	var gpuTexture = radianceTexture2.Result;
//			//	if( gpuTexture != null )
//			//		gpuTexture.SetData( new GpuTexture.SurfaceData[] { new GpuTexture.SurfaceData( gpuData ) } );
//			//}



//			//{
//			//	var bytes = CollectionUtility.ToByteArray( fullModeData.objects.ToArray() );
//			//	objectsTexture = CreateVec4Texture( context, bytes );

//			//	//var objectsBytes = CollectionUtility.ToByteArray( objects.ToArray() );
//			//	//var vertexElements = new VertexElement[] { new VertexElement( 0, 0, VertexElementType.Float4, VertexElementSemantic.Position ) };
//			//	//var vertexDeclaration = vertexElements.CreateVertexDeclaration( 0 );
//			//	//objectsBuffer = GpuBufferManager.CreateVertexBuffer( objectsBytes, vertexDeclaration, GpuBufferFlags.ComputeRead );
//			//}

//			//{
//			//	var bytes = CollectionUtility.ToByteArray( fullModeData.objectTypes.ToArray() );
//			//	objectTypesTexture = CreateFloatTexture( context, bytes );
//			//}


//			//!!!!
//			//{
//			//	//!!!!Half или byte. где еще так

//			//	var bytes = CollectionUtility.ToByteArray( fullModeData.radianceData );
//			//	radianceTexture = CreateFloatTexture( bytes );
//			//}


//			//Log.Info( "Calculate data end" );
//		}

//void DestroyData()
//{
//	//!!!!

//	//objectsTexture?.Dispose();
//	//objectsTexture = null;

//	//objectTypesTexture?.Dispose();
//	//objectTypesTexture = null;

//	//radianceTexture?.Dispose();
//	//radianceTexture = null;

//	////objectsBuffer?.Dispose();
//	////objectsBuffer = null;
//}

//protected override void OnEnabledInHierarchyChanged()
//{
//	base.OnEnabledInHierarchyChanged();

//	if( !EnabledInHierarchy )
//		DestroyData();
//}

//string GetIndirectLightingDataKey()
//{
//	return "IndirectLightingData " + GetPathFromRoot();
//}

////!!!!temp
//internal static unsafe byte[] GenerateMip( byte[] gridData, int sourceSize )
//{
//	var resultSize = sourceSize / 2;

//	var result = new byte[ resultSize * resultSize * resultSize * 4 ];

//	fixed( byte* pResult2 = result )
//	{
//		float* pResult = (float*)pResult2;

//		fixed( byte* pGridData2 = gridData )
//		{
//			float* pSource = (float*)pGridData2;

//			for( int z = 0; z < sourceSize; z += 2 )
//			{
//				for( int y = 0; y < sourceSize; y += 2 )
//				{
//					for( int x = 0; x < sourceSize; x += 2 )
//					{
//						float v = 0;

//						v += pSource[ ( ( z + 0 ) * sourceSize + ( y + 0 ) ) * sourceSize + ( x + 0 ) ];
//						v += pSource[ ( ( z + 0 ) * sourceSize + ( y + 0 ) ) * sourceSize + ( x + 1 ) ];
//						v += pSource[ ( ( z + 0 ) * sourceSize + ( y + 1 ) ) * sourceSize + ( x + 0 ) ];
//						v += pSource[ ( ( z + 0 ) * sourceSize + ( y + 1 ) ) * sourceSize + ( x + 1 ) ];
//						v += pSource[ ( ( z + 1 ) * sourceSize + ( y + 0 ) ) * sourceSize + ( x + 0 ) ];
//						v += pSource[ ( ( z + 1 ) * sourceSize + ( y + 0 ) ) * sourceSize + ( x + 1 ) ];
//						v += pSource[ ( ( z + 1 ) * sourceSize + ( y + 1 ) ) * sourceSize + ( x + 0 ) ];
//						v += pSource[ ( ( z + 1 ) * sourceSize + ( y + 1 ) ) * sourceSize + ( x + 1 ) ];

//						v /= 8;

//						var xx = x / 2;
//						var yy = y / 2;
//						var zz = z / 2;
//						pResult[ ( zz * resultSize + yy ) * resultSize + xx ] = v;
//					}
//				}
//			}
//		}
//	}

//	return result;
//}

//internal unsafe void RenderFullTechnique( ViewportRenderingContext context, RenderingPipeline_Basic.FrameData frameData, ref ImageComponent deferredLightTexture, ImageComponent normalTexture, ImageComponent gBuffer2Texture, ImageComponent gBuffer3Texture, ImageComponent gBuffer4Texture, ImageComponent gBuffer5Texture, ImageComponent motionAndObjectIdTexture, ImageComponent depthTexture, FullModeData fullModeData )
//{
//	zzzzz;

//	if( motionAndObjectIdTexture == null || depthTexture == null || fullModeData == null )
//		return;
//	if( Intensity <= 0 )
//		return;
//	var multiplier = Multiplier * GlobalMultiplier;
//	if( multiplier <= 0 )
//		return;

//	var pipeline = context.RenderingPipeline;
//	if( !pipeline.GetUseMultiRenderTargets() )
//		return;


//	//get current viewport context data

//	//var anyDataKey = GetIndirectLightingDataKey();

//	//ViewportContextData viewportContextData = null;
//	//{
//	//	context.AnyDataAutoDispose.TryGetValue( anyDataKey, out var current );
//	//	viewportContextData = current as ViewportContextData;
//	//}

//	//var recreate = false;
//	//if( viewportContextData != null )
//	//{
//	//	//if( viewportContextData.ObjectsTextureCreatedCount != fullModeData.objects.Count ||
//	//	//	viewportContextData.ObjectTypesTextureCreatedCount != fullModeData.objectTypes.Count ||
//	//	//	viewportContextData.DirectRadianceTextureCreatedCount != fullModeData.directRadiance.Count )
//	//	//{
//	//	//	recreate = true;
//	//	//}
//	//}

//	////if( !EngineApp._DebugCapsLock )
//	////	recreate = true;


//	////create new
//	//if( viewportContextData == null || recreate )
//	//{


//	//	//!!!!impl partial dynamic update



//	//	//delete old
//	//	if( viewportContextData != null )
//	//	{
//	//		viewportContextData.Dispose();
//	//		viewportContextData = null;

//	//		context.AnyDataAutoDispose.Remove( anyDataKey );
//	//	}


//	//	//create context data
//	//	viewportContextData = new ViewportContextData();
//	//	context.AnyDataAutoDispose[ anyDataKey ] = viewportContextData;

//	//	//create and copy textures

//	//	{
//	//		var gridData = fullModeData.GridData;



//	//		//var gridData = new byte[ GridSize * GridSize * GridSize * 4 ];
//	//		//var gridData = new float[ GridSize * GridSize * GridSize ];

//	//		//var gpuData = new byte[ textureSize * textureSize * 4 ];
//	//		//Array.Copy( data, 0, gpuData, 0, data.Length );

//	//		//var image = context.DynamicTexture_Alloc( ViewportRenderingContext.DynamicTextureType.DynamicTexture, ImageComponent.TypeEnum._2D, new Vector2I( textureSize, textureSize ), PixelFormat.Float32R, 0, false );


//	//		var image = ComponentUtility.CreateComponent<ImageComponent>( null, true, false );
//	//		image.CreateType = ImageComponent.TypeEnum._3D;
//	//		image.CreateSize = new Vector2I( GridSize, GridSize );
//	//		image.CreateDepth = GridSize;
//	//		//!!!!
//	//		image.CreateMipmaps = true;
//	//		//image.CreateMipmaps = false;
//	//		image.CreateFormat = PixelFormat.Float32R;
//	//		//!!!!AutoMipmaps
//	//		image.CreateUsage = ImageComponent.Usages.WriteOnly;// | ImageComponent.Usages.AutoMipmaps;
//	//		image.Enabled = true;

//	//		var gpuTexture = image.Result;
//	//		if( gpuTexture != null )
//	//		{
//	//			var datas = new List<GpuTexture.SurfaceData>();
//	//			datas.Add( new GpuTexture.SurfaceData( gridData ) );

//	//			var current = gridData;
//	//			var currentSize = GridSize;

//	//			for( int gridSize = GridSize / 2; gridSize > 0; gridSize /= 2 )
//	//			{
//	//				var mip = GenerateMip( current, currentSize );

//	//				datas.Add( new GpuTexture.SurfaceData( mip, mipLevel: datas.Count ) );

//	//				current = mip;
//	//				currentSize = gridSize;
//	//			}

//	//			gpuTexture.SetData( datas.ToArray() );


//	//			//var datas = new List<GpuTexture.SurfaceData>();
//	//			//for( int z = 0; z < GridSize; z++ )
//	//			//{
//	//			//	var data = new GpuTexture.SurfaceData( new ArraySegment<byte>( gridData, GridSize * GridSize * 4 * z, GridSize * GridSize * 4 ) );
//	//			//	datas.Add( data );
//	//			//}
//	//			//gpuTexture.SetData( datas.ToArray() );

//	//			//gpuTexture.SetData( new GpuTexture.SurfaceData[] { new GpuTexture.SurfaceData( gpuData ) } );
//	//		}

//	//		viewportContextData.GridTexture = image;
//	//	}
//	//}


//	//common settings
//	var gridSize = GetGridSize();
//	var distance = (float)Distance.Value;
//	var gridSideSize = distance * 2;
//	fullModeData.CellSize = gridSideSize / gridSize;
//	fullModeData.GridPosition = context.Owner.CameraSettings.Position - new Vector3( distance, distance, distance );


//	//Atomic limitations
//	//There are some severe limitations on image atomic operations.First, atomics can only be used on integer images, either signed or unsigned.Second, they can only be used on images with the GL_R32I/ r32i or GL_R32UI/ r32ui formats.

//	//alloc grid texture
//	var gridTexture = context.DynamicTexture_Alloc( ViewportRenderingContext.DynamicTextureType.ComputeWrite, new Vector3I( gridSize, gridSize, gridSize ), PixelFormat.R32_UInt /*A8R8G8B8*/ /*Float32R*/, 0, true );
//	if( gridTexture == null )
//		return;


//	//!!!!TEMP чтобы обновить view number counter
//	//!!!!name
//	context.SetComputeView();
//	//context.SetViewport( actualTexture.Result.GetRenderTarget().Viewports[ 0 ] );


//	pipeline.ClearGIGrid( context, frameData, gridSize, gridTexture );


//	//!!!!
//	//pipeline.Render3DSceneGI( context, frameData, zz, zz );



//	//inject lights

//	//var deferredLightTexture = context.RenderTarget2D_Alloc( destinationSize, GetHighDynamicRange() ? PixelFormat.Float16RGBA : PixelFormat.A8R8G8B8 );

//	////lighting pass for deferred shading
//	//context.SetViewport( deferredLightTexture.Result.GetRenderTarget().Viewports[ 0 ], viewMatrix, projectionMatrix, FrameBufferTypes.Color, ColorValue.Zero );

//	//if( DebugDirectLighting )
//	//	RenderLightsDeferred( context, frameData, sceneTexture, normalTexture, gBuffer2Texture, gBuffer3Texture, gBuffer4Texture, gBuffer5Texture, depthTexture );

//	//if( DebugIndirectLighting )
//	//{
//	//	//!!!!только для deferred? если да, то как на forward влияет?
//	//	//indirect lighting full mode
//	//	indirectLightingFullMode?.RenderFullTechnique( context, frameData, ref deferredLightTexture, normalTexture, gBuffer2Texture, gBuffer3Texture, gBuffer4Texture, gBuffer5Texture, motionAndObjectIdTexture, depthTexture, indirectLightingFullModeData );
//	//}


//	//voxel cone tracing

//	//!!!!downscale, resolution
//	var lightingTextureSize = deferredLightTexture/*downscaledTexture*/.Result.ResultSize;// / (int)Resolution.Value;

//	var lightingTexture = context.RenderTarget2D_Alloc( lightingTextureSize, deferredLightTexture/*downscaledTexture*/.Result.ResultFormat );
//	{
//		context.SetViewport( lightingTexture.Result.GetRenderTarget().Viewports[ 0 ] );

//		var shader = new CanvasRenderer.ShaderItem();
//		shader.VertexProgramFileName = @"Base\Shaders\EffectsCommon_vs.sc";
//		shader.FragmentProgramFileName = @"Base\Shaders\Effects\IndirectLighting\Full_Lighting_fs.sc";

//		var gridTextureFilter = DebugShowVoxelGrid.Value ? FilterOption.Point : FilterOption.Linear;
//		shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 0, gridTexture, TextureAddressingMode.Clamp, gridTextureFilter, gridTextureFilter, FilterOption.Linear ) );

//		shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 1, normalTexture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.Point ) );

//		shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 2, gBuffer2Texture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.Point ) );

//		shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 3, gBuffer3Texture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.Point ) );

//		shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 4, gBuffer4Texture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.Point ) );

//		shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 5, gBuffer5Texture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.Point ) );

//		shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 6, depthTexture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.None ) );

//		shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 7, RenderingPipeline_Basic.BrdfLUT, TextureAddressingMode.Clamp, FilterOption.Linear, FilterOption.Linear, FilterOption.Linear ) );


//		//NativeMethods.bgfx_set_vertex_buffer( 0, objectsBuffer.NativeObjectHandle, 0, -1 );
//		////Bgfx.SetVertexBuffer( 0, objectsBuffer.NativeObjectHandle );


//		//!!!!impl Intensity
//		//shader.Parameters.Set( "intensity", (float)Intensity );

//		shader.Parameters.Set( "multiplier", (float)multiplier );

//		//!!!!
//		shader.Parameters.Set( "gridSize", (float)GridSize.Value );
//		//!!!!double
//		shader.Parameters.Set( "gridPosition", fullModeData.GridPosition.ToVector3F() );
//		shader.Parameters.Set( "cellSize", (float)fullModeData.CellSize );

//		shader.Parameters.Set( "debugShowVoxelGrid", DebugShowVoxelGrid.Value ? 1.0f : 0.0f );

//		//!!!!need?

//		//!!!!double
//		Matrix4F projectionMatrix = context.Owner.CameraSettings.ProjectionMatrix.ToMatrix4F();
//		Matrix4F viewMatrix = context.Owner.CameraSettings.ViewMatrix.ToMatrix4F();

//		Matrix4F viewProjMatrix = projectionMatrix * viewMatrix;
//		viewProjMatrix.GetInverse( out var invViewProjMatrix );

//		//!!!!double
//		shader.Parameters.Set( "cameraPosition", context.Owner.CameraSettings.Position.ToVector3F() );

//		shader.Parameters.Set( "viewProj", viewProjMatrix );
//		shader.Parameters.Set( "invViewProj", invViewProjMatrix );


//		context.RenderQuadToCurrentViewport( shader );
//	}


//	//!!!!bilateral. где еще
//	//blur
//	var blurTexture = pipeline.GaussianBlur( context, lightingTexture, BlurFactor, BlurDownscalingMode, BlurDownscalingValue );


//	//free lighting texture
//	if( lightingTexture != null )
//		context.DynamicTexture_Free( lightingTexture );


//	//write to deferredLightTexture with Add blending
//	{
//		context.SetViewport( deferredLightTexture.Result.GetRenderTarget().Viewports[ 0 ] );

//		var shader = new CanvasRenderer.ShaderItem();
//		shader.VertexProgramFileName = @"Base\Shaders\EffectsCommon_vs.sc";
//		shader.FragmentProgramFileName = @"Base\Shaders\Effects\IndirectLighting\Full_Final_fs.sc";

//		shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 0, blurTexture, TextureAddressingMode.Clamp, FilterOption.Linear, FilterOption.Linear, FilterOption.Point ) );

//		shader.Parameters.Set( "intensity", (float)Intensity );
//		//shader.Parameters.Set( "multiplier", (float)multiplier );

//		context.RenderQuadToCurrentViewport( shader, CanvasRenderer.BlendingType.Add );
//	}


//	//free targets
//	context.DynamicTexture_Free( blurTexture );
//	context.DynamicTexture_Free( gridTexture );


//	if( DebugShowGridBounds )
//	{
//		var renderer = context.Owner.Simple3DRenderer;

//		var sideSize = fullModeData.CellSize * gridSize;
//		var b = new Bounds( fullModeData.GridPosition, fullModeData.GridPosition + new Vector3( sideSize, sideSize, sideSize ) );
//		renderer.SetColor( new ColorValue( 1, 0, 0 ) );
//		renderer.AddBounds( b );

//		//for( int z = 0; z < GridSize; z++ )
//		//{
//		//	for( int y = 0; y < GridSize; y++ )
//		//	{
//		//		for( int x = 0; x < GridSize; x++ )
//		//		{
//		//			var gridIndex = new Vector3I( x, y, z );

//		//			var min = fullModeData.GridPosition + gridIndex.ToVector3F() * fullModeData.CellSize;
//		//			var b = new Bounds( min, min + new Vector3( fullModeData.CellSize, fullModeData.CellSize, fullModeData.CellSize ) );
//		//			renderer.SetColor( new ColorValue( 1, 0, 0, 0.1 ) );
//		//			renderer.AddBounds( b );
//		//		}
//		//	}
//		//}
//	}

//}



/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



//{
//	var data = CollectionUtility.ToByteArray( fullModeData.objects.ToArray() );
//	viewportContextData.ObjectsTextureCreatedCount = fullModeData.objects.Count;
//	viewportContextData.ObjectsTexture = CreateVec4Texture( context, data );
//}

//{
//	var data = CollectionUtility.ToByteArray( fullModeData.objectTypes.ToArray() );
//	viewportContextData.ObjectTypesTextureCreatedCount = fullModeData.objectTypes.Count;
//	viewportContextData.ObjectTypesTexture = CreateFloatTexture( context, data );
//}

//{
//	var data = CollectionUtility.ToByteArray( fullModeData.directRadiance.ToArray() );
//	viewportContextData.DirectRadianceTextureCreatedCount = fullModeData.directRadiance.Count;
//	viewportContextData.DirectRadianceTexture = CreateFloatTexture( context, data );
//}

//{
//	var data = CollectionUtility.ToByteArray( fullModeData.radianceObjectIds.ToArray() );
//	viewportContextData.RadianceObjectIdsTexture = CreateFloatTexture( context, data );
//}

//{
//	var data = CollectionUtility.ToByteArray( fullModeData.radianceLocalVoxelIndices.ToArray() );
//	viewportContextData.RadianceLocalVoxelIndicesTexture = CreateFloatTexture( context, data );
//}

//viewportContextData.AccumulationRadianceTexture = CreateAccumulationBuffer( viewportContextData.DirectRadianceTexture.Result.ResultSize, PixelFormat.Float32R );
////clear
//context.SetViewport( viewportContextData.AccumulationRadianceTexture.Result.GetRenderTarget().Viewports[ 0 ], Matrix4F.Identity, Matrix4F.Identity, FrameBufferTypes.Color, ColorValue.Zero );





////!!!!temp
////update direct radiance texture
//{
//	var data = CollectionUtility.ToByteArray( fullModeData.directRadiance.ToArray() );
//	var textureSize = viewportContextData.DirectRadianceTexture.Result.ResultSize.X;

//	var gpuData = new byte[ textureSize * textureSize * 4 ];
//	Array.Copy( data, 0, gpuData, 0, data.Length );

//	var gpuTexture = viewportContextData.DirectRadianceTexture.Result;
//	if( gpuTexture != null )
//		gpuTexture.SetData( new GpuTexture.SurfaceData[] { new GpuTexture.SurfaceData( gpuData ) } );
//}



//!!!!temp
//{
//	Log.Info( "-------" );

//	for( int objectId = 0; objectId < fullModeData.objects.Count; objectId++ )
//	{
//		ref var obj = ref fullModeData.objects.Data[ objectId ];

//		//if( objectId != 23 )
//		//	continue;
//		if( !obj.Scale.Equals( new Vector3F( 2, 2, 2 ), 0.01f ) )
//			continue;

//		Log.Info( "this: " + objectId.ToString() );

//		var localSpaceBounds = fullModeData.objectsLocalSpaceBounds[ objectId ];

//		var renderer = context.Owner.simple3DRenderer;

//		var tr = new Transform( obj.Position, obj.Rotation, obj.Scale );
//		var worldBounds = SpaceBounds.Multiply( tr, localSpaceBounds );

//		renderer.SetColor( new ColorValue( 1, 0, 0 ), false );
//		renderer.AddBounds( worldBounds.CalculatedBoundingBox, false, 0 );


//		for( int n = 0; n < 4; n++ )
//		{
//			var index = (int)obj.RayObjectsPlusZ0[ n ];
//			//var index = (int)obj.RayObjectsMinusY0[ n ];
//			if( index >= 0 )
//			{
//				Log.Info( index.ToString() );

//				//ref var obj2 = ref fullModeData.objects.Data[ index ];
//				var worldSpaceBounds = fullModeData.objectsWorldSpaceBounds[ index ];

//				renderer.SetColor( new ColorValue( 0, 1, 0 ), false );
//				renderer.AddBounds( worldSpaceBounds.CalculatedBoundingBox, false, 0 );
//			}
//		}

//		for( int n = 0; n < 4; n++ )
//		{
//			var index = (int)obj.RayObjectsPlusZ1[ n ];
//			//var index = (int)obj.RayObjectsMinusY1[ n ];
//			if( index >= 0 )
//			{
//				Log.Info( index.ToString() );

//				//ref var obj2 = ref fullModeData.objects.Data[ index ];
//				var worldSpaceBounds = fullModeData.objectsWorldSpaceBounds[ index ];

//				renderer.SetColor( new ColorValue( 0, 1, 0 ), false );
//				renderer.AddBounds( worldSpaceBounds.CalculatedBoundingBox, false, 0 );
//			}
//		}



//		//var localBounds = spaceBounds.CalculatedBoundingBox;

//		//localBounds.ToPoints(

//		//renderer.add

//		//renderer.AddBounds( spaceBounds.CalculatedBoundingBox, false, 0 );

//		//renderer.AddBounds( spaceBounds.CalculatedBoundingBox, true, 0 );
//	}
//}




//#if _______

////!!!!
//if( objectsTexture2 == null )
//{
//	var textureSize = 1024;

//	var image = ComponentUtility.CreateComponent<ImageComponent>( null, true, false );
//	image.CreateType = ImageComponent.TypeEnum._2D;
//	image.CreateSize = new Vector2I( textureSize, textureSize );
//	image.CreateMipmaps = false;
//	image.CreateFormat = PixelFormat.Float32RGBA;
//	image.CreateUsage = ImageComponent.Usages.WriteOnly;
//	image.Enabled = true;

//	objectsTexture2 = image;
//}

////!!!!
//if( objectTypesTexture2 == null )
//{
//	var textureSize = 2048;

//	var image = ComponentUtility.CreateComponent<ImageComponent>( null, true, false );
//	image.CreateType = ImageComponent.TypeEnum._2D;
//	image.CreateSize = new Vector2I( textureSize, textureSize );
//	image.CreateMipmaps = false;
//	image.CreateFormat = PixelFormat.Float32R;
//	image.CreateUsage = ImageComponent.Usages.WriteOnly;
//	image.Enabled = true;

//	objectTypesTexture2 = image;
//}

////!!!!
//if( radianceTexture2 == null )
//{
//	var textureSize = 2048;

//	var image = ComponentUtility.CreateComponent<ImageComponent>( null, true, false );
//	image.CreateType = ImageComponent.TypeEnum._2D;
//	image.CreateSize = new Vector2I( textureSize, textureSize );
//	image.CreateMipmaps = false;
//	image.CreateFormat = PixelFormat.Float32R;
//	image.CreateUsage = ImageComponent.Usages.WriteOnly;
//	image.Enabled = true;

//	radianceTexture2 = image;
//}

//#endif




//!!!!temp
//CalculateData( context, fullModeData );
////CalculateData( context, fullModeData, out var objectsTexture, out var objectTypesTexture );



//!!!!сбрасывать при резком перемещении камеры. может счетчик в viewport добавить чтобы не через эвент

////update accumulation buffer
//ImageComponent radianceAccumulationBuffer = null;
//{
//	var anyDataKey = "IndirectLighting " + GetPathFromRoot();

//	//!!!!
//	//!!!!downscaled?
//	var demandedSize = radianceTexture2.Result.ResultSize;
//	//var demandedSize = actualTexture/*downscaledTexture*/.Result.ResultSize;

//	//check to destroy
//	{
//		context.AnyDataAutoDispose.TryGetValue( anyDataKey, out var current );

//		if( current != null && current.Result.ResultSize != demandedSize )
//		{
//			//destroy
//			context.AnyDataAutoDispose.Remove( anyDataKey );
//			current?.Dispose();
//		}
//	}

//	//create and get
//	{
//		context.AnyDataAutoDispose.TryGetValue( anyDataKey, out var current );

//		if( current == null )
//		{
//			//create
//			//!!!!
//			current = CreateAccumulationBuffer( demandedSize, PixelFormat.Float32R );// actualTexture.Result.ResultFormat );
//			context.AnyDataAutoDispose[ anyDataKey ] = current;

//			zzzzzz;
//			//clear
//			context.SetViewport( current.Result.GetRenderTarget().Viewports[ 0 ], Matrix4F.Identity, Matrix4F.Identity, FrameBufferTypes.Color, ColorValue.Zero );
//		}

//		radianceAccumulationBuffer = current;
//	}
//}



//!!!!swap не юзать когда не нужно. когда четное


//!!!!unique frame counter maybe
//var random = new FastRandom();


//!!!!
//var image = context.DynamicTexture_Alloc( ViewportRenderingContext.DynamicTextureType.DynamicTexture, ImageComponent.TypeEnum._3D, new Vector2I( textureSize, textureSize ), PixelFormat.Float32R, 0, false );




////calculate a new radiance buffer
//ImageComponent newRadianceBuffer = null;
//{
//	zzz;
//	newRadianceBuffer = context.RenderTarget2D_Alloc( viewportContextData.AccumulationRadianceTexture.Result.ResultSize, viewportContextData.AccumulationRadianceTexture.Result.ResultFormat );

//	context.SetViewport( newRadianceBuffer.Result.GetRenderTarget().Viewports[ 0 ] );

//	var shader = new CanvasRenderer.ShaderItem();
//	shader.VertexProgramFileName = @"Base\Shaders\EffectsCommon_vs.sc";
//	shader.FragmentProgramFileName = @"Base\Shaders\Effects\IndirectLighting\Full_CalculateRadiance_fs.sc";


//	shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 0, viewportContextData.ObjectsTexture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.None ) );

//	shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 1, viewportContextData.ObjectTypesTexture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.None ) );

//	shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 2, viewportContextData.DirectRadianceTexture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.None ) );

//	shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 3, viewportContextData.AccumulationRadianceTexture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.None ) );

//	shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 4, viewportContextData.RadianceObjectIdsTexture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.None ) );

//	shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 5, viewportContextData.RadianceLocalVoxelIndicesTexture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.None ) );


//	//shader.Parameters.Set( "intensity", (float)Intensity );
//	//shader.Parameters.Set( "multiplier", (float)multiplier );

//	var voxelCount = fullModeData.directRadiance.Count;
//	shader.Parameters.Set( "globalVoxelCount", (float)voxelCount );

//	//!!!!
//	var randomSeeds = new Vector4F( random.NextFloat(), random.NextFloat(), random.NextFloat(), random.NextFloat() );
//	shader.Parameters.Set( "randomSeeds", randomSeeds );

//	////!!!!double
//	//Matrix4F projectionMatrix = context.Owner.CameraSettings.ProjectionMatrix.ToMatrix4F();
//	//Matrix4F viewMatrix = context.Owner.CameraSettings.ViewMatrix.ToMatrix4F();

//	//Matrix4F viewProjMatrix = projectionMatrix * viewMatrix;
//	//viewProjMatrix.GetInverse( out var invViewProjMatrix );

//	//shader.Parameters.Set( "viewProj", viewProjMatrix );
//	//shader.Parameters.Set( "invViewProj", invViewProjMatrix );


//	context.RenderQuadToCurrentViewport( shader );
//}


////!!!!swap or calculate two steps each update
////copy back to radianceAccumulationBuffer
//context.SetViewport( viewportContextData.AccumulationRadianceTexture.Result.GetRenderTarget().Viewports[ 0 ] );
//pipeline.CopyToCurrentViewport( context, newRadianceBuffer );


////free newRadianceBuffer
//context.DynamicTexture_Free( newRadianceBuffer );






//var viewportOwner = context.Owner;


//for( int step = 0; step < RadianceSteps.Value; step++ )
//{
//	//calculate a new radiance buffer
//	ImageComponent newRadianceBuffer = null;
//	{
//		newRadianceBuffer = context.RenderTarget2D_Alloc( viewportContextData.AccumulationRadianceTexture.Result.ResultSize, viewportContextData.AccumulationRadianceTexture.Result.ResultFormat );

//		context.SetViewport( newRadianceBuffer.Result.GetRenderTarget().Viewports[ 0 ] );

//		var shader = new CanvasRenderer.ShaderItem();
//		shader.VertexProgramFileName = @"Base\Shaders\EffectsCommon_vs.sc";
//		shader.FragmentProgramFileName = @"Base\Shaders\Effects\IndirectLighting\Full_CalculateRadiance_fs.sc";

//		shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 0, viewportContextData.ObjectsTexture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.None ) );

//		shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 1, viewportContextData.ObjectTypesTexture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.None ) );

//		shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 2, viewportContextData.DirectRadianceTexture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.None ) );

//		shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 3, viewportContextData.AccumulationRadianceTexture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.None ) );

//		shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 4, viewportContextData.RadianceObjectIdsTexture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.None ) );

//		shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 5, viewportContextData.RadianceLocalVoxelIndicesTexture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.None ) );


//		//shader.Parameters.Set( "intensity", (float)Intensity );
//		//shader.Parameters.Set( "multiplier", (float)multiplier );

//		var voxelCount = fullModeData.directRadiance.Count;
//		shader.Parameters.Set( "globalVoxelCount", (float)voxelCount );

//		//!!!!
//		var randomSeeds = new Vector4F( random.NextFloat(), random.NextFloat(), random.NextFloat(), random.NextFloat() );
//		shader.Parameters.Set( "randomSeeds", randomSeeds );

//		////!!!!double
//		//Matrix4F projectionMatrix = context.Owner.CameraSettings.ProjectionMatrix.ToMatrix4F();
//		//Matrix4F viewMatrix = context.Owner.CameraSettings.ViewMatrix.ToMatrix4F();

//		//Matrix4F viewProjMatrix = projectionMatrix * viewMatrix;
//		//viewProjMatrix.GetInverse( out var invViewProjMatrix );

//		//shader.Parameters.Set( "viewProj", viewProjMatrix );
//		//shader.Parameters.Set( "invViewProj", invViewProjMatrix );


//		context.RenderQuadToCurrentViewport( shader );
//	}


//	//!!!!swap or calculate two steps each update
//	//copy back to radianceAccumulationBuffer
//	context.SetViewport( viewportContextData.AccumulationRadianceTexture.Result.GetRenderTarget().Viewports[ 0 ] );
//	pipeline.CopyToCurrentViewport( context, newRadianceBuffer );


//	//free newRadianceBuffer
//	context.DynamicTexture_Free( newRadianceBuffer );
//}


//var viewportOwner = context.Owner;


////!!!!downscale, resolution
//var lightingTextureSize = deferredLightTexture/*downscaledTexture*/.Result.ResultSize;// / (int)Resolution.Value;

//var lightingTexture = context.RenderTarget2D_Alloc( lightingTextureSize, deferredLightTexture/*downscaledTexture*/.Result.ResultFormat );
//{
//	context.SetViewport( lightingTexture.Result.GetRenderTarget().Viewports[ 0 ] );

//	var shader = new CanvasRenderer.ShaderItem();
//	shader.VertexProgramFileName = @"Base\Shaders\EffectsCommon_vs.sc";
//	shader.FragmentProgramFileName = @"Base\Shaders\Effects\IndirectLighting\Full_Lighting_fs.sc";


//	shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 0, viewportContextData.ObjectsTexture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.None ) );

//	shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 1, viewportContextData.ObjectTypesTexture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.None ) );

//	shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 2, viewportContextData.DirectRadianceTexture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.None ) );

//	shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 3, viewportContextData.AccumulationRadianceTexture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.None ) );

//	shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 4, motionAndObjectIdTexture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.None ) );

//	shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 5, depthTexture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.None ) );

//	//shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 5, viewportContextData.RadianceLocalVoxelIndicesTexture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.None ) );


//	//NativeMethods.bgfx_set_vertex_buffer( 0, objectsBuffer.NativeObjectHandle, 0, -1 );
//	////Bgfx.SetVertexBuffer( 0, objectsBuffer.NativeObjectHandle );

//	//public GpuVertexBuffer objectsBuffer;


//	//shader.Parameters.Set( "intensity", (float)Intensity );
//	shader.Parameters.Set( "multiplier", (float)multiplier );

//	//!!!!need?

//	//!!!!double
//	Matrix4F projectionMatrix = context.Owner.CameraSettings.ProjectionMatrix.ToMatrix4F();
//	Matrix4F viewMatrix = context.Owner.CameraSettings.ViewMatrix.ToMatrix4F();

//	Matrix4F viewProjMatrix = projectionMatrix * viewMatrix;
//	viewProjMatrix.GetInverse( out var invViewProjMatrix );

//	//!!!!double
//	shader.Parameters.Set( "cameraPosition", context.Owner.CameraSettings.Position.ToVector3F() );

//	shader.Parameters.Set( "viewProj", viewProjMatrix );
//	shader.Parameters.Set( "invViewProj", invViewProjMatrix );


//	context.RenderQuadToCurrentViewport( shader );
//}


////!!!!bilateral. где еще
////blur
//var blurTexture = pipeline.GaussianBlur( context, lightingTexture, BlurFactor, BlurDownscalingMode, BlurDownscalingValue );

////free lighting texture
//if( lightingTexture != null )
//	context.DynamicTexture_Free( lightingTexture );


////write to deferredLightTexture with Add blending
//{
//	context.SetViewport( deferredLightTexture.Result.GetRenderTarget().Viewports[ 0 ] );

//	var shader = new CanvasRenderer.ShaderItem();
//	shader.VertexProgramFileName = @"Base\Shaders\EffectsCommon_vs.sc";
//	shader.FragmentProgramFileName = @"Base\Shaders\Effects\IndirectLighting\Full_Final_fs.sc";

//	shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 0, blurTexture, TextureAddressingMode.Clamp, FilterOption.Linear, FilterOption.Linear, FilterOption.Point ) );

//	shader.Parameters.Set( "intensity", (float)Intensity );
//	//shader.Parameters.Set( "multiplier", (float)multiplier );

//	context.RenderQuadToCurrentViewport( shader, CanvasRenderer.BlendingType.Add );
//}


////free targets
//context.DynamicTexture_Free( blurTexture );
////!!!!какие еще?






//var lightingTextureSize = actualTexture/*downscaledTexture*/.Result.ResultSize;// / (int)Resolution.Value;

//var lightingTexture = context.RenderTarget2D_Alloc( lightingTextureSize, actualTexture/*downscaledTexture*/.Result.ResultFormat );
//{
//	context.SetViewport( lightingTexture.Result.GetRenderTarget().Viewports[ 0 ] );

//	CanvasRenderer.ShaderItem shader = new CanvasRenderer.ShaderItem();
//	shader.VertexProgramFileName = @"Base\Shaders\EffectsCommon_vs.sc";
//	shader.FragmentProgramFileName = @"Base\Shaders\Effects\IndirectLighting\FullLighting_fs.sc";


//	context.ObjectsDuringUpdate.namedTextures.TryGetValue( "depthTexture", out var depthTexture );
//	context.ObjectsDuringUpdate.namedTextures.TryGetValue( "normalTexture", out var normalTexture );

//	shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 0, actualTexture/*downscaledTexture*/, TextureAddressingMode.Clamp, FilterOption.Linear, FilterOption.Linear, FilterOption.None ) );
//	shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 1, depthTexture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.None ) );
//	shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 2, normalTexture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.None ) );
//	////shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 3, noiseTexture, TextureAddressingMode.Wrap, FilterOption.Point, FilterOption.Point, FilterOption.Point ) );
//	////shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 4, accumulationBuffer ?? ResourceUtility.BlackTexture2D, TextureAddressingMode.Wrap, FilterOption.Point, FilterOption.Point, FilterOption.Point ) );


//	////float aspectRatio = (float)context.Owner.CameraSettings.AspectRatio;
//	////float fov = (float)context.Owner.CameraSettings.FieldOfView;
//	////float zNear = (float)context.Owner.CameraSettings.NearClipDistance;
//	////float zFar = (float)context.Owner.CameraSettings.FarClipDistance;

//	////shader.Parameters.Set( "colorTextureSize", new Vector4F( (float)lightingTexture.Result.ResultSize.X, (float)lightingTexture.Result.ResultSize.Y, 0.0f, 0.0f ) );
//	////shader.Parameters.Set( "zNear", zNear );
//	////shader.Parameters.Set( "zFar", zFar );
//	////shader.Parameters.Set( "fov", fov );
//	////shader.Parameters.Set( "aspectRatio", aspectRatio );

//	////{
//	////	var size = lightingTexture.Result.ResultSize;
//	////	shader.Parameters.Set( "viewportSize", new Vector4( size.X, size.Y, 1.0 / (double)size.X, 1.0 / (double)size.Y ).ToVector4F() );
//	////}

//	////{
//	////	var size = noiseTexture.Result.ResultSize;
//	////	shader.Parameters.Set( "noiseTextureSize", new Vector4( size.X, size.Y, 1.0 / (double)size.X, 1.0 / (double)size.Y ).ToVector4F() );
//	////}




//	//int rotationCount = 3;
//	//int stepCount = 4;
//	//switch( Quality.Value )
//	//{
//	//case QualityEnum.Low: rotationCount = 2; stepCount = 3; break;
//	//case QualityEnum.Medium: rotationCount = 3; stepCount = 6; break;
//	//case QualityEnum.High: rotationCount = 4; stepCount = 9; break;
//	//}

//	//shader.Defines.Add( new CanvasRenderer.ShaderItem.DefineItem( "ROTATION_COUNT", rotationCount.ToString() ) );
//	//shader.Defines.Add( new CanvasRenderer.ShaderItem.DefineItem( "STEP_COUNT", stepCount.ToString() ) );


//	//shader.Parameters.Set( "resolution", (float)Resolution.Value );
//	//shader.Parameters.Set( "skyLighting", (float)SkyLighting.Value );

//	//shader.Parameters.Set( "radius", (float)Radius.Value );
//	//shader.Parameters.Set( "expStart", (float)ExpStart.Value );
//	//shader.Parameters.Set( "expFactor", (float)ExpFactor.Value );
//	////shader.Parameters.Set( "jitterSamples", (float)( JitterSamples.Value ? 1.0f : 0.0f ) );

//	////shader.Parameters.Set( "lnDlOffset", (float)LnDlOffset.Value );
//	////shader.Parameters.Set( "nDlOffset", (float)NDlOffset.Value );

//	//shader.Parameters.Set( "thickness", (float)Thickness.Value );
//	//shader.Parameters.Set( "falloff", (float)Falloff.Value );

//	//shader.Parameters.Set( "reduction", (float)Reduction.Value );

//	////!!!!double
//	//Matrix4F itViewMatrix = ( context.Owner.CameraSettings.ViewMatrix.GetInverse().ToMatrix4F() ).GetTranspose();
//	//shader.Parameters.Set( "itViewMatrix", itViewMatrix );

//	////Vector4F seeds = Vector4F.Zero;
//	////if( AccumulateFrames.Value != 0 )
//	////{
//	////	var random = new Random();
//	////	seeds = new Vector4F( random.NextFloat(), random.NextFloat(), random.NextFloat(), random.NextFloat() );
//	////}
//	////shader.Parameters.Set( "randomSeeds", seeds );

//	////shader.Parameters.Set( "accumulateFrames", (float)AccumulateFrames.Value );

//	////!!!!double
//	//Matrix4F projectionMatrix = context.Owner.CameraSettings.ProjectionMatrix.ToMatrix4F();
//	//Matrix4F invProjectionMatrix = projectionMatrix.GetInverse();
//	////Matrix4F viewMatrix = context.Owner.CameraSettings.ViewMatrix.ToMatrix4F();
//	////Matrix4F viewProjMatrix = projectionMatrix * viewMatrix;
//	////Matrix4F invViewProjMatrix = viewProjMatrix.GetInverse();

//	////shader.Parameters.Set( "viewProj", viewProjMatrix );
//	////shader.Parameters.Set( "invViewProj", invViewProjMatrix );
//	//shader.Parameters.Set( "invProj", invProjectionMatrix );

//	////!!!!double
//	//Vector3F cameraPosition = context.Owner.CameraSettings.Position.ToVector3F();
//	//shader.Parameters.Set( "cameraPosition", cameraPosition );

//	//var fov = (double)context.Owner.CameraSettings.FieldOfView.InRadians();
//	//double halfProjScale = (double)context.Owner.SizeInPixels.Y / ( Math.Tan( fov * 0.5f ) * 2 ) * 0.5f;
//	//shader.Parameters.Set( "halfProjScale", (float)halfProjScale );




//	//if( accumulationBuffer != null )
//	//	context.RenderQuadToCurrentViewport( shader, CanvasRenderer.BlendingType.Add );
//	//else
//	context.RenderQuadToCurrentViewport( shader );
//}

////////copy to accumulation buffer
//////if( accumulationBuffer != null )
//////{
//////	context.SetViewport( accumulationBuffer.Result.GetRenderTarget().Viewports[ 0 ] );
//////	( (RenderingPipeline_Basic)context.renderingPipeline ).CopyToCurrentViewport( context, lightingTexture );
//////}

//////if( downscaledTexture != actualTexture )
//////	context.DynamicTexture_Free( downscaledTexture );

////blur
//var blurTexture = pipeline.GaussianBlur( context, /* accumulationBuffer ?? */lightingTexture, BlurFactor, BlurDownscalingMode, BlurDownscalingValue );

//if( lightingTexture != null )
//	context.DynamicTexture_Free( lightingTexture );
//////if( lightingNoAccumulationTexture != null )
//////	context.DynamicTexture_Free( lightingNoAccumulationTexture );

////final pass
//var finalTexture = context.RenderTarget2D_Alloc( actualTexture.Result.ResultSize, actualTexture.Result.ResultFormat );
//{
//	context.SetViewport( finalTexture.Result.GetRenderTarget().Viewports[ 0 ] );

//	CanvasRenderer.ShaderItem shader = new CanvasRenderer.ShaderItem();
//	shader.VertexProgramFileName = @"Base\Shaders\EffectsCommon_vs.sc";
//	shader.FragmentProgramFileName = @"Base\Shaders\Effects\IndirectLighting\FullFinal_fs.sc";

//	shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 0, actualTexture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.None ) );
//	shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 1, blurTexture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.None ) );

//	shader.Parameters.Set( "intensity", (float)Intensity );
//	shader.Parameters.Set( "showIndirectLighting", ShowIndirectLighting.Value ? 1.0f : 0.0f );
//	shader.Parameters.Set( "multiplier", (float)multiplier );// * (float)Resolution.Value );

//	context.RenderQuadToCurrentViewport( shader );
//}

////free targets
//context.DynamicTexture_Free( blurTexture );
//context.DynamicTexture_Free( actualTexture );
////!!!!какие еще?

////update actual texture
//actualTexture = finalTexture;






////write to deferredLightTexture with Add blending
//{
//	context.SetViewport( deferredLightTexture.Result.GetRenderTarget().Viewports[ 0 ] );

//	////!!!!double
//	//var projectionMatrix = context.Owner.CameraSettings.ProjectionMatrix.ToMatrix4F();
//	//var viewMatrix = context.Owner.CameraSettings.ViewMatrix.ToMatrix4F();

//	//context.SetViewport( deferredLightTexture.Result.GetRenderTarget().Viewports[ 0 ], viewMatrix, projectionMatrix, 0 /*FrameBufferTypes.Color*/, ColorValue.Zero );


//	////generate compile arguments
//	//var vertexDefines = new List<(string, string)>();
//	//var fragmentDefines = new List<(string, string)>();
//	//{
//	//	var generalDefines = new List<(string, string)>();
//	//	//generalDefines.Add( ("LIGHT_TYPE_" + lightType.ToString().ToUpper(), "") );

//	//	vertexDefines.AddRange( generalDefines );
//	//	fragmentDefines.AddRange( generalDefines );
//	//}

//	//string error2;


//	////vertex program
//	//var vertexProgram = GpuProgramManager.GetProgram( "DeferredDirectLight_",
//	//	GpuProgramType.Vertex, @"Base\Shaders\Effects\IndirectLighting\DeferredDirectLight_vs.sc", vertexDefines, true, out error2 );
//	//if( !string.IsNullOrEmpty( error2 ) )
//	//{
//	//	Log.Fatal( error2 );
//	//	return;
//	//}

//	////fragment program
//	//var fragmentProgram = GpuProgramManager.GetProgram( "IndirectLightingFinal_",
//	//	GpuProgramType.Fragment, @"Base\Shaders\Effects\IndirectLighting\Full_Final_fs.sc", fragmentDefines, true, out error2 );
//	//if( !string.IsNullOrEmpty( error2 ) )
//	//{
//	//	Log.Fatal( error2 );
//	//	return;
//	//}


//	{
//		var shader = new CanvasRenderer.ShaderItem();
//		shader.VertexProgramFileName = @"Base\Shaders\EffectsCommon_vs.sc";
//		shader.FragmentProgramFileName = @"Base\Shaders\Effects\IndirectLighting\Full_Final_fs.sc";

//		//shader.CompiledVertexProgram = vertexProgram;
//		//shader.CompiledFragmentProgram = fragmentProgram;


//		//!!!!
//		//objectsTexture2 = ResourceUtility.BlackTexture2D;
//		//objectTypesTexture2 = ResourceUtility.BlackTexture2D;



//		shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 0, objectsTexture2, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.None ) );

//		shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 1, objectTypesTexture2, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.None ) );

//		shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 2, radianceTexture2, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.None ) );

//		shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 3, motionAndObjectIdTexture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.None ) );

//		shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 4, depthTexture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.None ) );


//		//NativeMethods.bgfx_set_vertex_buffer( 0, objectsBuffer.NativeObjectHandle, 0, -1 );
//		////Bgfx.SetVertexBuffer( 0, objectsBuffer.NativeObjectHandle );


//		shader.Parameters.Set( "intensity", (float)Intensity );
//		shader.Parameters.Set( "multiplier", (float)multiplier );

//		//!!!!need?

//		//!!!!double
//		Matrix4F projectionMatrix = context.Owner.CameraSettings.ProjectionMatrix.ToMatrix4F();
//		Matrix4F viewMatrix = context.Owner.CameraSettings.ViewMatrix.ToMatrix4F();

//		Matrix4F viewProjMatrix = projectionMatrix * viewMatrix;
//		viewProjMatrix.GetInverse( out var invViewProjMatrix );

//		shader.Parameters.Set( "viewProj", viewProjMatrix );
//		shader.Parameters.Set( "invViewProj", invViewProjMatrix );


//		//shader.AdditionalParameterContainers.Add( generalContainer );

//		context.RenderQuadToCurrentViewport( shader, CanvasRenderer.BlendingType.Add );
//	}
//}

