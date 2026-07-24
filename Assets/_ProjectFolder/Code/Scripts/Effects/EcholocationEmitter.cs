using UnityEngine;

public class EcholocationEmitter : MonoBehaviour
{
    [ContextMenu("Emit Effect")]
    public void Emit()
    {
        EcholocationManager.Instance.EmitRipple(transform.position);
    }
}