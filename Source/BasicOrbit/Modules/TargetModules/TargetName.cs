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

				if (targetVessel.loaded && BasicTargetting.TargetObject is ModuleDockingNode)
				{
					ModuleDockingNode target = BasicTargetting.TargetObject as ModuleDockingNode;

					if (target == null)
					{
						if (targetVessel.vesselName.Length > 28)
							return targetVessel.vesselName.Substring(0, 28);

						return targetVessel.vesselName;
					}

					int vL = targetVessel.vesselName.Length;
					int dL = target.part.partInfo.title.Length;

					string v = targetVessel.vesselName;
					string d = target.part.partInfo.title;

					if (vL + dL + 3 > 28)
					{
						if (vL > 12)
						{
							if (dL > 13)
							{
								v = v.Substring(0, 12);
								d = d.Substring(0, 13);
							}
							else
								v = v.Substring(0, 12);
						}
						else if (dL > 13)
						{
							if (vL > 12)
							{
								v = v.Substring(0, 12);
								d = d.Substring(0, 13);
							}
							else
								d = d.Substring(0, 13);
						}

					}

					return string.Format("{0} [{1}]", v, d);
				}
				else
				{
					if (targetVessel.vesselName.Length > 28)
						return targetVessel.vesselName.Substring(0, 28);

					return targetVessel.vesselName;
				}
			}
			else
				return "---";
		}
	}
}
