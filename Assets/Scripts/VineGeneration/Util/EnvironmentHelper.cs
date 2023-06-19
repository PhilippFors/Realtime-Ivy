using System.Collections.Generic;
using UnityEngine;
using VineGeneration.Core;

namespace VineGeneration.Util
{
	public static class EnvironmentHelper
	{
		public static bool SimpleEnvironmentQuery(BranchQueryInfo info, out EnvironmentHitResponse response)
		{
			return EnvironmentQuery(info, false, false, false, out response);
		}

		public static bool EnvironmentQuery(BranchQueryInfo info, bool spawnNode, bool vertLimit, bool buildMesh, out EnvironmentHitResponse response)
		{
			response = new EnvironmentHitResponse();

			var potentialPoint = info.point + info.direction * info.stepLength;
			response.newPoint = info.point;

			var hitEnv = CheckWall(info, spawnNode, buildMesh, vertLimit, ref response) ||
						CheckFloor(info, potentialPoint, spawnNode, ref response);
			if (!hitEnv) {
				hitEnv = CheckCorner(info, potentialPoint, spawnNode, vertLimit, buildMesh, ref response);
			}

			return hitEnv;
		}

		private static bool CheckWall(BranchQueryInfo info, bool spawnNode, bool vertLimit, bool buildMesh, ref EnvironmentHitResponse response)
		{
			Ray ray = new Ray(info.point, info.direction);
			if (Physics.Raycast(ray, out var hit, info.stepLength, GlobalVineSettings.current.environmentMask, QueryTriggerInteraction.Ignore)) {
				// NewGrowDirectionAfterWallSimple(info.normal, hit.normal, out response.newDirection);
				NewGrowDirectionAfterWall(hit.normal, info.normal, info.direction, out response.newDirection);

				var newPoint = hit.point + hit.normal * info.branch.currentHeight;
				if (spawnNode) {
					GrowUtil.AddNode(info.branch, newPoint, hit.normal, vertLimit, buildMesh);
				}

				response.newPoint = newPoint;
				response.newNormal = hit.normal;
				response.hit = true;
				return true;
			}

			return false;
		}

		private static bool CheckFloor(BranchQueryInfo info, Vector3 potentialPoint, bool spawnNode, ref EnvironmentHitResponse response)
		{
			Ray ray = new Ray(potentialPoint, -info.normal);

			if (Physics.Raycast(ray, out var hit, info.ivyParams.maxDistanceToSurface * 3f, GlobalVineSettings.current.environmentMask, QueryTriggerInteraction.Ignore)) {
				response.newPoint = info.point;
				response.newNormal = hit.normal;
				response.newDirection = info.direction;
				response.hit = true;
				return true;
			}

			return false;
		}

		private static bool CheckCorner(BranchQueryInfo info, Vector3 potentialPoint, bool spawnNode, bool vertLimit, bool buildMesh,
										ref EnvironmentHitResponse response)
		{
			response.newNormal = info.normal;
			Ray ray = new Ray(potentialPoint + info.direction * info.stepLength - info.normal * (info.ivyParams.maxDistanceToSurface * 4), -info.direction);
			if (Physics.Raycast(ray, out var hit, info.stepLength * 2f, GlobalVineSettings.current.environmentMask, QueryTriggerInteraction.Ignore)) {
				// NewGrowDirectionAfterCornerSimple(info.normal, hit.normal, out response.newDirection);
				NewGrowDirectionAfterCorner(info.normal, hit.normal, info.direction, out response.newDirection);
				var newPoint = hit.point + hit.normal * info.branch.currentHeight;
				if (spawnNode) {
					GrowUtil.AddNode(info.branch, potentialPoint, info.normal, vertLimit, buildMesh);
					GrowUtil.AddNode(info.branch, newPoint, hit.normal, vertLimit, buildMesh);
				}

				response.newPoint = newPoint;
				response.newNormal = hit.normal;
				response.hit = true;
				return true;
			}

			return false;
		}

		private static void NewGrowDirectionAfterCorner(Vector3 oldNormal, Vector3 newNormal, Vector3 oldDir,
														out Vector3 newMainGrowDir)
		{
			var right = Quaternion.AngleAxis(90, oldNormal) * oldDir;
			var angledDir = Quaternion.AngleAxis(20f, right) * oldDir;
			newMainGrowDir = Vector3.ProjectOnPlane(angledDir, newNormal).normalized;
		}

		private static void NewGrowDirectionAfterWall(Vector3 newNormal, Vector3 oldNormal, Vector3 oldDir,
													out Vector3 newMainGrowDir)
		{
			var right = Quaternion.AngleAxis(90, oldNormal) * oldDir;
			var angledDir = Quaternion.AngleAxis(-20f, right) * oldDir;
			newMainGrowDir = Vector3.ProjectOnPlane(angledDir, newNormal).normalized;
		}

		private static void NewGrowDirectionAfterWallSimple(Vector3 oldSurfaceNormal, Vector3 newSurfaceNormal, out Vector3 newMainGrowDir)
		{
			newMainGrowDir = Vector3.Normalize(Vector3.ProjectOnPlane(oldSurfaceNormal, newSurfaceNormal));
		}

		private static void NewGrowDirectionAfterCornerSimple(Vector3 oldSurfaceNormal, Vector3 newSurfaceNormal, out Vector3 newMainGrowDir)
		{
			newMainGrowDir = Vector3.Normalize(Vector3.ProjectOnPlane(-oldSurfaceNormal, newSurfaceNormal));
		}

		public static bool CheckPathForward(BranchQueryInfo info, ref Vector3[] turnCheckers, ref int[] resultArr, Vector3 startPoint, Vector3 direction, float distance, bool envCheck, float turnRate)
		{
			RayDirectionGenerator.GetForwardDirectionsNonAlloc(ref turnCheckers, direction, info.normal, turnRate);

			// DebugUtils.DrawDebugLines(startPoint, distance, turnCheckers, Color.blue, Color.magenta, 10f);

			foreach (var checkDir in turnCheckers) {
				Vector3 potentialPoint = startPoint + checkDir * distance;

				var hits = KnnHelper.KnnQueryNonAlloc(KnnIds.breadCrumbs, potentialPoint, ref resultArr, 1);
				if (hits == 0) {
					continue;
				}

				var newInfo = new BranchQueryInfo() {
					branch = info.branch,
					point = potentialPoint,
					normal = info.normal,
					direction = checkDir,
					ivyParams = info.ivyParams,
					stepLength = distance
				};

				if (envCheck && SimpleEnvironmentQuery(newInfo, out var response)) {
					potentialPoint = response.newPoint;
				}

				var bread = BreadCrumbGenerator.BreadCrumbs;
				if (bread.InBreadCrumbDistance(resultArr[0], potentialPoint) && checkDir.magnitude > 0.1f) {
					return true;
				}
			}

			return false;
		}

		public static bool InBreadCrumbDistance(this List<BreadCrumb> breadCrumbs, int res, Vector3 end)
		{
			if (breadCrumbs.Count <= res) {
				return false;
			}

			var start = breadCrumbs[res].point;
			var dist = Vector3.Distance(start, end);
			return dist <= GlobalVineSettings.current.breadCrumbRadius;
		}

		public static bool InBreadCrumbDistance(this List<BreadCrumb> breadCrumbs, int res, Vector3 end, out float distance)
		{
			distance = float.MaxValue;
			if (breadCrumbs.Count <= res) {
				return false;
			}

			var start = breadCrumbs[res].point;
			distance = Vector3.Distance(start, end);
			return distance <= GlobalVineSettings.current.breadCrumbRadius;
		}
	}
}
