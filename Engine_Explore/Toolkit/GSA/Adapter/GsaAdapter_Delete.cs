﻿using Engine_Explore.BHoM.Structural.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BHE = Engine_Explore.Engine;

namespace Engine_Explore.Adapter
{
    public partial class GsaAdapter
    {
        public bool DeleteAll(string filter = "", string config = "")
        {
            return m_Link.DeleteAll();
        }
    }
}
