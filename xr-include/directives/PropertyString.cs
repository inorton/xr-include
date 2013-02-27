using System;
using System.Reflection;

namespace XR.Include
{
	public class PropertyString : DirectiveBase
	{
	
		public string Property { get; set; }

		public override string Transform (int depth)
		{
			if (Context != null) {
				if ( Property != null ) {
					bool escapeHTML = true;
					var pname = Property;
					if ( pname.StartsWith("$") ){
						pname = pname.Substring(1);
						escapeHTML = false;
					}

					try {
						var pparts = pname.Split( new char[] { '.' } );
						object obj = Context;

						foreach ( var p in pparts ){
							var prop = obj.GetType().GetProperty( p );
							try {
								obj = prop.GetValue( obj, null );
							} catch ( Exception e ) { 
								return e.ToString();
							}
						}

						var rv = string.Format("{0}", obj );
						if ( escapeHTML )
							rv = System.Web.HttpUtility.HtmlEncode(rv);

						return rv;

					} catch ( Exception e ) {
						return e.Message;
					}
				}
			}
			return string.Empty;
		}
	}
}

