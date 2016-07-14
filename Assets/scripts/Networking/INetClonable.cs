using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VGDC_RPG.Networking
{
    public interface INetClonable
    {
        void Clone(DataWriter w);
    }
}
