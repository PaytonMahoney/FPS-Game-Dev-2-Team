using UnityEngine;

public class BulletMovement : MonoBehaviour
{

    [SerializeField] float speed;

    Vector3 direction;


    public void SetDirection(Vector3 dir)
    {
        direction = dir.normalized;
    }
   

    
    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }
}
