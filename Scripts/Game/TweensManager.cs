using UnityEngine;

public class TweensManager : MonoBehaviour
{
    public AnimationCurve easeCurve;
    public AnimationCurve bounceCurve;
    
    public static TweensManager Instance { get; private set; }
    
    private void Awake()
    {
        Instance = this;
    }
}
