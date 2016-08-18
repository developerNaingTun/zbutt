using System;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms.Design;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace Microsoft.Samples
{
	/// <summary>
	/// Summary description for StraightLine.
	/// </summary>
	public class StraightLine : LineBase
	{
		private StraightLineTypes lineType;

		public StraightLine() : base()
		{
			lineType = StraightLineTypes.Horizontal;
		}

		[
		Category("Line Properties"),
		DefaultValue(typeof(StraightLineTypes), "StraightLineTypes.Horizontal"),
		Description("Specifies the type of line the control will display")
		]
		public StraightLineTypes LineType
		{
			get 
			{
				return lineType;
			}
			set 
			{
				lineType = value;
				Invalidate();
			}
		}

		protected override void LineBase_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			pen = new Pen(ForeColor, Thickness);

			if (AntiAlias) 
			{
				e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
			}

			switch (lineType) 
			{
				case StraightLineTypes.Horizontal: 
					DrawCenteredHorizontalLine(e.Graphics);
					break;
				case StraightLineTypes.Vertical:
					DrawCenteredVerticalLine(e.Graphics);
					break;
				case StraightLineTypes.DiagonalAscending:
					DrawCenteredDiagonalAscendingLine(e.Graphics);
					break;
				case StraightLineTypes.DiagonalDescending:
					DrawCenteredDiagonalDescendingLine(e.Graphics);
					break;
				default: break;
			}
		}

		private void DrawCenteredHorizontalLine(Graphics g) 
		{
			g.DrawLine(pen, 0, Height / 2, Width, Height / 2);
		}

		private void DrawCenteredVerticalLine(Graphics g) 
		{
			g.DrawLine(pen, Width / 2, 0, Width / 2, Height);
		}

		private void DrawCenteredDiagonalAscendingLine(Graphics g) 
		{
			g.DrawLine(pen, 0, Height, Width, 0);
		}

		private void DrawCenteredDiagonalDescendingLine(Graphics g) 
		{
			g.DrawLine(pen, 0, 0, Width, Height);
		}
	}
}
