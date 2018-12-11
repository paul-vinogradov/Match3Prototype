using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public ElementGrid Grid;
    public GameObject InputPanel;
    public GameObject ScorePanel;
    public Text X;
    public Text Y;

    void Start()
    {
        if (Grid == null)
        {
            Grid = FindObjectOfType<ElementGrid>();
        }
    }
    public void Go()
    {
        int x, y;
        if (X.text == "")
            x = 6;
        else
        {
            if (!int.TryParse(X.text, out x))
                x = 6;
            else
            {
                if (x < 5)
                    x = 5;
            }
        }
        if (Y.text == "")
            y = 6;
        else
        {
            if (!int.TryParse(Y.text, out y))
                y = 6;
            else
            {
                if (y < 5)
                    y = 5;
            }
        }
        InputPanel.SetActive(false);
        ScorePanel.SetActive(true);
        Grid.Init(x,y);
    }
}
