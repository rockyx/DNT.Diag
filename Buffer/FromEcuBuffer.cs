using System;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics;

namespace DNT.Diag.Buffer
{
  internal class FromEcuBuffer
  {
    Timer _expire;
    Mutex _mutex;
    Queue<byte> _buffer;

    public FromEcuBuffer()
    {
      _expire = Timer.FromMilliseconds(500);
      _mutex = new Mutex();
      _buffer = new Queue<byte>();
    }

    int ReadImmediately(byte[] buff, int offset, int count)
    {
      _mutex.WaitOne();

      count = _buffer.Count < count ? _buffer.Count : count;

      count += offset;

      for (int i = offset; i < count; i++)
        buff[i] = _buffer.Dequeue();

      _mutex.ReleaseMutex();

      return count - offset;
    }

    int ReadNoTimeout(byte[] buff, int offset, int count)
    {
      while (_buffer.Count < count)
        Thread.Sleep(1);

      return ReadImmediately(buff, offset, count);
    }

    int ReadWithTimeout(byte[] buff, int offset, int count)
    {
      int cnt = 0;
      int leftCount = count;

      Stopwatch watcher = new Stopwatch();

      watcher.Start();

      var expire = _expire.ToTimeSpan();

      while (cnt < count)
      {
        _mutex.WaitOne();
        int size = _buffer.Count;
        _mutex.ReleaseMutex();

        if (size >= leftCount)
        {
          cnt += ReadImmediately(buff, cnt, leftCount);
          break;
        }

        if (size != 0)
        {
          cnt += ReadImmediately(buff, cnt, size);
          leftCount -= size;
        }

        var elapsed = watcher.Elapsed;
        if (elapsed > expire)
          break;
      }

      return cnt;
    }

    public void Write(byte[] buff, int offset, int count)
    {
      if ((count + offset) > buff.Length)
        throw new IndexOutOfRangeException();

      _mutex.WaitOne();

      count += offset;
      for (int i = offset; i < count; i++)
        _buffer.Enqueue(buff[i]);

      _mutex.ReleaseMutex();
    }

    public int Read(byte[] buff, int offset, int count)
    {
      if (_expire.Ticks == 0)
      {
        return ReadNoTimeout(buff, offset, count);
      }
      else
      {
        int ret = ReadImmediately(buff, offset, count);
        if (ret < count)
          ret += ReadWithTimeout(buff, offset + ret, count - ret);

        return ret;
      }
    }

    public int BytesToRead
    {
      get
      {
        _mutex.WaitOne();
        int bytes = _buffer.Count;
        _mutex.ReleaseMutex();

        return bytes;
      }
    }

    public void Clear()
    {
      _mutex.WaitOne();
      _buffer.Clear();
      _mutex.ReleaseMutex();
    }

    public Timer Timeout
    {
      get { return _expire; }
      set { _expire = value; }
    }
  }
}
