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
	// Used by the CurvedPanel to specify whether each corner 
	// will be an arc or a line.

	[Editor("Microsoft.Samples.CornerTypeEditor" , typeof(UITypeEditor))]
	public enum CornerTypes
	{
		Curve,
		Line,
	}

	// The UITypeEditor for the CornerTypes enum
	#region UITypeEditor
	public class CornerTypeEditor : UITypeEditor 
	{
		private CornerTypeUI lineTypeUI;

		public override object EditValue(ITypeDescriptorContext context,  IServiceProvider  provider, object value) 
		{
			object returnValue = value;
			if (provider != null) 
			{
				IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
				if (edSvc != null) 
				{
					if (lineTypeUI == null) 
					{
						lineTypeUI = new CornerTypeUI(this);
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

		#region Control Used in the UITypeEditor
		private class CornerTypeUI : Control 
		{
			private IWindowsFormsEditorService edSvc;
			private CornerTypeEditor editor = null;
			private CornerTypes oldCornerType;
			private object value;

			public CornerTypeUI(CornerTypeEditor editor) 
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
				this.oldCornerType = (CornerTypes)value;

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
				
				switch ((CornerTypes)value) 
				{
					case CornerTypes.Curve: 
						this.curvedPanel.BackColor = SystemColors.ControlText;
						this.curvedPanel.ForeColor = SystemColors.Control;
						break;
					case CornerTypes.Line:
						this.linePanel.BackColor = SystemColors.ControlText;
						this.linePanel.ForeColor = SystemColors.Control;
						break;
					default: break;
				}
			}

			private void Teardown(bool save) 
			{
				if (!save) 
				{
					value = oldCornerType;
				}
				edSvc.CloseDropDown();
			}

			private void button_Click(object sender, System.EventArgs e)
			{
				if ((sender as CurvedLine) == null) 
				{
					this.value = CornerTypes.Line;
				}
				else 
				{
					this.value = CornerTypes.Curve;
				}
				Teardown(true);
			}

			#region Component Designer generated code

			private System.Windows.Forms.Panel curvedPanel;
			private System.Windows.Forms.Panel linePanel;
			private Microsoft.Samples.CurvedLine curve;
			private Microsoft.Samples.StraightLine line;

			internal virtual void InitializeComponent() 
			{
				this.curvedPanel = new System.Windows.Forms.Panel();
				this.linePanel = new System.Windows.Forms.Panel();
				this.curve = new Microsoft.Samples.CurvedLine();
				this.line = new Microsoft.Samples.StraightLine();
				this.curvedPanel.SuspendLayout();
				this.linePanel.SuspendLayout();
				this.SuspendLayout();
				// 
				// curvedPanel
				// 
				this.curvedPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
				this.curvedPanel.Controls.AddRange(new System.Windows.Forms.Control[] {
																							 this.curve});
				this.curvedPanel.Location = new System.Drawing.Point(2, 2);
				this.curvedPanel.Name = "curvedPanel";
				this.curvedPanel.Size = new System.Drawing.Size(40, 40);
				this.curvedPanel.TabIndex = 0;
				// 
				// linePanel
				// 
				this.linePanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
				this.linePanel.Controls.AddRange(new System.Windows.Forms.Control[] {
																							  this.line});
				this.linePanel.Location = new System.Drawing.Point(44, 2);
				this.linePanel.Name = "linePanel";
				this.linePanel.Size = new System.Drawing.Size(40, 40);
				this.linePanel.TabIndex = 1;
				// 
				// curve
				// 
				this.curve.Dock = System.Windows.Forms.DockStyle.Fill;
				this.curve.CurveType = Microsoft.Samples.CurvedLineTypes.UpperLeftQuarterCirle;
				this.curve.Name = "curve";
				this.curve.Size = new System.Drawing.Size(38, 38);
				this.curve.TabIndex = 0;
				this.curve.Click += new EventHandler(this.button_Click);
				// 
				// line
				// 
				this.line.Dock = System.Windows.Forms.DockStyle.Fill;
				this.line.LineType = Microsoft.Samples.StraightLineTypes.DiagonalAscending;
				this.line.Name = "upperRightLine";
				this.line.Size = new System.Drawing.Size(38, 38);
				this.line.TabIndex = 0;
				this.line.Click += new EventHandler(this.button_Click);
				// 
				// UserControl1
				// 
				this.Controls.AddRange(new System.Windows.Forms.Control[] {
																			  this.curvedPanel,
																			  this.linePanel});
				this.Name = "UserControl1";
				this.Size = new System.Drawing.Size(164, 44);
				this.BackColor = System.Drawing.SystemColors.InactiveBorder;
				this.curvedPanel.ResumeLayout(false);
				this.linePanel.ResumeLayout(false);
				this.ResumeLayout(false);
			}
			#endregion
		}
		#endregion
	}
	#endregion
}
