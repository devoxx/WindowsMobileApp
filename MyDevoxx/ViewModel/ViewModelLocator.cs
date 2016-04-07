using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;

namespace MyDevoxx.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        public const string ScheduleViewKey = "ScheduleView";
        public const string MapViewKey = "MapView";
        public const string TracksViewKey = "TracksView";
        public const string SpeakersViewKey = "SpeakersView";
        public const string ConferenceSelectorViewKey = "ConferenceSelectorView";
        public const string TalkDetailsViewKey = "TalkDetailsView";
        public const string SpeakerDetailsViewKey = "SpeakerDetailsView";
        public const string SettingsViewKey = "SettingsView";
        public const string AboutViewKey = "AboutView";
        public const string CreditsViewKey = "CreditsView";
        public const string RegisterViewKey = "RegisterView";
        public const string VotingResultViewKey = "VotingResultView";

        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            SimpleIoc.Default.Register<MapViewModel>();
            SimpleIoc.Default.Register<ScheduleViewModel>();
            SimpleIoc.Default.Register<SpeakersViewModel>();
            SimpleIoc.Default.Register<SpeakerDetailsViewModel>();
            SimpleIoc.Default.Register<TracksViewModel>();
            SimpleIoc.Default.Register<TalkDetailsViewModel>();
            SimpleIoc.Default.Register<SettingsViewModel>();
            SimpleIoc.Default.Register<AboutViewModel>();
            SimpleIoc.Default.Register<VotingResultViewModel>();
            SimpleIoc.Default.Register<ConferenceSelectorViewModel>(true);
        }

        public MapViewModel Map
        {
            get { return ServiceLocator.Current.GetInstance<MapViewModel>(); }
        }

        public ScheduleViewModel Schedule
        {
            get { return ServiceLocator.Current.GetInstance<ScheduleViewModel>(); }
        }

        public SpeakersViewModel Speakers
        {
            get { return ServiceLocator.Current.GetInstance<SpeakersViewModel>(); }
        }

        public TracksViewModel Tracks
        {
            get { return ServiceLocator.Current.GetInstance<TracksViewModel>(); }
        }

        public ConferenceSelectorViewModel ConferenceSelectorViewModel
        {
            get { return ServiceLocator.Current.GetInstance<ConferenceSelectorViewModel>(); }
        }

        public TalkDetailsViewModel TalkDetails
        {
            get { return ServiceLocator.Current.GetInstance<TalkDetailsViewModel>(); }
        }

        public SpeakerDetailsViewModel SpeakerDetails
        {
            get { return ServiceLocator.Current.GetInstance<SpeakerDetailsViewModel>(); }
        }

        public AboutViewModel About
        {
            get { return ServiceLocator.Current.GetInstance<AboutViewModel>(); }
        }

        public SettingsViewModel Settings
        {
            get { return ServiceLocator.Current.GetInstance<SettingsViewModel>(); }
        }

        public VotingResultViewModel VotingResult
        {
            get { return ServiceLocator.Current.GetInstance<VotingResultViewModel>(); }
        }

        public static void Cleanup()
        {
            ServiceLocator.Current.GetInstance<MapViewModel>().Cleanup();
            ServiceLocator.Current.GetInstance<ScheduleViewModel>().Cleanup();
            ServiceLocator.Current.GetInstance<SpeakersViewModel>().Cleanup();
            ServiceLocator.Current.GetInstance<TracksViewModel>().Cleanup();
        }
    }
}