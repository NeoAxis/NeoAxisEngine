// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Text;
using System.Windows.Forms;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace NeoAxis.Editor
{
	partial class SettingsCellProcedureUI_Container : SettingsCell, ISettingsCellProcedureUI_Container
	{
		internal SettingsCellProcedureUI procedureUI;
		WinFormsProcedureUI.WinFormsForm procedureForm;

		/////////////////////////////////////////

		public SettingsCellProcedureUI_Container()
		{
			InitializeComponent();

			SizeType = FormSizeType.AutoSize;
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
}
