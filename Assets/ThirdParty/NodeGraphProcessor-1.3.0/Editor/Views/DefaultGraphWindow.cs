using GraphProcessor;
using UnityEditor;
using UnityEngine;

namespace ThirdParty.NodeGraphProcessor_1._3._0.Editor.Views
{
    public class DefaultGraphWindow : BaseGraphWindow
    {
        [MenuItem("Window/DefaultGraph")]
        public static BaseGraphWindow Open()
        {
            var graphWindow = GetWindow< DefaultGraphWindow >();

            graphWindow.Show();

            return graphWindow;
        }

        protected override void InitializeWindow(BaseGraph graph)
        {
            // Set the window title
            titleContent = new GUIContent("Default Graph");

            // Here you can use the default BaseGraphView or a custom one (see section below)
            var graphView = new BaseGraphView(this);
            graphView.Add(new ToolbarView(graphView));
            graphView.Add(new MiniMapView(graphView));

            rootView.Add(graphView);
        }
    }
}
