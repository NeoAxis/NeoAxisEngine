// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Text;
using System.Windows.Forms;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace NeoAxis.Editor
{
	partial class SettingsCellProcedureUI_Container : SettingsCell
	{
		internal SettingsCellProcedureUI procedureUI;
		WinFormsProcedureUI.WinFormsForm procedureForm;

		/////////////////////////////////////////

		public SettingsCellProcedureUI_Container()
		{
			InitializeComponent();

			SizeType = SizeType.AutoSize;
		}

		[Browsable( false )]
		public ProcedureUI.Form ProcedureForm
		{
			get { return procedureForm; }
		}

		internal override void PerformInit()
		{
			base.PerformInit();

			procedureForm = new WinFormsProcedureUI.WinFormsForm( this );
			procedureUI.PerformInit();

			Height = procedureForm.positionY + 6;
		}

		private void SettingsCellProcedureUI_Container_Load( object sender, EventArgs e )
		{
			timer1.Start();
			procedureUI.PerformUpdate();
		}

		private void timer1_Tick( object sender, EventArgs e )
		{
			if( !IsHandleCreated || WinFormsUtility.IsDesignerHosted( this ) || EditorAPI.ClosingApplication )
				return;

			procedureUI.PerformUpdate();
		}
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	public abstract class SettingsCellProcedureUI
	{
		internal SettingsCellProcedureUI_Container container;

		/////////////////////////////////////////

		public SettingsCell/*ProcedureUI_Container*/ Container
		{
			get { return container; }
		}

		public SettingsProvider Provider
		{
			get { return container.Provider; }
		}

		public virtual float CellsSortingPriority
		{
			get { return container.CellsSortingPriority; }
			set { container.CellsSortingPriority = value; }
		}

		public SizeType SizeType
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
