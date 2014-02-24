using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNT.Diag.IO.ISO9141
{
  internal class Formater : AbstractFormater<Options>
  {
    public Formater(Options opts)
    {
      Options = opts;
    }

    public override byte[] Pack(byte[] src, int offset, int count)
    {
      byte[] dest = new byte[4 + count];
      dest[0] = Options.Header;
      dest[1] = Options.TargetAddress;
      dest[2] = Options.SourceAddress;
      Array.Copy(src, offset, dest, 3, count);
      var last = count + 3;
      dest[last] = 0;
      for (int i = 0; i < last; i++)
        dest[last] += dest[i];
      return dest;
    }

    int SingleUnpack(byte[] sData, int sOffset, int count, byte[] tData, int tOffset)
    {
      byte cs = 0;
      int size = count - 1 + sOffset;
      for (int i = sOffset; i < size; i++)
        cs += sData[i];
      if (cs != sData[size])
        return -1;
      size -= 3 + sOffset;
      Array.Copy(sData, sOffset + 3, tData, tOffset, size);
      return size;
    }

    public override byte[] Unpack(byte[] src, int offset, int length)
    {
      byte[] temp = new byte[1024];
      int j = 3;
      int k = 0;
      int len = 0;

      while (j < length)
      {
        // Multi-frame
        if ((src[k + offset] == src[j + offset])
          && (src[k + 1 + offset] == src[j + 1 + offset])
          && (src[k + 2 + offset] == src[j + 2 + offset]))
        {
          len += SingleUnpack(src, k + offset, j - k, temp, len);
          k = j;
        }
        j++;
      }

      len += SingleUnpack(src, k + offset, j - k, temp, len);
      byte[] ret = new byte[len];
      Array.Copy(temp, 0, ret, 0, len);
      return ret;
    }
  }
}
