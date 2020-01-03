using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NVIDIA.VRWorksAudio.Internal;

[RequireComponent(typeof(AudioSource))]
public class NVAR_Source : MonoBehaviour {
    private AudioSource audioSource;
    private NVAR.Source NVARsource;
    private NVAR_Listener listener; 
    private float[][] audioDataOut;
    private float[] audioDataIn;
    // Use this for initialization
    void Start () {
        listener = Camera.main.gameObject.GetComponent<NVAR_Listener>();
        audioSource = GetComponent<AudioSource>();
        audioDataIn = new float[audioSource.clip.samples];
        Debug.Log(listener.context.isNull);
        Debug.Log(NVAR.CreateSource(listener.context, NVAR.EffectPreset.Medium, out NVARsource));
        audioSource.clip.GetData(audioDataIn, 0);
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown("down"))
        {
            sendAudio();
        }
        NVAR.SetSourceLocation(NVARsource, gameObject.transform.position);
    }
    private void sendAudio ()
    {
        Debug.Log(NVAR.ApplySourceFilters(NVARsource, out audioDataOut, audioDataIn, audioSource.clip.samples));
        NVAR.ApplySourceFilters(NVARsource, out audioDataOut, audioDataIn, audioSource.clip.samples);
        listener.SetData(audioDataOut);
    }
}
