using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace XR.Include
{
    public class MethodProcessor : Processor
    {
        public override FileContent RunDirectives(int offset, string type, System.Text.RegularExpressions.GroupCollection groups)
        {
            FileContent rv = base.RunDirectives(offset, type, groups);
            if (Context != null)
            {
                if (rv == null)
                {
                    var methodname = type;
                    var args = new object[0];
                    var ctype = Context.GetType();
                    var m = ctype.GetMethod(methodname, BindingFlags.Public | BindingFlags.Instance);
                    if (m != null)
                    {
                        if (groups[2].Value == "param")
                        {
                            args = new object[] { groups[3].Value };
                        }

                        var res = m.Invoke(Context, args);
                        if (res != null)
                        {
                            return new FileContent()
                            {
                                Chunk = res.ToString()
                            };
                        }
                    }
                    else
                    {
                        throw new MissingMethodException(ctype.Name.ToString(), methodname);
                    }
                }
            }

            return rv;
        }
    }
}
