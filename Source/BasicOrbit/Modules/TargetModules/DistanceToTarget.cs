#region License
/*
 * Basic Orbit
 * 
 * BasicOrbit DistanceToTarget - Readout module for distance to target
 * 
 * Copyright (C) 2016 DMagic
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by 
 * the Free Software Foundation, either version 3 of the License, or 
 * (at your option) any later version. 
 * 
 * This program is distributed in the hope that it will be useful, 
 * but WITHOUT ANY WARRANTY; without even the implied warranty of 
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the 
 * GNU General Public License for more details. 
 * 
 * You should have received a copy of the GNU General Public License 
 * along with this program.  If not, see <http://www.gnu.org/licenses/>. 
 * 
 * 
 */
#endregion

using UnityEngine;

namespace BasicOrbit.Modules.TargetModules
{
	public class DistanceToTarget : BasicModule
	{
		public DistanceToTarget(string t)
			: base(t)
		{

		}

		protected override void UpdateVisible()
		{
			BasicSettings.Instance.showDistance = IsVisible;
		}

		protected override void UpdateAlways()
		{
			BasicSettings.Instance.showDistanceAlways = AlwaysShow;
		}

		protected override string fieldUpdate()
		{
			if (!BasicTargetting.Updated)
				return "---";

			if (BasicTargetting.ActiveVessel == null)
				return "---";

			if (BasicTargetting.TargetObject == null)
				return "---";

			Transform tgtTransform = BasicTargetting.TargetObject.GetTransform();

			if (tgtTransform == null)
				return "---";

			if (BasicTargetting.IsVessel)
			{
				Vessel targetVessel = BasicTargetting.TargetObject.GetVessel();

				if (targetVessel == null)
					return "---";

				Vector3d targetPos = tgtTransform.position;
				Vector3d originPos = BasicTargetting.ActiveVessel.transform.position;

				if (targetVessel.loaded)
				{
					if (BasicTargetting.ActiveVessel.GetReferenceTransformPart() != null &&
						BasicTargetting.ActiveVessel.GetReferenceTransformPart().FindModulesImplementing<ModuleDockingNode>().Count > 0)
					{
						originPos = BasicTargetting.ActiveVessel.GetReferenceTransformPart().FindModulesImplementing<ModuleDockingNode>()[0].nodeTransform.position;

						if (BasicTargetting.TargetTransform != null)
							targetPos = BasicTargetting.TargetTransform.position;
						else
							targetPos = targetVessel.ReferenceTransform.position;
					}
					else if (FlightGlobals.fetch != null)
						return result(BasicTargetting.VesselTargetDelta.magnitude);
					else
					{
						originPos = BasicTargetting.ActiveVessel.ReferenceTransform.position;
						targetPos = targetVessel.ReferenceTransform.position;
					}
				}

				return result(Vector3d.Distance(targetPos, originPos));
			}
			else if (BasicTargetting.IsCelestial)
				return result(Vector3d.Distance(tgtTransform.position, BasicTargetting.ActiveVessel.transform.position));
			else
				return "---";
		}

		private string result(double d)
		{
			return string.Format("{0}", d.CloseDistance());
		}
	}
}
