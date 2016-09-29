using System;
using System.Collections.Generic;
using UnityEngine;

namespace BasicOrbit.Modules.TargetModules
{
	public class DistanceToTarget : BasicModule
	{
		public DistanceToTarget(string t)
			: base(t)
		{

		}

		protected override void UpdateSettings()
		{
			BasicSettings.Instance.showDistance = IsVisible;
			BasicSettings.Instance.showDistanceAlways = AlwaysShow;
		}

		protected override string fieldUpdate()
		{
			if (FlightGlobals.ActiveVessel == null)
				return "---";

			if (!BasicTargetting.Updated)
				return "---";

			ITargetable tgt = FlightGlobals.ActiveVessel.targetObject;

			Vector3 targetPos = BasicTargetting.TargetOrbit.pos;
			Vector3 originPos = BasicTargetting.ShipOrbit.pos;

			if (tgt.GetVessel() != null &&
				tgt.GetVessel().loaded &&
				tgt is ModuleDockingNode)
			{
				targetPos = ((ModuleDockingNode)tgt).nodeTransform.position;

				if (FlightGlobals.ActiveVessel != null &&
					FlightGlobals.ActiveVessel.GetReferenceTransformPart() != null &&
					FlightGlobals.ActiveVessel.GetReferenceTransformPart().FindModulesImplementing<ModuleDockingNode>().Count > 0)
				{
					originPos = FlightGlobals.ActiveVessel.GetReferenceTransformPart().FindModulesImplementing<ModuleDockingNode>()[0].referenceNode.position;
				}
			}

			return result(Vector3d.Distance(targetPos, originPos));
		}

		private string result(double d)
		{
			return string.Format("{0}", d.CloseDistance());
		}
	}
}
