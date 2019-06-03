using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float throwPower;
    public GameObject dotPrefab;
    public float distanceBetweenDots;
    public int maxDotsCount;

    private SpriteRenderer[] dots;

    void Awake() {
        GameManager.Instance.Player = this;

        dots = new SpriteRenderer[maxDotsCount];
        CreateAim();

    }

    // Update is called once per frame
    void Update() {

        if (Input.GetMouseButton(0)) {
            if (GameManager.Instance.freeBall != null && GameManager.Instance.gameActive) {
                Vector2 targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                int dotsCount = (int)(Vector2.Distance(transform.position, targetPos) / distanceBetweenDots);
                for (int i = 0; i <= dotsCount; i++) {
                    dots[i].gameObject.SetActive(true);
                    //dots[i].color =  UiManager.Instance.darkBackgroundEnabled ? Color.white : Color.black;
                    dots[i].color = GameManager.Instance.freeBall.GetComponent<SpriteRenderer>().color;

                }
                for (int i = dotsCount + 1; i < maxDotsCount; i++) {
                    dots[i].gameObject.SetActive(false);
                }
            }
        }

        if (Input.GetMouseButtonUp(0)) {
            Vector2 targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (GameManager.Instance.freeBall != null && GameManager.Instance.gameActive)
                GameManager.Instance.freeBall.GetComponent<Ball>().Throw(targetPos, throwPower);
            for (int i = 0; i < maxDotsCount; i++) {
                dots[i].gameObject.SetActive(false);
            }
        }

    }


    private void CreateAim() {

        float offset = distanceBetweenDots;

        for (int i = 0; i < maxDotsCount; i++) {
            GameObject dot = Instantiate(
                        dotPrefab,
                        new Vector3(transform.position.x, transform.position.y + offset, transform.position.z),
                        Quaternion.identity,
                        transform
                        );

            dots[i] = dot.GetComponent<SpriteRenderer>();

            dot.SetActive(false);

            offset += distanceBetweenDots;
        }
    }

}
