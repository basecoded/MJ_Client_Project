﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class ActionEffectHelper : MonoBehaviour {

	public GameObject chiEffect;
	public GameObject pengEffect;
	public GameObject gangEffect;
	public GameObject huEffect;
	public GameObject liujuEffect;

	public GameObject huBtn;
	public GameObject gangBtn;
	public GameObject pengBtn;
	public GameObject chiBtn;
	public GameObject passBtn;

	public void addListener(MyMahjongScript host){
		huBtn.GetComponent<Button> ().onClick.AddListener (host.myHuBtnClick);
		gangBtn.GetComponent<Button> ().onClick.AddListener (host.myGangBtnClick);
		pengBtn.GetComponent<Button> ().onClick.AddListener (host.myPengBtnClick);
		chiBtn.GetComponent<Button> ().onClick.AddListener (host.myChiBtnClick);
		passBtn.GetComponent<Button> ().onClick.AddListener (host.myPassBtnClick);
	}


	/// <summary>
	/// Shows the button.
	/// </summary>
	/// <param name="type">Type.</param> 1-胡，2-杠，3-碰
	public void showBtn(ActionType type){
		passBtn.SetActive (true);
		if (type == ActionType.HU) {
			huBtn.SetActive (true);
		}
		else if (type == ActionType.GANG) {
			gangBtn.SetActive (true);
		}
		else if (type == ActionType.PENG) {
			pengBtn.SetActive (true);
		}
		else if (type == ActionType.CHI) {
			chiBtn.SetActive (true);
		}
	}

	public void cleanBtnShow(){
		huBtn.SetActive (false);
		gangBtn.SetActive (false);
		pengBtn.SetActive (false);
		chiBtn.SetActive (false);
		passBtn.SetActive (false);

	}

	public void pengGangHuEffectCtrl(ActionType type)
	{
		if (type == ActionType.PENG)
		{
			pengEffect.SetActive (true);
		}
		else if (type == ActionType.GANG)
		{
			gangEffect.SetActive (true);
		}
		else if (type == ActionType.HU)
		{
			huEffect.SetActive (true);
		}
		else if(type == ActionType.LIUJU){
			liujuEffect.SetActive (true);
		}
		else if (type == ActionType.CHI) {
			chiEffect.SetActive (true);
		}
		invokeHidePengGangHuEff();
	}

	private void invokeHidePengGangHuEff()
	{
		Invoke("HidePengGangHuEff", 1f);
	}

	private void HidePengGangHuEff()
	{
		pengEffect.SetActive(false);
		gangEffect.SetActive (false);
		huEffect.SetActive (false);
		chiEffect.SetActive (false);
	}
}
public enum ActionType{
	GANG,
	PENG,
	CHI,
	HU,
	LIUJU
}