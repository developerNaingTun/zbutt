using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;

/// Hey thanks for looking at this control.
/// Email me directly at toughman_net@hotmail.com if you want to comment
/// or flame me, either way I'd be grateful of the mail.
/// I've got several websites but I have zero time to actually develop
/// them and get them moving.
/// 
/// This has been written in VS 2005 Beta 2 so if you want to run this in
/// an earlier version then you may need to may small adjustments.

namespace ScrollingBox
{
	[ToolboxBitmap("ScrollBoxToolIcon")]
    public class ScrollingBox : System.Windows.Forms.Control
    {
        private System.Timers.Timer timer;
        private ScrollingBoxCollection items;
        private ArrowDirection movingDirection;
        private bool showBackgroundImage;
		private StringFormat stringFormat;
		private int lastY;
		private bool isMouseDown;
		private Point mousePos;
		private bool startingPositionHasBeenSetAfterHeight;

        public ScrollingBox()
        {
            SetStyle(ControlStyles.UserPaint
				| ControlStyles.OptimizedDoubleBuffer			// Change this to ControlStyles.DoubleBuffer if not using .NET v2.0
                | ControlStyles.AllPaintingInWmPaint, true);

            items = new ScrollingBoxCollection();
            items.OnCollectionChanged += new EventHandler(items_OnCollectionChanged);

            movingDirection = ArrowDirection.Up;
            showBackgroundImage = false;
			stringFormat = new StringFormat();

			lastY = 0;
			isMouseDown = false;
			mousePos = new Point();
			startingPositionHasBeenSetAfterHeight = false;
			
            timer = new System.Timers.Timer();
            timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);
            timer.Interval = 25;
             
            timer.Enabled = true;
        }	

        void items_OnCollectionChanged(object sender, EventArgs e)
        {
            RecalculateItems();
        }

        private void RecalculateItems()
        {
			SizeF sizeF = new SizeF();
			Graphics g = this.CreateGraphics();
			int availWidth = this.Width - this.Padding.Left - this.Padding.Right;

            for (int i = 0; i < items.Count; i++)
            {
				ScrollingBoxItem item = items[i];

				item.rectF.X = this.Padding.Left;
				item.rectF.Width = (float)availWidth;

				if (item.GetType() == typeof(ScrollingBoxText))
				{
					ScrollingBoxText textItem = (ScrollingBoxText)item;
					sizeF = g.MeasureString(textItem.Text, this.Font, availWidth, stringFormat);
				}
				else if (item.GetType() == typeof(ScrollingBoxImage))
				{
					ScrollingBoxImage imgItem = (ScrollingBoxImage)item;
					sizeF = imgItem.Image.Size;
					imgItem.rectF.Width = sizeF.Width;
					switch (Alignment)
					{
						case StringAlignment.Near:
							item.rectF.X = this.Padding.Left;
							break;
						case StringAlignment.Center:
							item.rectF.X = (availWidth / 2) - (sizeF.Width / 2) + Padding.Left;
							break;
						case StringAlignment.Far:
							item.rectF.X = this.Width - sizeF.Width - this.Padding.Right;
							break;
					}
				}

				item.rectF.Height = sizeF.Height;

				if (i == 0)
				{
					if (startingPositionHasBeenSetAfterHeight == false)
					{
						item.rectF.Y = this.Height;
						startingPositionHasBeenSetAfterHeight = true;
					}
				}
				else
                {
					item.rectF.Y = (float)items[i - 1].rectF.Y + items[i - 1].rectF.Height;
                }
            }
        }
		private void PositionItems()
		{
			for (int i = 0; i < items.Count; i++)
			{
				ScrollingBoxItem item = items[i];

				if (movingDirection == ArrowDirection.Up)
				{
					if (item.rectF.Y + item.rectF.Height < 0)
					{
						if (i == 0)
						{
							// Goto the bottom of the screen list items
							item.rectF.Y = items[items.Count - 1].rectF.Y + this.Height + item.rectF.Height;
						}
						else
						{
							item.rectF.Y = items[i - 1].rectF.Y + items[i - 1].rectF.Height;
						}
					}
					else
					{
						// Move up the screen
						item.rectF.Y -= 1;
					}
				}
				else if (movingDirection == ArrowDirection.Down)
				{
					if (item.rectF.Y > this.Height)
					{
						if (i == items.Count - 1)
						{
							// Goto the top of the screen list items
							item.rectF.Y = items[0].rectF.Y - this.Height - item.rectF.Height;
						}
						else
						{
							item.rectF.Y = items[i + 1].rectF.Y - item.rectF.Height;
						}
					}
					else
					{
						// Move down the screen
						item.rectF.Y += 1;
					}
				}
			}

			this.Invalidate();
		}

        void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
			PositionItems();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            SolidBrush textBrush = new SolidBrush(this.ForeColor);
			RectangleF clipRectF = new RectangleF(e.ClipRectangle.X, e.ClipRectangle.Y, e.ClipRectangle.Width, e.ClipRectangle.Height);

            if (!showBackgroundImage)   // faster than checking if BackgroundImage is null
            {
                g.Clear(this.BackColor);
            }
            else
            {
                g.DrawImageUnscaled(this.BackgroundImage, 0, 0);
            }
         
            for(int i = 0; i < items.Count; i++)
            {
                ScrollingBoxItem item = items[i];

				if (clipRectF.IntersectsWith(item.rectF))
                {
					if (item.GetType() == typeof(ScrollingBoxText))
					{
						g.DrawString(((ScrollingBoxText)item).Text, this.Font, textBrush, item.rectF, stringFormat);
					}
					else if (item.GetType() == typeof(ScrollingBoxImage))
					{
						g.DrawImage(((ScrollingBoxImage)item).Image, item.rectF);
					}
                }
            }
			// Draw a border
			ControlPaint.DrawBorder(g, this.ClientRectangle, ControlPaint.Dark(this.BackColor), ButtonBorderStyle.Solid);
        }
        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            // overridden
        }
        protected override void OnResize(EventArgs e)
        {
			base.OnResize(e);
			RecalculateItems();
			this.Invalidate();
        }
        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            RecalculateItems();
			this.Invalidate();
        }
        protected override void OnBackgroundImageChanged(EventArgs e)
        {
            base.OnBackgroundImageChanged(e);
            if (this.BackgroundImage == null)
            {
                showBackgroundImage = false;
            }
            else
            {
                showBackgroundImage = true;
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            isMouseDown = true;
            mousePos = new Point(e.X, e.Y);
            timer.Enabled = false;
            Cursor = Cursors.Hand;
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            isMouseDown = false;
            timer.Enabled = true;
            movingDirection = ArrowDirection.Up;
            Cursor = Cursors.Default;
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
			if (!DesignMode)
			{
				if (isMouseDown)
				{
					if (lastY < e.Y)
					{
						movingDirection = ArrowDirection.Down;
					}
					else
					{
						movingDirection = ArrowDirection.Up;
					}
					lastY = e.Y;
					PositionItems();
				}
			}
        }

		protected override void OnPaddingChanged(EventArgs e)
		{
			base.OnPaddingChanged(e);
			RecalculateItems();
			this.Invalidate();
		}

		[Browsable(false)]
		public override string Text
		{
			get
			{
				return base.Text;
			}
			set
			{
				base.Text = value;
			}
		}
        [Browsable(false)]
        public ScrollingBoxCollection Items
        {
            get
            {
                return items;
            }
            set
            {
                items = value;
            }
        }
		[Browsable(true), DefaultValue(true)]
		public bool ScrollEnabled
		{
			get 
			{
				return timer.Enabled;	
			}
			set
			{
				timer.Enabled = value;
			}
		}
		[Browsable(true), DefaultValue(StringAlignment.Far)]
		public StringAlignment Alignment
		{
			get
			{
				return stringFormat.Alignment;
			}
			set
			{
				stringFormat.Alignment = value;
				RecalculateItems();
				this.Invalidate();
			}
		}
	}
}
