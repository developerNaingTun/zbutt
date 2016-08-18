using System.Design;
using System.ComponentModel;
using System;
using System.Collections;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;


namespace Microsoft.Samples
{
	/// <summary>
	/// Summary description for CurvedPanelDesigner.
	/// </summary>
	public class CurvedPanelDesigner : ParentControlDesigner
	{
		private DesignerVerbCollection verbs;
		private Cursor localCursor;
		private bool cursorInUpperLeft, cursorInUpperRight, cursorInLowerLeft, cursorInLowerRight;
		Rectangle upperLeft, upperRight, lowerRight, lowerLeft;
		
		CurvedPanel curvedPanel;
		Rectangle bounds;

		Point mouseStart;
		int originalRadius;

		///     Returns the design-time verbs supported by the component associated with
		///     the customizer. The verbs returned by this method are typically displayed
		///     in a right-click menu by the design-time environment. The return value may
		///     be null if the component has no design-time verbs. When a user selects one
		///     of the verbs, the performVerb() method is invoked with the the
		///     corresponding DesignerVerb object.
		///     NOTE: A design-time environment will typically provide a "Properties..."
		///     entry on a component's right-click menu. The getVerbs() method should
		///     therefore not include such an entry in the returned list of verbs.
		public override DesignerVerbCollection Verbs 
		{
			get 
			{
				if (verbs == null) 
				{
					verbs = new DesignerVerbCollection();
					verbs.Add(new DesignerVerb(("Increase Margins"), new EventHandler(this.OnIncreaseMargins)));
					verbs.Add(new DesignerVerb(("Decrease Margins"), new EventHandler(this.OnDecreaseMargins)));
					verbs.Add(new DesignerVerb(("Reset Margins"), new EventHandler(this.OnResetMargins)));
					verbs.Add(new DesignerVerb(("Increase Line Thickness"), new EventHandler(this.OnIncreaseLineThickness)));
					verbs.Add(new DesignerVerb(("Decrease Line Thickness"), new EventHandler(this.OnDecreaseLineThickness)));
					verbs.Add(new DesignerVerb(("Reset Line Thickness"), new EventHandler(this.OnResetLineThickness)));
					verbs.Add(new DesignerVerb(("Reset Corners"), new EventHandler(this.OnResetCorners)));
				}

				return verbs;
			}
		}

		private int CalculateOffset(int radius) 
		{
			int realRadius = radius / 2;
			return realRadius - (int)Math.Sqrt((double)((realRadius * realRadius) / 2));
		}

		// This method makes sure the radius of a corner isn't greater
		// than half the smaller of the height or width.
		//
		// Called when creating the GraphicsPath to render.
		// This way, only the rendering reflects the size limitation
		// and the intrinsic value the user set is maintained.
		private int CheckRadius(int radius, Rectangle bounds) 
		{
			if (bounds.Height <= bounds.Width) 
			{
				if (radius > bounds.Height) 
				{
					return bounds.Height;
				}
			}
			else 
			{
				if (radius > bounds.Width) 
				{
					return bounds.Width;
				}
			}
			return radius;
		}

		///     Allows your component to support a design time user interface.  A TabStrip
		///     control, for example, has a design time user interface that allows the user
		///     to click the tabs to change tabs.  To implement this, TabStrip returns
		///     true whenever the given point is within its tabs.
		protected override bool GetHitTest(Point point) 
		{
			bool needsInvalidate = false;
			
			Point clientPoint = Control.PointToClient(point);
			MakeGrabRectangles();			

			if (upperLeft.Contains(clientPoint)) 
			{
				if (!cursorInUpperLeft) 
				{
					localCursor = Cursors.SizeNWSE;
					cursorInUpperLeft = true;
					needsInvalidate = true;
				}
			}
			else
			{
				if (cursorInUpperLeft) 
				{
					localCursor = null;
					cursorInUpperLeft = false;
					needsInvalidate = true;
				}
			}
				
			if (upperRight.Contains(clientPoint)) 
			{
				if (!cursorInUpperRight) 
				{
					localCursor = Cursors.SizeNESW;
					cursorInUpperRight = true;
					needsInvalidate = true;
				}
			}
			else
			{
				if (cursorInUpperRight) 
				{
					localCursor = null;
					cursorInUpperRight = false;
					needsInvalidate = true;
				}
			}
			
			if (lowerRight.Contains(clientPoint)) 
			{
				if (!cursorInLowerRight) 
				{
					localCursor = Cursors.SizeNWSE;
					cursorInLowerRight = true;
					needsInvalidate = true;
				}
			}
			else
			{
				if (cursorInLowerRight) 
				{
					localCursor = null;
					cursorInLowerRight = false;
					needsInvalidate = true;
				}
			}
			
			if (lowerLeft.Contains(clientPoint)) 
			{
				if (!cursorInLowerLeft) 
				{
					localCursor = Cursors.SizeNESW;
					cursorInLowerLeft = true;
					needsInvalidate = true;
				}
			}
			else
			{
				if (cursorInLowerLeft) 
				{
					localCursor = null;
					cursorInLowerLeft = false;
					needsInvalidate = true;
				}
			}

			if (needsInvalidate) 
			{
				Control.Invalidate();
			}
			return false;
		}

		public override void Initialize(IComponent component) 
		{
			base.Initialize(component);
			curvedPanel = Control as CurvedPanel;
			if (curvedPanel == null) 
			{
				throw new Exception("The control this CurvedPanelDesigner is attatched to is not a CurvedPanel.");
			}

			// Set fields
			localCursor = null;
			MemberDescriptor member = TypeDescriptor.GetProperties(Control)["Controls"];
			cursorInUpperLeft = cursorInUpperRight = cursorInLowerLeft = cursorInLowerRight = false;
			MakeGrabRectangles();
		}

		private void MakeGrabRectangles() 
		{
			bounds = new Rectangle(curvedPanel.LeftMargin, curvedPanel.TopMargin, curvedPanel.Width - curvedPanel.LeftMargin - curvedPanel.RightMargin, curvedPanel.Height - curvedPanel.TopMargin - curvedPanel.BottomMargin);

			// Calculate radius for painting
			// if radius is greater than half the minimum of the height or width of the bounds, truncate
			int paintUpperLeftCornerRadius = CheckRadius(curvedPanel.UpperLeftCornerRadius, bounds);
			int paintUpperRightCornerRadius = CheckRadius(curvedPanel.UpperRightCornerRadius, bounds);
			int paintLowerLeftCornerRadius = CheckRadius(curvedPanel.LowerLeftCornerRadius, bounds);
			int paintLowerRightCornerRadius = CheckRadius(curvedPanel.LowerRightCornerRadius, bounds);

			if (curvedPanel.UpperLeftCornerType == CornerTypes.Line) 
			{
				upperLeft = new Rectangle((bounds.X + paintUpperLeftCornerRadius / 4) - 10, (bounds.Y + paintUpperLeftCornerRadius / 4) - 10, 20, 20);
			}
			else 
			{
				int offset = CalculateOffset(paintUpperLeftCornerRadius);
				upperLeft = new Rectangle((bounds.X + offset) - 10, (bounds.Y + offset) - 10, 20, 20);
			}
			
			if (curvedPanel.UpperRightCornerType == CornerTypes.Line) 
			{
				upperRight = new Rectangle((bounds.Right - paintUpperRightCornerRadius / 4) - 10, (bounds.Y + paintUpperRightCornerRadius / 4) - 10, 20, 20);
			}
			else 
			{
				int offset = CalculateOffset(paintUpperRightCornerRadius);
				upperRight = new Rectangle((bounds.Right - offset) - 10, (bounds.Y + offset) - 10, 20, 20);
			}

			if (curvedPanel.LowerLeftCornerType == CornerTypes.Line) 
			{
				lowerLeft = new Rectangle((bounds.X + paintLowerLeftCornerRadius / 4) - 10, (bounds.Bottom - paintLowerLeftCornerRadius / 4) - 10, 20, 20);
			}
			else 
			{
				int offset = CalculateOffset(paintLowerLeftCornerRadius);
				lowerLeft = new Rectangle((bounds.X + offset) - 10, (bounds.Bottom - offset) - 10, 20, 20);
			}

			if (curvedPanel.LowerRightCornerType == CornerTypes.Line) 
			{
				lowerRight = new Rectangle((bounds.Right - paintLowerRightCornerRadius / 4) - 10, (bounds.Bottom - paintLowerRightCornerRadius / 4) - 10, 20, 20);
			}
			else 
			{
				int offset = CalculateOffset(paintLowerRightCornerRadius);
				lowerRight = new Rectangle((bounds.Right - offset) - 10, (bounds.Bottom - offset) - 10, 20, 20);
			}
		}

		///      Called in response to a verb to decrease the line thickness.  This decreases
		///      the line thickness by 1 pixel.
		private void OnDecreaseLineThickness(object sender, EventArgs e) 
		{
			IDesignerHost host = (IDesignerHost)GetService(typeof(IDesignerHost));
			if (host != null) 
			{
				DesignerTransaction t = null;
				try 
				{
					t = host.CreateTransaction(curvedPanel.Name + " decrease line thickness");

					PropertyDescriptor prop;
					prop = TypeDescriptor.GetProperties(curvedPanel)["LineThickness"];
					prop.SetValue(curvedPanel, curvedPanel.LineThickness - 1);
				}
				finally 
				{
					if (t != null)
						t.Commit();
				}
			}
		}

		///      Called in response to a verb to decrease the margins.  This decrease
		///      margins by 1 pixel on each side.
		private void OnDecreaseMargins(object sender, EventArgs e) 
		{
			IDesignerHost host = (IDesignerHost)GetService(typeof(IDesignerHost));
			if (host != null) 
			{
				DesignerTransaction t = null;
				try 
				{
					t = host.CreateTransaction(curvedPanel.Name + " decrease margins");

					PropertyDescriptor prop;
					prop = TypeDescriptor.GetProperties(curvedPanel)["TopMargin"];
					prop.SetValue(curvedPanel, curvedPanel.TopMargin - 1);
					prop = TypeDescriptor.GetProperties(curvedPanel)["LeftMargin"];
					prop.SetValue(curvedPanel, curvedPanel.LeftMargin - 1);
					prop = TypeDescriptor.GetProperties(curvedPanel)["BottomMargin"];
					prop.SetValue(curvedPanel, curvedPanel.BottomMargin - 1);
					prop = TypeDescriptor.GetProperties(curvedPanel)["RightMargin"];
					prop.SetValue(curvedPanel, curvedPanel.RightMargin - 1);
				}
				finally 
				{
					if (t != null)
						t.Commit();
				}
			}
		}

		///      Called in response to a verb to increase the line thickness.  This increases
		///      the line thickness by 1 pixel.
		private void OnIncreaseLineThickness(object sender, EventArgs e) 
		{
			IDesignerHost host = (IDesignerHost)GetService(typeof(IDesignerHost));
			if (host != null) 
			{
				DesignerTransaction t = null;
				try 
				{
					t = host.CreateTransaction(curvedPanel.Name + " increase line thickness");

					PropertyDescriptor prop;
					prop = TypeDescriptor.GetProperties(curvedPanel)["LineThickness"];
					prop.SetValue(curvedPanel, curvedPanel.LineThickness + 1);
				}
				finally 
				{
					if (t != null)
						t.Commit();
				}
			}
		}

		/// <include file='doc\TabControlDesigner.uex' path='docs/doc[@for="TabControlDesigner.OnAdd"]/*' />
		/// <devdoc>
		///      Called in response to a verb to increase the margins.  This increases
		///      margins by 1 pixel on each side.
		/// </devdoc>
		private void OnIncreaseMargins(object sender, EventArgs e) 
		{
			IDesignerHost host = (IDesignerHost)GetService(typeof(IDesignerHost));
			if (host != null) 
			{
				DesignerTransaction t = null;
				try 
				{
					t = host.CreateTransaction(curvedPanel.Name + " increase margins");

					PropertyDescriptor prop;
					prop = TypeDescriptor.GetProperties(curvedPanel)["TopMargin"];
					prop.SetValue(curvedPanel, curvedPanel.TopMargin + 1);
					prop = TypeDescriptor.GetProperties(curvedPanel)["LeftMargin"];
					prop.SetValue(curvedPanel, curvedPanel.LeftMargin + 1);
					prop = TypeDescriptor.GetProperties(curvedPanel)["BottomMargin"];
					prop.SetValue(curvedPanel, curvedPanel.BottomMargin + 1);
					prop = TypeDescriptor.GetProperties(curvedPanel)["RightMargin"];
					prop.SetValue(curvedPanel, curvedPanel.RightMargin + 1);
				}
				finally 
				{
					if (t != null)
						t.Commit();
				}
			}
		}

		///      Called in response to a verb to reset the line thickness.  This resets
		///      the line thickness to 1 pixel.
		private void OnResetLineThickness(object sender, EventArgs e) 
		{
			IDesignerHost host = (IDesignerHost)GetService(typeof(IDesignerHost));
			if (host != null) 
			{
				DesignerTransaction t = null;
				try 
				{
					t = host.CreateTransaction(curvedPanel.Name + " reset LineThickness");

					PropertyDescriptor prop;
					prop = TypeDescriptor.GetProperties(curvedPanel)["LineThickness"];
					prop.SetValue(curvedPanel, 1);
				}
				finally 
				{
					if (t != null)
						t.Commit();
				}
			}
		}

		///      Called in response to a verb to reset the margins.  This sets each
		///      margin to 5 pixels.
		private void OnResetMargins(object sender, EventArgs e) 
		{
			IDesignerHost host = (IDesignerHost)GetService(typeof(IDesignerHost));
			if (host != null) 
			{
				DesignerTransaction t = null;
				try 
				{
					t = host.CreateTransaction(curvedPanel.Name + " reset margins");

					PropertyDescriptor prop;
					prop = TypeDescriptor.GetProperties(curvedPanel)["TopMargin"];
					prop.SetValue(curvedPanel, 5);
					prop = TypeDescriptor.GetProperties(curvedPanel)["LeftMargin"];
					prop.SetValue(curvedPanel, 5);
					prop = TypeDescriptor.GetProperties(curvedPanel)["BottomMargin"];
					prop.SetValue(curvedPanel, 5);
					prop = TypeDescriptor.GetProperties(curvedPanel)["RightMargin"];
					prop.SetValue(curvedPanel, 5);
				}
				finally 
				{
					if (t != null)
						t.Commit();
				}
			}
		}

		DesignerTransaction trans;
		protected override void OnMouseDragBegin(int x, int y) 
		{
			// Setup designer transaction
			IDesignerHost host = (IDesignerHost)GetService(typeof(IDesignerHost));
			if (host != null) 
			{
				trans = host.CreateTransaction(curvedPanel.Name + " Curve radius drag");
			}

			if (cursorInUpperLeft || cursorInUpperRight || cursorInLowerLeft || cursorInLowerRight) 
			{
				mouseStart = new Point(x, y);
				Control.Capture = true;
				
				if (cursorInUpperLeft) 
				{
					originalRadius = curvedPanel.UpperLeftCornerRadius;
				}

				if (cursorInUpperRight) 
				{
					originalRadius = curvedPanel.UpperRightCornerRadius;
				}

				if (cursorInLowerLeft) 
				{
					originalRadius = curvedPanel.LowerLeftCornerRadius;
				}

				if (cursorInLowerRight) 
				{
					originalRadius = curvedPanel.LowerRightCornerRadius;
				}
			}
			else 
			{
				base.OnMouseDragBegin(x, y);
			}
		}

		protected override void OnMouseDragEnd(bool cancel) 
		{
			if (!(cursorInUpperLeft || cursorInUpperRight || cursorInLowerLeft || cursorInLowerRight) ) 
			{
				base.OnMouseDragEnd(cancel);
			}
			else 
			{
				Control.Capture = false;
				if (trans != null) 
				{
					trans.Commit();
					trans = null;
				}
			}
		}

		protected override void OnMouseDragMove(int x, int y) 
		{
			if (!(cursorInUpperLeft || cursorInUpperRight || cursorInLowerLeft || cursorInLowerRight) ) 
			{
				base.OnMouseDragMove(x, y);
			}
			else 
			{
				PropertyDescriptor prop;
				
				if (curvedPanel != null) 
				{
					int delta = (mouseStart.X - x) * (mouseStart.X - x) +  (mouseStart.Y - y) * (mouseStart.Y - y);
					delta = (int)Math.Sqrt((double)delta);
					if (cursorInUpperLeft) 
					{
						if (((mouseStart.X - x) < 0) && ((mouseStart.Y - y) < 0)) 
						{
							prop = TypeDescriptor.GetProperties(curvedPanel)["UpperLeftCornerRadius"];
							prop.SetValue(curvedPanel, CheckRadius(originalRadius + delta * 2, bounds));
						}
						else if (((mouseStart.X - x) > 0) && ((mouseStart.Y - y) > 0)) 
						{
							prop = TypeDescriptor.GetProperties(curvedPanel)["UpperLeftCornerRadius"];
							prop.SetValue(curvedPanel, CheckRadius(originalRadius - delta * 2, bounds));
						}
					}
					else if (cursorInUpperRight) 
					{
						if (((mouseStart.X - x) > 0) && ((mouseStart.Y - y) < 0)) 
						{
							prop = TypeDescriptor.GetProperties(curvedPanel)["UpperRightCornerRadius"];
							prop.SetValue(curvedPanel, CheckRadius(originalRadius + delta * 2, bounds));
						}
						else if (((mouseStart.X - x) < 0) && ((mouseStart.Y - y) > 0)) 
						{
							prop = TypeDescriptor.GetProperties(curvedPanel)["UpperRightCornerRadius"];
							prop.SetValue(curvedPanel, CheckRadius(originalRadius - delta * 2, bounds));
						}
					}
					else if (cursorInLowerLeft) 
					{
						if (((mouseStart.X - x) < 0) && ((mouseStart.Y - y) > 0)) 
						{
							prop = TypeDescriptor.GetProperties(curvedPanel)["LowerLeftCornerRadius"];
							prop.SetValue(curvedPanel, CheckRadius(originalRadius + delta * 2, bounds));
						}
						else if (((mouseStart.X - x) > 0) && ((mouseStart.Y - y) < 0)) 
						{
							prop = TypeDescriptor.GetProperties(curvedPanel)["LowerLeftCornerRadius"];
							prop.SetValue(curvedPanel, CheckRadius(originalRadius - delta * 2, bounds));
						}
					}
					else if (cursorInLowerRight) 
					{
						if (((mouseStart.X - x) > 0) && ((mouseStart.Y - y) > 0)) 
						{
							prop = TypeDescriptor.GetProperties(curvedPanel)["LowerRightCornerRadius"];
							prop.SetValue(curvedPanel, CheckRadius(originalRadius + delta * 2, bounds));
						}
						else if (((mouseStart.X - x) < 0) && ((mouseStart.Y - y) < 0)) 
						{
							prop = TypeDescriptor.GetProperties(curvedPanel)["LowerRightCornerRadius"];
							prop.SetValue(curvedPanel, CheckRadius(originalRadius - delta * 2, bounds));
						}
					}
				}
			}
		}

		///      Called in response to a verb to reset the controls corners.
		private void OnResetCorners(object sender, EventArgs e) 
		{
			IDesignerHost host = (IDesignerHost)GetService(typeof(IDesignerHost));
			if (host != null) 
			{
				DesignerTransaction t = null;
				try 
				{
					t = host.CreateTransaction(curvedPanel.Name + " reset margins");

					PropertyDescriptor prop;

					// Reset corner radii
					prop = TypeDescriptor.GetProperties(curvedPanel)["UpperLeftCornerRadius"];
					prop.SetValue(curvedPanel, 50);
					prop = TypeDescriptor.GetProperties(curvedPanel)["UpperRightCornerRadius"];
					prop.SetValue(curvedPanel, 50);
					prop = TypeDescriptor.GetProperties(curvedPanel)["LowerLeftCornerRadius"];
					prop.SetValue(curvedPanel, 50);
					prop = TypeDescriptor.GetProperties(curvedPanel)["LowerRightCornerRadius"];
					prop.SetValue(curvedPanel, 50);

					// Reset corner types
					prop = TypeDescriptor.GetProperties(curvedPanel)["UpperLeftCornerType"];
					prop.SetValue(curvedPanel, CornerTypes.Curve);
					prop = TypeDescriptor.GetProperties(curvedPanel)["UpperRightCornerType"];
					prop.SetValue(curvedPanel, CornerTypes.Curve);
					prop = TypeDescriptor.GetProperties(curvedPanel)["LowerLeftCornerType"];
					prop.SetValue(curvedPanel, CornerTypes.Curve);
					prop = TypeDescriptor.GetProperties(curvedPanel)["LowerRightCornerType"];
					prop.SetValue(curvedPanel, CornerTypes.Curve);
				}
				finally 
				{
					if (t != null)
						t.Commit();
				}
			}
		}

		///     Called each time the cursor needs to be set.  The ParentControlDesigner behavior here
		///     will set the cursor to one of three things:
		///     1.  If the toolbox service has a tool selected, it will allow the toolbox service to
		///     set the cursor.
		///     2.  The arrow will be set.  Parent controls allow dragging within their interior.
		protected override void OnSetCursor() 
		{
			// If we've got a cursor for resizing the corner radii, use it.
			// If not, use the default cursor
			if (localCursor == null) 
			{
				base.OnSetCursor();
			}
			else 
			{
				Cursor.Current = localCursor;
			}
		}
	}
}
