using System;

namespace DNT.Diag.Commbox.GL.W80
{
  internal class W80Constant : Constant
  {
    public int BOXINFO_LEN
    {
      get { return 12; }
    }
    public int MAXPORT_NUM
    {
      get { return 4; }
    }
    public int MAXBUFF_NUM
    {
      get { return 4; }
    }
    public int MAXBUFF_LEN
    {
      get { return 0xA8; }
    }
    public override int LINKBLOCK
    {
      get { return 0x40; }
    }

    ///////////////////////////////////////////////////////////////////////////////
    //  接受命令类型定义
    ///////////////////////////////////////////////////////////////////////////////
    public override byte WR_DATA
    {
      get { return 0x00; }
    }
    public override byte WR_LINK
    {
      get { return 0xFF; }
    }
    public override byte STOP_REC
    {
      get { return 0x04; }
    }
    public override byte STOP_EXECUTE
    {
      get { return 0x08; }
    }
    public override byte SET_UPBAUD
    {
      get { return 0x0C; }
    }
    public override byte RESET
    {
      get { return 0x10; }
    }
    public byte GET_CPU
    {
      get { return 0x14; }
    }
    public override byte GET_TIME
    {
      get { return 0x18; }
    }
    public override byte GET_SET
    {
      get { return 0x1C; }
    }
    public byte GET_LINK
    {
      get { return 0x20; }
    }
    public byte GET_BUF
    {
      get { return 0x24; }
    }
    public byte GET_CMD
    {
      get { return 0x28; }
    }
    public override byte GET_PORT
    {
      get { return 0x2C; }
    }
    public byte GET_BOXID
    {
      get { return 0x30; }
    }
    public byte DO_BAT_C
    {
      get { return 0x34; }
    }
    public byte DO_BAT_CN
    {
      get { return 0x38; }
    }
    public byte DO_BAT_L
    {
      get { return 0x3C; }
    }
    public byte DO_BAT_LN
    {
      get { return 0x40; }
    }
    public override byte SET55_BAUD
    {
      get { return 0x44; }
    }
    public override byte SET_ONEBYONE
    {
      get { return 0x48; }
    }
    public byte SET_BAUD
    {
      get { return 0x4C; }
    }
    public byte RUN_LINK
    {
      get { return 0x50; }
    }
    public byte STOP_LINK
    {
      get { return 0x54; }
    }
    public byte CLEAR_LINK
    {
      get { return 0x58; }
    }
    public override byte GET_PORT1
    {
      get { return 0x5C; }
    }
    public override byte SEND_DATA
    {
      get { return 0x60; }
    }
    public byte SET_CTRL
    {
      get { return 0x64; }
    }
    public byte SET_PORT0
    {
      get { return 0x68; }
    }
    public byte SET_PORT1
    {
      get { return 0x6C; }
    }
    public byte SET_PORT2
    {
      get { return 0x70; }
    }
    public byte SET_PORT3
    {
      get { return 0x74; }
    }
    public override byte DELAYSHORT
    {
      get { return 0x78; }
    }
    public override byte DELAYTIME
    {
      get { return 0x7C; }
    }
    public byte DELAYDWORD
    {
      get { return 0x80; }
    }

    public byte COPY_BYTE
    {
      get { return 0x9C; }
    }
    public byte UPDATE_BYTE
    {
      get { return 0xA0; }
    }
    public byte INC_BYTE
    {
      get { return 0xA4; }
    }
    public byte DEC_BYTE
    {
      get { return 0xA8; }
    }
    public byte ADD_BYTE
    {
      get { return 0xAC; }
    }
    public override byte SUB_BYTE
    {
      get { return 0xB0; }
    }
    public byte INVERT_BYTE
    {
      get { return 0xB4; }
    }
    public byte RECV_ERR //接收错误
    {
      get { return 0xAA; }
    }
    public byte RECV_OK //接收正确
    {
      get { return 0x55; }
    }
    public byte BUSY //开始执行
    {
      get { return 0xBB; }
    }
    public byte READY //执行结束
    {
      get { return 0xDD; }
    }
    public byte ERROR //执行错误
    {
      get { return 0xEE; }
    }

    //RF多对一的设定接口,最多16个
    public byte RF_RESET
    {
      get { return 0xD0; }
    }
    public byte RF_SETDTR_L
    {
      get { return 0xD1; }
    }
    public byte RF_SETDTR_H
    {
      get { return 0xD2; }
    }
    public byte RF_SET_BAUD
    {
      get { return 0xD3; }
    }
    public byte RF_SET_ADDR
    {
      get { return 0xD8; }
    }
    public override byte COMMBOXID_ERR
    {
      get { return 1; }
    }
    public override byte DISCONNECT_COMM
    {
      get { return 2; }
    }
    public override byte DISCONNECT_COMMBOX
    {
      get { return 3; }
    }
    public byte OTHER_ERROR
    {
      get { return 4; }
    }

    // 錯誤標識
    public byte ERR_OPEN //OpenComm() 失敗
    {
      get { return 0x01; }
    }
    public byte ERR_CHECK //CheckEcm() 失敗
    {
      get { return 0x02; }
    }

    //接頭標識定義
    public byte OBDII_16
    {
      get { return 0x00; }
    }
    public byte UNIVERSAL_3
    {
      get { return 0x01; }
    }
    public byte BENZ_38
    {
      get { return 0x02; }
    }
    public byte BMW_20
    {
      get { return 0x03; }
    }
    public byte AUDI_4
    {
      get { return 0x04; }
    }
    public byte FIAT_3
    {
      get { return 0x05; }
    }
    public byte CITROEN_2
    {
      get { return 0x06; }
    }
    public byte CHRYSLER_6
    {
      get { return 0x07; }
    }
    public byte TOYOTA_17R
    {
      get { return 0x20; }
    }
    public byte TOYOTA_17F
    {
      get { return 0x21; }
    }
    public byte HONDA_3
    {
      get { return 0x22; }
    }
    public byte MITSUBISHI
    {
      get { return 0x23; }
    }
    public byte HYUNDAI
    {
      get { return 0x23; }
    }
    public byte NISSAN
    {
      get { return 0x24; }
    }
    public byte SUZUKI_3
    {
      get { return 0x25; }
    }
    public byte DAIHATSU_4
    {
      get { return 0x26; }
    }
    public byte ISUZU_3
    {
      get { return 0x27; }
    }
    public byte CANBUS_16
    {
      get { return 0x28; }
    }
    public byte GM_12
    {
      get { return 0x29; }
    }
    public byte KIA_20
    {
      get { return 0x30; }
    }

    //協議常量標誌定義
    public byte NO_PACK //發送的命令不需要打包
    {
      get { return 0x80; }
    }
    public byte UN_PACK //接收到的數據解包處理
    {
      get { return 0x08; }
    }
    public byte MFR_1
    {
      get { return 0x00; }
    }
    public byte MFR_2
    {
      get { return 0x02; }
    }
    public byte MFR_3
    {
      get { return 0x03; }
    }
    public byte MFR_4
    {
      get { return 0x04; }
    }
    public byte MFR_5
    {
      get { return 0x05; }
    }
    public byte MFR_6
    {
      get { return 0x06; }
    }
    public byte MFR_7
    {
      get { return 0x07; }
    }
    public byte MFR_N
    {
      get { return 0x01; }
    }
  }
}
