using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PegScript : MonoBehaviour {

	//booleans to register the state of the position on the board
	public bool hasPeg;
	public bool isSelected;

	//switch between selected and non selected sprites
	public Sprite peg;
	public Sprite pegSelected; 
	public Sprite boardSelected;
	private SpriteRenderer spriteRenderer;

	// Use this for initialization
	void Start () {
		//all the places start with a peg except for row0_0
		if (gameObject.name.Equals ("Row0_0")) {
			hasPeg = false;
		} else {
			hasPeg = true;
		}
		//none of the pegs are selected at the beginning
		isSelected = false;

		//set up sprite rendering 
		spriteRenderer = GetComponent<SpriteRenderer> ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	//register mouse clicks 
	void OnMouseDown(){
		isSelected = true; 
	}

	//getters and setters for boolean vars 
	public void setIsSelected(bool b)
	{
		isSelected = b;
	}
	public bool getIsSelected()
	{	
		return isSelected;
	}
	public void setHasPeg(bool b)
	{
		hasPeg = b;
	}
	public bool getHasPeg()
	{
		return hasPeg;
	}

	//set the sprite to different things 
	public void setPeg()
	{
		spriteRenderer.sprite = peg;
	}
	public void setPegSelected()
	{
		spriteRenderer.sprite = pegSelected;
	}
	public void setEmpty()
	{
		spriteRenderer.sprite = null;
	}
	public void setBoardSelected()
	{
		spriteRenderer.sprite = boardSelected;
	}
}
