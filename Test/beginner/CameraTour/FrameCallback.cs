using System.Runtime.InteropServices;
using Gvitech.CityMaker.FdeCore;

namespace CameraTour
{

    public delegate bool MouseButtonHandler(int x, int y, int buttonAction, int flags, int scroll);
    public delegate bool KeyHandler(uint keySymbol, byte keyMask);
    public delegate void FrameHandler(int frameIndex, double refTime);

    [ComVisible(true)]
    public class FrameCallback
    {
        public MouseButtonHandler onMouseButton;
        public KeyHandler onKey;
        public FrameHandler onFrame;

        public bool OnMouseButton(int x, int y, int buttonAction, int flags, int scroll)
        {
            if (onMouseButton != null)
                return onMouseButton(x, y, buttonAction, flags, scroll);
            else
                return false;
        }

        public bool OnKey(uint keySymbol, byte keyMask)
        {
            if (onKey != null)
                return onKey(keySymbol, keyMask);
            else
                return false;
        }

        public void OnFrame(int frameIndex, double refTime)
        {
            if (onFrame != null)
                onFrame(frameIndex, refTime);
        }
    }


    [ComVisible(true)]
    public class ReplicationStatusChanged
    {
        public bool OnReplicating(IFeatureProgress Progress)
        {
            //...
            return true;
        }
    }
}
