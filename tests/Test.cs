using System;
using System.IO;
using System.Text;
using NUnit.Framework;
using XR.Include;

namespace XR.Include.Tests
{
	public class UserObject {
		public string First { get; set; }
		public AnotherObject Second { get; set; }
		public string OtherTemplate { get { return "/include1.html"; } }
	}

	public class AnotherObject {
		public string First { get; set; }
	}

	[TestFixture()]
	public class Test
	{


		[Test()]
		public void IncludeOne ()
		{
			var proc = new Processor();
			proc.RootDirectory = Environment.CurrentDirectory;

			proc.Transform( "/testgoodone.html", Console.Out );
		}

		[Test()]
		public void IncludeRecurse ()
		{
			var proc = new Processor();
			proc.RootDirectory = Environment.CurrentDirectory;
			
			proc.Transform( "/testrecurse.html", Console.Out );
		}

		[Test()]
		[ExpectedException(typeof(FormatException))]
		public void IncludeRecurseOveflow ()
		{
			var proc = new Processor();
			proc.RootDirectory = Environment.CurrentDirectory;
			
			proc.Transform( "/testoverflow.html", Console.Out  );
		}

		[Test()]
		public void Variables() 
		{
			var proc = new Processor();
			var parent = new UserObject()
			{ 
				First = "(First)",
				Second = new AnotherObject() { First = "(Child First)" },
			};

			proc.Context = parent;

			proc.RootDirectory = Environment.CurrentDirectory;

			proc.Transform( "/testproperties.html", Console.Out );

		}
	}
}
