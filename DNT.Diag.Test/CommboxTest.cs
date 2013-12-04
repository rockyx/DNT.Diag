using System;
using System.IO;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using DNT.Diag.IO;

namespace DNT.Diag.Test
{
  [TestClass]
  public class CommboxTest
  {
    private Commbox commbox;

    [TestInitialize]
    public void Initialize()
    {
      commbox = null;
    }

    [TestMethod]
    public void C168CommboxTest()
    {
      try
      {
        commbox = new Commbox(CommboxVer.C168);
        commbox.Connect();
        Thread.Sleep(5000);
        commbox.Disconnect();
      }
      catch (IOException ex)
      {
      }
      finally
      {
        try
        {
          commbox.Disconnect();
        }
        finally
        {
        }
      }
    }
  }
}
