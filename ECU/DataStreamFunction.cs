using System;
using System.Threading.Tasks;
using System.IO;

using DNT.Diag.Interop;
using DNT.Diag.Data;


namespace DNT.Diag.ECU
{
  public class DataStreamFunction
  {
    IntPtr _native;
    AbstractECU _ecu;
    TaskFactory _taskFactory;
    Task[] _tasks;

    internal DataStreamFunction(IntPtr native, AbstractECU ecu)
    {
      _native = native;
      _ecu = ecu;
      _taskFactory = new TaskFactory();
    }

    ~DataStreamFunction()
    {
      if (_native == IntPtr.Zero)
        return;
      NativeMethods.RDataStreamFunctionFree(_native);
    }

    public LiveDataList LiveData
    {
      get
      {
        return new LiveDataList(NativeMethods.RDataStreamFunctionGetLiveData(_native));
      }
    }

    public void Start()
    {
      if (_native == IntPtr.Zero)
      {
        throw new NullReferenceException();
      }

      _tasks = new Task[]
      {
        _taskFactory.StartNew(() =>
        {
          if (!NativeMethods.RDataStreamFunctionReadData(_native))
            throw new IOException(_ecu.LastInfo);
        }),
        _taskFactory.StartNew(() =>
        {
          if (!NativeMethods.RDataStreamFunctionCalcData(_native))
            throw new IOException(_ecu.LastInfo);
        })
      };

      _taskFactory.ContinueWhenAll(_tasks, (tasks) =>
      {
        foreach (Task t in tasks)
        {
          if (t.IsFaulted)
          {
            throw t.Exception.InnerException;
          }
        }
      });
    }

    public void Stop()
    {
      if (_native == IntPtr.Zero)
      {
        throw new NullReferenceException();
      }

      if (_taskFactory == null || _tasks == null || _tasks.Length == 0)
        return;

      NativeMethods.RDataStreamFunctionStopCalc(_native);
      NativeMethods.RDataStreamFunctionStopRead(_native);
      foreach (Task t in _tasks)
      {
        t.Wait();
      }
    }

    public void Once()
    {
      if (_native == IntPtr.Zero)
      {
        throw new NullReferenceException();
      }

      _tasks = new Task[]
      {
        _taskFactory.StartNew(() =>
        {
          if (!NativeMethods.RDataStreamFunctionReadDataOnce(_native) ||
            !NativeMethods.RDataStreamFunctionCalcDataOnce(_native))
            throw new IOException(_ecu.LastInfo);
        })
      };

      _taskFactory.ContinueWhenAll(_tasks, (tasks) =>
      {
        foreach (Task t in tasks)
        {
          if (t.IsFaulted)
          {
            throw t.Exception.InnerException;
          }
        }
      });

      foreach (Task t in _tasks)
      {
        t.Wait();
      }
    }
  }
}
