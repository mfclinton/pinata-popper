using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NewgroundsIO.objects;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class NewgroundsHelper : MonoBehaviour
{
    private const string APP_ID= "";
    private const string AES_KEY = "";
    private const int SCOREBOARD_ID = 13598;

    void Awake()
    {
        InitializeNGO();
    }

    void InitializeNGO()
    {
        var options = new Dictionary<string, object>()
        {
            // This should match the version number in your Newgrounds App Settings page
            { "version",            "1.0.0" },
            { "preloadScoreBoards", true },
            { "preloadMedals",      true }
        };
        
        NGIO.Init(APP_ID, AES_KEY, options);
    }

    // Runs once per frame
    void Update()
    {
        /** 
         * Even though we call this on every frame, it will only trigger OnConnectionStatusChanged
         * when there is an actual status change
         **/
        StartCoroutine(NGIO.GetConnectionStatus(OnConnectionStatusChanged));
    }

    public void OnConnectionStatusChanged(string status)
    {
        // You blocked the website hosting this game!
        if (!NGIO.legalHost)
        {

            /**
             * Do something here to inform the player where they can play the game legally.
             * You can have a button that calls NGIO.LoadOfficialUrl(); when clicked.
             */
            return;
        }

        // This copy of the game is out of date
        if (NGIO.isDeprecated)
        {

            /**
             * This is a good place to show a 'New version available' message.
             * Throw in a button that calls NGIO.LoadOfficialUrl(); when clicked.
             */
        }

        // If the user is currently logging in, this will be true.
        if (NGIO.loginPageOpen)
        {

            /**
             * Here you should present the user with a 'please wait' message.
             * You should also show a 'Cancel Login' button, so they aren't
             * stuck on this message if they close their login browser 
             * without actually logging in.
             *
             * Your cancel button should call NGIO.CancelLogin();
             */

            // Here is where we check the actual status of the session.
        }
        else
        {

            switch (status)
            {

                case NGIO.STATUS_CHECKING_LOCAL_VERSION:
                    /**
                     * We're loading the host license and latest version info.
                     * Show a 'please wait' message.
                     */
                    break;

                case NGIO.STATUS_PRELOADING_ITEMS:
                    /**
                     * We're preloading medals, scoreboards, save slots, etc...
                     * Show a 'please wait' message.
                     */
                    break;

                case NGIO.STATUS_LOGIN_REQUIRED:
                    /**
                     * We have a valid session ID, but the player isn't logged in.
                     * Show a 'Log In' button, and a message about how the player
                     * needs to sign in to use certain features.
                     *
                     * The 'Log In' button should call NGIO.OpenLoginPage();
                     *
                     * It is also good practice to provide a 'No Thanks' button
                     * for players who don't want to sign in.
                     *
                     * The 'No Thanks' button should call NGIO.SkipLogin();
                     */
                    break;

                case NGIO.STATUS_READY:

                    /**
                     * The user has either logged in (or declined to do so), and everything else 
                     * has finished preloading.
                     */

                    if (NGIO.hasUser)
                    {
                        /**
                         * The user is signed in!
                         * If they selected the 'remember me' option, their session id will be saved automatically!
                         * 
                         * Show a friendly welcome message! You can get their user name via:
                         *   NGIO.user.name
                         */

                        GetPlayerHighScore();
                    }
                    else
                    {
                        /**
                         * The user doesn't want to sign in and use your cool features.
                         */
                    }

                    /**
                     * You can close any 'please wait' messages now!
                     */

                    break;
            }
        }
    }

    public void PostScore(int score_value)
    {
        if(!NGIO.isReady)
            return;

        StartCoroutine(NGIO.PostScore(SCOREBOARD_ID, score_value, null, OnScorePosted));
    }

    // handler function
    public void OnScorePosted(NewgroundsIO.objects.ScoreBoard board, NewgroundsIO.objects.Score score)
    {
        /**
         * The score is now saved on the server!
         *
         * If you need to refer to the board id use:
         *  board.id
         *
         * If you need to refer to the score value, use:
         *   score.value
         */
    }
    
    public void GetPlayerHighScore()
    {
        if(!NGIO.isReady)
            return;
        
        StartCoroutine(NGIO.GetScores(SCOREBOARD_ID, "A", social: true, callback: OnHighScoreFetched));
    }

    private void OnHighScoreFetched(NewgroundsIO.objects.ScoreBoard board, List<NewgroundsIO.objects.Score> scores, string period, string tag, bool social)
    {
        if (scores == null || scores.Count == 0)
        {
            Debug.Log("No high score data found.");
            return;
        }
        
        Score scoreData = scores.FirstOrDefault(data => data.user.id == NGIO.user.id);
        if (scoreData == null)
        {
            Debug.Log("No high score data found for current user.");
            return;
        }
        
        int highScore = scoreData.value;
        
        GameManager gameManager = FindObjectOfType<GameManager>();
        gameManager.UpdateHighScore(highScore, forceUpdateUI: true);
        
        Debug.Log($"High score found: {highScore}");

    }
}
