using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace Microsoft.Samples
{
	/// <summary>
	/// Summary description for CurvedLine.
	/// </summary>
	public class CurvedLine : LineBase
	{
		private CurvedLineTypes curveType;

		public CurvedLine() : base()
		{
			curveType = CurvedLineTypes.UpperLeftQuarterCirle;
		}

		[
		Category("Line Properties"),
		DefaultValue(typeof(CurvedLineTypes), "CurvedLineTypes.LowerLeftQuarterCirle"),
		Description("Specifies the type of curve the control will display")
		]
		public CurvedLineTypes CurveType
		{
			get 
			{
				return curveType;
			}
			set 
			{
				curveType = value;
				Invalidate();
			}
		}

		protected override void LineBase_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			int doubleWidth = Width * 2;
			int doubleHeight = Height * 2;
			Rectangle bounds = new Rectangle(0, 0, Width * 2, Height * 2);
			bounds.Inflate(-Thickness / 2, -Thickness / 2);
			pen = new Pen(ForeColor, Thickness);

			if (AntiAlias) 
			{
				e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
			}

			switch (curveType) 
			{
				case CurvedLineTypes.UpperLeftQuarterCirle:
					e.Graphics.DrawArc(pen, bounds, 180F, 90F);
					break;
				case CurvedLineTypes.UpperRightQuarterCirle:
					bounds.Offset(-Width, 0);
					e.Graphics.DrawArc(pen, bounds, 270F, 90F);
					break;
				case CurvedLineTypes.LowerLeftQuarterCirle:
					bounds.Offset(0, -Height);
					e.Graphics.DrawArc(pen, bounds, 90F, 90F);
					break;
				case CurvedLineTypes.LowerRightQuarterCircle:
					bounds.Offset(-Width, -Height);
					e.Graphics.DrawArc(pen, bounds, 0 ,90);
					break;

				default:break;
			}
		}
	}
}
