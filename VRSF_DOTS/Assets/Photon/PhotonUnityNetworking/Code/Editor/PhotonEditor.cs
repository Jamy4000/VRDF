// ----------------------------------------------------------------------------
// <copyright file="PhotonEditor.cs" company="Exit Games GmbH">
//   PhotonNetwork Framework for Unity - Copyright (C) 2018 Exit Games GmbH
// </copyright>
// <summary>
//   MenuItems and in-Editor scripts for PhotonNetwork.
// </summary>
// <author>developer@exitgames.com</author>
// ----------------------------------------------------------------------------


using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.Compilation;
using UnityEngine;

namespace Photon.Pun
{
    public class PunWizardText
    {
        public string WindowTitle = "PUN Wizard";
        public string SetupWizardWarningTitle = "Warning";
        public string SetupWizardWarningMessage = "You have not yet run the Photon setup wizard! Your game won't be able to connect. See Windows -> Photon Unity Networking.";
        public string MainMenuButton = "Main Menu";
        public string SetupWizardTitle = "PUN Setup";
        public string SetupWizardInfo = "Thanks for importing Photon Unity Networking.\nThis window should set you up.\n\n<b>-</b> To use an existing Photon Cloud App, enter your AppId.\n<b>-</b> To register an account or access an existing one, enter the account's mail address.\n<b>-</b> To use Photon OnPremise, skip this step.";
        public string EmailOrAppIdLabel = "AppId or Email";
        public string AlreadyRegisteredInfo = "The email is registered so we can't fetch your AppId (without password).\n\nPlease login online to get your AppId and paste it above.";
        public string SkipRegistrationInfo = "Skipping? No problem:\nEdit your server settings in the PhotonServerSettings file.";
        public string RegisteredNewAccountInfo = "We created a (free) account and fetched you an AppId.\nWelcome. Your PUN project is setup.";
        public string AppliedToSettingsInfo = "Your AppId is now applied to this project.";
        public string SetupCompleteInfo = "<b>Done!</b>\nAll connection settings can be edited in the <b>PhotonServerSettings</b> now.\nHave a look.";
        public string CloseWindowButton = "Close";
        public string SkipButton = "Skip";
        public string SetupButton = "Setup Project";
        public string CancelButton = "Cancel";
        public string PUNWizardLabel = "PUN Wizard";
        public string SettingsButton = "Settings";
        public string SetupServerCloudLabel = "Setup wizard for setting up your own server or the cloud.";
        public string WarningPhotonDisconnect = "Disconnecting PUN due to recompile.";
        public string StartButton = "Start";
        public string LocateSettingsButton = "Locate PhotonServerSettings";
        public string SettingsHighlightLabel = "Highlights the used photon settings file in the project.";
        public string DocumentationLabel = "Documentation";
        public string OpenPDFText = "Reference PDF";
        public string OpenPDFTooltip = "Opens the local documentation pdf.";
        public string OpenDevNetText = "Doc Pages / Manual";
        public string OpenDevNetTooltip = "Online documentation for Photon.";
        public string OpenCloudDashboardText = "Cloud Dashboard Login";
        public string OpenCloudDashboardTooltip = "Review Cloud App information and statistics.";
        public string OpenForumText = "Open Forum";
        public string OpenForumTooltip = "Online support for Photon.";
        public string OkButton = "Ok";
        public string OwnHostCloudCompareLabel = "I am not quite sure how 'my own host' compares to 'cloud'.";
        public string ComparisonPageButton = "Cloud versus OnPremise";
        public string ConnectionTitle = "Connecting";
        public string ConnectionInfo = "Connecting to the account service...";
        public string ErrorTextTitle = "Error";
        public string IncorrectRPCListTitle = "Warning: RPC-list becoming incompatible!";
        public string IncorrectRPCListLabel = "Your project's RPC-list is full, so we can't add some RPCs just compiled.\n\nBy removing outdated RPCs, the list will be long enough but incompatible with older client builds!\n\nMake sure you change the game version where you use PhotonNetwork.ConnectUsingSettings().";
        public string RemoveOutdatedRPCsLabel = "Remove outdated RPCs";
        public string FullRPCListTitle = "Warning: RPC-list is full!";
        public string FullRPCListLabel = "Your project's RPC-list is too long for PUN.\n\nYou can change PUN's source to use short-typed RPC index. Look for comments 'LIMITS RPC COUNT'\n\nAlternatively, remove some RPC methods (use more parameters per RPC maybe).\n\nAfter a RPC-list refresh, make sure you change the game version where you use PhotonNetwork.ConnectUsingSettings().";
        public string SkipRPCListUpdateLabel = "Skip RPC-list update";
        public string PUNNameReplaceTitle = "Warning: RPC-list Compatibility";
        public string PUNNameReplaceLabel = "PUN replaces RPC names with numbers by using the RPC-list. All clients must use the same list for that.\n\nClearing it most likely makes your client incompatible with previous versions! Change your game version or make sure the RPC-list matches other clients.";
        public string RPCListCleared = "Clear RPC-list";
        public string ServerSettingsCleanedWarning = "Cleared the PhotonServerSettings.RpcList, which breaks compatibility with older builds. You should update the \"App Version\" in the PhotonServerSettings to avoid issues.";
        public string WizardMainWindowInfo = "This window should help you find important settings for PUN, as well as documentation.";
    }


    [InitializeOnLoad]
    public class PhotonEditor : EditorWindow
    {
        protected static Type WindowType = typeof(PhotonEditor);

        protected Vector2 scrollPos = Vector2.zero;

        private readonly Vector2 preferredSize = new Vector2(350, 400);

        private static Texture2D BackgroundImage;

        public static PunWizardText CurrentLang = new PunWizardText();


        protected static AccountService.Origin RegisterOrigin = AccountService.Origin.Pun;

        protected static string DocumentationLocation = "Assets/Photon/PhotonNetworking-Documentation.pdf";

        protected static string UrlFreeLicense = "https://dashboard.photonengine.com/en-US/SelfHosted";

        public const string UrlDevNet = "https://doc.photonengine.com/en-us/pun/v2";

        protected static string UrlForum = "https://forum.photonengine.com";

        protected static string UrlCompare = "https://doc.photonengine.com/en-us/realtime/current/getting-started/onpremise-or-saas";

        protected static string UrlHowToSetup = "https://doc.photonengine.com/en-us/onpremise/current/getting-started/photon-server-in-5min";

        protected static string UrlAppIDExplained = "https://doc.photonengine.com/en-us/realtime/current/getting-started/obtain-your-app-id";

        public const string UrlCloudDashboard = "https://dashboard.photonengine.com/en-US/account/signin?email=";

        public const string UrlPunSettings = "https://doc.photonengine.com/en-us/pun/v2/getting-started/initial-setup"; // the SeverSettings class has this url directly in it's HelpURL attribute.

        private enum PhotonSetupStates
        {
            MainUi,

            RegisterForPhotonCloud,

            EmailAlreadyRegistered,

            GoEditPhotonServerSettings
        }

        private bool isSetupWizard = false;

        private PhotonSetupStates photonSetupState = PhotonSetupStates.RegisterForPhotonCloud;


        private bool minimumInput = false;
        private bool useMail = false;
        private bool useAppId = false;
        private bool useSkip = false;
        private bool highlightedSettings = false;
        private bool close = false;
        private string mailOrAppId = string.Empty;


        private static double lastWarning = 0;
        private static bool postCompileActionsDone;

        // setup once on load
        static PhotonEditor()
        {
            #if UNITY_2017_2_OR_NEWER
            EditorApplication.playModeStateChanged += PlaymodeStateChanged;
            #else
            EditorApplication.playmodeStateChanged += PlaymodeStateChanged;
            #endif

            #if (UNITY_2018 || UNITY_2018_1_OR_NEWER)
            EditorApplication.projectChanged += OnProjectChanged;
            EditorApplication.hierarchyChanged += OnInitialHierarchyChanged;
            #else
            EditorApplication.projectWindowChanged += OnProjectChanged;
            EditorApplication.hierarchyWindowChanged += OnInitialHierarchyChanged;
            #endif

            CompilationPipeline.assemblyCompilationStarted += OnCompileStarted;
        }


        // setup per window
        public PhotonEditor()
        {
            this.minSize = this.preferredSize;
        }

        [MenuItem("Window/Photon Unity Networking/PUN Wizard &p", false, 0)]
        protected static void MenuItemOpenWizard()
        {
            PhotonEditor win = GetWindow(WindowType, false, CurrentLang.WindowTitle, true) as PhotonEditor;
            if (win == null)
            {
                return;
            }
            win.photonSetupState = PhotonSetupStates.MainUi;
            win.isSetupWizard = false;
        }

        [MenuItem("Window/Photon Unity Networking/Highlight Server Settings %#&p", false, 1)]
        protected static void MenuItemHighlightSettings()
        {
            HighlightSettings();
        }


        [DidReloadScripts]
        private static void OnDidReloadScripts()
        {
            //Debug.Log("OnDidReloadScripts()");
            PhotonEditor.UpdateRpcList();
        }

        private static void OnInitialHierarchyChanged()
        {
            #if (UNITY_2018 || UNITY_2018_1_OR_NEWER)
            EditorApplication.hierarchyChanged -= OnInitialHierarchyChanged;
            #else
            EditorApplication.hierarchyWindowChanged -= OnInitialHierarchyChanged;
            #endif

            UpdateRpcList();
        }

        private static void OnCompileStarted(string obj)
        {
            if (PhotonNetwork.IsConnected)
            {
                // log warning, unless there was one recently
                if (EditorApplication.timeSinceStartup - lastWarning > 3)
                {
                    Debug.LogWarning(CurrentLang.WarningPhotonDisconnect);
                    lastWarning = EditorApplication.timeSinceStartup;
                }

                PhotonNetwork.Disconnect();
                PhotonNetwork.NetworkingClient.LoadBalancingPeer.DispatchIncomingCommands();
            }
        }

        // called in editor, opens wizard for initial setup, keeps scene PhotonViews up to date and closes connections when compiling (to avoid issues)
        private static void OnProjectChanged()
        {
            if (PhotonNetwork.PhotonServerSettings == null)
            {
                PhotonNetwork.CreateSettings();

                if (PhotonNetwork.PhotonServerSettings == null)
                {
                    Debug.LogError("CreateSettings() failed to create PhotonServerSettings.");
                    return;
                }
            }


            // serverSetting is null when the file gets deleted. otherwise, the wizard should only run once and only if hosting option is not (yet) set
            if (!PhotonNetwork.PhotonServerSettings.DisableAutoOpenWizard)
            {
                ShowRegistrationWizard();
                PhotonNetwork.PhotonServerSettings.DisableAutoOpenWizard = true;
                PhotonEditor.SaveSettings();
            }
        }

        // called in editor on change of play-mode (used to show a message popup that connection settings are incomplete)
        #if UNITY_2017_2_OR_NEWER
        private static void PlaymodeStateChanged(PlayModeStateChange state)
        #else
        private static void PlaymodeStateChanged()
        #endif
        {
            if (EditorApplication.isPlaying || !EditorApplication.isPlayingOrWillChangePlaymode)
            {
                return;
            }

            if (string.IsNullOrEmpty(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdRealtime) && !PhotonNetwork.PhotonServerSettings.AppSettings.IsMasterServerAddress)
            {
                EditorUtility.DisplayDialog(CurrentLang.SetupWizardWarningTitle, CurrentLang.SetupWizardWarningMessage, CurrentLang.OkButton);
            }
        }


        #region GUI and Wizard


        /// <summary>Creates an Editor window, showing the cloud-registration wizard for Photon (entry point to setup PUN).</summary>
        protected static void ShowRegistrationWizard()
        {
            PhotonEditor win = GetWindow(WindowType, false, CurrentLang.WindowTitle, true) as PhotonEditor;
            if (win == null)
            {
                return;
            }
            win.photonSetupState = PhotonSetupStates.RegisterForPhotonCloud;
            win.isSetupWizard = true;
        }

        // Window Update() callback. On-demand, when Window is open
        protected void Update()
        {
            if (this.close)
            {
                this.Close();
            }
        }

        protected virtual void OnGUI()
        {
            if (BackgroundImage == null)
            {
                string[] paths = AssetDatabase.FindAssets("PunGradient t:Texture2D");
                if (paths != null && paths.Length > 0)
                {
                    BackgroundImage = AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GUIDToAssetPath(paths[0]));
                }
            }

            PhotonSetupStates oldGuiState = this.photonSetupState; // used to fix an annoying Editor input field issue: wont refresh until focus is changed.

            GUI.SetNextControlName(string.Empty);
            this.scrollPos = GUILayout.BeginScrollView(this.scrollPos);


            if (this.photonSetupState == PhotonSetupStates.MainUi)
            {
                this.UiMainWizard();
            }
            else
            {
                this.UiSetupApp();
            }


            GUILayout.EndScrollView();

            if (oldGuiState != this.photonSetupState)
            {
                GUI.FocusControl(string.Empty);
            }
        }


        protected virtual void UiSetupApp()
        {
            GUI.skin.label.wordWrap = true;
            if (!this.isSetupWizard)
            {
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button(CurrentLang.MainMenuButton, GUILayout.ExpandWidth(false)))
                {
                    this.photonSetupState = PhotonSetupStates.MainUi;
                }

                GUILayout.EndHorizontal();
            }


            // setup header
            this.UiTitleBox(CurrentLang.SetupWizardTitle, BackgroundImage);

            // setup info text
            GUI.skin.label.richText = true;
            GUILayout.Label(CurrentLang.SetupWizardInfo);

            // input of appid or mail
            EditorGUILayout.Separator();
            GUILayout.Label(CurrentLang.EmailOrAppIdLabel);
            this.mailOrAppId = EditorGUILayout.TextField(this.mailOrAppId);
            if (!string.IsNullOrEmpty(this.mailOrAppId))
            {
                this.mailOrAppId = this.mailOrAppId.Trim(); // note: we trim all input
                if (PhotonEditor.IsValidEmail(this.mailOrAppId))
                {
                    // this should be a mail address
                    this.minimumInput = true;
                    this.useMail = this.minimumInput;
                    this.useAppId = false;
                }
                else if (ServerSettings.IsAppId(this.mailOrAppId))
                {
                    // this should be an appId
                    this.minimumInput = true;
                    this.useMail = false;
                    this.useAppId = this.minimumInput;
                }
                else
                {
                    this.minimumInput = false;
                    this.useMail = false;
                    this.useAppId = false;
                }
            }
            else
            {
                this.minimumInput = false;
                this.useMail = false;
                this.useAppId = false;
            }

            // button to skip setup
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(CurrentLang.SkipButton, GUILayout.Width(100)))
            {
                this.photonSetupState = PhotonSetupStates.GoEditPhotonServerSettings;
                this.useSkip = true;
                this.useMail = false;
                this.useAppId = false;
            }

            // SETUP button
            EditorGUI.BeginDisabledGroup(!this.minimumInput);
            if (GUILayout.Button(CurrentLang.SetupButton, GUILayout.Width(100)))
            {
                this.useSkip = false;
                GUIUtility.keyboardControl = 0;
                if (this.useMail)
                {
                    this.RegisterWithEmail(this.mailOrAppId); // sets state
                }
                if (this.useAppId)
                {
                    this.photonSetupState = PhotonSetupStates.GoEditPhotonServerSettings;
                    Undo.RecordObject(PhotonNetwork.PhotonServerSettings, "Update PhotonServerSettings for PUN");
                    PhotonNetwork.PhotonServerSettings.UseCloud(this.mailOrAppId);
                    PhotonEditor.SaveSettings();
                }
            }
            EditorGUI.EndDisabledGroup();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();


            // existing account needs to fetch AppId online
            if (this.photonSetupState == PhotonSetupStates.EmailAlreadyRegistered)
            {
                // button to open dashboard and get the AppId
                GUILayout.Space(15);
                GUILayout.Label(CurrentLang.AlreadyRegisteredInfo);


                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button(new GUIContent(CurrentLang.OpenCloudDashboardText, CurrentLang.OpenCloudDashboardTooltip), GUILayout.Width(205)))
                {
                    Application.OpenURL(string.Concat(UrlCloudDashboard, Uri.EscapeUriString(this.mailOrAppId)));
                    this.mailOrAppId = string.Empty;
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }


            if (this.photonSetupState == PhotonSetupStates.GoEditPhotonServerSettings)
            {
                if (!this.highlightedSettings)
                {
                    this.highlightedSettings = true;
                    HighlightSettings();
                }

                GUILayout.Space(15);
                if (this.useSkip)
                {
                    GUILayout.Label(CurrentLang.SkipRegistrationInfo);
                }
                else if (this.useMail)
                {
                    GUILayout.Label(CurrentLang.RegisteredNewAccountInfo);
                }
                else if (this.useAppId)
                {
                    GUILayout.Label(CurrentLang.AppliedToSettingsInfo);
                }


                // setup-complete info
                GUILayout.Space(15);
                GUILayout.Label(CurrentLang.SetupCompleteInfo);


                // close window (done)
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button(CurrentLang.CloseWindowButton, GUILayout.Width(205)))
                {
                    this.close = true;
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }
            GUI.skin.label.richText = false;
        }

        private void UiTitleBox(string title, Texture2D bgIcon)
        {
            GUIStyle bgStyle = new GUIStyle(GUI.skin.GetStyle("Label"));
            bgStyle.normal.background = bgIcon;
            bgStyle.fontSize = 22;
            bgStyle.fontStyle = FontStyle.Bold;

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            Rect scale = GUILayoutUtility.GetLastRect();
            scale.height = 30;

            GUI.Label(scale, title, bgStyle);
            GUILayout.Space(scale.height + 5);
        }

        protected virtual void UiMainWizard()
        {
            GUILayout.Space(15);

            // title
            this.UiTitleBox(CurrentLang.PUNWizardLabel, BackgroundImage);

            // wizard info text
            GUILayout.Label(CurrentLang.WizardMainWindowInfo);
            GUILayout.Space(15);


            // settings button
            GUILayout.BeginHorizontal();
            GUILayout.Label(CurrentLang.SettingsButton, EditorStyles.boldLabel, GUILayout.Width(100));
            GUILayout.BeginVertical();
            if (GUILayout.Button(new GUIContent(CurrentLang.LocateSettingsButton, CurrentLang.SettingsHighlightLabel)))
            {
                HighlightSettings();
            }
            if (GUILayout.Button(new GUIContent(CurrentLang.OpenCloudDashboardText, CurrentLang.OpenCloudDashboardTooltip)))
            {
                Application.OpenURL(UrlCloudDashboard + Uri.EscapeUriString(this.mailOrAppId));
            }
            if (GUILayout.Button(new GUIContent(CurrentLang.SetupButton, CurrentLang.SetupServerCloudLabel)))
            {
                this.photonSetupState = PhotonSetupStates.RegisterForPhotonCloud;
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            GUILayout.Space(15);


            EditorGUILayout.Separator();


            // documentation
            GUILayout.BeginHorizontal();
            GUILayout.Label(CurrentLang.DocumentationLabel, EditorStyles.boldLabel, GUILayout.Width(100));
            GUILayout.BeginVertical();
            if (GUILayout.Button(new GUIContent(CurrentLang.OpenPDFText, CurrentLang.OpenPDFTooltip)))
            {
                EditorUtility.OpenWithDefaultApp(DocumentationLocation);
            }

            if (GUILayout.Button(new GUIContent(CurrentLang.OpenDevNetText, CurrentLang.OpenDevNetTooltip)))
            {
                Application.OpenURL(UrlDevNet);
            }

            GUI.skin.label.wordWrap = true;
            GUILayout.Label(CurrentLang.OwnHostCloudCompareLabel);
            if (GUILayout.Button(CurrentLang.ComparisonPageButton))
            {
                Application.OpenURL(UrlCompare);
            }


            if (GUILayout.Button(new GUIContent(CurrentLang.OpenForumText, CurrentLang.OpenForumTooltip)))
            {
                Application.OpenURL(UrlForum);
            }

            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }

        #endregion


        protected virtual void RegisterWithEmail(string email)
        {
            EditorUtility.DisplayProgressBar(CurrentLang.ConnectionTitle, CurrentLang.ConnectionInfo, 0.5f);

            string accountServiceType = string.Empty;
            if (PhotonEditorUtils.HasVoice)
            {
                accountServiceType = "voice";
            }


            AccountService client = new AccountService();
            client.RegisterByEmail(email, RegisterOrigin, accountServiceType, RegisterWithEmailCallback); // this is the synchronous variant using the static RegisterOrigin. "result" is in the client
        }

        private void RegisterWithEmailCallback(AccountService client)
        {
            EditorUtility.ClearProgressBar();
            if (client.ReturnCode == 0)
            {
                this.mailOrAppId = client.AppId;
                PhotonNetwork.PhotonServerSettings.UseCloud(this.mailOrAppId, null);
                if (PhotonEditorUtils.HasVoice)
                {
                    PhotonNetwork.PhotonServerSettings.AppSettings.AppIdVoice = client.AppId2;
                }
                PhotonEditor.SaveSettings();

                this.photonSetupState = PhotonSetupStates.GoEditPhotonServerSettings;
            }
            else
            {
                PhotonEditor.SaveSettings();

                if (client.Message.Contains("registered"))
                {
                    this.photonSetupState = PhotonSetupStates.EmailAlreadyRegistered;
                }
                else
                {
                    EditorUtility.DisplayDialog(CurrentLang.ErrorTextTitle, client.Message, CurrentLang.OkButton);
                    this.photonSetupState = PhotonSetupStates.RegisterForPhotonCloud;
                }
            }
        }


        // Pings PhotonServerSettings and makes it selected (show in Inspector)
        private static void HighlightSettings()
        {
            ServerSettings serverSettings = (ServerSettings)Resources.Load(PhotonNetwork.ServerSettingsFileName, typeof(ServerSettings));
            Selection.objects = new UnityEngine.Object[] { serverSettings };
            EditorGUIUtility.PingObject(serverSettings);
        }

        // Marks settings object as dirty, so it gets saved.
        // unity 5.3 changes the usecase for SetDirty(). but here we don't modify a scene object! so it's ok to use
        private static void SaveSettings()
        {
            EditorUtility.SetDirty(PhotonNetwork.PhotonServerSettings);
        }

        // https://stackoverflow.com/a/1374644/1449056
        private static bool IsValidEmail(string email)
        {
            if (string.IsNullOrEmpty(email) || !email.Contains("@"))
            {
                return false;
            }
            try
            {
                System.Net.Mail.MailAddress addr = new System.Net.Mail.MailAddress(email);
                return email.Equals(addr.Address);
            }
            catch
            {
                return false;
            }
        }


        #region RPC List Handling


        public static void UpdateRpcList()
        {
            ServerSettings serverSettings = (ServerSettings)Resources.Load(PhotonNetwork.ServerSettingsFileName, typeof(ServerSettings));
            if (serverSettings == null)
            {
                if (!EditorApplication.isUpdating)
                {
                    Debug.LogError("Could not load PhotonServerSettings to update RPCs.");
                }
                return;
            }


            // check all "script assemblies" for methods with PunRPC attribute
            List<string> additionalRpcs = new List<string>();       // not yet listed rpc-method names go here
            List<string> allRpcs = new List<string>();


            #if UNITY_2019_2_OR_NEWER

            // we can make use of the new TypeCache to find methods with PunRPC attribute
            var extractedMethods = TypeCache.GetMethodsWithAttribute<PunRPC>();
            foreach (var methodInfo in extractedMethods)
            {
                allRpcs.Add(methodInfo.Name);
                if (!serverSettings.RpcList.Contains(methodInfo.Name))
                {
                    additionalRpcs.Add(methodInfo.Name);
                }
            }

            #else

            System.Reflection.Assembly[] assemblies;

            #if NET_4_6
            assemblies = System.AppDomain.CurrentDomain.GetAssemblies().Where(a => !a.IsDynamic).ToArray();
            #else
            assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
            #endif

            foreach (var assembly in assemblies)
            {
                if (!assembly.Location.Contains("ScriptAssemblies") || assembly.FullName.StartsWith("Assembly-CSharp-Editor"))
                {
                    continue;
                }

                var types = assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(MonoBehaviour)));
                var methodInfos = types.SelectMany(t => t.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance));
                var methodNames = methodInfos.Where(m => m.IsDefined(typeof(PunRPC), false)).Select(mi => mi.Name).ToArray();
                var additional = methodNames.Where(n => !serverSettings.RpcList.Contains(n));

                allRpcs.AddRange(methodNames);
                additionalRpcs.AddRange(additional);
            }

            #endif


            if (additionalRpcs.Count <= 0)
            {
                //Debug.Log("UpdateRPCs did not found new.");
                return;
            }


            if (additionalRpcs.Count + serverSettings.RpcList.Count >= byte.MaxValue)
            {
                if (allRpcs.Count <= byte.MaxValue)
                {
                    bool clearList = EditorUtility.DisplayDialog(CurrentLang.IncorrectRPCListTitle, CurrentLang.IncorrectRPCListLabel, CurrentLang.RemoveOutdatedRPCsLabel, CurrentLang.CancelButton);
                    if (clearList)
                    {
                        serverSettings.RpcList.Clear();
                        additionalRpcs = allRpcs.Distinct().ToList();   // we add all unique names
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    EditorUtility.DisplayDialog(CurrentLang.FullRPCListTitle, CurrentLang.FullRPCListLabel, CurrentLang.SkipRPCListUpdateLabel);
                    return;
                }
            }


            additionalRpcs.Sort();
            Undo.RecordObject(serverSettings, "RPC-list update of PUN.");
            serverSettings.RpcList.AddRange(additionalRpcs);
            EditorUtility.SetDirty(serverSettings);

            //Debug.Log("Updated RPCs. Added: "+additionalRpcs.Count);
        }


        public static void ClearRpcList()
        {
            bool clearList = EditorUtility.DisplayDialog(CurrentLang.PUNNameReplaceTitle, CurrentLang.PUNNameReplaceLabel, CurrentLang.RPCListCleared, CurrentLang.CancelButton);
            if (clearList)
            {
                ServerSettings serverSettings = (ServerSettings)Resources.Load(PhotonNetwork.ServerSettingsFileName, typeof(ServerSettings));

                Undo.RecordObject(serverSettings, "RPC-list cleared for PUN.");
                serverSettings.RpcList.Clear();
                EditorUtility.SetDirty(serverSettings);

                Debug.LogWarning(CurrentLang.ServerSettingsCleanedWarning);
            }
        }

        #endregion

    }
}