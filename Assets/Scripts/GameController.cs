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
            x = 5;
        else
        {
            if (!int.TryParse(X.text, out x))
                x = 5;
            else
            {
                if (x < 4)
                    x = 4;
            }
        }
        if (Y.text == "")
            y = 5;
        else
        {
            if (!int.TryParse(Y.text, out y))
                y = 5;
            else
            {
                if (y < 4)
                    y = 4;
            }
        }
        InputPanel.SetActive(false);
        ScorePanel.SetActive(true);
        Grid.Init(x,y);
    }
}
