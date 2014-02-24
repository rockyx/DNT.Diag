using System;
using System.ComponentModel;

namespace DNT.Diag.Data
{
  public class LiveDataItem : INotifyPropertyChanged
  {
    string _shortName;
    string _content;
    string _unit;
    string _defaultValue;
    string _description;
    string _minValue;
    string _maxValue;
    string _cmdName;
    string _cmdClass;
    string _value;
    byte[] _command;
    byte[] _formattedCommand;
    byte[] _ecuResponseBuff;
    int _indexForSort;
    bool _isEnabled;
    bool _isDisplay;
    bool _isOutOfRange;

    Func<LiveDataItem, string> CalcFunction;

    public event PropertyChangedEventHandler PropertyChanged;

    public LiveDataItem()
    {
      _shortName = "";
      _content = "";
      _unit = "";
      _defaultValue = "";
      _description = "";
      _minValue = "";
      _maxValue = "";
      _value = "";
      _command = null;
      _formattedCommand = null;
      _ecuResponseBuff = null;
      _indexForSort = -1;
      _isEnabled = false;
      _isDisplay = false;
      _isOutOfRange = false;
      CalcFunction = null;
    }

    public string ShortName
    {
      get { return _shortName; }
      set { _shortName = value; }
    }

    public string Content
    {
      get { return _content; }
      set { _content = value; }
    }

    public string Unit
    {
      get { return _unit; }
      set { _unit = value; }
    }

    public string DefaultValue
    {
      get { return _defaultValue; }
      set { _defaultValue = value; }
    }

    public string Description
    {
      get { return _description; }
      set { _description = value; }
    }

    public string MinValue
    {
      get { return _minValue; }
      set { _minValue = value; }
    }

    public string MaxValue
    {
      get { return _maxValue; }
      set { _maxValue = value; }
    }

    public string CmdName
    {
      get { return _cmdName; }
      set { _cmdName = value; }
    }

    public string CmdClass
    {
      get { return _cmdClass; }
      set { _cmdClass = value; }
    }

    public string Value
    {
      get { return _value; }
      set
      {
        if (_value != value)
        {
          _value = value;
          if (PropertyChanged != null)
            PropertyChanged(this, new PropertyChangedEventArgs("Value"));
        }
      }
    }

    public byte[] Command
    {
      get { return _command; }
      set { _command = value; }
    }

    public byte[] FormattedCommand
    {
      get { return _formattedCommand; }
      set { _formattedCommand = value; }
    }

    public byte[] EcuResponseBuff
    {
      get { return _ecuResponseBuff; }
      set { _ecuResponseBuff = value; }
    }

    public int IndexForSort
    {
      get { return _indexForSort; }
      set { _indexForSort = value; }
    }

    public bool IsEnabled
    {
      get { return _isEnabled; }
      set { _isEnabled = value; }
    }

    public bool IsDisplay
    {
      get { return _isDisplay; }
      set { _isDisplay = value; }
    }

    public bool IsOutOfRange
    {
      get { return _isOutOfRange; }
      set
      {
        if (_isOutOfRange != value)
        {
          _isOutOfRange = value;
          if (PropertyChanged != null)
            PropertyChanged(this, new PropertyChangedEventArgs("IsOutOfRange"));
        }
      }
    }

    public void CalcValue()
    {
      if (CalcFunction != null)
        Value = CalcFunction(this);
    }
  }
}
