using UnityEngine;

class Selector : MonoBehaviour
{
    private Transform _element;

    [SerializeField]
    ElementGrid _grid;


    //штука, которая отвечает за выбор элемента, переделывать я ее не буду
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hitInfo = new RaycastHit();
            bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);
            if (hit)
            {
                //чекаем что попали в блок
                if (hitInfo.transform.gameObject.tag == "Element")
                {
                    if (hitInfo.transform == _element)
                    {
                        //меняем обратно если передумали
                        _element.GetComponent<Renderer>().material.shader = Shader.Find("Standard");
                        _element = null;
                        return;//когда у тебя есть такие ифы, лучше сначала ставить те, которые делают что-то мелкое
                        //и писать ретурн после этого, потому что остальная логика не должна отрабатывать в принципе
                    }
                    if (_element == null)
                    {
                        //если элемент не выбран, то выбираем его и меняем ему шейдер
                        _element = hitInfo.transform;
                        //это костыльная херня, надо не менять шейдер, а менять одно свойство в шейдере
                        //но я сделал это за 3 часа поэтому забил
                        _element.GetComponent<Renderer>().material.shader = Shader.Find("Legacy Shaders/Self-Illumin/Diffuse");
                        return;
                        //возврат в ифе позволяет тебе не засорять код и не писать ненужный else
                    }
                    //если выбран другой элемент и есть выбранный элемент, то пытаемся его сместить
                    var first = _element.gameObject.GetComponent<Element>();
                    var second = hitInfo.transform.gameObject.GetComponent<Element>();

                    int xdiff = first.X - second.X;
                    int ydiff = first.Y - second.Y;
                    //чекаем, что он соседний. Вообще неплохо их линковать, но, опять же, я забил
                    if (xdiff < 2 && ydiff < 2 && (ydiff + xdiff < 2))
                    {
                        //говорит сетке, чтобы попробовала наш выбор
                        _grid.TryChange(first, second);
                    }

                    //меняем материал обратно и обнуляем выбор, хехе
                    _element.GetComponent<Renderer>().material.shader = Shader.Find("Standard");
                    _element = null;
                }
            }
        }
    }
}
