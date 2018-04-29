using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour {

    Rigidbody rigidBody;
    AudioSource rocketSound;
    Text godModeText;

    bool isTranscending = false;
    bool godmode = false;

    Vector3 thrust = new Vector3(0, 10, 0);

    [SerializeField] float rcsThrust = 100f;
    [SerializeField] float mainThrust = 100f;

    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip deathSound;
    [SerializeField] AudioClip winSound;

    [SerializeField] ParticleSystem mainEngineParticles;
    [SerializeField] ParticleSystem deathParticles;
    [SerializeField] ParticleSystem winParticles;

    [SerializeField] float levelLoadDelay = 2f;

    void Start () {
        rigidBody = GetComponent<Rigidbody>();
        rocketSound = GetComponent<AudioSource>();
        godModeText = GameObject.Find("Godmode Value").GetComponent<Text>();
	}
	
	void Update () {
        if (!isTranscending) {
            RespondToThrustInput();
            RespondToRotateInput();
        }
        if(Debug.isDebugBuild)
            RespondToDebugKeys();
    }

    private void RespondToDebugKeys() {
        //toggle godmode
        if (Input.GetKeyDown(KeyCode.C)) {
            godmode = !godmode;
            if (godmode == false)
                godModeText.text = "off";
            else if (godmode == true)
                godModeText.text = "on";
        }
        //load next level
        if (Input.GetKeyDown(KeyCode.L)) {
            LoadNextScene();
        }
    }

    private void OnCollisionEnter(Collision collision) {
        if (isTranscending || godmode) return; //ignore when dead

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
        isTranscending = true;
        rocketSound.Stop();
        rocketSound.PlayOneShot(winSound);
        winParticles.Play();
        Invoke("LoadNextScene", levelLoadDelay);
    }

    private void Die() {
        isTranscending = true;
        rocketSound.Stop();
        rocketSound.PlayOneShot(deathSound);
        mainEngineParticles.Stop();
        deathParticles.Play();
        Invoke("Respawn", levelLoadDelay);
    }

    private void LoadNextScene() {
        isTranscending = false;
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;
        if (nextSceneIndex == SceneManager.sceneCountInBuildSettings)
            nextSceneIndex = 0;
        SceneManager.LoadScene(nextSceneIndex);
    }

    private void Respawn() {
        SceneManager.LoadScene(0);
    }

    private void RespondToThrustInput() {
        if (Input.GetKey(KeyCode.Space)) {
            ApplyThrust();
        }
        else if (Input.GetKeyUp(KeyCode.Space)) {
            StopApplyingThrust();
        }
    }

    private void StopApplyingThrust() {
        rocketSound.Stop();
        mainEngineParticles.Stop();
    }

    private void ApplyThrust() {
        rigidBody.AddRelativeForce(Vector3.up * mainThrust);
        if (!rocketSound.isPlaying)
            rocketSound.PlayOneShot(mainEngine);
        mainEngineParticles.Play();
    }

    private void RespondToRotateInput() {
        rigidBody.angularVelocity = Vector3.zero; // stop current motion
        float rotationThisFrame = rcsThrust * Time.deltaTime;

        if (Input.GetKey(KeyCode.A)) 
            transform.Rotate(Vector3.forward * rotationThisFrame);

        else if (Input.GetKey(KeyCode.D))
            transform.Rotate(-Vector3.forward * rotationThisFrame);
    }
}
