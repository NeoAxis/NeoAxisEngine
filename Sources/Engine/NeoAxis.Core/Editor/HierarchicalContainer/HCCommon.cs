// Copyright 2006Ц2026 Ivan Efimov. All rights reserved.
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoAxis.Editor
{
	//!!!!еще сортировку где-то указывать
	[AttributeUsage( AttributeTargets.Class | AttributeTargets.Struct )]
	public class HCExpandableAttribute : Attribute
	{
		public HCExpandableAttribute()
		{
		}
	}

	[AttributeUsage( AttributeTargets.Property | AttributeTargets.Field )]
	public class HCTextBoxPasswordAttribute : Attribute
	{
		char? passwordChar;

		public HCTextBoxPasswordAttribute()
		{
		}

		public HCTextBoxPasswordAttribute( char passwordChar )
		{
			this.passwordChar = passwordChar;
		}

		public char? PasswordChar
		{
			get { return passwordChar; }
		}
	}
}
