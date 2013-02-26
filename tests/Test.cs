using System;
using NUnit.Framework;
using XR.Include;

namespace XR.Include.Tests
{
	[TestFixture()]
	public class Test
	{


		[Test()]
		public void IncludeOne ()
		{
			var proc = new Processor();
			proc.RootDirectory = Environment.CurrentDirectory;

			Console.WriteLine( proc.Transform( "/testgoodone.html" ) );
		}

		[Test()]
		public void IncludeRecurse ()
		{
			var proc = new Processor();
			proc.RootDirectory = Environment.CurrentDirectory;
			
			Console.WriteLine( proc.Transform( "/testrecurse.html" ) );
		}
	}
}

