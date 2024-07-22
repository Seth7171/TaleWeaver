
using UnityEngine;
using Utilities;
using UnityEngine.UI;

using Random = UnityEngine.Random;

[RequireComponent(typeof(DiceSides))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(AudioSource))]
public class DiceRoller : MonoBehaviour {

    [SerializeField] float rollForce = 50f;
    [SerializeField] float torqueAmount = 5f;
    [SerializeField] float maxRollTime = 3f;
    [SerializeField] float minAngularVelocity = 0.1f;
    [SerializeField] float smoothTime = 0.1f;
    [SerializeField] float maxSpeed = 15f;
    

    [SerializeField] TMPro.TextMeshProUGUI resultText;


    [SerializeField] AudioClip shakeClip;
    [SerializeField] AudioClip rollClip;
    [SerializeField] AudioClip impactClip;
    [SerializeField] AudioClip finalResultClip;
    //[SerializeField] GameObject impactEffect;
    //[SerializeField] GameObject finalResultEffect;

    DiceSides diceSides;
    AudioSource audioSource;
    Rigidbody rb;

    CountdownTimer rollTimer;
    
    Vector3 originPosition;
    Vector3 currentVelocity;
    bool finalize;

    public Button rollButton;

    Vector3 targetPosition;
    bool isTransitioning = false;
    Quaternion targetRotation;
    //bool isRotating = false;

    void Awake() {
        diceSides = GetComponent<DiceSides>();
        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody>();

        resultText.text = "";
        originPosition = transform.position;
        
        rollTimer = new CountdownTimer(maxRollTime);
        rollTimer.OnTimerStart += PerformInitialRoll;
        rollTimer.OnTimerStop += () => finalize = true;

        rb.useGravity = false;
    }

    void FixedUpdate()
    {
        // Make the cube fall towards the screen at each physics update
        float speedTowardsScreen = 9.81f; // Set this speed as needed
        rb.velocity = new Vector3(0, 0, -speedTowardsScreen); // Adjust direction and speed here
    }

    /*    void OnMouseUp() {
            if (rollTimer.IsRunning) return;
            rollTimer.Start();
        }*/

    void Update()
    {
        if (rollTimer != null)
        {
            rollTimer.Tick(Time.deltaTime);
        }

        if (finalize)
        {
            MoveDiceToCenter();
            finalize = false;  // Reset finalize flag
        }

        if (isTransitioning)
        {
            SmoothTransition();
        }
    }


    public void RollDice()
    {
        Debug.Log("RollDice called.");
        if (!rollTimer.IsRunning)
        {
            Debug.Log("Timer is not running, starting roll.");
            rollTimer.Start();
            if (rollButton != null)
            {
                //rollButton.interactable = false;
                Debug.Log("Button should be disabled now.");
            }
        }
        else
        {
            Debug.Log("Timer is already running.");
        }
    }


    void OnCollisionEnter(Collision col) {
        if (rollTimer.IsRunning && rollTimer.Progress < 0.5f && rb.angularVelocity.magnitude < minAngularVelocity) {
            finalize = true;
        }
        
        //audioSource.PlayOneShot(impactClip);
        //var particles = InstantiateFX(impactEffect, col.contacts[0].point, 1f);
        //Destroy(particles, 1f);
    }

    void PerformInitialRoll() {
        ResetDiceState();
        resultText.text = "";
        
        Vector3 targetPosition = new Vector3(0, 0, 1);
        rb.AddForce(targetPosition * rollForce, ForceMode.Impulse);
        rb.AddTorque(Random.insideUnitSphere * torqueAmount, ForceMode.Impulse);
        
        audioSource.clip = shakeClip;
        audioSource.loop = false;
        audioSource.Play();
    }

    void MoveDiceToCenter()
    {
        targetPosition = originPosition;
        AlignDiceToResult();  // Set target rotation
        isTransitioning = true;
    }

    void AlignDiceToResult()
    {
        int result = diceSides.GetMatch();
        if (diceSides.faceRotations.TryGetValue(result, out Quaternion rotation))
        {
            targetRotation = rotation;
        }
    }

    void SmoothTransition()
    {
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref currentVelocity, smoothTime, maxSpeed);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, smoothTime * maxSpeed * 5);
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f && Quaternion.Angle(transform.rotation, targetRotation) < 0.1f)
        {
            isTransitioning = false;
            FinalizeRoll();  // Ensure FinalizeRoll is called once the transition is complete
        }
    }
    
    void FinalizeRoll()
    {
        rollTimer.Stop();
        finalize = false;
        ResetDiceState();

        audioSource.loop = false;
        audioSource.Stop();
        audioSource.PlayOneShot(finalResultClip);

        //var particles = InstantiateFX(finalResultEffect, transform.position, 5f);
        //Destroy(particles, 3f);

        int result = diceSides.GetMatch();
        Debug.Log($"Dice landed on {result}");
        resultText.text = result.ToString();
    }

    void ResetDiceState() {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        transform.position = originPosition;
    }
    
    GameObject InstantiateFX(GameObject fx, Vector3 position, float size) {
        var particles = Instantiate(fx, position, Quaternion.identity);
        particles.transform.localScale = Vector3.one * size;
        return particles;
    }
}