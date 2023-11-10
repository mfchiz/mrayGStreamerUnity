using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GstCustomTexture : GstBaseTexture {


    protected static List<GstCustomTexture> _textureList = new List<GstCustomTexture>();

	public string Pipeline="";

	private GstCustomPlayer _player;

	private Coroutine _blitRoutine;

	int _countGrabs=0;

	public GstCustomPlayer Player
	{
		get	
		{
			return _player;
		}
		set
		{
			if (value != null)
				_player = value;
		}
	}

	public override int GetTextureCount ()
	{
		return 1;
	}
	/*public override int GetCaptureRate (int index)
	{
		return _player.GetCaptureRate (index);
	}*/

	public override IGstPlayer GetPlayer(){
		return _player;
	}

   	public GstCustomTexture()
    {
		_textureList.Add(this);
	}
	
	protected override void _initialize()
	{
		_player = new GstCustomPlayer ();
		_player.SetOnFrameReady(new GstCustomPlayer.OnFrameReady(OnFrameReady));
	}
	

	public void OnFrameReady()
	{
		if(!_imageGrabed) // only grab if it hasnt been grabbed yet for this frame
		{
			_countGrabs++;
			GrabFrame();
		}

	}

	public override void Destroy ()
	{
		
		if(_blitRoutine!=null)
		{
			StopCoroutine(_blitRoutine);
		}
		
		_textureList.Remove(this);

		base.Destroy ();
	}

	 public static void DestroyAll()
    {
		GstCustomTexture[] killList = _textureList.ToArray();

        foreach(GstCustomTexture t in killList)
        {
            t.Destroy();
        }
    }

	public void SetPipeline(string p)
	{
		Pipeline = p;
		if(_player.IsLoaded || _player.IsPlaying)
			_player.Close ();

		_player.SetPipeline (Pipeline);
		_player.CreateStream();
	}

	bool _imageGrabed=false;
	Vector2 _grabbedSize;
	int _grabbedComponents;

	void GrabFrame()
	{
		Vector2 sz;
		int c;

		if (_player.GrabFrame (out sz, out c)) {
			_grabbedSize = sz;
			_grabbedComponents = c;
			_imageGrabed = true;
			_triggerOnFrameGrabbed (0);
		}
	}


	/* old way to Grab Frame. Keeping just in case
	void ImageGrabberThread()
	{
	//	Vector2 sz;
	//	int c;
		while (!_isDone) {
			GrabFrame();
		
			if (_player.GrabFrame (out sz, out c)) {
				_grabbedSize = sz;
				_grabbedComponents = c;
				_imageGrabed = true;
				_triggerOnFrameGrabbed (0);
	
			}
			*
		}
	}
	*/

	private void BlitIfNeeded() 
	{
	
		if (_player == null)
		return;

		if(_imageGrabed){

			_countGrabs=0; // reset count. blitting happpening...

			Resize ((int)_grabbedSize.x,(int) _grabbedSize.y,_grabbedComponents,0);
			
			if (m_Texture[0] == null)
				Debug.LogError ("The GstTexture does not have a texture assigned and will not paint.");
			else {
				_player.BlitTexture (m_Texture [0].GetNativeTexturePtr (), m_Texture [0].width, m_Texture [0].height);
			}
			
			_imageGrabed = false;
			OnFrameCaptured(0);
			_triggerOnFrameBlitted (0);
			
		}	
	}

	private IEnumerator CallBlitEndOfFrames()
	{
		while (true) {

			// Wait until all frame rendering is done
			yield return new WaitForEndOfFrame();

			BlitIfNeeded(); 
		}
	}
	
	
	// Use this for initialization
	IEnumerator Start () {

		_blitRoutine = StartCoroutine("CallBlitEndOfFrames");

		yield return _blitRoutine;
	}


}
