using UnityEngine;
using UnityEditor;
using UnityEngine.TextCore;
using TMPro;
using System;
using System.Reflection;

namespace Fnt2TMPro.EditorUtilities
{
    public class Fnt2TMPro : EditorWindow
    {
        [MenuItem("Window/Bitmap Font Converter")]

        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(Fnt2TMPro), false, "Bitmap Font Converter");
        }
        private Texture2D m_Texture2D;
        private TextAsset m_SourceFontFile;
        private TMP_FontAsset m_DestinationFontFile;
        // Removed unused PatchGlyph method
        void UpdateFont(TMP_FontAsset fontFile)
        {
            if (m_Texture2D == null)
            {
                Debug.LogError("Font Texture is not assigned.");
                return;
            }
            if (m_SourceFontFile == null)
            {
                Debug.LogError("Source Font File (.fnt) is not assigned.");
                return;
            }
            if (fontFile == null)
            {
                Debug.LogError("Destination TMP_FontAsset is not assigned.");
                return;
            }

            var fontText = m_SourceFontFile.text;
            var fnt = FntParse.GetFntParse(ref fontText);
            if (fnt == null)
            {
                Debug.LogError("Failed to parse source .fnt file. Unsupported format?");
                return;
            }

            //fontFile.faceInfo = new() { lineHeight = fnt.lineHeight, ascentLine = fnt.lineHeight, scale = 1 };

            var shader = AssetDatabase.LoadAssetAtPath<Shader>("Assets/Plugins/TextMesh Pro/Shaders/TMP_Bitmap-Custom-Atlas.shader");
            if (shader == null)
            {
                Debug.LogWarning("Custom TMP bitmap shader not found. Using default TMP shader.");
                shader = Shader.Find("TextMeshPro/Bitmap Custom Atlas");
            }
            fontFile.material.shader = shader;
            fontFile.material.SetTexture("_MainTex", m_Texture2D);
            fontFile.atlasTextures[0] = m_Texture2D;
            fontFile.atlas = m_Texture2D;
            SetAtlasStat(fontFile, m_Texture2D.width, "m_AtlasWidth");
            SetAtlasStat(fontFile, m_Texture2D.height, "m_AtlasHeight");


            fontFile.glyphTable.Clear();
            fontFile.characterTable.Clear();

            foreach (var glyph in fnt.rawCharInfos)
            {
                Glyph TMPGlyph = new();
                var glyphWidth = glyph.Width;
                var glyphHeight = glyph.Height;
                TMPGlyph.glyphRect = new()
                {
                    x = glyph.X,
                    y = m_Texture2D.height - glyph.Y - glyph.Height,
                    width = glyphWidth,
                    height = glyphHeight
                };
                TMPGlyph.metrics = new()
                {
                    width = glyphWidth,
                    height = glyphHeight,
                    horizontalBearingY = -glyph.Yoffset,
                    horizontalBearingX = glyph.Xoffset,
                    horizontalAdvance = glyph.Xadvance,
                };
                TMPGlyph.index = Convert.ToUInt32(glyph.ID);
                fontFile.glyphTable.Add(TMPGlyph);
                TMP_Character TMPChar = new();
                TMPChar.glyphIndex = Convert.ToUInt32(glyph.ID);
                TMPChar.unicode = Convert.ToUInt32(glyph.ID);
                fontFile.characterTable.Add(TMPChar);
            }

            var newFaceInfo = fontFile.faceInfo;
            newFaceInfo.baseline = fnt.lineBaseHeight;
            newFaceInfo.lineHeight = fnt.lineHeight;
            newFaceInfo.ascentLine = fnt.lineHeight;
            newFaceInfo.pointSize = fnt.fontSize;
            fontFile.faceInfo = newFaceInfo;

            //var fontType = typeof(TMP_FontAsset);
            //var faceInfoProperty = fontType.GetProperty("faceInfo");
            //faceInfoProperty.SetValue(fontFile, newFaceInfo);

            EditorUtility.SetDirty(fontFile);
        }
        private static void SetAtlasStat(UnityEngine.Object targetObject, int newValue, string name)
        {
            if (targetObject == null)
            {
                Debug.LogError($"Target object is null when setting {name}.");
                return;
            }
            var type = targetObject.GetType();
            var field = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Instance);
            if (field == null)
            {
                Debug.LogError($"Field {name} not found on {type.Name}.");
                return;
            }
            field.SetValue(targetObject, newValue);
        }

        void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            m_Texture2D = EditorGUILayout.ObjectField("Font Texture",
                m_Texture2D, typeof(Texture2D), false) as Texture2D;
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            m_SourceFontFile = EditorGUILayout.ObjectField("Source Font File",
                m_SourceFontFile, typeof(TextAsset), false) as TextAsset;
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            m_DestinationFontFile = EditorGUILayout.ObjectField("Destination Font File",
                m_DestinationFontFile, typeof(TMP_FontAsset), false) as TMP_FontAsset;
            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("Convert"))
            {
                UpdateFont(m_DestinationFontFile);
            }
        }
    }
}