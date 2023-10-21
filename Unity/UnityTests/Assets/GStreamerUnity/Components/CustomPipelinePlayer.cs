using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

[RequireComponent(typeof(GstCustomTexture))]
public class CustomPipelinePlayer : BaseVideoPlayer {

	public string pipeline = "";
	
	public long Position;
	public long Duration;
	
	
	// Use this for initialization
	protected override string _GetPipeline()
	{
		string P = pipeline + " ! video/x-raw,format=I420 ! videoconvert ! appsink name=videoSink";
		//string P = pipeline + " video-sink=\"videoconvert ! video/x-raw,format=I420 ! appsink name=videoSink\""; 
		
		return P;
	}
	
	protected override void Update()
	{
		base.Update();
		Position = InternalTexture.Player.GetPosition() / 1000;
		Duration = InternalTexture.Player.GetDuration() / 1000;

		if (Input.GetKeyDown(KeyCode.LeftArrow))
		{

			var p = (Position - 10);
			if (p < 0)
				p = 0;
				
			print("yo seek p="+p);
			
			InternalTexture.Pause();
			InternalTexture.Player.Seek(p * 1000);
		}
		if (Input.GetKeyDown(KeyCode.RightArrow))
		{
			var p = (Position + 10);
			if (p >= Duration)
				p = Duration;
						
			InternalTexture.Pause();			
			InternalTexture.Player.Seek(p * 1000);
		}
		if (Input.GetKeyDown(KeyCode.S))
		{
			InternalTexture.Pause();
			
		}
		
		if (Input.GetKeyDown(KeyCode.P))
		{
			InternalTexture.Play();
					
		}
		//if (Input.GetKeyDown(KeyCode.L))
		//	Loop=!Loop;
	}
}
