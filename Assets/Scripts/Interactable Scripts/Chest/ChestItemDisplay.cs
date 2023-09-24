using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestItemDisplay : MonoBehaviour
{
    private Color spriteOpacity;    // The only reason I mess with the color is that Color.Clear is transparent and Color.White is opaque
    
    [SerializeField] private GameObject itemElement;
    [SerializeField] private GameObject backingElement;

    private SpriteRenderer itemRenderer;
    private SpriteRenderer backingRenderer;
    private Animator itemAnim;

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
        itemRenderer = itemElement.GetComponent<SpriteRenderer>();
        backingRenderer = backingElement.GetComponent<SpriteRenderer>();
        spriteOpacity = itemRenderer.material.color;
        itemAnim = itemElement.GetComponent<Animator>();

        spriteOpacity = new Color(spriteOpacity.r, spriteOpacity.g, spriteOpacity.b, 0);
        itemRenderer.material.color = spriteOpacity;
        backingRenderer.material.color = spriteOpacity;
        this.enabled = false;
    }

    public void showItem()
    {
        this.transform.position = new Vector3(PlayerManager.Instance.PlayerTransform().position.x, this.transform.position.y, PlayerManager.Instance.PlayerTransform().position.z);
        spriteOpacity = new Color(spriteOpacity.r, spriteOpacity.g, spriteOpacity.b, 1);
        itemRenderer.material.color = spriteOpacity;
        backingRenderer.material.color = spriteOpacity;
    }

    public void setItemSprite(Sprite sprite)
    {
        itemRenderer.sprite = sprite;
    }

    private void hideItem()
    {
        if(this.isActiveAndEnabled)
            StartCoroutine(DoHideItem());
    }

    private IEnumerator DoHideItem()
    {
        itemAnim.Play("Pocket");
        yield return new WaitForSeconds(.5f);
        itemElement.gameObject.SetActive(false);
        backingElement.gameObject.SetActive(false);
    }
}
