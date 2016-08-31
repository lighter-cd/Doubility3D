using System;
using LitJson;

namespace LeapMotionWebsocket
{
    class Header
    {
        public string serviceVersion = "";
        public int version = 0;
    }
    class Event
    {

    }

    public class FrameJson
    {

    }

    internal class Protocol
    {
        Header header;

        public delegate void FrameEventHandler(object sender, FrameJson obj, Frame frame);
        public event FrameEventHandler beforeFrameCreated;
        public event FrameEventHandler afterFrameCreated;

        public static Protocol ChooseProtocol(string message)
        {
            Protocol protocol = null;
            Header header = JsonMapper.ToObject<Header>(message);
            if(header.version >=1 && header.version <= 7) {
                protocol = new Protocol(header);
            }else{
                throw new Exception("unrecognized version");
            }
            return protocol;
        }
        public Protocol(Header header)
        {
            this.header = header;
        }
        public object Decode(string message)
        {
            if (message.IndexOf("event") >= 0)
            {
                Event e = JsonMapper.ToObject<Event>(message);
                return e;
            }else{
                
                FrameJson data = JsonMapper.ToObject<FrameJson>(message);
                beforeFrameCreated(this, data,null);
                Frame frame = new Frame(data);
                afterFrameCreated(this, data, frame);
                return frame;
            }
        }
        public Header Header{
            get {return header;}
        }

        static public string EncodeEnableGestures(bool b)
        {
            return EncodeBool("enableGestures", b);
        }
        static public string EncodeBackground(bool b)
        {
            return EncodeBool("background", b);
        }
        static public string EncodeOptimizeHMD(bool b)
        {
            return EncodeBool("optimizeHMD", b);
        }
        static public string EncodeFocused(bool b)
        {
            return EncodeBool("focused", b);
        }

        static string EncodeBool(string var, bool b)
        {
            return string.Format("{\"{0}\":{1}}",var,b);
        }
    }
}
