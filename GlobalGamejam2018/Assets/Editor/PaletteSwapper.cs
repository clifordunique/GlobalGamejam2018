﻿using UnityEngine;
using System.Collections;

public class PaletteSwapper : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public ColorPalette[] palettes;

    private Texture2D texture;
    private MaterialPropertyBlock block;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        if (palettes.Length > 0)
            SwapColors(palettes[Random.Range(0, palettes.Length)]);
    }

    public void SwapColors(ColorPalette palette)
    {
        if (palette.cachedTexture == null)
        {
            texture = spriteRenderer.sprite.texture;

            int w = texture.width;
            int h = texture.height;

            Texture2D cloneTexture = new Texture2D(w, h);
            cloneTexture.wrapMode = TextureWrapMode.Clamp;
            cloneTexture.filterMode = FilterMode.Point;

            Color[] colors = texture.GetPixels();

            for (int i = 0; i < colors.Length; i++)
            {
                colors[i] = palette.GetColor(colors[i]);
            }

            cloneTexture.SetPixels(colors);
            cloneTexture.Apply();

            palette.cachedTexture = cloneTexture;
        }

        block = new MaterialPropertyBlock();
        block.SetTexture("_MainTex", palette.cachedTexture);
    }

    private void LateUpdate()
    {
        spriteRenderer.SetPropertyBlock(block);
    }
}