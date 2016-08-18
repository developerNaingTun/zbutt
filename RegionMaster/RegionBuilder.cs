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
	/// Summary description for RegionBuilder.
	/// </summary>
	/// 
	[
	Designer(typeof(RegionBuilderDesigner))
	]
	public class RegionBuilder : Component
	{
		#region Fields
		private bool makeFormRegion;
		private Form parentForm;
		#endregion
		
		#region Constructors
		public RegionBuilder() : base()
		{
			makeFormRegion = false;
		}

		public RegionBuilder(IContainer container) : this() 
		{
			container.Add(this);
		}
		#endregion

		#region Properties
		[
		Category("Region"),
		DefaultValue(null),
		Description("The Form this component will be editing the region of")
		]
		public Form ParentForm 
		{
			get 
			{
				return parentForm;
			}
			set 
			{
				parentForm = value;
				if (parentForm != null) 
				{
					parentForm.Paint += new PaintEventHandler(this.regionbuilder_paint);
				}
			}
		}

		[
		Category("Region"),
		DefaultValue(false),
		Description("Specifies whether the IRegionControls on this form will be the shape of the form at runtime")
		]
		public bool MakeFormRegion 
		{
			get 
			{
				return makeFormRegion;
			}
			set 
			{
				makeFormRegion = value;
				if (ParentForm != null) 
				{
					parentForm.Invalidate();
				}
			}
		}
		#endregion

		#region Methods & Event Handlers
		private void regionbuilder_paint(object sender, PaintEventArgs ev) 
		{
			BuildRegion();
		}

		private void BuildRegion() 
		{
			if (parentForm == null) 
			{
				return;
			}

			if (MakeFormRegion) 
			{
				IRegionControl rControl;
				Region region = new Region(new Rectangle(0,0,0,0));

				foreach(Control c in parentForm.Controls) 
				{
					rControl = c as IRegionControl;
					if (rControl != null) 
					{
						if (rControl.AddToFormRegion) 
						{
							region.Union(rControl.MakeRegion(parentForm));
						}
					}
					rControl = null;
				}

				parentForm.Region = region;
			}
			else 
			{
				parentForm.Region = null;
			}
		}

		public void Invalidate() 
		{
			BuildRegion();
		}

		#endregion
	}
}