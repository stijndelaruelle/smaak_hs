using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class QuizUI : MonoBehaviour
{
    public delegate void StartQuizDelegate(int numOfQuestions);
    public delegate void NewQuizQuestionDelegate(int questionID, QuestionDefinition question);
    public delegate void QuizQuestionAnswered(int numOfQuestionsAsked, int numOfQuestionsCorrect, bool success);

    private QuestionPoolDefinition m_QuestionPool;
    private List<int> m_QuestionIDsRemaining; //All the questions we can still pick from. (makes sure we don't get the same questions twice during a level)
    private int m_CurrentQuestionID = 0;

    private int m_NumberOfQuestions = 0;
    private int m_NumberOfQuestionsAsked = 0;
    private int m_NumberOfQuestionsCorrect = 0;
    private int m_NumberOfQuestionsIncorrect = 0;

    private Stopwatch m_QuestionStopwatch;

    [Header("Required References")]
    [SerializeField]
    private CanvasGroup m_CanvasGroup;

    [Header("SFX")]
    [SerializeField]
    private AudioClip m_StartQuizSFX;

    [SerializeField]
    private AudioClip m_AnswerCorrectSFX;

    [SerializeField]
    private AudioClip m_AnswerIncorrectSFX;

    //Super dirty, but I feel like this is the most "error prone" way of enabling & disabling the quiz menu.
    //We can manually drag all the datablocks in here, but we all know how that's going to end.
    private DatablockPickup[] m_DatablockPickups;
    private DatablockPickup m_CurrentDatablockPickup;

    public event StartQuizDelegate QuizStartEvent;
    public event NewQuizQuestionDelegate QuizNewQuestionEvent;
    public event QuizQuestionAnswered QuizQuestionAnsweredEvent;

    private void Start()
    {
        m_CanvasGroup.Show(false);
        m_QuestionStopwatch = new Stopwatch();

        LevelDirector levelManager = LevelDirector.Instance;
        if (levelManager != null)
        {
            levelManager.LevelEndVictoryEvent += OnReset;
            levelManager.LevelEndDefeatEvent += OnReset;
            levelManager.LevelResetEvent += OnReset;
        }

        //Note: See variable
        m_DatablockPickups = FindObjectsOfType<DatablockPickup>();

        if (m_DatablockPickups != null)
        {
            foreach (DatablockPickup datablock in m_DatablockPickups)
            {
                if (datablock != null)
                    datablock.PickupEvent += OnPickupDatablock;
            }
        }
    }

    private void OnDestroy()
    {
        LevelDirector levelManager = LevelDirector.Instance;
        if (levelManager != null)
        {
            levelManager.LevelEndVictoryEvent -= OnReset;
            levelManager.LevelEndDefeatEvent -= OnReset;
            levelManager.LevelResetEvent -= OnReset;
        }

        if (m_DatablockPickups == null)
            return;

        foreach (DatablockPickup datablock in m_DatablockPickups)
        {
            if (datablock != null)
                datablock.PickupEvent -= OnPickupDatablock;
        }
    }

    private void Update()
    {
        //Bypass
        if (SaveGameManager.GetBool(SaveGameManager.SAVE_CHEATS) == true)
        {
            if (m_CanvasGroup.IsVisible())
            {
                if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftShift))
                {
                    if (Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.A))
                    {
                        m_NumberOfQuestionsCorrect = m_NumberOfQuestions;
                        m_NumberOfQuestionsIncorrect = 0;
                        CompleteQuiz();
                    }
                }
            }
        }
    }

    private void OnPickupDatablock(DatablockPickup datablockPickup, Character character)
    {
        //TODO: fix (dirty)
        if (character.Faction.name != "Player")
        {
            //If the character isn't the player, we complete the quiz!
            datablockPickup.OnQuizCompleted(true);
            return;
        }

        m_CurrentDatablockPickup = datablockPickup;

        if (m_QuestionPool != m_CurrentDatablockPickup.QuestionPool)
        {
            m_QuestionPool = m_CurrentDatablockPickup.QuestionPool;

            //Initialize the questions remaining (this list doesn't reset every quiz, only every level or every time a new pool is loaded)
            FillQuestionsRemaining();
        }

        if (m_QuestionPool == null)
        {
            m_CurrentDatablockPickup.OnQuizCompleted(true);
            return;
        }

        StartQuiz();
    }

    private void StartQuiz()
    {
        if (m_QuestionPool == null)
            return;

        //Disable player input
        LevelDirector.Instance.AddInputBlocker("QuizUI: Start Quiz");

        m_CanvasGroup.Show(true);

        m_NumberOfQuestions = m_QuestionPool.MaxNumberOfQuestions;
        if (m_NumberOfQuestions == 0) { m_NumberOfQuestions = m_QuestionIDsRemaining.Count; }

        m_NumberOfQuestionsAsked = 0;
        m_NumberOfQuestionsCorrect = 0;
        m_NumberOfQuestionsIncorrect = 0;

        //Let the world know
        if (QuizStartEvent != null)
            QuizStartEvent(m_NumberOfQuestions);

        //Start the first question
        bool hasNewQuestion = NewQuestion();

        //No more questions (would be weird), check if we made it!
        if (hasNewQuestion == false)
            CompleteQuiz();

        //Play SFX
        if (AudioPlayer.Instance != null)
            AudioPlayer.Instance.PlaySFXOneShot(m_StartQuizSFX);
    }

    private void CompleteQuiz()
    {
        if (m_QuestionPool == null)
            return;

        //Enable player input
        LevelDirector.Instance.RemoveInputBlocker("QuizUI: Complete Quiz");

        //if we answererd enough questions correct
        bool enoughCorrect = (m_NumberOfQuestionsCorrect >= m_QuestionPool.MinNumerOfCorrectAnswers);
        bool enoughIncorrect = (m_NumberOfQuestionsIncorrect > m_QuestionPool.MaxNumberOfIncorrectAnswers);
        m_CurrentDatablockPickup.OnQuizCompleted((enoughCorrect == true) && (enoughIncorrect == false));

        m_CanvasGroup.Show(false);
    }


    private bool NewQuestion()
    {
        if (m_QuestionPool == null)
            return false;

        m_CurrentQuestionID = -1;

        //Can we ask more questions?
        if (m_NumberOfQuestionsAsked >= m_NumberOfQuestions)
            return false;

        //Should we ask more questions?
        if (m_NumberOfQuestionsCorrect >= m_QuestionPool.MinNumerOfCorrectAnswers)
            return false;

        if (m_NumberOfQuestionsIncorrect > m_QuestionPool.MaxNumberOfIncorrectAnswers)
            return false;

        //if so, check if we have more questions in our pool, if not refill
        if (m_QuestionIDsRemaining.Count == 0)
        {
            FillQuestionsRemaining();
        }

        //Get the new questionID
        if (m_QuestionPool.ShowInOrder)
        {
            m_CurrentQuestionID = m_QuestionIDsRemaining[0];
            m_QuestionIDsRemaining.RemoveAt(0);
        }
        else
        {
            int rand = UnityEngine.Random.Range(0, m_QuestionIDsRemaining.Count);
            m_CurrentQuestionID = m_QuestionIDsRemaining[rand];
            m_QuestionIDsRemaining.RemoveAt(rand);
        }

        if (m_CurrentQuestionID < 0)
        {
            return false;
        }

        //Let everyone know!
        if (QuizNewQuestionEvent != null)
            QuizNewQuestionEvent(m_NumberOfQuestionsAsked, m_QuestionPool.GetQuestion(m_CurrentQuestionID));

        //Start a new timer
        m_QuestionStopwatch.Reset();
        m_QuestionStopwatch.Start();

        return true;
    }

    //Callback from the answer buttons
    public void AnswerQuestion(int answerID)
    {
        if (m_QuestionPool == null)
            return;

        //Check if the answer is correct
        bool correct = (m_QuestionPool.GetQuestion(m_CurrentQuestionID).GetAnswer(answerID).IsCorrect == true);

        if (correct)
            m_NumberOfQuestionsCorrect++;
        else
            m_NumberOfQuestionsIncorrect++;

        //Analytics
        m_QuestionStopwatch.Stop();

        if (LevelDirector.Instance != null)
        {
            AnalyticsManager.QuestionAnswererdEvent(LevelDirector.Instance.LevelData.GetSceneName(),
                                                    m_QuestionPool.GetQuestion(m_CurrentQuestionID).Question,
                                                    m_QuestionPool.GetQuestion(m_CurrentQuestionID).GetAnswer(answerID).Text,
                                                    m_QuestionPool.GetQuestion(m_CurrentQuestionID).NumberOfTimesAnswered(),
                                                    correct,
                                                    m_QuestionStopwatch.Elapsed.TotalSeconds);
        }

        m_QuestionPool.GetQuestion(m_CurrentQuestionID).AnswerQuestion();

        //Let the world know
        if (QuizQuestionAnsweredEvent != null)
            QuizQuestionAnsweredEvent(m_NumberOfQuestionsAsked, m_NumberOfQuestionsCorrect, correct);

        //Play SFX
        if (AudioPlayer.Instance != null)
        {
            if (correct)
            {
                AudioPlayer.Instance.PlaySFXOneShot(m_AnswerCorrectSFX);
            }
            else
            {
                AudioPlayer.Instance.PlaySFXOneShot(m_AnswerIncorrectSFX);
            }
        } 
    }

    //Callback from correct or incorrect window
    public void CompleteQuestion()
    {
        m_NumberOfQuestionsAsked += 1;

        bool hasNewQuestion = NewQuestion();

        //No more questions, check if we made it!
        if (hasNewQuestion == false)
            CompleteQuiz();
    }


    //Utility
    private void FillQuestionsRemaining()
    {
        if (m_QuestionPool == null)
            return;

        if (m_QuestionIDsRemaining == null)
            m_QuestionIDsRemaining = new List<int>();

        m_QuestionIDsRemaining.Clear();

        for (int i = 0; i < m_QuestionPool.GetNumberOfQuestions(); ++i)
        {
            m_QuestionIDsRemaining.Add(i);
        }
    }

    private void OnReset()
    {
        //Enable player input
        if (m_CanvasGroup.IsVisible())
        {
            LevelDirector.Instance.RemoveInputBlocker("QuizUI: Reset");
        }

        FillQuestionsRemaining();
        m_CurrentQuestionID = 0;

        m_CanvasGroup.Show(false);
    }
}
