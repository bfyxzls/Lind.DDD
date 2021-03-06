/**
 * Autogenerated by Thrift Compiler (0.9.1)
 *
 * DO NOT EDIT UNLESS YOU ARE SURE THAT YOU KNOW WHAT YOU ARE DOING
 *  @generated
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Thrift;
using Thrift.Collections;
using System.Runtime.Serialization;
using Thrift.Protocol;
using Thrift.Transport;

namespace HelloThriftspace
{
  public partial class HelloThrift {
    public interface Iface {
      void HelloWorld();
      #if SILVERLIGHT
      IAsyncResult Begin_HelloWorld(AsyncCallback callback, object state);
      void End_HelloWorld(IAsyncResult asyncResult);
      #endif
      string GetData(int uid);
      #if SILVERLIGHT
      IAsyncResult Begin_GetData(AsyncCallback callback, object state, int uid);
      string End_GetData(IAsyncResult asyncResult);
      #endif
      int adding(int a, int b);
      #if SILVERLIGHT
      IAsyncResult Begin_adding(AsyncCallback callback, object state, int a, int b);
      int End_adding(IAsyncResult asyncResult);
      #endif
    }

    public class Client : IDisposable, Iface {
      public Client(TProtocol prot) : this(prot, prot)
      {
      }

      public Client(TProtocol iprot, TProtocol oprot)
      {
        iprot_ = iprot;
        oprot_ = oprot;
      }

      protected TProtocol iprot_;
      protected TProtocol oprot_;
      protected int seqid_;

      public TProtocol InputProtocol
      {
        get { return iprot_; }
      }
      public TProtocol OutputProtocol
      {
        get { return oprot_; }
      }


      #region " IDisposable Support "
      private bool _IsDisposed;

      // IDisposable
      public void Dispose()
      {
        Dispose(true);
      }
      

      protected virtual void Dispose(bool disposing)
      {
        if (!_IsDisposed)
        {
          if (disposing)
          {
            if (iprot_ != null)
            {
              ((IDisposable)iprot_).Dispose();
            }
            if (oprot_ != null)
            {
              ((IDisposable)oprot_).Dispose();
            }
          }
        }
        _IsDisposed = true;
      }
      #endregion


      
      #if SILVERLIGHT
      public IAsyncResult Begin_HelloWorld(AsyncCallback callback, object state)
      {
        return send_HelloWorld(callback, state);
      }

      public void End_HelloWorld(IAsyncResult asyncResult)
      {
        oprot_.Transport.EndFlush(asyncResult);
        recv_HelloWorld();
      }

      #endif

      public void HelloWorld()
      {
        #if !SILVERLIGHT
        send_HelloWorld();
        recv_HelloWorld();

        #else
        var asyncResult = Begin_HelloWorld(null, null);
        End_HelloWorld(asyncResult);

        #endif
      }
      #if SILVERLIGHT
      public IAsyncResult send_HelloWorld(AsyncCallback callback, object state)
      #else
      public void send_HelloWorld()
      #endif
      {
        oprot_.WriteMessageBegin(new TMessage("HelloWorld", TMessageType.Call, seqid_));
        HelloWorld_args args = new HelloWorld_args();
        args.Write(oprot_);
        oprot_.WriteMessageEnd();
        #if SILVERLIGHT
        return oprot_.Transport.BeginFlush(callback, state);
        #else
        oprot_.Transport.Flush();
        #endif
      }

      public void recv_HelloWorld()
      {
        TMessage msg = iprot_.ReadMessageBegin();
        if (msg.Type == TMessageType.Exception) {
          TApplicationException x = TApplicationException.Read(iprot_);
          iprot_.ReadMessageEnd();
          throw x;
        }
        HelloWorld_result result = new HelloWorld_result();
        result.Read(iprot_);
        iprot_.ReadMessageEnd();
        return;
      }

      
      #if SILVERLIGHT
      public IAsyncResult Begin_GetData(AsyncCallback callback, object state, int uid)
      {
        return send_GetData(callback, state, uid);
      }

      public string End_GetData(IAsyncResult asyncResult)
      {
        oprot_.Transport.EndFlush(asyncResult);
        return recv_GetData();
      }

      #endif

      public string GetData(int uid)
      {
        #if !SILVERLIGHT
        send_GetData(uid);
        return recv_GetData();

        #else
        var asyncResult = Begin_GetData(null, null, uid);
        return End_GetData(asyncResult);

        #endif
      }
      #if SILVERLIGHT
      public IAsyncResult send_GetData(AsyncCallback callback, object state, int uid)
      #else
      public void send_GetData(int uid)
      #endif
      {
        oprot_.WriteMessageBegin(new TMessage("GetData", TMessageType.Call, seqid_));
        GetData_args args = new GetData_args();
        args.Uid = uid;
        args.Write(oprot_);
        oprot_.WriteMessageEnd();
        #if SILVERLIGHT
        return oprot_.Transport.BeginFlush(callback, state);
        #else
        oprot_.Transport.Flush();
        #endif
      }

      public string recv_GetData()
      {
        TMessage msg = iprot_.ReadMessageBegin();
        if (msg.Type == TMessageType.Exception) {
          TApplicationException x = TApplicationException.Read(iprot_);
          iprot_.ReadMessageEnd();
          throw x;
        }
        GetData_result result = new GetData_result();
        result.Read(iprot_);
        iprot_.ReadMessageEnd();
        if (result.__isset.success) {
          return result.Success;
        }
        throw new TApplicationException(TApplicationException.ExceptionType.MissingResult, "GetData failed: unknown result");
      }

      
      #if SILVERLIGHT
      public IAsyncResult Begin_adding(AsyncCallback callback, object state, int a, int b)
      {
        return send_adding(callback, state, a, b);
      }

      public int End_adding(IAsyncResult asyncResult)
      {
        oprot_.Transport.EndFlush(asyncResult);
        return recv_adding();
      }

      #endif

      public int adding(int a, int b)
      {
        #if !SILVERLIGHT
        send_adding(a, b);
        return recv_adding();

        #else
        var asyncResult = Begin_adding(null, null, a, b);
        return End_adding(asyncResult);

        #endif
      }
      #if SILVERLIGHT
      public IAsyncResult send_adding(AsyncCallback callback, object state, int a, int b)
      #else
      public void send_adding(int a, int b)
      #endif
      {
        oprot_.WriteMessageBegin(new TMessage("adding", TMessageType.Call, seqid_));
        adding_args args = new adding_args();
        args.A = a;
        args.B = b;
        args.Write(oprot_);
        oprot_.WriteMessageEnd();
        #if SILVERLIGHT
        return oprot_.Transport.BeginFlush(callback, state);
        #else
        oprot_.Transport.Flush();
        #endif
      }

      public int recv_adding()
      {
        TMessage msg = iprot_.ReadMessageBegin();
        if (msg.Type == TMessageType.Exception) {
          TApplicationException x = TApplicationException.Read(iprot_);
          iprot_.ReadMessageEnd();
          throw x;
        }
        adding_result result = new adding_result();
        result.Read(iprot_);
        iprot_.ReadMessageEnd();
        if (result.__isset.success) {
          return result.Success;
        }
        throw new TApplicationException(TApplicationException.ExceptionType.MissingResult, "adding failed: unknown result");
      }

    }
    public class Processor : TProcessor {
      public Processor(Iface iface)
      {
        iface_ = iface;
        processMap_["HelloWorld"] = HelloWorld_Process;
        processMap_["GetData"] = GetData_Process;
        processMap_["adding"] = adding_Process;
      }

      protected delegate void ProcessFunction(int seqid, TProtocol iprot, TProtocol oprot);
      private Iface iface_;
      protected Dictionary<string, ProcessFunction> processMap_ = new Dictionary<string, ProcessFunction>();

      public bool Process(TProtocol iprot, TProtocol oprot)
      {
        try
        {
          TMessage msg = iprot.ReadMessageBegin();
          ProcessFunction fn;
          processMap_.TryGetValue(msg.Name, out fn);
          if (fn == null) {
            TProtocolUtil.Skip(iprot, TType.Struct);
            iprot.ReadMessageEnd();
            TApplicationException x = new TApplicationException (TApplicationException.ExceptionType.UnknownMethod, "Invalid method name: '" + msg.Name + "'");
            oprot.WriteMessageBegin(new TMessage(msg.Name, TMessageType.Exception, msg.SeqID));
            x.Write(oprot);
            oprot.WriteMessageEnd();
            oprot.Transport.Flush();
            return true;
          }
          fn(msg.SeqID, iprot, oprot);
        }
        catch (IOException)
        {
          return false;
        }
        return true;
      }

      public void HelloWorld_Process(int seqid, TProtocol iprot, TProtocol oprot)
      {
        HelloWorld_args args = new HelloWorld_args();
        args.Read(iprot);
        iprot.ReadMessageEnd();
        HelloWorld_result result = new HelloWorld_result();
        iface_.HelloWorld();
        oprot.WriteMessageBegin(new TMessage("HelloWorld", TMessageType.Reply, seqid)); 
        result.Write(oprot);
        oprot.WriteMessageEnd();
        oprot.Transport.Flush();
      }

      public void GetData_Process(int seqid, TProtocol iprot, TProtocol oprot)
      {
        GetData_args args = new GetData_args();
        args.Read(iprot);
        iprot.ReadMessageEnd();
        GetData_result result = new GetData_result();
        result.Success = iface_.GetData(args.Uid);
        oprot.WriteMessageBegin(new TMessage("GetData", TMessageType.Reply, seqid)); 
        result.Write(oprot);
        oprot.WriteMessageEnd();
        oprot.Transport.Flush();
      }

      public void adding_Process(int seqid, TProtocol iprot, TProtocol oprot)
      {
        adding_args args = new adding_args();
        args.Read(iprot);
        iprot.ReadMessageEnd();
        adding_result result = new adding_result();
        result.Success = iface_.adding(args.A, args.B);
        oprot.WriteMessageBegin(new TMessage("adding", TMessageType.Reply, seqid)); 
        result.Write(oprot);
        oprot.WriteMessageEnd();
        oprot.Transport.Flush();
      }

    }


    #if !SILVERLIGHT
    [Serializable]
    #endif
    public partial class HelloWorld_args : TBase
    {

      public HelloWorld_args() {
      }

      public void Read (TProtocol iprot)
      {
        TField field;
        iprot.ReadStructBegin();
        while (true)
        {
          field = iprot.ReadFieldBegin();
          if (field.Type == TType.Stop) { 
            break;
          }
          switch (field.ID)
          {
            default: 
              TProtocolUtil.Skip(iprot, field.Type);
              break;
          }
          iprot.ReadFieldEnd();
        }
        iprot.ReadStructEnd();
      }

      public void Write(TProtocol oprot) {
        TStruct struc = new TStruct("HelloWorld_args");
        oprot.WriteStructBegin(struc);
        oprot.WriteFieldStop();
        oprot.WriteStructEnd();
      }

      public override string ToString() {
        StringBuilder sb = new StringBuilder("HelloWorld_args(");
        sb.Append(")");
        return sb.ToString();
      }

    }


    #if !SILVERLIGHT
    [Serializable]
    #endif
    public partial class HelloWorld_result : TBase
    {

      public HelloWorld_result() {
      }

      public void Read (TProtocol iprot)
      {
        TField field;
        iprot.ReadStructBegin();
        while (true)
        {
          field = iprot.ReadFieldBegin();
          if (field.Type == TType.Stop) { 
            break;
          }
          switch (field.ID)
          {
            default: 
              TProtocolUtil.Skip(iprot, field.Type);
              break;
          }
          iprot.ReadFieldEnd();
        }
        iprot.ReadStructEnd();
      }

      public void Write(TProtocol oprot) {
        TStruct struc = new TStruct("HelloWorld_result");
        oprot.WriteStructBegin(struc);

        oprot.WriteFieldStop();
        oprot.WriteStructEnd();
      }

      public override string ToString() {
        StringBuilder sb = new StringBuilder("HelloWorld_result(");
        sb.Append(")");
        return sb.ToString();
      }

    }


    #if !SILVERLIGHT
    [Serializable]
    #endif
    public partial class GetData_args : TBase
    {
      private int _uid;

      public int Uid
      {
        get
        {
          return _uid;
        }
        set
        {
          __isset.uid = true;
          this._uid = value;
        }
      }


      public Isset __isset;
      #if !SILVERLIGHT
      [Serializable]
      #endif
      public struct Isset {
        public bool uid;
      }

      public GetData_args() {
      }

      public void Read (TProtocol iprot)
      {
        TField field;
        iprot.ReadStructBegin();
        while (true)
        {
          field = iprot.ReadFieldBegin();
          if (field.Type == TType.Stop) { 
            break;
          }
          switch (field.ID)
          {
            case 1:
              if (field.Type == TType.I32) {
                Uid = iprot.ReadI32();
              } else { 
                TProtocolUtil.Skip(iprot, field.Type);
              }
              break;
            default: 
              TProtocolUtil.Skip(iprot, field.Type);
              break;
          }
          iprot.ReadFieldEnd();
        }
        iprot.ReadStructEnd();
      }

      public void Write(TProtocol oprot) {
        TStruct struc = new TStruct("GetData_args");
        oprot.WriteStructBegin(struc);
        TField field = new TField();
        if (__isset.uid) {
          field.Name = "uid";
          field.Type = TType.I32;
          field.ID = 1;
          oprot.WriteFieldBegin(field);
          oprot.WriteI32(Uid);
          oprot.WriteFieldEnd();
        }
        oprot.WriteFieldStop();
        oprot.WriteStructEnd();
      }

      public override string ToString() {
        StringBuilder sb = new StringBuilder("GetData_args(");
        sb.Append("Uid: ");
        sb.Append(Uid);
        sb.Append(")");
        return sb.ToString();
      }

    }


    #if !SILVERLIGHT
    [Serializable]
    #endif
    public partial class GetData_result : TBase
    {
      private string _success;

      public string Success
      {
        get
        {
          return _success;
        }
        set
        {
          __isset.success = true;
          this._success = value;
        }
      }


      public Isset __isset;
      #if !SILVERLIGHT
      [Serializable]
      #endif
      public struct Isset {
        public bool success;
      }

      public GetData_result() {
      }

      public void Read (TProtocol iprot)
      {
        TField field;
        iprot.ReadStructBegin();
        while (true)
        {
          field = iprot.ReadFieldBegin();
          if (field.Type == TType.Stop) { 
            break;
          }
          switch (field.ID)
          {
            case 0:
              if (field.Type == TType.String) {
                Success = iprot.ReadString();
              } else { 
                TProtocolUtil.Skip(iprot, field.Type);
              }
              break;
            default: 
              TProtocolUtil.Skip(iprot, field.Type);
              break;
          }
          iprot.ReadFieldEnd();
        }
        iprot.ReadStructEnd();
      }

      public void Write(TProtocol oprot) {
        TStruct struc = new TStruct("GetData_result");
        oprot.WriteStructBegin(struc);
        TField field = new TField();

        if (this.__isset.success) {
          if (Success != null) {
            field.Name = "Success";
            field.Type = TType.String;
            field.ID = 0;
            oprot.WriteFieldBegin(field);
            oprot.WriteString(Success);
            oprot.WriteFieldEnd();
          }
        }
        oprot.WriteFieldStop();
        oprot.WriteStructEnd();
      }

      public override string ToString() {
        StringBuilder sb = new StringBuilder("GetData_result(");
        sb.Append("Success: ");
        sb.Append(Success);
        sb.Append(")");
        return sb.ToString();
      }

    }


    #if !SILVERLIGHT
    [Serializable]
    #endif
    public partial class adding_args : TBase
    {
      private int _a;
      private int _b;

      public int A
      {
        get
        {
          return _a;
        }
        set
        {
          __isset.a = true;
          this._a = value;
        }
      }

      public int B
      {
        get
        {
          return _b;
        }
        set
        {
          __isset.b = true;
          this._b = value;
        }
      }


      public Isset __isset;
      #if !SILVERLIGHT
      [Serializable]
      #endif
      public struct Isset {
        public bool a;
        public bool b;
      }

      public adding_args() {
      }

      public void Read (TProtocol iprot)
      {
        TField field;
        iprot.ReadStructBegin();
        while (true)
        {
          field = iprot.ReadFieldBegin();
          if (field.Type == TType.Stop) { 
            break;
          }
          switch (field.ID)
          {
            case 1:
              if (field.Type == TType.I32) {
                A = iprot.ReadI32();
              } else { 
                TProtocolUtil.Skip(iprot, field.Type);
              }
              break;
            case 2:
              if (field.Type == TType.I32) {
                B = iprot.ReadI32();
              } else { 
                TProtocolUtil.Skip(iprot, field.Type);
              }
              break;
            default: 
              TProtocolUtil.Skip(iprot, field.Type);
              break;
          }
          iprot.ReadFieldEnd();
        }
        iprot.ReadStructEnd();
      }

      public void Write(TProtocol oprot) {
        TStruct struc = new TStruct("adding_args");
        oprot.WriteStructBegin(struc);
        TField field = new TField();
        if (__isset.a) {
          field.Name = "a";
          field.Type = TType.I32;
          field.ID = 1;
          oprot.WriteFieldBegin(field);
          oprot.WriteI32(A);
          oprot.WriteFieldEnd();
        }
        if (__isset.b) {
          field.Name = "b";
          field.Type = TType.I32;
          field.ID = 2;
          oprot.WriteFieldBegin(field);
          oprot.WriteI32(B);
          oprot.WriteFieldEnd();
        }
        oprot.WriteFieldStop();
        oprot.WriteStructEnd();
      }

      public override string ToString() {
        StringBuilder sb = new StringBuilder("adding_args(");
        sb.Append("A: ");
        sb.Append(A);
        sb.Append(",B: ");
        sb.Append(B);
        sb.Append(")");
        return sb.ToString();
      }

    }


    #if !SILVERLIGHT
    [Serializable]
    #endif
    public partial class adding_result : TBase
    {
      private int _success;

      public int Success
      {
        get
        {
          return _success;
        }
        set
        {
          __isset.success = true;
          this._success = value;
        }
      }


      public Isset __isset;
      #if !SILVERLIGHT
      [Serializable]
      #endif
      public struct Isset {
        public bool success;
      }

      public adding_result() {
      }

      public void Read (TProtocol iprot)
      {
        TField field;
        iprot.ReadStructBegin();
        while (true)
        {
          field = iprot.ReadFieldBegin();
          if (field.Type == TType.Stop) { 
            break;
          }
          switch (field.ID)
          {
            case 0:
              if (field.Type == TType.I32) {
                Success = iprot.ReadI32();
              } else { 
                TProtocolUtil.Skip(iprot, field.Type);
              }
              break;
            default: 
              TProtocolUtil.Skip(iprot, field.Type);
              break;
          }
          iprot.ReadFieldEnd();
        }
        iprot.ReadStructEnd();
      }

      public void Write(TProtocol oprot) {
        TStruct struc = new TStruct("adding_result");
        oprot.WriteStructBegin(struc);
        TField field = new TField();

        if (this.__isset.success) {
          field.Name = "Success";
          field.Type = TType.I32;
          field.ID = 0;
          oprot.WriteFieldBegin(field);
          oprot.WriteI32(Success);
          oprot.WriteFieldEnd();
        }
        oprot.WriteFieldStop();
        oprot.WriteStructEnd();
      }

      public override string ToString() {
        StringBuilder sb = new StringBuilder("adding_result(");
        sb.Append("Success: ");
        sb.Append(Success);
        sb.Append(")");
        return sb.ToString();
      }

    }

  }
}
