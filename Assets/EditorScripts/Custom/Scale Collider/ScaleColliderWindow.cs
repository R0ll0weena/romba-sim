using UnityEditor;
using UnityEngine;

public class ScaleColliderWindow : EditorWindow
{
    private float percentage = 100f;
    private bool onlyTriggerColliders;
    private bool onlyNonTriggerColliders;

    [MenuItem("Custom/Scale Collider", priority = 0)]
    private static void Open()
    {
        ScaleColliderWindow window = GetWindow<ScaleColliderWindow>(true, "Scale Collider", true);
        window.minSize = new Vector2(280f, 130f);
        window.maxSize = new Vector2(280f, 130f);
        window.Show();
    }

    [MenuItem("Custom/Scale Collider", true)]
    private static bool CanOpen()
    {
        return Selection.gameObjects.Length > 0;
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Scale selected colliders by percentage.");
        percentage = EditorGUILayout.FloatField("Percentage", percentage);
        onlyTriggerColliders = EditorGUILayout.ToggleLeft("Only Is Trigger colliders", onlyTriggerColliders);
        onlyNonTriggerColliders = EditorGUILayout.ToggleLeft("Only non-trigger colliders", onlyNonTriggerColliders);

        using (new EditorGUI.DisabledScope(percentage <= 0f))
        {
            if (GUILayout.Button("Scale Colliders"))
            {
                ScaleSelectedColliders();
                Close();
            }
        }
    }

    private void ScaleSelectedColliders()
    {
        float factor = percentage / 100f;
        int changedCount = 0;
        int unsupportedCount = 0;
        foreach (GameObject gameObject in Selection.gameObjects)
        {
            foreach (Collider collider in gameObject.GetComponents<Collider>())
            {
                if (!ShouldAffect(collider.isTrigger))
                {
                    continue;
                }

                if (ScaleCollider(collider, factor))
                {
                    changedCount++;
                }
                else
                {
                    unsupportedCount++;
                }
            }

            foreach (Collider2D collider in gameObject.GetComponents<Collider2D>())
            {
                if (!ShouldAffect(collider.isTrigger))
                {
                    continue;
                }

                if (ScaleCollider(collider, factor))
                {
                    changedCount++;
                }
                else
                {
                    unsupportedCount++;
                }
            }
        }

        if (changedCount == 0)
        {
            EditorUtility.DisplayDialog("Scale Collider", "No supported colliders were found on the selected objects.", "OK");
        }
        else if (unsupportedCount > 0)
        {
            Debug.LogWarning($"Scaled {changedCount} collider(s). {unsupportedCount} collider(s) were not supported by this tool.");
        }
    }

    private bool ShouldAffect(bool isTrigger)
    {
        if (onlyTriggerColliders == onlyNonTriggerColliders)
        {
            return true;
        }

        return onlyTriggerColliders == isTrigger;
    }

    private static bool ScaleCollider(Collider collider, float factor)
    {
        if (collider is BoxCollider boxCollider)
        {
            Undo.RecordObject(boxCollider, "Scale Colliders");
            boxCollider.size *= factor;
            return true;
        }

        if (collider is SphereCollider sphereCollider)
        {
            Undo.RecordObject(sphereCollider, "Scale Colliders");
            sphereCollider.radius *= factor;
            return true;
        }

        if (collider is CapsuleCollider capsuleCollider)
        {
            Undo.RecordObject(capsuleCollider, "Scale Colliders");
            capsuleCollider.radius *= factor;
            capsuleCollider.height *= factor;
            return true;
        }

        if (collider is CharacterController characterController)
        {
            Undo.RecordObject(characterController, "Scale Colliders");
            characterController.radius *= factor;
            characterController.height *= factor;
            return true;
        }

        if (collider is WheelCollider wheelCollider)
        {
            Undo.RecordObject(wheelCollider, "Scale Colliders");
            wheelCollider.radius *= factor;
            wheelCollider.suspensionDistance *= factor;
            return true;
        }

        return false;
    }

    private static bool ScaleCollider(Collider2D collider, float factor)
    {
        if (collider is BoxCollider2D boxCollider)
        {
            Undo.RecordObject(boxCollider, "Scale Colliders");
            boxCollider.size *= factor;
            return true;
        }

        if (collider is CircleCollider2D circleCollider)
        {
            Undo.RecordObject(circleCollider, "Scale Colliders");
            circleCollider.radius *= factor;
            return true;
        }

        if (collider is CapsuleCollider2D capsuleCollider)
        {
            Undo.RecordObject(capsuleCollider, "Scale Colliders");
            capsuleCollider.size *= factor;
            return true;
        }

        if (collider is PolygonCollider2D polygonCollider)
        {
            Undo.RecordObject(polygonCollider, "Scale Colliders");
            for (int pathIndex = 0; pathIndex < polygonCollider.pathCount; pathIndex++)
            {
                Vector2[] points = polygonCollider.GetPath(pathIndex);
                for (int pointIndex = 0; pointIndex < points.Length; pointIndex++)
                {
                    points[pointIndex] *= factor;
                }

                polygonCollider.SetPath(pathIndex, points);
            }

            return true;
        }

        return false;
    }
}