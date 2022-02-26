// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Reflection;
using System.IO;
using System.Drawing.Design;

namespace NeoAxis
{
	/// <summary>
	/// Base class of all virtual member components.
	/// </summary>
	public abstract class VirtualMember : Component
	{
		bool needUpdateCreatedMembers = true;
		Metadata.Member[] createdMembers;

		//

		//Description
		ReferenceField<string> _description = "";
		/// <summary>
		/// The description of the member.
		/// </summary>
		[Serialize]
		[DefaultValue( "" )]
		public Reference<string> Description
		{
			get
			{
				if( _description.BeginGet() )
					Description = _description.Get( this );
				return _description.value;
			}
			set
			{
				if( _description.BeginSet( ref value ) )
				{
					try
					{
						DescriptionChanged?.Invoke( this );
						//!!!!можно не пересоздавать
						if( Enabled )
							NeedUpdateCreatedMembers();
					}
					finally { _description.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Description"/> property value changes.</summary>
		public event Action<VirtualMember> DescriptionChanged;


		//Static
		ReferenceField<bool> _static;
		/// <summary>
		/// Whether the member is static.
		/// </summary>
		[Serialize]
		[DefaultValue( false )]
		public Reference<bool> Static
		{
			get
			{
				if( _static.BeginGet() )
					Static = _static.Get( this );
				return _static.value;
			}
			set
			{
				if( _static.BeginSet( ref value ) )
				{
					try
					{
						StaticChanged?.Invoke( this );
						if( Enabled )
							NeedUpdateCreatedMembers();
					}
					finally { _static.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Static"/> property value changes.</summary>
		public event Action<VirtualMember> StaticChanged;


		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		protected override void OnAddedToParent()
		{
			base.OnAddedToParent();

			//!!!!так?
			NeedUpdateCreatedMembers();
		}

		protected override void OnRemovedFromParent( Component oldParent )
		{
			base.OnRemovedFromParent( oldParent );

			NeedUpdateCreatedMembers();
			oldParent?.VirtualMembersNeedUpdate();
		}

		protected override void OnEnabledChanged()
		{
			base.OnEnabledChanged();

			NeedUpdateCreatedMembers();
		}

		public override string Name
		{
			get { return base.Name; }
			set
			{
				if( base.Name == value )
					return;
				base.Name = value;

				if( Enabled )
					NeedUpdateCreatedMembers();
			}
		}

		public virtual void NeedUpdateCreatedMembers()
		{
			needUpdateCreatedMembers = true;
			createdMembers = null;
			Parent?.VirtualMembersNeedUpdate();
		}

		protected abstract void CreateMembers( List<Metadata.Member> created );

		[Browsable( false )]
		public Metadata.Member[] CreatedMembers
		{
			get
			{
				if( needUpdateCreatedMembers )
				{
					needUpdateCreatedMembers = false;
					createdMembers = null;
					if( Enabled )
					{
						var list = new List<Metadata.Member>();
						CreateMembers( list );
						if( list.Count != 0 )
							createdMembers = list.ToArray();
					}
				}
				return createdMembers;
			}
		}

		public /*virtual */void UpdateParentVirtualMembers( ref List<Metadata.Member> members, ref Dictionary<string, Metadata.Member> memberBySignature )
		{
			//add to parent created members
			var list = CreatedMembers;
			if( list != null )
			{
				foreach( var member in list )
				{
					if( members == null )
					{
						members = new List<Metadata.Member>();
						memberBySignature = new Dictionary<string, Metadata.Member>();
					}
					members.Add( member );
					memberBySignature[ member.Signature ] = member;
				}
			}
		}

		[Browsable( false )]
		internal override bool TypeOnly
		{
			get { return true; }
		}
	}
}
