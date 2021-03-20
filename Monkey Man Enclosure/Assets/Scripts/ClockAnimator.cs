using System.Collections;
using UnityEngine;

public class ClockAnimator : MonoBehaviour
{
    [SerializeField] Transform handHolder;
    [Range(0,1)]
    public float timePercentage = 0;
    const float ROTATION_OFFSET = 93.091f;

    private void Start() => StartCoroutine(RefreshingClockHandRotation());

    IEnumerator RefreshingClockHandRotation()
    {
        while (true)
        {
            handHolder.localRotation =
                Quaternion.AngleAxis(-timePercentage * 360 + ROTATION_OFFSET, Vector3.forward);

            yield return new WaitForSeconds(GlobalVariables.UIRefreshInterval);
        }
    }
}
