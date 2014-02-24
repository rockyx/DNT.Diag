using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNT.Diag.Formats
{
  internal class ISO9141Format : AbstractFormat
  {

    public ISO9141Format(Attribute.Attribute attr)
      : base(attr)
    {
    }

    public override byte[] Pack(byte[] src, int offset, int count)
    {
      byte[] result = new byte[count + 4];
      result[0] = (byte)Attribute.ISOHeader;
      result[1] = (byte)Attribute.KLineTargetAddress;
      result[2] = (byte)Attribute.KLineSourceAddress;
      Array.Copy(src, offset, result, 3, count);

      int lastIndex = result.Length - 1;
      int length = count + 3;
      for (int i = 0; i < length; i++)
      {
        result[lastIndex] += result[i];
      }
      return result;
    }

    public override byte[] Unpack(byte[] src, int offset, int count)
    {
      byte cs = 0;
      int length = count - 1;
      for (int i = 0; i < length; i++)
      {
        cs += src[offset + i];
      }
      if (cs != src[offset + length])
      {
        return null;
      }

      length = count - 4;
      byte[] result = new byte[length];
      Array.Copy(src, offset + 3, result, 0, length);
      return result;
    }
  }
}
