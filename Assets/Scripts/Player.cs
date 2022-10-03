using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    float vertical;
    float horizontal;
    public float speed = 10f;
    public Vector2 direction;
    Transform cam;
    public Items item;
    public Map map;

    float boostTimer = 0;
    public Slider slider;
    public GameObject speedIcon;

    Rigidbody2D rb;
    SpriteRenderer sr;
    public SpriteRenderer itemRenderer;

    private void Awake()
    {
        cam = Camera.main.transform;
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        vertical = Input.GetAxisRaw("Vertical");
        horizontal = Input.GetAxisRaw("Horizontal");
        direction = new Vector2(horizontal, vertical).normalized;
        cam.position = new Vector3(Mathf.Clamp(transform.position.x, map.minCamX, map.maxCamX), cam.position.y, cam.position.z);

        if (horizontal != 0) sr.flipX = horizontal < 0;

        boostTimer += Time.deltaTime;
        slider.value = 1 - (boostTimer / 10);

        if (boostTimer > 10)
        {
            boostTimer = 0;
        }
    }

    private void FixedUpdate()
    {
        rb.velocity = direction * speed;
    }

    public void SetItem(Items item)
    {
        this.item = item;

        if (item == Items.None)
        {
            itemRenderer.sprite = null;
            return;
        }
        itemRenderer.sprite = map.items[(int)item - 1];
    }

    private void OnDisable()
    {
        rb.velocity = Vector2.zero;
        if (slider.gameObject != null) slider.gameObject.SetActive(false);
        speedIcon.SetActive(false);
    }
}
