using UnityEngine;

public class Platform : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D other) =>
        other.transform.SetParent(transform);
    
    private void OnCollisionExit2D(Collision2D other) =>
        other.transform.SetParent(null);
}