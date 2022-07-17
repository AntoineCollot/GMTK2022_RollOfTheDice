using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[ExecuteInEditMode]
public class LevelCreator : MonoBehaviour
{
    public Transform[] prefabs;
    public Transform spawnParent;
    Vector3 hoveredPosition;

    private void OnEnable()
    {
        if (!Application.isEditor)
        {
            Destroy(this);
            return;
        }
        SceneView.onSceneGUIDelegate += OnScene;
    }

    private void OnDisable()
    {
        SceneView.onSceneGUIDelegate -= OnScene;
    }

    void OnScene(SceneView scene)
    {
        Event e = Event.current;

        Vector3 mousePos = e.mousePosition;
        float ppp = EditorGUIUtility.pixelsPerPoint;
        mousePos.y = scene.camera.pixelHeight - mousePos.y * ppp;
        mousePos.x *= ppp;

        Ray ray = scene.camera.ScreenPointToRay(mousePos);
        RaycastHit hit;
        Vector3 randomDir = new Vector3(0, Random.Range(0, 4) * 90, 0);
        Vector3 roundedPos;

        if (Physics.Raycast(ray, out hit, 1 << gameObject.layer))
        {
            roundedPos = hit.point;
            roundedPos.x = Mathf.Round(roundedPos.x);
            roundedPos.y = 0;
            roundedPos.z = Mathf.Round(roundedPos.z);

            hoveredPosition = roundedPos;
        }
        else
            return;

        if (e.type == EventType.KeyDown)
        {
            int id = 0;
            switch(e.keyCode)
            {
                case KeyCode.Alpha1:
                    id = 0;
                    break;
                case KeyCode.Alpha2:
                    id = 1;
                    break;
                case KeyCode.Alpha3:
                    id = 2;
                    break;
                case KeyCode.Alpha4:
                    id = 3;
                    break;
                case KeyCode.Alpha5:
                    id = 4;
                    break;
                case KeyCode.Alpha6:
                    id = 5;
                    break;
                default:
                    return;
            }

            Transform newObj = Instantiate(prefabs[id], roundedPos + prefabs[id].position, Quaternion.Euler(randomDir), spawnParent);
            newObj.name = $"{prefabs[id].name} ({roundedPos.x},{roundedPos.z})";

            e.Use();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 1, 1, 0.3f);
        Gizmos.DrawCube(hoveredPosition, new Vector3(1, 0, 1));
    }
}
#endif