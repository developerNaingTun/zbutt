using System;    

namespace Microsoft.Samples
{
	// This enum is used to specify common Form behavior.
	//
	// If a control has a property of type FormBehavior, the description for that
	// property should list the values supported
	public enum FormBehaviors 
	{
		None,
		Close,
		Drag,
		Maximize,
		Minimize,
		TitleBar,
		ResizeUpperLeft,
		ResizeTop,
		ResizeUpperRight,
		ResizeRight,
		ResizeLowerRight,
		ResizeBottom,
		ResizeLowerLeft,
		ResizeLeft,
	}
}
