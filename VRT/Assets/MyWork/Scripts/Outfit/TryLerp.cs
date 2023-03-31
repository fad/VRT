using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TryLerp : MonoBehaviour
{
    public Vector3 startPos;
    public Transform endPos;
    // Start is called before the first frame update
    private void OnEnable()
    {
        // MoveObject(startPos, endPos , 3f);

        StartCoroutine(MyCoroutine(endPos));

        //MyCoroutine(endPos);
    }

 
    IEnumerator MyCoroutine(Transform target)
    {
        float timeToStart = Time.time;
        while (Vector3.Distance(startPos, target.localPosition) > 0.05f)
        {
            transform.localPosition = Vector3.Lerp(startPos, target.localPosition, (Time.time - timeToStart) * 1); //Here speed is the 1 or any number which decides the how fast it reach to one to other end.

            yield return null;
        }

        print("Reached the target.");

        yield return new WaitForSeconds(3f); // THis is just for how Coroutine works with delay

        print("MyCoroutine is now finished.");
    }
}
