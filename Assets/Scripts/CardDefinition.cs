using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardDefinition", menuName = "Cards/CardDefinition")]
public class CardDefinition : ScriptableObject
{
    public string cardName;
    public Sprite image;
    public string summary;
    public int[] cost;
    [TextArea(15, 20)]
    public string details;
    public List<EffectDefinition> effects = new();
}
