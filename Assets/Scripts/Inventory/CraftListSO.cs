using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New craft list", menuName = "Craft list")]
public class CraftListSO : ScriptableObject
{
    public List<CraftSO> craftList = new List<CraftSO>();
}