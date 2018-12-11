using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ElementGrid : MonoBehaviour
{
    public int YSize = 5;
    public int XSize = 5;
    
    public List<Material> Materials;

    public Element ElementPrefab;
    public GameObject BackgroudTile;
    public GameObject TileHolder;
    public GameObject ElementHolder;

    GameObject[,] tiles;
    private Element[,] elements;

    private int Score;
    public Text ScoreText;

	public void Init(int x, int y)
	{
	    XSize = x;
	    YSize = y;
        tiles = new GameObject[XSize, YSize];
        elements = new Element[XSize,YSize];
        SpawnGrid();
	    Score = 0;
	    ScoreText.text = Score.ToString();
    }

    public void TryChange(Element one, Element two)
    {
        int oneX = one.X;
        int oneY = one.Y;
        int twoX = two.X;
        int twoY = two.Y;

        elements[oneX, oneY] = two;
        elements[twoX, twoY] = one;

        if (CheckElement(oneX, oneY) || CheckElement(twoX, twoY))
        {
            one.X = twoX;
            one.Y = twoY;
            two.X = oneX;
            two.Y = oneY;

            one.gameObject.transform.position=new Vector3(one.X,one.Y);
            two.gameObject.transform.position = new Vector3(two.X, two.Y);
            PopLoop();
        }
        else
        {
            elements[oneX, oneY] = one;
            elements[twoX, twoY] = two;
        }
    }

    void SpawnGrid()
    {
        for (int i = 0; i < XSize; i++)
        {
            for (int j = 0; j < YSize; j++)
            {
                Vector2 temp = new Vector2(i, j);
                var tile = Instantiate(BackgroudTile, temp, Quaternion.identity);
                tile.transform.parent = TileHolder.transform;
                tiles[i, j] = tile;
                
                var element = CreateElement(temp);
                element.transform.parent = ElementHolder.transform;
                elements[i, j] = element;
                element.X = i;
                element.Y = j;
            }
        }
        CheckSame();
        PopLoop();
    }

    void PopLoop()
    {
        while (Pop())
        {
            AddNewElements();
            CheckSame();
        }
    }

    void AddNewElements()
    {
        for (int i = 0; i < XSize; i++)
        {
            for (int j = 0; j < YSize; j++)
            {
                if (elements[i, j] == null)
                {
                    var element = CreateElement(new Vector2(i, j));
                    element.transform.parent = ElementHolder.transform;
                    elements[i, j] = element;
                    element.X = i;
                    element.Y = j;
                }

            }
        }
    }
    
    bool Pop()
    {
        bool boardChanged = false;

        List<int> rowsToshift=new List<int>();
        foreach (Element element in elements)
        {
            if (element.Popped)
            {
                boardChanged = true;
                //element.gameObject.transform.localScale=new Vector3(0.2f,0.2f,0.2f); //debug view
                if (!rowsToshift.Contains(element.X))
                    rowsToshift.Add(element.X);
                Destroy(element.gameObject);
                elements[element.X, element.Y] = null;
                Score += 1;
                ScoreText.text = Score.ToString();
            }
        }

        foreach (int x in rowsToshift)
        {
            ShiftRow(x);
        }

        return boardChanged;
    }

    void ShiftRow(int x)
    {
        int y = 0;
        while (y<YSize)
        {
            if (elements[x, y] == null)
            {
                int tempy = y + 1;
                int yFrom = tempy;
                Element element=null;
                while (element==null && tempy<YSize)
                {
                    element = elements[x, tempy];
                    yFrom = tempy;
                    tempy++;
                }

                if (element != null)
                {
                    elements[x, y] = element;
                    //print("Shifting element " + element.X +" "+ element.Y+ " to " + x + " " + y);
                    element.X = x;
                    element.Y = y;
                    element.gameObject.transform.position = new Vector3(x,y,0f);
                    elements[x, yFrom] = null;
                }
            }
            y++;
        }
    }

    Element CreateElement(Vector2 position)
    {
        var go = Instantiate(ElementPrefab, position, Quaternion.identity);
        var mat = Materials[Random.Range(0, Materials.Count)];
        go.GetComponent<MeshRenderer>().sharedMaterial = mat;
        var el = go.GetComponent<Element>();
        el.color = mat.name;
        return el;
    }

    void CheckSame()
    {
        for (int i = 0; i < XSize; i++)
        {
            for (int j = 0; j < YSize; j++)
            {
                CheckElement(i, j);
            }
        }
    }

    void Traverse(int x, int y, string color, List<Element> same)
    {
        var element = elements[x, y];
        if (same.Contains(element))
            return;

        if (element.color==color)
            same.Add(element);
        else
        {
            return;
        }

        if (x - 1 > -1)
            Traverse(x-1,y,color,same);
        if (x+1<XSize)
            Traverse(x+1,y,color,same);

        if (y - 1 > -1)
            Traverse(x, y-1, color, same);
        if (y + 1 < YSize)
            Traverse(x, y+1, color, same);
    }

    bool CheckElement(int x, int y)
    {
        var element = elements[x, y];

        if (element.Popped)
            return true;

        string color = element.color;

        List<Element> same = new List<Element>();
        
        Traverse(x,y,color,same);

        if (same.Count > 2)
        {
            foreach (Element o in same)
            {
                o.Popped = true;
            }
            return true;
        }

        return false;
    }
}
