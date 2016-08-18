using System;
using System.Collections.Generic;
using System.Text;
using System.Management;

namespace zbutt
{
    class Registration
    {
        public static bool registered()
        {
            return MyMyanmar.RegFramework.isActivated();
        }
    }
}
