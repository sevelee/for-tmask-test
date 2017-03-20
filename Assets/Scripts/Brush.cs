using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brush : MonoBehaviour {
	//--------------------------------------------------

	#region Public Variables

	public Texture2D brushTexture;
	public Texture2D originalMaskTexture;

	#endregion

	//--------------------------------------------------

	#region Private Variables

	Texture2D maskTexture;

	Material maskMaterial;

	#endregion

	//--------------------------------------------------

	#region Unity Functions

	void Awake()
	{
		maskTexture = new Texture2D (originalMaskTexture.width, originalMaskTexture.height);
		Graphics.CopyTexture (originalMaskTexture, maskTexture);
		maskMaterial = gameObject.GetComponent<Renderer> ().material;
	}

	void Start () {
		DebugTextureInfo ();
	}

	void Update () 
	{
		MouseBrush ();
		UpdateTextureToMaterial ();
	}

	#endregion

	//--------------------------------------------------

	void DebugRandomBrush()
	{
		if (Input.GetMouseButtonDown(0))
		{
			int x, y;
			x = (int)(Random.value * maskTexture.width);
			y = (int)(Random.value * maskTexture.height);
			Debug.Log ("position is " + x + " " + y);
			AddBrushToMask (x, y);		
			UpdateTextureToMaterial ();
		}
	}

	void UpdateTextureToMaterial()
	{
		maskMaterial.SetTexture ("_MaskTex", maskTexture);
	}

	void MouseBrush()
	{
		Vector3 mousePosition = Input.mousePosition;
		Ray ray = Camera.main.ScreenPointToRay (mousePosition);
		RaycastHit hitInfo;
		Vector2 texturePosition;
		if (Physics.Raycast(ray, out hitInfo))
		{
			Debug.DrawLine (Camera.main.transform.position, hitInfo.point);
			texturePosition = hitInfo.textureCoord;
		}
		if (Input.GetMouseButton(0))
		{
			int startX, startY;
			startX = startY = 0;
			startX = (int)(texturePosition.x * maskTexture.width);
			startY = (int)(texturePosition.y * maskTexture.height);
			Debug.Log ("The Texture Position is " + startX + " , " + startY);
			AddBrushToMask (startX, startY);
		}
	}

	void AddBrushToMask(int startx, int starty)
	{
		Color brushColor, maskColor, finalColor;
		for (int i = startx; i < startx + brushTexture.width; ++i)
		{
			for (int j = starty; j < starty + brushTexture.height; ++j)
			{
				if (InsideMaskTexture(i, j))
				{
					brushColor = brushTexture.GetPixel (i - startx, j - starty);
					maskColor = maskTexture.GetPixel (i, j);
					finalColor = addBrushColor (brushColor, maskColor);
					Color debugColor = Color.red;
					maskTexture.SetPixel (i, j, finalColor);
				}
			}
		}
		maskTexture.Apply ();
	}
		
	/// <summary>
	/// Is the positon x,y inside MaskTexture or not.
	/// </summary>
	/// <returns><c>true</c>, if mask texture was insided, <c>false</c> otherwise.</returns>
	/// <param name="x">The x coordinate.</param>
	/// <param name="y">The y coordinate.</param>
	bool InsideMaskTexture(int x, int y)
	{
		return (x < maskTexture.width) && (y < maskTexture.height);
	}

	/// <summary>
	/// Adds the color of the brush.
	/// </summary>
	/// <returns>The brush color.</returns>
	/// <param name="brushColor">Brush color.</param>
	/// <param name="maskColor">Mask color.</param>
	Color addBrushColor(Color brushColor, Color maskColor)
	{
		Color finalColor;
		finalColor = maskColor;

		finalColor = brushColor + maskColor;

		return finalColor;
	}

	/// <summary>
	/// Debugs the texture info.
	/// </summary>
	void DebugTextureInfo()
	{
		Debug.Log ("The size of brush is " + brushTexture.width + " x " + brushTexture.height);
		Debug.Log ("The size of mask is " + originalMaskTexture.width + " x " + originalMaskTexture.height);
	}
}
