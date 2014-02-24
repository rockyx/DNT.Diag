using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNT.Diag.Commbox
{
  internal interface IStream
  {
    void Connect();
    void Disconnect();
  }
}
