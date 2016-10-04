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
