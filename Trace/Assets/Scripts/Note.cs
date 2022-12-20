using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Note : MonoBehaviour
{
    [TextArea(2,5)]
    public List<string> Notes = new List<string>(); 
}
