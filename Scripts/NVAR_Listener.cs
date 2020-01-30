using UnityEngine;
using NVIDIA.VRWorksAudio.Internal;
using System.Threading;

public class NVAR_Listener : MonoBehaviour {
    //used to determine the buffer length of filtered audio; if the Framerate goes below minFramerate, the filters will not be applied in time for proper playback; higher improves performance
    public float minFramerate;
    internal NVAR.Context context;

    private EventWaitHandle traceWaitHandle;

	// Use this for initialization
	void Awake () {
        traceWaitHandle = new EventWaitHandle(true, EventResetMode.AutoReset);
        NVAR.Initialize(0);
        NVAR.Create(out context, "VRWA-Unity", NVAR.EffectPreset.Low);
    }
	// Update is called once per frame
	void Update () {
        NVAR.SetListenerLocation(context, gameObject.transform.position);
        NVAR.SetListenerOrientation(context, gameObject.transform.forward, gameObject.transform.up);
        if (traceWaitHandle.WaitOne(0))
        {
            NVAR.TraceAudio(context, traceWaitHandle.SafeWaitHandle.DangerousGetHandle());
        }
    }
    private void OnDisable()
    {
        NVAR.Destroy(context);
        NVAR.Finalize();
    }
}
