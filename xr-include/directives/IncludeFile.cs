using System;
using System.IO;
using System.Diagnostics;

namespace XR.Include
{
	public class IncludeFile : DirectiveBase
	{
		public const int MaxRecursion = 8;

		public string Src { get; set; }

		public override void Transform (int depth, int line, TextWriter outputStream)
		{
			if (depth > MaxRecursion) {
				throw new FormatException( String.Format( "too many levels of recursion at {0}:{1}", Src, line ) );
			}
			Processor.Process (Src, outputStream);

		}
	}
}

