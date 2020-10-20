// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.ComponentModel;
using System.Reflection;
using System.Collections;
using System.Threading;
using System.Windows.Forms;

namespace NeoAxis.Editor
{
	public class EngineToolTip
	{
		static ESet<EngineToolTip> instancesWithData = new ESet<EngineToolTip>();

		Dictionary<Control, string> controls = new Dictionary<Control, string>();

		/////////////////////////////////////////

		static EngineToolTip global;
		public static EngineToolTip Global
		{
			get
			{
				if( global == null )
					global = new EngineToolTip();
				return global;
			}
		}

		public EngineToolTip()
		{
		}

		public EngineToolTip( IContainer container )
		{
		}

		public void Dispose()
		{
			lock( controls )
			{
				foreach( var control in controls.Keys )
					Hide( control );
			}
		}

		public string GetToolTip( Control control )
		{
			lock( controls )
			{
				if( control != null && controls.TryGetValue( control, out var text ) )
					return text;
				return "";
			}
		}

		public void SetToolTip( Control control, string text )
		{
			Hide( control );

			if( control != null )
			{
				lock( controls )
				{
					if( !string.IsNullOrEmpty( text ) )
						controls[ control ] = text;
					else
						controls.Remove( control );
				}
			}

			lock( instancesWithData )
			{
				if( controls.Count != 0 )
					instancesWithData.AddWithCheckAlreadyContained( this );
				else
					instancesWithData.Remove( this );
			}
		}

		public void Hide( Control control )
		{
			if( control != null )
				EngineToolTipManager.Hide( control );
		}

		void Update( out bool remove )
		{
			lock( controls )
			{
				if( controls.Count != 0 )
				{
					again:
					foreach( var control in controls.Keys )
					{
						if( control.IsDisposed )
						{
							controls.Remove( control );
							goto again;
						}
					}
				}

				remove = controls.Count == 0;
			}
		}

		internal static void UpdateAllInstances()
		{
			lock( instancesWithData )
			{
				var toRemove = new List<EngineToolTip>();

				foreach( var toolTip in instancesWithData )
				{
					toolTip.Update( out var remove );
					if( remove )
						toRemove.Add( toolTip );
				}

				foreach( var toolTip in toRemove )
					instancesWithData.Remove( toolTip );
			}
		}

		internal static (EngineToolTip toolTip, string text) GetToolTipByControl( Control control )
		{
			//!!!!slowly?

			lock( instancesWithData )
			{
				foreach( var toolTip in instancesWithData )
				{
					lock( toolTip.controls )
					{
						if( toolTip.controls.TryGetValue( control, out var text ) )
							return (toolTip, text);
					}
				}
			}

			return (null, "");
		}

	}
}
