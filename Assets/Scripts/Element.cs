using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Element : MonoBehaviour
{
    //по факту это просто структура, но так как мне надо было,
    //чтоб она висела на объекте это класс от монобеха
    public string Color { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public bool Popped { get; set; }
}
