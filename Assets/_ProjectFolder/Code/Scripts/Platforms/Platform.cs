using UnityEngine;

public class Platform : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.contacts[0].normal.y < -0.5f)
            other.transform.SetParent(transform);
    }
    
    private void OnCollisionExit2D(Collision2D other) =>
        other.transform.SetParent(null);
}