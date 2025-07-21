using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.InputSystem;

public class ChillNPC : MonoBehaviour
{


    [TextArea]
    public string[] dialogueLines;

    public TMP_Text dialogueText;
    public GameObject dialogueBox;
    public float typingSpeed = 0.03f;

    private bool inRange = false;
    private bool isTalking = false;
    private int currentLine = 0;
    private Coroutine typingCoroutine;
    public Transform player; // Drag in the player object

    private AudioSource dogAudio;
    public AudioClip[] voiceClips; // One clip per dialogue line

    void Start()
    {
        dialogueText.text = "";
        dialogueBox.SetActive(false);
        dogAudio = GetComponent<AudioSource>();
       
       

    }

    void Update()
    {
        
        if (inRange && Input.GetKeyDown(KeyCode.E))
        {
            if (!isTalking)
            {
                StartDialogue();
            }
            else
            {
                ShowNextLine();
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            inRange = true;
            Debug.Log("Player entered NPC trigger.");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            EndDialogue();
            inRange = false;
            Debug.Log("Player left NPC trigger.");
        }
    }

    void StartDialogue()
    {
        isTalking = true;
        LookAtPlayer();
        currentLine = 0;
        dialogueBox.SetActive(true);
        ShowNextLine();

        isTalking = true;
        currentLine = 0;
        dialogueBox.SetActive(true);

        if (voiceClips != null && dogAudio != null)
            dogAudio.PlayOneShot(voiceClips[currentLine]);

        ShowNextLine();
    }

    void ShowNextLine()
    {
        if (currentLine < dialogueLines.Length)
        {
            if (typingCoroutine != null)
                StopCoroutine(typingCoroutine);

            // Play matching sound for this line
            if (currentLine < voiceClips.Length && voiceClips[currentLine] != null && dogAudio != null)
            {
                dogAudio.PlayOneShot(voiceClips[currentLine]);
            }

            typingCoroutine = StartCoroutine(TypeLine(dialogueLines[currentLine]));
            currentLine++;
        }
        else
        {
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
        Vector3 direction = player.position - transform.position;
        direction.y = 0; // Keep the dog from tilting up/down
        if (direction != Vector3.zero)
        {
            Quaternion rotation = Quaternion.LookRotation(direction);
            transform.rotation = rotation;
        }
    }
}


