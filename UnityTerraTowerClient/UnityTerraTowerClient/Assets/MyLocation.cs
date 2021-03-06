﻿using UnityEngine;
using System;
using System.Collections;

/* Version 1.1 6/11/14 */
public class MyLocation : MonoBehaviour {
			
	private bool working;

	private float lastLat;
	private float lastLng;
	private float lastAlt;

	/* When mock locations is true the apparent movement is from north east to south west, from low altitude to high altude, over span seconds */
	public bool mockLocations;  //Set in the UI
	private long mockLocationCount = 0L;
	private float east = -117.8411751f;
	private float west = -117.8442599f;
	private float north = 33.6472217f;
	private float south = 33.6446338f;

	//private float north = 33.6358538f;
	//private float east = -117.8372621f;
	//private float south = 33.635025f;
	//private float west = -117.8384372f;

	private float low = 10f;
	private float high = 100f;
	private float span = 600f;

	public static class Haversine {
		/* Calculate the distance between 2 points in 3D.  This is a little sketchy because the distance between 2 points in 3D
		 * is illdefined if you aren't going to stay at the same altitude as you travel between the two points.  The
		 * curvature of the earth starts to mess with this */
		public static double calculate(double lat1, double lng1, double alt1,double lat2, double lng2,double alt2) {
			var R = 6372800; // In meters
			var dLat = toRadians(lat2 - lat1);
			var dLon = toRadians(lng2 - lng1);
			lat1 = toRadians(lat1);
			lat2 = toRadians(lat2);
			
			var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) + Math.Sin(dLon / 2) * Math.Sin(dLon / 2) * Math.Cos(lat1) * Math.Cos(lat2);
			var c = 2 * Math.Atan2(Math.Sqrt(a),Math.Sqrt(1-a));
			double distance = R * c;
			
			double height = alt1 - alt2;
			distance = Math.Pow (distance,2) + Math.Pow (height,2);
			return Math.Sqrt (distance);
			
			
		}
		
		public static double toRadians(double angle) {
			return Math.PI * angle / 180.0;
		}
		
		public static double toDegrees(double deg) {
			return deg * 180.0 / Math.PI;
		}
	
		/* Calculate the new latitude and longitude based on current lat and long and a bearing and distance away */
		/* bearing in degrees */
		/* distance in meters */
		public static double[] ComputeLatLng(double startLat, double startLng, double bearing, double distance) {
			double[] vNewLatLng = new double[2];
			//distance = distance / 6371000; //for meters
			distance = distance / 6372800; //for meters
			bearing = Haversine.toRadians(bearing);
			
			double vLat1 = Haversine.toRadians(startLat);
			double vLng1 = Haversine.toRadians(startLng);
			
			double vNewLat = Math.Asin(Math.Sin(vLat1) * Math.Cos(distance) +
			                           Math.Cos(vLat1) * Math.Sin(distance) * Math.Cos(bearing));
			
			double vNewLng = vLng1 + Math.Atan2(Math.Sin(bearing) * Math.Sin(distance) * Math.Cos(vLat1),
			                                 Math.Cos(distance) - Math.Sin(vLat1) * Math.Sin(vNewLat));

			if (double.IsNaN(vNewLat) || double.IsNaN(vNewLng)) {
				return null;
			}
			
			vNewLatLng[0] = Haversine.toDegrees(vNewLat);
			vNewLatLng[1] = Haversine.toDegrees(vNewLng);
			
			return vNewLatLng;
		}

		/* Computer a latitude and longitude based on an origin lat and lng and a distance away from the origin in meters */
		public static double[] ComputeLatLng2(double originLat, double originLng, double x, double y) {
			double h = Math.Sqrt (x * x + y * y);
			double bearing;
			if ((x > 0) & (y > 0)) {
				bearing = (Math.PI/2) - Math.Sin (y / h);
			} else {
				if ((x > 0) & (y < 0)) {
					bearing = Math.PI / 2 + Math.Sin (-y / h);
				} else {
					if ((x < 0) & (y > 0)) {
						bearing = 3*Math.PI / 2 + Math.Sin (y / h);
					} else {
						bearing = Math.PI + (Math.PI / 2 - Math.Sin (-y / h));
					}
				}
			}
			return ComputeLatLng (originLat, originLng, toDegrees(bearing), h);
		}



	}

	/* Getters for finding the most recent location */
	public float getLat(){
		return lastLat;
	}

	public float getLng(){
		return lastLng;
	}

	public float getAlt(){
		return lastAlt;
	}
		
		
	// Use this for initialization
	IEnumerator Start () {
		/*double[] thing = Haversine.ComputeLatLng2 (0.0f, 0.0f, 2000f, 2000f);
		Debug.Log ("Answer 0:" + thing[0]+":"+thing[1]);
		thing = Haversine.ComputeLatLng2 (0.0, 0.0, -2000, 2000);
		Debug.Log ("Answer 0:" + thing[0]+":"+thing[1]);
		thing = Haversine.ComputeLatLng2 (0.0, 0.0, 2000, -2000);
		Debug.Log ("Answer 0:" + thing[0]+":"+thing[1]);
		thing = Haversine.ComputeLatLng2 (0.0, 0.0, -2000, -2000);
		Debug.Log ("Answer 0:" + thing[0]+":"+thing[1]);


		Debug.Log("Answer 1:"+Haversine.toDegrees(Math.Asin (4.0/Math.Sqrt(32.0))));
		Debug.Log("Answer 2:"+Haversine.toDegrees(Math.Asin (3.0/Math.Sqrt(32.0))));
		Debug.Log("Answer 3:"+Haversine.toDegrees(Math.Asin (2.0/Math.Sqrt(32.0))));*/

		working = false;
			
		// First, check if user has location service enabled
		if (!Input.location.isEnabledByUser)
			yield return false;
			
		// Start service before querying location
		Input.location.Start (1f,1f);
			
		// Wait until service initializes
		int maxWait = 20;
		while ((Input.location.status == LocationServiceStatus.Initializing) && (maxWait > 0)) {
			yield return new WaitForSeconds (1);
			maxWait--;
		}
		// Service didn't initialize in 20 seconds
		if (maxWait < 1) {
			print ("Timed out");
			yield return false;
		}
		// Connection has failed
		if (Input.location.status == LocationServiceStatus.Failed) {
			return false;
		}
		// Access granted and location value could be retrieved
		else {
			working = true;
		}
		// Stop service if there is no need to query location updates continuously
		//Input.location.Stop ();
			
			
	}
		

	void Update(){
		if (working) {
			if(mockLocations){
				mockLocationCount++;
				if((mockLocationCount % 200)==0){
					//Debug.Log(mockLocationCount);
					lastLng = Mathf.Lerp(east,west,Time.realtimeSinceStartup/span);
					lastLat = Mathf.Lerp(north,south,Time.realtimeSinceStartup/span);
					lastAlt = Mathf.Lerp(low,high,Time.realtimeSinceStartup/span);
				}
			}
			else{
				if ((Input.location.lastData.longitude != lastLng) || (Input.location.lastData.latitude != lastLat)) {
					lastLng = Input.location.lastData.longitude;
					lastLat = Input.location.lastData.latitude;
					lastAlt=11;
					//lastAlt = Input.location.lastData.altitude;
				}
			}
		}
	}
		

}
