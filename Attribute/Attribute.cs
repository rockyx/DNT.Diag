using System;

namespace DNT.Diag.Attribute
{
  internal class Attribute
  {
    #region Common
    
    public byte[] HeartbeatBytes { get; set; }
    
    #endregion

    #region K Line

    public Parity KLineParity { get; set; }

    public bool KLineLLine { get; set; }
    public int KLineAddrCode { get; set; }
    public int KLineTargetAddress { get; set; }
    public int KLineSourceAddress { get; set; }
    int KLineBaudRate { get; set; }
    int KLineComLine { get; set; }

    #region KWP 2000

    public KWP2KStartType KWP2KStartType { get; set; }
    public KWP2KMode KWP2KHeartbeatMode { get; set; }
    public KWP2KMode KWP2KMsgMode { get; set; }
    public KWP2KMode KWP2KCurrentMode { get; set; }
    public byte[] KWP2KFastCmd { get; set; }

    #endregion

    #region ISO9141

    public int ISOHeader { get; set; }

    #endregion

    #endregion

    #region Canbus

    public int IdForEcuRecv { get; set; }
    public CanbusBaudRate CanbusBaudRate { get; set; }
    public CanbusIDMode CanbusIdMode { get; set; }
    public CanbusFilterMask CanbusFilterMask { get; set; }
    public CanbusFrameType CanbusFrameType { get; set; }
    public int CanbusHighPin { get; set; }
    public int CanbusLowPin { get; set; }
    public int CanbusIdRecvFilters { get; set; }
    public byte[] CanbusFlowControl { get; set; }


    #endregion
  }
}
