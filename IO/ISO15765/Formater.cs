using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNT.Diag.IO.ISO15765
{
  internal class Formater : AbstractFormater<Options>
  {
    public const int STD_FRAME_LENGTH = 11;
    public const int EXT_FRAME_LENGTH = 13;

    readonly byte[] DEFAULT_FLOW_CONTROL = new byte[8]
    {
      0x30, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
    };
    byte _frameCounter;

    public Formater(Options opts)
    {
      Options = opts;
      _frameCounter = 0x21;
    }

    int SinglePack(byte[] sData, int sOffset, int count, byte[] tData, int tOffset)
    {
      var pos = tOffset;
      var idMode = Options.IDMode;
      var frameType = Options.FrameType;
      var idForEcuRecv = Options.IDForEcuRecv;

      if (idMode == IDMode.Standard)
      {
        if (frameType == FrameType.Data)
        {
          tData[pos++] = Utils.LoByte(8 | (int)IDMode.Standard | (int)FrameType.Data);
        }
        else
        {
          tData[pos++] = Utils.LoByte(8 | (int)IDMode.Standard | (int)FrameType.Remote);
        }
        tData[pos++] = Utils.HiByte(Utils.LoWord(idForEcuRecv));
        tData[pos++] = Utils.LoByte(Utils.LoWord(idForEcuRecv));
      }
      else
      {
        if (frameType == FrameType.Data)
        {
          tData[pos++] = Utils.LoByte(8 | (int)IDMode.Extension | (int)FrameType.Data);
        }
        else
        {
          tData[pos++] = Utils.LoByte(8 | (int)IDMode.Extension | (int)FrameType.Remote);
        }
        tData[pos++] = Utils.HiByte(Utils.HiWord(idForEcuRecv));
        tData[pos++] = Utils.LoByte(Utils.HiWord(idForEcuRecv));
        tData[pos++] = Utils.HiByte(Utils.LoWord(idForEcuRecv));
        tData[pos++] = Utils.LoByte(Utils.LoWord(idForEcuRecv));
      }

      tData[pos++] = Utils.LoByte(count);
      Array.Copy(sData, sOffset, tData, pos, count);

      return pos - tOffset;
    }

    int CalcMultiFrameBytes(int count)
    {
      // all number using hex.
      // The first frame is 1x xx d0 d1 d2 d3 d4 d5
      // 1x xx is the length of bytes, max FFF
      // rest frame begin with counter (2x)
      // standard id header length is 3
      // extend id header length is 5
      int restLength = count - 6;
      int frameLength = 0;
      if (Options.IDMode == IDMode.Standard)
        frameLength = STD_FRAME_LENGTH;
      else
        frameLength = EXT_FRAME_LENGTH;
      return ((restLength / 7) + 1) * frameLength + ((restLength % 7) == 0 ? 0 : frameLength);
    }

    int PackMultiFrameFirst(byte[] sData, int sOffset, int length, byte[] tData, int tOffset)
    {
      _frameCounter = 0x21; // initialize frame counter
      int pos = tOffset;
      var idMode = Options.IDMode;
      var frameType = Options.FrameType;
      var idForEcuRecv = Options.IDForEcuRecv;
      if (idMode == IDMode.Standard)
      {
        if (frameType == FrameType.Data)
        {
          tData[pos++] = Utils.LoByte(8 | (int)IDMode.Standard | (int)FrameType.Data);
        }
        else
        {
          tData[pos++] = Utils.LoByte(8 | (int)IDMode.Standard | (int)FrameType.Remote);
        }
        tData[pos++] = Utils.HiByte(Utils.LoWord(idForEcuRecv));
        tData[pos++] = Utils.LoByte(Utils.LoWord(idForEcuRecv));
      }
      else
      {
        if (frameType == FrameType.Data)
        {
          tData[pos++] = Utils.LoByte(8 | (int)IDMode.Extension | (int)FrameType.Data);
        }
        else
        {
          tData[pos++] = Utils.LoByte(8 | (int)IDMode.Extension | (int)FrameType.Remote);
        }
        tData[pos++] = Utils.HiByte(Utils.HiWord(idForEcuRecv));
        tData[pos++] = Utils.LoByte(Utils.HiWord(idForEcuRecv));
        tData[pos++] = Utils.HiByte(Utils.LoWord(idForEcuRecv));
        tData[pos++] = Utils.LoByte(Utils.LoWord(idForEcuRecv));
      }

      tData[pos++] = Utils.LoByte(0x10 | Utils.HiByte(length));
      tData[pos++] = Utils.LoByte(length);

      Array.Copy(sData, sOffset, tData, pos, 6);
      pos += 6;
      return pos - tOffset;
    }

    int PackMultiFrames(byte[] sData, int sOffset, int length, byte[] tData, int tOffset)
    {
      int pos = tOffset;

      var idMode = Options.IDMode;
      var frameType = Options.FrameType;
      var idForEcuRecv = Options.IDForEcuRecv;

      if (idMode == IDMode.Standard)
      {
        if (frameType == FrameType.Data)
        {
          tData[pos++] = Utils.LoByte(8 | (int)IDMode.Standard | (int)FrameType.Data);
        }
        else
        {
          tData[pos++] = Utils.LoByte(8 | (int)IDMode.Standard | (int)FrameType.Remote);
        }
        tData[pos++] = Utils.HiByte(Utils.LoWord(idForEcuRecv));
        tData[pos++] = Utils.LoByte(Utils.LoWord(idForEcuRecv));
      }
      else
      {
        if (frameType == FrameType.Data)
        {
          tData[pos++] = Utils.LoByte(8 | (int)IDMode.Extension | (int)FrameType.Data);
        }
        else
        {
          tData[pos++] = Utils.LoByte(8 | (int)IDMode.Extension | (int)FrameType.Remote);
        }
        tData[pos++] = Utils.HiByte(Utils.HiWord(idForEcuRecv));
        tData[pos++] = Utils.LoByte(Utils.HiWord(idForEcuRecv));
        tData[pos++] = Utils.HiByte(Utils.LoWord(idForEcuRecv));
        tData[pos++] = Utils.LoByte(Utils.LoWord(idForEcuRecv));
      }
      tData[pos++] = _frameCounter;
      Array.Copy(sData, sOffset, tData, pos, length);
      pos += length;
      _frameCounter = Utils.LoByte(_frameCounter == 0x2F ? 0x20 : _frameCounter + 1);

      return pos - tOffset;
    }

    public override byte[] Pack(byte[] sData, int sOffset, int length)
    {
      byte[] dest = null;
      if (length < 8)
      {
        if (Options.IDMode == IDMode.Standard)
          dest = new byte[STD_FRAME_LENGTH];
        else
          dest = new byte[EXT_FRAME_LENGTH];
        int len = SinglePack(sData, sOffset, length, dest, 0);
        return dest;
      }

      dest = new byte[CalcMultiFrameBytes(length)];

      int pos = PackMultiFrameFirst(sData, sOffset, length, dest, 0);

      for (int i = 6; i < length; i += 7)
      {
        int count = length - i;
        pos += PackMultiFrames(sData, sOffset + i, count > 7 ? 7 : count, dest, pos);
      }
      return dest;
    }

    public override byte[] Unpack(byte[] sData, int sOffset, int length)
    {
      byte[] dest = null;

      int mode = sData[0] & (int)IDMode.Extension | (int)FrameType.Remote;

      if (mode == ((int)IDMode.Standard | (int)FrameType.Data))
      {
        if (length > STD_FRAME_LENGTH)
        {
          // multi-frame
          int count = ((sData[3] & 0x0F) << 8) + sData[4];
          dest = new byte[count];
          Array.Copy(sData, sOffset + 5, dest, 0, 6);
          int pos = 6;

          for (int i = STD_FRAME_LENGTH; i < count; i += STD_FRAME_LENGTH)
          {
            int cnt = length - i;
            Array.Copy(sData, sOffset + i + 4, dest, pos, cnt > 7 ? 7 : cnt);
            pos += cnt > 7 ? 7 : cnt;
          }
          return dest;
        }
        else
        {
          if ((sData[0] & 0x0F) != (length - 3))
            return null;
          dest = new byte[sData[3]];
          Array.Copy(sData, sOffset + 4, dest, 0, sData[3]);
          return dest;
        }
      }
      else if (mode == ((int)IDMode.Extension | (int)FrameType.Data))
      {
        if (length > EXT_FRAME_LENGTH)
        {
          // multi-frame
          int count = ((sData[5] & 0x0F) << 8) + sData[6];
          dest = new byte[count];
          Array.Copy(sData, sOffset + 7, dest, 0, 6);
          int pos = 6;
          for (int i = EXT_FRAME_LENGTH; i < count; i += EXT_FRAME_LENGTH)
          {
            int cnt = length - i;
            Array.Copy(sData, sOffset + i + 6, dest, pos, cnt > 7 ? 7 : cnt);
            pos += cnt > 7 ? 7 : cnt;
          }
          return dest;
        }
        else
        {
          if ((sData[0] & 0x0F) != (length - 5))
            return null;
          dest = new byte[sData[5]];
          Array.Copy(sData, sOffset + 6, dest, 0, sData[5]);
          return dest;
        }
      }

      return null;
    }
  }
}
