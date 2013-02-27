using System;
using System.Diagnostics;

namespace XR.Include
{
	public class IncludeFile : DirectiveBase
	{
		public const int MaxRecursion = 8;

		public string Src { get; set; }

		public override string Transform (int depth)
		{
			if (depth > MaxRecursion) {
				return "[error: too many levels of recursion]";
			}

			var sb = new System.Text.StringBuilder();
			var tokens = Processor.Process (Src);
			foreach (var t in tokens) {
				sb.Append( t.Chunk );
			}
			return sb.ToString();
		}
	}
}

