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
                if(hit.collider.tag == "Ingredient")
                {
                    Debug.Log("+10!");
                } else
                {
                    Debug.Log("GameOver!");
                }
                Destroy(hit.collider.gameObject);
            }
            previousPosition = currentPosition;
        }
    }
}
