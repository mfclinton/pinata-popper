using UnityEngine;

public static class TextureUtils
{
    public static Vector2 WorldCoordToTexture(Vector2 worldPos, SpriteRenderer spriteRenderer)
    {
        var sprite = spriteRenderer.sprite;
        Rect rect = sprite.textureRect;
        
        Vector2 localPos = spriteRenderer.transform.InverseTransformPoint(worldPos);

        // Get UV coords
        var uvX = (localPos.x - sprite.bounds.min.x) / sprite.bounds.size.x;
        var uvY = (localPos.y - sprite.bounds.min.y) / sprite.bounds.size.y;
        Vector2 normalizedLocalPos = new Vector2(uvX, uvY);
        
        // Get Texture Coord
        int x = Mathf.FloorToInt(rect.width * normalizedLocalPos.x);
        int y = Mathf.FloorToInt(rect.height * normalizedLocalPos.y);
        
        return new Vector2(x, y);
    }
    
    // TODO: validate this
    public static Vector2 TextureCoordToWorldCoord(Vector2 textureCoord, SpriteRenderer spriteRenderer)
    {
        var sprite = spriteRenderer.sprite;
    
        // Calculate the pivot offset
        Vector2 uv = new Vector2(textureCoord.x / sprite.texture.width, textureCoord.y / sprite.texture.height);
        
        // Convert from Normalized Local Position to Local Position
        float localX = sprite.bounds.min.x + uv.x * sprite.bounds.size.x;
        float localY = sprite.bounds.min.y + uv.y * sprite.bounds.size.y;
        Vector2 localPos = new Vector2(localX, localY);
    
        // Convert from Local Position to World Position
        Vector2 worldPos = spriteRenderer.transform.TransformPoint(localPos);
        return worldPos;
    }
}
