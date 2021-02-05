using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

public class ToonShaderGUI : ShaderGUI {
    
    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties) {
        MaterialProperty tint = FindProperty("_Tint", properties);
        MaterialProperty tex = FindProperty("_MainTex", properties);
        MaterialProperty normal = FindProperty("_NormalMap", properties);
        MaterialProperty normalInt = FindProperty("_NormalIntensity", properties);
        MaterialProperty ambient = FindProperty("_AmbientColorOverride", properties);
        MaterialProperty shadowStart = FindProperty("_ShadowStart", properties);
        MaterialProperty shadowEnd = FindProperty("_ShadowEnd", properties);
        MaterialProperty gloss = FindProperty("_Glossiness", properties);
        MaterialProperty specIntensity = FindProperty("_SpecularIntensity", properties);
        MaterialProperty specStart = FindProperty("_SpecularStart", properties);
        MaterialProperty specEnd = FindProperty("_SpecularEnd", properties);
        MaterialProperty rimStart = FindProperty("_RimStart", properties);
        MaterialProperty rimEnd = FindProperty("_RimEnd", properties);
        MaterialProperty rimIntensity = FindProperty("_RimIntensity", properties);
        MaterialProperty rimSensitivity = FindProperty("_RimSensitivity", properties);
        MaterialProperty outlineWidth = FindProperty("_OutlineWidth", properties);
        MaterialProperty bakedNormalsFloat = FindProperty("_BakedNormals", properties);
        MaterialProperty equalDistanceFloat = FindProperty("_EqualDistance", properties);
        MaterialProperty shadowIntensity = FindProperty("_ShadowIntensity", properties);
        MaterialProperty outlineColor = FindProperty("_OutlineColor", properties);

        bool ambientLightOverride = ambient.colorValue.a > 0;

        GUIStyle labelStyle = EditorStyles.boldLabel;
        GUIStyle helpBox = EditorStyles.helpBox;
        
        GUILayout.BeginVertical();
        
        GUILayout.Label("Albedo", labelStyle);
        materialEditor.ColorProperty(tint, "Tint");
        materialEditor.TextureProperty(tex, "Albedo Texture");
        GUILayout.Space(15);
        
        GUILayout.Label("Normals", labelStyle);
        materialEditor.RangeProperty(normalInt, "Normal Intensity");
        materialEditor.TextureProperty(normal, "Normal Map");
        GUILayout.Space(15);
        
        GUILayout.Label("Ambient Light", labelStyle);
        GUILayout.Box("_AmbientLight can be set globally through a script or other tool. Otherwise, the ambient lighting can be " +
                      "overridden by the property below (alpha not supported)", helpBox);
        
        Color globalAmbient = Shader.GetGlobalColor("_AmbientColor");
        GUILayout.BeginHorizontal();
        if (ambientLightOverride)
            GUI.enabled = false;
        globalAmbient = EditorGUILayout.ColorField("Global Ambient Light", globalAmbient);
        Shader.SetGlobalColor("_AmbientColor", globalAmbient);
        GUI.enabled = true;
        GUILayout.EndHorizontal();
        
        GUILayout.BeginHorizontal();
        GUILayout.Label("Override Global Ambient Light", EditorStyles.label);
        
        ambientLightOverride = GUILayout.Toggle(ambientLightOverride, "");
        GUILayout.EndHorizontal();
        
        if (ambientLightOverride) {
            materialEditor.ColorProperty(ambient, "Ambient Color");
            ambient.colorValue = new Color(ambient.colorValue.r, ambient.colorValue.g, ambient.colorValue.b, 1);
        }
        else {
            ambient.colorValue = new Color(ambient.colorValue.r, ambient.colorValue.g, ambient.colorValue.b, 0);
        }

        
        GUILayout.Space(15);
        
        GUILayout.Label("Diffuse Light and Shading", labelStyle);
        string shadowHelpBoxText = "_ShadowStart determines when the shadows starts fading\n" +
                                   "_ShadowEnd determines the end point for shadow fading";
        GUILayout.Box(shadowHelpBoxText, helpBox);
        materialEditor.RangeProperty(shadowStart, "Shadow Start Fade");
        materialEditor.RangeProperty(shadowEnd, "Shadow Stop Fade");
        materialEditor.RangeProperty(shadowIntensity, "Shadow Intensity");
        GUILayout.Space(15);
        
        GUILayout.Label("Specular Lighting", labelStyle);
        string specularHelpBoxText = "_Glossiness determines how large the specular highlight is.\n" +
                                   "_SpecularIntensity determines how bright the specular highlight is\n" +
                                   "_SpecularStart determines when the specular starts fading\n" +
                                   "_SpecularEnd determines the end point for specular fading";
        GUILayout.Box(specularHelpBoxText, helpBox);
        materialEditor.RangeProperty(gloss, "Gloss");
        materialEditor.RangeProperty(specIntensity, "Specular Intensity");
        materialEditor.RangeProperty(specStart, "Specular Start Fade");
        materialEditor.RangeProperty(specEnd, "Specular Stop Fade");
        GUILayout.Space(15);
        
        GUILayout.Label("Rim", labelStyle);
        string rimHelpBoxText = "Consider turning off the rim for objects that are not round.\n" +
                                     "_RimIntensity determines how bright the rim highlight is\n" +
                                     "_RimSensitivity determines how far along the edge the rim stretches\n" +
                                     "_RimStart determines when the rim starts fading\n" +
                                     "_RimEnd determines the end point for rim fading";
        GUILayout.Box(rimHelpBoxText, helpBox);
        materialEditor.RangeProperty(rimIntensity, "Rim Intensity");
        materialEditor.RangeProperty(rimSensitivity, "Rim Sensitivity");
        materialEditor.RangeProperty(rimStart, "Rim Start Fade");
        materialEditor.RangeProperty(rimEnd, "Rim Stop Fade");
        GUILayout.Space(15);
        
        GUILayout.Label("Outline", labelStyle);
        string outlineHelpBoxText = "Outlines work well on rounded objects but requires setup for hard edges.\n" +
                                    "For a proper outline on a hard edge, the smooth normals needs to be baked into the vertex colours" +
                                    "and the corresponding property to be enabled\n" +
                                    "This shader assumes the normals are baked in a right handed coordinate system (Autodesk Maya)";
        GUILayout.Box(outlineHelpBoxText, helpBox);

        materialEditor.RangeProperty(outlineWidth, "Outline Width");
        materialEditor.ColorProperty(outlineColor, "Outline Color");
        
        bool bakedNormals = bakedNormalsFloat.floatValue > 0.5f;
        if (EditorGUILayout.Toggle("Outline Baked Normals", bakedNormals)) {
            bakedNormalsFloat.floatValue = 1;
        }
        else {
            bakedNormalsFloat.floatValue = 0;
        }
        
        bool equalDistance = equalDistanceFloat.floatValue > 0.5f;
        if (EditorGUILayout.Toggle("Outline Ignore Distance", equalDistance)) {
            equalDistanceFloat.floatValue = 1;
        }
        else {
            equalDistanceFloat.floatValue = 0;
        }
        
        string equalDistanceHelpBoxText = "The above property makes it so that outlines always have the same width," +
                                          " regardless of distance from the camera. \nMay look bad on flat surfaces";
        GUILayout.Box(equalDistanceHelpBoxText, helpBox);
        GUILayout.EndVertical();
        
    }
}