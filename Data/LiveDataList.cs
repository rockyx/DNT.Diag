using System;
using System.Collections.Generic;

using DNT.Diag.Interop;

namespace DNT.Diag.Data
{
  public class LiveDataList : List<LiveDataItem>
  {
    IntPtr _native;
    LiveDataItem[] _enabledItems;
    LiveDataItem[] _showedItems;

    void FillEnabledItems()
    {
      int size = NativeMethods.RLiveDataListGetEnabledCount(_native);
      _enabledItems = new LiveDataItem[size];
      for (int i = 0; i < size; i++)
      {
        int index = NativeMethods.RLiveDataListGetEnabledIndex(_native, i);
        _enabledItems[i] = this[index];
      }
    }

    void FillShowedItems()
    {
      int size = NativeMethods.RLiveDataListGetShowedCount(_native);
      _showedItems = new LiveDataItem[size];
      for (int i = 0; i < size; i++)
      {
        int index = NativeMethods.RLiveDataListGetShowedIndex(_native, i);
        _showedItems[i] = this[index];
      }
    }

    internal LiveDataList(IntPtr native)
    {
      _native = native;
      int size = NativeMethods.RLiveDataListSize(_native);
      for (int i = 0; i < size; i++)
      {
        Add(new LiveDataItem(NativeMethods.RLiveDataListGet(_native, i)));
      }

      FillEnabledItems();

      FillShowedItems();
    }

    ~LiveDataList()
    {
      NativeMethods.RLiveDataListFree(_native);
    }

    public void Collate()
    {
      NativeMethods.RLiveDataListCollateEnable(_native);
      FillEnabledItems();

      NativeMethods.RLiveDataListCollateShowed(_native);
      FillShowedItems();
    }

    // Must call collate first
    public LiveDataItem[] EnabledItems
    {
      get
      {
        return _enabledItems;
      }
    }

    // Must call collate first
    public LiveDataItem[] ShowedItems
    {
      get
      {
        return _showedItems;
      }
    }

    public int NextShowedIndex
    {
      get
      {
        return NativeMethods.RLiveDataListGetNextShowedIndex(_native);
      }
    }

    public int GetShowedPosition(int index)
    {
      return NativeMethods.RLiveDataListGetShowedPosition(_native, index);
    }

    public bool IsEmpty
    {
      get
      {
        return NativeMethods.RLiveDataListEmpty(_native);
      }
    }
  }
}
