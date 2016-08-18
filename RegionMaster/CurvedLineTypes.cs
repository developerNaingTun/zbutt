using System.Runtime.InteropServices;
using System.ComponentModel;
using System;    
using System.ComponentModel.Design;
using Microsoft.Win32;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.ComponentModel;
using System.Windows.Forms.Design;

namespace Microsoft.Samples
{
	// Used by the CurvedLine to specify the corner of each arc
	[Editor("Microsoft.Samples.CurvedLineTypeEditor" , typeof(UITypeEditor))]
	public enum CurvedLineTypes
	{
		LowerLeftQuarterCirle,
		LowerRightQuarterCircle,
		UpperLeftQuarterCirle,
		UpperRightQuarterCirle
	}

	// The UITypeEditor for the CurvedLineTypes enum
	#region UITypeEditor
	public class CurvedLineTypeEditor : UITypeEditor 
	{
		private CurvedLineTypeUI lineTypeUI;

		public override object EditValue(ITypeDescriptorContext context, IServiceProvider  provider, object value) 
		{
			object returnValue = value;
			if (provider != null) 
			{
				IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
				if (edSvc != null) 
				{
					if (lineTypeUI == null) 
					{
						lineTypeUI = new CurvedLineTypeUI(this);
					}
					lineTypeUI.Start(edSvc, value);
					edSvc.DropDownControl(lineTypeUI);
					value = lineTypeUI.Value;
					lineTypeUI.End();
				}
			}
			return value;
		}


		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) 
		{
			return UITypeEditorEditStyle.DropDown;
		}

		#region Control used in the UITypeEditor
		private class CurvedLineTypeUI : Control 
		{
			private IWindowsFormsEditorService edSvc;
			private CurvedLineTypeEditor editor = null;
			private CurvedLineTypes oldLineType;
			private object value;

			public CurvedLineTypeUI(CurvedLineTypeEditor editor) 
			{
				this.editor = editor;
				InitializeComponent();
			}
            
			public object Value 
			{ 
				get 
				{
					return value;
				}
			}
            
			public void End() 
			{
				value = null;
				edSvc = null;
			}

			public void Start(IWindowsFormsEditorService edSvc, object value) 
			{
				this.edSvc = edSvc;
				this.value = value;
				this.oldLineType = (CurvedLineTypes)value;

				Panel panel;
				foreach (Control c in Controls) 
				{
					panel = c as Panel;
					if (c != null) 
					{
						c.BackColor = SystemColors.Control;
						c.ForeColor = SystemColors.ControlText;
					}
					panel = null;
				}
				
				switch ((CurvedLineTypes)value) 
				{
					case CurvedLineTypes.UpperLeftQuarterCirle: 
						this.upperLeftPanel.BackColor = SystemColors.ControlText;
						this.upperLeftPanel.ForeColor = SystemColors.Control;
						break;
					case CurvedLineTypes.UpperRightQuarterCirle:
						this.upperRightPanel.BackColor = SystemColors.ControlText;
						this.upperRightPanel.ForeColor = SystemColors.Control;
						break;
					case CurvedLineTypes.LowerLeftQuarterCirle:
						this.lowerLeftPanel.BackColor = SystemColors.ControlText;
						this.lowerLeftPanel.ForeColor = SystemColors.Control;
						break;
					case CurvedLineTypes.LowerRightQuarterCircle:
						this.lowerRightPanel.BackColor = SystemColors.ControlText;
						this.lowerRightPanel.ForeColor = SystemColors.Control;
						break;
					default: break;
				}
			}

			private void Teardown(bool save)
			{
				if (!save) 
				{
					value = oldLineType;
				}
				edSvc.CloseDropDown();
			}

			private void button_Click(object sender, System.EventArgs e)
			{
				CurvedLine line = sender as CurvedLine;
				if (line != null) 
				{
					this.value = line.CurveType;
					Teardown(true);
				}
			}

			#region Component Designer generated code

			private System.Windows.Forms.Panel upperLeftPanel;
			private System.Windows.Forms.Panel upperRightPanel;
			private System.Windows.Forms.Panel lowerLeftPanel;
			private System.Windows.Forms.Panel lowerRightPanel;
			private Microsoft.Samples.CurvedLine upperLeftLine;
			private Microsoft.Samples.CurvedLine upperRightLine;
			private Microsoft.Samples.CurvedLine lowerLeftLine;
			private Microsoft.Samples.CurvedLine lowerRighttLine;

			internal virtual void InitializeComponent() 
			{
				this.upperLeftPanel = new System.Windows.Forms.Panel();
				this.upperRightPanel = new System.Windows.Forms.Panel();
				this.lowerLeftPanel = new System.Windows.Forms.Panel();
				this.lowerRightPanel = new System.Windows.Forms.Panel();
				this.upperLeftLine = new Microsoft.Samples.CurvedLine();
				this.upperRightLine = new Microsoft.Samples.CurvedLine();
				this.lowerLeftLine = new Microsoft.Samples.CurvedLine();
				this.lowerRighttLine = new Microsoft.Samples.CurvedLine();
				this.upperLeftPanel.SuspendLayout();
				this.upperRightPanel.SuspendLayout();
				this.lowerLeftPanel.SuspendLayout();
				this.lowerRightPanel.SuspendLayout();
				this.SuspendLayout();
				// 
				// upperLeftPanel
				// 
				this.upperLeftPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
				this.upperLeftPanel.Controls.AddRange(new System.Windows.Forms.Control[] {
																					 this.upperLeftLine});
				this.upperLeftPanel.Location = new System.Drawing.Point(2, 2);
				this.upperLeftPanel.Name = "upperLeftPanel";
				this.upperLeftPanel.Size = new System.Drawing.Size(40, 40);
				this.upperLeftPanel.TabIndex = 0;
				// 
				// upperRightPanel
				// 
				this.upperRightPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
				this.upperRightPanel.Controls.AddRange(new System.Windows.Forms.Control[] {
																					 this.upperRightLine});
				this.upperRightPanel.Location = new System.Drawing.Point(44, 2);
				this.upperRightPanel.Name = "upperRightPanel";
				this.upperRightPanel.Size = new System.Drawing.Size(40, 40);
				this.upperRightPanel.TabIndex = 1;
				// 
				// lowerLeftPanel
				// 
				this.lowerLeftPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
				this.lowerLeftPanel.Controls.AddRange(new System.Windows.Forms.Control[] {
																					 this.lowerLeftLine});
				this.lowerLeftPanel.Location = new System.Drawing.Point(86, 2);
				this.lowerLeftPanel.Name = "lowerLeftPanel";
				this.lowerLeftPanel.Size = new System.Drawing.Size(40, 40);
				this.lowerLeftPanel.TabIndex = 2;
				// 
				// lowerRightPanel
				// 
				this.lowerRightPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
				this.lowerRightPanel.Controls.AddRange(new System.Windows.Forms.Control[] {
																					 this.lowerRighttLine});
				this.lowerRightPanel.Location = new System.Drawing.Point(128, 2);
				this.lowerRightPanel.Name = "lowerRightPanel";
				this.lowerRightPanel.Size = new System.Drawing.Size(40, 40);
				this.lowerRightPanel.TabIndex = 3;
				// 
				// upperLeftLine
				// 
				this.upperLeftLine.Dock = System.Windows.Forms.DockStyle.Fill;
				this.upperLeftLine.CurveType = Microsoft.Samples.CurvedLineTypes.UpperLeftQuarterCirle;
				this.upperLeftLine.Name = "upperLeftLine";
				this.upperLeftLine.Size = new System.Drawing.Size(38, 38);
				this.upperLeftLine.TabIndex = 0;
				this.upperLeftLine.Text = "CurvedLine1";
				this.upperLeftLine.Click += new EventHandler(this.button_Click);
				// 
				// upperRightLine
				// 
				this.upperRightLine.Dock = System.Windows.Forms.DockStyle.Fill;
				this.upperRightLine.CurveType = Microsoft.Samples.CurvedLineTypes.UpperRightQuarterCirle;
				this.upperRightLine.Name = "upperRightLine";
				this.upperRightLine.Size = new System.Drawing.Size(38, 38);
				this.upperRightLine.TabIndex = 0;
				this.upperRightLine.Text = "CurvedLine2";
				this.upperRightLine.Click += new EventHandler(this.button_Click);
				// 
				// lowerLeftLine
				// 
				this.lowerLeftLine.Dock = System.Windows.Forms.DockStyle.Fill;
				this.lowerLeftLine.CurveType = Microsoft.Samples.CurvedLineTypes.LowerLeftQuarterCirle;
				this.lowerLeftLine.Name = "lowerLeftLine";
				this.lowerLeftLine.Size = new System.Drawing.Size(38, 38);
				this.lowerLeftLine.TabIndex = 0;
				this.lowerLeftLine.Text = "CurvedLine3";
				this.lowerLeftLine.Click += new EventHandler(this.button_Click);
				// 
				// lowerRighttLine
				// 
				this.lowerRighttLine.Dock = System.Windows.Forms.DockStyle.Fill;
				this.lowerRighttLine.CurveType = Microsoft.Samples.CurvedLineTypes.LowerRightQuarterCircle;
				this.lowerRighttLine.Name = "lowerRighttLine";
				this.lowerRighttLine.Size = new System.Drawing.Size(38, 38);
				this.lowerRighttLine.TabIndex = 0;
				this.lowerRighttLine.Text = "CurvedLine4";
				this.lowerRighttLine.Click += new EventHandler(this.button_Click);
				// 
				// UserControl1
				// 
				this.Controls.AddRange(new System.Windows.Forms.Control[] {
																			  this.lowerRightPanel,
																			  this.lowerLeftPanel,
																			  this.upperRightPanel,
																			  this.upperLeftPanel});
				this.Name = "UserControl1";
				this.Size = new System.Drawing.Size(164, 44);
				this.BackColor = System.Drawing.SystemColors.InactiveBorder;
				this.upperLeftPanel.ResumeLayout(false);
				this.upperRightPanel.ResumeLayout(false);
				this.lowerLeftPanel.ResumeLayout(false);
				this.lowerRightPanel.ResumeLayout(false);
				this.ResumeLayout(false);
			}
			#endregion
		}
		#endregion
	}
	#endregion
}
