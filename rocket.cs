using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class rocket : MonoBehaviour{
   [SerializeField] float rcsThrust = 100f;
   [SerializeField] float mainThrust = 25f;
   [SerializeField] float levelLoadDelay = 2f;

   [SerializeField] AudioClip mainEngine;
   [SerializeField] AudioClip success;
   [SerializeField] AudioClip death;

   //[SerializeField] ParticleSystem mainEngineParticle;
   [SerializeField] ParticleSystem deathParticle;
   [SerializeField] ParticleSystem successParticle;

    Rigidbody rigidBody;
    AudioSource audioSource;

    enum State {alive, dead, transcending};
    State state = State.alive;
    bool collisionsDisabled = false;

    // Start is called before the first frame update
    void Start(){//generics
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update(){
        if (state == State.alive){
            RespondToThrustInput();
            RespondToRotateInput();
        }
        if(Debug.isDebugBuild){
            DebugKeys();
        }
    }
    //Debug
    private void DebugKeys(){
        if(Input.GetKeyDown(KeyCode.L)){
            LoadNextLevel();
        }
        else if(Input.GetKeyDown(KeyCode.C)){
            //toggle collision
            collisionsDisabled = !collisionsDisabled;
        }
    }
     void OnCollisionEnter(Collision collision){
         if(state != State.alive || collisionsDisabled){ return; }
         
         switch(collision.gameObject.tag){
            case "Friendly":
                print("Friendly");
                break;
            case "Finish":
                StartSucessSequence();
                break;
            default:
                StartDeathSequence();
                break;
        }
    }
    void StartSucessSequence(){
        print("Finished");
        state = State.transcending;
        audioSource.PlayOneShot(success);
        successParticle.Play();
        Invoke("LoadNextLevel", levelLoadDelay);
    }
    void StartDeathSequence(){
        print("Dead");
        state = State.dead;
        audioSource.PlayOneShot(death);
        Invoke("LoadCurrentLevel", levelLoadDelay);
    }
    private void LoadNextLevel(){
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;
        if (nextSceneIndex == SceneManager.sceneCountInBuildSettings){
            nextSceneIndex = 0; // loop back to start
        }
        SceneManager.LoadScene(nextSceneIndex);
    }
    private void LoadCurrentLevel(){
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }
    /*private void LoadFirstLevel(){
        SceneManager.LoadScene(0);
    }
    */
    void RespondToThrustInput(){
        if(Input.GetKey(KeyCode.Space)){
            // can thrust while rotating
            ApplyThrust();
        }
        else{
            StopApplyThrust();
        }
    }
    void StopApplyThrust(){
        audioSource.Stop();
        deathParticle.Stop();
    }
    void ApplyThrust(){
        //Time.deltaTime not working
        //float thrustPerFrame = mainThrust * Time.deltaTime;
        rigidBody.AddRelativeForce(Vector3.up * mainThrust);
            if (!audioSource.isPlaying){
                audioSource.PlayOneShot(mainEngine);
            }
       // mainEngineParticle.Play();
            //rigidBody.force(x, y, z);
    }

    private void RespondToRotateInput(){
        rigidBody.freezeRotation = true; //take manual control of rotation
        
        float rotationThisFrame = rcsThrust * Time.deltaTime;

        if(Input.GetKey(KeyCode.A)){
            //print("Rotating left");
            RotateManually(rcsThrust * Time.deltaTime);
        }
        else if(Input.GetKey(KeyCode.D)){
            RotateManually(-rcsThrust * Time.deltaTime);
            //transform.Rotate(-Vector3.forward * rotationThisFrame);
            //print("Rotating right");
        }
        //rigidBody.freezeRotation = false; //resume physics control of rotation
    }
    private void RotateManually(float rotationThisFrame){
        rigidBody.freezeRotation = true; // take manual control of rotation
        transform.Rotate(Vector3.forward * rotationThisFrame);
        rigidBody.freezeRotation = false; // resume physics control of rotation
    }
    
}
