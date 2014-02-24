using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace DNT.Diag
{
  public class Timer
  {
    long _ticks;

    const double NANO = 1 * 1000 * 1000 * 1000;
    const double MICRO = 1 * 1000 * 1000;
    const double MILLI = 1 * 1000;

    static readonly double TICKS_PER_NANO = Stopwatch.Frequency / NANO;
    static readonly double TICKS_PER_MICRO = Stopwatch.Frequency / MICRO;
    static readonly double TICKS_PER_MILLI = Stopwatch.Frequency / MILLI;

    static readonly double NANO_PER_TICKS = NANO / Stopwatch.Frequency;
    static readonly double MICRO_PER_TICKS = MICRO / Stopwatch.Frequency;
    static readonly double MILLI_PER_TICKS = MILLI / Stopwatch.Frequency;

    public Timer()
    {
      _ticks = 0;
    }

    public Timer(long tick)
    {
      _ticks = tick;
    }

    public Timer(TimeSpan time)
    {
      _ticks = time.Ticks;
    }

    public TimeSpan ToTimeSpan()
    {
      return TimeSpan.FromTicks(_ticks);
    }

    public long Nanoseconds
    {
      get
      {
        return (long)(_ticks * NANO_PER_TICKS);
      }
      set
      {
        _ticks = (long)(value * TICKS_PER_NANO);
      }
    }

    public long Microseconds
    {
      get
      {
        return (long)(_ticks * MICRO_PER_TICKS);
      }
      set
      {
        _ticks = (long)(value * TICKS_PER_MICRO);
      }
    }

    public long Milliseconds
    {
      get
      {
        return (long)(_ticks * MILLI_PER_TICKS);
      }
      set
      {
        _ticks = (long)(value * TICKS_PER_MILLI);
      }
    }

    public long Seconds
    {
      get
      {
        return (long)(_ticks * Stopwatch.Frequency);
      }
      set
      {
        _ticks = (long)(value * Stopwatch.Frequency);
      }
    }

    public static Timer FromMicroseconds(long time)
    {
      Timer t = new Timer();
      t.Microseconds = time;
      return t;
    }

    public static Timer FromMilliseconds(long time)
    {
      Timer t = new Timer();
      t.Milliseconds = time;
      return t;
    }

    public static Timer FromSeconds(long time)
    {
      Timer t = new Timer();
      t.Seconds = time;
      return t;
    }

    public long Ticks
    {
      get { return _ticks; }
      set { _ticks = value; }
    }
  }
}
