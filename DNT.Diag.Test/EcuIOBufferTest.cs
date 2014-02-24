//using System;
//using System.Threading;
//using System.Threading.Tasks;
//using System.Linq;
//using Microsoft.VisualStudio.TestTools.UnitTesting;

//using DNT.Diag.IO;

//namespace DNT.Diag.Test
//{
//  [TestClass]
//  public class EcuIOBufferTest
//  {
//    ToEcuBuffer _toBuffer;
//    FromEcuBuffer _fromBuffer;

//    [TestInitialize]
//    public void TestInitialize()
//    {
//      _toBuffer = new ToEcuBuffer();
//      _fromBuffer = new FromEcuBuffer();
//      _fromBuffer.Timeout = TimeSpan.FromMilliseconds(500);
//    }

//    [TestMethod]
//    public void IOBufferTestThread()
//    {
//      byte[] data = 
//      {
//        0x01, 0x02, 0x03, 0x04, 0x05
//      };
      
//      Task.Factory.StartNew(() => 
//      {
//        int i = 10;
//        while (i-- != 0)
//        {
//          byte[] buff = new byte[5];
          
//          _toBuffer.Write(data, 0, data.Length);
//          Thread.Sleep(200);
//          _fromBuffer.Read(buff, 0, buff.Length);
//          Assert.IsTrue(buff.SequenceEqual(data));

//          buff = new byte[5];
//          _fromBuffer.Read(buff, 0, buff.Length);
//          Assert.IsTrue(buff.SequenceEqual(data));
//        }
//      });

//      Task.Factory.StartNew(() =>
//      {
//        int i = 10;
//        while (i-- != 0)
//        {
//          byte[] buff = new byte[5];

//          Thread.Sleep(100);
          
//          _toBuffer.Read(buff, 0, buff.Length);
//          Assert.IsTrue(buff.SequenceEqual(data));

//          _fromBuffer.Write(buff, 0, buff.Length);

//          Thread.Sleep(300);

//          _fromBuffer.Write(data, 0, data.Length);
//        }
//      });
//    }

//    [TestMethod]
//    public void IOBufferTest()
//    {
//      byte[] data = 
//      {
//        0x01, 0x02, 0x03, 0x04, 0x05
//      };

//      _toBuffer.Write(data, 0, data.Length);

//      byte[] buff = new byte[5];

//      _toBuffer.Read(buff, 0, buff.Length);

//      Assert.IsTrue(buff.SequenceEqual(data));

//      _fromBuffer.Write(buff, 0, buff.Length);

//      buff = new byte[5];

//      _fromBuffer.Read(buff, 0, buff.Length);

//      Assert.IsTrue(buff.SequenceEqual(data));
//    }
//  }
//}
