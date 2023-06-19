using System.Collections.Generic;
using Lights;
using UnityEngine;
using Util.Locators;
using VineGeneration.Parameters;

namespace VineGeneration.Util
{
	public struct LightInfo
	{
		public Light light;
		public float intensity;
	}

	public static class LightIntensityHelper
	{
		public static bool IsInLight(GrowParameters ivyParams, Vector3 position)
		{
			if (!ivyParams.lightSensitive) {
				return true;
			}
			
			var lightCollection = ServiceLocator.Get<LightCollection>();
			if (!lightCollection) {
				return true;
			}
			return lightCollection.Lights.IsPointLit(position, ivyParams.minLightIntensity, GlobalVineSettings.current.generalMask);
		}

		public static bool IsPointLit(this List<Light> lights, Vector3 pos, float minIntensity, LayerMask hitMask,
									float maxRayDist = 100)
		{
			var cumulativeIntensity = 0f;

			for (int i = 0; i < lights.Count; i++) {
				var l = lights[i];
				if (!l.enabled || !l.gameObject.activeSelf) {
					continue;
				}

				GetLightIntensity(l, out var intensity, pos, hitMask, maxRayDist);
				cumulativeIntensity += intensity;
			}

			return cumulativeIntensity >= minIntensity;
		}

		public static bool IsPointLit(this List<Light> lights, List<LightInfo> outputLights, Vector3 pos, float minIntensity, LayerMask hitMask, float maxRayDist = 100)
		{
			var cumulativeIntensity = 0f;
			for (int i = 0; i < lights.Count; i++) {
				var l = lights[i];
				if (!l.enabled || !l.gameObject.activeSelf) {
					continue;
				}

				GetLightIntensity(l, out var intensity, pos, hitMask, maxRayDist);
				if (intensity > 0) {
					outputLights.Add(new LightInfo() {
						light = l,
						intensity = intensity
					});
				}
				cumulativeIntensity += intensity;
			}

			return cumulativeIntensity >= minIntensity;
		}

		private static void GetLightIntensity(this Light light, out float intensity, Vector3 pos, LayerMask hitMask, float maxRayDist = 100)
		{
			intensity = 0;
			switch (light.type) {
				case LightType.Point:
					InPointLight(light, pos, hitMask, out intensity);
					break;
				case LightType.Spot:
					InSpotLight(light, pos, hitMask, out intensity);
					break;
				case LightType.Directional:
					InDirectionalLight(light, pos, maxRayDist, hitMask, out intensity);
					break;
			}
		}

		private static void InPointLight(Light pointLight, Vector3 pos, LayerMask hitMask, out float intensityAdj)
		{
			intensityAdj = 0;
			var lightPos = pointLight.transform.position;
			var distance = Vector3.Distance(pos, lightPos);
			var range = pointLight.range;

			if (distance <= range) {
				if (Physics.Raycast(pos, lightPos - pos, distance, hitMask, QueryTriggerInteraction.Ignore)) {
					return;
				}

				var intensity = pointLight.intensity;

				intensityAdj = intensity * (1 / (distance * distance));
			}
		}

		private static void InSpotLight(Light spotLight, Vector3 pos, LayerMask hitMask, out float intensityAdj)
		{
			intensityAdj = 0;
			var range = spotLight.range;
			var lightPos = spotLight.transform.position;
			var distance = Vector3.Distance(pos, lightPos);

			if (distance <= range) {
				if (Physics.Raycast(pos, lightPos - pos, distance, hitMask, QueryTriggerInteraction.Ignore)) {
					return;
				}

				var lightForward = spotLight.transform.forward;
				var spotAngle = spotLight.spotAngle;
				var intensity = spotLight.intensity;
				var oldRot = spotLight.transform.eulerAngles;

				var newXRot = Quaternion.Euler(oldRot + new Vector3(spotAngle / 2, 0, 0)) * Vector3.forward;
				var maxAngle = Vector3.Angle(lightForward, newXRot);
				maxAngle -= maxAngle * 0.1f; // some offset to account for feathered edge of the spot light
				var angleToObject = Vector3.Angle(lightForward, (pos - spotLight.transform.position).normalized);

				if (angleToObject < maxAngle) {
					intensityAdj = intensity * (1 / (distance * distance));
				}
			}
		}

		private static void InDirectionalLight(Light dirLight, Vector3 pos, float maxDistance, LayerMask hitMask,
												out float intensityAdj)
		{
			intensityAdj = 0;
			var lightForward = dirLight.transform.forward;
			var intensity = dirLight.intensity;
			var ray = new Ray(pos, -lightForward);

			if (Physics.Raycast(ray, maxDistance, hitMask, QueryTriggerInteraction.Ignore)) {
				return;
			}

			intensityAdj = intensity;
		}
	}
}
