using System;

namespace XR.Include
{
	public class FileContent
	{
		public string Chunk { get; set; }
		public DirectiveBase Directive { get; set; }

		public string Render ( ) {
			if ( Chunk != null ) return Chunk;
			if ( Directive != null ) return Directive.Transform();
			return String.Empty;
		}
	}
}

