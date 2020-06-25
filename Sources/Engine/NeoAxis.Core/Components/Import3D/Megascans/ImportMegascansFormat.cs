namespace NeoAxis.Import
{
	using System;
	using System.Collections.Generic;

	using System.Globalization;
	using Newtonsoft.Json;
	using Newtonsoft.Json.Converters;

	using System.IO;

	static class ImportMegascansFormat
	{
		public partial class Welcome
		{
			//[JsonProperty( "pack" )]
			//public object Pack { get; set; }

			//[JsonProperty( "name" )]
			//public string Name { get; set; }

			//[JsonProperty( "tags" )]
			//public string[] Tags { get; set; }

			[JsonProperty( "models" )]
			public Model[] Models { get; set; }

			[JsonProperty( "billboards" )]
			public Billboard[] Billboards { get; set; }

			//[JsonProperty( "brushes" )]
			//public Json[] Brushes { get; set; }

			//[JsonProperty( "previews" )]
			//public Previews Previews { get; set; }

			//[JsonProperty( "environment" )]
			//public Environment Environment { get; set; }

			[JsonProperty( "maps" )]
			public WelcomeMap[] Maps { get; set; }

			//[JsonProperty( "json" )]
			//public Json Json { get; set; }

			//[JsonProperty( "points" )]
			//public long Points { get; set; }

			//[JsonProperty( "meta" )]
			//public Meta[] Meta { get; set; }

			//[JsonProperty( "averageColor" )]
			//public string AverageColor { get; set; }

			[JsonProperty( "components" )]
			public Component[] Components { get; set; }

			//[JsonProperty( "references" )]
			//public object[] References { get; set; }

			//[JsonProperty( "referencePreviews" )]
			//public ReferencePreviews ReferencePreviews { get; set; }

			//[JsonProperty( "properties" )]
			//public Property[] Properties { get; set; }

			//[JsonProperty( "categories" )]
			//public string[] Categories { get; set; }

			[JsonProperty( "meshes" )]
			public Mesh[] Meshes { get; set; }

			//[JsonProperty( "id" )]
			//public string Id { get; set; }

			//[JsonProperty( "physicalSize" )]
			//public string PhysicalSize { get; set; }
		}

		public partial class Json
		{
			[JsonProperty( "contentLength" )]
			public long ContentLength { get; set; }

			[JsonProperty( "uri" )]
			public string Uri { get; set; }
		}

		public partial class WelcomeMap
		{
			//[JsonProperty( "mimeType" )]
			//public MimeType MimeType { get; set; }

			[JsonProperty( "minIntensity" )]
			public long MinIntensity { get; set; }

			[JsonProperty( "bitDepth" )]
			public long BitDepth { get; set; }

			[JsonProperty( "name" )]
			public string Name { get; set; }

			//[JsonProperty( "resolution" )]
			//public Resolution Resolution { get; set; }

			[JsonProperty( "contentLength" )]
			public long ContentLength { get; set; }

			//[JsonProperty( "colorSpace" )]
			//public ColorSpace ColorSpace { get; set; }

			[JsonProperty( "uri" )]
			public string Uri { get; set; }

			//[JsonProperty( "physicalSize" )]
			//public PhysicalSize PhysicalSize { get; set; }

			[JsonProperty( "maxIntensity" )]
			public long MaxIntensity { get; set; }

			[JsonProperty( "type" )]
			public string Type { get; set; }

			[JsonProperty( "averageColor" )]
			public string AverageColor { get; set; }
		}

		public partial class Billboard
		{
			//[JsonProperty( "mimeType" )]
			//public BillboardMimeType MimeType { get; set; }

			[JsonProperty( "bitDepth" )]
			public long BitDepth { get; set; }

			//[JsonProperty( "name" )]
			//public Name Name { get; set; }

			//[JsonProperty( "resolution" )]
			//public Resolution Resolution { get; set; }

			[JsonProperty( "contentLength" )]
			public long ContentLength { get; set; }

			//[JsonProperty( "colorSpace" )]
			//public ColorSpace ColorSpace { get; set; }

			[JsonProperty( "uri" )]
			public string Uri { get; set; }

			//[JsonProperty( "asset" )]
			//public Id Asset { get; set; }

			//[JsonProperty( "physicalSize" )]
			//public PhysicalSize PhysicalSize { get; set; }

			[JsonProperty( "type" )]
			public string Type { get; set; }
			//[JsonProperty( "type" )]
			//public BillboardType Type { get; set; }

			//[JsonProperty( "averageColor" )]
			//public AverageColor AverageColor { get; set; }
		}

		public partial class Component
		{
			[JsonProperty( "colorSpace" )]
			public string ColorSpace { get; set; }

			[JsonProperty( "type" )]
			public string Type { get; set; }

			[JsonProperty( "name" )]
			public string Name { get; set; }

			[JsonProperty( "averageColor" )]
			public string AverageColor { get; set; }

			[JsonProperty( "uris" )]
			public ComponentUris[] Uris { get; set; }
		}

		public partial class ComponentUris
		{
			[JsonProperty( "physicalSize" )]
			public string PhysicalSize { get; set; }

			[JsonProperty( "resolutions" )]
			public ResolutionElement[] Resolutions { get; set; }
		}

		public partial class ResolutionElement
		{
			//[JsonProperty( "resolution" )]
			//public ResolutionResolution Resolution { get; set; }

			[JsonProperty( "formats" )]
			public Format[] Formats { get; set; }
		}

		public partial class Format
		{
			//[JsonProperty( "mimeType" )]
			//public FormatMimeType MimeType { get; set; }

			[JsonProperty( "contentLength" )]
			public long ContentLength { get; set; }

			[JsonProperty( "bitDepth" )]
			public long BitDepth { get; set; }

			[JsonProperty( "uri" )]
			public string Uri { get; set; }

			//[JsonProperty( "lodType", NullValueHandling = NullValueHandling.Ignore )]
			//public LodType? LodType { get; set; }

			[JsonProperty( "tris", NullValueHandling = NullValueHandling.Ignore )]
			public long? Tris { get; set; }
		}

		public partial class Environment
		{
			[JsonProperty( "biome" )]
			public string Biome { get; set; }

			[JsonProperty( "region" )]
			public string Region { get; set; }
		}

		public partial class Mesh
		{
			[JsonProperty( "type" )]
			public string Type { get; set; }

			[JsonProperty( "uris" )]
			public MeshUris[] Uris { get; set; }

			[JsonProperty( "tris", NullValueHandling = NullValueHandling.Ignore )]
			public long? Tris { get; set; }
		}

		public partial class MeshUris
		{
			//[JsonProperty( "mimeType" )]
			//public UrisMimeType MimeType { get; set; }

			[JsonProperty( "contentLength" )]
			public long ContentLength { get; set; }

			[JsonProperty( "uri" )]
			public string Uri { get; set; }
		}

		public partial class Meta
		{
			[JsonProperty( "key" )]
			public string Key { get; set; }

			[JsonProperty( "name" )]
			public string Name { get; set; }

			[JsonProperty( "value" )]
			public Value Value { get; set; }
		}

		public partial class Model
		{
			//[JsonProperty( "mimeType" )]
			//public ModelMimeType MimeType { get; set; }

			[JsonProperty( "lod" )]
			public long Lod { get; set; }

			[JsonProperty( "contentLength" )]
			public long ContentLength { get; set; }

			[JsonProperty( "uri" )]
			public string Uri { get; set; }

			[JsonProperty( "variation" )]
			public long Variation { get; set; }

			//[JsonProperty( "type" )]
			//public ModelType Type { get; set; }

			[JsonProperty( "tris", NullValueHandling = NullValueHandling.Ignore )]
			public long? Tris { get; set; }
		}

		public partial class Previews
		{
			[JsonProperty( "images" )]
			public Image[] Images { get; set; }

			[JsonProperty( "scaleReferences" )]
			public ScaleReference[] ScaleReferences { get; set; }

			[JsonProperty( "relativeSize" )]
			public string RelativeSize { get; set; }
		}

		public partial class Image
		{
			[JsonProperty( "contentLength" )]
			public long ContentLength { get; set; }

			[JsonProperty( "resolution" )]
			public ImageResolution Resolution { get; set; }

			[JsonProperty( "uri" )]
			public string Uri { get; set; }

			[JsonProperty( "tags" )]
			public string[] Tags { get; set; }
		}

		public partial class ScaleReference
		{
			[JsonProperty( "contentLength" )]
			public long ContentLength { get; set; }

			[JsonProperty( "tag" )]
			public string Tag { get; set; }

			[JsonProperty( "uri" )]
			public string Uri { get; set; }
		}

		public partial class Property
		{
			[JsonProperty( "key" )]
			public string Key { get; set; }

			[JsonProperty( "value" )]
			public string Value { get; set; }
		}

		public partial class ReferencePreviews
		{
			[JsonProperty( "models" )]
			public Map[] Models { get; set; }

			[JsonProperty( "maps" )]
			public Map[] Maps { get; set; }
		}

		public partial class Map
		{
			[JsonProperty( "mimeType" )]
			public MapMimeType MimeType { get; set; }

			[JsonProperty( "resolution", NullValueHandling = NullValueHandling.Ignore )]
			public ResolutionResolution? Resolution { get; set; }

			[JsonProperty( "contentLength" )]
			public long ContentLength { get; set; }

			[JsonProperty( "type" )]
			public string Type { get; set; }

			[JsonProperty( "uri" )]
			public string Uri { get; set; }
		}

		public enum LodType { Lod };

		public enum FormatMimeType { ImageJpeg, ImageXExr };

		public enum ResolutionResolution { The1024X1024, The2048X2048, The256X256, The4096X4096, The512X512, The8192X8192 };

		public enum UrisMimeType { ApplicationXFbx, ApplicationXObj, ApplicationXZtl };

		public enum ImageResolution { The1411X720, The1568X800, The47X24, The784X400 };

		public enum MapMimeType { ApplicationXFbx, ApplicationXObj, ImageJpeg };

		public partial struct Value
		{
			public long? Integer;
			public string String;

			public static implicit operator Value( long Integer ) => new Value { Integer = Integer };
			public static implicit operator Value( string String ) => new Value { String = String };
		}

		public partial class Welcome
		{
			public static Welcome FromJson( string json ) => JsonConvert.DeserializeObject<Welcome>( json, NeoAxis.Import.ImportMegascansFormat.Converter.Settings );
		}

		//public static class Serialize
		//{
		//	public static string ToJson( this Welcome self ) => JsonConvert.SerializeObject( self, QuickType.Converter.Settings );
		//}

		internal static class Converter
		{
			public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
			{
				MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
				DateParseHandling = DateParseHandling.None,
				Converters =
			{
				LodTypeConverter.Singleton,
				FormatMimeTypeConverter.Singleton,
				ResolutionResolutionConverter.Singleton,
				UrisMimeTypeConverter.Singleton,
				ValueConverter.Singleton,
				ImageResolutionConverter.Singleton,
				MapMimeTypeConverter.Singleton,
				new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
			},
			};
		}

		internal class LodTypeConverter : JsonConverter
		{
			public override bool CanConvert( Type t ) => t == typeof( LodType ) || t == typeof( LodType? );

			public override object ReadJson( JsonReader reader, Type t, object existingValue, JsonSerializer serializer )
			{
				if( reader.TokenType == JsonToken.Null ) return null;
				var value = serializer.Deserialize<string>( reader );
				if( value == "lod" )
				{
					return LodType.Lod;
				}
				throw new Exception( "Cannot unmarshal type LodType" );
			}

			public override void WriteJson( JsonWriter writer, object untypedValue, JsonSerializer serializer )
			{
				if( untypedValue == null )
				{
					serializer.Serialize( writer, null );
					return;
				}
				var value = (LodType)untypedValue;
				if( value == LodType.Lod )
				{
					serializer.Serialize( writer, "lod" );
					return;
				}
				throw new Exception( "Cannot marshal type LodType" );
			}

			public static readonly LodTypeConverter Singleton = new LodTypeConverter();
		}

		internal class FormatMimeTypeConverter : JsonConverter
		{
			public override bool CanConvert( Type t ) => t == typeof( FormatMimeType ) || t == typeof( FormatMimeType? );

			public override object ReadJson( JsonReader reader, Type t, object existingValue, JsonSerializer serializer )
			{
				if( reader.TokenType == JsonToken.Null ) return null;
				var value = serializer.Deserialize<string>( reader );
				switch( value )
				{
				case "image/jpeg":
					return FormatMimeType.ImageJpeg;
				case "image/x-exr":
					return FormatMimeType.ImageXExr;
				}
				throw new Exception( "Cannot unmarshal type FormatMimeType" );
			}

			public override void WriteJson( JsonWriter writer, object untypedValue, JsonSerializer serializer )
			{
				if( untypedValue == null )
				{
					serializer.Serialize( writer, null );
					return;
				}
				var value = (FormatMimeType)untypedValue;
				switch( value )
				{
				case FormatMimeType.ImageJpeg:
					serializer.Serialize( writer, "image/jpeg" );
					return;
				case FormatMimeType.ImageXExr:
					serializer.Serialize( writer, "image/x-exr" );
					return;
				}
				throw new Exception( "Cannot marshal type FormatMimeType" );
			}

			public static readonly FormatMimeTypeConverter Singleton = new FormatMimeTypeConverter();
		}

		internal class ResolutionResolutionConverter : JsonConverter
		{
			public override bool CanConvert( Type t ) => t == typeof( ResolutionResolution ) || t == typeof( ResolutionResolution? );

			public override object ReadJson( JsonReader reader, Type t, object existingValue, JsonSerializer serializer )
			{
				if( reader.TokenType == JsonToken.Null ) return null;
				var value = serializer.Deserialize<string>( reader );
				switch( value )
				{
				case "1024x1024":
					return ResolutionResolution.The1024X1024;
				case "2048x2048":
					return ResolutionResolution.The2048X2048;
				case "256x256":
					return ResolutionResolution.The256X256;
				case "4096x4096":
					return ResolutionResolution.The4096X4096;
				case "512x512":
					return ResolutionResolution.The512X512;
				case "8192x8192":
					return ResolutionResolution.The8192X8192;
				}
				throw new Exception( "Cannot unmarshal type ResolutionResolution" );
			}

			public override void WriteJson( JsonWriter writer, object untypedValue, JsonSerializer serializer )
			{
				if( untypedValue == null )
				{
					serializer.Serialize( writer, null );
					return;
				}
				var value = (ResolutionResolution)untypedValue;
				switch( value )
				{
				case ResolutionResolution.The1024X1024:
					serializer.Serialize( writer, "1024x1024" );
					return;
				case ResolutionResolution.The2048X2048:
					serializer.Serialize( writer, "2048x2048" );
					return;
				case ResolutionResolution.The256X256:
					serializer.Serialize( writer, "256x256" );
					return;
				case ResolutionResolution.The4096X4096:
					serializer.Serialize( writer, "4096x4096" );
					return;
				case ResolutionResolution.The512X512:
					serializer.Serialize( writer, "512x512" );
					return;
				case ResolutionResolution.The8192X8192:
					serializer.Serialize( writer, "8192x8192" );
					return;
				}
				throw new Exception( "Cannot marshal type ResolutionResolution" );
			}

			public static readonly ResolutionResolutionConverter Singleton = new ResolutionResolutionConverter();
		}

		internal class UrisMimeTypeConverter : JsonConverter
		{
			public override bool CanConvert( Type t ) => t == typeof( UrisMimeType ) || t == typeof( UrisMimeType? );

			public override object ReadJson( JsonReader reader, Type t, object existingValue, JsonSerializer serializer )
			{
				if( reader.TokenType == JsonToken.Null ) return null;
				var value = serializer.Deserialize<string>( reader );
				switch( value )
				{
				case "application/x-fbx":
					return UrisMimeType.ApplicationXFbx;
				case "application/x-obj":
					return UrisMimeType.ApplicationXObj;
				case "application/x-ztl":
					return UrisMimeType.ApplicationXZtl;
				}
				throw new Exception( "Cannot unmarshal type UrisMimeType" );
			}

			public override void WriteJson( JsonWriter writer, object untypedValue, JsonSerializer serializer )
			{
				if( untypedValue == null )
				{
					serializer.Serialize( writer, null );
					return;
				}
				var value = (UrisMimeType)untypedValue;
				switch( value )
				{
				case UrisMimeType.ApplicationXFbx:
					serializer.Serialize( writer, "application/x-fbx" );
					return;
				case UrisMimeType.ApplicationXObj:
					serializer.Serialize( writer, "application/x-obj" );
					return;
				case UrisMimeType.ApplicationXZtl:
					serializer.Serialize( writer, "application/x-ztl" );
					return;
				}
				throw new Exception( "Cannot marshal type UrisMimeType" );
			}

			public static readonly UrisMimeTypeConverter Singleton = new UrisMimeTypeConverter();
		}

		internal class ValueConverter : JsonConverter
		{
			public override bool CanConvert( Type t ) => t == typeof( Value ) || t == typeof( Value? );

			public override object ReadJson( JsonReader reader, Type t, object existingValue, JsonSerializer serializer )
			{
				switch( reader.TokenType )
				{
				case JsonToken.Integer:
					var integerValue = serializer.Deserialize<long>( reader );
					return new Value { Integer = integerValue };
				case JsonToken.String:
				case JsonToken.Date:
					var stringValue = serializer.Deserialize<string>( reader );
					return new Value { String = stringValue };
				}
				throw new Exception( "Cannot unmarshal type Value" );
			}

			public override void WriteJson( JsonWriter writer, object untypedValue, JsonSerializer serializer )
			{
				var value = (Value)untypedValue;
				if( value.Integer != null )
				{
					serializer.Serialize( writer, value.Integer.Value );
					return;
				}
				if( value.String != null )
				{
					serializer.Serialize( writer, value.String );
					return;
				}
				throw new Exception( "Cannot marshal type Value" );
			}

			public static readonly ValueConverter Singleton = new ValueConverter();
		}

		internal class ImageResolutionConverter : JsonConverter
		{
			public override bool CanConvert( Type t ) => t == typeof( ImageResolution ) || t == typeof( ImageResolution? );

			public override object ReadJson( JsonReader reader, Type t, object existingValue, JsonSerializer serializer )
			{
				if( reader.TokenType == JsonToken.Null ) return null;
				var value = serializer.Deserialize<string>( reader );
				switch( value )
				{
				case "1411x720":
					return ImageResolution.The1411X720;
				case "1568x800":
					return ImageResolution.The1568X800;
				case "47x24":
					return ImageResolution.The47X24;
				case "784x400":
					return ImageResolution.The784X400;
				}
				throw new Exception( "Cannot unmarshal type ImageResolution" );
			}

			public override void WriteJson( JsonWriter writer, object untypedValue, JsonSerializer serializer )
			{
				if( untypedValue == null )
				{
					serializer.Serialize( writer, null );
					return;
				}
				var value = (ImageResolution)untypedValue;
				switch( value )
				{
				case ImageResolution.The1411X720:
					serializer.Serialize( writer, "1411x720" );
					return;
				case ImageResolution.The1568X800:
					serializer.Serialize( writer, "1568x800" );
					return;
				case ImageResolution.The47X24:
					serializer.Serialize( writer, "47x24" );
					return;
				case ImageResolution.The784X400:
					serializer.Serialize( writer, "784x400" );
					return;
				}
				throw new Exception( "Cannot marshal type ImageResolution" );
			}

			public static readonly ImageResolutionConverter Singleton = new ImageResolutionConverter();
		}

		internal class MapMimeTypeConverter : JsonConverter
		{
			public override bool CanConvert( Type t ) => t == typeof( MapMimeType ) || t == typeof( MapMimeType? );

			public override object ReadJson( JsonReader reader, Type t, object existingValue, JsonSerializer serializer )
			{
				if( reader.TokenType == JsonToken.Null ) return null;
				var value = serializer.Deserialize<string>( reader );
				switch( value )
				{
				case "application/x-fbx":
					return MapMimeType.ApplicationXFbx;
				case "application/x-obj":
					return MapMimeType.ApplicationXObj;
				case "image/jpeg":
					return MapMimeType.ImageJpeg;
				}
				throw new Exception( "Cannot unmarshal type MapMimeType" );
			}

			public override void WriteJson( JsonWriter writer, object untypedValue, JsonSerializer serializer )
			{
				if( untypedValue == null )
				{
					serializer.Serialize( writer, null );
					return;
				}
				var value = (MapMimeType)untypedValue;
				switch( value )
				{
				case MapMimeType.ApplicationXFbx:
					serializer.Serialize( writer, "application/x-fbx" );
					return;
				case MapMimeType.ApplicationXObj:
					serializer.Serialize( writer, "application/x-obj" );
					return;
				case MapMimeType.ImageJpeg:
					serializer.Serialize( writer, "image/jpeg" );
					return;
				}
				throw new Exception( "Cannot marshal type MapMimeType" );
			}

			public static readonly MapMimeTypeConverter Singleton = new MapMimeTypeConverter();
		}

		/////////////////////////////////////////

		static string GetTextureName( Welcome json, string directoryName, string type )
		{
			if( json.Components != null )
			{
				Component GetComponent()
				{
					foreach( var c2 in json.Components )
						if( c2.Type == type )
							return c2;
					return null;
				}

				var c = GetComponent();
				if( c != null && c.Uris != null )
				{
					foreach( var uri in c.Uris )
					{
						if( uri.Resolutions != null )
						{
							foreach( var res in uri.Resolutions )
							{
								if( res.Formats != null )
								{
									foreach( var format in res.Formats )
									{
										if( !string.IsNullOrEmpty( format.Uri ) )
										{
											var fileName = Path.Combine( directoryName, format.Uri );
											if( VirtualFile.Exists( fileName ) )
											{
												//!!!!what to select when more than one file exists. exr, jpg.

												return fileName;
											}
										}
									}
								}
							}
						}
					}
				}
			}

			return "";
		}
	}
}