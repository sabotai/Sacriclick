using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MapKeys : MonoBehaviour {


	int howManyKeys = 0;
	Vector2 pStdDev = new Vector2(0f,0f);
	Vector2 initStdDev = new Vector2(0f,0f);
	public static float xMovement = 0.0f;
	float initialX = 0f;
	public static float gripOpenDelta = 0f;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		List<Vector2> keys = mapKeys();

		Vector2[] allKeys = new Vector2[keys.Count];
		keys.CopyTo(allKeys);

		howManyKeys = allKeys.Length;

		Vector2 centerPoint = mapCenter(allKeys);

		if (howManyKeys == 5 && initialX == 0f) {
			initialX = centerPoint.x;
			initStdDev = stdDev(keys);
		}
		xMovement = (centerPoint.x - initialX) / 10f;
		if (centerPoint != new Vector2(0f,0f)) {
			//Debug.Log("center = " + centerPoint);
			Vector2 myStdDev = stdDev(keys);
			//Debug.Log("stddev = " + myStdDev);//, centerPoint));
			if (myStdDev != new Vector2(0f,0f) && howManyKeys >= 5){
				//ne
				gripOpenDelta = (myStdDev.x + myStdDev.y) - (initStdDev.x + initStdDev.y);
				if (gripOpenDelta != 0f) {
					//Debug.Log("gripOpenDelta = " + gripOpenDelta);
				} 
				pStdDev = myStdDev;
			} else {
				gripOpenDelta = 0;
			}
		}
	}

	Vector2 mapCenter(Vector2[] points){ //finds the mean
		Vector2 keysCenter = new Vector2(0f,0f);
		for (int i = 0; i < points.Length; i++){
			points[i] /= points.Length;
			keysCenter += points[i];
		}
		return keysCenter;
	}
/*
	//OLD BROKEN METHOD
	Vector2 mapStdDev(Vector2[] points, Vector2 center){ //still something wrong with y for some reason
		Vector2 sumDevSq = new Vector2(0f,0f);
		for (int i = 0; i < points.Length; i++){
			Vector2 dev = points[i] - center;
			sumDevSq += Vector2.Scale(dev, dev);//(new Vector2 (dev.x * dev.x, dev.y * dev.y)); //adding the sq deviation of each
		}
		Vector2 variance = sumDevSq / (points.Length); //divide by N [N-1 for sample; N for total std dev]
		//Debug.Log("total variance = " + variance);
		Vector2 stdDev = new Vector2(Mathf.Sqrt(variance.x), Mathf.Sqrt(variance.y));

		return stdDev;
	}
*/
   //using list of vector2s
	public static Vector2 stdDev(List<Vector2> values) //for some reason, these only work using lists.  arrays fucks it up
    {
    	float[] xValues = values.Select(a => a.x).ToArray();
    	float[] yValues = values.Select(b => b.y).ToArray();
		
        float avgX = xValues.Average();
        float sdX = Mathf.Sqrt(xValues.Average(v=>Mathf.Pow(v-avgX,2)));

        float avgY = yValues.Average();
        float sdY = Mathf.Sqrt(yValues.Average(v=>Mathf.Pow(v-avgY,2)));

		return new Vector2(sdX, sdY);
   }

	List<Vector2> mapKeys() {
		List<Vector2> locKeys = new List<Vector2>();

		foreach(KeyCode vKey in System.Enum.GetValues(typeof(KeyCode))){
		    if(Input.GetKey(vKey)){
		    	Vector2 coord = new Vector2(0f,0f);
		        switch (vKey){
             		case KeyCode.F1:
		             	coord = new Vector2(1f,0f);
             			break;
		            case KeyCode.F2:
		             	coord = new Vector2(2f,0f);
		             	break;
		            case KeyCode.F3:
		             	coord = new Vector2(3f,0f);
		             	break;
		            case KeyCode.F4:
		             	coord = new Vector2(4f,0f);
		             	break;
		            case KeyCode.F5:
		             	coord = new Vector2(5f,0f);
		             	break;
		            case KeyCode.F6:
		             	coord = new Vector2(6f,0f);
		             	break;
		            case KeyCode.F7:
		             	coord = new Vector2(7f,0f);
		             	break;
		            case KeyCode.F8:
		             	coord = new Vector2(8f,0f);
		             	break;
		            case KeyCode.F9:
		             	coord = new Vector2(9f,0f);
		             	break;
		            case KeyCode.F10:
		             	coord = new Vector2(10f,0f);
		             	break;
		            case KeyCode.F11:
		             	coord = new Vector2(11f,0f);
		             	break;
		            case KeyCode.F12:
		             	coord = new Vector2(12f,0f);
		             	break;
		            case KeyCode.Pause:
		             	coord = new Vector2(13f,0f);
		             	break;
		            case KeyCode.Print:
		             	coord = new Vector2(14f,0f);
		             	break;
		            case KeyCode.Delete:
		             	coord = new Vector2(15f,0f);
		             	break;


		            case KeyCode.BackQuote:
		             	coord = new Vector2(0f,1f);
		             	break;
		            case KeyCode.Alpha1:
		             	coord = new Vector2(1f,1f);
		             	break;
		            case KeyCode.Alpha2:
		             	coord = new Vector2(2f,1f);
		             	break;
		            case KeyCode.Alpha3:
		             	coord = new Vector2(3f,1f);
		             	break;
		            case KeyCode.Alpha4:
		             	coord = new Vector2(4f,1f);
		             	break;
		            case KeyCode.Alpha5:
		             	coord = new Vector2(5f,1f);
		             	break;
		            case KeyCode.Alpha6:
		             	coord = new Vector2(6f,1f);
		             	break;
		            case KeyCode.Alpha7:
		             	coord = new Vector2(7f,1f);
		             	break;
		            case KeyCode.Alpha8:
		             	coord = new Vector2(8f,1f);
		             	break;
		            case KeyCode.Alpha9:
		             	coord = new Vector2(9f,1f);
		             	break;
		            case KeyCode.Alpha0:
		             	coord = new Vector2(10f,1f);
		             	break;
		            case KeyCode.Minus:
		             	coord = new Vector2(11f,1f);
		             	break;
		            case KeyCode.Equals:
		             	coord = new Vector2(12f,1f);
		             	break;
		            case KeyCode.Backspace:
		             	coord = new Vector2(13f,1f);
		             	break;


		            case KeyCode.Tab:
		             	coord = new Vector2(0f,2f);
		             	break; 
		            case KeyCode.Q:
		             	coord = new Vector2(1f,2f);
		             	break; 
		            case KeyCode.W:
		             	coord = new Vector2(2f,2f);
		             	break; 
		            case KeyCode.E:
		             	coord = new Vector2(3f,2f);
		             	break; 
		            case KeyCode.R:
		             	coord = new Vector2(4f,2f);
		             	break; 
		            case KeyCode.T:
		             	coord = new Vector2(5f,2f);
		             	break; 
		            case KeyCode.Y:
		             	coord = new Vector2(6f,2f);
		             	break; 
		            case KeyCode.U:
		             	coord = new Vector2(7f,2f);
		             	break; 
		            case KeyCode.I:
		             	coord = new Vector2(8f,2f);
		             	break; 
		            case KeyCode.O:
		             	coord = new Vector2(9f,2f);
		             	break; 
		            case KeyCode.P:
		             	coord = new Vector2(10f,2f);
		             	break; 
		            case KeyCode.LeftBracket:
		             	coord = new Vector2(11f,2f);
		             	break;
		            case KeyCode.RightBracket:
		             	coord = new Vector2(12f,2f);
		             	break;
		            case KeyCode.Backslash:
		             	coord = new Vector2(13f,2f);
		             	break;


		            case KeyCode.CapsLock:
		             	coord = new Vector2(0f,3f);
		             	break; 
		            case KeyCode.A:
		             	coord = new Vector2(1f,3f);
		             	break; 
		            case KeyCode.S:
		             	coord = new Vector2(2f,3f);
		             	break; 
		            case KeyCode.D:
		             	coord = new Vector2(3f,3f);
		             	break; 
		            case KeyCode.F:
		             	coord = new Vector2(4f,3f);
		             	break; 
		            case KeyCode.G:
		             	coord = new Vector2(5f,3f);
		             	break; 
		            case KeyCode.H:
		             	coord = new Vector2(6f,3f);
		             	break; 
		            case KeyCode.J:
		             	coord = new Vector2(7f,3f);
		             	break; 
		            case KeyCode.K:
		             	coord = new Vector2(8f,3f);
		             	break; 
		            case KeyCode.L:
		             	coord = new Vector2(9f,3f);
		             	break; 
		            case KeyCode.Semicolon:
		             	coord = new Vector2(10f,3f);
		             	break; 
		            case KeyCode.Quote:
		             	coord = new Vector2(11f,3f);
		             	break;
		            case KeyCode.Return:
		             	coord = new Vector2(12f,3f);
		             	break;

		            case KeyCode.LeftShift:
		             	coord = new Vector2(0f,4f);
		             	break; 
		            case KeyCode.Z:
		             	coord = new Vector2(1f,4f);
		             	break; 
		            case KeyCode.X:
		             	coord = new Vector2(2f,4f);
		             	break; 
		            case KeyCode.C:
		             	coord = new Vector2(3f,4f);
		             	break; 
		            case KeyCode.V:
		             	coord = new Vector2(4f,4f);
		             	break; 
		            case KeyCode.B:
		             	coord = new Vector2(5f,4f);
		             	break; 
		            case KeyCode.N:
		             	coord = new Vector2(6f,4f);
		             	break; 
		            case KeyCode.M:
		             	coord = new Vector2(7f,4f);
		             	break; 
		            case KeyCode.Comma:
		             	coord = new Vector2(8f,4f);
		             	break; 
		            case KeyCode.Period:
		             	coord = new Vector2(9f,4f);
		             	break; 
		            case KeyCode.Slash:
		             	coord = new Vector2(10f,4f);
		             	break; 
		            case KeyCode.RightShift:
		             	coord = new Vector2(11f,4f);
		             	break;

		            case KeyCode.LeftControl:
		             	coord = new Vector2(0f,5f);
		             	break; 
		            case KeyCode.LeftWindows:
		             	coord = new Vector2(1f,5f);
		             	break; 
		            case KeyCode.LeftCommand:
		             	coord = new Vector2(1f,5f);
		             	break; 
		            case KeyCode.LeftAlt:
		             	coord = new Vector2(2f,5f);
		             	break; 
		            case KeyCode.Space:
		             	coord = new Vector2(3f,5f);
		             	break; 
		            case KeyCode.RightAlt:
		             	coord = new Vector2(4f,5f);
		             	break; 
		            case KeyCode.RightCommand:
		             	coord = new Vector2(4f,5f);
		             	break; 
		            case KeyCode.Menu:
		             	coord = new Vector2(5f,5f);
		             	break; 
		            case KeyCode.RightControl:
		             	coord = new Vector2(6f,5f);
		             	break; 
             	} //end switch
				locKeys.Add(coord);
            }   //end if any key
		} //end foreach
		//Vector2[] allKeys = new Vector2[keys.Count];
		//keys.CopyTo(allKeys);

		return locKeys;
	}

}
