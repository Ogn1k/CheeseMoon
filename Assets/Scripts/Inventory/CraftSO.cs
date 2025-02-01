using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

[CreateAssetMenu(fileName = "New craft", menuName = "Craft")]
public class CraftSO : ScriptableObject
{
    public string craftName;
    public Sprite craftIcon;

    public List<PartOfCraft> craft;

    public ItemSO result;
    public int amount;
}


[System.Serializable]
public struct PartOfCraft
{
    [SerializeField] public int amount;
    [SerializeField] public ItemSO item;
}