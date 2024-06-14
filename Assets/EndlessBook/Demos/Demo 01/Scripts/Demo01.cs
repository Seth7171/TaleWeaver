namespace echo17.EndlessBook.Demo01
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using echo17.EndlessBook;
    using UnityEngine.SceneManagement;
    using TMPro;
    using UnityEngine.UI;

    public class Demo01 : MonoBehaviour
    {
        protected EndlessBook book;
        public float stateAnimationTime = 1f;
        public EndlessBook.PageTurnTimeTypeEnum turnTimeType = EndlessBook.PageTurnTimeTypeEnum.TotalTurnTime;
        public float turnTime = 1f;
        public Button ventureForthButton;
        public TMP_Text feedbackText;

        // Sound-related fields from Demo02
        public AudioSource bookOpenSound;
        public AudioSource bookCloseSound;
        public AudioSource pageTurnSound;
        public AudioSource pagesFlippingSound;
        public float pagesFlippingSoundDelay;
        protected bool audioOn = false;
        protected bool flipping = false;

        // Canvas and button
        public Canvas bookCanvas;
        public GameObject startButton;
        public float canvasEnableDelay = 2f; // Delay before enabling the canvas

        // Current Player that entered the game
        private PlayerData playerData;
        private Player currentPlayer;

        void Awake()
        {
            // Getting player data
            string playerName = PlayerSession.SelectedPlayerName;
            string apiKey = PlayerSession.SelectedPlayerApiKey;

            playerData = DataManager.LoadData();
            currentPlayer = playerData.Players.Find(player => player.PlayerName == playerName && player.ApiKey == apiKey);

            if (currentPlayer == null)
            {
                feedbackText.text = "Player not found.";
                ventureForthButton.interactable = false;
            }

            // Cache the book
            book = GameObject.Find("Book").GetComponent<EndlessBook>();

            // Disable the canvas at start
            if (bookCanvas != null)
            {
                bookCanvas.gameObject.SetActive(false);
            }
        }

        void Update()
        {
            // No keyboard input handling for state changes or page turns
        }

        public void OnStartButtonClicked()
        {
            StartCoroutine(EnableCanvasAfterDelay());

            // Disable the button after it is clicked
            if (startButton != null)
            {
                startButton.SetActive(false);
            }

            // Enable audio after initialization
            audioOn = true;
        }

        private IEnumerator EnableCanvasAfterDelay()
        {
            yield return new WaitForSeconds(canvasEnableDelay);

            if (bookCanvas != null)
            {
                bookCanvas.gameObject.SetActive(true);
            }
        }

        public virtual void OnStateButtonClicked(int buttonIndex)
        {
            book.SetState((EndlessBook.StateEnum)buttonIndex, animationTime: stateAnimationTime, onCompleted: OnBookStateChanged);
        }

        public virtual void OnPageButtonClicked(int pageNumber)
        {
            book.TurnToPage(pageNumber == 999 ? book.LastPageNumber : pageNumber,
                turnTimeType,
                turnTime,
                openTime: stateAnimationTime,
                onCompleted: OnBookTurnToPageCompleted,
                onPageTurnStart: OnPageTurnStart,
                onPageTurnEnd: OnPageTurnEnd
            );
        }

        public virtual void OnTurnButtonClicked(int direction)
        {
            if (direction == -1)
            {
                book.TurnBackward(turnTime,
                    onCompleted: OnBookTurnToPageCompleted,
                    onPageTurnStart: OnPageTurnStart,
                    onPageTurnEnd: OnPageTurnEnd);
            }
            else
            {
                book.TurnForward(turnTime,
                    onCompleted: OnBookTurnToPageCompleted,
                    onPageTurnStart: OnPageTurnStart,
                    onPageTurnEnd: OnPageTurnEnd);
            }
        }

        protected virtual void OnBookStateChanged(EndlessBook.StateEnum fromState, EndlessBook.StateEnum toState, int currentPageNumber)
        {
            Debug.Log("State set to " + toState + ". Current Page Number = " + currentPageNumber);
            PlayStateChangeSound(toState);
        }

        protected virtual void OnBookTurnToPageCompleted(EndlessBook.StateEnum fromState, EndlessBook.StateEnum toState, int currentPageNumber)
        {
            Debug.Log("OnBookTurnToPageCompleted: State set to " + toState + ". Current Page Number = " + currentPageNumber);
        }

        protected virtual void OnPageTurnStart(Page page, int pageNumberFront, int pageNumberBack, int pageNumberFirstVisible, int pageNumberLastVisible, Page.TurnDirectionEnum turnDirection)
        {
            Debug.Log("OnPageTurnStart: front [" + pageNumberFront + "] back [" + pageNumberBack + "] fv [" + pageNumberFirstVisible + "] lv [" + pageNumberLastVisible + "] dir [" + turnDirection + "]");
            if (!flipping)
            {
                pageTurnSound.Play();
            }
        }

        protected virtual void OnPageTurnEnd(Page page, int pageNumberFront, int pageNumberBack, int pageNumberFirstVisible, int pageNumberLastVisible, Page.TurnDirectionEnum turnDirection)
        {
            Debug.Log("OnPageTurnEnd: front [" + pageNumberFront + "] back [" + pageNumberBack + "] fv [" + pageNumberFirstVisible + "] lv [" + pageNumberLastVisible + "] dir [" + turnDirection + "]");
        }

        private void PlayStateChangeSound(EndlessBook.StateEnum state)
        {
            if (!audioOn) return;

            switch (state)
            {
                case EndlessBook.StateEnum.ClosedFront:
                case EndlessBook.StateEnum.ClosedBack:
                    bookCloseSound.Play();
                    break;

                case EndlessBook.StateEnum.OpenFront:
                case EndlessBook.StateEnum.OpenBack:
                case EndlessBook.StateEnum.OpenMiddle:
                    bookOpenSound.Play();
                    break;
            }
        }
        private IEnumerator ClearFeedbackText()
        {
            yield return new WaitForSeconds(3);
            feedbackText.text = "";
        }
    }
}
