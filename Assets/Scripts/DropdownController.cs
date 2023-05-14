using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System.Net.Http;
using System.Text;
using TMPro;

[System.Serializable]
public class SiteData
{
    public string name;
    public string url;
}

[System.Serializable]
public class SiteDataList
{
    public List<SiteData> example_sites;
    public List<SiteData> custom_sites;
}

public class DropdownController : MonoBehaviour
{
    public TMP_Dropdown dropdown;
    private XRLManager xrlManager;
    private Dictionary<string, string> valueMap = new();

    private async void Start()
    {
        // Set private variables
        xrlManager = FindObjectOfType<XRLManager>();
        
        // Add listener for value changes
        dropdown.onValueChanged.AddListener(delegate {xrlManager.LoadPage(valueMap[dropdown.options[dropdown.value].text]);});

        try {
            using (HttpClient client = new())
            {
                SiteDataList siteDataList = JsonUtility.FromJson<SiteDataList>(await client.GetStringAsync(xrlManager.serverUrl + "routes"));

                foreach (SiteData siteData in siteDataList.example_sites.Concat(siteDataList.custom_sites))
                {
                    valueMap[siteData.name] = siteData.url;
                    dropdown.options.Add(new(siteData.name));
                }
            }
        } catch (Exception)
        {
            valueMap["Internet error. Please check connection and restart."] = "https://www.google.com/";
            dropdown.options.Add(new("Internet error. Please check connection and restart."));
        }   

        // Set the initial value
        dropdown.value = 0;
        dropdown.RefreshShownValue();

        // Load the first site
        xrlManager.LoadPage(valueMap[dropdown.options[dropdown.value].text]);
    }
}
