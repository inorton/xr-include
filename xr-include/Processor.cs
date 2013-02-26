using System;
using System.Collections.Generic;
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

		Regex matchToken = new Regex(@"<%\s{0,}xr-([a-z]+)\s+([a-z]+)=""([^""]+?)""\s{0,}%>", RegexOptions.Compiled );

		public FileContent TokenFromMatch (int offset, GroupCollection groups)
		{
			if (groups.Count < 2) {
				return new FileContent () { 
					Chunk = string.Format( "invalid directive at offset {0}", offset )
				};
			}
			var type = groups [1].Value.ToLower();

			switch (type) {
			case "include":
				if ( groups.Count != 4 ) {
					return new FileContent() {
						Chunk = String.Format("too few parameters for directive '{0}' at offset {1}", type, offset )
					};
				}
				if ( groups[2].Value != "src" ) {
					return new FileContent() {
						Chunk = String.Format("expected src in directive '{0}' at offset {1}, got '{2}'", type, offset, groups[2].Value )
					};
				}
				return new FileContent() {
					Directive = new IncludeFile() { 
						Processor = this, 
						Src = groups[3].Value
					}
				};
				break;

			default:
				return new FileContent() { 
					Chunk = String.Format("unknown directive type '{0}' at offset {1}", type, offset ) 
				};
			}

		}

		public string Transform( string vpath ) 
		{
			var cont = Process(vpath);
			var sb = new System.Text.StringBuilder();
			foreach ( var c in cont )
				sb.Append( c.Chunk );
			return sb.ToString();
		}

		public List<FileContent> Process( string vpath )
		{
			var rv = new List<FileContent>();
			var txt = File.ReadAllText( VirtualToLocalPath( vpath ) );
			int offset = 0;
			do {
				var m = matchToken.Match( txt, offset );

				var start = m.Index;
				var len = m.Length;

				if ( start > offset ) {
					var before = new FileContent() { Chunk = txt.Substring( offset, start - offset ) };
					rv.Add( before );
				}

				if ( m.Length == 0 ) { // no more matches in txt
					var after = new FileContent() { Chunk = txt.Substring( offset ) };
					rv.Add( after );
					break;
				}

				// we have a match, process it into a directive
				var tagtxt = m.Groups[0].Value;
				var fc = TokenFromMatch( offset, m.Groups );

				if ( fc.Chunk != null ){
					rv.Add(fc);
				} else {
					if ( fc.Directive != null ){
						// we have a directive, do it
						var tfm = fc.Directive.Transform() ?? "";
						var c = new FileContent() { Chunk = tfm };
						rv.Add(c);
					}
				}


				// do we have enough groups?

				offset = m.Index + len;

			} while ( offset < txt.Length );


			return rv;
		}
	}
}

