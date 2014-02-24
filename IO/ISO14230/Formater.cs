using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNT.Diag.IO.ISO14230
{
  internal class Formater : AbstractFormater<Options>
  {
    public const int MODE_8X_HEADER_LENGTH = 3;
    public const int MODE_CX_HEADER_LENGTH = 3;
    public const int MODE_80_HEADER_LENGTH = 4;
    public const int MODE_XX_HEADER_LENGTH = 1;
    public const int MODE_00_HEADER_LENGTH = 2;
    public const int CHECKSUM_LENGTH = 1;
    public const int MAX_DATA_LENGTH = 128;

    public Formater(Options opts)
    {
      Options = opts;
    }

    public override byte[] Pack(byte[] src, int offset, int length)
    {
      int pos = 0;
      byte[] dest = null;

      switch (Options.CurrentMode)
      {
        case Mode.Mode8X:
          dest = new byte[MODE_8X_HEADER_LENGTH + length + CHECKSUM_LENGTH];
          dest[pos++] = Utils.LoByte(0x80 | length);
          dest[pos++] = Options.TargetAddress;
          dest[pos++] = Options.SourceAddres;
          break;
        case Mode.ModeCX:
          dest = new byte[MODE_CX_HEADER_LENGTH + length + CHECKSUM_LENGTH];
          dest[pos++] = Utils.LoByte(0xC0 | length);
          dest[pos++] = Options.TargetAddress;
          dest[pos++] = Options.SourceAddres;
          break;
        case Mode.Mode80:
          dest = new byte[MODE_80_HEADER_LENGTH + length + CHECKSUM_LENGTH];
          dest[pos++] = 0x80;
          dest[pos++] = Options.TargetAddress;
          dest[pos++] = Options.SourceAddres;
          dest[pos++] = Utils.LoByte(length);
          break;
        case Mode.Mode00:
          dest = new byte[MODE_00_HEADER_LENGTH + length + CHECKSUM_LENGTH];
          dest[pos++] = 0x00;
          dest[pos++] = Utils.LoByte(length);
          break;
        case Mode.ModeXX:
          dest = new byte[MODE_XX_HEADER_LENGTH + length + CHECKSUM_LENGTH];
          dest[pos++] = Utils.LoByte(length);
          break;
      }

      Array.Copy(src, offset, dest, pos, length);
      pos += length;

      byte checksum = 0;
      for (int i = 0; i < pos; i++)
        checksum += dest[i];
      dest[pos++] = checksum;
      return dest;
    }

    public override byte[] Unpack(byte[] src, int offset, int length)
    {
      int count = 0;
      int sOffset = offset;

      if ((src[0] & 0xFF) > 0x80)
      {
        count = (src[0] & 0xFF) - 0x80;
        if (src[1] != Options.SourceAddres)
          return null;
        if (count != (length - MODE_8X_HEADER_LENGTH - CHECKSUM_LENGTH))
        {
          count = src[0] - 0xC0; // for ModeCX
          if (count != (length - MODE_CX_HEADER_LENGTH - CHECKSUM_LENGTH))
            return null;
          offset += MODE_CX_HEADER_LENGTH;
        }
        else
        {
          offset += MODE_8X_HEADER_LENGTH;
        }
      }
      else if ((src[0] & 0xFF) == 0x80)
      {
        count = src[3] & 0xFF;
        if (src[1] != Options.SourceAddres)
          return null;
        if (count != (length - MODE_80_HEADER_LENGTH - CHECKSUM_LENGTH))
          return null;
        offset += MODE_80_HEADER_LENGTH;
      }
      else if (src[0] == 0x00)
      {
        count = src[1] & 0xFF;
        if (count != (length - MODE_00_HEADER_LENGTH - CHECKSUM_LENGTH))
          return null;
        offset += MODE_00_HEADER_LENGTH;
      }
      else
      {
        count = src[0] & 0xFF;
        if (count != (length - MODE_XX_HEADER_LENGTH - MODE_CX_HEADER_LENGTH))
          return null;
        offset += MODE_XX_HEADER_LENGTH;
      }

      byte checksum = 0;
      length--; // skip checksum
      length += sOffset;
      for (int i = sOffset; i < length; i++)
        checksum += src[i];
      if (checksum != src.Last())
        return null;

      byte[] dest = new byte[count];
      Array.Copy(src, offset, dest, 0, count);
      return dest;
    }
  }
}
