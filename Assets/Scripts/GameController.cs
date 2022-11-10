using UnityEngine;
using UnityEngine.UI;

//отвечает за логику игры в целом, размер сетки, старт, стоп
class GameController : MonoBehaviour
{
    [SerializeField]
    ElementGrid Grid;
    [SerializeField]
    GameObject InputPanel;
    [SerializeField]
    GameObject ScorePanel;
    [SerializeField]
    Text X;
    [SerializeField]
    Text Y;

    void Start()
    {
        if (Grid == null)
        {
            //просто проверка если грид не стоит
            Grid = FindObjectOfType<ElementGrid>();
        }
    }

    //метод запуска с кнопки
    public void StartGame()
    {
        int x = 6, y = 6;
        int.TryParse(X.text, out x);
        int.TryParse(Y.text, out y);

        InputPanel.SetActive(false);
        ScorePanel.SetActive(true);
        //запускаем сетку
        Grid.Init(Mathf.Max(5, x),Mathf.Max(5, y));
    }
}
