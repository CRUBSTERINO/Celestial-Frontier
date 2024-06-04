using System.IO;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace DialogSystem.Editor.Windows
{
    using Utilities;

    public class DialogEditorWindow : EditorWindow
    {
        private DialogGraphView _graphView;

        private readonly string _defaultFileName = "DialogsFileName";

        private static TextField _fileNameTextField;
        private Button _saveButton;
        private Button _miniMapButton;

        [MenuItem("Window/Dialog System/Dialog Graph")]
        public static void Open()
        {
            GetWindow<DialogEditorWindow>("Dialog Graph");
        }

        private void CreateGUI()
        {
            AddGraphView();
            AddToolbar();

            AddStyles();
        }

        private void AddGraphView()
        {
            _graphView = new DialogGraphView(this);

            _graphView.StretchToParentSize();

            rootVisualElement.Add(_graphView);
        }

        private void AddToolbar()
        {
            Toolbar toolbar = new Toolbar();

            _fileNameTextField = ElementUtility.CreateTextField(_defaultFileName, "File Name:", callback =>
            {
                _fileNameTextField.value = callback.newValue.RemoveWhitespaces().RemoveSpecialCharacters();
            });

            _saveButton = ElementUtility.CreateButton("Save", () => Save());

            Button loadButton = ElementUtility.CreateButton("Load", () => Load());
            Button clearButton = ElementUtility.CreateButton("Clear", () => Clear());
            Button resetButton = ElementUtility.CreateButton("Reset", () => ResetGraph());

            _miniMapButton = ElementUtility.CreateButton("Minimap", () => ToggleMiniMap());

            toolbar.Add(_fileNameTextField);
            toolbar.Add(_saveButton);
            toolbar.Add(loadButton);
            toolbar.Add(clearButton);
            toolbar.Add(resetButton);
            toolbar.Add(_miniMapButton);

            toolbar.AddStyleSheets("Dialog System/DSToolbarStyles.uss");

            rootVisualElement.Add(toolbar);
        }

        private void AddStyles()
        {
            rootVisualElement.AddStyleSheets("Dialog System/DSVariables.uss");
        }

        private void Save()
        {
            if (string.IsNullOrEmpty(_fileNameTextField.value))
            {
                EditorUtility.DisplayDialog("Invalid file name.", "Please ensure the file name you've typed in is valid.", "Roger!");

                return;
            }

            IOUtility.Initialize(_graphView, _fileNameTextField.value);
            IOUtility.Save();
        }

        private void Load()
        {
            string filePath = EditorUtility.OpenFilePanel("Dialogue Graphs", $"{IOConstants.ROOT_EDITOR_FOLDER_PATH}/Graphs", "asset");

            if (string.IsNullOrEmpty(filePath))
            {
                return;
            }

            Clear();

            IOUtility.Initialize(_graphView, Path.GetFileNameWithoutExtension(filePath));
            IOUtility.Load();
        }

        private void Clear()
        {
            _graphView.ClearGraph();
        }

        private void ResetGraph()
        {
            Clear();

            UpdateFileName(_defaultFileName);
        }

        private void ToggleMiniMap()
        {
            _graphView.ToggleMiniMap();

            _miniMapButton.ToggleInClassList("ds-toolbar__button__selected");
        }

        public static void UpdateFileName(string newFileName)
        {
            _fileNameTextField.value = newFileName;
        }

        public void EnableSaving()
        {
            _saveButton.SetEnabled(true);
        }

        public void DisableSaving()
        {
            _saveButton.SetEnabled(false);
        }
    } 
}