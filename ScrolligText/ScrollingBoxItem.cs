using System;
using System.Collections.Generic;

namespace ScrollingBox
{
    public class ScrollingBoxItem
    {
		internal System.Drawing.RectangleF rectF;
        public ScrollingBoxItem()
        {
			rectF = new System.Drawing.RectangleF(0, 0, 0, 0);
        }
    }
	public class ScrollingBoxText : ScrollingBoxItem
	{
		private string text;
		public ScrollingBoxText(String Text): base()
		{
			text = Text;
		}
		public string Text
        {
            get
            {
                return text;
            }
            set
            {
                text = value;
            }
        }
	}
	public class ScrollingBoxImage : ScrollingBoxItem
	{
		private System.Drawing.Image img;
		public ScrollingBoxImage(System.Drawing.Image Image): base()
		{
			img = Image;
		}
		public System.Drawing.Image Image
		{
			get
			{
				return img;
			}
			set
			{
				img = value;
			}
		}
	}
}
