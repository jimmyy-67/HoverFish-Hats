using Nautilus.Options;
using HoverfishHats.Config;
namespace HoverfishHats.Options
{
    public class HoverfishHatsOptions : ModOptions
    {
        public HoverfishHatsOptions() : base("Hoverfish Hats Mod")
        {
            var toggleVisible = ModToggleOption.Create(
                "hatVisible", "Hats Visible",
                ModConfig.HatVisible.Value);
            toggleVisible.OnChanged += (s, e) => {
                ModConfig.HatVisible.Value = e.Value;
                MainPlugin.UpdateAllHats();
            };
            AddItem(toggleVisible);
            var primaryChoice = ModChoiceOption<HatType>.Create(
                "primaryHat", "Primary Hat",
                (HatType[])System.Enum.GetValues(typeof(HatType)),
                ModConfig.PrimaryHat.Value);
            primaryChoice.OnChanged += (s, e) => {
                ModConfig.PrimaryHat.Value = e.Value;
                LoadNautilusDefaults(e.Value);
                MainPlugin.UpdateAllHats();
            };
            AddItem(primaryChoice);
            var wildToggle = ModToggleOption.Create(
                "wildHats", "Wild Hoverfish Hats",
                ModConfig.WildHatsEnabled.Value);
            wildToggle.OnChanged += (s, e) => {
                ModConfig.WildHatsEnabled.Value = e.Value;
                MainPlugin.UpdateAllHats();
            };
            AddItem(wildToggle);
            var warningAccepted = ModToggleOption.Create(
                "warningAccepted", "Accept Experimental Risks",
                ModConfig.ExperimentalWarningAccepted.Value);
            warningAccepted.OnChanged += (s, e) => {
                ModConfig.ExperimentalWarningAccepted.Value = e.Value;
                MainPlugin.UpdateAllHats();
            };
            AddItem(warningAccepted);
            var experimentalToggle = ModToggleOption.Create(
                "experimentalMode", "Experimental Mode",
                ModConfig.ExperimentalMode.Value);
            experimentalToggle.OnChanged += (s, e) => {
                ModConfig.ExperimentalMode.Value = e.Value;
                MainPlugin.UpdateAllHats();
            };
            AddItem(experimentalToggle);
            var enableSecondary = ModToggleOption.Create(
                "enableSecondary", "Secondary Hat",
                ModConfig.EnableSecondaryHat.Value);
            enableSecondary.OnChanged += (s, e) => {
                ModConfig.EnableSecondaryHat.Value = e.Value;
                MainPlugin.UpdateAllHats();
            };
            AddItem(enableSecondary);
            var secondaryChoice = ModChoiceOption<HatType>.Create(
                "secondaryHat", "Secondary Type",
                (HatType[])System.Enum.GetValues(typeof(HatType)),
                ModConfig.SecondaryHat.Value);
            secondaryChoice.OnChanged += (s, e) => {
                ModConfig.SecondaryHat.Value = e.Value;
                MainPlugin.UpdateAllHats();
            };
            AddItem(secondaryChoice);
            var enableTertiary = ModToggleOption.Create(
                "enableTertiary", "Tertiary Hat",
                ModConfig.EnableTertiaryHat.Value);
            enableTertiary.OnChanged += (s, e) => {
                ModConfig.EnableTertiaryHat.Value = e.Value;
                MainPlugin.UpdateAllHats();
            };
            AddItem(enableTertiary);
            var tertiaryChoice = ModChoiceOption<HatType>.Create(
                "tertiaryHat", "Tertiary Type",
                (HatType[])System.Enum.GetValues(typeof(HatType)),
                ModConfig.TertiaryHat.Value);
            tertiaryChoice.OnChanged += (s, e) => {
                ModConfig.TertiaryHat.Value = e.Value;
                MainPlugin.UpdateAllHats();
            };
            AddItem(tertiaryChoice);
        }
        private void LoadNautilusDefaults(HatType type)
        {
            ModConfig.ResetHatToDefaults(type);
        }
    }
}