using System;
using System.Text;
using System.Collections.Generic;

namespace DNT.Diag.Data
{
  public class LiveDataList : IEnumerable<LiveDataItem>
  {
    //const int MaxBuffCount = 0xFFF;
    //Dictionary<string, byte[]> _ecuResponseBuff;
    //Dictionary<string, string> _queryCmdNameClassByShortName;
    //Dictionary<string, byte[]> _commandNeed;
    //List<LiveDataItem> _needItems;
    List<LiveDataItem> _items;
    Dictionary<string, LiveDataItem> _queryByShortName;

    internal LiveDataList()
    {
      //_ecuResponseBuff = new Dictionary<string, byte[]>();
      //_queryCmdNameClassByShortName = new Dictionary<string, string>();
      //_commandNeed = new Dictionary<string, byte[]>();
      //_needItems = new List<LiveDataItem>();
      _items = new List<LiveDataItem>();
      _queryByShortName = new Dictionary<string, LiveDataItem>();
    }

    //public void PrepareDisplay()
    //{
    //  _commandNeed.Clear();
    //  _needItems.Clear();

    //  StringBuilder sb = new StringBuilder(100);

    //  for (int i = 0; i < _items.Count; i++)
    //  {
    //    if (_items[i].IsEnabled && _items[i].IsDisplay)
    //    {
    //      sb.Clear();
    //      sb.AppendFormat("{0}_{1}", _items[i].CmdName, _items[i].CmdClass);
    //      string key = sb.ToString();
    //      if (!_commandNeed.ContainsKey(key))
    //        _commandNeed[key] = _items[i].FormattedCommand;
    //      _needItems.Add(_items[i]);
    //    }
    //  }
    //}

    internal void Add(LiveDataItem item)
    {
      _items.Add(item);
      _queryByShortName[item.ShortName] = item;
      //string cmdClassName = item.CmdClass + item.CmdName;
      //if (!_ecuResponseBuff.ContainsKey(cmdClassName))
      //  _ecuResponseBuff[cmdClassName] = new byte[MaxBuffCount];
      //item.EcuResponseBuff = _ecuResponseBuff[cmdClassName];
      //_queryCmdNameClassByShortName[item.ShortName] = cmdClassName;
    }

    //public List<LiveDataItem> GetNeedItems()
    //{
    //  return _needItems;
    //}

    public LiveDataItem this[string shortName]
    {
      get
      {
        if (!_queryByShortName.ContainsKey(shortName))
          throw new ArgumentException("shortName");

        return _queryByShortName[shortName];
      }
    }

    public LiveDataItem this[int index]
    {
      get
      {
        return _items[index];
      }
    }

    public int Count
    {
      get { return _items.Count; }
    }

    public IEnumerator<LiveDataItem> GetEnumerator()
    {
      return _items.GetEnumerator();
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }
  }
}
