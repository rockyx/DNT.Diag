using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNT.Diag.IO.ISO14230
{
  internal abstract class Channel : AbstractChannel<Options>
  {
    public delegate void InitCommunication();

    Dictionary<InitType, InitCommunication> _startComms;

    public Channel()
    {
      _startComms = new Dictionary<InitType, InitCommunication>();
    }

    public Dictionary<InitType, InitCommunication> StartComms
    {
      get { return _startComms; }
    }
  }
}
