﻿using UnityEngine;
using System.Collections;
using System.Net.NetworkInformation;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;
using System;
//Created by Stephen on 13-8-14
public class Network : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}


	//Use port 33 for connection - Stephen
	public void connect()
	{

	}
}
/*
//TEST
static class Module1
{
	private static List<System.Net.NetworkInformation.Ping> pingers = new List<System.Net.NetworkInformation.Ping>();
	private static int instances = 0;
	
	private static object @lock = new object();
	
	private static int result = 0;
	private static int timeOut = 250;
	
	private static int ttl = 5;
	
	public static void Main()
	{
		string baseIP = "192.168.1.";
		
		Debug.Log("Pinging 255 destinations of D-class in {0}*", baseIP);
		
		CreatePingers(255);
		
		PingOptions po = new PingOptions(ttl, true);
		System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
		byte[] data = enc.GetBytes("abababababababababababababababab");
		
		SpinWait wait = new SpinWait();
		int cnt = 1;
		
		Stopwatch watch = Stopwatch.StartNew();
		
		foreach (Ping p in pingers) {
			lock (@lock) {
				instances += 1;
			}
			
			p.SendAsync(string.Concat(baseIP, cnt.ToString()), timeOut, data, po);
			cnt += 1;
		}
		
		while (instances > 0) {
			wait.SpinOnce();
		}
		
		watch.Stop();
		
		DestroyPingers();
		
		Debug.Log("Finished in {0}. Found {1} active IP-addresses.", watch.Elapsed.ToString(), result);
		Console.ReadKey();
		
	}
	
	public static void Ping_completed(object s, PingCompletedEventArgs e)
	{
		lock (@lock) {
			instances -= 1;
		}
		
		if (e.Reply.Status == IPStatus.Success) {
			Debug.Log(string.Concat("Active IP: ", e.Reply.Address.ToString()));
			result += 1;
		} else {
			//Debug.Log(String.Concat("Non-active IP: ", e.Reply.Address.ToString()))
		}
	}
	
	
	private static void CreatePingers(int cnt)
	{
		for (int i = 1; i <= cnt; i++) {
			Ping p = new Ping();
			p.PingCompleted += Ping_completed;
			pingers.Add(p);
		}
	}
	
	private static void DestroyPingers()
	{
		foreach (Ping p in pingers) {
			p.PingCompleted -= Ping_completed;
			p.Dispose();
		}
		
		pingers.Clear();
		
	}
	
}*/