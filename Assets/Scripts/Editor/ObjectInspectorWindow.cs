using System.Reflection;
using UnityEngine;
using UnityEditor;

public class ObjectInspectorWindow : MonoBehaviour
{
    [MenuItem("Assets/Create Inspector %g", false, 0)]
    public static void OpenEditorWindow(MenuCommand menuCommand) {
        EditorWindow inspector = EditorWindow.GetWindow(typeof(Editor).Assembly.GetType("UnityEditor.InspectorWindow"));
        Vector2 size = new Vector2(inspector.position.width, inspector.position.height);
        inspector = Instantiate(inspector);
        inspector.minSize = size;
        System.Type type = Assembly.GetAssembly(typeof(Editor)).GetType("UnityEditor.InspectorWindow");
        PropertyInfo propertyInfo = type.GetProperty("isLocked");
        propertyInfo.SetValue(inspector, true, null);
        inspector.Show();
        inspector.Focus();
    }
}