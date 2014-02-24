using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNT.Diag.IO.ISO15765
{
  internal abstract class Channel : AbstractChannel<Options>
  {
    protected bool IsMultiFrame(byte[] sData, int sOffset, int length)
    {
      int mode = sData[0] & (int)IDMode.Extension | (int)FrameType.Remote;
      if (mode == ((int)IDMode.Standard | (int)FrameType.Data))
      {
        if (length < ISO15765.Formater.STD_FRAME_LENGTH)
          return false;
        return true;
      }
      else
      {
        if (length < ISO15765.Formater.EXT_FRAME_LENGTH)
          return false;
        return true;
      }
    }
  }
}
