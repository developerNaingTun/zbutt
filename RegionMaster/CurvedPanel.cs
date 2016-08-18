using System;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Drawing.Design;
using System.Collections;
using System.ComponentModel;


namespace Microsoft.Samples
{
	/// <summary>
	/// Summary description for CurvedPanel.
	/// </summary>
	///

	[
	Designer(typeof(CurvedPanelDesigner)),
	ToolboxItem(true),
	ToolboxBitmap(typeof(Microsoft.Samples.CurvedPanel),"Microsoft.Samples.CurvedPanel.bmp")
	]
	public class CurvedPanel : Panel, IRegionControl
	{
		#region Fields
		
		private bool addToFormRegion;
		private bool antiAlias;
		private int bottomMargin;
		private bool drawBorder;
		private Color fillColor;
		private Image fillImage;
		private double fillTransparency;
		private FormBehaviors formBehavior;
		private int formBehaviorGlyphSize;
		private int leftMargin;
		private int lineThickness;
		private int lowerLeftCornerRadius;
		private CornerTypes lowerLeftCornerType;
		private int lowerRightCornerRadius;
		private CornerTypes lowerRightCornerType;
		private int rightMargin;
		private float rotationAngle;
		private bool showFormBehaviorGlyph;
		private int topMargin;
		private int upperLeftCornerRadius;
		private CornerTypes upperLeftCornerType;
		private int upperRightCornerRadius;
		private CornerTypes upperRightCornerType;

		private GraphicsPath path;
		private Pen pen;

		private Form parentForm;
		private Point formMove;
		private bool mouseDown;
		private Region region;

		
		#endregion

		#region Constructor
		public CurvedPanel()
		{
			SetStyle(ControlStyles.SupportsTransparentBackColor | ControlStyles.ResizeRedraw | ControlStyles.DoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
			SetStyle(ControlStyles.Selectable, false);
			
			addToFormRegion = false;
			antiAlias = true;
			//BackColor = Color.Transparent;
			drawBorder = true;
			fillColor = SystemColors.Control;
			fillTransparency = .3;
			formBehaviorGlyphSize = 24;
			lineThickness = 1;
			lowerLeftCornerRadius = 50;
			lowerRightCornerRadius = 50;
			upperLeftCornerRadius = 50;
			upperRightCornerRadius = 50;
			bottomMargin = 5;
			leftMargin = 5;
			rightMargin = 5;
			rotationAngle = 0f;
			showFormBehaviorGlyph = true;
			topMargin = 5;

			path = new GraphicsPath();

			this.Click += new EventHandler(this.curvedPanel_Click);
			this.LocationChanged += new EventHandler(this.curvedPanel_InvalidateRegionCache);
			this.MouseDown += new MouseEventHandler(this.curvedPanel_MouseDown);
			this.MouseMove += new MouseEventHandler(this.curvedPanel_MouseMove);
			this.MouseUp += new MouseEventHandler(this.curvedPanel_MouseUp);
			this.SizeChanged += new EventHandler(this.curvedPanel_InvalidateRegionCache);
		}
		#endregion

		#region Properties
		[
		Category("Appearance"),
		DefaultValue(false),
		Description("Specifies whether this control's region becomes part of the form's shape")
		]
		public bool AddToFormRegion 
		{
			get 
			{
				return addToFormRegion;
			}
			set 
			{
				addToFormRegion = value;
				Invalidate();
			}
		}

		[
		Category("Line Properties"),
		DefaultValue(true),
		Description("Specifies whether the border is drawn with anti-aliasing")
		]
		public bool AntiAlias 
		{
			get 
			{
				return antiAlias;
			}
			set 
			{
				antiAlias = value;
				Invalidate();
			}
		}

//		[
//		Category("Appearance"),
//		DefaultValue(typeof(Color), "Color.Transparent")
//		]
//		public override Color BackColor 
//		{
//			get 
//			{
//				return base.BackColor;
//			}
//			set 
//			{
//				base.BackColor = value;
//			}
//		}

		[
		Browsable(false)
		]
		public new BorderStyle BorderStyle
		{
			get 
			{
				return base.BorderStyle;
			}
			set 
			{
				base.BorderStyle = value;
			}
		}

		[
		Category("Appearance"),
		DefaultValue(5),
		Description("Specifies the amount of space between the bottom of the control and the border")
		]
		public int BottomMargin 
		{
			get 
			{
				return bottomMargin;
			}
			set 
			{
				bottomMargin = value;
				Invalidate();
			}
		}

		[
		Category("Appearance"),
		DefaultValue(true),
		Description("Specifies whether a border is drawn on the panel")
		]
		public bool DrawBorder 
		{
			get 
			{
				return drawBorder;
			}
			set 
			{
				drawBorder = value;
				Invalidate();
			}
		}

		[
		Category("Appearance"),
		DefaultValue(typeof(Color), "SystemColors.Control"),
		Description("The color used to fill the area inside the border")
		]
		public Color FillColor 
		{
			get 
			{
				return fillColor;
			}
			set 
			{
				fillColor = value;
				Invalidate();
			}
		}

		[
		Category("Appearance"),
		DefaultValue(.3),
		TypeConverterAttribute(typeof(OpacityConverter)),
		Editor("Microsoft.Samples.PercentEditor", typeof(UITypeEditor)),
		Description("The percentage of transparency the FillColor or FillImage is drawn with")
		]
		public double FillTransparency 
		{
			get 
			{
				return fillTransparency;
			}
			set 
			{
				fillTransparency = value;

				if (fillTransparency > 1) 
				{
					fillTransparency = 1;
				}
				else if (fillTransparency < 0) 
				{
					fillTransparency = 0;
				}

				Invalidate();
			}
		}

		[
		Category("Appearance"),
		DefaultValue(null),
		Description("The image used to fill the area inside the border.  The FillImage property takes precedence over the FillColor")
		]
		public Image FillImage 
		{
			get 
			{
				return fillImage;
			}
			set 
			{
				fillImage = value;
				this.Invalidate();
			}
		}

		[
		Category("Behavior"),
		DefaultValue(typeof(FormBehaviors), "FormBehaviors.None"),
		Description("Specifies the interaction with the form when clicking on the panel.  The values None, Close, Drag, Maximize, Minimize, and ResizeLowerRight are supported")
		]
		public FormBehaviors FormBehavior 
		{
			get 
			{
				return formBehavior;
			}
			set 
			{
				formBehavior = value;
				Invalidate();
			}
		}

		[
		Category("Appearance"),
		DefaultValue(24),
		Description("Specifies the font size of the FormBehavior glyph drawn")
		]
		public int FormBehaviorGlyphSize 
		{
			get 
			{
				return formBehaviorGlyphSize;
			}
			set 
			{
				formBehaviorGlyphSize = value;
				this.Invalidate();
			}
		}

		[
		Category("Appearance"),
		DefaultValue(5),
		Description("Specifies the amount of space between the left of the control and the border")
		]
		public int LeftMargin 
		{
			get 
			{
				return leftMargin;
			}
			set 
			{
				leftMargin = value;
				Invalidate();
			}
		}

		[
		Category("Line Properties"),
		DefaultValue(1),
		Description("Specifies the thickness of the border")
		]
		public int LineThickness 
		{
			get 
			{
				return lineThickness;
			}
			set 
			{
				lineThickness = value;
				Invalidate();
			}
		}

		[
		Category("Line Properties"),
		DefaultValue(50),
		Description("Specifies the size of the lower left corner of the panel")
		]
		public int LowerLeftCornerRadius 
		{
			get 
			{
				return lowerLeftCornerRadius;
			}
			set 
			{
				lowerLeftCornerRadius = value;
				if (lowerLeftCornerRadius < 1) 
				{
					lowerLeftCornerRadius = 1;
				}
				Invalidate();
			}
		}

		[
		Category("Line Properties"),
		DefaultValue(typeof(CornerTypes), "CornerTypes.Curve"),
		Description("Specifies the type of the lower left corner of the panel")
		]
		public CornerTypes LowerLeftCornerType 
		{
			get 
			{
				return lowerLeftCornerType;
			}
			set 
			{
				lowerLeftCornerType = value;
				Invalidate();
			}
		}
		
		[
		Category("Line Properties"),
		DefaultValue(50),
		Description("Specifies the size of the lower right corner of the panel")
		]
		public int LowerRightCornerRadius 
		{
			get 
			{
				return lowerRightCornerRadius;
			}
			set 
			{
				lowerRightCornerRadius = value;
				if (lowerRightCornerRadius < 1) 
				{
					lowerRightCornerRadius = 1;
				}
				Invalidate();
			}
		}

		[
		Category("Line Properties"),
		DefaultValue(typeof(CornerTypes), "CornerTypes.Curve"),
		Description("Specifies the type of the lower right corner of the panel")
		]
		public CornerTypes LowerRightCornerType 
		{
			get 
			{
				return lowerRightCornerType;
			}
			set 
			{
				lowerRightCornerType = value;
				Invalidate();
			}
		}

		[
		Category("Appearance"),
		DefaultValue(5),
		Description("Specifies the amount of space between the right of the control and the border")
		]
		public int RightMargin 
		{
			get 
			{
				return rightMargin;
			}
			set 
			{
				rightMargin = value;
				Invalidate();
			}
		}

		[
		Category("Appearance"),
		DefaultValue(0f),
		Description("Spcifies the angle (in degrees) the panel is rotated")
		]
		public float RotationAngle 
		{
			get 
			{
				return rotationAngle;
			}
			set 
			{
				rotationAngle = value;
				Invalidate();
			}
		}

		[
		Category("Appearance"),
		DefaultValue(true),
		Description("Determines whether a glyph is rendered on the panel for the chosen FormBehavior")
		]
		public bool ShowFormBehaviorGlyph 
		{
			get 
			{
				return showFormBehaviorGlyph;
			}
			set 
			{
				showFormBehaviorGlyph = value;
				Invalidate();
			}
		}

		[
		Browsable(true)
		]
		public override string Text 
		{
			get 
			{
				return base.Text;
			}
			set 
			{
				base.Text = value;
				this.Invalidate();
			}
		}

		[
		Category("Appearance"),
		DefaultValue(5),
		Description("Specifies the amount of space between the top of the control and the border")
		]
		public int TopMargin 
		{
			get 
			{
				return topMargin;
			}
			set 
			{
				topMargin = value;
				Invalidate();
			}
		}

		[
		Category("Line Properties"),
		DefaultValue(50),
		Description("Specifies the size of the upper left corner of the panel")
		]
		public int UpperLeftCornerRadius 
		{
			get 
			{
				return upperLeftCornerRadius;
			}
			set 
			{
				upperLeftCornerRadius = value;
				if (upperLeftCornerRadius < 1) 
				{
					upperLeftCornerRadius = 1;
				}
				Invalidate();
			}
		}

		[
		Category("Line Properties"),
		DefaultValue(typeof(CornerTypes), "CornerTypes.Curve"),
		Description("Specifies the type of the upper left corner of the panel")
		]
		public CornerTypes UpperLeftCornerType 
		{
			get 
			{
				return upperLeftCornerType;
			}
			set 
			{
				upperLeftCornerType = value;
				Invalidate();
			}
		}

		[
		Category("Line Properties"),
		DefaultValue(50),
		Description("Specifies the size of the upper right corner of the panel")
		]
		public int UpperRightCornerRadius 
		{
			get 
			{
				return upperRightCornerRadius;
			}
			set 
			{
				upperRightCornerRadius = value;
				if (upperRightCornerRadius < 1) 
				{
					upperRightCornerRadius = 1;
				}
				Invalidate();
			}
		}

		[
		Category("Line Properties"),
		DefaultValue(typeof(CornerTypes), "CornerTypes.Curve"),
		Description("Specifies the type of the upper right corner of the panel")
		]
		public CornerTypes UpperRightCornerType 
		{
			get 
			{
				return upperRightCornerType;
			}
			set 
			{
				upperRightCornerType = value;
				Invalidate();
			}
		}
#endregion

		#region Event Code (Painting, Layout, etc...)

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

		private void curvedPanel_Click(object sender, System.EventArgs e) 
		{
			if (parentForm == null) 
			{
				if (!GetParentForm()) 
				{
					return;
				}
			}

			if (FormBehavior == FormBehaviors.Close) 
			{
				parentForm.Close();
			}

			if (FormBehavior == FormBehaviors.Maximize)
			{
				if (parentForm.WindowState == FormWindowState.Maximized) 
				{
					parentForm.WindowState = FormWindowState.Normal;
				}
				else 
				{
					parentForm.WindowState = FormWindowState.Maximized;
				}
			}

			if (FormBehavior == FormBehaviors.Minimize)
			{
				parentForm.WindowState = FormWindowState.Minimized;
			}
		}

		private void curvedPanel_InvalidateRegionCache(object sender, EventArgs e) 
		{
			region = null;
		}

		private void curvedPanel_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if ((FormBehavior == FormBehaviors.Drag) || (FormBehavior == FormBehaviors.TitleBar) || (FormBehavior == FormBehaviors.ResizeLowerRight)) 
			{
				mouseDown = true;
				formMove = new Point(e.X,e.Y);
			}
		}

		private void curvedPanel_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (parentForm == null) 
			{
				if (!GetParentForm()) 
				{
					return;
				}
			}

			if ((FormBehavior == FormBehaviors.Drag) || (FormBehavior == FormBehaviors.TitleBar) || (FormBehavior == FormBehaviors.ResizeLowerRight)) 
			{
				if (mouseDown)
				{
					if (FormBehavior == FormBehaviors.ResizeLowerRight) 
					{
						parentForm.Width -= (formMove.X - e.X);
						parentForm.Height -= (formMove.Y - e.Y);
					}
					else 
					{
						parentForm.Location = new Point(parentForm.Left - (formMove.X - e.X), parentForm.Top - (formMove.Y - e.Y));
					}
				}
			}
		}

		private void curvedPanel_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if ((FormBehavior == FormBehaviors.Drag) || (FormBehavior == FormBehaviors.TitleBar) || (FormBehavior == FormBehaviors.ResizeLowerRight)) 
			{
				mouseDown = false;
			}
		}

		protected override void Dispose( bool disposing )
		{
			if( disposing && pen != null)
			{
				pen.Dispose();
			}
			base.Dispose( disposing );
		}

		// Recursivly walks up the parent chain to find a Form
		private bool GetParentForm() 
		{
			Control parent = this.Parent;
			while(true) 
			{
				if (parent == null) 
				{
					return false;
				}

				if ((parent as Form) != null) 
				{
					parentForm = (Form)parent;
					return true;
				}
				else 
				{
					parent = parent.Parent;
				}
			}
		}

		private GraphicsPath MakePath(int border, bool region) 
		{
			// If we have a border, we need to account for the thickness of it
			// the way that GDI+ handles drawing a GraphicsPath with a pen of a 
			// given thickness is to leak first off the bottom and right sides, 
			// then off the top and left sides.
			//
			// So if your border is 1 pixel thick, your margins will need to shrink
			// 0 pixels on the top and left and 1 pixel on the bottom and right.  
			// But if your border is 2 pixels, you'll need to shrink the margins
			// by 1 pixel on each side.

			int topLeft = border / 2;
			int bottomRight = (int)System.Math.Round((decimal) border / 2);

			Rectangle bounds;
			if (region && (lineThickness % 2 == 0))
			{
				bounds = new Rectangle(LeftMargin + topLeft, TopMargin + topLeft, (Width - LeftMargin - RightMargin - topLeft - bottomRight), (Height - topMargin - BottomMargin - topLeft - bottomRight));
			}
			else 
			{
				bounds = new Rectangle(LeftMargin + topLeft, TopMargin + topLeft, (Width - LeftMargin - RightMargin - topLeft - bottomRight) - 1, (Height - topMargin - BottomMargin - topLeft - bottomRight) - 1);
			}

			// Calculate radius for painting
			// if radius is greater than half the minimum of the height or width of the bounds, truncate
			int paintUpperLeftCornerRadius = CheckRadius(UpperLeftCornerRadius, bounds);
			int paintUpperRightCornerRadius = CheckRadius(UpperRightCornerRadius, bounds);
			int paintLowerLeftCornerRadius = CheckRadius(LowerLeftCornerRadius, bounds);
			int paintLowerRightCornerRadius = CheckRadius(LowerRightCornerRadius, bounds);
			
			// The code below creates the GraphicsPath
			// Each corner follows the below algorithm
			//
			// if (the corner's radius is greater than zero)
			//    if (the corner is a right angle)
			//		add a point
			//    if (the corner is round)
			//		add a round corner
			//    else if (the corner is a line)
		    //		add a line
			//

            GraphicsPath path = new GraphicsPath();
			if (paintUpperLeftCornerRadius >= 1) 
			{
				if (paintUpperLeftCornerRadius == 1) 
				{
					path.AddLine(bounds.X, bounds.Y, bounds.X, bounds.Y);
				}
				else if (UpperLeftCornerType == CornerTypes.Curve) 
				{
					path.AddArc(bounds.X, bounds.Y, paintUpperLeftCornerRadius, paintUpperLeftCornerRadius, 180, 90);
				}
				else 
				{
					path.AddLine(bounds.X, bounds.Y + (paintUpperLeftCornerRadius / 2), bounds.X + (paintUpperLeftCornerRadius / 2), bounds.Y);
				}
			}
			if (paintUpperRightCornerRadius >= 1) 
			{
				if (paintUpperRightCornerRadius == 1) 
				{
					path.AddLine(bounds.Right, bounds.Y, bounds.Right, bounds.Y);
				}
				else if (UpperRightCornerType == CornerTypes.Curve) 
				{
					path.AddArc(bounds.Right - (paintUpperRightCornerRadius), bounds.Y, paintUpperRightCornerRadius, paintUpperRightCornerRadius, 270, 90);
				}
				else 
				{
					path.AddLine(bounds.Right - (paintUpperRightCornerRadius / 2), bounds.Y, bounds.Right, bounds.Top + (paintUpperRightCornerRadius / 2));
				}
			}
			if (paintLowerRightCornerRadius >= 1) 
			{
				if (paintLowerRightCornerRadius == 1) 
				{
					path.AddLine(bounds.Right, bounds.Bottom, bounds.Right, bounds.Bottom);
				}
				else if (LowerRightCornerType == CornerTypes.Curve) 
				{
					path.AddArc(bounds.Right - paintLowerRightCornerRadius, bounds.Bottom - paintLowerRightCornerRadius, paintLowerRightCornerRadius, paintLowerRightCornerRadius, 0, 90);
				}
				else 
				{
					path.AddLine(bounds.Right, bounds.Bottom - (paintLowerRightCornerRadius / 2), bounds.Right - (paintLowerRightCornerRadius / 2), bounds.Bottom);
				}
			}
			if (paintLowerLeftCornerRadius >= 1) 
			{
				if (paintLowerLeftCornerRadius == 1) 
				{
					path.AddLine(bounds.X, bounds.Bottom, bounds.X, bounds.Bottom);
				}
				else if (LowerLeftCornerType == CornerTypes.Curve) 
				{
					path.AddArc(bounds.X, bounds.Bottom - paintLowerLeftCornerRadius, paintLowerLeftCornerRadius, paintLowerLeftCornerRadius, 90, 90);
				}
				else 
				{
					path.AddLine(bounds.X + (paintLowerLeftCornerRadius / 2), bounds.Bottom, bounds.X, bounds.Bottom - (paintLowerLeftCornerRadius / 2));
				}
			}

			// Calling close figure connects the corners together
			path.CloseFigure();
			
			// Deal with rotation
			Matrix matrix = new Matrix();
			matrix.RotateAt(rotationAngle, new PointF((float)this.Height / 2.0f, (float)this.Width / 2.0f));
			path.Transform(matrix);

			return path;
		}

		// This method returns the properly offset region for the panel's GraphicsPath
		public Region MakeRegion(Form parent) 
		{
			if (this.region != null) 
			{
				return this.region;
			}

			if (pen == null) 
			{
				pen = new Pen(ForeColor, lineThickness);
			}

			if (DrawBorder && (lineThickness >= 1)) 
			{
				path = MakePath(lineThickness, true);
			}
			else 
			{
				path = MakePath(0, true);
			}

			Region region = new Region(path);

			if (DrawBorder && (lineThickness >= 1)) 
			{
				GraphicsPath borderPath = (GraphicsPath)path.Clone();
				borderPath.Widen(pen);
				Region borderRegion = new Region(borderPath);
				region.Union(borderRegion);
			}
						
			Point formClientScreenLocation = parent.PointToScreen(new Point(parent.ClientRectangle.Left, parent.ClientRectangle.Top));
			int x = formClientScreenLocation.X - parent.DesktopLocation.X + this.Location.X;
			int y = formClientScreenLocation.Y - parent.DesktopLocation.Y + this.Location.Y;
			region.Translate(x, y);
			
			// Cache region
			this.region = region;

			return region;
		}

		protected override void OnPaint(System.Windows.Forms.PaintEventArgs e) 
		{
			base.OnPaint(e);
			pen = new Pen(ForeColor, lineThickness);
			
			// Enable anti-aliasing if its called for
			if (AntiAlias) 
			{
				e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
			}

			// Setup the graphics path.  If there is a border to be drawn
			// account for it in the size of the path.
			if (DrawBorder && (lineThickness >= 1)) 
			{
				path = MakePath(lineThickness, false);
			}
			else 
			{
				path = MakePath(0, false);
			}

			// If there is a fill image, fill the path with it
			// also respects the FillTransparency property
			//
			// Else fill with the FillColor property
			if (FillImage != null) 
			{
				double transparency = (1 - FillTransparency);

				ImageAttributes myImageAttributes = new System.Drawing.Imaging.ImageAttributes();
				System.Drawing.Imaging.ColorMatrix m = 
					new System.Drawing.Imaging.ColorMatrix(new float[][] {
																			 new float[] {1.0f, 0.0f, 0.0f, 0.0f, 0.0f,},
																			 new float[] {0.0f, 1.0f, 0.0f, 0.0f, 0.0f,},
																			 new float[] {0.0f, 0.0f, 1.0f, 0.0f, 0.0f,},
																			 new float[] {0.0f, 0.0f, 0.0f, 1.0f, 0.0f,},
																			 new float[] {0.0f, 0.0f, 0.0f, (float)-transparency, 0.0f,}});
				myImageAttributes.SetColorMatrix(m);
				TextureBrush brush = new TextureBrush(FillImage, new Rectangle(0, 0, FillImage.Width, FillImage.Height), myImageAttributes);
				brush.WrapMode = WrapMode.Tile;
				e.Graphics.FillPath(brush, path);
			}
			else 
			{
				if (FillColor != Color.Transparent) 
				{
					e.Graphics.FillPath(new SolidBrush(Color.FromArgb((int)(FillTransparency * 255), FillColor)), path);
				}
			}

			// If there is a border to be drawn, draw it
			if (DrawBorder && (lineThickness >= 1)) 
			{
				GraphicsPath borderPath = (GraphicsPath)path.Clone();
				
				// GraphicsPath.Widen() throws an OutOfMemoryException when the graphicsPath is too small
				// if this is the case, we're too small to care about the border anyway
				// so skip widening.
				try 
				{
					borderPath.Widen(pen);
				}
				catch (System.OutOfMemoryException ex) 
				{
				}
				e.Graphics.FillPath(new SolidBrush(pen.Color), borderPath);
			}

			// If a FormBehavior is specified and ShowFormBehaviorGlyph
			// is true, draw the appropriate glyph
			if (ShowFormBehaviorGlyph) 
			{
				Font f = new Font("Marlett", FormBehaviorGlyphSize);
				SizeF textSize;
				RectangleF layoutRect;
				if (FormBehavior == FormBehaviors.Close) 
				{
					textSize = e.Graphics.MeasureString("r", f);
					layoutRect = new RectangleF(
						((this.Width - textSize.Width) / 2.0F) + 1
						, ((this.Height - textSize.Height) / 2.0F) + 1
						, textSize.Width, textSize.Height);
					e.Graphics.DrawString("r", f, new SolidBrush(this.ForeColor), layoutRect);
				}
				else if (FormBehavior == FormBehaviors.Maximize) 
				{
					textSize = e.Graphics.MeasureString("1", f);
					layoutRect = new RectangleF((this.Width - textSize.Width) / 2.0F, (this.Height - textSize.Height) / 2.0F, textSize.Width, textSize.Height);
					e.Graphics.DrawString("1", f, new SolidBrush(this.ForeColor), layoutRect);
				} 
				else if (FormBehavior == FormBehaviors.Minimize) 
				{
					textSize = e.Graphics.MeasureString("0", f);
					layoutRect = new RectangleF((this.Width - textSize.Width) / 2.0F, (this.Height - textSize.Height) / 2.0F, textSize.Width, textSize.Height);
					e.Graphics.DrawString("0", f, new SolidBrush(this.ForeColor), layoutRect);
				} 
				else if (FormBehavior == FormBehaviors.TitleBar) 
				{
					textSize = e.Graphics.MeasureString(this.Text, this.Font);
					layoutRect = new RectangleF(RightMargin + lineThickness, (this.Height - textSize.Height) / 2.0F, textSize.Width, textSize.Height);
					e.Graphics.DrawString(this.Text, this.Font, new SolidBrush(this.ForeColor), layoutRect);
				}
			}
		}

		#endregion
	}
}
