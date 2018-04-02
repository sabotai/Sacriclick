using UnityEngine;
using System.Collections;

public class LerpColor : MonoBehaviour {
    public Color colorStart = Color.red;
    public Color colorEnd = Color.green;
    public float duration = 1.0F;
    public Renderer rend;
    void Start() {
        rend = GetComponent<Renderer>();
        if (ColorblindMode.cbMode){
            colorStart = ColorblindMode.cbRed;
            colorEnd = ColorblindMode.cbGreen;
        }
    }
    void Update() {
        float lerp = Mathf.PingPong(Time.time, duration) / duration;
        rend.material.color = Color.Lerp(colorStart, colorEnd, lerp);
    }
}