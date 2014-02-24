using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNT.Diag.Formats
{
  internal class KWP1282Format : AbstractFormat
  {
    byte _frameCounter;
    const byte FRAME_END = 0x03;

    public KWP1282Format(Attribute.Attribute attr)
      : base(attr)
    {
      _frameCounter = 0;
    }

    byte FrameCounterIncrement()
    {
      return ++_frameCounter;
    }

    public override byte[] Pack(byte[] src, int offset, int count)
    {
      byte[] result = new byte[count + 3];
      result[0] = (byte)(count + 2);
      result[1] = FrameCounterIncrement();

      Array.Copy(src, offset, result, 2, count);
      result[result.Length - 1] = FRAME_END;
      return result;
    }

    public override byte[] Unpack(byte[] src, int offset, int count)
    {
      byte[] result = new byte[count - 2];
      Array.Copy(src, offset + 1, result, 0, count - 2);
      return result;
    }
  }
}
