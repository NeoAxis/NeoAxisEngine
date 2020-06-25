// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Reflection;
using System.IO;
using System.Drawing.Design;

namespace NeoAxis
{
	/// <summary>
	/// Represents a basic page of project settings.
	/// </summary>
	public class Component_ProjectSettings_PageBasic : Component_ProjectSettings_Page
	{
		List<PropertyImpl> properties = new List<PropertyImpl>();
		bool initialized;

		/////////////////////////////////////////

		class PropertyImpl : Metadata.Property
		{
			Metadata.Property sourceProperty;
			Component_ProjectSettings settings;

			//

			public PropertyImpl( object owner, string name, bool isStatic, Metadata.TypeInfo type, Metadata.TypeInfo typeUnreferenced, Metadata.Parameter[] indexers, bool readOnly, Metadata.Property sourceProperty, Component_ProjectSettings settings )
				: base( owner, name, isStatic, type, typeUnreferenced, indexers, readOnly )
			{
				this.sourceProperty = sourceProperty;
				this.settings = settings;
			}

			protected override object[] OnGetCustomAttributes( Type attributeType, bool inherit )
			{
				var list = new List<object>( sourceProperty.GetCustomAttributes( attributeType, inherit ) );

				//add Description attribute
				if( attributeType == typeof( DescriptionAttribute ) )
				{
					var memberId = XmlDocumentationFiles.GetMemberId( sourceProperty );
					if( !string.IsNullOrEmpty( memberId ) )
					{
						var summary = XmlDocumentationFiles.GetMemberSummary( memberId );
						if( !string.IsNullOrEmpty( summary ) )
							list.Add( new DescriptionAttribute( summary ) );
					}
				}

				return list.ToArray();
			}

			public override object GetValue( object obj, object[] index )
			{
				return sourceProperty.GetValue( settings, index );
			}

			public override void SetValue( object obj, object value, object[] index )
			{
				sourceProperty.SetValue( settings, value, index );
			}
		}

		/////////////////////////////////////////

		protected override IEnumerable<Metadata.Member> OnMetadataGetMembers()
		{
			foreach( var p in base.OnMetadataGetMembers() )
				yield return p;

			if( UserMode )
			{
				if( !initialized )
				{
					initialized = true;

					var parent = Parent as Component_ProjectSettings;
					if( parent != null )
					{
						foreach( var member in parent.MetadataGetMembers() )
						{
							var sourceProperty = member as Metadata.Property;
							if( sourceProperty != null )
							{
								if( sourceProperty.Name != "Name" && sourceProperty.Name != "Enabled" )
								{
									var attrib = sourceProperty.GetCustomAttribute<CategoryAttribute>();
									if( attrib != null )
									{
										bool add = false;
										if( Name.Length <= attrib.Category.Length && Name == attrib.Category.Substring( 0, Name.Length ) )
											add = true;
										else
										{
											if( Name == "General" )
											{
												var categories = new string[] { "General", "Editor", "Colors", "Project Application", "Play", "Rendering", "Preview", "Import Content" };
												if( categories.Contains( attrib.Category ) )
													add = true;
											}
										}

										if( add )
										{
											var property = new PropertyImpl( this, sourceProperty.Name, sourceProperty.Static, sourceProperty.Type, sourceProperty.TypeUnreferenced, sourceProperty.Indexers, sourceProperty.ReadOnly, sourceProperty, parent );

											property.Browsable = sourceProperty.Browsable;
											property.Cloneable = sourceProperty.Cloneable;
											property.Serializable = SerializeType.Disable;//sourceProperty.Serializable;
											property.DefaultValueSpecified = sourceProperty.DefaultValueSpecified;
											property.DefaultValue = sourceProperty.DefaultValue;

											properties.Add( property );
										}
									}
								}
							}
						}
					}
				}

				foreach( var p in properties )
					yield return p;
			}
		}

		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			base.OnMetadataGetMembersFilter( context, member, ref skip );

			if( UserMode )
			{
				if( member is Metadata.Property )
				{
					if( member.Name == "Name" || member.Name == "Enabled" )
						skip = true;
				}
			}
		}
	}
}
