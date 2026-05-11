using Unity.Mathematics;
using UnityEngine;

public class BackgroundController : MonoBehaviour
{
    [SerializeField, Range(1f, 50f)]
    float scrollSpeed = 10f;

    [SerializeField]
    Transform background1;

    [SerializeField]
    Transform background2;

    [SerializeField]
    Camera targetCamera;

    private SpriteRenderer background1Renderer;
    private SpriteRenderer background2Renderer;

    void Start()
    {
        if(targetCamera == null) 
            targetCamera = Camera.main;
        
        if (targetCamera == null || background2 == null) 
            return;

        background1Renderer = background1.GetComponent<SpriteRenderer>();
        background2Renderer = background2.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if(background1 == null || background2 == null || background1Renderer == null || background2Renderer == null )
            return;
        MoveBackground(background1);
        MoveBackground(background2);

        RepositionIfOutside(background1, background1Renderer, background2Renderer);
        RepositionIfOutside(background2, background2Renderer, background1Renderer);

    }

    void MoveBackground(Transform bg)
    {
        bg.position += Vector3.left * GameManager.Instance.scrollSpeed * Time.deltaTime;
        // Vector.left = (-1, 0, 0) * 10f * Time.deltaTime(1프레임 당 걸리는 시간)
        // Vector.right = (1, 0, 0)
    }
    void RepositionIfOutside(Transform movingBackground, SpriteRenderer movingRenderer, SpriteRenderer otherRenderer)
    {
        if (movingRenderer.bounds.max.x > GetCameraLeftX())
            return;
        float newX = otherRenderer.bounds.max.x + movingRenderer.bounds.extents.x;
        movingBackground.position = new Vector3(newX, movingBackground.position.y, movingBackground.position.z);
    }
    float GetCameraLeftX()
    {
        if (targetCamera == null)
            return float.NegativeInfinity; // 무한대 값
        
        float distance = Mathf.Abs(targetCamera.transform.position.z - transform.position.z);
        return targetCamera.ViewportToWorldPoint(new Vector3(0f, 0.5f, distance)).x;
    }
}
