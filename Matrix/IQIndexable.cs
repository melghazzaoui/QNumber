using System;
using System.Collections.Generic;
using System.Text;

namespace Algebra
{
    interface IQIndexable
    {
        QNumber this[int i, int j]
        {
            get;
            set;
        }
    }
}
