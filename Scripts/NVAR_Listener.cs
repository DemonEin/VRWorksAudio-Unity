using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NVIDIA.VRWorksAudio.Internal;

[RequireComponent(typeof(AudioSource))]
public class NVAR_Listener : MonoBehaviour {
    private AudioSource output;
    internal NVAR.Context context;
    private NVAR.Material material;
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
        output = GetComponent<AudioSource>();
        //NVAR operations
        NVAR.Initialize(0);
        NVAR.Create(out context, "test", NVAR.EffectPreset.Low);
        NVAR.CreateMaterial(context, out material);

        output.clip = AudioClip.Create("test", 220500, 2, 44100, false);
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown("up"))
        { 
            NVAR.TraceAudio(context);
        }
        NVAR.SetListenerLocation(context, gameObject.transform.position);
        NVAR.SetListenerOrientation(context, gameObject.transform.forward, gameObject.transform.up);
    }
}
