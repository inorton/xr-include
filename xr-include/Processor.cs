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
			if (string.IsNullOrEmpty (vpath))
				return RootDirectory;

			var comps = vpath.Split (new char[]{ Processor.VirtualPathSeparator });

			var local = string.Join (VirtualPathSeparator.ToString (), comps);
			local = RootDirectory + VirtualPathSeparator.ToString () + local;

			return local;
		}

		public Processor ()
		{
			RootDirectory = Environment.CurrentDirectory;
		}

		public object Context { get; set; }

		public const char VirtualPathSeparator = '/';

		public string RootDirectory { get; set; }

		Regex matchAllDirectives = new Regex (@"<%\s{0,}([^%]+)\s{0,}%>");

		Regex matchContextProperty = new Regex (@"(\${1,2}[\w\.\d\_]+)");

		Regex matchDirective = new Regex (@"xr-([a-z]+)\s+([a-z]+)=""([^""]+?)""");


		public FileContent DirectiveFromMatch (int offset, GroupCollection groups)
		{
			if (groups.Count == 2) {
				// might this be a property getter?
				if ( groups[0].Value.StartsWith("$") ){
					return new FileContent() {
						Directive = new PropertyString() {
							Property = groups[1].Value.Substring(1)
						}
					};
				}
			}

			if (groups.Count < 3) {
				return new FileContent () { 
					Chunk = string.Format( "invalid directive at offset {0}", offset )
				};
			}

			// might it be an include or other directive?
			var type = groups [1].Value.ToLower ();

			switch (type) {
			case "include":
				if (groups.Count != 4) {
					return new FileContent () {
						Chunk = String.Format("too few parameters for directive '{0}' at offset {1}", type, offset )
					};
				}
				if (groups [2].Value != "src") {
					return new FileContent () {
						Chunk = String.Format("expected src in directive '{0}' at offset {1}, got '{2}'", type, offset, groups[2].Value )
					};
				}
				return new FileContent () {
					Directive = new IncludeFile() { 
						Processor = this, 
						Src = groups[3].Value
					}
				};


			default:
				return new FileContent () { 
					Chunk = String.Format("unknown directive type '{0}' at offset {1}", type, offset ) 
				};
			}

		}

		public string Transform (string vpath)
		{
			var cont = Process (vpath);
			var sb = new System.Text.StringBuilder ();
			foreach (var c in cont)
				sb.Append (c.Chunk);
			return sb.ToString ();
		}

		int processDepth = 0;

		public List<FileContent> Process (string vpath)
		{
			try {
				lock (matchDirective) {
					processDepth++;
				}
			
				var rv = new List<FileContent> ();
				var txt = File.ReadAllText (VirtualToLocalPath (vpath));
				int offset = 0;
				do {
					var start = 0;
					var len = 0;
					FileContent fc = null;
					Match m = null;
					var any = matchAllDirectives.Match( txt, offset );
					if (any.Success) {
						start = any.Index;
						len = any.Length;

						if (start > offset) {
							var before = new FileContent () { Chunk = txt.Substring( offset, start - offset ) };
							rv.Add (before);
						}
					
						m = matchDirective.Match (any.Groups[0].Value, 0);
						if (!m.Success) {
							m = matchContextProperty.Match( any.Groups[0].Value, 0 );
						}

						if ( m != null && m.Success ){
							// we have a match, process it into a directive
							fc = DirectiveFromMatch (offset, m.Groups);
						}
					}

					if ( !any.Success ) { 
						// no more matches in txt
						var after = new FileContent () { Chunk = txt.Substring( offset ) };
						rv.Add (after);
						break;
					}


					if (fc != null) {
						if (fc.Chunk != null) {
							rv.Add (fc);
						} else {
							if (fc.Directive != null) {
								fc.Directive.Context = Context;
								// we have a directive, do it
								var tfm = fc.Directive.Transform (processDepth) ?? "";
								var c = new FileContent () { Chunk = tfm };
								rv.Add (c);
							}
						}
					}

					// do we have enough groups?

					offset = any.Index + len;

				} while ( offset < txt.Length );

				return rv;
			} finally {
				lock (matchDirective) {
					processDepth--;
				}
			}
		}
	}
}

