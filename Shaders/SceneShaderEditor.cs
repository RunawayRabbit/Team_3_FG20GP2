using UnityEngine;

public class SceneShaderEditor : MonoBehaviour {
    public Color ambientLight;

    private void Awake() {
        UpdateShader();
    }

    private void OnValidate() {
        UpdateShader();
    }

    void UpdateShader() {
        Shader.SetGlobalColor("_AmbientColor", ambientLight);
    }
}
