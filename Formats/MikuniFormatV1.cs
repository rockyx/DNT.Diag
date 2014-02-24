using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNT.Diag.Formats
{
  internal class MikuniFormatV1 : AbstractFormat
  {
    const byte HEAD_FORMAT = 0x48;

    public MikuniFormatV1(Attribute.Attribute attr)
      : base(attr)
    {
    }

    public override byte[] Pack(byte[] src, int offset, int count)
    {
      byte[] result = new byte[count + 3];
      result[0] = HEAD_FORMAT;
      Array.Copy(src, offset, result, 1, count);
      result[count + 1] = 0x0D;
      result[count + 2] = 0x0A;

      return result;
    }

    public override byte[] Unpack(byte[] src, int offset, int count)
    {
      byte[] result = new byte[count - 3];
      Array.Copy(src, offset + 1, result, 0, count - 3);

      return result;
    }
  }
}
