using System.Runtime.InteropServices;

namespace Mongu_Audio_Sync
{
    public partial class Form1 : Form
    {

        private const int APPCOMMAND_VOLUME_MUTE = 0x80000;
        private const int APPCOMMAND_VOLUME_UP = 0xA0000;
        private const int APPCOMMAND_VOLUME_DOWN = 0x90000;
        private const int WM_APPCOMMAND = 0x319;

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessageW(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);


        [ComImport]
        [Guid("A95664D2-9614-4F35-A746-DE8DB63617E6"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IMMDeviceEnumerator
        {
            void _VtblGap1_1();
            int GetDefaultAudioEndpoint(int dataFlow, int role, out IMMDevice ppDevice);
        }
        private static class MMDeviceEnumeratorFactory
        {
            public static IMMDeviceEnumerator CreateInstance()
            {
                return (IMMDeviceEnumerator)Activator.CreateInstance(Type.GetTypeFromCLSID(new Guid("BCDE0395-E52F-467C-8E3D-C4579291692E"))); // a MMDeviceEnumerator
            }
        }
        [Guid("D666063F-1587-4E43-81F1-B948E807363F"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IMMDevice
        {
            int Activate([MarshalAs(UnmanagedType.LPStruct)] Guid iid, int dwClsCtx, IntPtr pActivationParams, [MarshalAs(UnmanagedType.IUnknown)] out object ppInterface);
        }

        [Guid("5CDF2C82-841E-4546-9722-0CF74078229A"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IAudioEndpointVolume
        {
            //virtual /* [helpstring] */ HRESULT STDMETHODCALLTYPE RegisterControlChangeNotify(/* [in] */__in IAudioEndpointVolumeCallback *pNotify) = 0;
            int RegisterControlChangeNotify(IntPtr pNotify);
            //virtual /* [helpstring] */ HRESULT STDMETHODCALLTYPE UnregisterControlChangeNotify(/* [in] */ __in IAudioEndpointVolumeCallback *pNotify) = 0;
            int UnregisterControlChangeNotify(IntPtr pNotify);
            //virtual /* [helpstring] */ HRESULT STDMETHODCALLTYPE GetChannelCount(/* [out] */ __out UINT *pnChannelCount) = 0;
            int GetChannelCount(out uint pnChannelCount);
            //virtual /* [helpstring] */ HRESULT STDMETHODCALLTYPE SetMasterVolumeLevel( /* [in] */ __in float fLevelDB,/* [unique][in] */ LPCGUID pguidEventContext) = 0;
            int SetMasterVolumeLevel(float fLevelDB, Guid pguidEventContext);
            //virtual /* [helpstring] */ HRESULT STDMETHODCALLTYPE SetMasterVolumeLevelScalar( /* [in] */ __in float fLevel,/* [unique][in] */ LPCGUID pguidEventContext) = 0;
            int SetMasterVolumeLevelScalar(float fLevel, Guid pguidEventContext);
            //virtual /* [helpstring] */ HRESULT STDMETHODCALLTYPE GetMasterVolumeLevel(/* [out] */ __out float *pfLevelDB) = 0;
            int GetMasterVolumeLevel(out float pfLevelDB);
            //virtual /* [helpstring] */ HRESULT STDMETHODCALLTYPE GetMasterVolumeLevelScalar( /* [out] */ __out float *pfLevel) = 0;
            int GetMasterVolumeLevelScalar(out float pfLevel);
            /// <summary>
            /// Gets the muting state of the audio stream.
            /// </summary>
            /// <param name="isMuted">The muting state. True if the stream is muted, false otherwise.</param>
            /// <returns>An HRESULT code indicating whether the operation passed of failed.</returns>
            [PreserveSig]
            int GetMute([Out][MarshalAs(UnmanagedType.Bool)] out Boolean isMuted);
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int level = 20;
           // if (args.Length == 1) { int.TryParse(args[0], out Level); }

            try
            {
                IMMDeviceEnumerator deviceEnumerator = MMDeviceEnumeratorFactory.CreateInstance();
                IMMDevice speakers;
                const int eRender = 0;
                const int eMultimedia = 1;
                deviceEnumerator.GetDefaultAudioEndpoint(eRender, eMultimedia, out speakers);

                object aepv_obj;
                speakers.Activate(typeof(IAudioEndpointVolume).GUID, 0, IntPtr.Zero, out aepv_obj);
                IAudioEndpointVolume aepv = (IAudioEndpointVolume)aepv_obj;
                Guid ZeroGuid = new Guid();
                int res = aepv.SetMasterVolumeLevelScalar(level / 100f, ZeroGuid);

                button1.Text = $"Audio set for {level}%";
            }
            catch (Exception ex) {
                button1.Text = $"**Could not set audio level** {ex.Message}"; }
        }


        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                IMMDeviceEnumerator deviceEnumerator = MMDeviceEnumeratorFactory.CreateInstance();
                IMMDevice speakers;
                const int eRender = 0;
                const int eMultimedia = 1;
                deviceEnumerator.GetDefaultAudioEndpoint(eRender, eMultimedia, out speakers);

                object aepv_obj;
                speakers.Activate(typeof(IAudioEndpointVolume).GUID, 0, IntPtr.Zero, out aepv_obj);
                IAudioEndpointVolume aepv = (IAudioEndpointVolume)aepv_obj;

                aepv.GetMasterVolumeLevelScalar(out float fLevel);

                button2.Text = $"Audio get for {(int)(fLevel * 100)}%";
            }
            catch (Exception ex)
            {
                button2.Text = $"**Could not set audio level** {ex.Message}";
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                IMMDeviceEnumerator deviceEnumerator = MMDeviceEnumeratorFactory.CreateInstance();
                IMMDevice speakers;
                const int eRender = 0;
                const int eMultimedia = 1;
                deviceEnumerator.GetDefaultAudioEndpoint(eRender, eMultimedia, out speakers);

                object aepv_obj;
                speakers.Activate(typeof(IAudioEndpointVolume).GUID, 0, IntPtr.Zero, out aepv_obj);
                IAudioEndpointVolume aepv = (IAudioEndpointVolume)aepv_obj;

                int isMuteda = aepv.GetMute(out bool a);

                button3.Text = $"Audio get for {isMuteda}%";
            }
            catch (Exception ex)
            {
                button3.Text = $"**Could not set audio level** {ex.Message}";
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            SendMessageW(this.Handle, WM_APPCOMMAND, this.Handle, (IntPtr)APPCOMMAND_VOLUME_UP);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            SendMessageW(this.Handle, WM_APPCOMMAND, this.Handle, (IntPtr)APPCOMMAND_VOLUME_DOWN);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            SendMessageW(this.Handle, WM_APPCOMMAND, this.Handle, (IntPtr)APPCOMMAND_VOLUME_MUTE);
        }
    }
}