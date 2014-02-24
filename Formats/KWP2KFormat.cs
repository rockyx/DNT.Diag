using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNT.Diag.Formats
{
  internal class KWP2KFormat : AbstractFormat
  {
    const int KWP8X_HEADER_LENGTH = 3;
    const int KWPCX_HEADER_LENGTH = 3;
    const int KWP80_HEADER_LENGTH = 4;
    const int KWPXX_HEADER_LENGTH = 1;
    const int KWP00_HEADER_LENGTH = 2;
    const int KWP_CHECKSUM_LENGTH = 1;
    const int KWP_MAX_DATA_LENGTH = 128;

    public KWP2KFormat(Attribute.Attribute attr)
      : base(attr)
    {
    }

    public override byte[] Pack(byte[] src, int offset, int count)
    {
      int pos = 0;
      byte cs = 0;
      byte[] result = null;

      switch (Attribute.KWP2KCurrentMode)
      {
        case Diag.Attribute.KWP2KMode.Mode8X:
          result = new byte[KWP8X_HEADER_LENGTH + count + KWP_CHECKSUM_LENGTH];
          result[pos++] = (byte)(0x80 | count);
          result[pos++] = (byte)Attribute.KLineTargetAddress;
          result[pos++] = (byte)Attribute.KLineSourceAddress;
          break;
        case Diag.Attribute.KWP2KMode.ModeCX:
          result = new byte[KWPCX_HEADER_LENGTH + count + KWP_CHECKSUM_LENGTH];
          result[pos++] = (byte)(0xC0 | count);
          result[pos++] = (byte)Attribute.KLineTargetAddress;
          result[pos++] = (byte)Attribute.KLineSourceAddress;
          break;
        case Diag.Attribute.KWP2KMode.Mode80:
          result = new byte[KWP80_HEADER_LENGTH + count + KWP_CHECKSUM_LENGTH];
          result[pos++] = 0x80;
          result[pos++] = (byte)Attribute.KLineTargetAddress;
          result[pos++] = (byte)Attribute.KLineSourceAddress;
          result[pos++] = (byte)count;
          break;
        case Diag.Attribute.KWP2KMode.ModeXX:
          result = new byte[KWPXX_HEADER_LENGTH + count + KWP_CHECKSUM_LENGTH];
          result[pos++] = (byte)count;
          break;
        default:
          return null;
      }

      Array.Copy(src, offset, result, pos, count);
      pos += count;

      int i;
      for (i = 0; i < pos; i++)
        cs += result[i];

      result[i] = cs;
      return result;
    }

    public override byte[] Unpack(byte[] src, int offset, int count)
    {
      int length = 0;
      byte[] result = null;

      if ((src[offset] & 0xFF) > 0x80)
      {
        length = (src[offset] & 0xFF) - 0x80;
        if (src[offset + 1] != Attribute.KLineSourceAddress)
          return null;

        if (length != (count - KWP8X_HEADER_LENGTH - KWP_CHECKSUM_LENGTH))
        {
          length = src[offset] - 0xC0; // for kwp cx
          if (length != (count - KWPCX_HEADER_LENGTH - KWP_CHECKSUM_LENGTH))
            return null;
          else
            offset = offset + KWPCX_HEADER_LENGTH;
        }
        else
        {
          offset = offset + KWP8X_HEADER_LENGTH;
        }
      }
      else if ((src[offset] & 0xFF) == 0x80)
      {
        length = src[offset + 3] & 0xFF;
        if (src[offset + 1] != Attribute.KLineSourceAddress)
          return null;

        if (length != (count - KWP80_HEADER_LENGTH - KWP_CHECKSUM_LENGTH))
          return null;
        offset = offset + KWP80_HEADER_LENGTH;
      }
      else if (src[offset] == 0x00)
      {
        length = src[offset + 1] & 0xFF;
        if (length != (count - KWP00_HEADER_LENGTH - KWP_CHECKSUM_LENGTH))
          return null;
        offset = offset + KWP00_HEADER_LENGTH;
      }
      else
      {
        length = src[offset] & 0xFF;
        if (length != (count - KWPXX_HEADER_LENGTH - KWP_CHECKSUM_LENGTH))
          return null;
        offset = offset + KWPXX_HEADER_LENGTH;
      }

      result = new byte[length];
      Array.Copy(src, offset, result, 0, length);
      return result;
    }
  }
}
