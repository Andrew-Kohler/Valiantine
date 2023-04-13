using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestItemDisplay : MonoBehaviour
{
    private Color spriteOpacity;    // The only reason I mess with the color is that Color.Clear is transparent and Color.White is opaque
    private SpriteRenderer spriteRenderer;

    private void OnEnable()
    {
        InGameUIView.onInteractionEnd += hideItem;
    }

    private void OnDisable()
    {
        InGameUIView.onInteractionEnd -= hideItem;
    }
    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteOpacity = spriteRenderer.material.color;

        spriteOpacity = new Color(spriteOpacity.r, spriteOpacity.g, spriteOpacity.b, 0);
        spriteRenderer.material.color = spriteOpacity;

    }

    public void showItem()
    {
        this.transform.position = new Vector3(PlayerManager.Instance.PlayerTransform().position.x, this.transform.position.y, PlayerManager.Instance.PlayerTransform().position.z);
        spriteOpacity = new Color(spriteOpacity.r, spriteOpacity.g, spriteOpacity.b, 1);
        spriteRenderer.material.color = spriteOpacity;
    }

    public void setItemSprite(Sprite sprite)
    {
        spriteRenderer.sprite = sprite;
    }

    private void hideItem()
    {
        this.gameObject.SetActive(false);
    }
}
