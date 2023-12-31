using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/*
    This script is for buttons that appear on a scrollable window and connect to a list.
*/
public class ScrollButton : MonoBehaviour
{
    [SerializeField] private int index;
    private IScrollSelector Selector;

    public void Setup(int index, IScrollSelector Selector)
    {
        this.index = index;
        this.Selector = Selector;

        // Set the onClick event
        GetComponent<Button>().onClick.AddListener(OnButtonClick);
    }

    public void OnButtonClick()
    {
        Selector.OnButtonClick(index);
    }
}