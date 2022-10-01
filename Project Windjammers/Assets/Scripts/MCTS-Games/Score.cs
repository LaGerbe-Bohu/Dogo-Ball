using UnityEngine;

[System.Serializable]
public class Score 
{
    public int points;
    public int setCount;

    public Score(int points, int setCount)
    {
        this.points = points;
        this.setCount = setCount;
    }

    public Score(Score score)
    {
        this.points = score.points;
        this.setCount = score.setCount;
    }
}
