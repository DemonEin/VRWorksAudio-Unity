using UnityEngine;
using NVIDIA.VRWorksAudio.Internal;
using System.Threading;

[RequireComponent(typeof(AudioSource))]
public class NVAR_Listener : MonoBehaviour {
    //used to determine the buffer length of filtered audio; if the Framerate goes below minFramerate, the filters will not be applied in time for proper playback; higher improves performance
    public readonly float minFramerate = 10;
    private AudioSource output;
    internal NVAR.Context context;
    private EventWaitHandle traceWaitHandle;
    //private AudioClip nextClip;
    public void SetData (float[][] data)
    {
        float[] monoData = new float[data[0].Length * 2];
        Debug.Log(data[0].Length);
        Debug.Log(monoData.Length);
        for (int i = 0; i < data[0].Length; i++)
        {
            monoData[i * 2] = data[0][i];
            monoData[i * 2 + 1] = data[1][i];
        }
        output.clip.SetData(monoData, 0);
        output.Play();
    }
	// Use this for initialization
	void Awake () {
        traceWaitHandle = new EventWaitHandle(true, EventResetMode.AutoReset);
        output = GetComponent<AudioSource>();
        NVAR.Initialize(0);
        NVAR.Create(out context, "test", NVAR.EffectPreset.Low);
        output.clip = AudioClip.Create("test", 220500, 2, 44100, false);
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
}
