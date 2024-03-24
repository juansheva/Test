using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TextReaderLoading : MonoBehaviour
{
    [SerializeField] private string _path = Path.Combine(Application.streamingAssetsPath,"test.txt");

    [SerializeField] private Slider _loadingBar;

    [SerializeField] private TextScriptable _textScriptable;
    private void Start()
    {
        _loadingBar.value = 0;
        LoadTextFile(delegate
        {
            SceneManager.LoadScene("Procedural");
        });
    }

    public void LoadTextFile(Action completeloadtext)
    {

        StartCoroutine(LoadTextFileCorou(completeloadtext));
    }

    IEnumerator LoadTextFileCorou(Action completeloadtext)
    {

        if (File.Exists(_path))
        {
            using (StreamReader sr = new StreamReader(_path))
            {
                
                string line;
                _textScriptable.Line = 0;
                _textScriptable.Text = "";

                while ((line=sr.ReadLine()) != null)
                {
                    yield return new WaitForSeconds(0.01f);
                    _textScriptable.Line += 1;
                    _textScriptable.Text += line + "\n";
                    _loadingBar.value += _loadingBar.maxValue / 1024;
                }
                
                
                
                
                
                // int totalLine=0;
                // string line2;
                // while ((line2=sr.ReadLine()) != null)
                // {
                //     totalLine += 1;
                // }
                //
                // for (int i = 0; i < totalLine; i++)
                // {
                //     yield return new WaitForSeconds(0.05f);
                //     _loadingBar.value += _loadingBar.maxValue / totalLine;
                // }
                completeloadtext?.Invoke();
            }
        }

        yield return null;
    }

    public void MakeTextFile()
    {
        if (!File.Exists(_path))
        {
            using (StreamWriter sw = new StreamWriter(_path))
            {
                for (int i = 0; i < 1024; i++)
                {
                    sw.WriteLine(i);                
                }
            }
        }

    }
}
