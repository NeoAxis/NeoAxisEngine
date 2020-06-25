// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Text;
using System.Windows.Forms;
using System.Collections.Generic;
using System.ComponentModel;

namespace NeoAxis.Editor
{
	public partial class SettingsCellProcedureUI : SettingsCell
	{
		WinFormsProcedureUI.WinFormsForm procedureForm;


		/////////////////////////////////////////

		public SettingsCellProcedureUI()
		{
			InitializeComponent();

			SizeType = SizeType.AutoSize;

			//procedureForm = new WinFormsProcedureUI.WinFormsForm( this );
			//OnInitUI();

			//Height = procedureForm.positionY + 6;
		}

		internal override void PerformInit()
		{
			base.PerformInit();

			procedureForm = new WinFormsProcedureUI.WinFormsForm( this );
			OnInitUI();

			Height = procedureForm.positionY + 6;
		}

		protected virtual void OnInitUI() { }
		protected virtual void OnUpdate() { }

		[Browsable( false )]
		public ProcedureUI.Form ProcedureForm
		{
			get { return procedureForm; }
		}

		private void SettingsCellProcedureUI_Load( object sender, EventArgs e )
		{
			timer1.Start();
			OnUpdate();
		}

		private void timer1_Tick( object sender, EventArgs e )
		{
			if( !IsHandleCreated || EditorUtility.IsDesignerHosted( this ) || EditorAPI.ClosingApplication )
				return;

			OnUpdate();
		}
	}
}
