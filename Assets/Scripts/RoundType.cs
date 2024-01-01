using UnityEngine;

namespace LevelUpStudio.ChaosBlitzSprint
{
    [CreateAssetMenu(fileName = "[RoundType]", menuName = "RoundType")]
    public class RoundType : ScriptableObject {
        
        [SerializeField]private string roundName;
        [SerializeField]private int nOfLaps;
        [SerializeField]private int pointsRequired;

        public string GetRoundName(){return roundName;}
        public int GetLapAmount(){return nOfLaps;}
        public int GetPointRequirement(){return pointsRequired;}
    }
}