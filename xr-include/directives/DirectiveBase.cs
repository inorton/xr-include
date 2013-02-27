using System;
using System.IO;

namespace XR.Include
{
	public class DirectiveBase
	{
		public object Context { get; set; }
		public Processor Processor { get; set; }


		public virtual void Transform( int depth, int line, TextWriter outputStream ) {

		}
	}
}

