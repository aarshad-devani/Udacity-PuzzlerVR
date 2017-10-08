using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OceanProcedural : MonoBehaviour {

    public Material ocean_material;
    // Use this for initialization
    void Start () {
        Ocean.gameObject.SetActive(true);
        Ocean.gameObject.transform.position = new Vector3(0.0f, -1.7f, 0.0f);
        Ocean.gameObject.transform.localScale = Vector3.one * 64.0f;

        Ocean.gameObject.GetComponent<MeshRenderer>().material = ocean_material;

        Ocean.audio_source.volume = 0.125f;
        Ocean.audio_source.Play();
    }
	
	// Update is called once per frame
	void Update () {
        Ocean.AdjustPitch();
        Ocean.SetSoundPositionRelativeToViewer();
    }
}
