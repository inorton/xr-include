using System;
using System.IO;
using System.Reflection;

namespace XR.Include
{
	public class PropertyString : DirectiveBase
	{
	
		public string FileName { get; set; }

		public string Property { get; set; }

		public override void Transform (int depth, int line, TextWriter outputStream)
		{
			if (Context != null) {
				if (Property != null) {
					bool escapeHTML = true;
					var pname = Property;
					if (pname.StartsWith (":")) {
						pname = pname.Substring (1);
						escapeHTML = false;
					}

					var pparts = pname.Split (new char[] { '.' });
					object obj = Context;
					var ppath = "Context";
					foreach (var p in pparts) {
						ppath += "." + p;
						var prop = obj.GetType ().GetProperty (p);
						if (prop == null) {
							throw new FormatException (string.Format ("Failed to resolve Property '{0}' at {1}:{2}", ppath, FileName, line ));
						}
						try {
							obj = prop.GetValue (obj, null);
						} catch (Exception e) { 
							obj = e;
							escapeHTML = true;
							break;
						}
					}

					var rv = string.Format ("{0}", obj);
					if (escapeHTML)
						rv = System.Web.HttpUtility.HtmlEncode (rv);

					outputStream.Write (rv);
				}
			}
		}
	}
}

