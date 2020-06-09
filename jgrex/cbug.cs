using System;
using System.Collections.Generic;
using System.Text;

namespace jgrex
{
    public static class cbug
    {
        public static void write(string data, params object[] pars)
        {
#if DEBUG
            Console.WriteLine(data,pars);
#endif
        }
    }
}
