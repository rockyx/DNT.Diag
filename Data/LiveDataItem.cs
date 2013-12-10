using System;
using System.Text;

using DNT.Diag.Interop;

namespace DNT.Diag.Data
{
  public class LiveDataItem
  {
    IntPtr _native;
    string _shortName = null;
    string _content = null;
    string _unit = null;
    string _defaultValue = null;
    string _description = null;
    string _cmdName = null;
    string _cmdClass = null;
    string _minValue = null;
    string _maxValue = null;
    int _index;
    byte[] _cmd = null;
    byte[] _valueBuff = new byte[100];

    internal LiveDataItem(IntPtr native)
    {
      _native = native;
      byte[] utf8 = new byte[1024];
      int length = NativeMethods.RLiveDataItemGetShortName(_native, utf8);
      _shortName = UTF8Encoding.UTF8.GetString(utf8, 0, length);

      length = NativeMethods.RLiveDataItemGetContent(_native, utf8);
      _content = UTF8Encoding.UTF8.GetString(utf8, 0, length);

      length = NativeMethods.RLiveDataItemGetUnit(_native, utf8);
      _unit = UTF8Encoding.UTF8.GetString(utf8, 0, length);

      length = NativeMethods.RLiveDataItemGetDefaultValue(_native, utf8);
      _defaultValue = UTF8Encoding.UTF8.GetString(utf8, 0, length);

      length = NativeMethods.RLiveDataItemGetDescription(_native, utf8);
      _description = UTF8Encoding.UTF8.GetString(utf8, 0, length);

      length = NativeMethods.RLiveDataItemGetCmdName(_native, utf8);
      _cmdName = UTF8Encoding.UTF8.GetString(utf8, 0, length);

      length = NativeMethods.RLiveDataItemGetCmdClass(_native, utf8);
      _cmdClass = UTF8Encoding.UTF8.GetString(utf8, 0, length);

      _index = NativeMethods.RLiveDataItemGetIndex(_native);

      length = NativeMethods.RLiveDataItemGetMinValue(_native, utf8);
      _minValue = UTF8Encoding.UTF8.GetString(utf8, 0, length);

      length = NativeMethods.RLiveDataItemGetMaxValue(_native, utf8);
      _maxValue = UTF8Encoding.UTF8.GetString(utf8, 0, length);

      length = NativeMethods.RLiveDataItemGetCommand(_native, utf8);
      _cmd = new byte[length];
      Array.Copy(utf8, _cmd, length);
    }

    ~LiveDataItem()
    {
      NativeMethods.RLiveDataItemFree(_native);
    }

    public string ShortName
    {
      get
      {
        return _shortName;
      }
    }

    public string Content
    {
      get
      {
        return _content;
      }
    }

    public string Unit
    {
      get
      {
        return _unit;
      }
    }

    public string DefaultValue
    {
      get
      {
        return _defaultValue;
      }
    }

    public string Description
    {
      get
      {
        return _description;
      }
    }

    public string CmdName
    {
      get
      {
        return _cmdName;
      }
    }

    public string CmdClass
    {
      get
      {
        return _cmdClass;
      }
    }

    public int Index
    {
      get
      {
        return _index;
      }
    }

    public int Position
    {
      get
      {
        return NativeMethods.RLiveDataItemGetPosition(_native);
      }
    }

    public bool IsEnabled
    {
      get
      {
        return NativeMethods.RLiveDataItemIsEnabled(_native);
      }
    }

    public bool IsShowed
    {
      get
      {
        return NativeMethods.RLiveDataItemIsShowed(_native);
      }
      set
      {
        NativeMethods.RLiveDataItemSetShowed(_native, value);
      }
    }

    public bool IsOutOfRange
    {
      get
      {
        return NativeMethods.RLiveDataItemIsOutOfRange(_native);
      }
    }

    public string MinValue
    {
      get
      {
        return _minValue;
      }
    }

    public string MaxValue
    {
      get
      {
        return _maxValue;
      }
    }

    public byte[] Command
    {
      get
      {
        return _cmd;
      }
    }

    public string Value
    {
      get
      {
        int length = NativeMethods.RLiveDataItemGetValue(_native, _valueBuff);
        return UTF8Encoding.UTF8.GetString(_valueBuff, 0, length);
      }
    }
  }
}
