using System.Collections.Generic;
using System;
using Mentor.RpgCore.Database;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace Mentor.RpgCore.Editor
{
    public class RpgBaseWindowTemplate<T> : EditorWindow where T : RpgBaseDBEntry
    {
        VisualTreeAsset _original;
        VisualTreeAsset _listEntrySingle;
        T _currentBaseDB;
        List<T> _allBaseDB;
        VisualElement _currentListEntry;
        Image _currentListEntryImage;
        Label _currentListEntryLabel;
        bool _isInitialized;

        private Type _targetType;

        public virtual void InitializeWindow(Type typeFor)
        {
            _targetType = typeFor;

            if (_targetType == null)
            {
                Debug.LogError("Specific type for RPGBaseWindowTemplate was not set.");
                return;
            }

            _original = Resources.Load<VisualTreeAsset>("UIToolkit/Documents/BasePanels/BaseWindowTemplate");
            if (_original == null)
            {
                Debug.LogError($"Failed to load BaseWindowTemplate UXML. Ensure path is correct.");
                return;
            }

            _listEntrySingle = Resources.Load<VisualTreeAsset>("UIToolkit/Documents/BasePanels/BaseListSection/SingleEntryWithIcon");

            if (_listEntrySingle == null)
            {
                Debug.LogWarning(
                    $"Failed to load VisualTreeAsset for list entry. Path: UIToolkit/Documents/BasePanels/BaseListSection/SingleEntryWithIcon");
                return;
            }

            TemplateContainer treeAsset = _original.CloneTree();
            rootVisualElement.Add(treeAsset);
            InitializeGenericFields();
            _isInitialized = true;
            BindData();
        }

        private void OnEnable()
        {
            if (_isInitialized)
                BindData();
        }

        private void BindData()
        {
            if (_original == null)
            {
                Debug.LogError($"Failed to load BaseWindowTemplate UXML. Ensure path is correct.");
                return;
            }

            if (_listEntrySingle == null)
            {
                Debug.LogWarning(
                    $"Failed to load VisualTreeAsset for list entry. Path: UIToolkit/Documents/BasePanels/BaseListSection/SingleEntryWithIcon");
                return;
            }

            _currentBaseDB = null;

            if (_allBaseDB.Count == 0)
            {
                Debug.LogWarning($"No assets of type {_targetType.Name} found.");
                return; // Exit if no assets are found
            }

            ScrollView listScrollView = rootVisualElement.Query<ScrollView>("LeftList_ScrollView").First();
            int cnt = 0;

            foreach (T entry in _allBaseDB)
            {
                TemplateContainer entryAsset = _listEntrySingle.CloneTree();
                listScrollView.Add(entryAsset);

                VisualElement entryPanel = entryAsset.Query<VisualElement>("ListEntry_Panel_X").First();
                entryPanel.name = "ListEntry_Panel_" + cnt.ToString();

                #region Entry Icon

                Image entryIcon = entryAsset.Query<Image>("ListEntry_X_Icon_Image").First();

                if (entryIcon == null)
                    Debug.LogWarning("No Entry Icon found.");
                else
                {
                    entryIcon.name = "ListEntry_" + cnt.ToString() + "_Icon_Image";
                    entryIcon.image = entry.Icon != null ? entry.Icon.texture : null;
                }

                #endregion

                #region Entry Label

                Label entryLabel = entryAsset.Query<Label>("ListEntry_X_Name_Label").First();

                if (entryIcon == null)
                    Debug.LogWarning("No Entry Icon found.");
                else
                {
                    entryLabel.name = "ListEntry_" + cnt + "_Name_Label";
                    entryLabel.text = entry.DisplayName;
                }

                #endregion

                #region Entry Selection

                entryPanel.RegisterCallback<ClickEvent>(_ =>
                {
                    _currentBaseDB = entry;
                    _currentListEntry = entryPanel;
                    _currentListEntryImage = entryIcon;
                    _currentListEntryLabel = entryLabel;
                    BindCurrentInfo();
                });

                #endregion

                cnt++;
            }

            _currentBaseDB = _allBaseDB[0];
            _currentListEntry = rootVisualElement.Query<VisualElement>("ListEntry_Panel_0").First();
            _currentListEntryImage = rootVisualElement.Query<Image>("ListEntry_0_Icon_Image").First();
            _currentListEntryLabel = rootVisualElement.Query<Label>("ListEntry_0_Name_Label").First();
            BindCurrentInfo();

            BindMenuButtons();
        }

        protected virtual void InitializeGenericFields()
        {
            if (_targetType == null)
            {
                Debug.LogError("Cannot initialize generic fields: targetType is null.");
                _allBaseDB = new List<T>(); // Initialize to empty list to prevent NullReferenceException later
                return;
            }

            string[] guids = AssetDatabase.FindAssets($"t:{_targetType.Name}");
            _allBaseDB = new List<T>();

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                T entry = AssetDatabase.LoadAssetAtPath<T>(path);
                if (entry != null)
                {
                    _allBaseDB.Add(entry);
                }
                else
                {
                    Debug.LogWarning("No BaseDBEntry assets found or loaded.");
                }
            }
        }

        protected virtual void BindCurrentInfo()
        {
            if (_currentBaseDB == null)
                return;

            #region ID Field

            TextField id = rootVisualElement.Query<TextField>("BaseInfo_ID_TextField").First();

            if (id == null)
                Debug.LogWarning("No ID Text Field found.");
            else
            {
                id.value = _currentBaseDB.ID.ToString();
                id.RegisterValueChangedCallback(evt =>
                {
                    if (int.TryParse(evt.newValue, out int newID))
                    {
                        _currentBaseDB.ID = newID;
                        // If you want changes to be saved immediately to the asset:
                        EditorUtility.SetDirty(_currentBaseDB);
                        //AssetDatabase.SaveAssets(); // Uncomment if you want to force save
                    }
                });
            }

            #endregion

            #region Name Field

            TextField nameElement = rootVisualElement.Query<TextField>("BaseInfo_Name_TextField").First();

            if (nameElement == null)
                Debug.LogWarning("No Name Text Field found");
            else
            {
                nameElement.value = _currentBaseDB.Name;
                nameElement.RegisterValueChangedCallback(evt =>
                {

                    _currentBaseDB.Name = evt.newValue;
                    // If you want changes to be saved immediately to the asset:
                    EditorUtility.SetDirty(_currentBaseDB);
                    //AssetDatabase.SaveAssets(); // Uncomment if you want to force save
                });
            }

            #endregion

            #region Display Name Field

            TextField displayName = rootVisualElement.Query<TextField>("BaseInfo_DisplayName_TextField").First();

            if (displayName == null)
                Debug.LogWarning("No Display Name Text Field found");
            else
            {
                displayName.value = _currentBaseDB.DisplayName;
                displayName.RegisterValueChangedCallback(evt =>
                {

                    _currentBaseDB.DisplayName = evt.newValue;
                    if (_currentListEntryLabel != null)
                        _currentListEntryLabel.text = evt.newValue;
                    // If you want changes to be saved immediately to the asset:
                    EditorUtility.SetDirty(_currentBaseDB);
                    //AssetDatabase.SaveAssets(); // Uncomment if you want to force save
                });
            }

            #endregion

            #region Description Field

            TextField description = rootVisualElement.Query<TextField>("BaseInfo_Description_TextField").First();

            if (description == null)
                Debug.LogWarning("No Description Text Field found");
            else
            {
                description.value = _currentBaseDB.Description;
                description.RegisterValueChangedCallback(evt =>
                {

                    _currentBaseDB.Description = evt.newValue;
                    // If you want changes to be saved immediately to the asset:
                    EditorUtility.SetDirty(_currentBaseDB);
                    //AssetDatabase.SaveAssets(); // Uncomment if you want to force save
                });
            }

            #endregion

            #region Icon Field

            Image infoIcon = rootVisualElement.Query<Image>("BaseInfo_Icon_Image").First();
            ObjectField iconSelector = rootVisualElement.Query<ObjectField>("BaseInfo_Icon_ObjectField").First();

            if (iconSelector == null)
                Debug.LogWarning("No Info Icon Selector Field found");
            else
            {
                iconSelector.value = _currentBaseDB.Icon != null ? _currentBaseDB.Icon.texture : null;

                iconSelector.RegisterValueChangedCallback(evt =>
                {
                    Sprite newSprite = evt.newValue as Sprite;
                    if (newSprite != null)
                    {
                        _currentBaseDB.Icon = newSprite;
                        infoIcon.image = newSprite.texture;
                        if (_currentListEntryImage != null)
                            _currentListEntryImage.image = newSprite.texture;
                    }
                    else
                    {
                        _currentBaseDB.Icon = null;
                        infoIcon.image = null;
                        if (_currentListEntryImage != null)
                            _currentListEntryImage.image = null;
                    }

                    // If you want changes to be saved immediately to the asset:
                    EditorUtility.SetDirty(_currentBaseDB);
                    //AssetDatabase.SaveAssets(); // Uncomment if you want to force save
                });
            }

            if (infoIcon == null)
                Debug.LogWarning("No Info Icon Image Field found");
            else
                infoIcon.image = _currentBaseDB.Icon != null ? _currentBaseDB.Icon.texture : null;

            #endregion
        }

        protected virtual void BindMenuButtons()
        {
        }
    }
}