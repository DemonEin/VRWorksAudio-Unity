using UnityEngine;
using NVIDIA.VRWorksAudio.Internal;

[RequireComponent(typeof(AudioSource))]
public class NVAR_Source : MonoBehaviour {
    private AudioSource audioSource;
    private NVAR.Source NVARsource;
    private NVAR_Listener listener;
    private AudioClip baseClip;

    private void ApplyFilter()
    {
        int bufferLength = (int)((1 / listener.minFramerate) * baseClip.frequency);
        float[] audioDataIn = new float[bufferLength];
        float[][] audioDataOut;
        baseClip.GetData(audioDataIn, audioSource.timeSamples);
        NVAR.ApplySourceFilters(NVARsource, out audioDataOut, audioDataIn, audioDataIn.Length);
        audioSource.clip.SetData(ParallelToWeaved(audioDataOut), audioSource.timeSamples);
    }
    private float[] ParallelToWeaved(float[][] parallel)
    {
        float[] weaved = new float[parallel.Length * parallel[0].Length];
        int i = 0;
        for (int sample = 0; sample < parallel[0].Length; sample++)
        {
            for (int channel = 0; channel < parallel.Length; channel++)
            {
                weaved[i] = parallel[channel][sample];
                i++;
            }
        }
        return weaved;
    }
    // Use this for initialization
    void Start () {
        listener = Camera.main.gameObject.GetComponent<NVAR_Listener>();
        audioSource = GetComponent<AudioSource>();
        baseClip = audioSource.clip;
        NVAR.CreateSource(listener.context, NVAR.EffectPreset.Medium, out NVARsource);
        audioSource.clip = AudioClip.Create("NVARAudio", baseClip.samples, 2, baseClip.frequency, false);
        audioSource.Play();
    }
	// Update is called once per frame
	void Update () {
        ApplyFilter();
        NVAR.SetSourceLocation(NVARsource, gameObject.transform.position);
    }
}
