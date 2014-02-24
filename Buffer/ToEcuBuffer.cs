using System;
using System.Collections.Generic;
using System.Threading;

namespace DNT.Diag.Buffer
{
  internal class ToEcuBuffer
  {
    Queue<byte> _buffer;
    Mutex _mutex;

    public ToEcuBuffer()
    {
      _buffer = new Queue<byte>();
      _mutex = new Mutex();
    }

    public int Read(byte[] buff, int offset, int count)
    {
      _mutex.WaitOne();

      count = count < _buffer.Count ? count : _buffer.Count;

      for (int i = 0; i < count; i++)
        buff[i] = _buffer.Dequeue();
      _mutex.ReleaseMutex();

      return count;
    }

    public void Write(byte[] buff, int offset, int count)
    {
      if ((count + offset) > buff.Length)
        throw new IndexOutOfRangeException("buff");

      _mutex.WaitOne();

      count += offset;
      for (int i = offset; i < count; i++)
        _buffer.Enqueue(buff[i]);

      _mutex.ReleaseMutex();
    }

    public int BytesToRead
    {
      get
      {
        _mutex.WaitOne();
        int ret = _buffer.Count;
        _mutex.ReleaseMutex();

        return ret;
      }
    }

    public void Clear()
    {
      _mutex.WaitOne();
      _buffer.Clear();
      _mutex.ReleaseMutex();
    }
  }
}
