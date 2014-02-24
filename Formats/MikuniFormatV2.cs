using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNT.Diag.Formats
{
  internal class MikuniFormatV2 : AbstractFormat
  {
    public MikuniFormatV2(Attribute.Attribute attr)
      : base(attr)
    {

    }

    public override byte[] Pack(byte[] src, int offset, int count)
    {
      byte[] result = new byte[count + 3];
      result[0] = Utils.HiByte(count + 1);
      result[1] = Utils.LoByte(count + 1);
      Array.Copy(src, offset, result, 2, count);

      byte cs = 0;
      int length = result.Length - 1;
      for (int i = 0; i < length; i++)
        cs += result[i];

      cs = (byte)(0x00 - cs);
      result[length] = cs;

      return result;
    }

    public override byte[] Unpack(byte[] src, int offset, int count)
    {
      byte[] result = new byte[count - 3];
      Array.Copy(src, offset - 2, result, 0, count - 3);
      return result;
    }
  }
}
