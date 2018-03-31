using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Menu : MonoBehaviour {

    [System.Serializable]
    public class Item
    {
        public string Text;
        //public UnityAction onSelect;
        public UnityEvent onSelect;
    }

    public Item[] elements;

    private int _index;
    public int index
    {
        get { return _index; }
        set
        {
            _index = (int)Mathf.Repeat(value, elements.Length);
            destination = _index * (360f / elements.Length);
        }
    }

    public float Length { get { return elements.Length; } }
    public float rotateSpeed = 10;

    private float destination;
    public string Text { get { return elements[index].Text; }}

    public void Activate()
    {
        if (elements[index].onSelect != null)
        {
            elements[index].onSelect.Invoke();
        }
    }
    
    void Update()
    {
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, 0, destination), Time.deltaTime * 10f * rotateSpeed);
    }

#if UNITY_EDITOR
    [ExposeInEditor]
    private void Previous()
    {
        index = index - 1;
    }
    [ExposeInEditor]
    private void Next()
    {
        index = index + 1;
    }
#endif
}
