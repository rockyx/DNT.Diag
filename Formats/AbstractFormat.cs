using System;

using DNT.Diag.Attribute;

namespace DNT.Diag.Formats
{
  internal abstract class AbstractFormat
  {
    public AbstractFormat(Attribute.Attribute attribute)
    {
      Attribute = attribute;
    }

    public abstract byte[] Pack(byte[] src, int offset, int count);
    public abstract byte[] Unpack(byte[] src, int offset, int count);

    public byte[] Pack(params byte[] src)
    {
      if (src == null)
        throw new ArgumentNullException("src");

      return Pack(src, 0, src.Length);
    }

    public byte[] Unpack(params byte[] src)
    {
      if (src == null)
        throw new ArgumentNullException("src");

      return Unpack(src, 0, src.Length);
    }

    protected Attribute.Attribute Attribute { get; set; }
  }
}
