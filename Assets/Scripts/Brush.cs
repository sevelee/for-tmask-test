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
		maskTexture.Apply ();
		UpdateTextureToMaterial ();
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
		Color[] finalColors, brushColors, maskColors;

		int bWidth, bHeight;
		bWidth = brushTexture.width;
		bHeight = brushTexture.height;

		int startPositionX, startPositionY;
		startPositionX = startx - bWidth / 2;
		startPositionY = starty - bHeight / 2;

		startPositionX = (startPositionX < 0) ? 0 : startPositionX;
		startPositionY = (startPositionY < 0) ? 0 : startPositionY;

		//waiting for further developement
		int brushStartX, brushStartY;

		bWidth = (bWidth + startPositionX > maskTexture.width) ? (maskTexture.width - startPositionX) : bWidth;
		bHeight = (bHeight + startPositionY > maskTexture.height) ? (maskTexture.height - startPositionY) : bHeight;

		finalColors = new Color[bWidth * bHeight];
		brushColors = new Color[finalColors.Length];
		maskColors = new Color[finalColors.Length];

		brushColors = brushTexture.GetPixels (0, 0, bWidth, bHeight);
		maskColors = maskTexture.GetPixels (startPositionX, startPositionY, bWidth, bHeight);

		int finalColorIndex = 0;

		for (int i = 0; i < brushColors.Length; ++i)
		{
			finalColors [i] = addBrushColor (brushColors [i], maskColors [i]);
		}
			
		maskTexture.SetPixels (startPositionX, startPositionY, bWidth, bHeight, finalColors);
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
