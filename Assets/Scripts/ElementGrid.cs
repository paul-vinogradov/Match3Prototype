using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//самое сочное, сетка, тут вся логика самой игры
public class ElementGrid : MonoBehaviour
{
    #region serializes
    [SerializeField]
    List<Material> Materials;
    [SerializeField]
    Element ElementPrefab;
    [SerializeField]
    GameObject BackgroudTile;
    [SerializeField]
    GameObject TileHolder;
    [SerializeField]//вот, видишь, о чем я говорил, когда у тебя тонна сериалайзов
    GameObject ElementHolder;
    [SerializeField]//в глазах рябит
    Text ScoreText;
    #endregion

    #region privateFields
    //двойной массив, олдскул стайл
    GameObject[,] _tiles;
    Element[,] _elements;
    int _score;
    int _ySize = 5;
    int _xSize = 5;
    #endregion

    #region gridCreation
    //инитм сетку, обнуляем счем
    public void Init(int x, int y)
    {
        _xSize = x;
        _ySize = y;
        //так давно не видел двойной массив, немного ступор случился
        _tiles = new GameObject[_xSize, _ySize];
        _elements = new Element[_xSize, _ySize];
        SpawnGrid();
        _score = 0;
        ScoreText.text = _score.ToString();
    }

    void SpawnGrid()
    {
        for (int i = 0; i < _xSize; i++)
        {
            for (int j = 0; j < _ySize; j++)
            {
                Vector2 tempPosition = new Vector2(i, j);
                //делаем префабы для фона. Можно просто сделать большой фон и не заниматься этой фигней
                var tile = Instantiate(BackgroudTile, tempPosition, Quaternion.identity);
                tile.transform.parent = TileHolder.transform;
                _tiles[i, j] = tile;

                //создаем элемент и задаем его индексы
                var element = CreateElement(tempPosition);
                element.transform.parent = ElementHolder.transform;
                _elements[i, j] = element;
                element.X = i;
                element.Y = j;
            }
        }
        CheckAllElements();
        PopLoop();
    }

    //создаем элемент
    Element CreateElement(Vector2 position)
    {
        var go = Instantiate(ElementPrefab, position, Quaternion.identity);
        //выбираем ему рандомный цвет
        var mat = Materials[Random.Range(0, Materials.Count)];
        go.GetComponent<MeshRenderer>().sharedMaterial = mat;
        var el = go.GetComponent<Element>();
        el.Color = mat.name;
        return el;
    }
    #endregion

    #region elementChecking
    //метод попытки мува из селектора
    public void TryChange(Element one, Element two)
    {
        int oneX = one.X;
        int oneY = one.Y;
        int twoX = two.X;
        int twoY = two.Y;

        _elements[oneX, oneY] = two;
        _elements[twoX, twoY] = one;

        if (CheckElement(oneX, oneY) || CheckElement(twoX, twoY))
        {
            one.X = twoX;
            one.Y = twoY;
            two.X = oneX;
            two.Y = oneY;

            one.gameObject.transform.position = new Vector3(one.X, one.Y);
            two.gameObject.transform.position = new Vector3(two.X, two.Y);
            PopLoop();
        }
        else
        {
            _elements[oneX, oneY] = one;
            _elements[twoX, twoY] = two;
        }
    }

    //проверяем наличие одинаковых элементов во всей сетке
    void CheckAllElements()
    {
        //это полный треш, но это нам надо в случае, когда мы стартуем игру.
        //при старте игры у нас не должно быть цепочек в ней, так как это рандом и не хардков
        //гарантировать мы это не можем, поэтому приходится проверять, но это все равно неоптимально
        for (int i = 0; i < _xSize; i++)
        {
            for (int j = 0; j < _ySize; j++)
            {
                CheckElement(i, j);
            }
        }
    }

    bool CheckElement(int x, int y)
    {
        var element = _elements[x, y];

        //возвращаем тру если он уже проверен и в очереди на удаление
        if (element.Popped)
            return true;

        string color = element.Color;

        List<Element> same = new List<Element>();

        Traverse(x, y, color, same);

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

    //рекурсивный метод для прохода элементов
    void Traverse(int x, int y, string color, List<Element> same)
    {
        var element = _elements[x, y];

        //проверяем не прошли ли мы его уже
        if (same.Contains(element) || element.Color != color)
            return;//видишь как удобно, никаких элсов, никакого лишнего исполнения

        //если цвет такой же - добавляем в массив одинаковых
        same.Add(element);

        //и идем дальше во все стороны
        if (x - 1 > -1)
            Traverse(x - 1, y, color, same);
        if (x + 1 < _xSize)
            Traverse(x + 1, y, color, same);

        if (y - 1 > -1)
            Traverse(x, y - 1, color, same);
        if (y + 1 < _ySize)
            Traverse(x, y + 1, color, same);

        //тут, кстати, баг, потому что идти во все стороны надо только с первого элемента проверки
        //в противном случае у тебя может быть зигзаг из элементов и они все попадут в массив одинаковых
        //а хотя, кстати, вроде так и должно быть. Но в любом случае 3 в ряд должно быть точно
        //но тут нет проверки на это и это сработает сразу с зигзагом
        //менять я это, конечно же, не буду
    }
    #endregion

    #region elementDeletion
    //метод для удаления элементов пока сетка не стабилизируетя
    void PopLoop()
    {
        //пока мы хоть что-то удаляем, чекаем дальше
        while (Pop())
        {
            //добавляем новые элементы сверху
            AddNewElements();
            //чекаем все заново
            CheckAllElements();

            //чекать все заново - максимально неоптимально
            //это быстро работает, даже на огромных сетках, но все равно
            //проверять нужно только то, что было изменено, а остальное не трогать
        }
    }

    //вхвхв, он добавляет элементы не сверху, а везде, это неправильно
    //у меня есть метод смещения, он должен чередоваться с добавлением новых элементов
    void AddNewElements()
    {
        for (int i = 0; i < _xSize; i++)
        {
            for (int j = 0; j < _ySize; j++)
            {
                if (_elements[i, j] == null)
                {
                    //собсна везде где пусто - создаем новый блок
                    var element = CreateElement(new Vector2(i, j));
                    element.transform.parent = ElementHolder.transform;
                    _elements[i, j] = element;
                    element.X = i;
                    element.Y = j;
                }

            }
        }
    }

    bool Pop()
    {
        bool boardChanged = false;

        List<int> rowsToshift = new List<int>();

        //проверяем по каждому, втф, это тоже крайне неоптимально
        //ну, здесь нужен массив элементов на удаление, когда мы в траверсе их метим
        //менять я это, конечно же, не буду
        foreach (Element element in _elements)
        {
            if (element.Popped)
            {
                //если что-то помечено на удаление - отмечаем, что сетка изменится
                boardChanged = true;

                //отмечаем, что элементы в ряду нужно будет сместить вниз
                if (!rowsToshift.Contains(element.X))
                    rowsToshift.Add(element.X);

                //удаляем, тут должен быть пул, а не удаление
                Destroy(element.gameObject);

                //помечаем элемент как пустой
                //использование нула здесь - херня, нам не надо ни удалять ниче, ни создавать
                //так как элемент это класс, мы можем просто менять его свойства
                _elements[element.X, element.Y] = null;

                //обновляем счет
                _score += 1;
                ScoreText.text = _score.ToString();
            }
        }

        //смещаем ряды, в которых чет менялось
        foreach (int x in rowsToshift)
        {
            ShiftRow(x);
        }

        return boardChanged;
    }

    void ShiftRow(int x)
    {
        int y = 0;
        while (y < _ySize)
        {
            if (_elements[x, y] == null)
            {
                int tempY = y + 1;
                int yFrom = tempY;
                Element element = null;

                //берем первый элемент выше и смещаем его на этот ряд
                while (element == null && tempY < _ySize)
                {
                    element = _elements[x, tempY];
                    yFrom = tempY;
                    tempY++;
                }

                //если мы его таки нашли, закидываем его на новое место
                if (element != null)
                {
                    _elements[x, y] = element;
                    element.X = x;
                    element.Y = y;
                    element.gameObject.transform.position = new Vector3(x, y, 0f);
                    _elements[x, yFrom] = null;
                }
            }
            y++; //идем дальше по всему ряду

            //рофл в том, что это неправильно работает, так как во-первых, смещения неотсорированные
            //а во-вторых создание новых элементов и смещения происходят не одновременно
            //соответственно у нас может быть такое что мы сместили один ряд, а потом сместили другой над ним
            //теоретически у нас могут новые элементы создаться в середине сетки
        }
    }
    #endregion
}
