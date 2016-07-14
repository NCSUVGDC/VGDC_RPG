﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VGDC_RPG.Networking
{
    public interface INetEventHandler
    {
        int HandlerID { get; }

        void HandleEvent(DataReader r);
    }
}
