using System;
using System.IO;
using System.Data;

namespace Lind.DDD.Utils.Encryptor
{

    internal class Base64Encryptor
    {
        public string EncryptString(string Message)
        {
            char[] Base64Code=new char[]{'A','B','C','D','E','F','G','H','I','J','K','L','M','N','O','P','Q','R','S','T',
                     'U','V','W','X','Y','Z','a','b','c','d','e','f','g','h','i','j','k','l','m','n',
                     'o','p','q','r','s','t','u','v','w','x','y','z','0','1','2','3','4','5','6','7',
                     '8','9','+','/','='};
            byte empty=(byte)0;
            System.Collections.ArrayList byteMessage=new  System.Collections.ArrayList(System.Text.Encoding.Default.GetBytes(Message));
            System.Text.StringBuilder outmessage;
            int messageLen=byteMessage.Count;
            int page=messageLen/3;
            int use=0;
            if((use=messageLen%3)>0)
             {
              for(int i=0;i<3-use;i++)
               byteMessage.Add(empty);
              page++;
             }
            outmessage=new System.Text.StringBuilder(page*4);
            for(int i=0;i<page;i++)
             {
              byte[] instr = new byte[3];
              instr[0]=(byte)byteMessage[i*3];
              instr[1]=(byte)byteMessage[i*3+1];
              instr[2]=(byte)byteMessage[i*3+2];
              int[] outstr=new int[4];
              outstr[0]=instr[0]>>2;
              outstr[1]=((instr[0]&0x03)<<4)^(instr[1]>>4);
              if(!instr[1].Equals(empty))
               outstr[2]=((instr[1]&0x0f)<<2)^(instr[2]>>6);
              else
               outstr[2]=64;
              if(!instr[2].Equals(empty))
                        outstr[3]=(instr[2]&0x3f);
              else
               outstr[3]=64;
              outmessage.Append(Base64Code[outstr[0]]);
              outmessage.Append(Base64Code[outstr[1]]);
              outmessage.Append(Base64Code[outstr[2]]);
              outmessage.Append(Base64Code[outstr[3]]);
             }
             return outmessage.ToString();
        }


        public string DecryptString(string Message)
    {
         if((Message.Length%4)!=0){
          throw new ArgumentException("不是正确的BASE64编码，请检查。","Message");
         }
         if(!System.Text.RegularExpressions.Regex.IsMatch(Message,"^[A-Z0-9/+=]*$",System.Text.RegularExpressions.RegexOptions.IgnoreCase)){
          throw new ArgumentException("包含不正确的BASE64编码，请检查。","Message");
         }
         string Base64Code="ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=";
         int page=Message.Length/4;
         System.Collections.ArrayList outMessage=new System.Collections.ArrayList(page*3);
         char[] message=Message.ToCharArray();
         for(int i=0;i<page;i++)
         {
          byte[] instr=new byte[4];
          instr[0]=(byte)Base64Code.IndexOf(message[i*4]);
          instr[1]=(byte)Base64Code.IndexOf(message[i*4+1]);
          instr[2]=(byte)Base64Code.IndexOf(message[i*4+2]);
          instr[3]=(byte)Base64Code.IndexOf(message[i*4+3]);
          byte[] outstr=new byte[3];
          outstr[0]=(byte)((instr[0]<<2)^((instr[1]&0x30)>>4));
          if(instr[2]!=64)
          {
           outstr[1]=(byte)((instr[1]<<4)^((instr[2]&0x3c)>>2));
          }
          else
          {
           outstr[2]=0;
          }
          if(instr[3]!=64)
          {
           outstr[2]=(byte)((instr[2]<<6)^instr[3]);
          }
          else
          {
           outstr[2]=0;
          }
          outMessage.Add(outstr[0]);
          if(outstr[1]!=0)
           outMessage.Add(outstr[1]);
          if(outstr[2]!=0)
           outMessage.Add(outstr[2]);
         }
         byte[] outbyte=(byte[])outMessage.ToArray(Type.GetType("System.Byte"));
         return System.Text.Encoding.Default.GetString(outbyte);
         }
     }

}
