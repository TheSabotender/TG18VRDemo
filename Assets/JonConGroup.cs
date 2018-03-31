using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JonConGroup : JoyControlled
{
    private JoyControlled[] _group;

    private JoyControlled[] Group
    {
        get
        {
            if (_group == null)
            {
                List<JoyControlled> cons = new List<JoyControlled>();
                foreach (JoyControlled jc in GetComponentsInChildren<JoyControlled>())
                {
                    if (jc != this)
                    {
                        cons.Add(jc);
                    }
                }
                _group = cons.ToArray();
            }
            return _group;
        }
    }

    public override float Current
    {
        get
        {
            float c = 0;
            foreach (JoyControlled jc in Group)
            {
                c += jc.Current;
            }
            return c / Group.Length;
        }
        set
        {
            foreach (JoyControlled jc in Group)
            {
                jc.Current = value;
            }
        }
    }

    public override float CalculateSuccess()
    {
        float s = 0;
        foreach (JoyControlled jc in Group)
        {
            s += jc.CalculateSuccess();
        }
        return s / Group.Length;
    }

    [ExposeInEditor(buttonNameOverride = "Group Update")]
    private void UpdateValues()
    {
        foreach (JoyControlled jc in Group)
        {
            jc.UpdateValues();
        }
    }


    [ExposeInEditor(buttonNameOverride = "Group Reset")]
    public override void Reset()
    {
        foreach (JoyControlled jc in Group)
        {
            jc.Reset();
        }
    }
}
