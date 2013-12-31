using System;
using System.Text;

namespace XR.Include
{
	public class BaseContext
	{
		public string Title { get; set; }

        public static string HtmlEscape(string str)
        {
            var sb = new StringBuilder();
            string rv = str;
            foreach (char c in str.ToCharArray())
            {
                switch (c)
                {
                    case '<':
                        sb.Append("&lt;");
                        break;
                    case '>':
                        sb.Append("&gt;");
                        break;
                    case '&':
                        sb.Append("&amp;");
                        break;
                    case '"':
                        sb.Append("&#34;");
                        break;
                    case '\'':
                        sb.Append("&#39;");
                        break;
                    default:
                        sb.Append(c);
                        break;
                }
            }

            if (sb.Length > str.Length)
                str = sb.ToString();

            return str;
        }
	}
}

