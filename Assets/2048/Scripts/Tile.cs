using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
public class Tile : MonoBehaviour {

	public bool mergeThisReturn = false;
	public int indRow;
	public int indCol;

	public int Number{
		get{
			return number;
		}
		set{
			number = value;
			if (number == 0) {
				SetEmpty ();
			}
			else{
				ApplyStyle (number);
				SetVisible ();
			}
		}
	}

	private Animator anim;

	private int number;
	private Text TileText;
	private Image ImageText;

	void Awake(){
		anim = GetComponent<Animator> ();
		TileText = GetComponentInChildren<Text> ();
		ImageText = transform.Find("NumberedCell").GetComponent<Image> ();
	}


	public void PlayMergedAnimation(){
		anim.SetTrigger ("Merge");
	}

	public void PlayAppearAnimation(){
		anim.SetTrigger ("Appear");
	}

	void ApplyStyleFromHolder(int index){
		TileText.text = TileStyleHolder.Instance.TitleStyles [index].Number.ToString();
		TileText.color = TileStyleHolder.Instance.TitleStyles [index].TextColor;
		ImageText.color = TileStyleHolder.Instance.TitleStyles [index].TileColor;
	}

	void ApplyStyle(int num){
		List<int> listOfMatchs = new List<int>(new int[]{ 2, 4, 8, 16, 32, 64, 128, 256, 512, 1024, 2048, 4096, 8192, 16384, 32768, 65536 });
		int index = listOfMatchs.IndexOf (num);
		ApplyStyleFromHolder (index);
	}

	private void SetVisible(){
		ImageText.enabled = true;
		TileText.enabled = true;
	}


	private void SetEmpty(){
		ImageText.enabled = false;
		TileText.enabled = false;
	}

}
