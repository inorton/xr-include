using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
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

		// match : <% DIRECTIVE %>
		Regex matchAllDirectives = new Regex (@"<%\s{0,}([^%]+)\s{0,}%>");

		// match : $PROPERTY or $:PROPERTY
		Regex matchContextProperty = new Regex (@"(\$+:{0,1}[\w\.\d_]+)");

		// match : xr-TYPE PARAM="VALUE"
		Regex matchDirective = new Regex (@"xr-([a-z]+)\s+([a-z]+)=""([^""]+?)""");



		public FileContent PropertyFromMatch( string vpath, GroupCollection groups)
		{
			if (groups.Count == 2) {
				// might this be a property getter?
				var grp0 = groups[0].Value;
				if ( grp0.StartsWith("$") ){
					var propstr = grp0.Substring(1);
					if ( grp0.StartsWith("$$") ){
						return new FileContent() {
							Chunk = grp0.Substring(1)
						};
					} else {
						if ( !Regex.IsMatch( propstr, "^\\.|^:\\." ) ){
							return new FileContent() {
								Directive = new PropertyString() {
									Context = Context,
									FileName = vpath,
									Property = propstr,
								}
							};
						}
					}
				}
			}
			return null;
		}

		public FileContent DirectiveFromMatch (int offset, GroupCollection groups)
		{
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
						Context = Context,
						Src = groups[3].Value
					}
				};


			default:
				return new FileContent () { 
					Chunk = String.Format("unknown directive type '{0}' at offset {1}", type, offset ) 
				};
			}

		}

		public void Transform (string vpath, TextWriter outputStream )
		{
			Process (vpath, outputStream);
		}

		string ProcessLineProperties (string vpath, string line, int linenum)
		{
			var sb = new StringBuilder( line.Length );
			var offset = 0;
			do {
				var props = matchContextProperty.Match( line, offset );
				if ( props.Success ) {
					if ( props.Index > 0 ) {
						// text before the match
						var before = line.Substring( offset, props.Index - offset );
						sb.Append(before);
					}
					offset = props.Index;

					// got a match
					var fc = PropertyFromMatch( vpath, props.Groups );
					if ( fc != null ) {

						using ( var sw = new StringWriter() ) {
							if ( fc.Directive != null ) {
								fc.Directive.Transform( 0, linenum, sw );
							}
							if ( fc.Chunk != null ) {
								sw.Write( fc.Chunk );
							}
							sb.Append( sw.ToString() );
						}
					} else {
						throw new FormatException("property matched but no directive created!");
					}
					offset += props.Length;
				} else {
					sb.Append( line.Substring( offset ) ); // no more
					break;
				}

			} while ( offset < line.Length );

			return sb.ToString();
		}

		int processDepth = 0;

		public void Process (string vpath, TextWriter outputStream)
		{
			try {
				lock (matchDirective) {
					processDepth++;
				}
			
				// read each line, first parse it for $Property expressions,
				// then parse the result for directives

				var fh = File.OpenRead( VirtualToLocalPath (vpath) );
				var linenum = 0;
				var sr = new StreamReader( fh );
				var line = string.Empty;
				do {
					line = sr.ReadLine();
					linenum++;
					if ( line == null ) break;
					line += Environment.NewLine;

					// do properties here
					line = ProcessLineProperties( vpath, line, linenum );

					int offset = 0;
					do {
						var start = 0;
						var len = 0;
						FileContent fc = null;
						Match m = null;

						// do directives
						var dirs = matchAllDirectives.Match( line, offset );
						if (dirs.Success) {
							start = dirs.Index;
							len = dirs.Length;

							if (start > offset) {
								outputStream.Write( line.Substring( offset, start - offset ) );
							}
					
							m = matchDirective.Match (dirs.Groups[0].Value, 0);
							if (!m.Success) {
								m = matchContextProperty.Match( dirs.Groups[0].Value, 0 );
							}

							if ( m != null && m.Success ){
								// we have a match, process it into a directive
								fc = DirectiveFromMatch (offset, m.Groups);
							}
						}

						if ( !dirs.Success ) { 
							// no more matches in line
							outputStream.Write( line.Substring( offset ) );
							break;
						}

						if (fc != null) {
							if (fc.Chunk != null) {
								outputStream.Write( fc.Chunk );
							} else {
								if (fc.Directive != null) {
									// we have a directive, do it
									fc.Directive.Transform (processDepth, linenum, outputStream);
								}
							}
						}

						// do we have enough groups?

						offset = dirs.Index + len;
	
					} while ( offset < line.Length );

				} while ( true );

			} finally {
				lock (matchDirective) {
					processDepth--;
				}
			}
		}
	}
}

