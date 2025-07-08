using UnityEngine;

public class Structures : MonoBehaviour
{
    [SerializeField] Transform start;
    [SerializeField] Transform end;
    [SerializeField] int speed;
    [SerializeField] int startDelay;
   
    [SerializeField] bool rotate;
    [SerializeField] int rotateSpeed;


    float delayTimer;
    bool forward;
    Vector3 endposition;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (end != null)
        {
            endposition = end.position;
            forward = true;
            delayTimer = 0;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (end != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, endposition, speed * Time.deltaTime);
            if (Vector3.Distance(transform.position, endposition) < 0.01f)
            {
                delayTimer += Time.deltaTime;
                if (delayTimer > startDelay)
                {
                    if (forward)
                    {
                        endposition = start.position;
                        forward = false;
                    }
                    else
                    {
                        endposition = end.position;
                        forward = true;
                    }
                    delayTimer = 0;
                }
            }
        }
        if (rotate)
        {
            transform.Rotate(0, rotateSpeed, 0);

        }
        
    }
}
