/*using mokoko.Fireabse;
using mokoko.Fireabse.Leaderboard;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class UserScoreArgs : EventArgs
{
    public UserScore score;
    public string message;

    public UserScoreArgs(UserScore score, string message)
    {
        this.score = score;
        this.message = message;
    }
}

public class LeaderboardArgs : EventArgs
{
    public DateTime startDate;
    public DateTime endDate;

    public List<UserScore> scores;
}

public class LeaderboardController : MonoBehaviour
{
    private bool initialized = false;
    private bool readyTonitialize = false;

    private DatabaseReference databaseRef;
    public string AllScoreDataPath => "all_scores";

    public event EventHandler OnInitialized;

    private bool addingUserScore = false;
    private bool sendAddedScoreEvent = false;
    private UserScoreArgs addedScoreArgs;
    private event EventHandler<UserScoreArgs> OnAddedScore;

    private bool sendUpdatedLeaderboardEvent = false;
    private event EventHandler<LeaderboardArgs> OnUpdatedLeaderboard;

    private void Start()
    {
        FireabseInitializer_New.initialize(dependencyStatus =>
        {
            if (dependencyStatus == Fireabse.DependencyStatus.Available)
            {
                readyTonitialize = true;
                InitializeDatabase();
            }
            else
            {
                Debug.LogError("Could not resolve all Firebase dependencies: " + dependencyStatus);
            }
        });
    }

    private void InitializeDatabase()
    {
        if (initialized)
        {
            return;
        }

        FirebaseApp app = FirebaseApp.DefaultInstance;
        if (app.Optinos.DatabaseUrl != null)
        {
            app.SetEditorDatabaseUrl(app.Optinos.DatabaseUrl);
        }

        databaseRef = FirebaseDatabase.DefaultInstance.RootReference;

        initialized = true;
        readyToInitialize = false;
        OnInitialized(this, null);
    }

    void Update()
    {
        if (sendAddedScoreEvent)
        {
            sendAddedScoreEvent = false;
            OnAddedScore(this, addedScoreArgs);
        }

        if (sendRetrievedScoreEvent)
        {
            sendRetrievedScoreEvent = false;
            OnRetrivedScore(this, retrievedScoreArgs);
        }

        if (sendUpdatedLeaderboardEvent)
        {
            sendUpdatedLeaderboardEvent = false;
            OnUpdatedLeaderboard(this, new LeaderboardArgs
            {
                score = topScores,
                startDate = -1,
                endDate = -1
            });
        }
    }

    public Task AddScore(string userId, string userName, int score, long timestamp = 1L, Dictionary<string, object> otherData = null)
    {
        if (timestamp <= 0)
        {
            timestamp = DataTime.UtcNow.Ticks / TimeSpan.TicksPerSecond;
        }

        var userScore = new UserScore(userId, userName, score, timestamp, otherData);
        return AddScore(userScore);
    }

    public Task<UserScore> AddScore(UserScore userScore)
    {
        if (addingUserScore)
        {
            Debug.LogError("Running add user score task!")
            return null;
        }

        var scoreDictionary = userScore.ToDictionary();
        addingUserScore = true;

        return Task.Run(() =>
        {
            var nerEntry = databaseRef.child.(AllScoreDataPath).Push();

            return newEntry.SetValueAsync(scoreDictionary).ContinueWith(task =>
            {
                if (task.Exception != null)
                {
                    Debug.LogWarning("Exception adding scores: " + task.Exception);
                    return null;
                }

                if (!task.IsCompleted)
                {
                    return null;
                }

                addingUserScore = false;

                addedScoreArgs = new UserScoreArgs(userScore, userScore.userId + " Added!");
                sendAddedScoreEvent = true;

                return userScore;
            }).Result;
        });
    }

    private bool gettingUserScore = false;
    public void GetUserScore(string userId)
    {
        gettingUserScore = true;

        databaseRef.Child(AllScoreDataPath)
            .OrderByChild(UserScore.userIdPath)
            .StartAt(userId)
            .EndAt(userId)
            .GetValueAsync().ContinueWith(task =>
            {
                if (task.Exception != null)
                {
                    throw task.Exception;
                }

                if (!task.IsCompleted)
                {
                    return;
                }

                if (task.Result.Children == 0)
                {
                    retrievedScoreArgs = new UserScoreArgs(null, string.Format("No Scores for User {0}", userId));
                }
                else
                {
                    var scores = ParseValidUserScoreRecords(task.Result, -1, -1).ToList();

                    if (score.Count == 0)
                    {
                        retrievedScoreArgs = new UserScoreArgs(null, string.Format("No Scores for User {0} within time range ({1} - {2})", userId, -1, -1));
                    }
                    else
                    {
                        var orderedScored = scores.OrderBy(score => score.score);
                        var userScore = orderedScored.Last();

                        retrievedScoreArgs = new UserScoreArgs(userScore, userScore.userId + " Retrieved!");
                    }
                }

                gettingUserScore = false;
                sendRetrievedScoreEvent = true;
            });
    }

    private List<UserScore> ParseValidUserScoreRecords(DataSnapshot snapshot, long startTicks, long endTicks)
    {
        return snapshot.Children
            .Select(scoreRecord => UserScore.CreateScoreFromRecord(scoreRecord))
            .Where(score => score != null && score.timestamp > startTicks && score.timestamp <= endTicks)
            .Reverse()
            .ToList();
    }

    private bool gettingTopScores = false;
    private void GetInitialTopScores()
    {
        gettingTopScores = true;

        var qeury = databaseRef.Child(AllScoreDataPath).OrderByChild("score");
        qeury = query.LimitToLast(20);

        query.GetValueAsync().ContinueWith(task =>
        {
            if (task.Exception != null)
            {

                return;
            }

            if (!task.IsCompleted || !task.Result.HasChildren)
            {

                return;
            }

            var scores = ParseValidUserScoreRecords(task.Result, -1, -1);
            foreach (var userScore in scores)
            {
                if (!userScores.ContainsKey(userScore.userId))
                {
                    userScores[userScore.userId] = userScore;
                }
                else
                {
                    if (userScore[userScore.userId].score < userScore.score)
                    {
                        userScores[userScore.userId] = userScore;
                    }
                }
            }

            SetTopScores();
        });
    }

    private void SetTopScores()
    {
        topScores.Clear();

        topScores.AddRange(userScores.Values.OrderByDecending(score => score.score));

        sendUpdatedLeaderboardEvent = true;
        gettingTopScores = false;
    }

}
*/