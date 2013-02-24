using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;

namespace XR.Include
{
	public class Processor 
	{
		public string VirtualToLocalPath (string vpath)
		{
			if ( string.IsNullOrEmpty(vpath ) ) return RootDirectory;

			var comps = vpath.Split( new char[]{ Processor.VirtualPathSeparator } );

			var local = string.Join( VirtualPathSeparator.ToString(), comps );
			local = RootDirectory + VirtualPathSeparator.ToString() + local;

			return local;
		}

		public Processor()
		{
			RootDirectory = Environment.CurrentDirectory;
		}

		public const char VirtualPathSeparator = '/';

		public string RootDirectory { get; set; }

		public string Transform( string path, params object[] args ){
			throw new NotImplementedException();
		}
	}
}

