using System;
using NUnit.Framework;
using XR.Include;

namespace XR.Include.Tests
{
	public class UserObject {
		public string Name { get; set; }
		public AnotherObject Child { get; set; }
	}

	public class AnotherObject {
		public string Name { get; set; }
	}

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

		[Test()]
		public void IncludeRecurseOveflow ()
		{
			var proc = new Processor();
			proc.RootDirectory = Environment.CurrentDirectory;
			
			Console.WriteLine( proc.Transform( "/testoverflow.html" ) );
		}

		[Test()]
		public void Variables() 
		{
			var proc = new Processor();
			var parent = new UserObject()
			{ 
				Name = "the parent >> escaped",
				Child = new AnotherObject() { Name = "<b>the child</b> literal" },
			};

			proc.Context = parent;

			proc.RootDirectory = Environment.CurrentDirectory;

			Console.WriteLine( proc.Transform( "/testproperties.html" ) );

		}
	}
}

