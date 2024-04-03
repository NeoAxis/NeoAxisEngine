#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace NeoAxis.Editor
{
	internal interface ISettingsCellProcedureUI_Container : ISettingsCell
	{
		ProcedureUI.Form ProcedureForm { get; }
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	public abstract class SettingsCellProcedureUI
	{
		internal ISettingsCellProcedureUI_Container container;

		/////////////////////////////////////////

		public ISettingsCell Container
		{
			get { return container; }
		}

		public ISettingsProvider Provider
		{
			get { return container.Provider; }
		}

		public virtual float CellsSortingPriority
		{
			get { return container.CellsSortingPriority; }
			set { container.CellsSortingPriority = value; }
		}

		public FormSizeType SizeType
		{
			get { return container.SizeType; }
			set { container.SizeType = value; }
		}

		public T[] GetObjects<T>() where T : class
		{
			return Provider.SelectedObjects.OfType<T>().ToArray();
		}

		public T GetFirstObject<T>() where T : class
		{
			foreach( var obj in Provider.SelectedObjects )
			{
				var obj2 = obj as T;
				if( obj2 != null )
					return obj2;
			}
			return null;
		}

		/////////////////////////////////////////

		protected virtual void OnInit() { }
		internal void PerformInit()
		{
			OnInit();
		}

		protected virtual void OnUpdate() { }
		internal void PerformUpdate()
		{
			OnUpdate();
		}

		[Browsable( false )]
		public ProcedureUI.Form ProcedureForm
		{
			get { return container.ProcedureForm; }
		}
	}

}

#endif