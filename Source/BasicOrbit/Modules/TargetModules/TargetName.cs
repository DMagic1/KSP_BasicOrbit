#region License
/*
 * Basic Orbit
 * 
 * BasicOrbit TargetName - Readout module for the target name
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

namespace BasicOrbit.Modules.TargetModules
{
	public class TargetName : BasicModule
	{
		public TargetName(string t)
			: base(t)
		{

		}

		protected override void UpdateVisible()
		{
			BasicSettings.Instance.showTargetName = IsVisible;
		}

		protected override void UpdateAlways()
		{
			BasicSettings.Instance.showTargetNameAlways = AlwaysShow;
		}

		protected override string fieldUpdate()
		{
			if (!BasicTargetting.Updated)
				return "---";

			if (BasicTargetting.IsCelestial)
			{
				if (BasicTargetting.TargetBody == null)
					return "---";

				return BasicTargetting.TargetBody.displayName.LocalizeBodyName();
			}
			else if (BasicTargetting.IsVessel)
			{
				if (BasicTargetting.TargetObject == null)
					return "---";

				Vessel targetVessel = BasicTargetting.TargetObject.GetVessel();

				if (targetVessel == null)
					return "---";

				if (targetVessel.loaded && targetVessel.GetReferenceTransformPart() != null && targetVessel.GetReferenceTransformPart().FindModulesImplementing<ModuleDockingNode>().Count > 0)
					return string.Format("{0} [{1}]", targetVessel.vesselName, targetVessel.GetReferenceTransformPart().partInfo.title);
				else
					return targetVessel.vesselName;
			}
			else
				return "---";
		}
	}
}
