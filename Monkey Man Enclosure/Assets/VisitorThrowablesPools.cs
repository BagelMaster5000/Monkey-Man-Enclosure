using UnityEngine;

public class VisitorThrowablesPools : MonoBehaviour
{
    public static VisitorThrowablesPools instance;
    public GameObject[] foodPelletsPool;
    public GameObject[] bricksPool;
    public GameObject[] bananasPool;

    private void Awake()
    {
        instance = this;
    }
}
