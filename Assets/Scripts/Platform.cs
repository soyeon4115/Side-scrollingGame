using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    [HideInInspector]
    public Camera targetCamera;

    private BoxCollider2D platformCollider;
    private List<GameObject> bonusItems = new List<GameObject>();

    void Start()
    {
        platformCollider = GetComponent<BoxCollider2D>();
        if (platformCollider != null)
            platformCollider.isTrigger = true;
    }

    void Update()
    {
        // Vector3 movement = Vector3.left * GameManager.Instance.scrollSpeed * Time.deltaTime;
        // transform.position += movement;

        transform.position += Vector3.left * GameManager.Instance.scrollSpeed * Time.deltaTime;

        // for (int i = bonusItems.Count - 1; i >= 0; i--)
        // {
        //     if (bonusItems[i] != null)
        //         bonusItems[i].transform.position += movement;
        //     else
        //         bonusItems.RemoveAt(i);
        // }

        if (transform.position.x < GetCameraLeftX())
            Destroy(gameObject);
    }

    public void AddBonusItem(GameObject item)
    {
        // 아이템을 플랫폼의 자식으로 설정 -> 플랫폼이 이동할 때 아이템은 자동으로 따라옴.
        item.transform.SetParent(this.transform);
        bonusItems.Add(item);
    }

    // 카메라의 왼쪽 경계의 월드 좌표를 반환하는 함수
    float GetCameraLeftX()
    {
        Camera cam = targetCamera != null ? targetCamera : Camera.main;
        if (cam == null)
            return float.NegativeInfinity;

        float distance = Mathf.Abs(cam.transform.position.z - transform.position.z);
        return cam.ViewportToWorldPoint(new Vector3(0f, 0.5f, distance)).x;
    }

    void OnDestroy()
    {
        foreach (GameObject item in bonusItems)
        {
            if (item != null)
                Destroy(item);
        }
        bonusItems.Clear();

        if (GameManager.Instance != null)
            GameManager.Instance.OnPlatformDestroyed();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        Destroy(gameObject);
        ClearBonusItems();
    }

    public void ClearBonusItems()
    {
        // 체리에 닿았을 때 호출될 함수
        // 플랫폼 아래의 모든 아이템을 제거합니다.
        foreach (GameObject item in bonusItems)
        {
            if (item != null) Destroy(item);
        }
        bonusItems.Clear();
    }
}
