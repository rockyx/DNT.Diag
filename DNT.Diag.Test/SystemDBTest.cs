using System;
using System.IO;
using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using DNT.Diag.DB;
using DNT.Diag;

namespace DNT.Diag.Test
{
  [TestClass]
  public class SystemDBTest
  {
    private string zhCNReg = @"whVht1so+a3p8MBQlnRUnJQF+GEVrAVajXEnFJFX5FqbCnHCVPWToT2sRtxHqwvT2qNOLwLk
OifpSMGhKH8Dh2rfK5f9DMtttQ+eFP3tDd+bQcEx+UOsuuomppZJQILFUzFGnskVy0xqB64+
x0jK54XTnqRCMTtdLfePzR26O1I3sZ7H4Vf9Jy19ijT0ovepadCqcR9OngnzzxZXC2loOh6c
1HEpM7jLMjTQPRObt9BFXYtweHcP6YxSDeg7RiZhYmTKQb0tk5PRq+E/6WGmalwDzzTp3TsQ
S66Ch34Tb/g3NBx/9Des+eeRt+OT8i2gmhaJKvdKEylJGXFy+RCy+g==";
    private string enUSReg = @"FL/u8PI5ocI2n5jZ7AzsSu7eLDzVu3b2FU8VjtBVbRrzmuil9ZBXMj/nRCDDbICN9eOpK3NT
2h/LXOBYjQIvTsBxoD4+/Vkox9iwbvOvE5h2rpWoV+CXTWg7RFgox+KnZtVDToVRqV9Vaknx
Jb1uyc/VlF+AuElJAw7g4m/2Wse9YbtYzAQ1v7GiX/vuJHrspqNMiTO2tDUXZZ7tdk09asFZ
XuXoVRdjYf1MqKXzL65az0vecmt/hplcg4SPjzM1FUxTQmVXVDnTSUihD2+m4c6kJxQ0TjH+
6m3o3Mv5cG+mjzIS9TnClsoZaYIBjvNZKg7zrfQ7pBCB0d9dQ+MRRA==";
    private string zhCNPath = Path.GetFullPath("zh-CN");
    private string enUSPath = Path.GetFullPath("en-US");

    [TestInitialize]
    public void Initialize()
    {
      if (Directory.Exists("zh-CN"))
        Directory.Delete("zh-CN", true);
      Directory.CreateDirectory("zh-CN");
      if (Directory.Exists("en-US"))
        Directory.Delete("en-US", true);
      Directory.CreateDirectory("en-US");

      // Create and save reg code in zh-CN/demo.dat
      using (FileStream fs = File.Create("zh-CN//demo.dat"))
      {
        byte[] utf8 = UTF8Encoding.UTF8.GetBytes(zhCNReg);
        fs.Write(utf8, 0, utf8.Length);
      }

      // Create and save reg code in en-US/demo.dat
      using (FileStream fs = File.Create("en-US//demo.dat"))
      {
        byte[] utf8 = UTF8Encoding.UTF8.GetBytes(enUSReg);
        fs.Write(utf8, 0, utf8.Length);
      }
      SystemDB.Init(Directory.GetCurrentDirectory());
    }

    [TestMethod]
    public void QueryText()
    {
      string queryName = "Communicating";
      string zhCNExpect = "正在通讯...";
      string enUSExpect = "Communicating...";

      Register.Init(zhCNPath);
      string result = SystemDB.QueryText(queryName);
      Assert.AreEqual(zhCNExpect, result);

      Register.Init(enUSPath);
      result = SystemDB.QueryText(queryName);
      Assert.AreEqual(enUSExpect, result);
    }
  }
}
