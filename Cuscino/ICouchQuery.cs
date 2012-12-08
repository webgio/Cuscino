using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cuscino
{
    public interface ICouchQuery
    {
        string GetCriteria();
        string GetViewDef();
    }
}
