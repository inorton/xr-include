using System;
using System.IO;

namespace XR.Include
{
	public class IncludeFile : DirectiveBase
	{
		public string Src { get; set; }

		public IncludeFile ()
		{
		}

		public override string Transform ()
		{
			var sb = new System.Text.StringBuilder();
			var tokens = Processor.Process (Src);
			foreach (var t in tokens) {
				sb.Append( t.Chunk );
			}
			return sb.ToString();
		}
	}
}

