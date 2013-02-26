using System;

namespace XR.Include
{
	public class DirectiveBase
	{
		public object Context { get; set; }
		public Processor Processor { get; set; }


		public virtual string Transform() {
			return string.Empty;
		}
	}
}

