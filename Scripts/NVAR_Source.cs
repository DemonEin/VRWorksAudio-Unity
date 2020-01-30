using UnityEngine;
using NVIDIA.VRWorksAudio.Internal;

[RequireComponent(typeof(AudioSource))]
public class NVAR_Source : MonoBehaviour {
    private AudioSource audioSource;
    private NVAR.Source NVARsource;
    private NVAR_Listener listener;
    private AudioClip baseClip;
    private int frameCount;
    public int smoothingLength;
    public int updateLength;

    private int NextZeroCrossing()
    {
        float[] samples = new float[4];
        for (int i = audioSource.timeSamples; i < audioSource.timeSamples + 1000; i += 1)
        {
            audioSource.clip.GetData(samples, i);
            if (samples[0] < 0 && samples[2] > 0) return i;
        }
        Debug.LogError("Failed to find zero crossing");
        return audioSource.timeSamples;
    }
    private void ApplyFilter()
    {
        int bufferLength = (int)((1 / listener.minFramerate) * updateLength * baseClip.frequency);
        float[] audioDataIn = new float[bufferLength];
        float[][] audioDataOut;
        int filterStart = NextZeroCrossing();
        baseClip.GetData(audioDataIn, filterStart);
        NVAR.ApplySourceFilters(NVARsource, out audioDataOut, audioDataIn, audioDataIn.Length);
        NVAR.ApplySourceFilters(NVARsource, out audioDataOut, audioDataIn, audioDataIn.Length); //might be unneccessary
        audioSource.clip.SetData(ParallelToWeaved(audioDataOut), filterStart);
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
    private void SmoothSetData (float[] audioDataIn, int offset)
    {
        float[] oldAudio = new float[smoothingLength * 2];
        audioSource.clip.GetData(oldAudio, offset);
        for (int i = 0; i < smoothingLength * 2; i++)
        {
            audioDataIn[i] = (oldAudio[i] + audioDataIn[i]) / 2;
        }
        audioSource.clip.SetData(audioDataIn, offset);
    }
    // Use this for initialization
    void Start () {
        listener = Camera.main.gameObject.GetComponent<NVAR_Listener>();
        audioSource = GetComponent<AudioSource>();
        baseClip = audioSource.clip;
        NVAR.CreateSource(listener.context, NVAR.EffectPreset.Medium, out NVARsource);
        audioSource.clip = AudioClip.Create("NVARAudio", baseClip.samples, 2, baseClip.frequency, false);
        audioSource.Play();
        frameCount = updateLength;
    }
	// Update is called once per frame
	void Update () {
        if (frameCount == updateLength)
        //if (Input.GetKeyDown("down"))
        {
            ApplyFilter();
            frameCount = 0;
        }
        frameCount++;
        //ApplyFilter();
        NVAR.SetSourceLocation(NVARsource, gameObject.transform.position);
    }
}
