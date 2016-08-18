using System;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Drawing.Design;


namespace Microsoft.Samples
{
	/// <summary>
	/// Summary description for IRegionControl.
	/// </summary>
	public interface IRegionControl
	{
		// This property indicates whether the control's region should be
		// added to the Form's
		bool AddToFormRegion { get; set; }

		// This method returns a properly offset Region object
		// based on the control's shape.
		//
		// Properly offset means offset in relation to the form border's
		// Top and Left coordinates.
		//
		// The code to offset might look something like:
		//
		//		Point formClientScreenLocation = parent.PointToScreen(new Point(parent.ClientRectangle.Left, parent.ClientRectangle.Top));
		//		int x = formClientScreenLocation.X - parent.DesktopLocation.X + this.Location.X;
		//		int y = formClientScreenLocation.Y - parent.DesktopLocation.Y + this.Location.Y;
		//		region.Translate(x, y);
		//
		Region MakeRegion(Form parent);
	}
}
