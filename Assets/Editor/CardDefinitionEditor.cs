using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CardDefinition))]
public class CardDefinitionEditor : Editor
{
    public override Texture2D RenderStaticPreview(
        string assetPath,
        Object[] subAssets,
        int width,
        int height)
    {
        CardDefinition card = (CardDefinition)target;

        if (card.image == null)
            return null;

        var preview = new Texture2D(width, height);
        EditorUtility.CopySerialized(card.image.texture, preview);
        return preview;
    }
}