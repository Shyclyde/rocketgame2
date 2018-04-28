using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour {

    Rigidbody rigidBody;
    AudioSource rocketSound;

    enum State { Alive, Dying, Transcending }
    State state = State.Alive;

    [SerializeField] float rcsThrust = 100f;
    [SerializeField] float mainThrust = 100f;

    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip deathSound;
    [SerializeField] AudioClip winSound;

    [SerializeField] ParticleSystem mainEngineParticles;
    [SerializeField] ParticleSystem deathParticles;
    [SerializeField] ParticleSystem winParticles;

    [SerializeField] float levelLoadDelay = 2f;

    Vector3 thrust = new Vector3(0, 10, 0);

    void Start () {
        rigidBody = GetComponent<Rigidbody>();
        rocketSound = GetComponent<AudioSource>();
	}
	
	void Update () {
        if(state == State.Alive) {
            RespondToThrustInput();
            RespondToRotateInput();
        }
	}

    private void OnCollisionEnter(Collision collision) {
        if (state != State.Alive) return; //ignore when dead

        switch (collision.gameObject.tag) {
            case "Friendly":
                // do nothing
                break;
            case "Fuel":
                // should we add fuel?
                break;
            case "Finish":
                Win();
                break;
            default:               
                Die();
                break;
        }
    }

    private void Win() {
        print("Won");
        state = State.Transcending;
        rocketSound.Stop();
        rocketSound.PlayOneShot(winSound);
        winParticles.Play();
        Invoke("LoadNextScene", levelLoadDelay);
    }

    private void Die() {
        print("Dead");
        state = State.Dying;
        rocketSound.Stop();
        rocketSound.PlayOneShot(deathSound);
        mainEngineParticles.Stop();
        deathParticles.Play();
        Invoke("Respawn", levelLoadDelay);
    }

    private void LoadNextScene() {
        state = State.Alive;
        SceneManager.LoadScene(1);
    }

    private void Respawn() {
        state = State.Alive;
        mainEngineParticles.Stop();
        winParticles.Stop();
        deathParticles.Stop();
        SceneManager.LoadScene(0);
    }

    private void RespondToThrustInput() {
        if (Input.GetKey(KeyCode.Space)) {
            ApplyThrust();
        }
        else if (Input.GetKeyUp(KeyCode.Space)) {
            rocketSound.Stop();
            mainEngineParticles.Stop();
        }
    }

    private void ApplyThrust() {
        rigidBody.AddRelativeForce(Vector3.up * mainThrust);
        if (!rocketSound.isPlaying)
            rocketSound.PlayOneShot(mainEngine);
        mainEngineParticles.Play();
    }

    private void RespondToRotateInput() {
        rigidBody.freezeRotation = true; // stop the physical rotation so we can just turn it

        float rotationThisFrame = rcsThrust * Time.deltaTime;
        if (Input.GetKey(KeyCode.A)) {
            
            transform.Rotate(Vector3.forward * rotationThisFrame);
        }
        else if (Input.GetKey(KeyCode.D)) {
            transform.Rotate(-Vector3.forward * rotationThisFrame);
        }
        rigidBody.freezeRotation = false; // resume the normal physics rotation
    }
}
