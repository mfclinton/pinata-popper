using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[System.Serializable]
public class SpriteDistributorData : ScriptableObject
{
    public GameObject mainSprite;
    public Transform frillsParent;
    public Vector2Int cells;
    public List<GameObject> spawnables = new List<GameObject>();
    public Vector3 spawnableScale = Vector3.one;
    public Vector2 offset;
}

public class SpriteDistributor : EditorWindow
{
    private SpriteDistributorData data;
    private SerializedObject serializedData;

    #region Editor Code

    [MenuItem("Tools/Sprite Distributor")]
    public static void ShowWindow()
    {
        GetWindow<SpriteDistributor>("Sprite Distributor");
    }

    private void OnEnable()
    {
        data = CreateInstance<SpriteDistributorData>();
        serializedData = new SerializedObject(data);
    }

    private void OnGUI()
    {
        serializedData.Update();
        DrawProperties();
        DrawButtons();
        serializedData.ApplyModifiedProperties();
    }

    private void DrawProperties()
    {
        EditorGUILayout.PropertyField(serializedData.FindProperty("mainSprite"), new GUIContent("Main Sprite"));
        EditorGUILayout.PropertyField(serializedData.FindProperty("frillsParent"), new GUIContent("Frills Parent"));
        EditorGUILayout.PropertyField(serializedData.FindProperty("cells"), new GUIContent("Number of Cells in Each Dimension"));
        EditorGUILayout.PropertyField(serializedData.FindProperty("spawnables"), new GUIContent("Spawnables"), true);
        EditorGUILayout.PropertyField(serializedData.FindProperty("spawnableScale"), new GUIContent("Spawnable Scale"));
        EditorGUILayout.PropertyField(serializedData.FindProperty("offset"), new GUIContent("Offset"));
    }

    private void DrawButtons()
    {
        if (GUILayout.Button("Distribute"))
        {
            DistributeSprites();
        }
        
        if (GUILayout.Button("Destroy Children"))
        {
            ClearChildren();
        }
    }

    #endregion

    private void ClearChildren()
    {
        if (data.frillsParent == null)
        {
            Debug.LogError("Please assign the frills parent Transform.");
            return;
        }

        int numChildren = data.frillsParent.childCount;
        for (int i = numChildren - 1; i >= 0; i--)
        {
            DestroyImmediate(data.frillsParent.GetChild(i).gameObject);
        }
    }

    private void DistributeSprites()
    {
        if (!ValidateDistributeSpritesRequirements())
        {
            return;
        }

        SpriteRenderer mainSpriteRenderer = data.mainSprite.GetComponent<SpriteRenderer>();
        Texture2D texture = mainSpriteRenderer.sprite.texture;

        // Cell Sample Variables
        Bounds spriteBounds = mainSpriteRenderer.bounds;
        Vector2 cellSize = new Vector2(spriteBounds.size.x / data.cells.x, spriteBounds.size.y / data.cells.y);
        Vector2 halfCellSize = cellSize * 0.5f;
        Vector2 randomizationRange = halfCellSize;
        Vector2 startingPos = spriteBounds.min;
        
        int numCreated = 0;
        for (int y = 0; y < data.cells.y; y++)
        {
            for (int x = 0; x < data.cells.x; x++)
            {
                Vector2 cellCenter = new Vector2(x * cellSize.x, y * cellSize.y) + halfCellSize;
                Vector2 worldPos = startingPos + cellCenter + Random.insideUnitCircle * randomizationRange.magnitude;
                Vector2 texturePos = TextureUtils.WorldCoordToTexture(worldPos, mainSpriteRenderer);
                if (IsValidTextureCoordinate(texturePos, texture))
                {
                    InstantiatePrefab(worldPos, data.spawnableScale, data.frillsParent, texture.GetPixel((int)texturePos.x, (int)texturePos.y));
                    numCreated++;
                }
            }
        }

        Debug.Log($"Created {numCreated} objects.");
    }

    #region Validation

    private bool ValidateDistributeSpritesRequirements()
    {
        if (data.mainSprite == null || data.spawnables.Count == 0)
        {
            Debug.LogError("Please assign all the required fields and ensure spawnables is not empty.");
            return false;
        }

        if (data.mainSprite.GetComponent<SpriteRenderer>() == null)
        {
            Debug.LogError("The main sprite GameObject does not have a SpriteRenderer component.");
            return false;
        }

        return true;
    }

    private bool IsValidTextureCoordinate(Vector2 coord, Texture2D texture)
    {
        int x = (int)coord.x;
        int y = (int)coord.y;
    
        if (x < 0 || x >= texture.width || y < 0 || y >= texture.height)
            return false;

        Color pixelColor = texture.GetPixel(x, y);
        return pixelColor.a > 0;
    }

    #endregion

    #region Create Sprite

    private void InstantiatePrefab(Vector2 position, Vector3 scale, Transform parent, Color baseColor)
    {
        GameObject prefab = data.spawnables[Random.Range(0, data.spawnables.Count)];
        GameObject instance = PrefabUtility.InstantiatePrefab(prefab, parent) as GameObject;
        instance.transform.localScale = scale;

        if (instance != null)
        {
            instance.transform.position = position + data.offset;
            ApplyColorVariation(instance, baseColor);
        }
        else
        {
            Debug.LogError("Failed to instantiate prefab.");
        }
    }

    private void ApplyColorVariation(GameObject instance, Color baseColor)
    {
        SpriteRenderer spriteRenderer = instance.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            // Applying a slight color variation to the base color
            Color instanceColor = baseColor + new Color(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f), 0);
            spriteRenderer.color = instanceColor;
        }
    }

    #endregion
}
