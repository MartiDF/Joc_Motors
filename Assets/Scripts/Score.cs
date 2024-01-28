using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Score : MonoBehaviour
{
    private float punts;
    private TextMeshProUGUI textmesh;

    private void Start(){
        textmesh = GetComponent<TextMeshProUGUI>();
    }

    private void Update(){
        textmesh.text=((int)(punts * 10f)).ToString("0");
    }

    public void Sumarpunts(float puntssumar){
        punts+=puntssumar;
    }
}
