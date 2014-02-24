using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNT.Diag.IO
{
  internal interface IChannel
  {
    void Send(byte[] data, int offset, int count);
    void Send(params byte[] data);
    byte[] Recv();
    byte[] SendAndRecv(byte[] sData, int sOffset, int sCount);
    void StartHeartbeat(byte[] data, int offset, int count);
    void StartHeartbeat(params byte[] data);
    void StopHeartbeat();
    void SetByteInterval(Timer tx, Timer rx);
    void SetFrameInterval(Timer tx, Timer rx);
    void SetTimeout(Timer time);
  }
}
