using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

//  参考URL)
//  http://asaconsultant.blogspot.jp/2014/05/toying-with-audio-in-powershell.html

namespace CLTools.Class
{
    [Guid("5CDF2C82-841E-4546-9722-0CF74078229A"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    interface IAudioEndpointVolume
    {
        int f();
        int g();
        int h();
        int i();
        int SetMasterVolumeLevelScalar(float fLevel, Guid pguidEventContext);
        int j();
        int GetMasterVolumeLevelScalar(out float pfLevel);
        int k();
        int l();
        int m();
        int n();
        int SetMute([MarshalAs(UnmanagedType.Bool)]bool bMute, Guid pguidEventContext);
        int GetMute(out bool pbMute);
    }

    [Guid("D666063F-1587-4E43-81F1-B948E807363F"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    interface IMMDevice
    {
        int Activate(ref Guid id, int clsCtx, int activationParams, out IAudioEndpointVolume aev);
    }

    [Guid("A95664D2-9614-4F35-A746-DE8DB63617E6"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    interface IMMDeviceEnumerator
    {
        int f();
        int GetDefaultAudioEndpoint(int dataFlow, int role, out IMMDevice endpoint);
    }

    [ComImport, Guid("BCDE0395-E52F-467C-8E3D-C4579291692E")] class MMDeviceEnumeratorComObject { }


    class Sound
    {
        private static float Volume
        {
            get
            {
                Marshal.ThrowExceptionForHR(Vol().GetMasterVolumeLevelScalar(out float volume));
                return volume;
            }
            set
            {
                Marshal.ThrowExceptionForHR(Vol().SetMasterVolumeLevelScalar(value, Guid.Empty));
            }
        }
        public static void SetVolume(float volume)
        {
            Marshal.ThrowExceptionForHR(Vol().SetMasterVolumeLevelScalar(volume, Guid.Empty));
        }
        public static float GetVolume()
        {
            Marshal.ThrowExceptionForHR(Vol().GetMasterVolumeLevelScalar(out float volume));
            return volume;
        }

        private static bool Mute
        {
            get
            {
                Marshal.ThrowExceptionForHR(Vol().GetMute(out bool mute));
                return mute;
            }
            set
            {
                Marshal.ThrowExceptionForHR(Vol().SetMute(value, Guid.Empty));
            }
        }
        public static void SetMute(bool mute)
        {
            Marshal.ThrowExceptionForHR(Vol().SetMute(mute, Guid.Empty));
        }
        public static bool GetMute()
        {
            Marshal.ThrowExceptionForHR(Vol().GetMute(out bool mute));
            return mute;
        }

        private static IAudioEndpointVolume Vol()
        {
            IMMDeviceEnumerator enumerator = new MMDeviceEnumeratorComObject() as IMMDeviceEnumerator;
            IMMDevice dev = null;
            Marshal.ThrowExceptionForHR(enumerator.GetDefaultAudioEndpoint(0, 1, out dev));
            IAudioEndpointVolume epv = null;
            Guid epvid = typeof(IAudioEndpointVolume).GUID;
            Marshal.ThrowExceptionForHR(dev.Activate(ref epvid, 23, 0, out epv));
            return epv;
        }
    }
}