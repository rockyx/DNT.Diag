﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNT.Diag.IO
{
  internal interface IChannelFactory
  {
    IChannel Create(AbstractCommbox box, ChannelCatagory cag);
  }
}
