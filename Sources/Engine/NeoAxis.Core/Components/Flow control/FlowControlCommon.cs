// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace NeoAxis
{
	/// <summary>
	/// Represents an input connector data for <see cref="Flow"/>.
	/// </summary>
	public sealed class FlowInput
	{
		IFlowExecutionComponent owner;
		//Component owner;
		string propertyName;

		//!!!!crash on cloning
		public FlowInput()
		{
		}

		public FlowInput( IFlowExecutionComponent owner, string propertyName )
		{
			this.owner = owner;
			this.propertyName = propertyName;
		}

		public IFlowExecutionComponent Owner
		{
			get { return owner; }
		}

		public string PropertyName
		{
			get { return propertyName; }
		}

		public override string ToString()
		{
			if( owner != null )
				return owner.ToString();
			return base.ToString();
		}

		public override bool Equals( object obj )
		{
			return ( obj is FlowInput && this == (FlowInput)obj );
		}

		public bool Equals( FlowInput input )
		{
			return this == input;
		}

		public override int GetHashCode()
		{
			if( owner != null )
				return owner.GetHashCode();
			else
				return base.GetHashCode();
		}

		bool EqualsImpl( FlowInput a )
		{
			if( ReferenceEquals( this, a ) )
				return true;
			return owner == a.owner;
		}

		public static bool operator ==( FlowInput a, FlowInput b )
		{
			bool aNull = ReferenceEquals( a, null );
			bool bNull = ReferenceEquals( b, null );
			if( aNull || bNull )
				return aNull && bNull;
			return a.EqualsImpl( b );
		}

		public static bool operator !=( FlowInput a, FlowInput b )
		{
			bool aNull = ReferenceEquals( a, null );
			bool bNull = ReferenceEquals( b, null );
			if( aNull || bNull )
				return !( aNull && bNull );
			return !a.EqualsImpl( b );
		}
	}
}
