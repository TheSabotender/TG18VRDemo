using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class ProfileManager : MonoBehaviour
{
    public static Profile current;

    [System.Serializable]
    public class Profile
    {
        [System.Serializable]
        public class Currency
        {
            public string name;
            public int amount;            
        }

        [SerializeField] private string _userId;
        public string UserId { get { return _userId; } set { _userId = value; _updated = true; } }

        [SerializeField] private string _creationDate;
        public string CreationDate { get { return _creationDate; } }

        [SerializeField] public string _username;
        public string Username { get { return _username; } set { _username = value; _updated = true; } }

        [SerializeField] private Currency[] _currencies;
        public string[] currencies { get { List<string> l = new List<string>(); foreach (Currency c in _currencies) l.Add(c.name); return l.ToArray(); } }

        public int GetCurrency(string key)
        {
            if (_currencies == null) _currencies = new Currency[0];
            foreach (Currency c in _currencies)
            {
                if (c.name == key)
                {
                    return c.amount;
                }
            }
            return 0;
        }
        public void SetCurrency(string key, int newValue) { if (_currencies == null) _currencies = new Currency[0]; _updated = true; foreach (Currency c in _currencies) { if (c.name == key) { c.amount = newValue; return; } } List<Currency> l = new List<Currency>(_currencies); l.Add(new Currency(){name = key, amount = newValue}); _currencies = l.ToArray(); }

        [SerializeField] private string[] _unlocks;
        public string[] Unlocks { get { if (_unlocks == null) _unlocks = new string[0]; return _unlocks; } }
        public void Unlock(string key) { if (_unlocks == null) _unlocks = new string[0]; List<string> l = new List<string>(_unlocks); if(!l.Contains(key)) l.Add(key); _unlocks = l.ToArray(); _updated = true; }
        public bool HasUnlock(string key) { if (_unlocks == null) return false; return Unlocks.Contains(key); }

        [NonSerialized] private bool _updated;
        public bool OutOfDate { get { return _updated; } set { _updated = value; } }

        public Profile()
        {
            _userId = SystemInfo.deviceUniqueIdentifier;
            _creationDate = System.DateTime.UtcNow.ToString();

            _username = "";

            _currencies = new Currency[1];
            _currencies[0] = new Currency(){name = "credits", amount = 100};

            _unlocks = new string[0];
        }
    }

    public float dirtyTime = 60;
    private float updateTime;

	// Use this for initialization
	void Start () {        
	    //Login
        //wait for result

	    bool valid = false;
	    if (File.Exists(Application.persistentDataPath + "/profile.save"))
	    {
	        var json = File.ReadAllText(Application.persistentDataPath + "/profile.save");
	        var p = JsonUtility.FromJson<Profile>(json);
	        if (p != null)
	        {
	            valid = true;
	            OnReceiveProfile(p);
	        }
	    }

	    if(!valid)
	    {
            current = new Profile();
	        SaveProfile();

            OnReceiveProfile(current);
	    }
	}

    void Update()
    {
        if (current != null && current.OutOfDate)
        {
            if (updateTime > dirtyTime)
            {
                SaveProfile();
            }
            else
            {
                updateTime += Time.deltaTime;
            }
        }
        else
        {
            updateTime = 0;
        }
    }
    
    void OnReceiveProfile(Profile p)
    {
        current = p;
    }

    [ExposeInEditor]
    void CheckProfilePath()
    {
        if (File.Exists(Application.persistentDataPath + "/profile.save"))
        {
            Debug.Log("Found: " + Application.persistentDataPath + "/profile.save");

            var json = File.ReadAllText(Application.persistentDataPath + "/profile.save");
            var p = JsonUtility.FromJson<Profile>(json);
            if (p != null)
            {
                Debug.Log("Profile is valid.");
            }
            else
            {
                Debug.Log("Profile not valid.");
            }
        }
        else
        {
            Debug.Log("No file at: " + Application.persistentDataPath + "/profile.save");
        }
    }

    [ExposeInEditor(allowInEditor = false)]
    void CheckCurrentProfile()
    {
        if (current == null)
        {
            Debug.Log("No profile loaded");
            return;
        }

        string result = "Current Profile:\n" +
                        "userId=" + current.UserId + "\n" +
                        "username=" + current.Username;

        foreach (string key in current.currencies)
        {
            result += "\n" + key + "=" + current.GetCurrency(key);
        }

        foreach (string unlock in current.Unlocks)
        {
            result += "\n" + unlock + " is unlocked";
        }

        Debug.Log(result);
    }

    [ExposeInEditor(allowInEditor = false)]
    public void SaveProfile()
    {
        if (current == null)
            return;

        var json = JsonUtility.ToJson(current, true);
        File.WriteAllText(Application.persistentDataPath + "/profile.save", json);
        current.OutOfDate = false;
        updateTime = 0;
    }

#if UNITY_EDITOR
    [ExposeInEditor]
    void OpenSavePath()
    {
        if (File.Exists(Application.persistentDataPath + "/profile.save"))
        {
            var itemPath = (Application.persistentDataPath + "/profile.save").Replace(@"/", @"\"); // explorer doesn't like front slashes
            System.Diagnostics.Process.Start("explorer.exe", "/select," + itemPath);
        }
        else
        {
            string path = (Application.persistentDataPath + "/").TrimEnd(new[]{'\\', '/'}); // Mac doesn't like trailing slash
            System.Diagnostics.Process.Start(path);
        }
    }
#endif
}
