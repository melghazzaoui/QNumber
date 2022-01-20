using System;
using System.Collections.Generic;
using System.Text;

namespace Algebra
{
    interface I2DIndexable
    {
        double this[int i, int j] { 
            get; 
            set; 
        }
    }
}
