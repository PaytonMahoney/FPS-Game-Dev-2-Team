using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.InputSystem;

public class ChillNPC : MonoBehaviour
{


    [Header("Dialogue")]
    [TextArea] public string[] firstTimeLines;
    [TextArea] public string[] repeatLines;

    [Header("UI References")]
    public TMP_Text dialogueText;
    public GameObject dialogueBox;
    public float typingSpeed = 0.03f;
    public Transform player;

    [Header("Voice Clips")]
    public AudioClip[] voiceClipsFirstTime;
    public AudioClip[] voiceClipsRepeat;

    private bool inRange = false;
    private bool isTalking = false;
    private int currentLine = 0;
    private Coroutine typingCoroutine;
    private AudioSource dogAudio;
    private bool hasTalked = false;
    public GameObject interactButton;

    void Start()
    {
        dogAudio = GetComponent<AudioSource>();
        dialogueText.text = "";
        dialogueBox.SetActive(false);
    }

    void Update()
    {

        if (inRange && Input.GetKeyDown(KeyCode.E))
        {
            if (!isTalking)
                StartDialogue();
            else
                ShowNextLine();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {


            inRange = true;
            if (interactButton != null)
                interactButton.SetActive(true);
        }

    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            inRange = false;
            EndDialogue();
            if (interactButton != null)
                interactButton.SetActive(false);
        }
    }

    void StartDialogue()
    {
        isTalking = true;
        currentLine = 0;
        if (interactButton != null)
            interactButton.SetActive(false);
        dialogueBox.SetActive(true);
        LookAtPlayer();
        ShowNextLine();
    }

    void ShowNextLine()
    {
        string[] lines = hasTalked ? repeatLines : firstTimeLines;
        AudioClip[] voices = hasTalked ? voiceClipsRepeat : voiceClipsFirstTime;

        if (currentLine < lines.Length)
        {
            if (typingCoroutine != null)
                StopCoroutine(typingCoroutine);

            if (currentLine < voices.Length && voices[currentLine] != null)
                dogAudio.PlayOneShot(voices[currentLine]);

            typingCoroutine = StartCoroutine(TypeLine(lines[currentLine]));
            currentLine++;
        }
        else
        {
            hasTalked = true;
            EndDialogue();
        }
    }

    IEnumerator TypeLine(string line)
    {
        dialogueText.text = "";
        foreach (char c in line)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    void EndDialogue()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        dialogueText.text = "";
        dialogueBox.SetActive(false);
        isTalking = false;
    }

    void LookAtPlayer()
    {
        if (player == null) return;

        Vector3 direction = player.position - transform.position;
        direction.y = 0;
        if (direction != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(direction);
    }

    
}



