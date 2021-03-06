﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using AssemblyCSharp;
using System.Collections.Generic;
using System;
using LitJson;


public class CrateRoomView : MonoBehaviour {
	
	public GameObject panelZhuanzhuanSetting;
	public GameObject panelJiPingHuSetting;
	public GameObject panelHuashuiSetting;
	public GameObject panelDevoloping;

	public List<Toggle> zhuanzhuanRoomCards;//转转麻将房卡数
	public List<Toggle> jiPingHuRoomCards;//鸡平胡房卡数
	public List<Toggle> huashuiRoomCards;//划水麻将房卡数

	public List<Toggle> zhuanzhuanGameRule;//转转麻将玩法
	public List<Toggle> jiPingHuGameRule;//鸡平胡玩法
	public List<Toggle> huashuiGameRule;//划水麻将玩法

	public List<Toggle> zhuanzhuanZhuama;//转转麻将抓码个数
	public List<Toggle> jiPingHuZhuama;//鸡平胡将抓码个数
	public List<Toggle> huashuixiayu;//划水麻将下鱼条数


	private int roomCardCount;//房卡数
	private GameObject gameSence;
	private RoomVO roomVO;//创建房间的信息


	public GameObject giPingHuBt;
	public GameObject zzBt;
	public GameObject huaShuiBt;
	public GameObject closeBt;
	void Start () {
		initUI ();

		SocketEventHandle.getInstance ().CreateRoomCallBack += onCreateRoomCallback;


	}
	void initUI(){
		giPingHuBt.GetComponent<Button> ().onClick.AddListener (onClickBtn_GiPingHu);
		zzBt.GetComponent<Button> ().onClick.AddListener (onClickBtn_ZhuanZhuan);
		huaShuiBt.GetComponent<Button> ().onClick.AddListener (onClickBtn_Hua_Shui);
		closeBt.GetComponent<Button> ().onClick.AddListener (onClickBtn_Close);
		switchSetting (GameType.GI_PING_HU);
	}

	private void onClickBtn_GiPingHu(){
		switchSetting (GameType.GI_PING_HU);
	}
	private void onClickBtn_ZhuanZhuan(){
		switchSetting (GameType.ZHUAN_ZHUAN);
	}

	private void onClickBtn_Hua_Shui(){
		switchSetting (GameType.HUA_SHUI);
	}
	private void switchSetting(GameType type){
		panelZhuanzhuanSetting.SetActive (false);
		panelJiPingHuSetting.SetActive (false);
		panelHuashuiSetting.SetActive (false);
		panelDevoloping.SetActive (false);
		switch (type) {
		case GameType.GI_PING_HU:
			panelJiPingHuSetting.SetActive (true);
			break;
		case GameType.ZHUAN_ZHUAN:
			panelZhuanzhuanSetting.SetActive (true);
			break;
		case GameType.HUA_SHUI:
			panelHuashuiSetting.SetActive (true);
			break;
		default:
			panelDevoloping.SetActive (true);
			break;
		}

	}

	private void onClickBtn_Close(){
		SocketEventHandle.getInstance ().CreateRoomCallBack -= onCreateRoomCallback;
		Destroy (this);
		Destroy (gameObject);
	}

	/**
	 * 创建鸡平胡房间
	 */
	public void createJiPingHuRoom(){

		int roomCardNum = costRoomCardNum (huashuiRoomCards);

		//抓码
		int maCount = 0;
		for (int i = 0; i <jiPingHuZhuama.Count; i++) {
			if (jiPingHuZhuama [i].isOn) {
				maCount = (int)Math.Pow(2,i);
				break;
			}
		}

		roomVO = new RoomVO ();
		roomVO.roomType = GameType.GI_PING_HU;

		roomVO.magnification = maCount;

		createRoom (roomVO,roomCardNum);

	}

	/**
	 * 创建转转麻将房间
	 */ 
	public void createZhuanzhuanRoom(){
		
		int roomCardNum = costRoomCardNum (huashuiRoomCards);

		bool isZimo=false;//自摸
		if (zhuanzhuanGameRule [0].isOn) {
			isZimo = true;
		}
		
		bool hasHong=false;//红中赖子
		if (zhuanzhuanGameRule [2].isOn) {
			hasHong = true;
		}

		bool isSevenDoube =false;//七小对
		if (zhuanzhuanGameRule [3].isOn) {
			isSevenDoube = true;
		}

		
		int maCount = 0;
		for (int i = 0; i < zhuanzhuanZhuama.Count; i++) {
			if (zhuanzhuanZhuama [i].isOn) {
				maCount = 2 * (i + 1);
				break;
			}
		}
		roomVO = new RoomVO ();
		roomVO.roomType = GameType.ZHUAN_ZHUAN;

		roomVO.ma = maCount;
		roomVO.ziMo = isZimo?1:0;
		roomVO.hong = hasHong;
		roomVO.sevenDouble = isSevenDoube;

		createRoom (roomVO,roomCardNum);

	}

	/**
	 * 创建划水麻将房间
	 */
	public void createHuashuiRoom(){
		int roomCardNum = costRoomCardNum (huashuiRoomCards);

		bool isFengpai =false;//七小对
		if (huashuiGameRule [0].isOn) {
			isFengpai = true;
		}

		bool isZimo=false;//自摸
		if (huashuiGameRule [1].isOn) {
			isZimo = true;
		}
	

		int maCount = 0;
		for (int i = 0; i <huashuixiayu.Count; i++) {
			if (huashuixiayu [i].isOn) {
				maCount = 2 * (i + 1);
				break;
			}
		}

		roomVO = new RoomVO ();
		roomVO.roomType = GameType.HUA_SHUI;

		roomVO.xiaYu = maCount;
		roomVO.ziMo = isZimo?1:0;
		roomVO.addWordCard = isFengpai;
		roomVO.sevenDouble = true;

		createRoom (roomVO,roomCardNum);
	}
	private int costRoomCardNum(List<Toggle> list){
		for (int i = 0; i < list.Count; i++) {
			Toggle item = list [i];
			if (item.isOn) {
				return i + 1;
			}
		}
		return 1;
	}
	private void createRoom(RoomVO roomVO,int roomCardNum){
		if (GlobalData.myAvatarVO.account.roomcard >= roomCardNum) {
			roomVO.roundNumber = roomCardNum * 8;
			string sendmsgstr = JsonMapper.ToJson (roomVO);
			CustomSocket.getInstance ().sendMsg (new CreateRoomRequest (sendmsgstr));
		} else {
			TipsManager.getInstance ().setTips ("你的房卡数量不足，不能创建房间");
		}
	}
	public void onCreateRoomCallback(ClientResponse response){
		MyDebug.Log (response.message);
		if (response.status == 1) {
			
			int roomid = Int32.Parse(response.message);
			roomVO.roomId = roomid;
			GlobalData.roomVO = roomVO;
			GlobalData.myAvatarVO.roomId = roomid;
			GlobalData.myAvatarVO.main = true;
			GlobalData.myAvatarVO.isOnLine = true;
			SceneManager.getInstance ().changeToScene (SceneType.GAME);

			SceneManager.getInstance().CurScenePanel.GetComponent<MyMahjongScript> ().createRoomAddAvatarVO (GlobalData.myAvatarVO);
		
			onClickBtn_Close ();

		} else {
			TipsManager.getInstance ().setTips (response.message);
		}
	}

}
