using UnityEngine;

namespace DialogSystem.Editor.Errors
{
    public class ErrorData
    {
        public Color Color { get; set; }

        public ErrorData()
        {
            GenerateRandomColor();
        }

        private void GenerateRandomColor()
        {
            Color = new Color32(
                (byte)Random.Range(65, 256),
                (byte)Random.Range(50, 176),
                (byte)Random.Range(50, 176),
                255
            );
        }
    } 
}