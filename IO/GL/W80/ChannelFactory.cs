using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNT.Diag.IO.GL.W80
{
  internal class ChannelFactory : IO.IChannelFactory
  {

    public IO.IChannel Create(AbstractCommbox box, ChannelCatagory cag)
    {
      var realBox = box as Commbox;
      if (realBox == null)
        throw new ArgumentException();

      switch (cag)
      {
        case ChannelCatagory.ISO9141_2:
          return new ISO9141.Channel<Constant, Commbox>(realBox);
        case ChannelCatagory.ISO14230:
          return new ISO14230.Channel<Constant, Commbox>(realBox);
        case ChannelCatagory.ISO15765:
          //return new ISO15765.Channel<Constant, Commbox>(realBox);
        case ChannelCatagory.MIKUNI:
          //return new Mikuni.Channel<Constant, Commbox>(realBox);
        default:
          throw new NotImplementedException();
      }
    }
  }
}
