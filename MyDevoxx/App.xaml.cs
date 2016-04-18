using MyDevoxx.Model;
using MyDevoxx.Services;
using MyDevoxx.ViewModel;
using MyDevoxx.Views;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;
using Microsoft.Practices.ServiceLocation;
using SQLite.Net;
using System;
using System.IO;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Phone.UI.Input;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using GalaSoft.MvvmLight.Messaging;
using MyDevoxx.Utils;
using System.Diagnostics;

// The Blank Application template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace MyDevoxx
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public sealed partial class App : Application
    {
        private TransitionCollection transitions;
        private ApplicationDataContainer settings = ApplicationData.Current.LocalSettings;

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            // Configure and register the MVVM Light NavigationService
            var nav = new NavigationService();
            SimpleIoc.Default.Register<INavigationService>(() => nav);
            nav.Configure(ViewModelLocator.ScheduleViewKey, typeof(ScheduleView));
            nav.Configure(ViewModelLocator.MapViewKey, typeof(MapView));
            nav.Configure(ViewModelLocator.TracksViewKey, typeof(TracksView));
            nav.Configure(ViewModelLocator.SpeakersViewKey, typeof(SpeakersView));
            nav.Configure(ViewModelLocator.ConferenceSelectorViewKey, typeof(ConferenceSelectorView));
            nav.Configure(ViewModelLocator.TalkDetailsViewKey, typeof(TalkDetailsView));
            nav.Configure(ViewModelLocator.SpeakerDetailsViewKey, typeof(SpeakerDetailsView));
            nav.Configure(ViewModelLocator.SettingsViewKey, typeof(SettingsView));
            nav.Configure(ViewModelLocator.CreditsViewKey, typeof(CreditsView));
            nav.Configure(ViewModelLocator.AboutViewKey, typeof(AboutView));
            nav.Configure(ViewModelLocator.RegisterViewKey, typeof(RegisterView));
            nav.Configure(ViewModelLocator.VotingResultViewKey, typeof(VotingResultView));

            // Register the MVVM Light DialogService
            SimpleIoc.Default.Register<IDialogService, DialogService>();
            SimpleIoc.Default.Register<IRestService, RestService>();
            SimpleIoc.Default.Register<IDataService, DataService>();
            SimpleIoc.Default.Register<ITwitterService, TwitterService>();
            SimpleIoc.Default.Register<IVotingService, VotingService>();

            this.InitializeComponent();
            this.Suspending += this.OnSuspending;

            HardwareButtons.BackPressed += HardwareButtons_BackPressed;
        }

        private void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            if (rootFrame != null &&
                rootFrame.Content is TalkDetailsView &&
                (rootFrame.Content as TalkDetailsView).IsVotingVisile())
            {
                (rootFrame.Content as TalkDetailsView).CloseVotingGrid();
                e.Handled = true;
            }
            else if (rootFrame != null && (
                      rootFrame.Content is TalkDetailsView ||
                      rootFrame.Content is SpeakerDetailsView ||
                      rootFrame.Content is SettingsView ||
                      rootFrame.Content is AboutView ||
                      rootFrame.Content is CreditsView ||
                      rootFrame.Content is RegisterView ||
                      rootFrame.Content is VotingResultView))
            {
                SimpleIoc.Default.GetInstance<INavigationService>().GoBack();
                e.Handled = true;
            }
            else if (rootFrame != null &&
                rootFrame.Content is ConferenceSelectorView &&
                settings.Values["conferenceId"] != null &&
                (bool)settings.Values["loadedSuccessful"])
            {
                SimpleIoc.Default.GetInstance<INavigationService>().GoBack();
                e.Handled = true;
            }
            else if (rootFrame != null &&
                rootFrame.Content is ScheduleView &&
                (rootFrame.Content as ScheduleView).IsFilterVisile())
            {
                (rootFrame.Content as ScheduleView).CloseFilter();
                e.Handled = true;
            }
            else if (rootFrame != null &&
                rootFrame.Content is TracksView &&
                (rootFrame.Content as TracksView).IsFilterVisile())
            {
                (rootFrame.Content as TracksView).CloseFilter();
                e.Handled = true;
            }
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used when the application is launched to open a specific file, to display
        /// search results, and so forth.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif
            InitDB();

            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                // TODO: change this value to a cache size that is appropriate for your application
                rootFrame.CacheSize = 1;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    // TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
                // Removes the turnstile navigation for startup.
                if (rootFrame.ContentTransitions != null)
                {
                    this.transitions = new TransitionCollection();
                    foreach (var c in rootFrame.ContentTransitions)
                    {
                        this.transitions.Add(c);
                    }
                }

                rootFrame.ContentTransitions = null;
                rootFrame.Navigated += this.RootFrame_FirstNavigated;

                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter

                Type viewType;
                if (settings.Values["conferenceId"] != null
                    && settings.Values["loadedSuccessful"] != null
                    && (bool)settings.Values["loadedSuccessful"])
                {
                    viewType = typeof(ScheduleView);
                }
                else
                {
                    viewType = typeof(ConferenceSelectorView);
                }

                if (!rootFrame.Navigate(viewType, e.Arguments))
                {
                    throw new Exception("Failed to create initial page");
                }

                Update();
            }

            // Ensure the current window is active
            Window.Current.Activate();
        }

        /// <summary>
        /// Restores the content transitions after the app has launched.
        /// </summary>
        /// <param name="sender">The object where the handler is attached.</param>
        /// <param name="e">Details about the navigation event.</param>
        private void RootFrame_FirstNavigated(object sender, NavigationEventArgs e)
        {
            var rootFrame = sender as Frame;
            rootFrame.ContentTransitions = this.transitions ?? new TransitionCollection()
            {
                //new NavigationThemeTransition()
            };
            rootFrame.Navigated -= this.RootFrame_FirstNavigated;
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();

            // Save application state and stop any background activity
            deferral.Complete();
        }

        private StorageFolder localFolder = ApplicationData.Current.LocalFolder;

        private void InitDB()
        {
            var path = Path.Combine(ApplicationData.Current.LocalFolder.Path, "db.sqlite");
            SQLiteConnection conn = new SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path);

            conn.CreateTable<Conference>();
            conn.CreateTable<Event>();
            conn.CreateTable<Model.Floor>();
            conn.CreateTable<ETag>();
            conn.CreateTable<Speaker>();
            conn.CreateTable<Track>();
            conn.CreateTable<Note>();
            conn.CreateTable<Vote>();
        }

        private void Update()
        {
            if (settings.Values["conferenceId"] != null
                && settings.Values["loadedSuccessful"] != null
                && (bool)settings.Values["loadedSuccessful"])
            {
                IDataService dataService = ServiceLocator.Current.GetInstance<IDataService>();
                dataService.UpdateAll(OnUpdateFinished);
            }
        }

        private void OnUpdateFinished(bool successful)
        {
            if (successful)
            {
                Debug.WriteLine("Send Refresh Message");
                Messenger.Default.Send<MessageType>(MessageType.REFRESH_SPEAKERS);
                Messenger.Default.Send<MessageType>(MessageType.REQUEST_REFRESH_SCHEDULE);
                Messenger.Default.Send<MessageType>(MessageType.REFRESH_TRACKS);
            }
        }
    }
}