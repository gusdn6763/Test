using UnityEngine;

namespace Febucci.UI.Core
{
    public struct CharacterData
    {
        public CharInfo info;

        public int index;
        public int wordIndex;
        public bool isVisible;
        public float passedTime;
        
        public float uniformIntensity;

        public MeshData source;
        public MeshData current;

        public void ResetInfo(int i, bool resetVisibility = true)
        {

            index = i;
            wordIndex = -1;
            
            if(resetVisibility) isVisible = true; //text is visible by default

            //--Initializes first time only--
            if (!info.initialized)
            {
                source.positions = new Vector3[Core.TextUtilities.verticesPerChar];
                source.colors = new Color32[Core.TextUtilities.verticesPerChar];

                current.positions = new Vector3[Core.TextUtilities.verticesPerChar];
                current.colors = new Color32[Core.TextUtilities.verticesPerChar];
                info.initialized = true;
            }

        }

        public void ResetAnimation()
        {
            for (int i = 0; i < source.positions.Length; i++)
            {
                current.positions[i] = source.positions[i];
                current.colors[i] = source.colors[i];
            }
        }

        public void Hide()
        {
            for (byte i = 0; i < source.positions.Length; i++)
            {
                current.positions[i] = Vector3.zero;
            }
        }

        public void UpdateIntensity(float referenceFontSize)
        {
            uniformIntensity = info.pointSize / referenceFontSize;
        }
    }

    public struct MyCharacterData
    {
        public MyCharInfo info;

        public int index;
        public int wordIndex;
        public bool isVisible;
        public float passedTime;
        public float appearancesMaxDuration;
        public float disappearancesMaxDuration;
        public float behaviorMaxDuration;

        public float uniformIntensity;

        public MyMeshData source;
        public MyMeshData current;
        public MyMeshData currentBefore;

        public void ResetInfo(int i, bool resetVisibility = true)
        {

            index = i;
            wordIndex = -1;

            if (resetVisibility) isVisible = true; //text is visible by default

            //--Initializes first time only--
            if (!info.initialized)
            {
                currentBefore.positions = new Vector3[Core.TextUtilities.verticesPerChar];
                currentBefore.colors = new Color32[Core.TextUtilities.verticesPerChar];

                source.positions = new Vector3[Core.TextUtilities.verticesPerChar];
                source.colors = new Color32[Core.TextUtilities.verticesPerChar];

                current.positions = new Vector3[Core.TextUtilities.verticesPerChar];
                current.colors = new Color32[Core.TextUtilities.verticesPerChar];

                info.initialized = true;
            }

        }

        public void ResetAnimation()
        {
            for (int i = 0; i < source.positions.Length; i++)
            {
                current.positions[i] = source.positions[i];
                current.colors[i] = source.colors[i];
                current.scale = source.scale;
            }
        }

        public void Hide()
        {
            for (byte i = 0; i < source.positions.Length; i++)
            {
                current.positions[i] = Vector3.zero;
            }
        }

        public bool IsHiding()
        {
            for (byte i = 0; i < source.positions.Length; i++)
            {
                if (current.positions[i] != Vector3.zero)
                    return false;
            }
            return true;
        }

        public void UpdateIntensity(float referenceFontSize)
        {
            uniformIntensity = info.pointSize / referenceFontSize;
        }

        public void SaveBeforePositions()
        {
            for (int i = 0; i < source.positions.Length; i++)
                currentBefore.positions[i] = new Vector3(current.positions[i].x, current.positions[i].y, current.positions[i].z);
        }
    }
}