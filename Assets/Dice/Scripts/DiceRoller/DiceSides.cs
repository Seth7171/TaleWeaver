using UnityEngine;
using System.Collections.Generic;

public struct FaceRotation
{
    public int FaceValue;
    public Quaternion Rotation;

    public FaceRotation(int faceValue, Quaternion rotation)
    {
        FaceValue = faceValue;
        Rotation = rotation;
    }
}

[System.Serializable]
public class DiceSide {
    public Vector3 Center;
    public Vector3 Normal;
    public int Value;
}

public class DiceSides : MonoBehaviour {
    [SerializeField] public DiceSide[] Sides;
    public Dictionary<int, Quaternion> faceRotations;

    const float k_exactMatchValue = 0.995f;

    void Awake()
    {
        faceRotations = new Dictionary<int, Quaternion>
        {
            { 1, Quaternion.Euler(-110.924f, 717.625f, -87.45801f) },    
            { 2, Quaternion.Euler(-185.586f, 485.458f, 408.966f) },     
            { 3, Quaternion.Euler(-169.876f, 738.387f, 297.869f) },
            { 4, Quaternion.Euler(-190.259f, 1278.314f, 421.717f) },
            { 5, Quaternion.Euler(-187.32f, 1314.402f, 309.789f) },
            { 6, Quaternion.Euler(-58.875f, 1530, 290.905f) },
            { 7, Quaternion.Euler(-5.964f, 1565.485f, 490.764f) },
            { 8, Quaternion.Euler(-69.042f, 1616.015f, 634.266f) },
            { 9, Quaternion.Euler(119.729f, 1530, 609.095f) },
            { 10, Quaternion.Euler(186.684f, 1494.458f, 769.754f) },
            { 11, Quaternion.Euler(173.686f, 1565.512f, 949.488f) },
            { 12, Quaternion.Euler(-61.192f, 1710, 969.095f) },
            { 13, Quaternion.Euler(69.055f, 1436.559f, 446.316f) },
            { 14, Quaternion.Euler(7.035f, 1494.428f, 309.994f) },
            { 15, Quaternion.Euler(59.456f, 1170, 110.905f) },
            { 16, Quaternion.Euler(-5.648f, 845.462f, -49.01f) },
            { 17, Quaternion.Euler(-10.177f, 521.642f, 61.967f) },
            { 18, Quaternion.Euler(10.965f, 342.015f, -419.937f) },
            { 19, Quaternion.Euler(-367.282f, 594.405f, 50.184f) },
            { 20, Quaternion.Euler(69.089f, 1.342f, -448.563f) }
        };
    }


    public DiceSide GetDiceSide(int index) => Sides[index];

    public Quaternion GetWorldRotationFor(int index) {
        Vector3 worldNormalToMatch = transform.TransformDirection(GetDiceSide(index).Normal);
        return Quaternion.FromToRotation(worldNormalToMatch, Vector3.up) * transform.rotation;
    }

    public int GetMatch() {
        int sideCount = Sides.Length;
        
        Vector3 localVectorToMatch = transform.InverseTransformDirection(Vector3.forward);

        DiceSide closestSide = null;
        float closestDot = -1f;

        for (int i = 0; i < sideCount; i++) {
            DiceSide side = Sides[i];
            float dot = Vector3.Dot(side.Normal, localVectorToMatch);
            
            if (closestSide == null || dot > closestDot) {
                closestSide = side;
                closestDot = dot;
            }
            
            if (dot > k_exactMatchValue) {
                return side.Value;
            }
        }
        
        return closestSide?.Value ?? -1;
    }
}