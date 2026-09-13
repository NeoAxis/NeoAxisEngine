// Copyright 2006–2026 Ivan Efimov. All rights reserved.
using System;
using System.ComponentModel;
using System.IO;
using System.Collections.Generic;

namespace NeoAxis
{
	static class PointCloudImport
	{
		public class InputData
		{
			public int PointLimit;
			public string SourceRealFileName;
		}

		public class ResultData
		{
			public Vector3F[] Positions;
			public ColorByte[] Colors;
		}

		public static ResultData Load( InputData inputData, out string error )
		{
			if( !File.Exists( inputData.SourceRealFileName ) )
			{
				error = "File not found.";
				return null;
			}

			var extension = Path.GetExtension( inputData.SourceRealFileName ).ToLower();
			if( extension == ".xyz" || extension == ".txt" )
				return Load_XYZ( inputData, out error );
			if( extension == ".pts" )
				return Load_PTS( inputData, out error );

			error = "Unsupported file format.";
			return null;
		}

		public static ResultData Load_XYZ( InputData inputData, out string error )
		{
			try
			{
				//Format:
				//# helix_201.xyz
				//#
				//	0.3517846 -0.7869986 -2.873479
				//  0.5057634 -0.7079139 -2.871073
				//  0.6422613 -0.5988617 -2.868600
				//  0.7554799 -0.4636500 -2.866056
				//  0.8404377 -0.3072074 -2.863440


				var lines = File.ReadAllLines( inputData.SourceRealFileName );

				var positions = new List<Vector3F>( lines.Length );
				var colors = new List<ColorByte>( lines.Length );

				foreach( var line in lines )
				{
					var line2 = line.Trim();

					if( string.IsNullOrEmpty( line2 ) || line2.StartsWith( "#" ) )
						continue;

					if( positions.Count >= inputData.PointLimit )
					{
						error = "Point limit exceeded.";
						return null;
					}

					var strings = line2.Split( new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries );

					var values = new List<double>( strings.Length );
					foreach( var v in strings )
					{
						if( double.TryParse( v, out var value ) )
							values.Add( value );
						else
						{
							error = "Invalid number format. Line: " + line2;
							return null;
						}
					}

					if( values.Count >= 3 )
					{
						positions.Add( new Vector3( values[ 0 ], values[ 1 ], values[ 2 ] ).ToVector3F() );

						if( values.Count == 6 || values.Count == 7 )
						{
							var colorStart = values.Count == 6 ? 3 : 4;
							colors.Add( new ColorByte(
								MathEx.Clamp( (int)( values[ colorStart + 0 ] * 255.0 ), 0, 255 ),
								MathEx.Clamp( (int)( values[ colorStart + 1 ] * 255.0 ), 0, 255 ),
								MathEx.Clamp( (int)( values[ colorStart + 2 ] * 255.0 ), 0, 255 ) ) );
						}
					}
				}

				if( colors.Count != 0 && positions.Count != colors.Count )
				{
					error = "Positions and colors count mismatch.";
					return null;
				}

				var result = new ResultData();
				result.Positions = positions.ToArray();
				result.Colors = colors.ToArray();

				error = null;
				return result;
			}
			catch( Exception e )
			{
				error = "Error loading file. Exception: " + e.Message;
				return null;
			}
		}

		public static ResultData Load_PTS( InputData inputData, out string error )
		{
			try
			{
				//Format:
				//4
				//0.0 0.0 0.0 100 255 0 0
				//1.0 0.0 0.0 100 0 255 0
				//1.0 1.0 0.0 100 0 0 255
				//0.0 1.0 0.0 100 255 255 255


				var lines = File.ReadAllLines( inputData.SourceRealFileName );

				int? checkCount = null;

				var positions = new List<Vector3F>( lines.Length );
				var colors = new List<ColorByte>( lines.Length );

				foreach( var line in lines )
				{
					var line2 = line.Trim();

					if( string.IsNullOrEmpty( line2 ) )
						continue;

					if( positions.Count >= inputData.PointLimit )
					{
						error = "Point limit exceeded.";
						return null;
					}

					var strings = line2.Split( new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries );

					var values = new List<double>( strings.Length );
					foreach( var v in strings )
					{
						if( double.TryParse( v, out var value ) )
							values.Add( value );
						else
						{
							error = "Invalid number format. Line: " + line2;
							return null;
						}
					}

					if( checkCount == null )
					{
						if( values.Count == 1 )
						{
							checkCount = (int)values[ 0 ];
							continue;
						}
						else
						{
							error = "Invalid file format. The first line must contain the number of points.";
							return null;
						}
					}

					if( values.Count >= 3 )
					{
						positions.Add( new Vector3( values[ 0 ], values[ 1 ], values[ 2 ] ).ToVector3F() );

						if( values.Count == 6 || values.Count == 7 )
						{
							var colorStart = values.Count == 6 ? 3 : 4;
							colors.Add( new ColorByte(
								MathEx.Clamp( (int)values[ colorStart + 0 ], 0, 255 ),
								MathEx.Clamp( (int)values[ colorStart + 1 ], 0, 255 ),
								MathEx.Clamp( (int)values[ colorStart + 2 ], 0, 255 ) ) );
						}
					}
				}

				if( colors.Count != 0 && positions.Count != colors.Count )
				{
					error = "Positions and colors count mismatch.";
					return null;
				}

				if( checkCount.Value != positions.Count )
				{
					error = "The number of points in the file does not match the count specified in the first line.";
					return null;
				}

				var result = new ResultData();
				result.Positions = positions.ToArray();
				result.Colors = colors.ToArray();

				error = null;
				return result;
			}
			catch( Exception e )
			{
				error = "Error loading file. Exception: " + e.Message;
				return null;
			}
		}
	}
}
