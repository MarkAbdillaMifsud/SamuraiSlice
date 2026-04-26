using UnityEngine;

public class SwipeDetector : MonoBehaviour
{
    private Vector2 previousPosition;

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            previousPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        } else if (Input.GetMouseButton(0))
        {
            Vector2 currentPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Linecast(previousPosition, currentPosition);
            if(hit.collider != null)
            {
                Destroy(hit.collider.gameObject);
                Debug.Log("+10!");
            }
            previousPosition = currentPosition;
        }
    }
}
