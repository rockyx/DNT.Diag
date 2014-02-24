using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNT.Diag.IO
{
  public class ChannelException : ApplicationException
  {
    public ChannelException()
    {
    }

    public ChannelException(string message)
      : base(message)
    {
    }
  }
}
