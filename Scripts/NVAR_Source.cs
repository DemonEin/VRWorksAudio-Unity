using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NVIDIA.VRWorksAudio.Internal;

[RequireComponent(typeof(AudioSource))]
public class NVAR_Source : MonoBehaviour {
    private AudioSource audioSource;
    private NVAR.Source NVARsource;
    private NVAR_Listener listener;
    private AudioClip baseClip;
    private float[][] audioDataOut;
    private float[] audioDataIn;
    // Use this for initialization
    void Start () {
        listener = Camera.main.gameObject.GetComponent<NVAR_Listener>();
        audioSource = GetComponent<AudioSource>();
        baseClip = audioSource.clip;
        audioDataIn = new float[audioSource.clip.samples];
        NVAR.CreateSource(listener.context, NVAR.EffectPreset.Medium, out NVARsource);
        audioSource.clip = AudioClip.Create("NVARAudio", baseClip.samples, 2, 44100, false);
        audioSource.Play();
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown("down"))
        {
            ApplyFilter();
        }
        ApplyFilter();
        NVAR.SetSourceLocation(NVARsource, gameObject.transform.position);
    }
    private void sendAudio ()
    {
        Debug.Log(NVAR.ApplySourceFilters(NVARsource, out audioDataOut, audioDataIn, audioSource.clip.samples));
        NVAR.ApplySourceFilters(NVARsource, out audioDataOut, audioDataIn, audioSource.clip.samples);
        listener.SetData(audioDataOut);
    }
    private void ApplyFilter()
    {
        int bufferLength = (int) ((1 / listener.minFramerate) * baseClip.frequency);
        float[] audioDataIn = new float[bufferLength];
        float[][] audioDataOut;
        baseClip.GetData(audioDataIn, audioSource.timeSamples);
        NVAR.ApplySourceFilters(NVARsource, out audioDataOut, audioDataIn, audioDataIn.Length);
        audioSource.clip.SetData(ParallelToWeaved(audioDataOut), audioSource.timeSamples);
        //audioSource.clip.SetData(audioDataIn, 0);
    }
    private float[] ParallelToWeaved (float[][] parallel)
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
        //Debug.Log(weaved.Length);
        return weaved;
    }
}
