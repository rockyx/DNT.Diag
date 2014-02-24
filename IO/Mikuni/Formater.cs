using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNT.Diag.IO.Mikuni
{
  internal abstract class Formater : AbstractFormater<Options>
  {
    public Formater(Options opts)
    {
      Options = opts;
    }

    public const byte HEAD_FORMAT = 0x48;

    public override byte[] Pack(byte[] src, int offset, int length)
    {
      int pos = 0;
      byte[] dest = new byte[3 + length];
      dest[pos++] = HEAD_FORMAT;
      Array.Copy(src, offset, dest, pos, length);
      pos += length;
      dest[pos++] = 0x0D;
      dest[pos++] = 0x0A;
      return dest;
    }

    public override byte[] Unpack(byte[] src, int offset, int length)
    {
      byte[] dest = new byte[length - 3];
      Array.Copy(src, offset, dest, 0, length - 3);
      return dest;
    }
  }
}
