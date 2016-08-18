using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;

namespace MirrorControl
{
	/// <summary>
	/// Summary description for UserControl1.
	/// </summary>
	public class MirrorControl : System.Windows.Forms.UserControl
	{
		private System.Windows.Forms.Timer timer1;
		private System.ComponentModel.IContainer components;

		private float angle = 0f;

		private Color color;
		private Brush brush;

		private int amplitude = 4;

		public MirrorControl()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			this.SetStyle(ControlStyles.DoubleBuffer, true);
			this.SetStyle(ControlStyles.Opaque, true);
			this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);

			color = Color.FromArgb(100, Color.SeaGreen);
			brush = new SolidBrush(color);
		}

		public int Amplitude
		{
			get
			{
				return amplitude;
			}
			set
			{
				amplitude = value;
			}
		}

		public Color Color
		{
			get
			{
				return color;
			}
			set
			{
				color = value;
				brush = new SolidBrush(color);
			}
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if( components != null )
					components.Dispose();
			}
			base.Dispose( disposing );
		}

		[DllImport("gdi32.dll", ExactSpelling=true, CharSet=System.Runtime.InteropServices.CharSet.Auto)]
		public static extern bool BitBlt(IntPtr hDC, int x, int y, int nWidth, int nHeight,
			IntPtr hSrcDC, int xSrc, int ySrc, int dwRop);

		[DllImport("gdi32.dll", ExactSpelling=true, EntryPoint="CreateCompatibleDC", CharSet=CharSet.Auto)]
		private static extern IntPtr CreateCompatibleDC(IntPtr hDC);

		[DllImport("gdi32.dll")]
		public static extern IntPtr CreateCompatibleBitmap(IntPtr hDC, int width, int height);

		[DllImport("gdi32.dll", ExactSpelling=true, CharSet=CharSet.Auto)]
		public static extern IntPtr SelectObject(IntPtr hdc, IntPtr obj);

		[DllImport("gdi32.dll", ExactSpelling=true, EntryPoint="DeleteObject", CharSet=System.Runtime.InteropServices.CharSet.Auto)]
		public static extern bool DeleteObject(IntPtr hObject);

		[DllImport("gdi32.dll", ExactSpelling=true, EntryPoint="DeleteDC", CharSet=CharSet.Auto)]
		private static extern bool DeleteDC(IntPtr hDC);

		protected override void OnPaint(PaintEventArgs e) 
		{		
			lock(this) 
			{
				try 
				{					
					Graphics g = Graphics.FromHwnd(this.Parent.Handle);
					IntPtr srcDc = g.GetHdc();

					IntPtr destDc = CreateCompatibleDC(srcDc);
					IntPtr bmp = CreateCompatibleBitmap(srcDc, this.Size.Width, this.Size.Height);

					SelectObject(destDc, bmp);

					BitBlt(destDc, 0, 0, this.Size.Width, this.Size.Height, srcDc, this.Location.X, this.Location.Y - 1 - this.Height, 0x00CC0020);

					Bitmap bitmap = Bitmap.FromHbitmap(bmp);

					DistortBitmap(bitmap);

					e.Graphics.DrawImage(bitmap, 0, 0);
					
					e.Graphics.FillRectangle(brush, 0, 0, this.Size.Width, this.Size.Height);

					DeleteDC(destDc);
					g.ReleaseHdc(srcDc);	
				
					DeleteObject(bmp);
				}
				catch (Exception) 
				{
				}
			}
		}

		private void DistortBitmap(Bitmap bmp) 
		{
			lock(bmp) 
			{
				bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);

				BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

				unsafe 
				{
					int* ptr = (int*)data.Scan0;

					for(int i=0; i < bmp.Height; i++) 
					{
						int offset = (int)(amplitude * Math.Sin(angle + 5.0 * i / bmp.Height * Math.PI));					
						if (offset > 0) 
						{
							for(int j=0; j < bmp.Width - offset; j++)
							{								
								*(ptr + j) = *(ptr + j + offset);
							}
						}
						else if (offset < 0) {												
							for(int j = bmp.Width - 1; j > -offset; j--) 
							{
								*(ptr + j) = *(ptr + j + offset);
							}
						}						

						ptr += data.Stride / 4;
					}
				}

				bmp.UnlockBits(data);
			}
		}

		#region Component Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.timer1 = new System.Windows.Forms.Timer(this.components);
			// 
			// timer1
			// 
			this.timer1.Enabled = true;
			this.timer1.Interval = 30;
			this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
			// 
			// MirrorControl
			// 
			this.Name = "MirrorControl";
			this.Size = new System.Drawing.Size(288, 136);

		}
		#endregion

		private void timer1_Tick(object sender, System.EventArgs e)
		{
			this.angle += 0.2f;
			this.Invalidate();
		}
	}
}
