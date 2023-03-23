using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public static class _ClassExtensions
{

	#region GameObject Extensions

	/// <summary>
	/// Determines if gameObject is Null.
	/// </summary>
	/// <returns><c>true</c> if is null the specified g; otherwise, <c>false</c>.</returns>
	/// <param name="g">The green component.</param>
	public static bool IsNull (this GameObject g)
	{
		return object.ReferenceEquals (g, null);
	}


    public static bool CompareName(this GameObject g, string name)
    {
        return g.name == name;
    }

	/// <summary>
	/// Sets the parent of the game object.
	/// </summary>
	/// <param name="g">The green component.</param>
	/// <param name="parent">Parent.</param>
	public static void SetParent (this GameObject g, GameObject parent)
	{
		g.transform.parent = parent.transform;
	}


	/// <summary>
	/// Destroies the children of the game object.
	/// </summary>
	/// <param name="parent">Parent.</param>
	public static void DestroyChildren (this GameObject parent)
	{
		Transform[] children = new Transform[parent.transform.childCount];
		for (int i = 0; i < parent.transform.childCount; i++)
			children [i] = parent.transform.GetChild (i);
		for (int i = 0; i < children.Length; i++)
			GameObject.Destroy (children [i].gameObject);
	}


	/// <summary>
	/// Moves the children from the game object to another game object.
	/// </summary>
	/// <param name="from">From.</param>
	/// <param name="to">To.</param>
	public static void MoveChildren (this GameObject from, GameObject to)
	{
		Transform[] children = new Transform[from.transform.childCount];
		for (int i = 0; i < from.transform.childCount; i++)
			children [i] = from.transform.GetChild (i);
		for (int i = 0; i < children.Length; i++)
			children [i].gameObject.SetParent (to);
	}

	/// <summary>
	/// Gets the layer collision mask of a gameObject.
	/// </summary>
	/// <returns>The collision mask.</returns>
	/// <param name="gameObject">Game object.</param>
	/// <param name="layer">Layer.</param>
	public static int GetLayerCollisionMask (this GameObject gameObject, int layer = -1)
	{
		if (layer == -1)
			layer = gameObject.layer;

		int mask = 0;
		for (int i = 0; i < 32; i++)
			mask |= (Physics.GetIgnoreLayerCollision (layer, i) ? 0 : 1) << i;

		return mask;
	}


	#endregion GameObject Extensions


	#region Component Extensions

	public static bool IsNull (this Component c)
	{
		return object.ReferenceEquals (c, null);
	}

	#endregion Component Extensions


	#region Transform Extensions

	/// <summary>
	/// Resets the transform.
	/// </summary>
	/// <param name="t">T.</param>
	public static void ResetTransform (this Transform t)
	{
		t.position = Vector3.zero;
		t.localRotation = Quaternion.identity;
		t.localScale = new Vector3 (1, 1, 1);
	}

	/// <summary>
	/// Sets the X of transform position.
	/// </summary>
	/// <param name="t">T.</param>
	/// <param name="xValue">X value.</param>
	public static void SetPositionX (this Transform t, float xValue)
	{
		t.position = new Vector3 (xValue, t.position.y, t.position.z);
	}


	/// <summary>
	/// Sets the Y of transform position.
	/// </summary>
	/// <param name="t">T.</param>
	/// <param name="yValue">Y value.</param>
	public static void SetPositionY (this Transform t, float yValue)
	{
		t.position = new Vector3 (t.position.x, yValue, t.position.z);
	}


	/// <summary>
	/// Sets the Z of transform position.
	/// </summary>
	/// <param name="t">T.</param>
	/// <param name="zValue">Z value.</param>
	public static void SetPositionZ (this Transform t, float zValue)
	{
		t.position = new Vector3 (t.position.x, t.position.y, zValue);
	}

	#endregion Transform Extensions

	#region Vector Extensions

	/// <summary>
	/// Calculates the nearest point on line.
	/// </summary>
	/// <returns>The point on line.</returns>
	/// <param name="lineDirection">unit vector in direction of line.</param>
	/// <param name="point">a point on the line (allowing us to define an actual line in space.</param>
	/// <param name="pointOnLine">the point to find nearest on line for.</param>
	/// <param name="isNormalized">If set to <c>true</c> is normalized.</param>
	public static Vector3 NearestPointOnLine (this Vector3 lineDirection, Vector3 point, Vector3 pointOnLine, bool isNormalized = false)
	{
		if (!isNormalized) lineDirection.Normalize ();
		var d = Vector3.Dot (point - pointOnLine, lineDirection);
		return pointOnLine + (lineDirection * d);
	}


	#endregion Vector Extensions

	#region UI Extensions

	public static void SetTransparency(this UnityEngine.UI.Image p_image, float p_transparency)
	{
		if (p_image != null)
		{
			UnityEngine.Color __alpha = p_image.color;
			__alpha.a = p_transparency;
			p_image.color = __alpha;
		}
	}

	#endregion UI Extensions
}
