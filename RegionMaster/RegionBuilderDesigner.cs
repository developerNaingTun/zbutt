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
	/// Summary description for RegionBuilderDesigner.
	/// </summary>
	/// 


	// This designer exists to shadow the MakeFormRegion property
	// to false at designtime.
	//
	public class RegionBuilderDesigner : ComponentDesigner
	{
		private bool MakeFormRegion 
		{
			get 
			{
				return (bool)ShadowProperties["MakeFormRegion"];
			}
			set
			{
				ShadowProperties["MakeFormRegion"] = value;
			}
		}

		public override void Initialize(IComponent component) 
		{
			base.Initialize(component);
			MakeFormRegion = false;
		}

		protected override void PreFilterProperties(IDictionary properties) 
		{
			base.PreFilterProperties(properties);
			PropertyDescriptor p;
				
			p = (PropertyDescriptor) properties["MakeFormRegion"];
			p = TypeDescriptor.CreateProperty(typeof(RegionBuilderDesigner), p, null);
			properties["MakeFormRegion"] = p;
		}
	}
}
