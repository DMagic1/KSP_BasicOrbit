
namespace BasicOrbit.Modules.ManeuverModules
{
    public class ManeuverTotal : BasicModule
    {
        public ManeuverTotal(string t)
            : base(t)
        {

        }

        protected override void UpdateVisible()
        {
            BasicSettings.Instance.showManeuverNodeTotal = IsVisible;
        }

        protected override void UpdateAlways()
        {
            BasicSettings.Instance.showManeuverNodeTotalAlways = AlwaysShow;
        }

        protected override string fieldUpdate()
        {
            if (!BasicManeuvering.Updated)
                return "---";

            if (!BasicManeuvering.MultipleManeuvers)
                return "---";

            return result(BasicManeuvering.AllManeuverRemaining, BasicManeuvering.AllManeuverTotal);
        }

        private string result(double r, double t)
        {
            return string.Format("{0} / {1}m/s", r.ToString("N1"), t.ToString("N1"));
        }
    }
}
