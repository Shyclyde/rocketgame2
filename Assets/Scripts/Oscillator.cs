using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Oscillator : MonoBehaviour {

    [SerializeField] Vector3 movementVector;
    [SerializeField] float period = 2f;
    [Range(0, 1)] [SerializeField] float movementFactor;

    Vector3 startingPosition;

	void Start () {
        startingPosition = transform.position;
	}
	
	void Update () {
        if (period <= Mathf.Epsilon) return; // can't have a period of 0 dummy
        float cycles = Time.time / period;
        const float tau = Mathf.PI * 2; //about 6.28
        float rawSinWave = Mathf.Sin(cycles * tau);
        //print(rawSinWave);

        Vector3 offset = movementVector * (movementFactor * rawSinWave);
        transform.position = startingPosition + offset;
    }
}
