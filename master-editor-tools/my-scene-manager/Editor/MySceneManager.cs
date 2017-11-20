using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System;
#if UNITY_5_3 || UNITY_2017
    using UnityEditor.SceneManagement;
#endif

public class EditorSceneClass
{
    public string sceneName;
    public bool isAddedToBuildSettings;
    public string scenePath;
}


public class MySceneManager : EditorWindow
{

    [MenuItem("[Master_Tools]/Scene Manager")]
    static void Init()
    {

        GetWindow(typeof(MySceneManager));
        GetWindow(typeof(MySceneManager)).minSize = new Vector2(250, 200);
        GetWindow(typeof(MySceneManager)).titleContent = new GUIContent("All Scenes v0.2");
    }
    public MySceneManager()
    {
        sceneData = new List<EditorSceneClass>();
        buildSettingsSceneNames = new List<string>();
        editModesOfScenes = new List<bool>();
        newNames = new List<string>();
    }

    void Awake()
    {

    }


    GUIStyle styleHelpboxInner;
    GUIStyle titleLabel, editorAddedButtonStyle, normalButtonStyle;
    Texture editButtonIcon, saveButtonIcon;
    void InitStyles()
    {
        styleHelpboxInner = new GUIStyle("HelpBox");
        styleHelpboxInner.padding = new RectOffset(6, 6, 6, 6);

        titleLabel = new GUIStyle();
        titleLabel.fontSize = 10;
        titleLabel.normal.textColor = Color.white;
        titleLabel.alignment = TextAnchor.UpperCenter;
        titleLabel.fixedHeight = 15;

        editorAddedButtonStyle = new GUIStyle(GUI.skin.button);
        editorAddedButtonStyle.alignment = TextAnchor.MiddleLeft;
        editorAddedButtonStyle.normal.textColor = Color.yellow;

        normalButtonStyle = new GUIStyle(GUI.skin.button);
        normalButtonStyle.alignment = TextAnchor.MiddleLeft;

    }
    Vector2 scrollPos;
    List<EditorSceneClass> sceneData;
    List<string> buildSettingsSceneNames;
    List<bool> editModesOfScenes;
    List<string> newNames;
    bool isEditModeOpen = false;
    void OnGUI()
    {
        InitStyles();
        styleHelpboxInner = new GUIStyle("HelpBox");
        styleHelpboxInner.padding = new RectOffset(6, 6, 6, 6);
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, false, GUILayout.Width(position.width), GUILayout.Height(position.height));
        GUILayout.BeginHorizontal(styleHelpboxInner);
        GUILayout.FlexibleSpace();
        GUILayout.Label("All scenes that are in the Scenes folder", titleLabel);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.BeginVertical(styleHelpboxInner);
        GetSceneNames();
        if (sceneData.Count == 0)
            GUILayout.Label("No scenes in the Scenes folder.", titleLabel);
        else
            for (int count = 0; count < sceneData.Count; count++)
            {
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();

                if (!editModesOfScenes[count])
                {
                    newNames[count] = sceneData[count].sceneName;
                    if (sceneData[count].isAddedToBuildSettings)
                    {
                        if (GUILayout.Button(sceneData[count].sceneName, editorAddedButtonStyle, GUILayout.MinWidth(200), GUILayout.MaxWidth(1000), GUILayout.Height(20)))
                        {
                        #if UNITY_5
                            if (EditorApplication.SaveCurrentSceneIfUserWantsTo())
                            {
                                EditorApplication.OpenScene(sceneData[count].scenePath + "/" + sceneData[count].sceneName + ".unity");
                            }
                        #endif
                            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                            {
                                EditorSceneManager.OpenScene(sceneData[count].scenePath + "/" + sceneData[count].sceneName + ".unity");
                            }
                        }
                    }
                    else
                    {
                        if (GUILayout.Button(sceneData[count].sceneName, normalButtonStyle, GUILayout.MinWidth(200), GUILayout.MaxWidth(1000), GUILayout.Height(20)))
                        {
                        #if UNITY_5
                            if (EditorApplication.SaveCurrentSceneIfUserWantsTo())
                            {
                                EditorApplication.OpenScene(sceneData[count].scenePath + "/" + sceneData[count].sceneName + ".unity");
                             }
                        #endif
                            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                            {
                                EditorSceneManager.OpenScene(sceneData[count].scenePath + "/" + sceneData[count].sceneName + ".unity");
                            }
                        }
                    }

                    if (GUILayout.Button(editButtonIcon, normalButtonStyle, GUILayout.Width(30), GUILayout.Height(20)))
                    {
                        newNames[count] = sceneData[count].sceneName;
                        editModesOfScenes[count] = true;
                        ResetAllEditings(count);
                        isEditModeOpen = true;
                    }
                }
                else
                {

                    newNames[count] = GUILayout.TextField(newNames[count], GUILayout.Width(200), GUILayout.Height(20));
                    if (GUILayout.Button(saveButtonIcon, GUILayout.Width(30), GUILayout.Height(20)))
                    {
                        var dirInfo = new DirectoryInfo(Application.dataPath + "/Scenes");
                        var allFileInfos = dirInfo.GetFiles("*.unity", SearchOption.AllDirectories);
                        foreach (var fileInfo in allFileInfos)
                        {
                            if (fileInfo.Name.Equals(sceneData[count].sceneName + ".unity"))
                            {
                                AssetDatabase.RenameAsset("Assets/Scenes/" + sceneData[count].sceneName + ".unity", newNames[count]);
                                AssetDatabase.Refresh();
                                editModesOfScenes[count] = false;
                                isEditModeOpen = false;

                            }
                        }
                    }
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }

        GUILayout.EndVertical();
        GUILayout.EndScrollView();
    }

    void ResetAllEditings(int tempCount)
    {
        for (int count = 0; count < editModesOfScenes.Count; count++)
        {
            if (count != tempCount)
                editModesOfScenes[count] = false;
        }
    }

    void GetSceneNames()
    {
        editButtonIcon = Resources.Load("toolPencil") as Texture;
        saveButtonIcon = Resources.Load("save") as Texture;
        if (isEditModeOpen)
            return;
        string folderName = Application.dataPath + "/Scenes";
        
        if (Directory.Exists(Application.dataPath+"/Scenes"))
        {
            try
            {
                var dirInfo = new DirectoryInfo(folderName);
                var allFileInfos = dirInfo.GetFiles("*.unity", SearchOption.AllDirectories);

                sceneData.Clear();
                editModesOfScenes.Clear();
                newNames.Clear();
                foreach (var fileInfo in allFileInfos)
                {
                    EditorSceneClass tempData = new EditorSceneClass();
                    tempData.sceneName = getSceneName(fileInfo.Name);
                    //string[] newStringCOl = fileInfo.DirectoryName.Split(new String[] { "/Scenes/" }, StringSplitOptions.None);
                    tempData.scenePath = fileInfo.DirectoryName;
                    tempData.isAddedToBuildSettings = false;
                    sceneData.Add(tempData);
                    editModesOfScenes.Add(false);
                    newNames.Add("0");
                }
                //   try
                //    {
                //        sceneData.Sort();
                //    }
                //    catch (System.Exception ex)
                //    {
                //       Debug.Log(ex);
                //    }

                buildSettingsSceneNames.Clear();
                for (int count = 0; count < EditorBuildSettings.scenes.Length; count++)
                {
                    string tempNamSSS = EditorBuildSettings.scenes[count].path;
                    string[] newStringCOl = tempNamSSS.Split('/');
                    buildSettingsSceneNames.Add(getSceneName(newStringCOl[newStringCOl.Length - 1]));
                }
                for (int sceneCount = 0; sceneCount < sceneData.Count; sceneCount++)
                {
                    for (int count = 0; count < buildSettingsSceneNames.Count; count++)
                    {
                        if (sceneData[sceneCount].sceneName.Equals(buildSettingsSceneNames[count]))
                        {
                            sceneData[sceneCount].isAddedToBuildSettings = true;
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning(ex);
            }
        }
        else
        {
            sceneData.Clear();
        }
       
    }



    string getSceneName(string tempName)
    {
        string extName = ".unity";
        int index1 = tempName.IndexOf(extName);
        string result2 = tempName.Remove(index1);
        return result2;
    }

}