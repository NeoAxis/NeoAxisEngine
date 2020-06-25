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
	/// <summary>
	/// A definition of surface type which contains material, set of meshes and other objects. Surfaces are used for painting and object creation by means brush.
	/// </summary>
	[ResourceFileExtension( "surface" )]
	[EditorDocumentWindow( typeof( Component_Surface_DocumentWindow ) )]
	[EditorPreviewControl( typeof( Component_Surface_PreviewControl ) )]
	public class Component_Surface : Component
	{
		//[DefaultValue( null )]
		//public Reference<Component_Material> Material
		//{
		//	get { if( _material.BeginGet() ) Material = _material.Get( this ); return _material.value; }
		//	set { if( _material.BeginSet( ref value ) ) { try { MaterialChanged?.Invoke( this ); } finally { _material.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="Material"/> property value changes.</summary>
		//public event Action<Component_Surface> MaterialChanged;
		//ReferenceField<Component_Material> _material = null;

		////!!!!name
		//[DefaultValue( "1 1" )]
		//[DisplayName( "Material UV Tiles Per Unit" )]
		//public Reference<Vector2> MaterialUVTilesPerUnit
		//{
		//	get { if( _materialUVTilesPerUnit.BeginGet() ) MaterialUVTilesPerUnit = _materialUVTilesPerUnit.Get( this ); return _materialUVTilesPerUnit.value; }
		//	set { if( _materialUVTilesPerUnit.BeginSet( ref value ) ) { try { MaterialUVTilesPerUnitChanged?.Invoke( this ); } finally { _materialUVTilesPerUnit.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="MaterialUVTilesPerUnit"/> property value changes.</summary>
		//public event Action<Component_Surface> MaterialUVTilesPerUnitChanged;
		//ReferenceField<Vector2> _materialUVTilesPerUnit = new Vector2( 1, 1 );

		//!!!!painting mask cell size

		/////////////////////////////////////////

		static int GetRandomIndex( Random random, double[] probabilities )
		{
			return RandomUtility.GetRandomIndexByProbabilities( random, probabilities );

			//int result = 0;

			//double total = 0;
			//foreach( var p in probabilities )
			//	total += p;

			//var v = random.Next( total );

			//double previous = 0;
			//double current = 0;
			//for( int n = 0; n < probabilities.Length; n++ )
			//{
			//	current += probabilities[ n ];

			//	if( v >= previous && v < current && probabilities[ n ] > 0 )
			//	{
			//		result = n;
			//		break;
			//	}

			//	previous = current;
			//}

			//if( result >= probabilities.Length )
			//	result = probabilities.Length - 1;
			//if( result < 0 )
			//	result = 0;
			//return result;
		}

		public struct GetRandomVariationOptions
		{
			public byte? SetGroup;

			public GetRandomVariationOptions( byte? setGroup )
			{
				SetGroup = setGroup;
			}
		}

		public delegate void GetRandomVariationEventDelegate( Component_Surface sender, GetRandomVariationOptions options, ref bool handled, ref byte groupIndex, ref byte elementIndex );
		public event GetRandomVariationEventDelegate GetRandomVariationEvent;

		public virtual void GetRandomVariation( GetRandomVariationOptions options, Random random, out byte groupIndex, out byte elementIndex, out double positionZ, out QuaternionF rotation, out Vector3F scale )
		{
			//!!!!CompiledData?

			if( options.SetGroup.HasValue )
				groupIndex = options.SetGroup.Value;
			else
				groupIndex = 0;
			elementIndex = 0;
			positionZ = 0;
			rotation = QuaternionF.Identity;
			scale = Vector3F.One;

			bool handled = false;
			GetRandomVariationEvent?.Invoke( this, options, ref handled, ref groupIndex, ref elementIndex );
			if( handled )
				return;

			var groups = GetComponents<Component_SurfaceGroupOfElements>();
			if( groups.Length != 0 )
			{
				if( !options.SetGroup.HasValue )
				{
					var groupProbabilities = new double[ groups.Length ];
					for( int n = 0; n < groupProbabilities.Length; n++ )
					{
						if( groups[ n ].Enabled )
							groupProbabilities[ n ] = groups[ n ].Probability;
					}
					groupIndex = (byte)GetRandomIndex( random, groupProbabilities );
				}

				if( groupIndex < groups.Length )
				{
					var group = groups[ groupIndex ];

					var groupElements = group.GetComponents<Component_SurfaceElement>();
					if( groupElements.Length != 0 )
					{
						var elementProbabilities = new double[ groupElements.Length ];
						for( int n = 0; n < elementProbabilities.Length; n++ )
						{
							if( groupElements[ n ].Enabled )
								elementProbabilities[ n ] = groupElements[ n ].Probability;
						}
						elementIndex = (byte)GetRandomIndex( random, elementProbabilities );
					}

					//PositionZRange
					var positionZRange = group.PositionZRange.Value;
					if( positionZRange.Minimum != positionZRange.Maximum )
						positionZ = random.Next( positionZRange.Minimum, positionZRange.Maximum );
					else
						positionZ = positionZRange.Minimum;

					//RotateAroundItsAxis
					if( group.RotateAroundItsAxis )
						rotation *= Quaternion.FromRotateByZ( random.Next( 0, MathEx.PI * 2 ) ).ToQuaternionF();
					//MaxIncline
					if( group.MaxIncline.Value != 0 )
					{
						var incline = random.Next( group.MaxIncline.Value.InRadians() );
						rotation *= QuaternionF.FromRotateByX( (float)incline );
					}

					//ScaleRange
					var scaleRange = group.ScaleRange.Value;
					float scaleV;
					if( scaleRange.Minimum != scaleRange.Maximum )
						scaleV = (float)random.Next( scaleRange.Minimum, scaleRange.Maximum );
					else
						scaleV = (float)scaleRange.Minimum;
					scale = new Vector3F( scaleV, scaleV, scaleV );
				}
			}
		}

		public Component_SurfaceGroupOfElements GetGroup( int index )
		{
			//!!!!slowly?

			var groups = GetComponents<Component_SurfaceGroupOfElements>();
			if( index >= 0 && index < groups.Length )
				return groups[ index ];
			return null;
		}

		public override void NewObjectSetDefaultConfiguration( bool createdFromNewObjectWindow = false )
		{
			var group = CreateComponent<Component_SurfaceGroupOfElements>();
			group.Name = "Group 1";

			var element = group.CreateComponent<Component_SurfaceElement_Mesh>();
			element.Name = "Mesh 1";
		}

		public List<Component_Mesh> GetAllMeshes()
		{
			//!!!!slowly?

			var result = new List<Component_Mesh>();
			foreach( var element in GetComponents<Component_SurfaceElement_Mesh>( checkChildren: true ) )
			{
				var mesh = element.Mesh.Value;
				if( mesh != null )
					result.Add( mesh );
			}
			return result;
		}
	}
}
