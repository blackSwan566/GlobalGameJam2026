using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(Light))]
public class ShaderLightSource : MonoBehaviour
{
    private Light _lightSource;

    // Shader property IDs are faster than strings
    private static readonly int LightPosID = Shader.PropertyToID("_LightPos");
    private static readonly int LightRangeID = Shader.PropertyToID("_LightRange");

    void OnEnable()
    {
        _lightSource = GetComponent<Light>();
        UpdateGlobals();
    }

    void Update()
    {
        // Keeps the globals in sync if the light moves or the range changes
        UpdateGlobals();
    }

    private void UpdateGlobals()
    {
        if (_lightSource == null) return;

        // Sets the global Vector4 (Position)
        Shader.SetGlobalVector(LightPosID, transform.position);

        // Sets the global Float (Range)
        Shader.SetGlobalFloat(LightRangeID, _lightSource.range);
    }
}
